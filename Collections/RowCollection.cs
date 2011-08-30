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
    using System.Collections.Generic;

    using Data;

    /// <summary>
    /// A collection of rows. A collection of rows is typically used by
    /// <see cref="Table">WebGrid.Table</see> as a placeholder for the recordset from a data source.
    /// </summary>
    public class RowCollection : List<Row>
    {
        #region Fields

        private readonly Table m_Table;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RowCollection">RowCollection</see> class.
        /// </summary>
        /// <param name="table">The table to be used by the collection.</param>
        public RowCollection(Table table)
        {
            m_Table = table;
        }

        #endregion Constructors

        #region Indexers

        /// <summary>
        /// Gets or sets the Row at the specified index in the collection.
        /// </summary>
        public new Row this[int index]
        {
            get { return Count <= index ? null : base[index]; }
            set { base[index] = value; }
        }

        // 2004.01.09 - jorn : String.Compare
        /// <summary>
        /// Returns the row at the specified primary key(s) in the table.
        /// </summary>
        /// <value>Return specified (by primary keys) row within this table.</value>
        /// <remarks>
        /// If a row with the specified primary key(s) does not exist in the table then null is returned.
        /// </remarks>
        public Row this[string primarykeys]
        {
            get
            {
                return Find(delegate(Row row) { return row.PrimaryKeyUpdateValues == primarykeys; });

            }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Adds a Row object to the collection.
        /// </summary>
        /// <param name="row">The row to be added.</param>
        /// <returns>The index of the Row that has been added to the collection.</returns>
        public new void Add(Row row)
        {
            
            if (m_Table == m_Table.m_Grid.MasterTable && row.RowType == Enums.RowType.DataRow && !m_Table.m_Grid.AddRow(ref row))
                return;

            base.Add(row);
        }

        /// <summary>
        /// Removes an item from the collection.
        /// </summary>
        /// <param name="index">The index of the Row to be removed.</param>
        public void Remove(int index)
        {
            if (index < Count - 1 && index >= 0)
                RemoveAt(index);
        }

        #endregion Methods
    }
}