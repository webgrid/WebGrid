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

namespace WebGrid.Design
{
    using System;
    using System.Drawing;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using Collections;
    using Data;
    using Data.Database;
    using Enums;
    using Util;
    using Util.Json;

    /// <summary>
    /// This class contains methods and properties. This class contains code that is used when rendering the 
    /// <see cref="WebGrid.Grid">WebGrid.Grid</see> web control and triggers the <see cref="WebGrid.Grid.Render">WebGrid.Grid.Render</see> event.
    /// </summary>
    internal class Rendering : RenderingMethods
    {
        #region Fields

        private readonly StringBuilder m_Checkboxlist = new StringBuilder(string.Empty);

        #endregion Fields

        #region Methods

        internal static void RenderDetail(ref Grid grid, ref WebGridHtmlWriter writer)
        {
            string width = grid.Width.IsEmpty == false ? grid.Width.ToString() : "100%";

            /* cellpadding and cellspacing are fixed because CSS does not support similar behavior*/
            if (grid.IsUsingJQueryUICSSFramework)
                writer.Write( //ui-helper-reset
                    "<table width=\"{0}\" class=\"ui-widget-content  wgmaindetail\" cellspacing=\"0\" cellpadding=\"0\">", width);
            else
                writer.Write("<table width=\"{0}\" class=\"wgmaindetail\" cellspacing=\"0\" cellpadding=\"0\">", width);

            int[] sortedColumns = grid.VisibleSortedColumns;

            int columnWidth = -1;
            int allRowsColumns = 0;
            int ant = 0;

            bool firstvisiblecolumn = true;

            bool editablecolumns = grid.m_Forceeditablecolumns;

            for (int i = 0; i < sortedColumns.Length; i++)
            {
                ant += 2;
                // if( Grid.BasicDetailLayout )
                //	 ant--;
                if (grid.DisplayRequiredColumn)
                    ant++;

                if (ant > columnWidth)
                    columnWidth = ant;

                Column c = grid.MasterTable.Columns[sortedColumns[i]];
                if (c.UseAllRows)
                    allRowsColumns++;
                if (c.IsVisible == false)
                    continue;
                if (editablecolumns == false && c.AllowEdit)
                    editablecolumns = true;
                if (firstvisiblecolumn)
                {
                    firstvisiblecolumn = false;
                    continue;
                }
                if (c.NewRowInDetail)
                    ant = 0;
            }
            // Create head
            CreateTitle(grid, DisplayView.Detail, writer, sortedColumns, columnWidth,"td");

            if ((grid.CollapseIfSlaveGrid && grid.ActiveMenuSlaveGrid != null) || grid.CollapsedGrid)
            {
                writer.Write("</table>");
                return;
            }
            // Create toolbar
            if (string.IsNullOrEmpty(grid.GetToolbarHtml(ToolbarType.ToolbarDetailTop)) == false)
            {
                if( grid.IsUsingJQueryUICSSFramework)
                writer.Write(
                    "<tr class=\"ui-widget-header wgrow\"><td class=\"wgtoolbardetail\" colspan=\"{0}\">",
                    columnWidth);
                else
                    writer.Write(
                          "<tr class=\"wgrow\"><td class=\"wgtoolbardetail\" colspan=\"{0}\">",
                          columnWidth);
                CreateDetailToolbar(grid.GetToolbarHtml(ToolbarType.ToolbarDetailTop), writer, grid, editablecolumns);
                writer.Write("</td></tr>");
            }
            int antcols = 0;
            if (allRowsColumns > 0)
                writer.Write(
                    "<tr><td colspan=\"{0}\"><table class=\"wgrowspan\"><tr valign=\"top\"><td><table class=\"wgrowspan\">",
                    columnWidth);
            grid.MasterTable.Rows[0].RowIndex = -1;
            for (int i = 0; i < sortedColumns.Length; i++)
            {
                Column column = grid.MasterTable.Rows[0] != null ? grid.MasterTable.Columns[sortedColumns[i]] : grid.MasterTable.Columns[sortedColumns[i]];
                if (column.UseAllRows && i > 0)
                    writer.Write("</table></td><td><table class=\"wgrowspan\">");
                if (i == 0 || (i < sortedColumns.Length && column.NewRowInDetail))
                {
                    antcols = 0;
                    writer.Write("<tr class=\"wgrow\">");
                }
                if (!column.HideDetailTitle)
                {
                    ColumnHeader columnName = new ColumnHeader
                                        {
                                            m_VAlign = column.EditVAlign,
                                            m_Align = column.EditAlignTitle,
                                            m_ColumnWidth = column.WidthColumnTitle,
                                            m_Class = string.Format("{0} wgdetailheader", column.CssClassTitle),
                                            m_GridRowCount = (-1),
                                            m_GridId = grid.ID
                                        };
                    if (columnName.m_ColumnWidth == Unit.Empty)
                    {
                        int columnwidth = column.Title.Length * 12;

                        if (columnwidth > 140)
                            columnwidth = 140;
                        columnName.m_ColumnWidth = Unit.Pixel(columnwidth);
                    }
                    columnName.RenderBeginTag(writer, column, null,"<td");

                    if (string.IsNullOrEmpty(column.ToolTipEditTitle))
                        writer.Write(column.Title);
                    else
                        writer.Write(Tooltip.Add(column.Title, column.ToolTipEditTitle));

                    antcols++;

                    if (grid.BasicDetailLayout == false)
                        columnName.RenderEndTag(writer,"</td>");

                    if (grid.DisplayRequiredColumn)
                    {
                        antcols++;
                        ColumnHeader requiredColumn = new ColumnHeader
                                                {
                                                    m_Class =
                                                        string.Format("wgrequiredcolumn {0}", column.CssClassTitle),
                                                    m_ColumnWidth = Unit.Pixel(1),
                                                    m_VAlign =
                                                        (column.HeightEditableColumn.IsEmpty
                                                             ? column.EditVAlign
                                                             : VerticalPosition.top)
                                                };

                        requiredColumn.RenderBeginTag(writer, column, null,"<td");

                        if (column.Required && column.AllowEdit)
                            writer.Write(@"<span title=""{0}"" class=""ui-icon ui-icon-info""/>",
                                         grid.GetSystemMessage("Required"));
                        else
                            writer.Write("&nbsp;");

                        requiredColumn.RenderEndTag(writer, "</td>");
                    }
                }
                antcols++;
                if ((i == sortedColumns.Length - 1 || (i + 1 < sortedColumns.Length && grid[sortedColumns[i + 1]].NewRowInDetail)) && antcols < columnWidth)
                    grid.Rows[0][column.ColumnId].InternalCellSpan += columnWidth - antcols;

                //   column.Row = grid.MasterTable.Rows[0];
                column.Render(writer, grid.Rows[0][column.ColumnId]);

                if (i == sortedColumns.Length - 1 ||
                    (i + 1 < sortedColumns.Length &&
                     grid[sortedColumns[i + 1]].NewRowInDetail))
                {
                    writer.Write("</tr>");
                }

                if (column.UseAllRows)
                    if (i + 1 < sortedColumns.Length)
                        writer.Write("</table></td><td><table class=\"wgrowspan\">");
                    else
                        writer.Write("</table>");
                else if (allRowsColumns > 0 && i + 1 == sortedColumns.Length)
                    writer.Write("</table>");
            }
            if (allRowsColumns > 0)
                writer.Write("</td></tr></table></td></tr>");

            // Create tail
            if (string.IsNullOrEmpty(grid.GetToolbarHtml(ToolbarType.ToolbarDetailBottom)) == false)
            {
                if(grid.IsUsingJQueryUICSSFramework)
                    writer.Write(
                        "<tr class=\"ui-widget-header wgrow\"><td class=\"wgtoolbardetail\" colspan=\"{0}\">",
                        columnWidth);
                else

                writer.Write(
                    "<tr class=\"wgrow\"><td class=\"wgtoolbardetail\" colspan=\"{0}\">",
                    columnWidth);
                CreateDetailToolbar(grid.GetToolbarHtml(ToolbarType.ToolbarDetailBottom), writer, grid, editablecolumns);
                writer.Write("</td></tr>");
            }

            writer.Write("</table>");
        }

        internal void RenderGrid(ref Grid grid, ref WebGridHtmlWriter writer)
        {
            string width = grid.Width.IsEmpty == false ? grid.Width.ToString() : "100%";
            /* cellpadding and cellspacing are fixed because CSS does not support similar behavior*/
            if (grid.IsUsingJQueryUICSSFramework)
                writer.Write(
                    "<table width=\"{0}\" id=\"{1}\" class=\"ui-widget-content\"  cellpadding=\"0\" cellspacing=\"0\">",
                    width, grid.ID);
            else
                writer.Write(
                    "<table width=\"{0}\" id=\"{1}\" cellpadding=\"0\" cellspacing=\"0\">",
                    width, grid.ID);
            int[] sortedColumns = grid.VisibleSortedColumns;

            if (grid.RecordsPerRow == 1)
            {
                //extra table is to get rid of css background glitch in ie and safari.
                if (grid.IsUsingJQueryUICSSFramework)
                    writer.Write("<thead class=\"ui-widget-header wgthead\">");
                else
                    writer.Write("<thead class=\"wgthead\">");
            }

            CreateTitle(grid, DisplayView.Grid, writer, sortedColumns, 0, "th");

            if ((grid.CollapseIfSlaveGrid && grid.ActiveMenuSlaveGrid != null) || grid.CollapsedGrid)
            {
                if (grid.RecordsPerRow == 1)
                    writer.Write("</thead></table>");
                else
                    writer.Write("</table>");
                return;
            }
            if (grid.MasterTable.Columns.Primarykeys == null)
            {
                grid.AllowDelete = false;
                grid.AllowEdit = false;
                grid.AllowNew = false;
                grid.AllowCopy = false;
            }
            if (string.IsNullOrEmpty(grid.GetToolbarHtml(ToolbarType.ToolbarGridTop)) == false)
            {
                if (grid.IsUsingJQueryUICSSFramework)
                    writer.Write("<tr class=\"ui-widget-header wgrow\"><td colspan=\"{0}\" class=\"wgtoolbargrid\">",
                                 sortedColumns.Length);
                else
                    writer.Write("<tr class=\"wgrow\"><td colspan=\"{0}\" class=\"wgtoolbargrid\">",
                                 sortedColumns.Length);
                CreateGridToolbar(grid.GetToolbarHtml(ToolbarType.ToolbarGridTop), writer, grid);
                writer.Write("</td></tr>");
            }
            // Show Header
            bool FilterByColumns = grid.MasterTable.Columns.Exists(delegate(Column column)
            { return column.FilterByColumnCollection != null && column.FilterByColumnCollection.Count > 0;});

            // Create column filter
            if (FilterByColumns)
                CreateFilterByColumns(grid, writer, sortedColumns);
            // RENDER ROWS

            if (grid.DisplayHeader)
            {
                // CREATE HEADING
                writer.Write("<tr valign=\"bottom\" class=\"wgrow wggridheader\" id=\"{0}r0\">", grid.ID);

                for (int ii = 0; ii < sortedColumns.Length; ii++)
                {
                    Column column = grid.MasterTable.Columns[sortedColumns[ii]];

                    ColumnHeader addcolumn = new ColumnHeader
                                                 {
                                                     m_Class = string.Format("{0} wgheadercell", column.CssClassTitle),
                                                     m_Align = column.GridAlign,
                                                     m_VAlign = VerticalPosition.bottom,
                                                     m_GridRowCount = (-1),
                                                     m_GridId = grid.ID
                                                 };
                    if ((column.ColumnType == ColumnType.SystemColumn &&
                         ((WebGrid.SystemColumn) column).SystemColumnType == SystemColumn.SpacingColumn) ==
                        false)
                    {
                        if (column.WidthColumnHeaderTitle == Unit.Empty)
                        {
                            switch (column.ColumnType)
                            {
                                case ColumnType.SystemColumn:
                                    addcolumn.m_ColumnWidth = Unit.Percentage(1);
                                    break;
                                case ColumnType.Decimal:
                                case ColumnType.Number:
                                    {
                                        int columnwidth = column.Title.Length*12;

                                        if (columnwidth > 200)
                                            columnwidth = 200;

                                        addcolumn.m_ColumnWidth = Unit.Pixel(columnwidth);
                                    }
                                    break;
                            }
                            //                                else
                            //                                    addcolumn.ColumnWidth = Unit.Pixel(100);
                        }
                        else
                            addcolumn.m_ColumnWidth = column.WidthColumnHeaderTitle;
                        grid[column.ColumnId].WidthColumnHeaderTitle = addcolumn.m_ColumnWidth;
                    }
                    addcolumn.RenderBeginTag(writer, column, null, "<th");
                    // writer.Write("<div class=\"wgheadercell\" id=\"{0}h{1}\">", grid.ID, column.ColumnId);

                    if (column.UseAllRows &&
                        (grid.SelectableRows || grid.RowHighLight != Color.Empty))
                    {
                        grid.RowHighLight = Color.Empty;
                        grid.SelectableRows = false;
                    }

                    if (column.IsInDataSource && column.Sortable)
                    {
                        string[] arrayParams = new string[1];
                        if (grid.MasterTable.DataSource != null)
                            arrayParams[0] = column.ColumnId;
                        else /*if (column.ColumnType == ColumnType.Foreignkey || !string.IsNullOrEmpty(grid.Sql))*/
                            arrayParams[0] = Interface.BuildTableElement(true, column.m_Table.DataSourceId,column.ColumnId);
                        /*else
                        {
                            Foreignkey fk = (Foreignkey) column;
                            arrayParams[0] =
                                Foreignkey.BuildDisplayTextSql(
                                    string.Format(" [_pk{0}_{1}].", sortedColumns[ii], fk.DataSourceId),
                                    fk.ValueColumn, string.Empty,
                                    true, fk.Table);
                            if (fk.Table.DataSource != null)
                                arrayParams[0] = fk.ColumnId;
                        }*/
                        string columnHeaderText = column.HeaderText;

                        if (string.IsNullOrEmpty(column.ToolTipGridHeaderText) == false)
                            columnHeaderText = Tooltip.Add(columnHeaderText, column.ToolTipGridHeaderText);



                        StringBuilder hyperlink;
                        if (!grid.AllowSort)
                            hyperlink = new StringBuilder(
                                String.Format("<span  class=\"{1}\">{0}</span>",
                                              columnHeaderText, null));
                        else
                        {

                            string sortimage = string.Empty;
                            // Up/down images
                            if (grid.DisplaySortIcons &&
                                grid.MasterTable.OrderBySortList[arrayParams[0]] != null)
                            {
                                /*StringBuilder img = new StringBuilder("sort-");
                                if (grid.MasterTable.OrderBySortList[arrayParams[0]].m_Desc)
                                    img.Append("down");
                                else
                                    img.Append("up");
                                if (grid.MasterTable.OrderBySortList[arrayParams[0]].m_DisplayIndex != 0)
                                    img.Append("2");
                                img.Append(".gif");

                                string sort =
                                    string.Format("&nbsp;<img style=\" border:0px\" alt=\"{0}\" src=\"{1}{2}\" />",
                                                  grid.GetSystemMessage("SortIcon"), grid.ImagePath, img);*/
                                if (grid.MasterTable.OrderBySortList[arrayParams[0]].m_Desc)
                                    sortimage = string.Format(
                                        @"<span title=""{0}"" style=""float:left""  class=""ui-icon ui-icon-triangle-1-s""/>",
                                        grid.GetSystemMessage("SortIcon"));
                                else
                                    sortimage = string.Format(
                                        @"<span title=""{0}"" style=""float:left"" class=""ui-icon ui-icon-triangle-1-n""/>",
                                        grid.GetSystemMessage("SortIcon"));
                            }
                            hyperlink = new StringBuilder("<table cellspacing=\"0\" cellpadding=\"0\"><tr><td>");
                            hyperlink.Append(new StringBuilder(
                                                 Buttons.Anchor(grid, columnHeaderText,
                                                                "ColumnHeaderClick", arrayParams, null,
                                                                null, null, null, false)));
                            hyperlink.Append("</td><td>");
                            hyperlink.Append(sortimage);
                            hyperlink.Append("</td></tr></table>");
                        }
                        if (grid.NonBreakingHeaders)
                        {
                            hyperlink.Insert(0, "<span class=\"wgnowrap\">");
                            hyperlink.Append("</span>");
                        }
                        writer.Write(hyperlink.ToString());
                    }
                    else
                    {
                        string css = string.Empty;
                        if (grid.NonBreakingHeaders)
                            css += " wgnowrap";
                        writer.Write("<span class=\"{0}\">{1}</span>", css, column.HeaderText);
                    }

                    addcolumn.RenderEndTag(writer, "</th>");
                }
                writer.Write("</tr>");
                // FINISH CREATE HEADING
            }
              if (grid.RecordsPerRow == 1)
                writer.Write("</thead>");

            bool tree = !string.IsNullOrEmpty(grid.TreeParentId);

            writer.Write(grid.BeforeRowsEvent());
            if (grid.Trace.IsTracing) grid.Trace.Trace("Started Rendering Rows ({0})", grid.ID);
            writer.Write("<tbody>");

            CreateRows(grid, tree, null, 0, writer, sortedColumns, 0);

            if (grid.RecordsPerRow == 1)
                CreateTotalSummary(grid, writer, sortedColumns);

            if (grid.Trace.IsTracing) grid.Trace.Trace("Stopped Rendering Rows ({0}).", grid.ID);
            writer.Write(grid.AfterRowsEvent());

            writer.Write("</tbody>");

            if (!string.IsNullOrEmpty(grid.GetToolbarHtml(ToolbarType.ToolbarGridBottom)))
            {
                writer.Write("<tfoot>");
                if (grid.IsUsingJQueryUICSSFramework)
                    writer.Write("<tr class=\"ui-widget-header wgrow\"><td colspan=\"{0}\" class=\"wgtoolbargrid\">",
                                 sortedColumns.Length);
                else
                    writer.Write("<tr class=\"wgrow\"><td colspan=\"{0}\" class=\"wgtoolbargrid\">",
                                 sortedColumns.Length);
                CreateGridToolbar(grid.GetToolbarHtml(ToolbarType.ToolbarGridBottom), writer, grid);
                writer.Write("</td></tr></tfoot>");
            }
            writer.Write("</table>");

            if (grid.Page != null)
                if (grid.EnableCallBack)
                    writer.Write("<input type=\"hidden\" id=\"{0}checkboxes\" name=\"{0}checkboxes\" value=\"{1}\" />",
                                 grid.ClientID, m_Checkboxlist.ToString());
                else
                    grid.Page.ClientScript.RegisterHiddenField(string.Format("{0}checkboxes", grid.ClientID),
                                                               m_Checkboxlist.ToString());
        }

        private static void CreateDetailToolbar(string toolbar, WebGridHtmlWriter writer, Grid grid, bool editablecolumns)
        {
            if (toolbar.IndexOf("[UPDATE]", StringComparison.OrdinalIgnoreCase) > -1)
            {
                if (editablecolumns)
                {
                    string updatemessage = grid.StayInDetail || grid.m_IsGridInsideGrid ? "Update" : "UpdateClose";

                    string status = string.Empty;
                    if (grid.m_IsOneToOneRelationGrid && grid.m_IsOneToOneRelationNewRecord)
                        status = "!new";
                    if (grid.m_IsOneToOneRelationGrid && grid.m_IsOneToOneRelationNewRecord == false)
                        status = "!update";
                    toolbar =
                        toolbar.Replace("[UPDATE]",
                                        (
                                         Buttons.TextButtonControl(grid, grid.GetSystemMessage(updatemessage),
                                                                   "RecordUpdateClick",
                                                                   new[]
                                                                       {
                                                                           string.Format("{0}!{1}{2}", grid.InternalId,
                                                                                         grid.m_IsCopyClick, status)
                                                                       },
                                                                   grid.GetSystemMessageClass(updatemessage,
                                                                                              "wgupdaterecord"), null, grid.ID + "_wgupdaterecord")));
                }
                else
                    toolbar = toolbar.Replace("[UPDATE]", string.Empty);
            }

            if (toolbar.IndexOf("[CLOSEWINDOW]", StringComparison.OrdinalIgnoreCase) > -1)
            {
                if ((grid.MasterWebGrid != null && !grid.m_IsGridInsideGrid) ||
                    (grid.ActiveMenuSlaveGrid == null || (grid.m_HasGridInsideGrid && grid.ActiveMenuSlaveGrid == null)) &&
                    (grid.AllowCancel) || (grid.m_IsGridInsideGrid && grid.AllowCancel))
                {
                    if ((grid.MasterWebGrid != null && grid.m_IsGridInsideGrid == false) && grid.AllowCancel == false)
                        grid.m_IsOneToOneRelationGrid = true;
                    toolbar = toolbar.Replace("[CLOSEWINDOW]",
                                              (
                                               Buttons.TextButtonControl(grid, grid.GetSystemMessage("CloseWindow"),
                                                                         "RecordCancelClick",
                                                                         new[]
                                                                             {
                                                                                 string.Format("{0}!{1}", grid.InternalId,
                                                                                               grid.
                                                                                                   m_IsOneToOneRelationGrid)
                                                                             }
                                                                         ,
                                                                         grid.GetSystemMessageClass("CloseWindow",
                                                                                                    "wgclosewindow"))));
                }
                else
                {
                    toolbar = toolbar.Replace("[CLOSEWINDOW]", string.Empty);
                }
            }
            if (string.IsNullOrEmpty(toolbar) == false)
                writer.Write(toolbar);
        }

        private static void CreateGridToolbar(string toolbar, WebGridHtmlWriter writer, Grid grid)
        {
            if (toolbar.IndexOf("[NEWRECORD]", StringComparison.OrdinalIgnoreCase) > -1)
            {
                if (grid.AllowNew)
                {
                    bool allownew = false;
                    for (int i = 0; i < grid.MasterTable.Columns.Count; i++)
                        if (grid.MasterTable.Columns[i].AllowEdit &&
                            grid.MasterTable.Columns[i].IsInDataSource &&
                            (grid.MasterTable.Columns[i].Visibility == Visibility.Both ||
                             grid.MasterTable.Columns[i].Visibility == Visibility.Detail))
                        {
                            allownew = true;
                            break;
                        }
                    toolbar = allownew
                                  ? toolbar.Replace("[NEWRECORD]", CreateNewPostButton(grid))
                                  : toolbar.Replace("[NEWRECORD]", string.Empty);
                }
                else
                    toolbar = toolbar.Replace("[NEWRECORD]", string.Empty);
            }
            if (toolbar.IndexOf("[CLOSEWINDOW]", StringComparison.OrdinalIgnoreCase) > -1)
            {
                if (grid.m_MasterWebGrid != null && grid.m_MasterWebGrid.DisplaySlaveGridMenu
                    && grid.m_IsGridInsideGrid == false)
                {
                    toolbar =
                        toolbar.Replace("[CLOSEWINDOW]",
                                        (
                                            Buttons.TextButtonControl(grid, grid.GetSystemMessage("CloseWindow"),
                                                                      "RecordCancelClick",
                                                                      new[]
                                                                          {
                                                                              string.Format("{0}!{1}", grid.InternalId,
                                                                                            grid.
                                                                                                m_IsOneToOneRelationGrid)
                                                                          }
                                                                      ,
                                                                      grid.GetSystemMessageClass("CloseWindow",
                                                                                                 "wgclosewindow"))));
                }
                else
                    toolbar = toolbar.Replace("[CLOSEWINDOW]", string.Empty);
            }

            if (toolbar.IndexOf("[UPDATEROWS]", StringComparison.OrdinalIgnoreCase) > -1)
            {
                toolbar = grid.AllowUpdateRows
                              ? toolbar.Replace("[UPDATEROWS]", CreateUpdateRowsButton(grid))
                              : toolbar.Replace("[UPDATEROWS]", string.Empty);
            }
            if (toolbar.IndexOf("[SEARCHFIELD]", StringComparison.OrdinalIgnoreCase) > -1)
            {
                toolbar = grid.DisplaySearchBox != Position.Hide
                              ? toolbar.Replace("[SEARCHFIELD]", CreateSearchField(grid))
                              : toolbar.Replace("[SEARCHFIELD]", string.Empty);
            }
            if (toolbar.IndexOf("[PAGER]", StringComparison.OrdinalIgnoreCase) > -1)
            {
                if (grid.PagerSettings.PagerType != PagerType.None && grid.RecordCount > grid.PageSize)
                {
                    string pager = CreatePager(grid);
                    if (string.IsNullOrEmpty(pager) == false)
                        pager =
                            string.Format(
                                "<table class=\"wgpager\"><tr><td>{0}</td><td>{1}</td></tr></table>",
                                grid.PagerPrefix, pager);
                    toolbar = toolbar.Replace("[PAGER]", pager);
                }
                else
                    toolbar = toolbar.Replace("[PAGER]", string.Empty);
            }
            if (toolbar.IndexOf("[PAGERSTATUS]", StringComparison.OrdinalIgnoreCase) > -1)
                toolbar = PagerType.None != grid.PagerSettings.PagerType && grid.RecordCount > grid.PageSize
                              ? toolbar.Replace("[PAGERSTATUS]", grid.PagerSettings.GetPagerStatus)
                              : toolbar.Replace("[PAGERSTATUS]", string.Empty);

            if (string.IsNullOrEmpty(toolbar) == false)
                writer.Write(toolbar);
        }

        private static void CreateTotalSummary(Grid grid, WebGridHtmlWriter writer, int[] sortedColumnID)
        {
            if (grid.MasterTable.Rows.Count == 0)
                return;
            int firstSumColumn = GetFirstSummary(sortedColumnID, grid);

            if (firstSumColumn == sortedColumnID.Length)
                return;

            string columnId = null;
            foreach (int ii in sortedColumnID)
            {
                Column column = grid.MasterTable.Columns[ii];
                if (column.ColumnType == ColumnType.SystemColumn)
                    continue;
                columnId = column.ColumnId;
                break;
            }
            if (string.IsNullOrEmpty(columnId))
                columnId = grid[firstSumColumn].ColumnId;
            grid.m_RowsAdded++;
            if(!grid.IsUsingJQueryUICSSFramework)
                writer.Write(
                "<tr id=\"{0}r{1}\" class=\"wgrow\"><td colspan=\"{2}\" id=\"{0}{4}r{1}\" class=\"wgtotal\">{3}</td>",
                grid.ID, grid.m_RowsAdded, firstSumColumn, grid.GetSystemMessage("TotalSummary"), columnId);
            else
            {
                writer.Write(
                    "<tr id=\"{0}r{1}\" class=\"ui-state-highlight wgrow\"><td colspan=\"{2}\" id=\"{0}{4}r{1}\" class=\"wgtotal\">{3}</td>",
                    grid.ID, grid.m_RowsAdded, firstSumColumn, grid.GetSystemMessage("TotalSummary"), columnId);

            }
            grid.MasterTable[0].m_RowIndex = grid.m_RowsAdded;
            for (int ii = firstSumColumn; ii < sortedColumnID.Length; ii++)
            {
                grid.MasterTable.Columns[sortedColumnID[ii]].Grid = grid;

                RowCell cell = grid.MasterTable[0][sortedColumnID[ii]];
                Column column = grid.MasterTable.Columns[sortedColumnID[ii]];
                if (column.ColumnType == ColumnType.Decimal && ((WebGrid.Decimal)column).DisplayTotalSummary)
                {
                    column.CssClass = "wgtotalcell";
                    column.AllowEdit = false;
                    if (grid.Trace.IsTracing)
                        grid.Trace.Trace("TotalSummary for column '{0}' is {1}", column.ColumnId,
                                        ((WebGrid.Decimal)column).TotalSum);
                    cell.Value = ((WebGrid.Decimal)column).TotalSum;
                    ((WebGrid.Decimal)column).Sum = null;
                    column.Visibility = Visibility.Grid;
                    column.Render(writer, cell);
                    column.CssClass = null;
                }
                else
                {
                    writer.Write("<td id=\"{0}{1}r{2}\" class=\"wgtotal\">&nbsp;</td>", grid.ID, column.ColumnId,
                                 grid.MasterTable[0].m_RowIndex);
                }
            }

            writer.Write("</tr>");
        }

        private static void EndRow(Row row, WebGridHtmlWriter writer)
        {
            writer.Write("</tr>");
            if (row.PostRowHtml != null)
                if (row.PostRowHtml is string)
                    writer.Write((string) row.PostRowHtml);
                else if (row.PostRowHtml is Control)
                    writer.Write((Control) row.PostRowHtml);
        }

        private static int GetFirstSummary(int[] sortedColumnID, Grid grid)
        {
            int columns = 0;
            for (int ii = 0; ii < sortedColumnID.Length; ii++)
            {
                Column column = grid.MasterTable.Columns[sortedColumnID[ii]];
                if (column.ColumnType == ColumnType.Decimal && ((WebGrid.Decimal)column).DisplayTotalSummary)
                    break;
                columns++;
            }
            return columns;
        }

        private static void StartNewRow(Row row, string style, WebGridHtmlWriter writer)
        {
            Grid grid = row.m_Table.m_Grid;
            StringBuilder js = new StringBuilder(string.Empty);
            string onmouseover = string.Empty;
            string onmouseout = string.Empty;

            if (grid.RowHighLight != Color.Empty)
            {
                row.RowHighLight = grid.RowHighLight;

                if (row.RowHighLight != Color.Empty)
                {
                    onmouseover += string.Format("wgrowhighlight(this, 'over', '{0}');", Grid.ColorToHtml(grid.RowHighLight));
                    onmouseout += "wgrowhighlight(this,'','');";
                }
            }

            #region Client Row Events

            if (!string.IsNullOrEmpty(grid.OnClientRowClick))
            {
                ClientRowEventArgs ea = new ClientRowEventArgs
                                            {
                                                RowIndex = row.RowIndex,
                                                ClientEventType = ClientEventType.OnClientRowClick
                                            };

                string content = JavaScriptConvert.SerializeObject(ea);
                writer.Grid.JsOnData.AppendLine();
                string jsonId = string.Format("{0}r{1}rowclick", grid.ID, row.RowIndex).Replace("-", "A");

                grid.JsOnData.AppendFormat("{0} = {1}", jsonId, content);

                js.AppendFormat(@" onclick=""{0}(this,{1});return false"" ", grid.OnClientRowClick, jsonId);
            }
            if (!string.IsNullOrEmpty(grid.OnClientRowDblClick))
            {
                ClientRowEventArgs ea = new ClientRowEventArgs
                                            {
                                                RowIndex = row.RowIndex,
                                                ClientEventType = ClientEventType.OnClientRowDblClick
                                            };

                string content = JavaScriptConvert.SerializeObject(ea);
                writer.Grid.JsOnData.AppendLine();
                string jsonId = string.Format("{0}r{1}rowdblclick", grid.ID, row.RowIndex).Replace("-", "A");

                grid.JsOnData.AppendFormat("{0} = {1}", jsonId, content);

                js.AppendFormat(@" ondblclick=""{0}(this,{1});return false"" ", grid.OnClientRowDblClick, jsonId);
            }
            if (!string.IsNullOrEmpty(grid.OnClientRowMouseOut))
            {
                ClientRowEventArgs ea = new ClientRowEventArgs
                                            {
                                                RowIndex = row.RowIndex,
                                                ClientEventType = ClientEventType.OnClientRowMouseOut
                                            };

                string content = JavaScriptConvert.SerializeObject(ea);
                writer.Grid.JsOnData.AppendLine();
                string jsonId = string.Format("{0}r{1}rowmouseout", grid.ID, row.RowIndex).Replace("-", "A");

                grid.JsOnData.AppendFormat("{0} = {1}", jsonId, content);

                onmouseout += string.Format(@"{0}(this,{1}); ", grid.OnClientRowMouseOut, jsonId);
            }
            if (!string.IsNullOrEmpty(grid.OnClientRowMouseOver))
            {
                ClientRowEventArgs ea = new ClientRowEventArgs
                                            {
                                                RowIndex = row.RowIndex,
                                                ClientEventType = ClientEventType.OnClientRowMouseOver
                                            };

                string content = JavaScriptConvert.SerializeObject(ea);
                writer.Grid.JsOnData.AppendLine();
                string jsonId = string.Format("{0}r{1}rowmouseout", grid.ID, row.RowIndex).Replace("-", "A");

                grid.JsOnData.AppendFormat("{0} = {1}", jsonId, content);

                onmouseover += string.Format(@"{0}(this,{1}); ", grid.OnClientRowMouseOver, jsonId);

            }

            if (!string.IsNullOrEmpty(onmouseover))
                js.AppendFormat(@" onmouseover=""{0}""", onmouseover);
            if (!string.IsNullOrEmpty(onmouseout))
                js.AppendFormat(@" onmouseout=""{0}""", onmouseout);

            #endregion

            if (row.PreRowHtml != null)
                if (row.PreRowHtml is string)
                    writer.Write((string) row.PreRowHtml);
                else if (row.PreRowHtml is Control)
                    writer.Write((Control) row.PreRowHtml);

            if (grid.RecordsPerRow > 1)
            {
                grid.m_RowsAdded++;

                writer.Write("<tr class=\"wgrow\" id=\"{0}r{1}\" {2} class=\"{3} {4}\">", row.m_Table.m_Grid.ID,
                             grid.m_RowsAdded, js.ToString(), style, row.CssClass);
            }
            else
                writer.Write("<tr class=\"wgrow\" id=\"{0}r{1}\" {2} class=\"{3} {4}\">", row.m_Table.m_Grid.ID,
                             row.RowIndex, js.ToString(), style, row.CssClass);
        }

        private void CreateRows(Grid grid, bool tree, string parentValue, int level, WebGridHtmlWriter writer,
            int[] sortedColumnID, int numberOfRowsDrawn)
        {
            // CREATE ROWS
            int styleRows = 0;
            int rows = 1;
            bool isNewRecordsPerRow = false;
            if (grid.RecordsPerRow > 1 && grid.MasterTable.Rows.Count > 0)
                if (grid.IsUsingJQueryUICSSFramework)
                    writer.Write(
                        "<tr><td width=\"100%\" colspan=\"{0}\"><table class=\"wgrecordsperrow\"><tr class=\"ui-widget\">",
                        sortedColumnID.Length);
                else
                    writer.Write(
                        "<tr><td width=\"100%\" colspan=\"{0}\"><table class=\"wgrecordsperrow\"><tr>",
                        sortedColumnID.Length);

            ColumnCollection columns = grid.MasterTable.Columns;
            for (int i = 0; i < grid.MasterTable.Rows.Count; i++)
            {
                Row currentRow = grid.MasterTable.Rows[i];
                if (!grid.BeforeRowRenderEvent(ref currentRow, ref columns))
                    continue;
                grid.MasterTable.Columns = columns;
                grid.m_RowsAdded++;
                currentRow.RowIndex = grid.m_RowsAdded;
                if (grid.RecordsPerRow > 1)
                {
                    if (isNewRecordsPerRow)
                    {
                        if (grid.IsUsingJQueryUICSSFramework)
                            writer.Write("<tr class=\"ui-widget\" id=\"{0}r{1}\">", grid.ID, grid.m_RowsAdded);
                        else
                            writer.Write("<tr id=\"{0}r{1}\">", grid.ID, grid.m_RowsAdded);
                        isNewRecordsPerRow = false;
                    }
                    if (grid.IsUsingJQueryUICSSFramework)
                        writer.Write(
                            "<td style=\"height: 100%; vertical-align: top\" width=\"{0}%\"><table class=\"wgrecordsperrow\">",
                            (100/grid.RecordsPerRow));
                    else
                        writer.Write(
                            "<td style=\"height: 100%; vertical-align: top\" width=\"{0}%\"><table class=\"wgrecordsperrow\">",
                            (100/grid.RecordsPerRow));
                }

                if (tree)
                {
                    string treeValue = null;

                   if (currentRow[grid.TreeParentId].Value != null)
                        treeValue = currentRow[grid.TreeParentId].Value.ToString();
                    if (treeValue != parentValue || Grid.EMPTYSTRINGCONSTANT.Equals(currentRow.PrimaryKeyValues))
                        continue;
                }

                numberOfRowsDrawn++;

                styleRows++;

                string style = "wgrow";
                if (styleRows > 1)
                    style = "wgalternativerow wgrow";
                if (grid.IsUsingJQueryUICSSFramework)
                    style = "ui-widget " + style;
                if (styleRows >= 2)
                    styleRows = 0;

                // Past this page, no more to draw
                if (tree && grid.PageIndex != 0 && grid.PageSize != 0 &&
                    numberOfRowsDrawn > (grid.PageIndex)*grid.PageSize)
                {
                    if (grid.Trace.IsTracing)
                        grid.Trace.Trace("Did break row writing. Reason: Past this page, no more to draw.");
                    break;
                }
                // For tree, don't draw previous pages, but still run recursive functions.
                if (tree == false || grid.PageIndex == 0 || grid.PageSize == 0 ||
                    numberOfRowsDrawn > (grid.PageIndex - 1)*grid.PageSize)
                {
                    int rowsInThisRecord = 0;
                    for (int ii = 0; ii < sortedColumnID.Length; ii++) // Create cells...
                    {
                        Column column = columns[sortedColumnID[ii]];
                        RowCell rowcell = currentRow[column.ColumnId];

                        if (ii == 0 || column.NewRowInGrid)
                        {
                            if (rowsInThisRecord > 0)
                                writer.Write("</tr>");
                            rowsInThisRecord++;
                            StartNewRow(grid.MasterTable[i], style, writer);
                        }

                        // Shall this column span over all the rows?
                        if (i != 0 && column.UseAllRows && grid.RecordsPerRow == 1)
                            continue;

                        // Automatically set columnSpan if 'NewRowInGrid' property is enabled.

                        if (ii < sortedColumnID.Length - 1 && columns[sortedColumnID[ii + 1]].NewRowInGrid)
                            rowcell.InternalCellSpan = sortedColumnID.Length - ii;

                        column.m_TreeLevel = level;

                        if (columns[sortedColumnID[ii]].UseAllRows)
                            column.Rowspan = grid.RecordsPerRow == 1 ? grid.MasterTable.Rows.Count : 1;

                        column.Render(writer, rowcell);

                        if (column.ColumnType == ColumnType.SystemColumn &&
                            ((WebGrid.SystemColumn)column).SystemColumnType == SystemColumn.SelectColumn)
                            m_Checkboxlist.AppendFormat("{0}!", rowcell.Row.PrimaryKeyValues);

                        if (sortedColumnID.Length - 1 == ii ||
                            (sortedColumnID.Length + 1 < ii &&
                             columns[sortedColumnID[ii + 1]].NewRowInGrid))
                        {
                            EndRow(grid.MasterTable[i], writer);
                        }

                    }
                }

                if (tree)
                    CreateRows(grid, true, currentRow.PrimaryKeyValues, level + 1, writer,
                               sortedColumnID, numberOfRowsDrawn);

                if (grid.RecordsPerRow <= 1) continue;
                writer.Write("</table></td>");

                if ((i + 1) == grid.MasterTable.Rows.Count &&
                    (grid.MasterTable.Rows.Count < grid.RecordsPerRow || rows < grid.RecordsPerRow))
                {
                    while (rows < grid.RecordsPerRow)
                    {
                        writer.Write("<td height=\"100%\" width=\"{0}%\"></td>", (100/grid.RecordsPerRow));
                        rows++;
                    }
                }

                if (rows == grid.RecordsPerRow)
                {
                    writer.Write("</tr>");
                    rows = 0;
                    isNewRecordsPerRow = true;
                }
                rows++;
            }

            if (grid.RecordsPerRow <= 1 || grid.MasterTable.Rows.Count <= 0) return;
            if (rows > 1)
                writer.Write("</tr>");
            writer.Write("</table></td></tr>");
        }

        #endregion Methods
    }
}