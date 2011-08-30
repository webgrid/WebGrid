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
    using System.ComponentModel;
    using System.Drawing;
    using System.Text;
    using System.Web;
    using System.Web.UI.WebControls;

    using WebGrid.Collections;
    using WebGrid.Data;
    using WebGrid.Data.Database;
    using Table = WebGrid.Data.Table;
    using WebGrid.Design;
    using WebGrid.Enums;
    using WebGrid.Util;

    /// <summary>
    /// The ManyToMany class is displayed as a column with two or more check boxes in the web interface.
    /// Many-to-many relationships occur where, for each instance of table A, there are many instances of table B, 
    /// and for each instance of table B, there are many instances of the table A. For example, a poetry anthology 
    /// can have many authors, and each author can appear in many poetry anthologies.
    /// This class inherits from the <see cref="WebGrid.Column">WebGrid.Column</see> class.
    /// </summary>
    [Serializable]
    public class ManyToMany : Column
    {
        #region Fields

        internal string m_FilterExpression;
        internal Table m_ForeignDataSource;

        private readonly ManyToManyCollection m_Manytomanyitems = new ManyToManyCollection();

        private string m_CheckedAlias;

        // 2005.01.19 - jorn, added support for TreeParentId
        private bool m_EnsuredItems;
        private string m_ForeignIdentityColumn;
        private ManyToManyType m_Manytomanytype = ManyToManyType.Checkboxes;
        private string m_MatrixDatasource;
        private Table m_MatrixTable;
        private string m_MatrixidentityColumn;
        private int m_RecordsPerRow = 1;
        private int m_Rowcounter;
        private string m_SortExpression = string.Empty;
        private string m_TreeParentId;
        private string m_UncheckedAlias;
        private string m_ValueColumn;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WebGrid.ManyToMany">ManyToMany</see> class.
        /// </summary>
        public ManyToMany()
        {
            IsInDataSource = false;
            ManyToManyType = ManyToManyType.Checkboxes;
            m_ForeignDataSource = new Table(null) { RetrieveForeignkeys = false };
            m_ColumnType = ColumnType.ManyToMany;
            Searchable = false;
       
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebGrid.ManyToMany">ManyToMany</see> class.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="table">The table object.</param>
        public ManyToMany(string columnName, Table table)
            : base(columnName, table)
        {
            Grid = table.m_Grid;
            m_Table = table;
            ColumnId = columnName;
            IsInDataSource = false;
            ManyToManyType = ManyToManyType.Checkboxes;
            m_ForeignDataSource = new Table(table.m_Grid) { RetrieveForeignkeys = false };
            Grid.Tables.Add(m_ForeignDataSource);
            m_ColumnType = ColumnType.ManyToMany;
            Searchable = false;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Sets or gets an column grid Alias for a checked checkbox. The default is null (nothing).
        /// A checked checkbox normally being rendered with this column type can be replaced by
        /// the string provided. (This string can also be HTML.)
        /// </summary>
        /// <value>The text/HTML to display when checked.</value>
        [Browsable(true),
        Category("Behavior"),
        Description(@"Sets or gets an alias for a checked checkbox. Default is null (nothing).")]
        public string CheckedAlias
        {
            get { return m_CheckedAlias; }
            set { m_CheckedAlias = value; }
        }

        /// <summary>
        /// Set or gets a standard OleDB database connection string.
        /// </summary>
        /// <value>The OleDB connection string.</value>
        [Browsable(true),
        Category("Many to Many options"),
        SettingsBindable(true),
        Description(@"Set or gets a standard OleDB database connection string.")]
        public string ConnectionString
        {
            get { return m_ForeignDataSource.ConnectionString; }
            set { m_ForeignDataSource.ConnectionString = value; }
        }

        /// <summary>
        /// Sets or gets a filter for the data source. "WHERE" should not be written. Only the statement.
        /// </summary>
        /// <value>The SQL 'where'-filter.</value>
        /// <remarks>
        /// Sets or gets a filter for the data source.
        /// "WHERE" should not be written. Only the statement.
        /// If your data source contains more then one table, the
        /// 'where'-statement might require you to include both table name and table field.
        /// Example: [Customers].[CustomerId]
        /// Where Customers is table name and CustomerId is table field.
        /// </remarks>
        [Browsable(true),
        Category("Many to Many options"),
        Description(
             @"Sets or gets a filter for the data source. ""WHERE"" should not be written. Only the statement.")]
        public string FilterExpression
        {
            get { return m_FilterExpression; }
            set { m_FilterExpression = value; }
        }

        /// <summary>
        /// Sets or gets the foreign data source name.
        /// </summary>
        /// <value>The table name</value>
        [Browsable(true),
        Category("Many to Many options"),
        Description(@"Sets or gets the foreign datasource name.")]
        public string ForeignDataSource
        {
            get { return m_ForeignDataSource.DataSourceId; }
            set { m_ForeignDataSource.DataSourceId = value; }
        }

        /// <summary>
        /// Sets or gets the ForeignKey of the table in the data source that holds the Many-to-Many matrix.
        /// </summary>
        /// <value>The middle foreign key.</value>
        [Browsable(true),
        Category("Many to Many options"),
        Description(
             @"Sets or gets the ForeignKey of the table in the data source that holds the Many-to-Many matrix.")]
        public string ForeignIdentityColumn
        {
            get
            {
                if (string.IsNullOrEmpty(m_ForeignIdentityColumn) && m_ForeignDataSource != null)
                    throw new GridException(string.Format("Please set a foreign identity column for the column '{0}' in grid '{1}'", ColumnId, Grid.ID));
                return m_ForeignIdentityColumn;
            }
            set { m_ForeignIdentityColumn = value; }
        }

        /// <summary>
        /// Gets or sets the foreign table.
        /// </summary>
        /// <value>The foreign table.</value>
        public Table ForeignTable
        {
            get
            {
                return m_ForeignDataSource;
            }
            set
            {
                m_ForeignDataSource = value;
                m_ForeignDataSource.m_GotSchema = true;
            }
        }

        ///<summary>
        /// Gets loaded items for this column.
        ///</summary>
        public ManyToManyCollection Items
        {
            get
            {

                return m_Manytomanyitems;

            }
        }

        /// <summary>
        /// Sets or gets the visual output of the Many-to-Many column.
        /// </summary>
        /// <value>The Many-to-Many type.</value>
        [Browsable(true),
        Category("Many to Many options"),
        DefaultValue(typeof(ManyToManyType), "Checkboxes"),
        Description(@"Sets or gets the visual output of the Many-to-Many column.")]
        public ManyToManyType ManyToManyType
        {
            get { return m_Manytomanytype; }
            set { m_Manytomanytype = value; }
        }

        /// <summary>
        /// Sets or gets the name of the table in the data source that holds the Many-to-Many matrix.
        /// </summary>
        /// <value>the data source that holds the Many-to-Many data.</value>
        [Browsable(true),
        Category("Many to Many options"),
        Description(@"Sets or gets the name of the table in the data source that holds the Many-to-Many matrix."
             )]
        public string MatrixDataSource
        {
            get { return m_MatrixDatasource; }
            set { m_MatrixDatasource = value; }
        }

        /// <summary>
        /// Sets or gets the Identity column of the table in the data source that holds the Many-to-Many matrix.
        /// </summary>
        /// <value>The Identity column for the many-to-many matrix table.</value>
        [Browsable(true),
        Category("Many to Many options"),
        Description(
             @"Sets or gets the Identity column of the table in the data source that holds the Many-to-Many matrix."
             )]
        public string MatrixIdentityColumn
        {
            get { return m_MatrixidentityColumn; }
            set { m_MatrixidentityColumn = value; }
        }

        /// <summary>
        /// Gets or sets the foreign table.
        /// </summary>
        /// <value>The foreign table.</value>
        public Table MatrixTable
        {
            get
            {
                return m_MatrixTable;
            }
            set
            {
                m_MatrixTable = value;
                m_MatrixTable.m_GotSchema = true;
            }
        }

        /// <summary>
        /// Sets if many to many items should be loaded (again)
        /// </summary>
        public bool ReLoad
        {
            get { return !m_EnsuredItems; }
            set { m_EnsuredItems = !value; }
        }

        /// <summary>
        /// Sets or gets how many records we should display per row on the column. Example by setting RecordsPerRow = 4 it will render 4 database records per row in your column.
        /// </summary>
        /// <remarks>
        /// Minimum rows per record is one.
        /// </remarks>
        /// <value>The records per row.</value>
        [Browsable(true),
        Category("Many to Many options"),
        DefaultValue(1),
        Description(
             @"Sets or gets how many records we should display per row on the column. Example by setting RecordsPerRow = 4 it will render 4 database records per row in your column."
             )]
        public int RecordsPerRow
        {
            get { return m_RecordsPerRow; }
            set
            {
                m_RecordsPerRow = value < 1 ? 1 : value;
            }
        }

        /// <summary>
        /// Sets or gets the fields from the data source to specify data ordering.
        /// </summary>
        /// <value>The order by SQL.</value>
        /// <remarks>
        /// If you have several tables in your view then both table name and field name is required.
        /// </remarks>
        [Browsable(true),
        Category("Many to Many options"),
        Description(@"Sets or gets the fields from the data source to specify data ordering.")]
        public string SortExpression
        {
            get { return m_SortExpression; }
            set { m_SortExpression = value; }
        }

        /// <summary>
        /// Sets or gets the parent identifier to create a tree structure.</summary>
        /// <value>The tree parent ColumnId.</value>
        /// <remarks>Setting this property makes the column render as a tree structure.</remarks>
        [Browsable(true),
        Category("Many to Many options"),
        Description(@"Sets or gets the parent identifier to create a tree structure."),
        TypeConverter(typeof(WebGridTreeParentSelector))]
        public string TreeParentId
        {
            get { return m_TreeParentId; }

            set { m_TreeParentId = value; }
        }

        /// <summary>
        /// Sets or gets the column grid Alias for an unchecked checkbox. Default is null (nothing).
        /// An unchecked checkbox normally being rendered with this column type can be replaced by
        /// the string provided. (This string can also be html.)
        /// </summary>
        /// <value>The text/HTML to display when unchecked.</value>
        [Browsable(true),
        Category("Behavior"),
        Description(@"Sets or gets the alias for an unchecked checkbox. Default is null (nothing).")]
        public string UncheckedAlias
        {
            get { return m_UncheckedAlias; }
            set { m_UncheckedAlias = value; }
        }

        /// <summary>
        /// Sets or gets the value of the column.
        /// </summary>
        /// <value>The value of the column.</value>
        [Browsable(true),
        Category("Many to Many options"),
        Description(@"Sets or gets the value of the column.")]
        public string ValueColumn
        {
            get
            {
                try
                {
                    if (m_ValueColumn != null)
                        return m_ValueColumn;

                    if (m_ForeignDataSource == null || m_ForeignDataSource.Columns == null)
                        return null;

                    // Find first varchar or something
                    for (int i = 0; i < m_ForeignDataSource.Columns.Count; i++)
                    {
                        if (m_ForeignDataSource.Columns[i].Primarykey) continue;
                        m_ValueColumn = m_ForeignDataSource.Columns[i].ColumnId;
                        return m_ValueColumn;
                    }
                    return string.Empty;
                }
                catch // FAEN FAEN...
                {
                    return null;
                }
            }
            set { m_ValueColumn = value; }
        }

        /// <summary>
        /// Gets if this column requires single quote for
        /// data source operations.
        /// </summary>
        internal override bool HasDataSourceQuote
        {
            get
            {
                if (m_MatrixTable != null && m_MatrixTable.Primarykeys != null &&
                    m_MatrixTable.Primarykeys.Count > 0)
                    return m_MatrixTable.Primarykeys.Count != 1 ||
                           m_MatrixTable.Primarykeys[0].HasDataSourceQuote;
                return false;
            }
        }

        #endregion Properties

        #region Methods

        ///<summary>
        /// Clear the collection
        ///</summary>
        public void Clear()
        {
            if (Items != null)
                Items.Clear();
        }

        /// <summary>
        /// Gets the items in the Many-to-Many collection.
        /// </summary>
        /// <value>The items.</value>
        public ManyToManyCollection LoadItems(RowCell cell)
        {
            GetColumnPostBackData(cell);
            EnsureItems(cell);

            if ((Value(cell) != null))
                foreach (string item in Value(cell).ToString().Split(','))
                    if (Items[item] != null)
                        Items[item].Checked = true;
            return Items;
        }

        internal override void AfterUpdate(string editIndex, RowCell cell)
        {
            if (AllowEdit == false || Grid.DisplayView == DisplayView.Grid)
                return;

            if (editIndex != null) //Used on inserts.
                cell.Row[MatrixIdentityColumn].Value = editIndex;
            if( cell.Row[MatrixIdentityColumn].Value == null)
            {
                throw new GridException(string.Format("Column '{0}' in Grid '{1}' is missing a value for MatrixIdentityColumn property.", Grid.ID, Grid.ID));
            }
            EnsureItems(cell);
            // FAEN interface + varchar PK ?
            string sql = null;
            try
            {

                sql = string.Format("DELETE FROM [{0}] WHERE {1}", MatrixDataSource,
                                    Interface.BuildFilterElement(true, null, m_Table.Columns[MatrixIdentityColumn],
                                                                 cell.Row[MatrixIdentityColumn].Value));
                Query.ExecuteNonQuery(sql, m_ForeignDataSource.ConnectionString);
                if (Grid.Debug)
                    Grid.m_DebugString.AppendFormat("<b>Many-To-Many ({0}) Delete</b>: {1}<br/>", ColumnId, sql);
            }
            catch (Exception ee)
            {
                throw new GridException(sql, String.Format("Error updating Many to many column '{0}'", ColumnId), ee);
            }

            if (cell.Value == null)
            {
                m_EnsuredItems = false;
                EnsureItems(cell);
                return;
            }
            string[] values = cell.Value.ToString().Split(',');
            for (int i = 0; i < values.Length; i++)
            {
                try
                {
                    string val = values[i];
                    string key = null;
                    if (cell.Row[MatrixIdentityColumn].Value != null)
                        key = cell.Row[MatrixIdentityColumn].Value.ToString();
                    if (m_ForeignDataSource.Columns[ValueColumn].HasDataSourceQuote)
                        val = String.Format("'{0}'", val);
                    if (!Util.Validate.IsNumeric(key))
                        key = String.Format("'{0}'", key);
                    sql = string.Format("INSERT INTO {0}({1},{2}) VALUES({3},{4});",
                                        MatrixDataSource, MatrixIdentityColumn, ForeignIdentityColumn, key,
                                        val);
                    Query.ExecuteNonQuery(sql, m_ForeignDataSource.ConnectionString);

                    if (Grid.Debug)
                        Grid.m_DebugString.AppendFormat("<b>Many-To-Many ({0}) Insert</b>: {1}<br/>", ColumnId, sql);
                }
                catch (Exception ee)
                {
                    throw new GridException(sql, String.Format("Error updating Many to many column '{0}'", ColumnId),
                                            ee);
                }
            }
            m_EnsuredItems = false;
            EnsureItems(cell);
        }

        internal override void CopyFrom(Column column)
        {
            base.CopyFrom(column);
            if (column.ColumnType != ColumnType.ManyToMany) return;
            ManyToMany mk = (ManyToMany)column;
            m_ForeignDataSource = mk.m_ForeignDataSource;
            m_ValueColumn = mk.m_ValueColumn;
            m_Manytomanytype = mk.m_Manytomanytype;
            m_ForeignIdentityColumn = mk.m_ForeignIdentityColumn;
            m_MatrixidentityColumn = mk.m_MatrixidentityColumn;
            m_MatrixDatasource = mk.m_MatrixDatasource;
            m_FilterExpression = mk.m_FilterExpression;
            ConnectionString = mk.ConnectionString;
            m_TreeParentId = mk.m_TreeParentId;
            m_RecordsPerRow = mk.m_RecordsPerRow;
            m_SortExpression = mk.m_SortExpression;
            m_CheckedAlias = mk.m_CheckedAlias;
            m_UncheckedAlias = mk.m_UncheckedAlias;
            m_MatrixTable = mk.m_MatrixTable;
        }

        internal override Column CopyTo(Column column)
        {
            if (column.ColumnType == ColumnType.ManyToMany)
            {
                ManyToMany mk = (ManyToMany)base.CopyTo(column);
                mk.m_ForeignDataSource = m_ForeignDataSource;
                mk.m_ValueColumn = m_ValueColumn;
                mk.m_Manytomanytype = m_Manytomanytype;
                mk.m_ForeignIdentityColumn = m_ForeignIdentityColumn;
                mk.m_MatrixidentityColumn = m_MatrixidentityColumn;
                mk.m_MatrixDatasource = m_MatrixDatasource;
                mk.m_TreeParentId = m_TreeParentId;
                mk.m_RecordsPerRow = m_RecordsPerRow;
                mk.m_FilterExpression = m_FilterExpression;
                mk.ConnectionString = ConnectionString;
                mk.m_SortExpression = m_SortExpression;
                mk.m_CheckedAlias = m_CheckedAlias;
                mk.m_UncheckedAlias = m_UncheckedAlias;
                mk.m_MatrixTable = m_MatrixTable;
                return mk;
            }
            return base.CopyTo(column);
        }

        internal override Column CreateColumn()
        {
            return new ManyToMany(ColumnId, m_Table);
        }

        internal override Column Duplicate()
        {
            ManyToMany c = new ManyToMany(ColumnId, m_Table);
            CopyTo(c);
            return c;
        }

        internal void EnsureItems(RowCell cell)
        {
            if (m_EnsuredItems)
                return;
            m_EnsuredItems = true;
            Items.Clear();
            if (string.IsNullOrEmpty(m_ForeignDataSource.DataSourceId))
            {
                if (m_MatrixTable == null)
                    throw new GridException("MatrixTable must be set");

                foreach (Row row in m_ForeignDataSource.Rows)
                {
                    if (row[ForeignIdentityColumn].Value == null)
                        continue;
                    ManyToManyItem i = new ManyToManyItem
                                           {
                                               Value = row[ForeignIdentityColumn].Value.ToString(),
                                               DisplayText = row[ValueColumn].Value.ToString(),
                                               Checked = false,
                                               ParentId = (TreeParentId != null ? string.Empty : null)
                                           };
                    Items.Add(i);
                }

            }
            else
            {
                m_ForeignDataSource.GetSchema();

                StringBuilder sql = new StringBuilder(string.Empty);
                Query q = null;
                string matrixkey = null;
                if (cell.Row[MatrixIdentityColumn].Value != null)
                    matrixkey = cell.Row[MatrixIdentityColumn].Value.ToString();
                try
                {
                    // FAENFAEN Må flyttes over i DBInterface
                    sql = new StringBuilder(string.Empty);
                    sql.AppendFormat("SELECT {0},{1} ", ForeignIdentityColumn,
                                     ValueColumn);
                    if (matrixkey != null)
                    {
                        sql.AppendFormat(",(SELECT COUNT(*) FROM [{0}] WHERE ", MatrixDataSource);
                        sql.AppendFormat("{0} = {1} AND ", ForeignIdentityColumn, Interface.BuildTableElement(true, m_ForeignDataSource.DataSourceId,
                                         ForeignIdentityColumn));
                        sql.AppendFormat(
                            !m_Table.Columns[MatrixIdentityColumn].HasDataSourceQuote ? "{0} = {1}" : "{0} = '{1}'",
                            MatrixIdentityColumn, matrixkey);

                        if (string.IsNullOrEmpty(FilterExpression) == false)
                            sql.AppendFormat(" AND {0}", FilterExpression);
                        sql.Append(" ) ");
                    }
                    if (string.IsNullOrEmpty(TreeParentId) == false)
                        sql.AppendFormat(", {0} as ParentID ", TreeParentId);
                    sql.AppendFormat(" FROM [{0}]", m_ForeignDataSource.DataSourceId);
                    if (string.IsNullOrEmpty(FilterExpression) == false)
                        sql.AppendFormat(" WHERE {0}", FilterExpression);

                    if (string.IsNullOrEmpty(SortExpression) == false)
                        sql.AppendFormat(" ORDER BY {0}", SortExpression);

                    q = Query.ExecuteReader(sql.ToString(), m_ForeignDataSource.ConnectionString, Grid.DatabaseConnectionType);
                    while (q.Read())
                    {
                        ManyToManyItem i = new ManyToManyItem();
                        i.Value = q[0].ToString();
                        i.DisplayText = q[1].ToString();
                        i.ParentId = (TreeParentId != null ? q["ParentID"].ToString() : null);
                        i.Checked = matrixkey != null && q[2].ToString() != "0";

                        Items.Add(i);
                    }
                    q.Close();
                }
                catch (Exception ee)
                {
                    if (q != null)
                        q.Close();
                    throw new GridException(sql.ToString(), "Error retrieving data for many-to-many sql", ee);
                }

                if (Grid.Debug)
                {
                    Grid.m_DebugString.AppendFormat("<b>Many-To-Many ({0})</b>: {1}<br/>", ColumnId, sql);
                }
            }
        }

        internal override void GetColumnPostBackData(RowCell cell)
        {
            cell.GotPostBackData = true;

            if (Grid.GotHttpContext && HttpContext.Current.Request[cell.CellClientId] != null)
                cell.Value = HttpContext.Current.Request[cell.CellClientId];
        }

        internal override void RenderEditView(WebGridHtmlWriter writer, RowCell cell)
        {
            StringBuilder sb = new StringBuilder();
            GetColumnPostBackData(cell);

            EnsureItems(cell);

            if (Items.Count > 0)
            {
                string strHeight = "5";
                if (HeightEditableColumn != Unit.Empty)
                    strHeight = HeightEditableColumn.Value.ToString();

                if (ManyToManyType == ManyToManyType.Multiselect)
                    sb.AppendFormat(
                        "<select size={0} multiple=\"multiple\" class=\"wgeditfield\" id=\"{1}\" name=\"{1}\">",
                        strHeight,
                        cell.CellClientId);
                else
                {
                    sb.Append("<table class=\"wgmanytomany\"><tr><td>");
                    m_Rowcounter = RecordsPerRow;
                }

                RenderDetailTree(sb, null, 0, cell);

                sb.Append(ManyToManyType == ManyToManyType.Multiselect ? "</select>" : "</tr></table>");
            }
            string displayText = sb.ToString();
            if (string.IsNullOrEmpty(ToolTipInput) == false)
                displayText = Tooltip.Add(displayText, ToolTipInput);
            EditHtml(displayText, writer, cell);
        }

        internal override void RenderLabelView(WebGridHtmlWriter writer, RowCell cell)
        {
            LabelHtml("many-to-many column is not supported in grid view.", writer, cell);
        }

        // 2005.01.19 - jorn, added this recursive function.
        private void RenderDetailTree(StringBuilder sb, string parentID, int level, RowCell cell)
        {
            if (parentID == null)
                parentID = string.Empty;
            List<string> selectedItems = new List<string>();
            if (cell.Value != null && Grid.Page != null && Grid.Page.IsPostBack)
                selectedItems.InsertRange(0, cell.Value.ToString().Split(','));

            for (int i = 0; i < Items.Count; i++)
            {
                if ((Items[i].ParentId ?? string.Empty) != parentID)
                    continue;
                if (Grid.Page != null && Grid.Page.IsPostBack && selectedItems.Count > 0)
                    Items[i].Checked = selectedItems.Contains(Items[i].Value);

                StringBuilder indentText = new StringBuilder(string.Empty);
                if (level > 0)
                    for (int j = 0; j < level; j++)
                        indentText.Append(TreeIndentText);

                StringBuilder selected = new StringBuilder(string.Empty);

                if (Items[i].Checked)
                {
                    selected.Append(ManyToManyType == ManyToManyType.Multiselect
                                        ? " selected=\"selected\" "
                                        : " checked=\"checked\" ");
                }
                if (AllowEdit == false)
                    selected.Append(" disabled=\"disabled\" ");

                if (ManyToManyType == ManyToManyType.Multiselect)
                    sb.AppendFormat("<option {0} value=\"{1}\">{2}{3}</option>", selected, Items[i].Value, indentText,
                                    Items[i].DisplayText);
                else
                {
                    if (i != 0)
                    {
                        if (m_Rowcounter == 0)
                        {
                            sb.Append("</tr><tr><td>");
                            m_Rowcounter = RecordsPerRow;
                        }
                        else
                            sb.Append("<td>");
                    }
                    m_Rowcounter--;
                    sb.Append(indentText);

                    StringBuilder javascript = new StringBuilder(string.Empty);
                    StringBuilder onblur = new StringBuilder(" onblur=\"");
                    if (Grid.InputHighLight != Color.Empty)
                    {
                        javascript.AppendFormat(
                            " onfocus=\"this.accessKey = this.style.backgroundColor;this.style.backgroundColor='{0}';\"",
                            Grid.ColorToHtml(Grid.InputHighLight));
                        onblur.Append("this.style.backgroundColor=this.accessKey;");
                    }
                    if (Grid.ColumnChangedColour != Color.Empty)
                        onblur.AppendFormat("isChanged(this,'{0}');", Grid.ColorToHtml(Grid.ColumnChangedColour));
                    onblur.Append("\"");
                    javascript.Append(onblur);

                    if ((AutoPostback || !string.IsNullOrEmpty(ConfirmMessage)) && Grid.Page != null)
                    {
                        StringBuilder eventScript = new StringBuilder(" onclick=\"");
                        if (!string.IsNullOrEmpty(ConfirmMessage))
                            eventScript.AppendFormat(" if(wgconfirm('{0}',this),'{1}') ", ConfirmMessage.Replace("'", "\\'"), Grid.DialogTitle.Replace("'", "\\'"));
                        string link = Grid.EnableCallBack && !ForcePostBack
                                          ? Asynchronous.GetCallbackEventReference(Grid,
                                                                                   string.Format(
                                                                                       "ElementPostBack!{0}!{1}",
                                                                                       ColumnId,
                                                                                       cell.Row.PrimaryKeyValues),
                                                                                   false,
                                                                                   string.Empty, string.Empty)
                                          : Grid.Page.ClientScript.GetPostBackEventReference(Grid,
                                                                                             string.Format(
                                                                                                 "ElementPostBack!{0}!{1}",
                                                                                                 ColumnId,
                                                                                                 cell.Row.
                                                                                                     PrimaryKeyValues));
                        eventScript.AppendFormat("{0}\"{1}", link, eventScript);
                        javascript.Append(eventScript);
                    }

                    if (!string.IsNullOrEmpty(CheckedAlias) && Items[i].Checked)
                    {
                        sb.Append(CheckedAlias);
                        sb.Append(Items[i].DisplayText);
                    }
                    else if (!string.IsNullOrEmpty(UncheckedAlias) && !Items[i].Checked)
                    {
                        sb.Append(UncheckedAlias);
                        sb.Append(Items[i].DisplayText);
                    }
                    else
                    {
                        sb.AppendFormat(
                            "<input {0} type=\"checkbox\" id=\"cb_{1}_{2}\" name=\"{1}\" value=\"{2}\" {3} />",
                            javascript, cell.CellClientId,
                            Items[i].Value, selected);
                        sb.AppendFormat(
                            "<label class=\"wglabel\"  id=\"label_{0}_{1}\" for=\"cb_{0}_{1}\">", cell.CellClientId, Items[i].Value);

                        sb.AppendFormat("{0}</label>", Items[i].DisplayText);
                    }
                    sb.Append("</td>");
                }

                RenderDetailTree(sb, Items[i].Value, level + 1, cell);
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
        private bool ShouldSerializeForeignTable()
        {
            return false;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeReLoad()
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

        #endregion Methods
    }
}