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

namespace WebGrid.Collections
{
    using System;
    using System.Collections.Generic;

    using WebGrid.Data;
    using WebGrid.Design;

    /// <summary>
    /// A collection of the columns. A collection of columns is typically used by
    /// <see cref="Row">WebGrid.Row</see> as a placeholder for the fields of a recordset from a
    /// data source.
    /// </summary>
    [Serializable]
    public class RowCellCollection : List<RowCell>
    {
        #region Fields

        private readonly Row m_Row;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnCollection">ColumnCollection</see> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public RowCellCollection(Row parent)
        {
            m_Row = parent;
        }
        #endregion Constructors

        #region Indexers

        /// <summary>
        /// Gets or sets the column with the specified name.
        /// </summary>
        public RowCell this[string cellName]
        {
            get
            {
                int index = GetIndex(cellName);
                if (index == -1)
                {
                    if (m_Row.m_Table.Columns.GetIndex(cellName) == -1)
                    {
                        UnknownColumn column = new UnknownColumn(cellName, m_Row.m_Table);
                        m_Row.m_Table.Columns.Add(column);
                    }
                    RowCell cell = new RowCell(cellName, m_Row);
                    Add(cell);
                    return cell;
                }
                return this[index];
            }
            set
            {
                int index = GetIndex(cellName);
                if (index != -1)
                    this[index] = value;
                else
                    Add(value);
            }
        }

        /// <summary>
        /// Gets or sets the column at the specified index.
        /// </summary>
        public new RowCell this[int index]
        {
            get
            {
                if( index == -1)
                    return null;
                return Count <= index ? this[m_Row.Columns[index].ColumnId] : base[index];
            }
            set
            {
                base[index] = value;
            }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Adds a column object to the collection.
        /// </summary>
        /// <param name="cell">The column to add.</param>
        /// <returns>The index of the column that has been added to the collection.</returns>
        public new void Add(RowCell cell)
        {
            //TODO: REMOVE WHEN GOIN PROD.
            if (FindIndex(colcell => string.Compare(colcell.CellId, cell.CellId, true) == 0) > -1)
            {
                this[cell.CellId] = cell;
                return;
            }
            base.Add(cell);
        }

        /// <summary>
        /// Detects if a column already exist.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public bool Contains(string cell)
        {
            return GetIndex(cell) > -1;
        }

        /// <summary>
        /// Removes an item from the collection.
        /// </summary>
        /// <param name="index">The index of the column to be removed.</param>
        public void Remove(int index)
        {
            if (index < Count - 1 && index >= 0)
                RemoveAt(index);
        }

        internal int GetIndex(string columnName)
        {
            return FindIndex(cell => string.Compare(cell.CellId, columnName, true) == 0);
        }

        #endregion Methods
    }

    /// <summary>
    /// Public Collections classes used by WebGrid.
    /// </summary>
    internal class NamespaceDoc2
    {
    }
}