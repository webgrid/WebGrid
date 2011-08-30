/*
Copyright ©  Olav Christian Botterli.

Dual licensed under the MIT or GPL Version 2 licenses.

Date: 30.08.2011, Norway.

http://www.webgrid.com
*/


#region Header

/*
Copyright ©  Olav Christian Botterli. 

Dual licensed under the MIT or GPL Version 2 licenses.

Date: 30.08.2011, Norway.

http://www.webgrid.com
*/

#endregion Header

namespace WebGrid
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web;
    using System.Web.UI;

    using Data.Database;
    using Design;
    using Enums;
    using Util;

    public partial class Grid
    {
        #region Methods

        /// <summary>
        /// Inserts a client script resource into the Html header of the page rather 
        /// than into the body as RegisterClientScriptInclude does.
        /// 
        /// Scripts references are embedded at the bottom of the Html Header after
        /// any manually added header scripts.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="type"></param>
        /// <param name="resourceName"></param>
        /// <param name="scriptName"></param>
        internal void RegisterClientScriptResourceInHeader(Page page, Type type, string resourceName, string scriptName)
        {
            if (page == null)
                return;

            // Can't do this if there's no header to work with - degrade
            if (page.Header == null || !GotHttpContext)
            {
                page.ClientScript.RegisterClientScriptInclude(scriptName,
                                                              page.ClientScript.GetWebResourceUrl(type.GetType(),
                                                                                                  resourceName).Replace(
                                                                  "&", "&amp;"));
               return;
            }

            // *** Keep duplicates from getting written
            const string identifier = "headerscript_";
            if (HttpContext.Current.Items.Contains(identifier + resourceName))
                return;
            HttpContext.Current.Items.Add(identifier + resourceName, string.Empty);

            object val = HttpContext.Current.Items["__ScriptResourceIndex"];
            int index = 0;
            if (val != null)
                index = (int)val;
            else if (Scripts != null && page.Header.Controls.Count - 1 > Scripts.HeaderStartIndex)
                index = Scripts.HeaderStartIndex;

            string script = page.ClientScript.GetWebResourceUrl(type, resourceName).Replace("&", "&amp;");

            StringBuilder sb = new StringBuilder(300);

            sb.AppendFormat("<script src=\"{0}\" type=\"text/javascript\"></script>\r\n", script);

            if (page.Header == null)
                throw new InvalidOperationException(
                    "Can't add resources to page: missing <head runat=\"server\"> tag in the page.");

            page.Header.Controls.AddAt(index, new LiteralControl(sb.ToString()));

            index++;
            HttpContext.Current.Items["__ScriptResourceIndex"] = index;
        }

        internal void SetupFilterByColumns()
        {
            if (DesignMode || DisplayView != DisplayView.Grid)
                return;

            foreach (Column column in MasterTable.Columns)
            {
                if (!column.FilterByColumn)
                    continue;
                if (MasterTable.DataSourceType != DataSourceControlType.InternalDataSource)
                    throw new GridException(
                        "FilterByColumn property is only supported when the Grid.DataSourceID has reference to a database table.");
                string where = Interface.BuildFilter(MasterTable, true);

                string sowhere = BuildFilterByColumnSql();

                if (string.IsNullOrEmpty(sowhere) == false)
                    where = where.Replace(where, string.Empty);

                switch (column.ColumnType)
                {
                    case ColumnType.Foreignkey:
                        {
                            where = null;

                            if (string.IsNullOrEmpty(((Foreignkey)column).FilterExpression) == false)
                                where = string.Format(" WHERE {0}", ((Foreignkey)column).FilterExpression);
                            if (column.m_Table.m_Grid.Page != null)
                                foreach (Column foreignkeycolumn in ((Foreignkey)column).Table.Columns)
                                {
                                    if (column.m_Table.m_Grid.Page.Session[string.Format("WG{0}", column.ColumnId)] !=
                                        null)
                                        if (!string.IsNullOrEmpty(where))
                                            where +=
                                                string.Format(" AND {0} = '{1}'",
                                                              Interface.BuildTableElement(true,
                                                                                               foreignkeycolumn.
                                                                                                   DataSourceId,
                                                                                               column.ColumnId),

                                                              column.m_Table.m_Grid.Page.Session[
                                                                  string.Format("WG{0}", column.ColumnId)]);
                                        else
                                            where =
                                                string.Format(" WHERE {0} = '{1}'",
                                                              Interface.BuildTableElement(true,
                                                                                               foreignkeycolumn.
                                                                                                   DataSourceId,
                                                                                               column.ColumnId),
                                                              column.m_Table.m_Grid.Page.Session[
                                                                  string.Format("WG{0}", column.ColumnId)]);
                                }
                            string sqlwhere =
                                string.Format(@"SELECT DISTINCT {0} , {1} as fk_text FROM [{2}]{3}", column.ColumnId,
                                              ((Foreignkey)column).ValueColumn, column.DataSourceId, where);

                            if (Debug)
                                m_DebugString.AppendFormat("<b>{0} - FilterByColumn (Foreignkey)</b> - {1}<br/>",
                                                           column.ColumnId, sqlwhere);
                            Query getData =
                                Query.ExecuteReader(sqlwhere, ActiveConnectionString, DatabaseConnectionType);
                            column.FilterByColumnCollection.Clear();
                            if (!string.IsNullOrEmpty(column.NullText))
                                column.SetFilterByColumn(column.NullText, "is null");
                            while (getData.Read())
                            {
                                if (getData[column.ColumnId] == null ||
                                    string.IsNullOrEmpty(getData[column.ColumnId].ToString()))
                                    continue;

                                if (!column.HasDataSourceQuote)
                                    column.SetFilterByColumn(getData["fk_text"].ToString(),
                                                           string.Format("= {0}", getData[column.ColumnId]));
                                else
                                    column.SetFilterByColumn(getData["fk_text"].ToString(),
                                                           string.Format("= '{0}'", getData[column.ColumnId]));
                            }
                            getData.Close();
                        }
                        break;
                    case ColumnType.DateTime:
                        //It is triggered by 'SearchFilter' in WebGrid.DateTime.cs
                        column.SetFilterByColumn("active", "active");
                        break;
                    default:
                        {
                            string datasource = column.DataSourceId;
                            if (string.IsNullOrEmpty(datasource))
                                datasource = DataSourceId;

                            if (string.IsNullOrEmpty(datasource))
                                throw new GridException(
                                    string.Format(
                                        "'{0}' is missing database table reference. You should set 'DataSourceID' in the column settings '{0}'.",
                                        column.ColumnId));
                            string sqlWhere =
                                string.Format(@"SELECT DISTINCT {0} FROM [{1}]{2}", column.ColumnId, datasource, where);
                            if (Debug)
                                m_DebugString.AppendFormat("<b>{0} - FilterByColumn</b> - {1}<br/>", column.ColumnId,
                                                           sqlWhere);
                            Query getData =
                                Query.ExecuteReader(sqlWhere, ActiveConnectionString, DatabaseConnectionType);

                            column.FilterByColumnCollection.Clear();
                            while (getData.Read())
                            {
                                if (getData[column.ColumnId] == null || getData[column.ColumnId] == DBNull.Value)
                                    continue;
                                if (getData[column.ColumnId] is Boolean)
                                {
                                    if ((bool)getData[column.ColumnId])
                                        column.SetFilterByColumn(getData[column.ColumnId].ToString(), "= 1");
                                    else column.SetFilterByColumn(getData[column.ColumnId].ToString(), "= 0");

                                }
                                else if (!column.HasDataSourceQuote)
                                    column.SetFilterByColumn(getData[column.ColumnId].ToString(),
                                                           string.Format("= {0}", getData[column.ColumnId]));
                                else
                                    column.SetFilterByColumn(getData[column.ColumnId].ToString(),
                                                           string.Format("= '{0}'", getData[column.ColumnId]));
                            }
                            getData.Close();
                        }
                        break;
                }
            }
        }

        private void SetupGridMessages()
        {
            if (SystemMessage.CriticalCount > 0 && SystemMessageCritical != null)
            {
                SystemMessage.Clear();
                SystemMessage.Add(SystemMessageCritical, true);
            }

            if (m_IsRecordUpdate && m_SystemMessageUpdate != null)
                SystemMessage.Add(m_SystemMessageUpdate);
            else if (m_IsRecordNew && SystemMessageInsert != null)
                SystemMessage.Add(SystemMessageInsert);
            else if (m_IsRecordDelete && SystemMessageDelete != null)
                SystemMessage.Add(SystemMessageDelete);
            else if (Equals(m_IsRecordUpdateRows, true) && SystemMessageUpdateRows != null)
                SystemMessage.Add(SystemMessageUpdateRows);

            if (Page != null && MailForm != null)
                if (m_IsRecordNew && !string.IsNullOrEmpty(MailForm.EmailAdresses))
                {
                    string email = MailForm.EmailAdresses;
                    if (email.IndexOf("[") > -1)
                    {
                        foreach (Column column in MasterTable.Columns)
                        {
                            string format = String.Format("[{0}]", column.ColumnId);
                            if (email.IndexOf(format, StringComparison.OrdinalIgnoreCase) > -1)
                                email = email.Replace(format, column.ColumnId);
                        }
                    }
                    Mail.SendEmail(email.Split(';'), null, this);
                }

            if (SystemMessage.Count <= 0) return;
            if (SystemMessage.CriticalCount > 0 && Debug == false && SystemMessageCritical == null)
                SystemMessage.Add(
                    "Set debug=\"true\" to retrieve debug information, or replace critical system messages by setting 'SystemMessageCritical' property.<br/>(technical support is available at <a href=\"http://forums.webgrid.com\" target=\"webgrid\">forums.webgrid.com</a>)");
        }

        private void SetupGridRelations()
        {
            // DETECT one-to-one relation
            if (MasterWebGrid == null || MasterWebGrid.DisplayView != DisplayView.Detail ||
                MasterTable.Columns.Primarykeys.Count == 0 ||
                MasterTable.Columns.Primarykeys.Count !=
                MasterWebGrid.MasterTable.Columns.Primarykeys.Count && !m_AllowCancel)
                return;

            bool isOneToOneGrid = true;
            List<Column> masterWebGridPrimaryKeys = MasterWebGrid.MasterTable.Columns.Primarykeys;

            foreach (Column column in MasterTable.Columns.Primarykeys)
            {
                Column column1 = column;
                if (column.Identity || !masterWebGridPrimaryKeys.Exists(
                                            mastercolumn => mastercolumn.ColumnId.Equals(column1.ColumnId)))
                {
                    // one-to-one relation is when there are no identity in table.
                    // and all primarykeys exist in masterwebgrid.
                    isOneToOneGrid = false;

                }
            }
            if (Debug)
                m_DebugString.AppendFormat("<b>{0}: Detected one-to-one relation for grid: {1}</b><br/>", ID,
                                           isOneToOneGrid);

            if (!isOneToOneGrid) return;
            if (m_AllowCancel == false)
            {
                m_IsOneToOneRelationGrid = true;
                return;
            }
            DisplayView = DisplayView.Detail;
            MasterTable.m_GotPostBackData = true;
            MasterTable.m_GotData = false;

            InternalId = MasterWebGrid.InternalId;
            m_IsOneToOneRelationGrid = true;

            if (Trace.IsTracing)
                Trace.Trace("One To One Relation enabled.");
            MasterTable.Columns.Primarykeys.ForEach(delegate(Column column)
                                                        {

                                                            MasterTable.Rows[0][column.ColumnId].Value =
                                                                MasterWebGrid.MasterTable.Rows[0][column.ColumnId].
                                                                    Value;
                                                            MasterTable.Rows[0][column.ColumnId].DataSourceValue =
                                                                MasterWebGrid.MasterTable.Rows[0][column.ColumnId].
                                                                    Value;
                                                        });
        }

        private void SetupGridScripts()
        {
            if ((Page == null || !GotHttpContext) && !DesignMode)
                 return;

            if (Scripts == null || !Scripts.DisableJQuery)
                RegisterClientScriptResourceInHeader(Page, GetType(), "WebGrid.Resources.jquery-1.3.2.min.js", "JQuery");
            if (Scripts == null || !Scripts.DisableJQueryUI)
                RegisterClientScriptResourceInHeader(Page, GetType(), "WebGrid.Resources.jquery-ui-1.7.2.custom.min.js", "JQueryUI");
            if (Scripts == null || !Scripts.DisableBookmark)
                RegisterClientScriptResourceInHeader(Page, GetType(), "WebGrid.Resources.jquery-bookmark.js", "JqueryBookmark");
            if (Scripts == null || !Scripts.DisableContextMenu)
                RegisterClientScriptResourceInHeader(Page, GetType(), "WebGrid.Resources.jquery.contextMenu.js", "contextMenu");
            if (Scripts == null || !Scripts.DisableSelectMenu)
                RegisterClientScriptResourceInHeader(Page, GetType(), "WebGrid.Resources.jquery-ui.selectmenu.js", "contextMenu");

            

            if (Scripts == null || !Scripts.DisableInputMask)
                RegisterClientScriptResourceInHeader(Page, GetType(), "WebGrid.Resources.jquery.meio.mask.min.js", "meiomask");
            if (Scripts == null || !Scripts.DisableClientNotification)
                RegisterClientScriptResourceInHeader(Page, GetType(), "WebGrid.Resources.jquery.jgrowl_minimized.js", "jgrowl");

            if (Scripts == null || !Scripts.DisableToolTip)
                RegisterClientScriptResourceInHeader(Page, GetType(), "WebGrid.Resources.jquery.qtip-1.0.0-rc3.min.js", "qtip");

            if (Scripts == null || !Scripts.DisablePopupExtender)
                RegisterClientScriptResourceInHeader(Page, GetType(), "WebGrid.Resources.jquery.colorbox-min.js", "colorbox");

            RegisterClientScriptResourceInHeader(Page, GetType(), "WebGrid.Resources.WebGrid_ColumnChanged.js", "WebGrid_ColumnChanged");
            RegisterClientScriptResourceInHeader(Page, GetType(), "WebGrid.Resources.WebGrid_Misc.js", "WebGrid_Misc");

            if (Page != null)
            {
                if (EnableCallBack && !Page.ClientScript.IsClientScriptBlockRegistered("wgRegisterAnthem"))
                {
                    Page.ClientScript.RegisterClientScriptBlock(typeof (Page), "wgRegisterAnthem", string.Empty);
                    Page.ClientScript.RegisterHiddenField("WebGrid_EnabledAnthem", "true");

                    if (EnableCallBackErrorAlert)
                    {

                        if (!Debug)
                            Page.ClientScript.RegisterClientScriptBlock(typeof (Page), "wgAnthemError",
                                                                        "function Anthem_Error(result) { alert('Ajax  Error:\\r'+ result.error);}",
                                                                        true);
                        else
                            Page.ClientScript.RegisterClientScriptBlock(typeof (Page), "wgAnthemDebugError",
                                                                        "function Anthem_DebugError(text) { alert('Ajax Debug Error:\\r'+ text);}",
                                                                        true);

                    }
                    if (Scripts == null || !Scripts.DisableAjaxLoader)
                    {
                        Page.ClientScript.RegisterClientScriptBlock(typeof (Page), "wgAnthemPreCallBack",
                                                                    @"function Anthem_PreCallBack() {
                        $(""#"" + wg_gridupdate).find("".wgAjaxLoader:first"").css(""z-index"", ""10000"");
                        $(""#"" + wg_gridupdate).find("".wgAjaxLoader:first"").css(""visibility"", ""visible"");}",
                                                                    true);
                    }
                }

                if (!Page.ClientScript.IsClientScriptBlockRegistered("wgRegisterStyle"))
                {
                    Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "wgRegisterStyle", string.Empty);
                    StringBuilder csslink = new StringBuilder();

                    switch (SkinType)
                    {
                        case SkinType.WebGridJQuery:
                            csslink.Append("<link href=\"");
                            csslink.Append(Page.ClientScript.GetWebResourceUrl(GetType(),"WebGrid.Resources.WebGrid_JQueryUI.css").Replace("&", "&amp;"));
                            csslink.Append("\" rel=\"stylesheet\" type=\"text/css\" />");
                            break;
                        case SkinType.Disabled:
                            break;
                        
                        default:
                            csslink.AppendFormat("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />",
                                                 Page.ClientScript.GetWebResourceUrl(GetType(),
                                                                                     string.Format("WebGrid.Resources.jquery-ui-{0}.css", SkinType.ToString().ToLower()))).
                                Replace("&", "&amp;");
                            csslink.Append("<link href=\"");
                            csslink.Append(Page.ClientScript.GetWebResourceUrl(GetType(),"WebGrid.Resources.WebGrid_JQueryUI.css").Replace("&", "&amp;"));
                            csslink.Append("\" rel=\"stylesheet\" type=\"text/css\" />");
                            break;
                    }

                    //Add
                    if (!Page.IsPostBack)
                        Page.ClientScript.RegisterClientScriptBlock(typeof (Page), "wgHoverButtons", HoverButtonScript,
                                                                    true);
                    if (Page.Header != null && DesignMode == false)
                        Page.Header.Controls.Add(new LiteralControl(csslink.ToString()));
                    else
                        Tag["wgcsslink"] = csslink.ToString();

                    if (Page.Header != null)
                        Page.Header.Controls.Add(new LiteralControl(m_Scriptandstyles.ToString()));
                    else
                        Tag["wgscriptstyles"] = m_Scriptandstyles.ToString();
                }
            }
        }

        #endregion Methods
    }
}