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
    using System.Collections;
    using System.ComponentModel;
    using System.Text;
    using System.Web;
    using System.Web.UI.WebControls;

    using Data;
    using Table = Data.Table;
    using Design;
    using Enums;
    using Events;
    using Util;

    /// <summary>
    /// The Chart class is displayed as an image (graph) in the web interface.
    /// This class inherits from the <see cref="WebGrid.Column">WebGrid.Column</see> class.
    /// The Chart column is only presentable in grid view, and uses rowspan equal to
    /// the number of records visibility on the screen. 
    /// </summary>
    public class Chart : Column
    {
        #region Fields

        /// <summary>
        /// Sets or gets the image the chart is ending with.
        /// </summary>
        internal string m_EndImage;
        internal int m_IValue;

        /// <summary>
        /// Sets or gets the image used for chart.
        /// </summary>
        internal string m_Image;

        /// <summary>
        /// Sets or gets the image the chart is starting with.
        /// </summary>
        internal string m_StartImage;

        private string m_BackgroundImage;
        private ChartType m_ChartType = ChartType.Undefined;
        private bool m_TitleBoardOnChart;
        private string m_TitleColumn;
        private string m_ValueColumn;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WebGrid.Chart">Chart</see> class.
        /// </summary>
        public Chart()
        {
            Searchable = false;
            Sortable = false;
            IsInDataSource = false;
            m_ColumnType = ColumnType.Chart;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebGrid.Chart">Chart</see> class.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="table">The table object.</param>
        public Chart(string columnName, Table table)
            : base(columnName, table)
        {
            Grid = table.m_Grid;
            m_Table = table;
            ColumnId = columnName;
            Searchable = false;
            Sortable = false;
            IsInDataSource = false;
            m_ColumnType = ColumnType.Chart;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Sets or gets the url for the background image.
        /// </summary>
        /// <value>The url to the background image.</value>
        [Browsable(true),
        Category("Behavior"),
        Description(@"Sets or gets the url for the background image for this column.")]
        public string BackgroundImage
        {
            get { return m_BackgroundImage; }
            set { m_BackgroundImage = value; }
        }

        /// <summary>
        /// Sets or gets the chart type. The default chart type is Undefined.
        /// </summary>
        /// <value>The type of the chart.</value>
        /// <remarks>
        /// If you are using Charts then row interaction is disabled (like rowhighlight and selectable rows).
        /// The reason for this is that table head (thead) is effected by rowspan for this column.
        /// </remarks>
        [Browsable(true),
        Category("Appearance"),
        Description(@"Sets or gets the chart type. The default chart type is Undefined.")]
        public ChartType ChartType
        {
            get
            {
                return m_ChartType == ChartType.Undefined ? ChartType.PieChart3D : m_ChartType;
            }
            set
            {
                m_ChartType = value;
              
                UseAllRows = (value != ChartType.HorizontalBar);
                //
            }
        }

        /// <summary>
        /// Sets or gets whether there should be rendered a label board for the graph.
        /// </summary>
        /// <value><c>true</c> if [render label board]; otherwise, <c>false</c>.</value>
        /// <remarks>If true the label board will be displayed on the right hand side of the graph.</remarks>
        [Browsable(true),
        Category("Appearance"),
        Description(
             @"Sets or gets whether there should be rendered a label board for the graph (label board will be displayed on the right hand side of the graph)."
             )]
        public bool TitleBoardOnChart
        {
            get { return m_TitleBoardOnChart; }
            set { m_TitleBoardOnChart = value; }
        }

        /// <summary>
        /// Sets or gets a name column for this column. This property is used as title
        /// name for the values you retrieve from your data source.
        /// </summary>
        /// <value>The name column.</value>
        [Browsable(true),
        Category("Data"),
        Description(
             @"Sets or gets a name column for this column. This property is used as title name for the values you retrieve from your data source."
             )]
        public string TitleColumn
        {
            get { return m_TitleColumn; }
            set { m_TitleColumn = value; }
        }

        /// <summary>
        /// Sets or gets a value column for this column.
        /// The value column must be a numeric/float field from your data-source.
        /// </summary>
        /// <value>The value column.</value>
        [Browsable(true),
        Category("Data"),
        Description(
             @"Sets or gets a value column for this column. The value column must be a numeric/float field from your data-source."
             )]
        public string ValueColumn
        {
            get { return m_ValueColumn; }
            set { m_ValueColumn = value; }
        }

        internal int BarWidth
        {
            get
            {
                if (MaxSize == 0) return 0;
                if (m_IValue == 0) return 0;
                float f = (m_IValue * (float)WidthEditableColumn.Value / MaxSize);
                return (int)f;
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
            return true;
        }

        internal override void CopyFrom(Column column)
        {
            base.CopyFrom(column);
            if (column.ColumnType != ColumnType.Chart) return;
            Chart c = (Chart)column;
            m_ValueColumn = c.m_ValueColumn;
            m_TitleColumn = c.m_TitleColumn;
            m_IValue = c.m_IValue;
            m_ChartType = c.m_ChartType;
            m_BackgroundImage = c.m_BackgroundImage;
            m_Image = c.m_Image;
            m_StartImage = c.m_StartImage;
            m_EndImage = c.m_EndImage;
            m_TitleBoardOnChart = c.m_TitleBoardOnChart;
        }

        internal override Column CopyTo(Column column)
        {
            if (column.ColumnType == ColumnType.Chart)
            {
                Chart c = (Chart)base.CopyTo(column);
                c.m_ValueColumn = m_ValueColumn;
                c.m_TitleColumn = m_TitleColumn;
                c.m_IValue = m_IValue;
                c.m_ChartType = m_ChartType;
                c.m_BackgroundImage = m_BackgroundImage;
                c.m_Image = m_Image;
                c.m_StartImage = m_StartImage;
                c.m_EndImage = m_EndImage;
                c.m_TitleBoardOnChart = m_TitleBoardOnChart;
                return c;
            }
            return base.CopyTo(column);
        }

        internal override Column CreateColumn()
        {
            return new Chart(ColumnId, m_Table);
        }

        internal override Column Duplicate()
        {
            Chart c = new Chart(ColumnId, m_Table);
            CopyTo(c);
            return c;
        }

        internal override void GetColumnPostBackData(RowCell cell)
        {
            // do nothing
        }

        internal override void OnUpdateInsert(CellUpdateInsertArgument ea, RowCell cell)
        {
            ea.IgnoreChanges = true;
        }

        internal override void RenderEditView(WebGridHtmlWriter writer, RowCell cell)
        {
            const string s = "Graphs and charts are not supported in detail view.";
            EditHtml(s, writer, cell);
        }

        internal override void RenderLabelView(WebGridHtmlWriter writer, RowCell cell)
        {
            string uniqueID = cell.CellClientId;
            if (ChartType == ChartType.HorizontalBar)
            {
                if (m_Image == null) // Draw using table
                {
                    StringBuilder bg = new StringBuilder(string.Empty);
                    if (BackgroundImage != null)
                        bg.AppendFormat(" background=\"{0}\"", BackgroundImage);
                    StringBuilder s = new StringBuilder(string.Empty);
                    s.AppendFormat("<table height=\"{0}\">", HeightEditableColumn);
                    if (m_StartImage != null)
                        s.AppendFormat(
                            "<td><img alt=\"{2}\" border=\"0\" src=\"{0}\"  height={1}/></td>", m_StartImage,
                            HeightEditableColumn, Title);
                    s.AppendFormat("<td width=\"{0}\" {1}></td>", BarWidth, bg);
                    if (m_EndImage != null)
                        s.AppendFormat(
                            "<td><img alt=\"{2}\" border=\"0\" src=\"{0}\"  height={1}/></td>", m_EndImage,
                            HeightEditableColumn, Title);
                    s.Append("</table>");
                    LabelHtml(s.ToString(), writer, cell);
                    return;
                }
                else // Draw using image
                {
                    StringBuilder s = new StringBuilder(string.Empty);
                    if (m_StartImage != null)
                        s.AppendFormat("<img alt=\"{2}\" border=\"0\" src=\"{0}\"  height={1}/>",
                                       m_StartImage, HeightEditableColumn, Title);
                    s.AppendFormat("<img alt=\"{3}\" border=\"0\" src=\"{0}\"  height={1} width={2}/>",
                                   m_Image, HeightEditableColumn, BarWidth, Title);
                    if (m_EndImage != null)
                        s.AppendFormat("<img alt=\"{2}\" border=\"0\" src=\"{0}\"  height={1}/>", m_EndImage,
                                       HeightEditableColumn, Title);

                    LabelHtml(s.ToString(), writer, cell);
                    return;
                }
            }
            if (ChartType == ChartType.HorizontalBar)
            {
                // 2005.01.09 - jorn, string optimization
                StringBuilder namesBuilder = new StringBuilder();
                string values = string.Empty;
                Table t = Grid.MasterTable;
                for (int i = 0; i < t.Rows.Count; i++)
                {
                    if (namesBuilder.Length != 0) namesBuilder.Append(",");
                    if (values.Length != 0) values += ",";
                    namesBuilder.Append(HttpUtility.UrlEncode(t.Rows[i][TitleColumn].Value.ToString(), Encoding.Default));
                    try
                    {
                        values +=
                            HttpUtility.UrlEncode(t.Rows[i][ValueColumn].Value.ToString().Replace(",", "."),
                                                  Encoding.Default);
                    }
                    catch
                    {
                        values += "0";
                    }
                }

                string width = "150";
                if (WidthEditableColumn.IsEmpty == false)
                    width = WidthEditableColumn.Value.ToString();
                string s =
                    string.Format(
                        "<img alt=\"{4}\" border=\"0\" src=\"?wgpiechart={0}&values={1}&names={2}&width={3}\"/>",
                        uniqueID, values, namesBuilder, width, Title);
                LabelHtml(s, writer, cell);
                return;
            }
            else
            {
                ArrayList values = new ArrayList();

                Table t = Grid.MasterTable;
                for (int i = 0; i < t.Rows.Count; i++)
                {
                    double val = double.Parse(t.Rows[i][ValueColumn].Value.ToString());
                    string name = string.Empty;
                    if (t.Rows[i][TitleColumn].Value != null)
                        name = t.Rows[i][TitleColumn].Value.ToString();

                    ChartValue cv = new ChartValue(name, val);
                    values.Add(cv);

                    /*					if(names != string.Empty) names += ",";
                                        if(values != string.Empty) values += ",";
                                        names += HttpUtility.UrlEncode( t.Rows[i][ TitleColumn ].Value );
                                        try
                                        {
                                            values += HttpUtility.UrlEncode( t.Rows[i][ ValueColumn ].Value.Replace(",",".") );
                                        }
                                        catch
                                        {
                                            values += "0";
                                        }*/
                }

                int width = (int)WidthEditableColumn.Value;
                if (width < 16) width = 150;
                int height = (int)HeightEditableColumn.Value;
                if (height < 16) height = 200;

                //try
                //{
                Util.Chart c = new Util.Chart(ChartType, width, height);
                if (BackgroundImage != null && Grid.GotHttpContext)
                {
                    if (BackgroundImage.ToUpperInvariant().StartsWith("HTTP://") == false)
                        BackgroundImage = HttpContext.Current.Server.MapPath(BackgroundImage);
                    c.ChartBackground = new ChartBackground(BackgroundImage);
                }
                c.ShowPieValues = false;
                c.ChartHeadline.Headline = Title;
                c.AddDataArray(values);
                c.RenderLabelBoard = TitleBoardOnChart;
                if (Grid.GotHttpContext)
                    HttpContext.Current.Session[string.Format("{0}_{1}_img", Grid.ClientID, uniqueID)] = c.Render();

                string s; //src=\"?wgchart=" + this.Grid.ColumnId + "_" + this.FormElementName + "_img\"
                if (Grid.GotHttpContext &&
                    HttpContext.Current.Request.Browser.Type.ToUpperInvariant().StartsWith("IE"))
                    s =
                        string.Format(
                            "<img height=\"{0}\" width=\"{1}\"  src=\"{2}\" style=\"filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='?wgchart={3}_{4}_img', sizingMethod='scale');\" alt=\"{5}\" border=\"0\" />",
                            Unit.Pixel(height), Unit.Pixel(width), Grid.Page.ClientScript.GetWebResourceUrl(Grid.GetType(), "WebGrid.Resources.images.transparent.gif"), Grid.ClientID, uniqueID, Title);
                else
                {
                    s =
                        string.Format("<img alt=\"{2}\" border=\"0\" src=\"?wgchart={0}_{1}_img\"/>",
                                      Grid.ClientID, uniqueID, Title);
                }
                LabelHtml(s, writer, cell);
                return;
            }
            /*
            else		// impossible!!!
            {
                return null;
            }*/
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeChartType()
        {
            return m_ChartType != ChartType.Undefined;
        }

        #endregion Methods
    }
}