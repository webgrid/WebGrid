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
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using Collections;
    using Config;
    using Data.Database;
    using Design;
    using Enums;
    using Util;

    public partial class Grid
    {
        #region Fields

        internal readonly List<string> ClientNotifications = new List<string>();

        internal bool m_BindDataSource;
        internal bool m_ActiveColumnFilter;

        #endregion Fields

        #region Properties

        //   internal object m_Datasourcedotnet;
        //    internal DataTable m_DataTableSettings;
        //  internal string m_XmlDataDocument;
        //   internal DataSet m_XmlDataSet;
        //   internal XmlNodeList m_XmlNodeList;
        //   internal string m_Xmlxpath;
        /// <summary>
        /// Gets or sets the source containing a list of values used to populate the items within the control.
        /// </summary>
        /// <value>The datasource.</value>
        [Browsable(false)]
        public object DataSource
        {
            get { return   MasterTable.DataSource; }
            set
            {

                MasterTable.DataSource = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Finds the control.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startingControl">The starting control.</param>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static T FindControl<T>(Control startingControl, string id)
            where T : Control
        {
            if (startingControl == null || string.IsNullOrEmpty(id))
                return null;
            T found = null;

            foreach (Control activeControl in startingControl.Controls)
            {
                found = activeControl as T;
                if (found == null || (string.Compare(id, activeControl.ID, true) != 0 && activeControl.HasControls()))
                {
                    found = FindControl<T>(activeControl, id);
                }
                else if (string.Compare(id, found.ID, true) != 0)
                    found = null;

                if (found != null)
                {
                    break;
                }
            }

            return found;
        }

        /// <summary>
        /// Finds a WebGrid.Grid object within a Control.
        /// </summary>
        /// <param name="parent">The control to look into</param>
        /// <param name="gridID">The grid to search for</param>
        /// <returns></returns>
        public static Grid FindGrid(Control parent, string gridID)
        {
            return FindControl<Grid>(parent, gridID);
        }

        ///<summary>
        /// The message to add.
        ///</summary>
        ///<param name="message"></param>
        public void AddClientNotificationMessage(string message)
        {
            ClientNotifications.Add(message);
        }

        ///<summary>
        /// The message to add.
        ///</summary>
        ///<param name="message"></param>
        ///<param name="cookieName"></param>
        ///<param name="cookeTimeOut"></param>
        public bool AddClientNotificationMessage(string message, string cookieName, System.DateTime cookeTimeOut)
        {
            if (HttpContext.Current.Request.Cookies[cookieName] != null || string.IsNullOrEmpty(cookieName))
                return false;

            ClientNotifications.Add(message);

            HttpCookie cookie = new HttpCookie(cookieName);
            cookie.Expires = cookeTimeOut;
            HttpContext.Current.Response.Cookies.Add(cookie);
            return true;
        }

        /// <summary>
        /// Cancel your changes and return to grid view. (Applies to detail view)
        /// This function is the same as "Cancel" button in detail view. It uses the same events and same methods.
        /// </summary>
        public void Cancel()
        {
            RecordCancelClickEvent((string.Format("RecordCancelClick!{0}", InternalId)).Split("!"[0]));
        }

        /// <summary>
        ///  Binds WebGrid and all its child controls to the data source.
        /// </summary>
        public override void DataBind()
        {
            base.OnDataBinding(EventArgs.Empty);

            m_BindDataSource = true;
        }

        /// <summary>
        /// Finds controls on the Page by ID string
        /// </summary>
        /// <typeparam name="T">The type to find</typeparam>
        /// <param name="id">The identifer to find</param>
        /// <returns></returns>
        public T FindControl<T>(string id)
            where T : Control
        {
            return FindControl<T>(Page, id);
        }

        /// <summary>
        /// Save your changes to the data source for both updating and new records.
        /// This function applies to both grid and detail view.
        /// </summary>
        public void Save()
        {
            if (Trace.IsTracing)
                Trace.Trace("Start Save({0});", ID);

            if (Page == null || MasterTable == null)
                return;
            m_GetColumnsPostBackData = true;
            switch (DisplayView)
            {
                case DisplayView.Detail:
                    {

                        if (Trace.IsTracing)
                            Trace.Trace("Save(Edit)");
                        string status = string.Empty;
                        SetupGridRelations();
                        if (m_IsOneToOneRelationGrid)
                            status = InternalId == null ? "new" : "update";
                        RecordUpdateClickEvent(new[]
                                                   {
                                                       string.Format("RecordUpdateClick!{0}!{1}!{2}", InternalId, m_IsCopyClick,
                                                                     status)
                                                   });
                    }
                    break;
                case DisplayView.Grid:
                    if (Trace.IsTracing)
                        Trace.Trace("Save(Grid)");
                    UpdateRowsClickEvent();
                    break;
            }
            if (Trace.IsTracing)
                Trace.Trace("Finish Save({0});", ID);
        }

        internal static void AddClientScript(WebGridHtmlWriter writer, string scriptcontent)
        {
            if (Anthem.Manager.IsCallBack)
                Anthem.Manager.AddScriptForClientSideEval(scriptcontent);
            else
                writer.Write(string.Format(@"<script type=""text/javascript"">{0}</script>", scriptcontent));
        }

        internal static void FindEditorControls(ref GridCollection grids, ControlCollection controls)
        {
            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i] is Grid)
                    grids.Add((Grid)controls[i]);
                if (controls[i].Controls.Count > 0)
                    FindEditorControls(ref grids, controls[i].Controls);
            }
        }

        internal void BindDataSourceSession()
        {
            if (HttpContext.Current == null || HttpContext.Current.Session == null)
                return;
            MasterTable.ClearDataSourceSession();

            if ( MasterTable.DataSource != null)
                HttpContext.Current.Session[UniqueID + "_Datasourcedotnet"] =  MasterTable.DataSource;
            if (MasterTable.m_DataSourceColumns != null)
                HttpContext.Current.Session[UniqueID + "_DataTableSettings"] = MasterTable.m_DataSourceColumns;
        }

        internal object GetState(string key)
        {
            return ViewState[key];
        }

        internal ICollection GetStateKeys()
        {
            return ViewState.Keys;
        }

        internal void ProcessSystemColumns()
        {
            bool haveDelete = false;
            bool haveCopy = false;
            bool haveSpacingTag = false;

            for (int i = 0; i < MasterTable.Columns.Count; i++)
            {
                if (MasterTable.Columns[i].ColumnType != ColumnType.SystemColumn)
                    continue;
                if (haveDelete == false &&
                         ((SystemColumn)MasterTable.Columns[i]).SystemColumnType ==
                         Enums.SystemColumn.DeleteColumn)
                    haveDelete = true;
                else if (haveCopy == false &&
                         ((SystemColumn)MasterTable.Columns[i]).SystemColumnType ==
                         Enums.SystemColumn.CopyColumn)
                    haveCopy = true;
                else if (haveSpacingTag == false &&
                         ((SystemColumn)MasterTable.Columns[i]).SystemColumnType ==
                         Enums.SystemColumn.SpacingColumn)
                {
                    haveSpacingTag = true;
                    MasterTable.Columns[i].WidthColumnHeaderTitle = Unit.Empty;
                    MasterTable.Columns[i].Title = string.Empty;
                }
            }

            if (IsDesignTime) return;

            if (haveCopy == false)
            {
                SystemColumn colcopy = new SystemColumn("wgSystemCopy", Enums.SystemColumn.CopyColumn, MasterTable)
                                                   {
                                                       DisplayIndex = 1,
                                                       Title = string.Empty,
                                                       Visibility = Visibility.Grid,
                                                       m_ColumnType = ColumnType.SystemColumn
                                                   };
                MasterTable.Columns.Add(colcopy);
            }

            if (haveDelete == false)
            {
                SystemColumn coldelete = new SystemColumn("wgSystemDelete", Enums.SystemColumn.DeleteColumn, MasterTable)
                                                     {
                                                         DisplayIndex = (MasterTable.Columns.Count * 10) + 10,
                                                         Title = string.Empty,
                                                         Visibility = Visibility.Grid,
                                                         m_ColumnType = ColumnType.SystemColumn
                                                     };
                MasterTable.Columns.Add(coldelete);
            }
            if (Width.Type == UnitType.Percentage || haveSpacingTag) return;
            int prioritysystem = -1;
            int prioritycolumn = -1;

            MasterTable.Columns.ForEach(delegate(Column column)
            {
                if (column.ColumnType == ColumnType.SystemColumn &&
                    column.DisplayIndex > prioritysystem)
                    prioritysystem = column.DisplayIndex;
                else if (column.DisplayIndex == -1 && column.DisplayIndex > prioritycolumn)
                    prioritycolumn = column.DisplayIndex;
            });

            SystemColumn col = new SystemColumn("wgspacingcolumn", Enums.SystemColumn.SpacingColumn, MasterTable)
                                           {
                                               DisplayIndex =
                                                   prioritycolumn > prioritysystem
                                                       ? prioritycolumn + 1
                                                       : prioritysystem - 1,
                                               Title = string.Empty,
                                               Visibility = Visibility.Grid,
                                               m_ColumnType = ColumnType.SystemColumn
                                           };
            MasterTable.Columns.Add(col);
        }

        internal void SetState(string key, object value)
        {
            ViewState[key] = value;
        }

        internal void State(string key, object value)
        {
            if (ViewState != null)
                ViewState[key] = value;
        }

        // 24.03.2005 - jorn : added extra debug-info to foreignkeys.
        internal void UpdateDebugString()
        {
            string debuginfo = null;

            if (m_DebugString != null)
                debuginfo = m_DebugString.ToString();
            m_DebugString = new StringBuilder(string.Format("<br/><table width=\"{0}\" class=\"wgmaingrid\">", Width));
            m_DebugString.Append("<tr><td colspan=\"9\" align=\"left\"><b>WebGrid debug information</b></td></tr>");
            m_DebugString.Append("<tr class=\"wgrow\">");
            m_DebugString.Append("<td><b>ColumnId:</b></td>");
            m_DebugString.Append("<td><b>Default DataSourceId:</b></td>");
            m_DebugString.Append("<td><b>DataSourceId:</b></td>");
            m_DebugString.Append("<td><b>Primarykey:</b></td>");
            m_DebugString.Append("<td><b>ColumnType:</b></td>");
            m_DebugString.Append("<td><b>AllowEdit:</b></td>");
            m_DebugString.Append("<td><b>IsInDataSource:</b></td>");
            m_DebugString.Append("<td><b>DataSourceValue:</b></td>");
            m_DebugString.Append("<td><b>Value:</b></td>");
            m_DebugString.Append("<td><b>DisplayIndex:</b></td>");
            m_DebugString.Append("</tr>");

            for (int i = 0; i < MasterTable.Columns.Count; i++)
            {
                m_DebugString.Append("<tr class=\"wgrow\">");
                m_DebugString.AppendFormat("<td>{0}</td>", MasterTable.Columns[i].ColumnId);
                m_DebugString.AppendFormat("<td>{0}</td>", MasterTable.Columns[i].DefaultDataSourceId);
                m_DebugString.AppendFormat("<td>{0}</td>", MasterTable.Columns[i].DataSourceId);
                m_DebugString.AppendFormat("<td>{0}</td>", MasterTable.Columns[i].Primarykey);
                m_DebugString.AppendFormat("<td>{0}</td>", MasterTable.Columns[i].ColumnType);
                m_DebugString.AppendFormat("<td>{0}</td>", MasterTable.Columns[i].AllowEdit);
                m_DebugString.AppendFormat("<td>{0}</td>", MasterTable.Columns[i].IsInDataSource);
                if (DisplayView == DisplayView.Detail && MasterTable.Rows.Count > 0)
                    m_DebugString.AppendFormat("<td>{0}", MasterTable.Rows[0][i].DataSourceValue);
                else
                    m_DebugString.AppendFormat("<td> N/A");
                if (MasterTable.Columns[i].ColumnType == ColumnType.Foreignkey)
                {
                    if (DisplayView == DisplayView.Grid || MasterTable.Rows.Count == 0)
                        m_DebugString.AppendFormat("<br/>ID:{0}", "N/A");
                    else
                        m_DebugString.AppendFormat("<br/>ID:{0}", MasterTable.Rows[0][i].Value);

                    m_DebugString.AppendFormat("<br/>ValueColumn: {0}",
                                             ((Foreignkey)MasterTable.Columns[i]).ValueColumn);
                    m_DebugString.AppendFormat("<br/>DataSourceID: {0}", MasterTable.Columns[i].DataSourceId);
                }
                m_DebugString.Append("</td>");

                if (DisplayView == DisplayView.Detail && MasterTable.Rows.Count > 0)
                    m_DebugString.AppendFormat("<td>{0}", MasterTable.Rows[0][i].Value);
                else
                    m_DebugString.AppendFormat("<td>{0}", "N/A");
                try
                {
                    if (MasterTable.Columns[i].ColumnType == ColumnType.Foreignkey && DisplayView == DisplayView.Detail && MasterTable.Rows.Count > 0)
                        m_DebugString.AppendFormat("<br/>ID:{0}", MasterTable.Rows[0][i].Value);
                }
                catch (Exception ee)
                {
                    throw new GridException("Error creating debug information for column foreignkey column", ee);
                }
                m_DebugString.Append("</td>");

                m_DebugString.AppendFormat("<td>{0}</td>", MasterTable.Columns[i].DisplayIndex);
                m_DebugString.Append("</tr>");
            }

            m_DebugString.Append("<tr><td colspan=\"9\" align=\"left\"><b>Addition Debug information</b>");
            m_DebugString.Append("</td></tr>");
            m_DebugString.Append("<tr valign=\"top\"><td colspan=\"9\" valign=\"top\" align=\"left\">");
            m_DebugString.AppendFormat("<b>{0}: Grid unique Id: {1}</b> <br/>", ID, base.ClientID);
            m_DebugString.AppendFormat(
                "<b>{0}: CacheGridStructure status is: {1}</b> (Used to cache system messages and data source structures)<br/>",
                ID, CacheGridStructure);
            m_DebugString.AppendFormat("<b>{0}: EnableCallBack (Ajax) status is: {1}</b> <br/>", ID, EnableCallBack);
            m_DebugString.AppendFormat("{0}</td></tr>", debuginfo);
            m_DebugString.Append("</table>");
        }

        private static string FixSelectedValue(string selectedValue)
        {
            if (selectedValue.Equals("between", StringComparison.OrdinalIgnoreCase))
                return selectedValue;
            if (selectedValue.Equals("true", StringComparison.OrdinalIgnoreCase))
                selectedValue = "1";
            else if (selectedValue.Equals("false", StringComparison.OrdinalIgnoreCase))
                selectedValue = "0";

            if (Equals(selectedValue.StartsWith("is", StringComparison.OrdinalIgnoreCase), true) ||
                Equals(selectedValue.StartsWith("=", StringComparison.OrdinalIgnoreCase), true))
                return selectedValue;
            if (selectedValue.StartsWith(">", StringComparison.OrdinalIgnoreCase) ||
                selectedValue.StartsWith("<", StringComparison.OrdinalIgnoreCase))
                return selectedValue;
            if (Equals(Validate.IsFloat(selectedValue), true))
                return String.Format(" = {0}", selectedValue.Replace(",", "."));
            if (Equals(selectedValue.StartsWith("%", StringComparison.OrdinalIgnoreCase), true))
                return String.Format(" LIKE '{0}'", selectedValue);
            if (selectedValue.StartsWith("'%", StringComparison.OrdinalIgnoreCase))
                return String.Format(" LIKE {0}", selectedValue);
            if (selectedValue.StartsWith("'", StringComparison.OrdinalIgnoreCase) == false &&
                selectedValue.EndsWith("'", StringComparison.OrdinalIgnoreCase) == false)
                return String.Format(" = '{0}'", selectedValue);
            return selectedValue;
        }

        /// <summary>
        /// Deeps the search master of WebGrid.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <returns></returns>
        private static bool RecursiveSearchMasterWebGrid(Grid grid)
        {
            if (grid.MasterWebGrid.DisplayView == DisplayView.Detail && Equals(grid.RequiredbyMasterWebGrid(), true))
                return grid.MasterWebGrid.MasterWebGrid == null || RecursiveSearchMasterWebGrid(grid.MasterWebGrid);
            return false;
        }

        private string BuildFilterByColumnSql()
        {
            if (!GotHttpContext)
                return string.Empty;
            StringBuilder filter = new StringBuilder(null);
            int i = 0;

            while (i < MasterTable.Columns.Count)
            {
                Column column = MasterTable.Columns[i];
                i++;
                string selectedValue = null;

                if (column.ColumnType == ColumnType.DateTime)
                    selectedValue = ((DateTime)column).SearchFilter;

                else if (GotHttpContext &&
                         HttpContext.Current.Request.Form[string.Format("ddl{0}", column.ClientHeaderId)] != null)
                {
                    selectedValue =
                        HttpUtility.HtmlEncode(
                            HttpContext.Current.Request.Form[string.Format("ddl{0}", column.ClientHeaderId)]);

                }
                else if (GetState(string.Format("ddl{0}", column.ClientHeaderId)) != null)
                    selectedValue = GetState(string.Format("ddl{0}", column.ClientHeaderId)) as string;
                if (string.IsNullOrEmpty(selectedValue))
                    continue;

                if (filter.Length > 0)
                    filter.Append(" AND ");
                filter.AppendFormat("{0} {1}",
                                               Interface.BuildTableElement(true, MasterTable.DataSourceId,
                                                                                column.ColumnId),
                                               FixSelectedValue(selectedValue));
            }
            m_ActiveColumnFilter = filter.Length > 0;
            return filter.ToString();
        }

        private void DisableShowRequiredIconIfThereIsNoRequiredFields()
        {
            if (DisplayView == DisplayView.Grid || DisplayRequiredColumn == false || DesignMode)
                return;
            if (MasterTable.Rows.Count <= 0 || !DisplayRequiredColumn) return;
            bool status = false;
            for (int i = 0; i < MasterTable.Columns.Count; i++)
            {
                Column column = MasterTable.Columns[i];
                if (!column.Required || !Equals(column.AllowEdit, true) ||
                    (column.Visibility != Visibility.Both && column.Visibility != Visibility.Detail)) continue;
                status = true;
                break;
            }
            DisplayRequiredColumn = status;
        }

        private bool RequiredbyMasterWebGrid()
        {
            return ((!m_IsGridInsideGrid || MasterWebGrid.InternalId != null) && (MasterWebGrid.Visible &&
                ((((!MasterWebGrid.DisplaySlaveGridMenu || MasterWebGrid.ActiveMenuSlaveGrid == this)
                && MasterWebGrid.InternalId != null) && MasterWebGrid.DisplayView == DisplayView.Detail) ||
                m_IsGridInsideGrid))) &&
                (MasterWebGrid.MasterWebGrid == null || RecursiveSearchMasterWebGrid(MasterWebGrid) || m_IsGridInsideGrid);
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeConnectionString()
        {
            if (string.IsNullOrEmpty(ConnectionString) == false &&
                (string.IsNullOrEmpty(GridConfig.Get("WGConnectionString", null as string))
                 || ConnectionString.Equals(GridConfig.Get("WGConnectionString", null as string)) == false))
            {
                return true;
            }
            return Equals(m_ShouldSerializeConnectionString, true) ? true : false;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeDataSource()
        {
            return false;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeDataSourceId()
        {
            return !string.IsNullOrEmpty(MasterTable.DataSourceId) &&
                   MasterTable.DataSourceId.Equals(DATASOURCEID_NULL) == false;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeDatabaseConnectionType()
        {
            return false;
        }

        private void UpdateDebug()
        {
            if (!Debug) return;
            UpdateDebugString();
        }

        #endregion Methods
    }
}