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

using WebGrid.Enums;
using WebGrid.Util;

namespace WebGrid.Design
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Web;
    using System.Web.UI.WebControls;

  

    /// <summary>
    /// This class contains properties and methods that affect the rendering process of the <see cref="WebGrid.Grid">WebGrid.Grid</see>
    /// web control.
    /// </summary>
    internal class RenderingMethods
    {
        #region Methods

        internal static string CreateNewPostButton(Grid grid)
        {
            if (grid.AllowNew)
            {
                foreach (Column column in grid.MasterTable.Columns)
                    if (column.ColumnType == ColumnType.SystemColumn &&
                        ((SystemColumn) column).SystemColumnType == Enums.SystemColumn.NewRecordLink)
                    {
                        if (column.Visibility == Visibility.None)
                            return string.Empty;
                        if (((SystemColumn) column).Html != null)
                            return
                                Buttons.TextButtonControl(grid, ((SystemColumn) column).Html, "NewRecordClick",
                                                          new string[] {},
                                                          grid.GetSystemMessageClass("NewRecord", "wgnewrecord"));
                        break;
                    }

                GridPart customNewRecord = GridPart.GetControl(grid, Enums.GridPart.NewRecord);
                if (customNewRecord != null)
                {
                    StringBuilder builder = new StringBuilder();
                    StringWriter theStringWriter = new StringWriter(builder);
                    System.Web.UI.HtmlTextWriter theHtmlWriter = new System.Web.UI.HtmlTextWriter(theStringWriter);
                    string html = customNewRecord.Html ?? grid.GetSystemMessage("NewRecord");
                    theHtmlWriter.Write(Buttons.TextButtonControl(grid, html, "NewRecordClick", new string[] {},
                                                                  grid.GetSystemMessageClass("NewRecord", "wgnewrecord")));
                    customNewRecord.m_InternalHtml = builder.ToString();
                }
                else
                    return Buttons.TextButtonControl(grid, grid.GetSystemMessage("NewRecord"), "NewRecordClick",
                                                     new string[] {},
                                                     grid.GetSystemMessageClass("NewRecord", "wgnewrecord"));
            }
            return string.Empty;
        }

        internal static string CreatePager(Grid grid)
        {
            Pager pager = grid.PagerSettings;
            if (pager.PagerType == PagerType.None)
                return string.Empty;
            pager.ImagePath = grid.ImagePath;
            pager.m_PagerGrid = grid;

            GridPart customPager = GridPart.GetControl(grid, Enums.GridPart.Pager);
            if (customPager != null)
            {
                customPager.m_InternalHtml = pager.GetPager;
                return null;
            }
            return pager.GetPager;
        }

        internal static string CreateSearchField(Grid grid)
        {
            StringBuilder javascript = new StringBuilder(string.Empty);

            StringBuilder onblur = new StringBuilder(" onblur=\"");
            if (grid.InputHighLight != Color.Empty)
            {
                javascript.AppendFormat(
                    " onfocus=\"this.accessKey = this.style.backgroundColor;this.style.backgroundColor='{0}';\"",
                    Grid.ColorToHtml(grid.InputHighLight));
                onblur.Append("this.style.backgroundColor=this.accessKey;");
            }
            if (grid.ColumnChangedColour != Color.Empty)
            {
                onblur.AppendFormat("isChanged(this,'{0}');", Grid.ColorToHtml(grid.ColumnChangedColour));
            }
            onblur.Append("\"");
            javascript.Append(onblur);

            string s =
                string.Format(
                    "<input {0}  type=\"text\" onkeypress=\"return submitenter(this,event)\" class=\"wgeditfield wgsearchfield\" name=\"{1}_gridsearch\" value=\"{2}\"/>&nbsp;",
                    javascript, grid.ClientID, grid.Search);

            GridPart customSearchField = GridPart.GetControl(grid, Enums.GridPart.SearchField);
            //			editorSystem.Web.HttpContext.Current.Server.ClientScript.RegisterClientScriptBlock(page.GetType(),"WGSearchBox", @"

            if (customSearchField != null)
            {
                StringBuilder builder = new StringBuilder();
                StringWriter theStringWriter = new StringWriter(builder);
                System.Web.UI.HtmlTextWriter theHtmlWriter = new System.Web.UI.HtmlTextWriter(theStringWriter);
                if (customSearchField.Html != null)
                {
                    Buttons.TextButtonControl(grid, customSearchField.Html, "SearchClick", new string[] {},
                                              grid.GetSystemMessageClass("Search", "wgsearch"));
                    customSearchField.m_InternalHtml = s + builder;
                }
                else
                {
                    theHtmlWriter.Write(Buttons.TextButtonControl(grid, grid.GetSystemMessage("Search"), "SearchClick", new string[] { },
                                              grid.GetSystemMessageClass("Search", "wgsearch")));
                    customSearchField.m_InternalHtml = s + builder;
                }
            }
            else
            {
                return
                    String.Format(
                        "<table cellspacing=\"0\" cellpadding=\"0\"><tr valign=\"middle\"><td>{0}</td><td>{1}</td></tr></table>",
                        s,

                         Buttons.TextButtonControl(grid, grid.GetSystemMessage("Search"), "SearchClick", new string[] {},
                                                   grid.GetSystemMessageClass("Search", "wgsearch")));
            }
            return string.Empty;
        }

        internal static void CreateFilterByColumns(Grid grid, WebGridHtmlWriter writer, int[] sortedColumnID)
        {
            grid.m_RowsAdded++;
            writer.Write(
                grid.IsUsingJQueryUICSSFramework
                    ? "<tr class=\"ui-widget wgrow\" id=\"{0}r{1}\">"
                    : "<tr class=\"wgrow\" id=\"{0}r{1}\">", grid.ID, grid.m_RowsAdded);
            for (int ii = 0; ii < sortedColumnID.Length; ii++)
            {
                Column column = grid.MasterTable.Columns[sortedColumnID[ii]];
                if (column.FilterByColumnCollection.Count == 0)
                {
                    string columnstyle = string.Empty;
                    string columnId = string.Format(" id=\"{0}{1}r{2}\"", grid.ID, column.ColumnId, grid.m_RowsAdded);
                    if (grid.MasterTable.Columns[column.ColumnId].WidthColumnHeaderTitle != Unit.Empty)
                        columnstyle =
                            string.Format("style=\"width: {0};\"",
                                          grid.MasterTable.Columns[column.ColumnId].WidthColumnHeaderTitle);
                    writer.Write("<th {0} {1}>&nbsp;</th>", columnstyle,columnId);
                    continue;
                }

                ColumnHeader addcolumn = new ColumnHeader
                                   {
                                       m_Class = string.Format("{0} wggridcell", column.CssClassTitle),
                                       m_Align = column.GridAlign,
                                       m_VAlign = VerticalPosition.bottom,
                                       m_ColumnWidth =
                                           grid.MasterTable.Columns[column.ColumnId].WidthColumnHeaderTitle,
                                       m_GridRowCount = grid.m_RowsAdded,

                                       m_GridId = grid.ID
                                   };

                addcolumn.RenderBeginTag(writer,column,null,"<th");

                string selectedValue = null;
                if (Grid.GotHttpContext &&
                    HttpContext.Current.Request.Form[string.Format("ddl{0}", column.ClientHeaderId)] != null)
                {
                    selectedValue =
                        HttpUtility.HtmlEncode(
                            HttpContext.Current.Request.Form[string.Format("ddl{0}", column.ClientHeaderId)]);
                    grid.State(string.Format("ddl{0}", column.ClientHeaderId), selectedValue);
                }
                else if (grid.GetState(string.Format("ddl{0}", column.ClientHeaderId)) != null)
                    selectedValue = grid.GetState(string.Format("ddl{0}", column.ClientHeaderId)) as string;
                bool isdate = false;
                if (grid.MasterTable.Columns[column.ColumnId].ColumnType == ColumnType.DateTime)
                    isdate = true;

                string eventScript = string.Empty;
                if (grid.Page != null)
                {
                    string link = grid.EnableCallBack && !column.ForcePostBack ? Asynchronous.GetCallbackEventReference(grid,
                                                                                               string.Format("ElementPostBack!ddl{0}",
                                                                                                             column.ColumnId), false,
                                                                                               string.Empty, string.Empty) : grid.Page.ClientScript.GetPostBackEventReference(grid,
                                                                                                                                                                              string.Format("ElementPostBack!ddl{0}",
                                                                                                                                                                                            column.ColumnId));
                    eventScript = string.Format(" onchange=\"{0}\" ", link);
                }
                string style = grid.MasterTable.Columns[column.ColumnId].WidthColumnHeaderTitle != Unit.Empty ? "class=\"wgeditfield wgselectbox \"" : " class=\"wgeditfield wgselectbox \" ";

                if (!isdate)
                {
                    StringBuilder dropdownbox = new StringBuilder("<select ");
                    dropdownbox.AppendFormat("{0} name=\"ddl{1}\" id=\"ddl{1}\" {2}><option value=\"\" selected=\"selected\">{3}</option>", style, column.ClientHeaderId, eventScript, column.Grid.GetSystemMessage("EmptySearchFilter"));

                    if (grid.Scripts == null || !grid.Scripts.DisableSelectMenu)
                        Grid.AddClientScript(writer, string.Format("$(document).ready(function() {{$('#ddl{0}').selectmenu({{maxHeight: {2},style:'dropdown',width: {1}}});}});", column.ClientHeaderId, column.WidthColumnHeaderTitle.Value, 400));
          
                    
                    int i = 0;
                    while (i < column.FilterByColumnCollection.Count)
                    {
                        string _value = HttpUtility.HtmlEncode(column.FilterByColumnCollection[i]);
                        string _title = HttpUtility.HtmlEncode(column.FilterByColumnCollection.GetKey(i));
                        if (selectedValue != null && selectedValue == _value)
                            dropdownbox.AppendFormat("<option selected=\"selected\" value=\"{0}\">{1}</option>",
                                                     HttpUtility.HtmlEncode(column.FilterByColumnCollection[i]), _title);
                        else
                            dropdownbox.AppendFormat("<option value=\"{0}\">{1}</option>",
                                                     HttpUtility.HtmlEncode(column.FilterByColumnCollection[i]), _title);
                        i++;
                    }

                    writer.Write(dropdownbox);
                    writer.Write("</select> ");
                }
                else
                {
                    if (grid.DatabaseConnectionType != DataBaseConnectionType.SqlConnection)
                        throw new GridException(
                            string.Format("'FilterByColumn' property for 'DateTime' ({0}) columns are only supported by 'SqlConnection' connection strings.", column.ColumnId));
                    writer.Write("<span style=\"white-space: nowrap;vertical-align:middle;\">");

                    string filter = ((WebGrid.DateTime)grid.MasterTable.Columns[column.ColumnId]).SearchFilter;

                    if (string.IsNullOrEmpty(filter))
                    {
                        StringBuilder s = new StringBuilder(string.Empty);
                        s.AppendFormat(
                            "<a class=\"wglinkfield\" href=\"?DateFilter={0}\">{1}</a>",
                             grid.MasterTable.Columns[column.ColumnId].ColumnId,
                            column.Grid.GetSystemMessage("DateTimeSearch"));
                        writer.Write(s);
                    }
                    else
                    {
                        StringBuilder s = new StringBuilder(string.Empty);
                        s.AppendFormat(
                            "<a class=\"wglinkfield\" href=\"#\"   onclick=\"javascript:WGdisableDateTimeSearch('{0}');\";return false;\">{1}</a>",
                            column.ColumnId,column.Grid.GetSystemMessage("DateTimeSearchReset"));
                        writer.Write(s);
                    }
                    // From date
                    writer.Write("</span>");
                }
                addcolumn.RenderEndTag(writer,"</th>");
            }

            writer.Write("</tr>");
        }

        internal static void CreateTitle(Grid grid, DisplayView mode, WebGridHtmlWriter writer, int[] sortedColumnID,
            int editMaxCells, string tagstyle)
        {
            string strclass = "wgtitlegrid";
            if (mode == DisplayView.Detail)
                strclass = "wgtitledetail";

            bool isHeader = GetIsHeader(grid);

            if (!isHeader) return;
            if (grid.IsUsingJQueryUICSSFramework && grid.DisplayView == DisplayView.Detail)
                writer.Write("<tr class=\"ui-widget-header wgrow\">");
            else
                writer.Write("<tr class=\"wgrow\">");

            ColumnHeader header = new ColumnHeader
                                      {
                                          m_ColumnSpan =(mode == DisplayView.Grid ? sortedColumnID.Length : editMaxCells),
                                          m_Class = string.Format("{0} wgheadercell", strclass)
                                      };

            header.RenderBeginTag(writer, null, null, string.Format("<{0}", tagstyle));
            writer.Write(grid.Title);
            header.RenderEndTag(writer, string.Format("</{0}>", tagstyle));
            writer.Write("</tr>");
        }

        internal static string CreateUpdateRowsButton(Grid grid)
        {
            foreach (Column column in grid.MasterTable.Columns)
            {
                if (column.Visibility != Visibility.Grid)
                    continue;
                if (column.ColumnType != ColumnType.SystemColumn ||
                    ((SystemColumn) column).SystemColumnType != Enums.SystemColumn.UpdateRecordsLink) continue;
                string html = ((SystemColumn) column).Html;

                if (string.IsNullOrEmpty(html))
                    continue;
                return Buttons.TextButtonControl(grid, html, "UpdateRowsClick", new string[] {},
                                                 grid.GetSystemMessageClass("NewRecord", "wgnewrecord"));
            }

            GridPart customUpdateRecords = GridPart.GetControl(grid, Enums.GridPart.UpdateRecords);

            if (customUpdateRecords != null)
            {
                StringBuilder builder = new StringBuilder();
                StringWriter theStringWriter = new StringWriter(builder);
                System.Web.UI.HtmlTextWriter theHtmlWriter = new System.Web.UI.HtmlTextWriter(theStringWriter);
                string html = customUpdateRecords.Html ?? grid.GetSystemMessage("UpdateRows");
                theHtmlWriter.Write(Buttons.TextButtonControl(grid, html, "UpdateRowsClick", new string[] { },
                                          grid.GetSystemMessageClass("UpdateRows", "wgupdaterecords"),null,string.Format("{0}_wgupdaterecords", grid.ID))
                    );

                customUpdateRecords.m_InternalHtml = builder.ToString();
                return null;
            }
            return Buttons.TextButtonControl(grid, grid.GetSystemMessage("UpdateRows"), "UpdateRowsClick",
                                             new string[] {},
                                             grid.GetSystemMessageClass("UpdateRows", "wgupdaterecords"), null, string.Format("{0}_wgupdaterecords", grid.ID));
        }
        /*
        /// <summary>
        /// Sorts the columns.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <returns>The sorted columns.</returns>
        internal static int[] GetSortedColumns(Grid grid)
        {
            if (grid.Trace.IsTracing) grid.Trace.Trace("Start SortColumns();");

            int ant = 0;

            for (int i = 0; i < grid.MasterTable.Columns.Count; i++)
            {
                Column column = grid.MasterTable.Columns[i];
                if (column.IsVisible == false)
                    continue;

                // Check if all rows are empty and then we don't draw.
                if (grid.DisplayView == DisplayView.Grid && column.HideIfEmpty)
                {
                    bool hidden = true;
                    for (int num = 0; num < grid.MasterTable.Rows.Count; num++)
                    {

                        if (column.ColumnType == ColumnType.SystemColumn
                            && ((SystemColumn)column).ShouldRender(grid.MasterTable.Rows[num]))
                        {
                            hidden = false;
                            break;
                        }
                        if (grid.MasterTable.Rows[num].Cells.GetIndex(column.ColumnId) == -1 || grid.MasterTable.Rows[num][column.ColumnId].Value == null)
                            continue;
                        hidden = false;
                        break;
                    }
                    if (hidden)
                        continue;
                }

                ant++;
            }

            int[] sortedColumnID = new int[ant];

            int tmpi = 0;
            for (int i = 0; i < grid.MasterTable.Columns.Count; i++)
            {
                Column column = grid.MasterTable.Columns[i];
                if (column.IsVisible == false)
                    continue;

                // Check if all rows are empty and then we don't draw.
                if (grid.DisplayView == DisplayView.Grid && column.HideIfEmpty)
                {
                    bool hidden = true;
                    for (int num = 0; num < grid.MasterTable.Rows.Count; num++)
                    {
                        if (column.ColumnType == ColumnType.SystemColumn
                            && ((SystemColumn)column).ShouldRender(grid.MasterTable.Rows[num]))
                        {
                            hidden = false;
                            break;
                        }
                        if (grid.MasterTable.Rows[num].Cells.GetIndex(column.ColumnId) == -1 || grid.MasterTable.Rows[num][column.ColumnId].Value == null)
                            continue;
                        hidden = false;
                        break;
                    }
                    if (hidden)
                    {
                        if (grid.MasterTable.Columns[i].AllowEditInGrid)
                        {
                            string columnName = grid.MasterTable.Columns[i].ColumnId;
                            for (int ii = 0; ii < grid.MasterTable.Rows.Count; ii++)
                                grid.MasterTable.Columns[columnName].AllowEditInGrid = false;
                        }
                        continue;
                    }
                }

                sortedColumnID[tmpi] = i;
                tmpi++;
            }

            for (int i = 0; i < sortedColumnID.Length; i++) // Sort
                for (int ii = 0; ii < sortedColumnID.Length; ii++)
                    if (grid.MasterTable.Columns[sortedColumnID[i]].DisplayIndex <
                        grid.MasterTable.Columns[sortedColumnID[ii]].DisplayIndex)
                    {
                        int tmp = sortedColumnID[i];
                        sortedColumnID[i] = sortedColumnID[ii];
                        sortedColumnID[ii] = tmp;
                    }

            if (grid.Trace.IsTracing) grid.Trace.Trace("Finish SortColumns();");
            return sortedColumnID;
        }
        */
        private static bool GetIsHeader(Grid grid)
        {
            bool isHeader = false; // Is there any heading stuff at all??
            isHeader |= !String.IsNullOrEmpty(grid.Title);
            isHeader &= grid.DisplayTitle;
            return isHeader;
        }

        #endregion Methods
    }
}