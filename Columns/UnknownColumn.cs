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

    using WebGrid.Data;

    /// <summary>
    /// The UnknownColumn class is displayed as a column with a free text input box in the web interface.
    /// This class inherits from the <see cref="WebGrid.Column">WebGrid.Column</see> class.
    /// </summary>
    /// 
    [DesignTimeVisible(false),
    Browsable(false),
    ]
    public class UnknownColumn : Column
    {
        #region Constructors

        /// <summary>
        /// Empty Constructor.
        /// </summary>
        public UnknownColumn()
        {
        }

        /// <summary>
        /// UnknownColumn Constructor.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="table">The table object.</param>
        public UnknownColumn(string columnName, Table table)
            : base(columnName, table)
        {
            Grid = table.m_Grid;
            m_Table = table;
            ColumnId = columnName;
        }

        #endregion Constructors

        #region Methods

        internal override Column CreateColumn()
        {
            return new UnknownColumn(ColumnId, m_Table);
        }

        internal override Column Duplicate()
        {
            UnknownColumn c = new UnknownColumn(ColumnId, m_Table);
            CopyTo(c);
            return c;
        }

        #endregion Methods
    }
}