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
    using System.ComponentModel;
    using System.IO;
    using System.Text;

    using WebGrid.Data;
    using WebGrid.Design;
    using WebGrid.Enums;
    using WebGrid.Util;
    using WebGrid.Util.MultiSelect;

    /// <summary>
    /// The SystemColumn class is displayed as a column with various layout in the web interface.
    /// The SystemColumns are columns used by WebGrid. The most common columns are "edit record" and "delete record" columns.
    /// An Enumeration list is available at <see cref="WebGrid.Enums.SystemColumn">WebGrid.Enums.SystemColumn</see>.
    /// This class inherits from the <see cref="WebGrid.Column">WebGrid.Column</see> class.
    /// </summary>
    [DesignTimeVisible(true),
    Browsable(true),
    ]
    public class SystemColumn : Column
    {
        #region Fields

        internal Enums.SystemColumn m_SystemColumnType = Enums.SystemColumn.Undefined;

        private string m_Html;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WebGrid.SystemColumn">SystemColumn</see> class.
        /// </summary>
        public SystemColumn()
        {
            IsInDataSource = false;
            GridAlign = HorizontalPosition.Left;
            SystemColumnType = Enums.SystemColumn.Undefined;
            m_ColumnType = ColumnType.SystemColumn;
            Searchable = false;
            Sortable = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebGrid.SystemColumn">SystemColumn</see> class.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="columnType">The type of the column.</param>
        /// <param name="table">The table object.</param>
        public SystemColumn(string columnName, Enums.SystemColumn columnType, Table table)
            : base(columnName, table)
        {
            Grid = table.m_Grid;
            m_Table = table;
            ColumnId = columnName;
            IsInDataSource = false;
            GridAlign = HorizontalPosition.Left;
            Searchable = false;
            Sortable = false;
            SystemColumnType = columnType;
            m_ColumnType = ColumnType.SystemColumn;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Sets or gets the html for this system column.
        /// If html is null then standard WebGrid icons are used.
        /// </summary>
        /// <value>The HTML.</value>
        [Browsable(true),
        Category("System column options"),
        Description(
             @"Sets or gets the html for this system column. If html is null then standard WebGrid icons are used."
             )]
        public string Html
        {
            get { return m_Html; }
            set { m_Html = value; }
        }

        /// <summary>
        /// Sets or gets what system column should be displayed.
        /// </summary>
        /// <value>The system-column type.</value>
        [Browsable(true),
        Category("System column type"),
        Description(@"Sets or gets what system column should be displayed.")]
        public Enums.SystemColumn SystemColumnType
        {
            get { return m_SystemColumnType; }
            set
            {
                m_SystemColumnType = value;

                switch (value)
                {
                    case Enums.SystemColumn.DeleteColumn:
                        GridAlign = HorizontalPosition.Left;
                        break;
                    case Enums.SystemColumn.CopyColumn:
                        GridAlign = HorizontalPosition.Left;
                        break;
                    case Enums.SystemColumn.PagerColumn:
                        UseAllRows = true;
                        break;
                }
            }
        }

        #endregion Properties

        #region Methods

        internal override Column CopyTo(Column column)
        {
            if (column.ColumnType == ColumnType.SystemColumn)
            {
                SystemColumn sc = (SystemColumn)base.CopyTo(column);
                sc.m_Html = m_Html;
                sc.m_SystemColumnType = m_SystemColumnType;
                return sc;
            }
            return base.CopyTo(column);
        }

        internal override Column CreateColumn()
        {
            return new SystemColumn(ColumnId, Enums.SystemColumn.Undefined, m_Table);
        }

        internal override Column Duplicate()
        {
            SystemColumn c = new SystemColumn(ColumnId, SystemColumnType, m_Table)
                                 {
                                     m_Html = Html,
                                     m_SystemColumnType = m_SystemColumnType
                                 };
            CopyTo(c);
            return c;
        }

        internal override void RenderEditView(WebGridHtmlWriter writer, RowCell cell)
        {
            RenderLabelView(writer, cell);
        }

        internal override void RenderLabelView(WebGridHtmlWriter writer, RowCell cell)
        {
            if (cell == null)
                return;
            switch (SystemColumnType)
            {
                case Enums.SystemColumn.PagerColumn:
                    if (Grid.RecordCount > Grid.PageSize)
                        return;
                    cell.Row.m_Table.m_Grid.PagerSettings.SliderOperation = System.Web.UI.WebControls.Orientation.Vertical;
                    writer.Write(cell.Row.m_Table.m_Grid.PagerSettings.GetPager);
                    cell.Row.m_Table.m_Grid.PagerSettings.PagerType = PagerType.None;
                    break;
                case Enums.SystemColumn.SelectColumn:
                    writer.Write(
                       SelectableRows.GetItemCheckbox(Grid, cell.Row.PrimaryKeyValues, string.Empty,
                                                      Grid.ColorToHtml(Grid.SelectRowColor)));
                    break;

                case Enums.SystemColumn.DeleteColumn:
                    if (cell.Row.AllowDelete == false)
                    {
                        Visibility = Visibility.None;
                        Title = null;
                    }
                    else
                    {
                        string strConfirmDelete = Grid.GetSystemMessage("ConfirmDelete");
                        if (Html == null)
                            writer.Write(Buttons.Anchor(Grid, " ", "RecordDeleteClick",
                                                        new[] { cell.Row.PrimaryKeyValues }, strConfirmDelete,
                                                        Grid.GetSystemMessage("DeleteRow"), "ui-icon ui-icon-trash",
                                                        null, false));

                        else
                            writer.Write(
                                Buttons.TextButtonControl(Grid, Html, "RecordDeleteClick",
                                                          new[] { cell.Row.PrimaryKeyValues }, null,
                                                          strConfirmDelete));
                    }
                    break;
                case Enums.SystemColumn.SpacingColumn:
                    WidthColumnHeaderTitle = System.Web.UI.WebControls.Unit.Empty;
                    writer.Write("&nbsp;");
                    break;
                case Enums.SystemColumn.CopyColumn:
                    if (cell.Row.AllowCopy == false)
                    {
                        Visibility = Visibility.None;
                        Title = null;
                    }
                    else
                    {
                        const string strConfirmCopy = null;
                        if (Html == null)
                            writer.Write(

                                Buttons.Anchor(Grid, " ", "RecordCopyClick",
                                               new[] { string.Empty, cell.Row.PrimaryKeyValues }, strConfirmCopy,
                                               Grid.GetSystemMessage("CopyRow"), "ui-icon ui-icon-copy",
                                               null, false));

                        else
                            writer.Write(
                                Buttons.TextButtonControl(Grid, Html, "RecordCopyClick",
                                                          new[] { string.Empty, cell.Row.PrimaryKeyValues }, null,
                                                          strConfirmCopy));
                    }
                    break;
                case Enums.SystemColumn.NewRecordColumn:
                    if (Grid.AllowNew == false)
                    {
                        Visibility = Visibility.None;
                        Title = null;
                    }
                    else
                    {
                        if (Html == null)
                            writer.Write(
                                Buttons.TextButtonControl(Grid, Grid.GetSystemMessage("NewRecord"), "NewRecordClick",
                                                          new string[] { },
                                                          Grid.GetSystemMessageClass("NewRecord", "wgnewrecord")));
                        else
                            writer.Write(
                                Buttons.TextButtonControl(Grid, Html, "NewRecordClick", new string[] { },
                                                          Grid.GetSystemMessageClass("NewRecord", "wgnewrecord")));
                    }
                    break;
                case Enums.SystemColumn.UpdateGridRecordsColumn:
                    if (Grid.AllowUpdate == false)
                    {
                        Visibility = Visibility.None;
                        Title = null;
                    }
                    else
                    {
                        if (Html == null)
                            writer.Write(
                                Buttons.TextButtonControl(Grid, Grid.GetSystemMessage("UpdateRows"), "UpdateRowsClick",
                                                          new string[] { },
                                                          Grid.GetSystemMessageClass("UpdateRows", "wgUpdateRows")));
                        else
                            writer.Write(
                                Buttons.TextButtonControl(Grid, Html, "UpdateRowsClick", new string[] { },
                                                          Grid.GetSystemMessageClass("UpdateRows", "wgUpdateRows")));
                    }
                    break;
                case Enums.SystemColumn.UpdateGridRecordColumn:
                    if (Grid.AllowUpdate == false)
                    {
                        Visibility = Visibility.None;
                        Title = null;
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        StringWriter sw = new StringWriter(sb);
                        System.Web.UI.HtmlTextWriter textwriter = new System.Web.UI.HtmlTextWriter(sw);

                        if (Html == null)
                            textwriter.Write(Buttons.TextButtonControl(Grid, Grid.GetSystemMessage("UpdateRow"), "UpdateRowClick",
                                                                       new[] { cell.Row.PrimaryKeyValues },
                                                                       Grid.GetSystemMessageClass("UpdateRow", "wgUpdateRow")));
                        else
                            textwriter.Write(Buttons.TextButtonControl(Grid, Html, "UpdateRowClick",
                                                                       new[] { cell.Row.PrimaryKeyValues },
                                                                       Grid.GetSystemMessageClass("UpdateRow", "wgUpdateRow")));

                        writer.Write(sb);
                    }
                    break;
            }
            // base.RenderLabelView(writer,cell);
            RenderGrid(null, writer, cell);
        }

        internal bool ShouldRender(Row row)
        {
            switch (SystemColumnType)
            {
                case Enums.SystemColumn.DeleteColumn:
                    if (row.AllowDelete == false)
                        return false;
                    break;
                case Enums.SystemColumn.CopyColumn:
                    if (row.AllowCopy == false)
                        return false;
                    break;
                case Enums.SystemColumn.UpdateGridRecordsColumn:
                    if (Grid.AllowUpdate == false)
                        return false;
                    break;
            }
            return true;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeSystemColumnType()
        {
            return m_SystemColumnType != Enums.SystemColumn.Undefined;
        }

        #endregion Methods
    }
}