/*
Copyright ©  Olav Christian Botterli.

Dual licensed under the MIT or GPL Version 2 licenses.

Date: 30.08.2011, Norway.

http://www.webgrid.com
*/


/*
Copyright ©  Olav Christian Botterli.

Dual licensed under the MIT or GPL Version 2 licenses.

Date: 30.08.2011, Norway.

http://www.webgrid.com
*/


using System;
using System.ComponentModel;
using System.Web.UI;
using WebGrid.Data;
using WebGrid.Design;
using WebGrid.Enums;
using WebGrid.Events;

namespace WebGrid
{


    /// <summary>
    /// The ColumnTemplate class  is for  third-part ASP.NET controls and integrate them into WebGrid,
    /// this column type supportes both gridview and  detailview.
    /// This class inherits from the <see cref="WebGrid.Column">WebGrid.Column</see> class.
    /// </summary>
    [ParseChildren(true), PersistChildren(true)]
    public class ColumnTemplate : Column, INamingContainer
    {
        #region Properties





        // private ITemplate m_Controls;



        #endregion

        #region Data handling


        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns></returns>
        public override bool Validate(RowCell cell)
        {
            if (DenyValidate())
                return true;
            const bool res = true;


            return res;
        }

        internal override void GetColumnPostBackData(RowCell cell)
        {
            return;
        }

        internal override void OnUpdateInsert(CellUpdateInsertArgument ea, RowCell cell)
        {
            return;
        }

        #endregion

        #region Required functions

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnTemplate"/> class.
        /// </summary>
        public ColumnTemplate()
        {

            GridAlign = HorizontalPosition.Left;
            m_ColumnType = ColumnType.ColumnTemplate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnTemplate"/> class.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="table">The table.</param>
        public ColumnTemplate(string columnName, Table table)
            : base(columnName, table)
        {

            Grid = table.m_Grid;
            m_Table = table;
            ColumnId = columnName;
            GridAlign = HorizontalPosition.Center;
            EditAlign = HorizontalPosition.Center;
            m_ColumnType = ColumnType.ColumnTemplate;
        }

        internal override void CopyFrom(Column column)
        {
            base.CopyFrom(column);
            if (!(column is ColumnTemplate)) return;
            ColumnTemplate c = (ColumnTemplate)column;

            //m_DetailContainer = c.m_DetailContainer;
            m_GridViewTemplate = c.m_GridViewTemplate;
            m_DetailViewTemplate = c.m_DetailViewTemplate;
            m_DetailTemplateContainer = c.m_DetailTemplateContainer;

        }

        internal override Column CopyTo(Column column)
        {
            if (column is ColumnTemplate)
            {
                ColumnTemplate c = (ColumnTemplate)base.CopyTo(column);

                //   c.m_DetailContainer = m_DetailContainer;
                c.m_GridViewTemplate = m_GridViewTemplate;

                c.m_DetailViewTemplate = m_DetailViewTemplate;

                c.m_DetailTemplateContainer = m_DetailTemplateContainer;
                return c;
            }
            return base.CopyTo(column);
        }


        internal override Column CreateColumn()
        {
            return new ColumnTemplate(ColumnId, m_Table);
        }

        internal override Column Duplicate()
        {
            ColumnTemplate c = new ColumnTemplate(ColumnId, m_Table);
            CopyTo(c);
            return c;

        #endregion


        }


        #region Render


        /// <summary>
        /// Gets the ASP.NET controls used by this WebGrid column
        /// </summary>
        /// <value>The controls.</value>
        public Control CreateCellControls
        {
            get
            {
                if (Visibility == Visibility.None || (Grid.DisplayView == DisplayView.Detail && Visibility == Visibility.Grid)
                    || (Grid.DisplayView == DisplayView.Grid && Visibility == Visibility.Detail))
                    return null;

                if (Grid.DisplayView == DisplayView.Detail && m_DetailViewTemplate != null)
                {
                   if (m_DetailTemplateContainer != null)
                      return m_DetailTemplateContainer;

                    Control m_templateContainer = new Control();
                    m_DetailViewTemplate.InstantiateIn(m_templateContainer);
                    m_templateContainer.Visible = false;

                    return m_templateContainer;
                }
                if (Grid.DisplayView == DisplayView.Grid && m_GridViewTemplate != null)
                {
                    Control m_templateContainer = new Control();
                    m_GridViewTemplate.InstantiateIn(m_templateContainer);
                    m_templateContainer.Visible = false;

                    return m_templateContainer;

                }
                return null;

            }
        }

        /// <summary>
        /// HTML Template for WebGrid Columns, if set it will render the content of this instead of the Value of the content.
        /// Create dynamic content by referring to columnId surrounded by [ and ] an example of dynamic content is:
        /// "Hi my first name is [FirstName]"
        /// </summary>
        /// <value>The html content</value>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateInstance(TemplateInstance.Single)]
        [DefaultValue(null)]
        [Browsable(false)]
        [MergableProperty(false)]
        [NotifyParentProperty(true)]
        public ITemplate GridViewTemplate
        {
            get
            {
                //  if (m_GridViewTemplate == null)
                //      m_GridViewTemplate = new PlaceHolder();
                return m_GridViewTemplate;
            }
            set
            {
                m_GridViewTemplate = value;


            }
        }

        private ITemplate m_GridViewTemplate;


        /// <summary>
        /// HTML Template for WebGrid Columns, if set it will render the content of this instead of the Value of the content.
        /// Create dynamic content by referring to columnId surrounded by [ and ] an example of dynamic content is:
        /// "Hi my first name is [FirstName]"
        /// </summary>
        /// <value>The html content</value>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateInstance(TemplateInstance.Single)]
        [DefaultValue(null)]
        [Browsable(false)]
        [MergableProperty(false)]
        [NotifyParentProperty(true)]
        public ITemplate DetailViewTemplate
        {
            get
            {
                //   if (m_DetailViewTemplate == null)
                //       m_DetailViewTemplate = new ITemplate();
                return m_DetailViewTemplate;
            }
            set
            {
                m_DetailViewTemplate = value;


            }
        }

        private ITemplate m_DetailViewTemplate;

        ///<summary>
        /// Detailview template. Used 
        ///</summary>
        private Control m_DetailTemplateContainer;

        internal Control DetailTemplateContainer
        {
            get { return m_DetailTemplateContainer; }
            set { m_DetailTemplateContainer = value; }
        }

        internal Control RenderTemplate(RowCell cell)
        {
            if (cell.Value != null)
            {

                Control ctrl = cell.Value as Control;

                if (ctrl != null)
                {

                    ctrl.Visible = true;
                    ApplyInternalValues(cell, ctrl);

                    return ctrl;
                }
                string isindb = string.Empty;
                if (IsInDataSource)
                    isindb = " The value provided was loaded from your data source. You can disable data source load for this column by 'IsInDataSource=false'";
                throw new GridException(string.Format("ColumnTemplate '{0}' requires a Cell.Value of type 'System.Web.UI.Control'. '{1}' type was provided." + isindb, ColumnId, cell.Value.GetType()));

            }
            return null;
        }


        private void ApplyInternalValues(RowCell cell, Control ctrl)
        {
            foreach (Control control in ctrl.Controls)
            {
                if (control is LiteralControl == false)
                    continue;

                LiteralControl ctr = control as LiteralControl;
                if (ctr == null)
                    continue;
                string innerhtml = ctr.Text;
                if (!string.IsNullOrEmpty(innerhtml) &&
                    innerhtml.IndexOf(string.Format("[{0}]", ColumnId),
                                      StringComparison.OrdinalIgnoreCase) != -1)
                    throw new GridException(
                        string.Format("The template '{0}' can't contain reference to itself.",
                                      ColumnId));

                if (string.IsNullOrEmpty(innerhtml) || !innerhtml.Contains("["))
                    continue;
                string content = innerhtml;
                cell.Row.Columns.ForEach(delegate(Column column)
                {
                    if (
                        content.IndexOf(string.Format("[{0}]", column.ColumnId),
                                        StringComparison.OrdinalIgnoreCase) == -1)
                        return;
                    WebGridHtmlWriter writer = new WebGridHtmlWriter(Grid);
                    Visibility columnvisibility = column.Visibility;
                    column.Visibility = Visibility;
                    column.SystemMessageStyle = SystemMessageStyle.WebGrid;
                    if (Grid.Trace.IsTracing)
                        Grid.Trace.Trace(
                            "Rendering ColumnName: {0} in column: {1}",
                            column.ColumnId, ColumnId);
                    column.RenderEditView(writer, cell.Row.Cells[column.ColumnId]);
                    column.Visibility = columnvisibility;
                    if (column.AllowEdit)
                        writer.Write(
                            "<input type=\"hidden\" value=\"validate\" name=\"{0}_{1}_validate\" />",
                            Grid.ClientID, column.ColumnId);
                    content = content.Replace(string.Format("[{0}]", column.ColumnId),
                                        writer.ToString());
                });
                ((LiteralControl)control).Text = content;
            }
        }


        internal override void RenderEditView(WebGridHtmlWriter writer, RowCell cell)
        {

            if (cell.Value != null && writer != null && Visibility != Visibility.None)
                writer.Write(RenderTemplate(cell));
        }



        internal override void RenderLabelView(WebGridHtmlWriter writer, RowCell cell)
        {

            if (cell.Value != null && Visibility != Visibility.None)
                writer.Write(RenderTemplate(cell));
        }

        #endregion
    }
}
