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

namespace WebGrid.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.Web.UI;

    using Collections;
    using Design;
    using Enums;

    /// <summary>
    /// This class typically represents a row in a <see cref="Table">WebGrid.Table</see> and is used
    /// to store a record from a data source. A row instance uses <see cref="WebGrid.Collections.ColumnCollection">WebGrid.Collections.ColumnCollection</see>
    /// to store fields and field properties from a recordset.
    /// </summary>
    public class Row
    {
        #region Fields

        /// <summary>
        /// Table container for this row.
        /// </summary>
        internal readonly Table m_Table;

        internal DataRow m_DataRow;
        internal int m_RowIndex;

        private bool? m_AllowCopy;
        private bool? m_AllowDelete;
        private string m_GetPrimarykeysInitialValues = string.Empty;
        private string m_GetPrimarykeysUpdateValues = string.Empty;
        private Color m_RowHighLight = Color.Empty;
        private bool? m_SelectableRow;
        List<RowCell> m_primarykeys;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Row">Row</see> class.
        /// </summary>
        /// <param name="table">The table that should contain this row.</param>
        public Row(Table table)
        {
            m_Table = table;
            Cells = new RowCellCollection(this);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Row">Row</see> class.
        /// </summary>
        /// <param name="table">The table that should contain this row.</param>
        ///<param name="createcells"></param>
        public Row(Table table, bool createcells)
        {
            m_Table = table;
            Cells = new RowCellCollection(this);

            if (createcells)
                foreach (Column column in Columns)
                    Cells.Add(new RowCell(column.ColumnId, this));
        }
        #endregion Constructors

        #region Properties

        /// <summary>
        /// Indicates whether the row can be copied.
        /// </summary>
        /// <value><c>true</c> if [allow copy]; otherwise, <c>false</c>.</value>
        public bool AllowCopy
        {
            get { return TristateBoolType.ToBool(m_AllowCopy, m_Table.m_Grid.AllowCopy); }
            set { m_AllowCopy =value; }
        }

        /// <summary>
        /// Indicates whether the row can be deleted.
        /// </summary>
        /// <value><c>true</c> if [allow delete]; otherwise, <c>false</c>.</value>
        public bool AllowDelete
        {
            get { return TristateBoolType.ToBool(m_AllowDelete, m_Table.m_Grid.AllowDelete); }
            set { m_AllowDelete = value; }
        }

        /// <summary>
        /// Gets or sets the columns in this row.
        /// </summary>
        /// <value>The columns for this row</value>
        public RowCellCollection Cells
        {
            get; set;
        }

        ///<summary>
        /// Css class for this Row
        ///</summary>
        [CssClassProperty]
        public string CssClass
        {
            get; set;
        }


        ///<summary>
        /// Gets the name of column being grouped.
        ///</summary>
        public string GroupColumnID
        {
            get;
            internal set;
        }
        ///<summary>
        /// Gets if the column group is expanded.
        ///</summary>
        public bool GroupIsExpanded
        {
            get;
            set;
        }

        private RowType _rowType = RowType.DataRow;

        ///<summary>
        /// WebGrid Row Type
        ///</summary>
        public RowType RowType
        {
            get { return _rowType; }
            internal set { _rowType = value; }
        }

        /// <summary>
        /// Gets the data row. DataRow is used as an reference to the datasource being set in the method <see cref="Table.DataSource"/>
        /// </summary>
        /// <value>The data row.</value>
        internal DataRow DataRow
        {
            get { return m_DataRow; }
        }

        /// <summary>
        /// Gets the primary keys.
        /// </summary>
        /// <value>The primary key.</value>
        public List<RowCell> GetPrimarykeys
        {
            get
            {
                if (m_primarykeys == null)
                {
                    m_primarykeys = new List<RowCell>();

                    foreach (Column column in Columns.Primarykeys)
                        m_primarykeys.Add(Cells[column.ColumnId]);
                }
                return m_primarykeys;
            }
        }

        /// <summary>
        /// Inserts an object placed after of the column in grid view.
        /// </summary>
        /// <value>The HTML to be inserted.</value>
        public object PostRowHtml
        {
            get; set;
        }

        /// <summary>
        /// Inserts an object placed in front of the row.
        /// </summary>
        /// <value>The HTML to be inserted.</value>
        public object PreRowHtml
        {
            get; set;
        }

        /// <summary>
        /// Gets data source primary key values for this row, keys are seperated by ';'
        /// </summary>
        public string PrimaryKeyValues
        {
            get
            {
                if (!string.IsNullOrEmpty(m_GetPrimarykeysInitialValues))
                    return m_GetPrimarykeysInitialValues;
                if (m_Table.Columns.Primarykeys == null)
                    return string.Empty;

                m_Table.Columns.Primarykeys.ForEach(OnDataSourcePrimaryKey);
                return m_GetPrimarykeysInitialValues;
            }
            internal set { m_GetPrimarykeysInitialValues = value; }
        }

        /// <summary>
        /// Gets or sets the color used for highlighting a row on mouseover.
        /// </summary>
        /// <value>The colour.</value>
        public Color RowHighLight
        {
            get { return m_RowHighLight; }
            set { m_RowHighLight = value; }
        }

        /// <summary>
        /// Gets the grid row index (Always 0 in detail view)
        /// </summary>
        /// <remarks>
        /// Header row gives an GridRowIndex at -1
        /// </remarks>
        /// <value>The grid row number.</value>
        public int RowIndex
        {
            get { return m_RowIndex; }
            set { m_RowIndex = value; }
        }

        /// <summary>
        /// Gets or sets whether a checkbox for multiselect is present in the row.
        /// </summary>
        /// <value><c>true</c> if [selectable row]; otherwise, <c>false</c>.</value>
        public bool SelectableRow
        {
            get
            {
                return m_SelectableRow == null && m_Table.m_Grid.SelectableRows ||
                       TristateBoolType.ToBool(m_SelectableRow, true);
            }
            set { m_SelectableRow = value; }
        }

        /// <summary>
        /// Gets or sets the color used for highlighting of a row when it's selected.
        /// </summary>
        /// <value>The colour.</value>
        public Color SelectableRowHighLight
        {
            get; set;
        }

        /// <summary>
        /// Gets the columns in this row.
        /// </summary>
        /// <value>The columns for this row</value>
        internal ColumnCollection Columns
        {
            get { return m_Table.Columns; }
        }

        /// <summary>
        /// Update primary key values for this row, keys are seperated by ';'
        /// </summary>
        internal string PrimaryKeyUpdateValues
        {
            get
            {
                if (string.IsNullOrEmpty(m_GetPrimarykeysUpdateValues) == false)
                    return m_GetPrimarykeysUpdateValues;

                m_Table.Columns.Primarykeys.ForEach(OnUpdatePrimaryKey);

                return m_GetPrimarykeysUpdateValues;
            }
        }

        private int m_RowSpan;
        //How many cells we should span over in this row.
        internal int CellsToSpan
        {
            get
            {
                return m_RowSpan;
            }
            set
            {
                m_RowSpan = value;
            }
        }

        #endregion Properties

        #region Indexers

        /*  /// <summary>
         /// Gets or sets whether this row is checkbox selected. Default is false.
         /// </summary>
         /// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
         /// <remarks>
         /// The grid will be forced to selectablerows = true if this property is set.
         /// </remarks>
        public bool Selected
         {
             get
             {
                 if (m_Selected == null)
                 {
                     for (int i = 0; i < Columns.Count; i++)
                     {
                         if (!(Columns[i].ColumnType == ColumnType.SystemColumn) ||
                             ((SystemColumn)Columns[i]).SystemColumnType != Enums.SystemColumn.SelectColumn ||
                             !Grid.GotHttpContext ||
                             HttpContext.Current.Request[
                                 Columns[i].CellClientId.Replace(string.Format("_{0}", Columns[i].ColumnId), string.Empty)] == null) continue;
                         m_Selected = true;
                         break;
                     }
                 }
                 return TristateBoolType.ToBool(m_Selected, false);
             }
             set
             {
                 if (value && m_Table.m_Grid.SelectableRows == false)
                     m_Table.m_Grid.SelectableRows = true;

                 m_Selected = value;
             }
         }
         */
        /// <summary>
        /// Gets or sets the column with the specified name.
        /// </summary>
        public RowCell this[string cellName]
        {
            get
            {
                int index = Cells.GetIndex(cellName);
                if (index == -1)
                    return Cells[cellName];
                return index != -1 ? Cells[index] : Cells[cellName];
            }
            set
            {
                try
                {
                    int index = Cells.GetIndex(cellName);
                    if (index != -1)
                        Cells[index] = value;
                    else
                        Cells.Add(value);
                }
                catch (Exception ee)
                {
                    throw new GridException(string.Format("cell '{0}' does not exist", cellName), ee);
                }
            }
        }

        /// <summary>
        /// Gets or sets the column at the specified index in this row.
        /// </summary>
        /// <value>The specified column</value>
        public RowCell this[int index]
        {
            get { return Cells[index]; }
            set { Cells[index] = value; }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Validates the columns in this row.
        /// </summary>
        public void ValidateCells()
        {
            for (int i = 0; i < Columns.Count; i++)
            {
                if (Columns[i].ColumnType == ColumnType.SystemColumn || Columns[i].Identity ||
                    (Columns[i].AllowEdit == false && Columns[i].Required == false &&
                     Columns[i].IsInDataSource == false && m_Table.m_Grid.InternalId != null)
                    || (m_Table.m_Grid.DisplayView == DisplayView.Grid && Columns[i].AllowEditInGrid == false) ||
                    (m_Table.m_Grid.DisplayView == DisplayView.Detail && Columns[i].Visibility == Visibility.Grid))
                    continue;
                Columns[i].Validate(this[Columns[i].ColumnId]);
            }
        }

        internal string GetColumnInitKeys(string columnName)
        {
            return string.Format("{0};{1}", PrimaryKeyValues, columnName);
        }

        private void OnDataSourcePrimaryKey(Column primarykey)
        {
            object val =  Cells[primarykey.ColumnId].DataSourceValue;
            if (val == null)
            {
                if (primarykey.IsInDataSource == false && Cells[primarykey.ColumnId].Value != null)
                    val = Cells[primarykey.ColumnId].Value;
                if (val == null)
                    val = Grid.EMPTYSTRINGCONSTANT;
            }
            else
            {
                if (ReferenceEquals(val, string.Empty))
                    val = Grid.EMPTYSTRINGCONSTANT;
                else if (primarykey.ColumnType == ColumnType.Checkbox)
                    val = val.Equals(true) ? Grid.STRINGTRUE : Grid.STRINGFALSE;
            }
            if (!string.IsNullOrEmpty(m_GetPrimarykeysInitialValues))
                m_GetPrimarykeysInitialValues += ",";
            m_GetPrimarykeysInitialValues += val.ToString().Replace(",", ",,");
        }

        private void OnUpdatePrimaryKey(Column primarykey)
        {
            object val =  Cells[primarykey.ColumnId].Value;

              //  if (val == null)
               //     val = Grid.NULLCONSTANT;
            if (val != null)
            {
                if (ReferenceEquals(val, string.Empty))
                    val = Grid.EMPTYSTRINGCONSTANT;
                else if (primarykey.ColumnType == ColumnType.Checkbox)
                    val = val.Equals(true) ? Grid.STRINGTRUE : Grid.STRINGFALSE;
            }
            if (m_GetPrimarykeysUpdateValues != string.Empty)
                m_GetPrimarykeysUpdateValues += ",";
            if (val != null) m_GetPrimarykeysUpdateValues += val.ToString().Replace(",", ",,");
        }

        #endregion Methods

        #region Other

        //   private bool? m_Selected;

        #endregion Other
    }

    /// <summary>
    /// Contains classes that represents data rows, tables, and columns.
    /// </summary>
    internal class NamespaceDoc
    {
    }
}