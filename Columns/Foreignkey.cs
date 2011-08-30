/*
Copyright ©  Olav Christian Botterli.

Dual licensed under the MIT or GPL Version 2 licenses.

E-mail: olav@com, olav@botterli.com (Olav Botterli)

http://www.com
*/


#region Header

/*
Copyright ©  Olav Christian Botterli. 

Dual licensed under the MIT or GPL Version 2 licenses.

E-mail: olav@com, olav@botterli.com (Olav Botterli)

http://www.com
*/

#endregion Header

namespace WebGrid
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Text;
    using System.Web;

    using Collections;
    using Data;
    using Design;
    using HtmlTextWriter = Design.WebGridHtmlWriter;
    using Enums;
    using Events;
    using Util;

    /// <summary>
    /// The foreign key class is displayed as a column with a dropdown box by default (radio buttons is also available).
    /// This class inherits from the <see cref="Column">Column</see> class.
    /// </summary>
    public class Foreignkey : Column
    {
        #region Fields

        internal string m_ConnectionString;
        internal bool m_ConnectionStringSet;
        internal Table m_ForeignkeyTable;

        //public	string	ValueText;
        internal ForeignkeyType m_Foreignkeytype = ForeignkeyType.Select;
        internal int m_SelectMenuWidth;
        internal int m_SelectMenuMaxHeight = 400;
        
        internal string m_ValueColumn;

        private bool m_EnableOnlyTreeLeaf;
        private string m_FilterExpression = string.Empty;
        private int m_RecordsPerRow = 1;
        private bool m_RememberState;
        private int m_Rowcounter;
        private string m_SortExpression = string.Empty;
        private string m_TreeParentId;
        private string m_TreeStructureSeperator;
        private string m_identityColumn;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Foreignkey">Foreignkey</see> class.
        /// </summary>
        public Foreignkey()
        {
            Searchable = true;
            m_SaveValueToViewState = true;
            m_ColumnType = ColumnType.Foreignkey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Foreignkey">Foreignkey</see> class.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="table">The table object.</param>
        public Foreignkey(string columnName, Table table)
            : base(columnName, table)
        {
            Grid = table.m_Grid;
            m_Table = table;
            Searchable = true;
            ColumnId = columnName;
            m_SaveValueToViewState = true;
            m_ColumnType = ColumnType.Foreignkey;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// A standard OleDB connection string for Access and SQLClient connection for MS SQL.
        /// Example how a connection string looks:
        /// "Data Source='DataSorce';Initial Catalog='Databasename';User ColumnId='userID';Password='password'";
        /// Web.Config key for this property is "WG" + property name.
        /// </summary>
        /// <value>The connection string for the foreign key column.</value>
        [Browsable(true),
        Category("Foreign key options"),
        SettingsBindable(true),
        Description(@"A standard OleDB database connection string.")]
        public string ConnectionString
        {
            get
            {
                return m_ConnectionString ?? (Grid != null ? Grid.ConnectionString : null);
            }
            set
            {
                if (Grid != null && value != Grid.ActiveConnectionString && value != Grid.ConnectionString)
                {
                    if (Grid.ActiveConnectionString.IndexOf("|DataDirectory|", StringComparison.OrdinalIgnoreCase) == -1)
                    {
                        m_ConnectionStringSet = true;
                        Sortable = false;
                    }
                }
                m_ConnectionString = value;
            }
        }

        /// <summary>
        /// Sets or gets a filter for the data source.
        /// "WHERE" should not be written. Only the statement.
        /// If your data source contains more than one table, the 
        /// where statement might require you to include both table name and table field.
        /// Example: [Customers].[CustomerId]
        /// Where Customers is the table name and CustomerId is the table field.
        /// </summary>
        [Browsable(true),
        Category("Foreign key options"),
        DefaultValue(""),
        Description(
             @"Sets or gets a filter for the data source. ""WHERE"" should not be written. Only the statement.")]
        public string FilterExpression
        {
            get { return m_FilterExpression; }
            set
            {
                // DataSource for the foreign key is already loaded.
                m_FilterExpression = value;
                if (m_ForeignkeyTable == null)
                    return;
                m_ForeignkeyTable.FilterExpression = m_FilterExpression;
            }
        }

        /// <summary>
        /// Sets or gets the width of JQuery UI Select Menu.
        /// </summary>
        /// <value>The foreign key type.</value>
        [Browsable(true),
        Category("Foreign key options"),
        DefaultValue(0),
        Description(@"Sets or gets the width of JQuery UI Select Menu.")]
        public int SelectMenuWidth
        {
            get { return m_SelectMenuWidth; }
            set { m_SelectMenuWidth = value; }
        }
        /// <summary>
        /// Sets or gets the max height of JQuery UI Select Menu.
        /// </summary>
        /// <value>The foreign key type.</value>
        [Browsable(true),
        Category("Foreign key options"),
        DefaultValue(400),
        Description(@"Sets or gets the max height of JQuery UI Select Menu.")]
        public int SelectMenuMaxHeight
        {
            get { return m_SelectMenuMaxHeight; }
            set { m_SelectMenuMaxHeight = value; }
        }

        /// <summary>
        /// Sets or gets how the foreign key column should be rendered at the web page.
        /// </summary>
        /// <value>The foreign key type.</value>
        [Browsable(true),
        Category("Foreign key options"),
        DefaultValue(typeof(ForeignkeyType), "Select"),
        Description(@"Sets or gets how the foreign key column should be rendered at the web page.")]
        public ForeignkeyType ForeignkeyType
        {
            get { return m_Foreignkeytype; }
            set { m_Foreignkeytype = value; }
        }

        /// <summary>
        /// Sets or gets the Identity column for the foreign key.
        /// </summary>
        /// <value>The primary key column.</value>
        [Browsable(true),
        Category("Foreign key options"),
        ParenthesizePropertyName(true),
        Description(@"Sets or gets the Identity column for the foreign key.")]
        public string IdentityColumn
        {
            get { return m_identityColumn; }
            set
            {
                m_identityColumn = value;

            }
        }

        /// <summary>
        /// Sets or gets how many records we should display per row on the column. Example by setting RecordsPerRow = 4 it will render 4 database records per row in your column.
        /// </summary>
        /// <remarks>
        /// Minimum rows per record is one.
        /// </remarks>
        /// <value>The records per cell.Row.</value>
        [Browsable(true),
        Category("Foreign key options (For Radio buttons)"),
        DefaultValue(1),
        Description(
             @"Sets or gets how many records we should display per row on the column. Example by setting RecordsPerRow = 4 it will render 4 database records per row in your column."
             )]
        public int RecordsPerRow
        {
            get { return m_RecordsPerRow; }
            set { m_RecordsPerRow = value < 1 ? 1 : value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the column should remember it's state.
        /// </summary>
        /// <value><c>true</c> if [remember state]; otherwise, <c>false</c>.</value>
        [Browsable(true),
        DefaultValue(false),
        Description(@"Gets or sets a value indicating whether the column should remember it's state.")]
        public bool RememberState
        {
            get { return m_RememberState; }
            set { m_RememberState = value; }
        }

        /// <summary>
        /// Sets or gets the fields from the data source to specify data ordering.
        /// </summary>
        /// <value>The order by SQL.</value>
        /// <remarks>
        /// If you have several tables in your view then both table name and field name is required.
        /// </remarks>
        [Browsable(true),
        Category("Foreign key options"),
        Description(@"Sets or gets the fields from the data source to specify data ordering.")]
        public string SortExpression
        {
            get { return m_SortExpression; }
            set { m_SortExpression = value; }
        }

        /// <summary>
        /// gets the foreign key table from the data source.
        /// </summary>
        /// <value>The table.</value>
        public Table Table
        {
            get
            {

                if (m_ForeignkeyTable == null)
                    m_ForeignkeyTable = new Table(Grid);
                else
                    return m_ForeignkeyTable;
                if (string.IsNullOrEmpty(DataSourceId))
                    return m_ForeignkeyTable;
                m_ForeignkeyTable.ConnectionString = ConnectionString;
                m_ForeignkeyTable.FilterExpression = FilterExpression;

                m_ForeignkeyTable.DataSourceId = DataSourceId;

                m_ForeignkeyTable.RetrieveForeignkeys = false;
                m_ForeignkeyTable.m_GotSchema = false;
                m_ForeignkeyTable.m_Grid = Grid;
                m_ForeignkeyTable.SortExpression = SortExpression;

                return m_ForeignkeyTable;
            }
            set
            {
                m_ForeignkeyTable = value;
            }
        }

        /// <summary>
        /// Sets or gets the parent identifier column to create a tree structure.
        /// </summary>
        /// <value>The tree parent ColumnId.</value>
        /// <remarks>
        /// Setting this property makes the foreign key render as a tree structure when editable.
        /// </remarks>
        [Browsable(true),
        Category("Foreign key options"),
        Description(@"Sets or gets the parent identifier column to create a tree structure."),
        TypeConverter(typeof(WebGridTreeParentSelector))]
        public string TreeParentId
        {
            get { return m_TreeParentId; }

            set { m_TreeParentId = value; }
        }

        /// <summary>
        /// Sets or Gets Tree Structure seperator, if empty or null this the tree structure level is not rendered.
        /// This property applies to non-editable foreign keys columns.
        /// </summary>
        /// <remarks>Experimental.</remarks>
        [DefaultValue(null),
        Description(
             @"Sets or Gets Tree Structure seperator, if empty or null this the tree structure level is not rendered."
             )]
        public string TreeStructureSeperator
        {
            get { return m_TreeStructureSeperator; }

            set { m_TreeStructureSeperator = value; }
        }

        /// <summary>
        /// Sets or gets which data source field(s) should be displayed for this foreign key.
        /// Example of usage: ValueColumn = "firstname + ' ' + lastname"
        /// </summary>
        /// <remarks>
        /// More than one field can be displayed by using '+' (plus) as value for the ValueColumn.
        /// </remarks>
        /// <value>The value column for the foreign key.</value>
        [Browsable(true),
        Category("Foreign key options"),
        Description(
             @"Sets or gets which datasource field(s) should be displayed for this foreign key. Example of usage: ValueColumn = ""firstname + ' ' + lastname"""
             )]
        public string ValueColumn
        {
            get
            {
                if (!string.IsNullOrEmpty(m_ValueColumn))
                    return m_ValueColumn;
                if (Table == null || Table.Columns == null || Table.Columns.Count == 0)
                    throw new GridException("No Data.Table is set for this foreignkey.");

                // Find first varchar or something
                m_ValueColumn = null;

                for (int i = 0; i < Table.Columns.Count - 1; i++)
                {
                    if (Table.Columns[i].ColumnType != ColumnType.Text)
                        continue;

                    if (Grid.Trace.IsTracing)
                        Grid.Trace.Trace("Detecting value column for {0}({1})", ColumnId, Grid.ID);
                    m_ValueColumn = Table.Columns[i].ColumnId;
                    break;
                }

                if (string.IsNullOrEmpty(m_ValueColumn))
                    throw new GridException(string.Format("Unable to detect 'ValueColumn' property for Foreignkey '{0}' in Grid '{1}'. Please specify the Property manually' ", ColumnId, Grid.ID));
                return m_ValueColumn;
            }
            set { m_ValueColumn = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether only leafs (end node) should be selectable.
        /// This property applies only if you use TreeParentId.
        /// </summary>
        /// <value><c>true</c> if [enable only tree leaf]; otherwise, <c>false</c>.</value>
        internal bool EnableOnlyTreeLeaf
        {
            get { return m_EnableOnlyTreeLeaf; }

            set { m_EnableOnlyTreeLeaf = value; }
        }

        /// <summary>
        /// Gets if this column requires single quote for
        /// data source operations.
        /// </summary>
        internal override bool HasDataSourceQuote
        {
            get
            {
                if (m_ForeignkeyTable != null && m_ForeignkeyTable.Primarykeys != null &&
                    m_ForeignkeyTable.Primarykeys.Count > 0)
                    return m_ForeignkeyTable.Primarykeys.Count != 1 ||
                           m_ForeignkeyTable.Primarykeys[0].HasDataSourceQuote;
                return false;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns></returns>
        public override bool Validate(RowCell cell)
        {
            if (DenyValidate())
                return true;

            if (!Equals(cell.Value, cell.DataSourceValue))
                cell.PostBackValue = cell.Value != null ? cell.Value.ToString() : null;

            if (string.IsNullOrEmpty(cell.PostBackValue) /*|| cell.PostBackValue == Grid.NULLCONSTANT*/)
            {
                if (AllowEmpty && Required == false)
                    return true; // no further checking necessary
                if (string.IsNullOrEmpty(SystemMessage) == false)
                    Grid.SystemMessage.Add(SystemMessage, SystemMessageStyle,
                                           cell.Row.GetColumnInitKeys(ColumnId));
                else if (Grid.DisplayView == DisplayView.Grid && SystemMessageStyle != SystemMessageStyle.WebGrid)
                    Grid.SystemMessage.Add(String.Format(Grid.GetSystemMessage("SystemMessage_Grid_required")),
                                           SystemMessageStyle, cell.Row.GetColumnInitKeys(ColumnId));
                else
                {
                    string title = Title;
                    if (string.IsNullOrEmpty(title))
                        title = ColumnId;
                    Grid.SystemMessage.Add(
                        String.Format(Grid.GetSystemMessage("SystemMessage_Required"), title),
                        SystemMessageStyle, cell.Row.GetColumnInitKeys(ColumnId));
                }
                return false;
            }
            cell.Value = cell.PostBackValue;

            return true;
        }

        // 2005.01.09 - jorn, optimized
        internal static string BuildDisplayText(int index, string valueColumns, Table table)
        {
            try
            {
                Row row = table.Rows[index];
                // NEW
                if (string.IsNullOrEmpty(valueColumns) ||
                    valueColumns.IndexOf("+", StringComparison.OrdinalIgnoreCase) == -1)
                {
                    if (string.IsNullOrEmpty(valueColumns))
                        throw new GridException(string.Format("ValueColumn property is null or empty in foreignkey for grid '{0}'", table.m_Grid.ID));
                    //Table is not loaded yet.
                    if (table.Columns.GetIndex(valueColumns) == -1)
                        throw new GridException(string.Format("Could not find ValueColumn '{0}' property in Foreignkey for grid '{1}'", valueColumns, table.m_Grid.ID));
                    return row[valueColumns].Value != null ? row[valueColumns].Value.ToString() : null;
                }
                string[] columns = valueColumns.Split('+');
                StringBuilder _columnvalue = new StringBuilder(string.Empty);

                for (int i = 0; i < columns.Length; i++)
                {
                    string column = columns[i].Trim();
                    if (column.StartsWith("'") && column.EndsWith("'"))
                        _columnvalue.Append(column.Replace("'", string.Empty));
                    else
                        _columnvalue.Append(row[column].Value);
                }
                return _columnvalue.ToString();
            }
            catch (Exception e)
            {
                throw new GridException("Error processing foreign key", e);
            }
        }

        // 2005.01.09 - jorn, optimized
        internal static string BuildDisplayTextSql(string table, string columns, string alias, bool noComma,
            Table foreigntable)
        {
            if (string.IsNullOrEmpty(columns) || columns.IndexOf("+") == -1)
                return string.Format("{0}[{1}]{2}", table, columns, alias);
            table = table.Trim(); //
            string[] columnarray = columns.Split('+');
            StringBuilder columnvalue = new StringBuilder(string.Empty);

            if (noComma == false && table.StartsWith(","))
                table = table.Substring(1);

            for (int i = 0; i < columnarray.Length; i++)
            {
                if (noComma == false)
                    columnvalue.Append(i == 0 ? ", " : " + ");
                else if (i > 0)
                    columnvalue.Append(" + ");

                string column = columnarray[i].Trim();
                if (column.StartsWith("'") && column.EndsWith("'"))
                    columnvalue.Append(column);
                else
                {
                    columnvalue.AppendFormat(
                        foreigntable.Columns[column].ColumnType == ColumnType.Text
                            ? "{0}[{1}]"
                            : "CAST ({0}[{1}] as varchar(12) )", table, column);
                }
            }
            columnvalue.Append(alias);
            return columnvalue.ToString();
        }

        internal override void CopyFrom(Column column)
        {
            base.CopyFrom(column);
            if (column.ColumnType != ColumnType.Foreignkey) return;
            Foreignkey fk = (Foreignkey)column;
            m_ForeignkeyTable = fk.m_ForeignkeyTable;

            m_ValueColumn = fk.m_ValueColumn;

            m_ConnectionString = fk.m_ConnectionString;
            m_ConnectionStringSet = fk.m_ConnectionStringSet;
            m_Foreignkeytype = fk.m_Foreignkeytype;
            m_SelectMenuWidth = fk.m_SelectMenuWidth;
            m_SelectMenuMaxHeight = fk.m_SelectMenuMaxHeight;
            m_FilterExpression = fk.m_FilterExpression;
            m_TreeParentId = fk.m_TreeParentId;
            m_SortExpression = fk.m_SortExpression;
            m_EnableOnlyTreeLeaf = fk.m_EnableOnlyTreeLeaf;
            m_RecordsPerRow = fk.m_RecordsPerRow;
            m_TreeStructureSeperator = fk.m_TreeStructureSeperator;
            m_RememberState = fk.m_RememberState;
            m_identityColumn = fk.m_identityColumn;
        }

        internal override Column CopyTo(Column column)
        {
            if (column.ColumnType == ColumnType.Foreignkey)
            {
                Foreignkey fk = (Foreignkey)base.CopyTo(column);
                fk.m_ForeignkeyTable = m_ForeignkeyTable;
                fk.m_ConnectionString = m_ConnectionString;
                fk.m_ConnectionStringSet = m_ConnectionStringSet;
                fk.m_ValueColumn = m_ValueColumn;

                fk.m_Foreignkeytype = m_Foreignkeytype;
                fk.m_SelectMenuWidth = m_SelectMenuWidth;
                fk.m_SelectMenuMaxHeight = m_SelectMenuMaxHeight;

                fk.m_FilterExpression = m_FilterExpression;
                fk.m_TreeParentId = m_TreeParentId;
                fk.m_SortExpression = m_SortExpression;

                fk.m_EnableOnlyTreeLeaf = m_EnableOnlyTreeLeaf;
                fk.m_RecordsPerRow = m_RecordsPerRow;
                fk.m_TreeStructureSeperator = m_TreeStructureSeperator;
                fk.m_RememberState = m_RememberState;
                fk.m_identityColumn = m_identityColumn;
                return fk;
            }
            return base.CopyTo(column);
        }

        internal override Column CreateColumn()
        {
            return new Foreignkey(ColumnId, m_Table);
        }

        internal override Column Duplicate()
        {
            Foreignkey c = new Foreignkey(ColumnId, m_Table);
            CopyTo(c);
            return c;
        }

        internal static object GetApostropheValue(RowCell cell)
        {
            return (cell.Value == null || cell.Value is int) ? cell.Value : string.Format("'{0}'", cell.Value);
        }

        // 2005.01.09 - jorn, string optimize
        internal override void GetColumnPostBackData(RowCell cell)
        {
            try
            {
                string uniqueID = cell.CellClientId;
                if (Grid.GotHttpContext && HttpContext.Current.Request.Form[uniqueID] != null)
                {
                    cell.GotPostBackData = true;
                    cell.PostBackValue = HttpContext.Current.Request.Form[uniqueID];
                    if (cell.PostBackValue == string.Empty)
                    {
                        cell.PostBackValue = null;
                        cell.Value = null;
                    }
                    else
                        cell.Value = cell.PostBackValue;
                    Grid.State(uniqueID, cell.PostBackValue);
                }
                else
                {
                    if (!string.IsNullOrEmpty(Table.DataSourceId) &&
                        (Table.Columns == null || Table.Columns.Primarykeys.Count == 0))
                        throw new GridException(string.Format(
                                                    "No Primarykey was detected for foreign key column '{0}'.", Title));
                    // Check if we are using a mastergrid;
                    bool t1 = Grid.MasterWebGrid != null;

                    // Are we in detail view?
                    bool t3 = (Grid.DisplayView == DisplayView.Detail);

                    // Are we in insert-mode, not update-mode?
                    bool t6 = (Grid.InternalId == null);

                    bool t5 = cell.Value == null;

                    // Auto fill in
                    if (t1 && t3 && t5 && t6)
                        cell.Value = Grid.MasterWebGrid.InternalId;

                }
            }
            catch (Exception ee)
            {
                throw new GridException(string.Format("Error getting post back-data for foreign key '{0}' in Grid '{1}'", ColumnId, Grid.ID), ee);
            }
        }

        // 2005.01.09 - jorn, string optimize
        internal override void OnUpdateInsert(CellUpdateInsertArgument ea, RowCell cell)
        {
            // Fix value
            ea.AddApostrophe = true;

            // If update and value matches with DB then don't update
            if (ea.Value != null && ea.Value.Equals(cell.DataSourceValue))
                ea.IgnoreChanges = true;

            if (ea.Value == null)
                ea.AddApostrophe = false;
        }

        internal override void RenderEditView(HtmlTextWriter writer, RowCell cell)
        {
            if (Table.m_GotData == false) // Data need to be recovered if CacheDatasourceStructure is active.
                Table.GetData(true);

            if (Identity || AllowEdit == false || (Grid.DisplayView == DisplayView.Grid && AllowEditInGrid == false))
            {
                RenderLabelView(writer, cell);
                return;
            }

            if (string.IsNullOrEmpty(ValueColumn))
                throw new GridException(
                    String.Format(Table.m_Grid.GetSystemMessage("SystemMessage_ForeignkeyNoValueColumn"), Title));

            StringBuilder s = new StringBuilder(string.Empty);

            switch (ForeignkeyType)
            {
                case ForeignkeyType.Select:
                case ForeignkeyType.SelectMenu:
                    {
                        StringBuilder eventScript = new StringBuilder(string.Empty);
                        StringBuilder javascript = new StringBuilder(string.Empty);
                        if (AutoPostback || string.IsNullOrEmpty(ConfirmMessage) == false)
                        {
                            eventScript = new StringBuilder(" onchange=\"");
                            if (string.IsNullOrEmpty(ConfirmMessage) == false)
                                eventScript.AppendFormat(" if(wgconfirm('{0}',this,'{1}')) ",
                                                         ConfirmMessage.Replace("'", "\\'"),
                                                         Grid.DialogTitle.Replace("'", "\\'"));
                            string link = Grid.EnableCallBack && !ForcePostBack
                                              ? Asynchronous.GetCallbackEventReference(Grid,
                                                                                       string.Format(
                                                                                           "ElementPostBack!{0}!{1}",
                                                                                           ColumnId,
                                                                                           cell.Row.PrimaryKeyValues),
                                                                                       false,
                                                                                       string.Empty,
                                                                                       string.Empty)
                                              : Grid.Page.ClientScript.GetPostBackEventReference(Grid,
                                                                                                 string.Format(
                                                                                                     "ElementPostBack!{0}!{1}",
                                                                                                     ColumnId,
                                                                                                     cell.Row.
                                                                                                         PrimaryKeyValues));
                            eventScript.AppendFormat("{0}\"", link);
                        }

                        StringBuilder onblur = new StringBuilder(" onblur=\"");

                        if (Grid.ColumnChangedColour != Color.Empty)
                            onblur.AppendFormat("isChanged(this,'{0}');", Grid.ColorToHtml(Grid.ColumnChangedColour));
                        onblur.Append("\"");
                        javascript.Append(onblur);
                        s.AppendFormat("<select {0} class=\"wgeditfield wgselectbox {4}\" id=\"{1}\" name=\"{1}\" {2} {3}>",
                                       javascript, cell.CellClientId, eventScript, Attributes,CssClass);
                        if (m_Foreignkeytype == ForeignkeyType.SelectMenu && ( Grid.Scripts == null || !Grid.Scripts.DisableSelectMenu))
                            if (SelectMenuWidth > 0)
                                Grid.AddClientScript(writer,
                                                     string.Format(
                                                         "$(document).ready(function() {{$('#{0}').selectmenu({{maxHeight: {2},style:'dropdown',width: {1}}});}});",
                                                         cell.CellClientId, SelectMenuWidth, SelectMenuMaxHeight));
                            else
                                Grid.AddClientScript(writer,
                                                     string.Format(
                                                         "$(document).ready(function() {{$('#{0}').selectmenu({{maxHeight: {1},style:'dropdown'}});}});",
                                                         cell.CellClientId, SelectMenuMaxHeight));

                        if (Table.Rows.Count == 0 && NullText == null)
                            NullText = "No data available.";

                        if (NullText != null)
                            s.AppendFormat("<option value=\"{1}\">{0}</option>", NullText, string.Empty);

                        //CreateRows(Grid,tree,null,0);

                        s.Append(CreateSelectRows(TreeParentId != null, null, 0, cell));
                        s.Append("</select>");
                    }
                    break;
                case ForeignkeyType.Radiobuttons:
                    m_IsFormElement = false;
                    m_Rowcounter = RecordsPerRow;
                    s.AppendFormat("<table class=\"wgradiobuttons {1}\"><tr><td>{0}</td></tr></table>",
                                   CreateRadioButtons((TreeParentId != null), null, 0, cell),CssClass);
                    break;
            }

            if (string.IsNullOrEmpty(ToolTipInput) == false)
                s = new StringBuilder(Tooltip.Add(s.ToString(), ToolTipInput));

            EditHtml(s.ToString(), writer, cell);
        }

        internal override void RenderLabelView(HtmlTextWriter writer, RowCell cell)
        {
            if (cell.Value != null && !string.IsNullOrEmpty(ValueColumn))
            {
                RowCollection row = Table.Rows;
                string value = cell.Value.ToString();
                for (int i = 0; i < row.Count; i++)
                {
                    if (!string.IsNullOrEmpty(IdentityColumn) && Table.Rows[i][IdentityColumn].Value != null)
                    {
                        if (Table.Rows[i][IdentityColumn].Value.ToString() != value)
                            continue;
                    }
                    else if (row[i].PrimaryKeyUpdateValues != value)
                        continue;
                    cell.Value = BuildDisplayText(i, ValueColumn, Table);
                    break;
                }
            }
            if (cell.Value == null)
                cell.Value = string.Empty;

            string s = cell.Value.ToString();

            if (string.IsNullOrEmpty(TreeStructureSeperator) == false && TreeParentId != null)
            {
                string text = string.Empty;
                RenderEntireTreeList(ref text, cell.Value, 0);
                if (string.IsNullOrEmpty(text) == false)
                    s = text + TreeStructureSeperator + s;
            }
            RenderGrid(s, writer, cell);
        }

        // 2005.01.09 - jorn, string optimize
        private string CreateRadioButtons(bool tree, string parentValue, int level, RowCell cell)
        {
            bool m_Foreignkeyselected = false;
            StringBuilder s = new StringBuilder(string.Empty);
            for (int i = 0; i < Table.Rows.Count; i++)
            {
                string optionValue = Table.Rows[i].PrimaryKeyValues;

                if (!string.IsNullOrEmpty(IdentityColumn) && Table.Rows[i][IdentityColumn].Value != null)
                    optionValue = Table.Rows[i][IdentityColumn].Value.ToString();

                if (tree)
                {
                    string treeValue = null;

                    if (Table.Rows[i][TreeParentId].Value != null)
                        treeValue = Table.Rows[i][TreeParentId].Value.ToString();
                    if (treeValue != parentValue || Grid.EMPTYSTRINGCONSTANT.Equals(optionValue))
                        continue;
                }

                if (i != 0)
                {
                    if (m_Rowcounter == 0)
                    {
                        s.Append("</tr><tr><td>");
                        m_Rowcounter = RecordsPerRow;
                    }
                    else
                        s.Append("<td>");
                }
                m_Rowcounter--;
                string selected = string.Empty;

                if (!m_Foreignkeyselected && optionValue != null)
                {
                    if (cell.PostBackValue != null)
                        selected = Equals(optionValue, cell.PostBackValue)
                                       ? " checked=\"checked\" "
                                       : string.Empty;
                    else if (cell.Value != null)
                        selected = Equals(optionValue, cell.Value.ToString()) ? "checked=\"checked\" " : string.Empty;
                    else if (RememberState && string.IsNullOrEmpty(selected) && (string)Grid.GetState(cell.CellClientId) != null)
                    {
                        selected =
                            (String.Compare(optionValue, Grid.GetState(cell.CellClientId) as string, true) == 0)
                                ? "checked=\"checked\" "
                                : string.Empty;
                    }
                    if (string.IsNullOrEmpty(selected) == false)
                        m_Foreignkeyselected = true;
                }
                StringBuilder optionText = new StringBuilder(string.Empty);
                if (level > 0)
                {
                    for (int j = 0; j < level; j++)
                        optionText.Append(TreeIndentText);
                }
                try
                {
                    optionText.Append(BuildDisplayText(i, ValueColumn, Table));
                }
                catch (Exception e)
                {
                    throw new GridException(string.Format("Error creating radio buttons rows for foreign keys ({0})", ColumnId),
                                            e);
                }
                StringBuilder javascript = new StringBuilder();
                if (AutoPostback || string.IsNullOrEmpty(ConfirmMessage) == false)
                {
                    StringBuilder eventScript = new StringBuilder(" onchange=\"");
                    if (string.IsNullOrEmpty(ConfirmMessage) == false)
                        eventScript.AppendFormat(" if(confirm('{0}')) ", ConfirmMessage);
                    string link = Grid.EnableCallBack && !ForcePostBack ? Asynchronous.GetCallbackEventReference(Grid,
                                                                                               string.Format("ElementPostBack!{0}!{1}",
                                                                                                             ColumnId, cell.Row.PrimaryKeyValues), false,
                                                                                               string.Empty, string.Empty) : Grid.Page.ClientScript.GetPostBackEventReference(Grid,
                                                                                                                                                                              string.Format(
                                                                                                                                                                                  "ElementPostBack!{0}!{1}", ColumnId, cell.Row.PrimaryKeyValues));
                    eventScript.AppendFormat("{0}\"", link);

                    javascript.Append(eventScript);
                }
                s.AppendFormat(
                    "<input type=\"radio\" id=\"rb_{0}_{1}\" {4} name=\"{0}\" value=\"{1}\" {3} /><label class=\"wglabel\" for=\"rb_{0}_{1}\">{2}</label>",
                     cell.CellClientId, optionValue, optionText, selected, javascript);
                if (tree)
                    s.Append(CreateRadioButtons(true, optionValue, level + 1, cell));
            }
            return s.ToString();
        }

        // 2005.01.09 - jorn, string optimize
        private string CreateSelectRows(bool tree, string parentValue, int level, RowCell cell)
        {
            StringBuilder s = new StringBuilder(string.Empty);
            bool m_Foreignkeyselected = false;
            for (int i = 0; i < Table.Rows.Count; i++)
            {

                string optionValue = Table.Rows[i].PrimaryKeyValues;

                if (!string.IsNullOrEmpty(IdentityColumn) && Table.Rows[i][IdentityColumn].Value != null)
                    optionValue = Table.Rows[i][IdentityColumn].Value.ToString();

                if (tree)
                {
                    string treeValue = null;

                    if (Table.Rows[i][TreeParentId].Value != null)
                        treeValue = Table.Rows[i][TreeParentId].Value.ToString();
                    if (treeValue != parentValue || Grid.EMPTYSTRINGCONSTANT.Equals(optionValue))
                        continue;
                }

                string selected = string.Empty;

                if (!m_Foreignkeyselected && optionValue != null)
                {
                    if (cell.PostBackValue != null)
                        selected = Equals(optionValue, cell.PostBackValue)
                                       ? " selected=\"selected\" "
                                       : string.Empty;
                    else if (cell.Value != null)
                        selected = Equals(optionValue, cell.Value.ToString()) ? " selected=\"selected\" " : string.Empty;
                    else if (RememberState && string.IsNullOrEmpty(selected) &&
                             (string)Grid.GetState(cell.CellClientId) != null)
                    {
                        selected =
                            (String.Compare(optionValue, Grid.GetState(cell.CellClientId) as string, true) == 0)
                                ? "selected=\"selected\" "
                                : string.Empty;
                    }
                    if (string.IsNullOrEmpty(selected) == false)
                        m_Foreignkeyselected = true;
                }
                StringBuilder optionText = new StringBuilder(string.Empty);
                if (level > 0)
                    for (int j = 0; j < level; j++)
                        optionText.Append(TreeIndentText);
                try
                {
                    optionText.Append(BuildDisplayText(i, ValueColumn, Table));
                }
                catch (Exception e)
                {
                    throw new GridException(
                        string.Format("Error creating select rows for foreign keys ({0})", ColumnId), e);
                }
                //	if( _enableOnlyTreeLeaf && IsEndLeaf(optionValue) == false )
                //		s += "<option " + selected + "value=\"" + optionValue + "\">" + optionText;
                //	else
                s.AppendFormat("<option{0} value=\"{1}\">{2}</option>", selected, optionValue, optionText);
                if (tree)
                    s.Append(CreateSelectRows(true, optionValue, level + 1, cell));
            }
            return s.ToString();
        }

        private void RenderEntireTreeList(ref string text, object parentID, int level)
        {
            for (int i = 0; i < Table.Rows.Count; i++)
            {
                if (Table.Rows[i][ColumnId].Value != parentID) continue;
                if (Table.Rows[i][TreeParentId].Value != null && level == 0)
                {
                    for (int ii = 0; ii < Table.Rows.Count; ii++)
                    {
                        if (Table.Rows[i][TreeParentId].Value != Table.Rows[ii][ColumnId].Value)
                            continue;

                        text += BuildDisplayText(ii, ValueColumn, Table);
                        if (Table.Rows[ii][TreeParentId].Value != null)
                        {
                            RenderEntireTreeList(ref text, Table.Rows[ii][TreeParentId].Value,
                                                 ++level);
                            return;
                        }
                        return;
                    }
                }
                else if (Table.Rows[i][TreeParentId].Value != null && level > 0)
                {
                    text = BuildDisplayText(i, ValueColumn, Table) + TreeStructureSeperator + text;
                    RenderEntireTreeList(ref text, Table.Rows[i][TreeParentId].Value, ++level);
                }
                else if (level > 0)
                    text = BuildDisplayText(i, ValueColumn, Table) + TreeStructureSeperator + text;
            }
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeConnectionString()
        {
            return false;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeIdentityColumn()
        {
            return false;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeSortExpression()
        {
            return !string.IsNullOrEmpty(m_SortExpression);
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeTable()
        {
            return false;
        }

        #endregion Methods
    }
}