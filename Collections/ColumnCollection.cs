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

namespace WebGrid.Collections
{
    using System;
    using System.Collections.Generic;

    using Data;
    using Design;

    /// <summary>
    /// A collection of the columns. A collection of columns is typically used by
    /// <see cref="Row">WebGrid.Row</see> as a placeholder for the fields of a recordset from a
    /// data source.
    /// </summary>
    [Serializable]
    public class ColumnCollection : List<Column>
    {
        #region Fields

        private readonly Table m_Table;

        private List<Column> m_StoredPrimarykeys;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnCollection">ColumnCollection</see> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public ColumnCollection(Table parent)
        {
            m_Table = parent;
        }

        #endregion Constructors

        #region Properties

        // Primarykeys for this column collections
        /// <summary>
        /// Gets a Column vollection of all primary keys for this Table
        /// </summary>
        public List<Column> Primarykeys
        {
            get
            {
                if (Count == 0)
                    return null;
                return m_StoredPrimarykeys ?? (m_StoredPrimarykeys = FindAll(column => column.Primarykey));
            }
            set
            {
                m_StoredPrimarykeys = value;
            }
        }

        #endregion Properties

        #region Indexers

        /// <summary>
        /// Gets or sets the column with the specified name.
        /// </summary>
        public Column this[string columnName]
        {
            get
            {
                int index = GetIndex(columnName);
                if (index == -1 && string.IsNullOrEmpty(columnName) == false)
                {
                    Column column = new UnknownColumn(columnName, m_Table);
                    Add(column);
                    return column;
                }
                return this[index];
            }
            set
            {
                int index = GetIndex(columnName);
                if (index != -1)
                    this[index] = value;
                else
                    Add(value);
            }
        }

        /// <summary>
        /// Gets or sets the column at the specified index.
        /// </summary>
        public new Column this[int index]
        {
            get
            {
                if (!m_Table.m_GotSchema)
                    m_Table.GetSchema();
                return base[index];
            }
            set
            {
                if (!m_Table.m_GotSchema)
                    m_Table.GetSchema();
                base[index] = value;
            }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Adds a column object to the collection.
        /// </summary>
        /// <param name="column">The column to add.</param>
        /// <returns>The index of the column that has been added to the collection.</returns>
        public new void Add(Column column)
        {
            if (GetIndex(column.ColumnId) > -1)
            {
                if (column.Grid != null)
                    throw new GridException(
                        string.Format("There already exist an column with columnID '{0}' in grid '{1}'", column.ColumnId,
                                      column.Grid.ID));
                throw new GridException(string.Format("There already exist an column with columnID '{0}'",
                                                      column.ColumnId));
            }

            base.Add(column);
        }


        internal void SortByDisplayIndex()
        {
              Sort((p1, p2) => p1.DisplayIndex.CompareTo(p2.DisplayIndex));
         }
      //  private int[] columnsToDisplay;
        internal int[] VisibleColumnsByIndex
        {
            get
            {
                //if (columnsToDisplay != null)
                //    return columnsToDisplay;

                List<int> tmparray = new List<int>();
                SortByDisplayIndex();
                ForEach(delegate(Column column)
                            {
                                if (!column.IsVisible)
                                    return;

                                if (column.HideIfEmpty && !column.AllowEditInGrid &&
                                    m_Table.m_Grid.DisplayView == DisplayView.Grid)
                                {

                                    bool hidden = true;
                                    for (int num = 0; num < m_Table.Rows.Count; num++)
                                    {

                                        if (column.ColumnType == ColumnType.SystemColumn &&
                                            ((SystemColumn)column).ShouldRender(m_Table.Rows[num]))
                                        {
                                            hidden = false;
                                            break;
                                        }
                                        if (m_Table.Rows[num].Cells.GetIndex(column.ColumnId) == -1 ||
                                            m_Table.Rows[num][column.ColumnId].Value == null)
                                            continue;
                                        hidden = false;
                                        break;
                                    }
                                    if (hidden)
                                        return;

                                }

                                tmparray.Add(GetIndex(column.ColumnId));
                            });
                return tmparray.ToArray(); //columnsToDisplay = 
            }
        }

        /// <summary>
        /// Detects if a column already exist.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool Contains(string column)
        {
            return GetIndex(column) > -1;
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
            return FindIndex(column => string.Compare(column.ColumnId, columnName, true) == 0);
        }

        #endregion Methods
    }

    /// <summary>
    /// Public Collections classes used by WebGrid.
    /// </summary>
    internal class NamespaceDoc
    {
    }
}