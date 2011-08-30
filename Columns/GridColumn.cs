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
    using System.ComponentModel;
    using System.IO;
    using System.Text;

    using Data;
    using Design;
    using Enums;
    using Events;

    /// <summary>
    /// The GridColumn class is displayed as Grid in the column space. The Grid becomes automatically
    /// a slavegrid for this grid, whoever it is not displayed in the slave grid menu. This column type is 
    /// only supported in detail view (Detail mode for a record)
    /// This class inherits from the <see cref="WebGrid.Column">WebGrid.Column</see> class.
    /// </summary>
    public class GridColumn : Column
    {
        #region Fields

        private Grid m_GridColumn;
        private string m_GridId;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GridColumn"/> class.
        /// </summary>
        public GridColumn()
        {
            GridAlign = HorizontalPosition.Left;
            m_ColumnType = ColumnType.GridColumn;
            m_AllowEdit = false ;
            m_AllowUpdate = false ;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridColumn"/> class.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="table">The grid.</param>
        public GridColumn(string columnName, Table table)
            : base(columnName, table)
        {
            Grid = table.m_Grid;
            m_Table = table;
            ColumnId = columnName;
            GridAlign = HorizontalPosition.Center;
            EditAlign = HorizontalPosition.Center;
            m_ColumnType = ColumnType.GridColumn;
            m_AllowEdit = false ;
            m_AllowUpdate = false ;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the grid ID.
        /// </summary>
        /// <value>The grid ID.</value>
        /// Sets or gets the grid identifier
        /// If the grid is not found or grid is not a slavegrid for this grid's column it will fail.
        [Browsable(true),
        Category("Data"),
        DefaultValue(null),
        Description(
             @"Sets or gets the grid identifier.")]
        public string GridId
        {
            get { return m_GridId; }
            set { m_GridId = value; }
        }

        /// <summary>
        /// Gets or sets the grid object for this column.
        /// </summary>
        /// <value>The grid object.</value>
        public Grid GridObject
        {
            get
            {
                if (m_GridColumn != null || Grid == null || Grid.Page == null || string.IsNullOrEmpty(GridId))
                    return m_GridColumn;

                m_GridColumn = Grid.Page.Parent != null ? Grid.FindGrid(Grid.Page.Parent, GridId) : Grid.FindGrid(Grid.Page, GridId);

                return m_GridColumn;
            }
            set { m_GridColumn = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns></returns>
        public override bool Validate(RowCell cell)
        {
            return !string.IsNullOrEmpty(m_GridId);
        }

        internal override void CopyFrom(Column column)
        {
            base.CopyFrom(column);
            if (column.ColumnType != ColumnType.GridColumn) return;
            GridColumn c = (GridColumn)column;
            m_GridId = c.m_GridId;
        }

        internal override Column CopyTo(Column column)
        {
            if (column.ColumnType == ColumnType.GridColumn)
            {
                GridColumn c = (GridColumn)base.CopyTo(column);
                c.m_GridId = m_GridId;
                return c;
            }
            return base.CopyTo(column);
        }

        internal override Column CreateColumn()
        {
            return new GridColumn(ColumnId, m_Table);
        }

        internal override Column Duplicate()
        {
            GridColumn c = new GridColumn(ColumnId, m_Table);
            CopyTo(c);
            return c;
        }

        internal override void GetColumnPostBackData(RowCell cell)
        {
            return;
        }

        internal override void OnUpdateInsert(CellUpdateInsertArgument ea, RowCell cell)
        {
            return;
        }

        internal override void RenderEditView(WebGridHtmlWriter writer,RowCell cell)
        {
            if (Grid.InternalId == null)
                return;
            if (GridObject == null)
                throw new ApplicationException(
                    string.Format("Grid '{0}' Not found for column '{1}' in grid '{2}'", GridId, Title, Grid.Title));
            Grid.m_HasGridInsideGrid = true;
            GridObject.m_IsGridInsideGrid = true;
            GridObject.m_EventRanDoRender = false;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            System.Web.UI.HtmlTextWriter gridwriter = new System.Web.UI.HtmlTextWriter(sw);

            GridObject.RenderControl(gridwriter);

            GridObject.m_EventRanDoRender = true;
            writer.Write(sb);
        }

        internal override void RenderLabelView(WebGridHtmlWriter writer, RowCell cell)
        {
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeGridObject()
        {
            return false;
        }

        #endregion Methods
    }
}