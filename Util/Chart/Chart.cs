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

namespace WebGrid.Util
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Drawing.Text;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.WebControls.WebParts;

    using WebGrid.Enums;

    /// <summary>
    /// Nice charting control.
    /// </summary>
    [ToolboxData("<{0}:Chart runat=server></{0}:Chart>")]
    [ControlBuilder(typeof (MyControlBuilder))]
    [ToolboxBitmap(typeof (PageCatalogPart))]
    [Guid("470C1B65-8C5D-4bcf-A4E1-E2862F2CBCAE")]
    public class Chart : Control
    {
        #region Fields

        // internal bool IsCalculated;
        //   internal bool TrialVersion;
        private readonly ArrayList m_dataArray = new ArrayList();
        private readonly Color shadowColor = Color.FromArgb(0xaa, 0, 0, 0);

        private double _bottomValue = double.MaxValue;
        private Color[] defaultBarColors;
        private string m_ChartImageTitle;
        private ChartHeadline m_chartHeadline;
        private ChartType m_chartType = ChartType.VerticalBarChart;
        private int m_extractedPieNo = -1;
        private int m_height = 400;
        private bool m_renderLabelBoard = true;
        private bool m_showPieValues = true;
        private double m_topValue = double.MinValue;
        private int m_width = 300;
        private int spacingPx = 16;

        // Where Zero line is, set by GridRender
        private double zeroLineAt;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructor for charts.
        /// </summary>
        public Chart()
        {
            m_chartHeadline = new ChartHeadline("");
            CreateDefaultBarColors();
        }

        /// <summary>
        /// Chart constructor with headline and background.
        /// </summary>
        /// <param name="chartType">Index of which ChartType to render.</param>
        /// <param name="chartHeadline">Create a ChartHeadline object, null if there is no chart headline to render.</param>
        /// <param name="chartBackground">Create a ChartBackground object, null if there is no background (white).</param>
        /// <param name="width">The charts width.</param>
        /// <param name="height">The charts height.</param>
        public Chart(ChartType chartType, ChartHeadline chartHeadline, ChartBackground chartBackground, int width,
            int height)
        {
            m_chartType = chartType;
            if (chartHeadline == null)
            {
                chartHeadline = new ChartHeadline("");
            }
            m_chartHeadline = chartHeadline;
            ChartBackground = chartBackground;
            m_width = width;
            m_height = height;
            CreateDefaultBarColors();
        }

        /// <summary>
        /// Creates a chart with default properties.
        /// </summary>
        /// <param name="chartType">The type of chart to create.</param>
        /// <param name="width">The charts width.</param>
        /// <param name="height">The charts height.</param>
        public Chart(ChartType chartType, int width, int height)
        {
            m_chartHeadline = new ChartHeadline("");
            m_chartType = chartType;
            m_width = width;
            m_height = height;

            CreateDefaultBarColors();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the chart background.
        /// </summary>
        /// <value>The chart background.</value>
        [Bindable(true),
        Browsable(false),
        Description(@"Gets or sets the chart background."),
        Category("Chart properties")]
        public ChartBackground ChartBackground
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the chart headline.
        /// </summary>
        /// <value>The chart headline.</value>
        [Bindable(true),
        Browsable(false),
        Description(@"Gets or sets the chart headline."),
        Category("Chart properties")]
        public ChartHeadline ChartHeadline
        {
            get { return m_chartHeadline; }
            set { m_chartHeadline = value; }
        }

        /// <summary>
        /// Gets or sets the chart headline.
        /// </summary>
        /// <value>The chart headline.</value>
        [Bindable(true),
        Browsable(false),
        Description(@"Title for the Chart Image"),
        Category("Chart properties")]
        public string ChartImageTitle
        {
            get { return m_ChartImageTitle; }
            set { m_ChartImageTitle = value; }
        }

        /// <summary>
        /// Sets or gets the supported chart types to be rendered.
        /// </summary>
        /// <value>Chart type</value>
        [Bindable(true),
        Browsable(true),
        DefaultValue("PieChart3D"),
        Description(@"Sets or gets the supported chart types to be rendered."),
        Category("Chart properties")]
        public ChartType ChartType
        {
            get { return m_chartType; }
            set { m_chartType = value; }
        }

        /// <summary>
        /// Sets or gets what part of the pie diagram should be highlighted.
        /// </summary>
        /// <value>The part of the pie to highlight.</value>
        /// <remarks>
        /// This feature is only for the 2d and 3d pie diagrams.
        /// </remarks>
        [Bindable(true),
        Browsable(true),
        DefaultValue("0"),
        Description(
             @"Sets or gets what part of the pie diagram should be highlighted. This feature is only for the 2d and 3d pie diagrams."
             ),
        Category("Chart properties")]
        public int ExtractedPieNo
        {
            get { return m_extractedPieNo; }
            set { m_extractedPieNo = value; }
        }

        /// <summary>
        /// Sets or gets the heightEditableContent of the chart.
        /// </summary>
        /// <value>The heightEditableContent.</value>
        [Bindable(true),
        Browsable(true),
        DefaultValue("300"),
        Description(@"Sets or gets the height of the chart."),
        Category("Chart properties")]
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        /// <summary>
        /// Gets or sets the image path for the chart
        /// </summary>
        /// <value>The imagepath.</value>
        [Bindable(true),
        Browsable(true),
        DefaultValue("Images"),
        Description(@"Gets or sets the image path for the chart."),
        Category("Chart properties")]
        public string ImagePath
        {
            get; set;
        }

        /// <summary>
        /// Indicates whether a data label with details about the chart should be displayed.
        /// </summary>
        /// <value><c>true</c> if [render label board]; otherwise, <c>false</c>.</value>
        [Bindable(true),
        Browsable(true),
        DefaultValue("300"),
        Description(@"Indicates whether a data label with details about the chart should be displayed."),
        Category("Chart properties")]
        public bool RenderLabelBoard
        {
            get { return m_renderLabelBoard; }
            set { m_renderLabelBoard = value; }
        }

        /// <summary>
        /// Indicates whether Pie values are visibility.
        /// </summary>
        /// <value><c>true</c> if [display pie values]; otherwise, <c>false</c>.</value>
        [Bindable(true),
        Browsable(true),
        DefaultValue(false),
        Description(@"Indicates whether Pie values are visible."),
        Category("Chart properties")]
        public bool ShowPieValues
        {
            get { return m_showPieValues; }
            set { m_showPieValues = value; }
        }

        /// <summary>
        /// Sets or gets the widthEditableArea of the chart.
        /// </summary>
        /// <value>The widthEditableArea.</value>
        [Bindable(true),
        Browsable(true),
        DefaultValue("300"),
        Description(@"Sets or gets the width of the chart."),
        Category("Chart properties")]
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Add a data array or list where the values will be stored.
        /// </summary>
        /// <param name="valueList">An arraylist with Values.</param>
        public void AddDataArray(ArrayList valueList)
        {
            m_dataArray.Add(valueList);
        }

        /// <summary>
        /// Use the object settings to render a image.
        /// Return the image, null if unsuccessful.
        /// </summary>
        public System.Drawing.Image Render()
        {
            Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            Graphics gfx = Graphics.FromImage(bitmap);

            spacingPx = ChartHeadline.FontSize;
            Rectangle chartDrawArea = new Rectangle(
                spacingPx,
                spacingPx,
                Width - (spacingPx*2),
                Height - (spacingPx*2)
                );

            // render background
            if (ChartBackground != null)
            {
                ChartBackground.Render(bitmap);
            }
            else
            {
                gfx.Clear(Color.White);
            }

            // render Headline
            if (ChartHeadline != null)
            {
                // Paint the headline, returns a new rectangle where the headline area is subtracted
                chartDrawArea = ChartHeadline.Render(bitmap);
            }

            // Create a rectangle where the chart bars vil be present
            chartDrawArea = new Rectangle(
                chartDrawArea.X + spacingPx,
                chartDrawArea.Y + spacingPx,
                chartDrawArea.Width - (spacingPx*2),
                chartDrawArea.Height - (spacingPx*2) - 1
                );

            // Render Bars

            Color gridColor = ChartHeadline.HeadlineColor;

            SizeF stringSize = gfx.MeasureString("ABC1245", new Font(ChartHeadline.FontName, ChartHeadline.FontSize));

            switch (m_chartType)
            {
                case ChartType.VerticalBarChart:

                    // Make room for bar names
                    chartDrawArea.Height -= (int) stringSize.Height;

                    chartDrawArea = RenderBarGrid(bitmap, gridColor, chartDrawArea);
                    RenderBarChart(bitmap, chartDrawArea);

                    break;
                case ChartType.VerticalBarChart3D:

                    // Make room for bar names
                    chartDrawArea.Height -= (int) stringSize.Height;

                    chartDrawArea = RenderBarGrid(bitmap, gridColor, chartDrawArea);
                    Render3DBarChart(bitmap, chartDrawArea);

                    break;

                case ChartType.AreaChart:
                    if (m_renderLabelBoard) chartDrawArea = RenderAreaChartLabelBoard(bitmap, chartDrawArea);
                    chartDrawArea = RenderBarGrid(bitmap, gridColor, chartDrawArea);
                    RenderAreaChart(bitmap, chartDrawArea);
                    break;

                case ChartType.LineChart:
                    if (m_renderLabelBoard) chartDrawArea = RenderAreaChartLabelBoard(bitmap, chartDrawArea);
                    chartDrawArea = RenderBarGrid(bitmap, gridColor, chartDrawArea);
                    RenderLineChart(bitmap, chartDrawArea);
                    break;

                case ChartType.PieChart:
                    if (m_renderLabelBoard) chartDrawArea = RenderPieLabelBoard(bitmap, chartDrawArea);
                    RenderPieChart(bitmap, chartDrawArea);
                    break;

                case ChartType.PieChart3D:
                    if (m_renderLabelBoard) chartDrawArea = RenderPieLabelBoard(bitmap, chartDrawArea);
                    Render3DPieChart(bitmap, chartDrawArea);
                    break;
            }

            bitmap.MakeTransparent(Color.White);

            return bitmap;
        }

        /// <summary>
        /// Change default bar color. 
        /// </summary>
        /// <param name="index">Whitch bar to modify.</param>
        /// <param name="color">The new color.</param>
        public void SetBarColor(int index, Color color)
        {
            if (index < defaultBarColors.Length && index >= 0)
            {
                defaultBarColors[index%defaultBarColors.Length] = color;
            }
        }

        /// <summary>
        /// Creates the default bar colors for this chart. The maximum is 16.
        /// </summary>
        internal void CreateDefaultBarColors()
        {
            unchecked
            {
                defaultBarColors = new Color[16];
                defaultBarColors[0] = Color.FromArgb((int) 0xff47B354);
                defaultBarColors[1] = Color.FromArgb((int) 0xff4766B3);
                defaultBarColors[2] = Color.FromArgb((int) 0xffB34747);
                defaultBarColors[3] = Color.FromArgb((int) 0xffDEDC22);
                defaultBarColors[4] = Color.FromArgb((int) 0xff22DEDC);
                defaultBarColors[5] = Color.FromArgb((int) 0xff7A47B3);
                defaultBarColors[6] = Color.FromArgb((int) 0xffAFB347);
                defaultBarColors[7] = Color.FromArgb((int) 0xffB36847);
                defaultBarColors[8] = Color.FromArgb(0, 200, 0);
                defaultBarColors[9] = Color.FromArgb(238, 238, 238);
                defaultBarColors[10] = Color.FromArgb(255, 0, 255);
                defaultBarColors[11] = Color.FromArgb(0, 0, 255);
                defaultBarColors[12] = Color.FromArgb(0, 255, 255);
                defaultBarColors[13] = Color.FromArgb(0, 255, 0);
                defaultBarColors[14] = Color.FromArgb(255, 255, 0);
                defaultBarColors[15] = Color.FromArgb(255, 0, 0);
            }
        }

        internal Font HorizontalFont(Graphics gfx, int maxWidth)
        {
            // Find longest value text
            string longestName = string.Empty;
            for (int i = 0; i < m_dataArray.Count; i++)
            {
                ArrayList values = (ArrayList) m_dataArray[i];
                for (int j = 0; j < values.Count; j++)
                {
                    ChartValue val = (ChartValue) values[j];
                    if (val.Name.Length > longestName.Length)
                    {
                        longestName = val.Name;
                    }
                }
            }

            int size = ChartHeadline.FontSize;
            while (gfx.MeasureString(longestName, new Font(ChartHeadline.FontName, size)).Width > maxWidth && size > 4)
            {
                size--;
            }

            return new Font(ChartHeadline.FontName, size);
        }

        internal void Render3DBarChart(Bitmap bitmap, Rectangle drawArea)
        {
            Graphics gfx = Graphics.FromImage(bitmap);
            gfx.SmoothingMode = SmoothingMode.Default;

            gfx.TextRenderingHint = TextRenderingHint.AntiAlias;

            //	Font valueFont = new Font( this.ChartHeadline.FontName, this.ChartHeadline.FontSize);

            StringFormat barstringformat = new StringFormat {Alignment = StringAlignment.Center};

            int barsGroup = 0;
            int totalBars = 0;
            for (int linesChart = 0; linesChart < m_dataArray.Count; linesChart++)
            {
                ArrayList values = (ArrayList) m_dataArray[linesChart];
                barsGroup = values.Count;
                totalBars += barsGroup;
            }

            // messure Horizontal font size
            Font horizontalValueFont = HorizontalFont(gfx, drawArea.Width/barsGroup);

            int barW = (drawArea.Width)/(totalBars + barsGroup);
            int barZ = barW/2;
            int barXpos = drawArea.X + (barW/2);

            // Ground shadow

            Brush shadow = new SolidBrush(shadowColor);

            int shadowX = drawArea.X + 2;
            int shadowY = (int) (drawArea.Y + drawArea.Height - zeroLineAt);

            Point[] polyABD = new Point[3];
            polyABD[0] = new Point(shadowX, shadowY);
            polyABD[1] = new Point(shadowX + barZ, shadowY - barZ);
            polyABD[2] = new Point(shadowX + barZ, shadowY);
            gfx.FillPolygon(shadow, polyABD);

            Point[] polyBCD = new Point[3];
            polyBCD[0] = new Point(shadowX + drawArea.Width, shadowY);
            polyBCD[1] = new Point(shadowX + drawArea.Width + barZ, shadowY - barZ);
            polyBCD[2] = new Point(shadowX + drawArea.Width, shadowY - barZ);
            gfx.FillPolygon(shadow, polyBCD);

            gfx.FillRectangle(shadow, shadowX + barZ, shadowY - barZ, drawArea.Width - barZ, barZ);

            barZ = barW/3;

            for (int i = 0; i < barsGroup; i++)
            {
                for (int linesChart = 0; linesChart < m_dataArray.Count; linesChart++)
                {
                    ArrayList values = (ArrayList) m_dataArray[linesChart];

                    ChartValue val = (ChartValue) values[i];

                    int barH = (int) Math.Abs(val._percent*drawArea.Height);

                    int yP = 0;
                    if (val._percent < 0)
                    {
                        yP = barH + barZ;
                    }

                    Rectangle bar = new Rectangle(
                        barXpos,
                        (int) (drawArea.Y + drawArea.Height - barH + yP - zeroLineAt - 1),
                        barW - 2,
                        barH
                        );

                    barXpos += barW;

                    int colNo = linesChart;
                    if (m_dataArray.Count == 1)
                    {
                        colNo = i;
                    }

                    Color barColor = defaultBarColors[(colNo)%defaultBarColors.Length];

                    // Light
                    //	Pen light = new Pen( Color.FromArgb(0xcc, 0xff, 0xff, 0xff) ,1);
                    // dark
                    Pen dark = new Pen(shadowColor, 1);

                    SolidBrush brush = new SolidBrush(barColor);

                    SolidBrush shadowBrush = new SolidBrush(
                        Color.FromArgb(
                            barColor.A,
                            Math.Max(barColor.R - 40, 0),
                            Math.Max(barColor.G - 40, 0),
                            Math.Max(barColor.B - 40, 0)
                            )
                        );

                    gfx.FillRectangle(
                        brush,
                        bar
                        );

                    byte[] types = new byte[4];
                    types[0] = (byte) PathPointType.Start;
                    types[1] = (byte) PathPointType.Line;
                    types[2] = (byte) PathPointType.Line;
                    types[3] = (byte) PathPointType.Line;

                    // Top shadow fill
                    Point[] pathTopPoints = new Point[4];
                    pathTopPoints[0] = new Point(bar.X, bar.Y);
                    pathTopPoints[1] = new Point(bar.X + barZ, bar.Y - barZ);
                    pathTopPoints[2] = new Point(pathTopPoints[1].X + bar.Width, pathTopPoints[1].Y);
                    pathTopPoints[3] = new Point(pathTopPoints[0].X + bar.Width, pathTopPoints[0].Y);
                    GraphicsPath pathTop = new GraphicsPath(pathTopPoints, types);
                    gfx.FillPath(shadowBrush, pathTop);

                    // Rightside  shadow fill
                    Point[] pathSidePoints = new Point[4];
                    pathSidePoints[0] = new Point(bar.X + bar.Width, bar.Y);
                    pathSidePoints[1] = new Point(bar.X + barZ + bar.Width, bar.Y - barZ);
                    pathSidePoints[2] = new Point(pathSidePoints[1].X, pathSidePoints[1].Y + bar.Height);
                    pathSidePoints[3] = new Point(pathSidePoints[0].X, pathSidePoints[0].Y + bar.Height);
                    GraphicsPath pathSide = new GraphicsPath(pathSidePoints, types);
                    gfx.FillPath(shadowBrush, pathSide);

                    // Out line with lines
                    gfx.DrawRectangle(
                        dark,
                        bar
                        );

                    gfx.DrawPath(dark, pathTop);
                    gfx.DrawPath(dark, pathSide);

                    // Bar name

                    if (linesChart != m_dataArray.Count/2) continue;
                    int tx = bar.X + (bar.Width/2);
                    int ty = drawArea.Y + drawArea.Height + (int) (horizontalValueFont.Size/3);

                    // (Set same Alpha for shadow)
                    gfx.DrawString(
                        val.Name,
                        horizontalValueFont,
                        new SolidBrush(shadowColor),
                        tx + (horizontalValueFont.Size/10),
                        ty + (horizontalValueFont.Size/10),
                        barstringformat);

                    gfx.DrawString(
                        val.Name,
                        horizontalValueFont,
                        new SolidBrush(m_chartHeadline.HeadlineColor),
                        tx,
                        ty,
                        barstringformat);
                }

                barXpos += barW;
            }
        }

        internal void Render3DPieChart(Bitmap bitmap, Rectangle drawArea)
        {
            Graphics gfx = Graphics.FromImage(bitmap);
            gfx.SmoothingMode = SmoothingMode.AntiAlias;
            gfx.TextRenderingHint = TextRenderingHint.AntiAlias;

            Pen contour = new Pen(Color.Black, 1.5f);

            int pieRows = (int) (Math.Sqrt(m_dataArray.Count)*10)/10;
            if (pieRows == 0) pieRows = 1;
            int pieCols = m_dataArray.Count/pieRows;

            int pieDiam = Math.Min((drawArea.Width/pieCols), (drawArea.Height/pieRows));

            if (m_extractedPieNo >= 0)
            {
                pieDiam -= (pieDiam/9);
            }

            int zdepth = (pieDiam/5);

            Font valueFont = VerticalFont(gfx, pieDiam/(((ArrayList) m_dataArray[0]).Count));
            StringFormat stringformat = new StringFormat {Alignment = StringAlignment.Center};

            int countCols = 0;

            // Caluclate percent value for each pie
            for (int pies = 0; pies < m_dataArray.Count; pies++)
            {
                ArrayList values = (ArrayList) m_dataArray[pies];

                double totalvalue = 0.0;
                foreach (ChartValue v in values)
                    totalvalue += Math.Abs(v.Value);

                foreach (ChartValue v in values)
                    v._percent = Math.Abs(v.Value)/totalvalue;

                int pieX = drawArea.X + (drawArea.Width/2) - (pieDiam/2);
                int pieY = drawArea.Y + (drawArea.Height/2) - (pieDiam/2);

                float pieSliceSize;
                float startDegree;

                for (int z = 0; z <= zdepth; z++)
                {
                    startDegree = 0;

                    for (int i = 0; i < values.Count; i++)
                    {
                        ChartValue currentValue = (ChartValue) values[i];

                        Color pieSliceColor = defaultBarColors[i%defaultBarColors.Length];

                        SolidBrush pieFill = z == zdepth ? new SolidBrush(pieSliceColor) : new SolidBrush(
                                                                                               Color.FromArgb(
                                                                                                   pieSliceColor.A,
                                                                                                   Math.Max(pieSliceColor.R - 40, 0),
                                                                                                   Math.Max(pieSliceColor.G - 40, 0),
                                                                                                   Math.Max(pieSliceColor.B - 40, 0)
                                                                                                   ));

                        pieSliceSize = (float) (currentValue._percent*360.0);

                        float x = pieX;
                        float y = (pieY + zdepth) - z;

                        double radianValue = (startDegree + (pieSliceSize/2))*(Math.PI/180.0);

                        float widthDiam = pieDiam;
                        float heightDiam = pieDiam - zdepth;

                        if (m_extractedPieNo >= 0 && i == m_extractedPieNo)
                        {
                            x += (float) (Math.Cos(radianValue)*(widthDiam/10.0));
                            y += (float) (Math.Sin(radianValue)*(heightDiam/10.0));
                        }

                        gfx.FillPie(pieFill, x, y, pieDiam, pieDiam - zdepth, startDegree, pieSliceSize);

                        gfx.DrawArc(contour, x, y, pieDiam, pieDiam - zdepth, startDegree, contour.Width/3);
                        gfx.DrawArc(contour, x, y, pieDiam, pieDiam - zdepth,
                                    startDegree + pieSliceSize - (contour.Width/4), contour.Width/3);

                        if (z <= 1 || z == zdepth)
                        {
                            gfx.DrawPie(contour, x, y, pieDiam, pieDiam - zdepth, startDegree, pieSliceSize);
                        }

                        startDegree += pieSliceSize;
                    }
                }

                // Draw values of pie's
                startDegree = 0;
                for (int i = 0; i < values.Count; i++)
                {
                    ChartValue currentValue = (ChartValue) values[i];

                    pieSliceSize = (float) (currentValue._percent*360.0);

                    //	int x = pieX;
                    //	int y = pieY;

                    double radianValue = (startDegree + (pieSliceSize/2))*(Math.PI/180.0);

                    ChartValue cv = (ChartValue) values[i];

                    float xt = (float) (drawArea.X + (drawArea.Width/2) + (Math.Cos(radianValue)*((float)pieDiam/3)));
                    float yt =
                        (float)(drawArea.Y + (drawArea.Height / 2) + (Math.Sin(radianValue) * ((pieDiam - (float)zdepth) / 3))) -
                        (valueFont.Size / 2) - ((float)zdepth / 2);

                    double v = ((int) (cv._value*1000))/1000.0;
                    string text = v.ToString("N0");

                    if (!m_showPieValues)
                    {
                        text = cv.Name;
                    }

                    gfx.DrawString(
                        text,
                        valueFont,
                        new SolidBrush(shadowColor),
                        xt + (valueFont.Size/10),
                        yt + (valueFont.Size/10),
                        stringformat);

                    gfx.DrawString(
                        text,
                        valueFont,
                        new SolidBrush(m_chartHeadline.HeadlineColor),
                        xt,
                        yt,
                        stringformat);

                    startDegree += pieSliceSize;
                }

                countCols++;

            /*
                pieX += pieDiam + spacingPx;
            */
                if (countCols >= pieCols)
                {
                    countCols = 0;

                }
            }
        }

        internal void RenderAreaChart(Bitmap bitmap, Rectangle drawArea)
        {
            Graphics gfx = Graphics.FromImage(bitmap);
            gfx.SmoothingMode = SmoothingMode.HighSpeed;

            int areaWith = drawArea.Width - spacingPx*2;
            int[] lastAreaY = new int[areaWith];

            // Fill ereas

            for (int linesChart = 0; linesChart < m_dataArray.Count; linesChart++)
            {
                ArrayList values = (ArrayList) m_dataArray[linesChart];

                int barW = areaWith/(values.Count - 1);

                Color penCol = defaultBarColors[linesChart%defaultBarColors.Length];

                // A lighter color for the dots
                Pen areaFill = new Pen(
                    Color.FromArgb(
                        0xdd,
                        Math.Min(penCol.R + 1, 0xff),
                        Math.Min(penCol.G + 1, 0xff),
                        Math.Min(penCol.B + 1, 0xff)
                        ), 1
                    );

                int lastAreaYIndex = 0;

                for (int i = 0; i < values.Count - 1; i++)
                {
                    ChartValue val = (ChartValue) values[i];

                    int px1 = drawArea.X + (i*barW) + spacingPx;
                    int py1 = (int) (drawArea.Y + drawArea.Height - (val._percent*drawArea.Height) - zeroLineAt);

                    ChartValue val2 = (ChartValue) values[i + 1];

                    int px2 = drawArea.X + ((i + 1)*barW) + spacingPx;
                    int py2 = (int) (drawArea.Y + drawArea.Height - (val2._percent*drawArea.Height) - zeroLineAt);

                    float y = py1;
                    float ystp = (py2 - py1)/(float) (px2 - px1);

                    for (int x = px1; x < px2; x++)
                    {
                        int ry1 = (int) y;
                        int ry2;

                        if (linesChart == 0)
                        {
                            ry2 = (int) (drawArea.Y + drawArea.Height - zeroLineAt);
                            if (py1 > zeroLineAt)
                            {
                                ry2--;
                            }
                            else
                            {
                                ry2++;
                            }
                        }
                        else
                        {
                            ry2 = lastAreaY[lastAreaYIndex];
                        }

                        lastAreaY[lastAreaYIndex] = ry1;
                        lastAreaYIndex++;

                        gfx.DrawLine(areaFill,
                                     x,
                                     ry1,
                                     x,
                                     ry2
                            );

                        y += ystp;
                    }
                }
            }

            // Draw lighter seperation lines

            gfx.SmoothingMode = SmoothingMode.AntiAlias;

            for (int linesChart = 0; linesChart < m_dataArray.Count; linesChart++)
            {
                ArrayList values = (ArrayList) m_dataArray[linesChart];

                int barW = areaWith/(values.Count - 1);

                Color penCol = defaultBarColors[linesChart%defaultBarColors.Length];
                Pen linepen = new Pen(
                    Color.FromArgb(
                        0xff,
                        Math.Min(penCol.R + 50, 0xff),
                        Math.Min(penCol.G + 50, 0xff),
                        Math.Min(penCol.B + 50, 0xff)
                        ), 2);

                for (int i = 0; i < values.Count - 1; i++)
                {
                    ChartValue val = (ChartValue) values[i];

                    int px1 = drawArea.X + (i*barW) + spacingPx;
                    int py1 = (int) (drawArea.Y + drawArea.Height - (val._percent*drawArea.Height) - zeroLineAt);

                    ChartValue val2 = (ChartValue) values[i + 1];

                    int px2 = drawArea.X + ((i + 1)*barW) + spacingPx;
                    int py2 = (int) (drawArea.Y + drawArea.Height - (val2._percent*drawArea.Height) - zeroLineAt);

                    gfx.DrawLine(
                        linepen,
                        px1,
                        py1,
                        px2,
                        py2
                        );
                }
            }
        }

        internal Rectangle RenderAreaChartLabelBoard(Bitmap bitmap, Rectangle drawArea)
        {
            Graphics gfx = Graphics.FromImage(bitmap);
            gfx.TextRenderingHint = TextRenderingHint.AntiAlias;

            int size = ChartHeadline.FontSize;

            Font valueFont = new Font(ChartHeadline.FontName, size);

            while (size > 8 &&
                   (gfx.MeasureString("ABCD1234", valueFont).Height*m_dataArray.Count) >
                   (drawArea.Height - (spacingPx*2)))
            {
                size -= 4;
                valueFont = new Font(ChartHeadline.FontName, size);
            }

            // Find the largest name
            SizeF stringSize = new SizeF(0, 0);

            foreach (ArrayList values in m_dataArray)
            {
                ChartValue cVal = (ChartValue) values[0];

                SizeF check = gfx.MeasureString(string.Format("{0}", cVal.Name), valueFont);
                if (stringSize.Width < check.Width)
                {
                    stringSize = check;
                }
            }

            Rectangle sample = new Rectangle(0, 0, (int) stringSize.Height, (int) stringSize.Height);

            Rectangle labelBoard =
                new Rectangle(0, 0, (int) (stringSize.Width + sample.Width + spacingPx*2),
                              (sample.Height + spacingPx)*m_dataArray.Count + spacingPx);
            labelBoard.X = ((drawArea.X + drawArea.Width) - labelBoard.Width) + (spacingPx/2);
            labelBoard.Y = drawArea.Y + ((drawArea.Height - labelBoard.Height)/2);

            drawArea.Width -= labelBoard.Width;

            gfx.FillRectangle(new SolidBrush(shadowColor), labelBoard.X + 2, labelBoard.Y + 2, labelBoard.Width,
                              labelBoard.Height);
            gfx.FillRectangle(new SolidBrush(Color.White), labelBoard);
            //	gfx.FillRectangle(new SolidBrush(Color.Transparent),labelBoard);
            gfx.DrawRectangle(new Pen(Color.Black, 1), labelBoard);

            int x = labelBoard.X + spacingPx;
            int y = labelBoard.Y + spacingPx;

            int lineSize = (labelBoard.Height - spacingPx)/m_dataArray.Count;

            StringFormat stringFormat = new StringFormat();

            for (int i = 0; i < m_dataArray.Count; i++)
            {
                ArrayList values = (ArrayList) m_dataArray[i];
                ChartValue cVal = (ChartValue) values[0];
                Color sampleColor = defaultBarColors[i%defaultBarColors.Length];

                gfx.FillRectangle(new SolidBrush(shadowColor), x + 2, y + 2, sample.Width, sample.Height);
                gfx.FillRectangle(new SolidBrush(sampleColor), x, y, sample.Width, sample.Height);
                gfx.DrawRectangle(new Pen(Color.Black, 1), x, y, sample.Width, sample.Height);

                int tx = x + sample.Width + (spacingPx/4);
                int ty = y + 4;

                // (Set same Alpha for shadow)
                gfx.DrawString(
                    cVal.Name,
                    valueFont,
                    new SolidBrush(Color.Black),
                    tx + (valueFont.Size/10),
                    ty + (valueFont.Size/10),
                    stringFormat);

                gfx.DrawString(
                    cVal.Name,
                    valueFont,
                    new SolidBrush(sampleColor),
                    tx,
                    ty,
                    stringFormat);

                y += lineSize;
            }

            return drawArea;
        }

        internal void RenderBarChart(Bitmap bitmap, Rectangle drawArea)
        {
            Graphics gfx = Graphics.FromImage(bitmap);
            gfx.SmoothingMode = SmoothingMode.AntiAlias;

            //	Font valueFont = new Font( this.ChartHeadline.FontName, this.ChartHeadline.FontSize);

            StringFormat barstringformat = new StringFormat {Alignment = StringAlignment.Center};

            gfx.TextRenderingHint = TextRenderingHint.AntiAlias;

            int barsGroup = 0;
            int totalBars = 0;
            for (int linesChart = 0; linesChart < m_dataArray.Count; linesChart++)
            {
                ArrayList values = (ArrayList) m_dataArray[linesChart];
                barsGroup = values.Count;
                totalBars += barsGroup;
            }

            // messure Horizontal font size
            Font horizontalValueFont = HorizontalFont(gfx, drawArea.Width/barsGroup);

            // Other stuff

            int barW = (drawArea.Width)/(totalBars + barsGroup);
            int barXpos = drawArea.X + (barW/2);

            for (int i = 0; i < barsGroup; i++)
            {
                for (int linesChart = 0; linesChart < m_dataArray.Count; linesChart++)
                {
                    ArrayList values = (ArrayList) m_dataArray[linesChart];

                    ChartValue val = (ChartValue) values[i];

                    int barH = (int) Math.Abs(val._percent*drawArea.Height);

                    int yP = 0;
                    if (val._percent < 0)
                    {
                        yP = barH;
                    }

                    Rectangle bar = new Rectangle(
                        barXpos,
                        (int) (drawArea.Y + drawArea.Height - barH + yP - zeroLineAt),
                        barW - 1,
                        barH
                        );

                    barXpos += barW;

                    // The shadow

                    Brush brush;
                    unchecked
                    {
                        brush = new SolidBrush(shadowColor);
                    }

                    int shadowCordX = (bar.Width/10) + 1;
                    int shadowCordY = (bar.Width/10) + 1;
                    int shadowCut = shadowCordY;

                    if (val._percent < 0)
                    {
                        shadowCut = - shadowCordY;
                        shadowCordY = 1;
                    }

                    gfx.FillRectangle(
                        brush,
                        bar.X + shadowCordX,
                        bar.Y + shadowCordY,
                        bar.Width,
                        bar.Height - shadowCut
                        );

                    int colNo = linesChart;
                    if (m_dataArray.Count == 1)
                    {
                        colNo = i;
                    }

                    // The main bar
                    gfx.FillRectangle(
                        new SolidBrush(defaultBarColors[(colNo)%defaultBarColors.Length]),
                        bar
                        );

                    // Border
                    gfx.DrawRectangle(
                        new Pen(Color.Black, 1),
                        bar
                        );

                    // Light
                    Pen light = new Pen(Color.FromArgb(0xcc, 0xff, 0xff, 0xff), 1);

                    gfx.DrawLine(
                        light,
                        bar.X,
                        bar.Y,
                        bar.X,
                        bar.Y + bar.Height - 1
                        );
                    gfx.DrawLine(
                        light,
                        bar.X + 1,
                        bar.Y,
                        bar.X + bar.Width - 1,
                        bar.Y
                        );

                    // Bar name

                    if (linesChart != m_dataArray.Count/2) continue;
                    int tx = bar.X + (bar.Width/2);
                    int ty = drawArea.Y + drawArea.Height + (int) (horizontalValueFont.Size/3);

                    // (Set same Alpha for shadow)
                    gfx.DrawString(
                        val.Name,
                        horizontalValueFont,
                        new SolidBrush(shadowColor),
                        tx + (horizontalValueFont.Size/10),
                        ty + (horizontalValueFont.Size/10),
                        barstringformat);

                    gfx.DrawString(
                        val.Name,
                        horizontalValueFont,
                        new SolidBrush(m_chartHeadline.HeadlineColor),
                        tx,
                        ty,
                        barstringformat);
                }

                barXpos += barW;
            }
        }

        internal Rectangle RenderBarGrid(Bitmap bitmap, Color color, Rectangle drawArea)
        {
            Graphics gfx = Graphics.FromImage(bitmap);
            gfx.TextRenderingHint = TextRenderingHint.AntiAlias;
            Pen simplePen = new Pen(color, 1);
            Pen shadowPen = new Pen(shadowColor, 1);

            for (int linesChart = 0; linesChart < m_dataArray.Count; linesChart++)
            {
                ArrayList values = (ArrayList) m_dataArray[linesChart];

                // Find max and min
                foreach (ChartValue cVal in values)
                {
                    if (cVal.Value > m_topValue)
                    {
                        m_topValue = cVal.Value;
                    }
                    if (cVal.Value < _bottomValue)
                    {
                        _bottomValue = cVal.Value;
                    }
                }
            }

            float totalHeight = (float) (Math.Abs(m_topValue));
            if (Math.Abs(_bottomValue) > totalHeight)
            {
                totalHeight = (float) Math.Abs(_bottomValue);
            }

            // size of values
            double maxValueSize = 0.1;
            double tempTop = totalHeight;
            while (tempTop > 1.0)
            {
                tempTop = tempTop/10.0;
                maxValueSize = maxValueSize*10;
            }

            double highestChartGridValue = 0;
            if (m_topValue > 0)
            {
                highestChartGridValue = ((int) (m_topValue/maxValueSize) + 1);
            }

            double lowestChartGridValue = 0;
            if (_bottomValue < 0)
            {
                lowestChartGridValue = ((int) (_bottomValue/maxValueSize) - 1);
            }

            // Calulate number of Horizontal lines
            int horLines = (int) (highestChartGridValue + Math.Abs(lowestChartGridValue));

            if (horLines == 0)
            {
                horLines = 1;
            }

            while (drawArea.Height/horLines > drawArea.Height/4 && horLines < drawArea.Height/5)
            {
                horLines = horLines*2;
            }

            // Calculate fontsize for the values
            int fontValueSize = ChartHeadline.FontSize;
            if (fontValueSize < 8) fontValueSize = 8;
            Font valueFont = VerticalFont(gfx, drawArea.Height/horLines);

            // Find the max size of the text
            SizeF stringSize = gfx.MeasureString(string.Format("{0}", totalHeight), valueFont);

            double horLineSpace = drawArea.Height/(double) horLines;
            // Value step for each horline
            //	double horLineValueSpacing = maxValueSize;

            double horLineValue = lowestChartGridValue*maxValueSize;
            double horlineValueStep = ((highestChartGridValue + Math.Abs(lowestChartGridValue))*maxValueSize)/horLines;

            gfx.DrawLine(
                shadowPen,
                drawArea.X + stringSize.Width + 1,
                drawArea.Y + 1,
                drawArea.X + stringSize.Width + 1,
                drawArea.Y + drawArea.Height + 1
                );

            for (int horLine = 0; horLine <= horLines; horLine++)
            {
                double y = (drawArea.Y + drawArea.Height) - (horLineSpace*horLine);

                int x1 = (int) (drawArea.X + stringSize.Width);
                int y1 = (int) y;

                int x2 = drawArea.X + drawArea.Width;
                int y2 = y1;

                gfx.DrawLine(
                    shadowPen,
                    x1 + 1,
                    y1 + 1,
                    x2 + 1,
                    y2 + 1
                    );

                gfx.DrawLine(
                    simplePen,
                    x1 + 1,
                    y1,
                    x2,
                    y2
                    );

                float tx = (drawArea.X + stringSize.Width) - gfx.MeasureString("" + horLineValue, valueFont).Width;
                float ty = (float) (y - (+ stringSize.Height/2));

                // (Set same Alpha for shadow)
                gfx.DrawString(
                    horLineValue.ToString("N0"),
                    valueFont,
                    new SolidBrush(shadowColor),
                    tx + (valueFont.Size/10),
                    ty + (valueFont.Size/10),
                    new StringFormat());

                gfx.DrawString(
                    horLineValue.ToString("N0"),
                    valueFont,
                    new SolidBrush(color),
                    tx,
                    ty,
                    new StringFormat());

                horLineValue += horlineValueStep;
            }

            gfx.DrawLine(
                simplePen,
                drawArea.X + stringSize.Width,
                drawArea.Y,
                drawArea.X + stringSize.Width,
                drawArea.Y + drawArea.Height
                );

            for (int linesChart = 0; linesChart < m_dataArray.Count; linesChart++)
            {
                ArrayList values = (ArrayList) m_dataArray[linesChart];

                foreach (ChartValue val in values)
                {
                    val._percent = val.Value/(horLines*horlineValueStep);
                }
            }

            // Where Zero is
            zeroLineAt = horLineSpace*Math.Abs(lowestChartGridValue);

            return
                new Rectangle((int) (drawArea.X + stringSize.Width), drawArea.Y,
                              (int) (drawArea.Width - stringSize.Width), drawArea.Height);
        }

        internal void RenderLineChart(Bitmap bitmap, Rectangle drawArea)
        {
            Graphics gfx = Graphics.FromImage(bitmap);

            gfx.SmoothingMode = SmoothingMode.AntiAlias;

            int lineThickness = (drawArea.Width/200) + 1;

            for (int linesChart = 0; linesChart < m_dataArray.Count; linesChart++)
            {
                ArrayList values = (ArrayList) m_dataArray[linesChart];

                int barW = (drawArea.Width - spacingPx*2)/(values.Count - 1);

                Color penCol = Color.FromArgb(
                    0xff,
                    defaultBarColors[linesChart%defaultBarColors.Length].R,
                    defaultBarColors[linesChart%defaultBarColors.Length].G,
                    defaultBarColors[linesChart%defaultBarColors.Length].B
                    );

                Pen linepen = new Pen(penCol, lineThickness);
                Pen lineShadow = new Pen(shadowColor, lineThickness);

                // A lighter color for the dots
                SolidBrush areaFill = new SolidBrush(
                    Color.FromArgb(
                        0xff,
                        Math.Min(penCol.R + 50, 0xff),
                        Math.Min(penCol.G + 50, 0xff),
                        Math.Min(penCol.B + 50, 0xff)
                        )
                    );

                SolidBrush areaFillShadow = new SolidBrush(shadowColor);

                for (int i = 0; i < values.Count - 1; i++)
                {
                    ChartValue val = (ChartValue) values[i];

                    int px1 = drawArea.X + (i*barW) + spacingPx;
                    int py1 = (int) (drawArea.Y + drawArea.Height - (val._percent*drawArea.Height) - zeroLineAt);

                    ChartValue val2 = (ChartValue) values[i + 1];

                    int px2 = drawArea.X + ((i + 1)*barW) + spacingPx;
                    int py2 = (int) (drawArea.Y + drawArea.Height - (val2._percent*drawArea.Height) - zeroLineAt);

                    //	float y = py1;
                    //	float ystp = (py2 - py1) / (float)(px2-px1);

                    gfx.DrawLine(lineShadow,
                                 px1,
                                 py1 + lineThickness/2,
                                 px2,
                                 py2 + lineThickness/2
                        );

                    gfx.DrawLine(linepen,
                                 px1,
                                 py1,
                                 px2,
                                 py2
                        );
                }

                for (int i = 0; i < values.Count; i++)
                {
                    ChartValue val = (ChartValue) values[i];

                    int px1 = drawArea.X + (i*barW) + spacingPx;
                    int py1 = (int) (drawArea.Y + drawArea.Height - (val._percent*drawArea.Height) - zeroLineAt);

                    int circleSize = (lineThickness*2);

                    gfx.FillEllipse(
                        areaFillShadow,
                        px1 - (circleSize/2),
                        py1 - (circleSize/2) + lineThickness/2,
                        circleSize,
                        circleSize
                        );

                    gfx.FillEllipse(
                        areaFill,
                        px1 - (circleSize/2),
                        py1 - (circleSize/2),
                        circleSize,
                        circleSize
                        );
                }
            }
        }

        internal void RenderPieChart(Bitmap bitmap, Rectangle drawArea)
        {
            Graphics gfx = Graphics.FromImage(bitmap);
            gfx.SmoothingMode = SmoothingMode.AntiAlias;

            Pen contour = new Pen(Color.Black, 1);

            int pieRows = (int) (Math.Sqrt(m_dataArray.Count)*10)/10;
            if (pieRows == 0) pieRows = 1;
            int pieCols = m_dataArray.Count/pieRows;

            int pieDiam = Math.Min((drawArea.Width/pieCols), (drawArea.Height/pieRows));

            if (m_extractedPieNo >= 0)
            {
                pieDiam -= (pieDiam/9);
            }

            Font valueFont = VerticalFont(gfx, pieDiam/( ((ArrayList)m_dataArray[0]).Count ));
            StringFormat stringformat = new StringFormat {Alignment = StringAlignment.Center};

            int countCols = 0;
            // Caluclate percent value for each pie
            for (int pies = 0; pies < m_dataArray.Count; pies++)
            {
                ArrayList values = (ArrayList) m_dataArray[pies];

                double totalvalue = 0.0;
                foreach (ChartValue v in values)
                {
                    totalvalue += Math.Abs(v.Value);
                }

                foreach (ChartValue v in values)
                {
                    v._percent = Math.Abs(v.Value/totalvalue);
                }

                int pieX = drawArea.X + (drawArea.Width/2) - (pieDiam/2);
                int pieY = drawArea.Y + (drawArea.Height/2) - (pieDiam/2);

                double pieSliceSize;
                double startDegree;

                for (int shadowloop = 0; shadowloop < 2; shadowloop++)
                {
            /*
                    pieSliceSize = 0;
            */
                    startDegree = 0;

                    for (int i = 0; i < values.Count; i++)
                    {
                        ChartValue currentValue = (ChartValue) values[i];

                        Color pieSliceColor = defaultBarColors[i%defaultBarColors.Length];

                        pieSliceSize = currentValue._percent*360.0;

                        float x = pieX;
                        float y = pieY;

                        double radianValue = (startDegree + (pieSliceSize/2))*(Math.PI/180.0);

                        if (m_extractedPieNo >= 0 && i == m_extractedPieNo)
                        {
                            x += (float)(Math.Cos(radianValue) * ((float)pieDiam / 10));
                            y += (float)(Math.Sin(radianValue) * ((float)pieDiam / 10));
                        }

                        if (shadowloop == 0)
                            gfx.FillPie(new SolidBrush(Color.Black), x, y + ((float) pieDiam/70) + 1, pieDiam, pieDiam,
                                        (float) startDegree, (float) pieSliceSize);
                        else
                        {
                            gfx.FillPie(new SolidBrush(pieSliceColor), x, y, pieDiam, pieDiam, (float) startDegree,
                                        (float) pieSliceSize);
                            gfx.DrawPie(contour, x, y, pieDiam, pieDiam, (float) startDegree, (float) pieSliceSize);
                        }

                        startDegree += pieSliceSize;
                    }
                }

                // Draw values of pie's
                pieSliceSize = 0;
                startDegree = 0;
                for (int i = 0; i < values.Count; i++)
                {
                    ChartValue currentValue = (ChartValue) values[i];

                    pieSliceSize = currentValue._percent*360.0;

                    //	int x = pieX;
                    //	int y = pieY;

                    double radianValue = (startDegree + (pieSliceSize/2))*(Math.PI/180.0);

                    ChartValue cv = (ChartValue) values[i];

                    float xt = (float)(drawArea.X + (drawArea.Width / 2) + (Math.Cos(radianValue) * ((float)pieDiam / 3)));
                    float yt = (float)(drawArea.Y + (drawArea.Height / 2) + (Math.Sin(radianValue) * ((float)pieDiam / 3))) -
                               (valueFont.Size/2);

                    double v = ((int) (cv._value*1000))/1000.0;
                    string text = v.ToString("N0");

                    if (!m_showPieValues)
                    {
                        text = cv.Name;
                    }

                    gfx.DrawString(
                        text,
                        valueFont,
                        new SolidBrush(shadowColor),
                        xt + (valueFont.Size/10),
                        yt + (valueFont.Size/10),
                        stringformat);

                    gfx.DrawString(
                        text,
                        valueFont,
                        new SolidBrush(m_chartHeadline.HeadlineColor),
                        xt,
                        yt,
                        stringformat);

                    startDegree += pieSliceSize;
                }

                countCols++;

               // pieX += pieDiam + spacingPx;
                if (countCols >= pieCols)
                    countCols = 0;
            }
        }

        internal Rectangle RenderPieLabelBoard(Bitmap bitmap, Rectangle drawArea)
        {
            Graphics gfx = Graphics.FromImage(bitmap);
            gfx.TextRenderingHint = TextRenderingHint.AntiAlias;

            ArrayList values = (ArrayList) m_dataArray[0];

            Font valueFont = new Font(ChartHeadline.FontName, (((float)drawArea.Height / 3) / values.Count));

            // Find the largest name
            SizeF stringSize = new SizeF(0, 0);

            foreach (ChartValue cVal in values)
            {
                SizeF check = gfx.MeasureString(string.Format("{0}", cVal.Name), valueFont);
                if (stringSize.Width < check.Width)
                {
                    stringSize = check;
                }
            }

            Rectangle sample = new Rectangle(0, 0, (int) stringSize.Height, (int) stringSize.Height);

            Rectangle labelBoard =
                new Rectangle(0, 0, (int) (stringSize.Width + sample.Width + spacingPx*2),
                              (int) ((stringSize.Height + spacingPx)*values.Count) + spacingPx);
            labelBoard.X = (drawArea.X + drawArea.Width) - labelBoard.Width;
            labelBoard.Y = drawArea.Y + ((drawArea.Height - labelBoard.Height)/2);

            drawArea.Width -= labelBoard.Width;

            gfx.FillRectangle(new SolidBrush(shadowColor), labelBoard.X + 2, labelBoard.Y + 2, labelBoard.Width,
                              labelBoard.Height);
            gfx.FillRectangle(new SolidBrush(Color.White), labelBoard);
            //	gfx.FillRectangle(new SolidBrush(Color.Transparent),labelBoard);
            gfx.DrawRectangle(new Pen(Color.Black, 1), labelBoard);

            int x = labelBoard.X + spacingPx;
            int y = labelBoard.Y + spacingPx;

            int lineSize = (labelBoard.Height - spacingPx)/values.Count;

            StringFormat stringFormat = new StringFormat();

            for (int i = 0; i < values.Count; i++)
            {
                ChartValue cVal = (ChartValue) values[i];
                Color sampleColor = defaultBarColors[i%defaultBarColors.Length];

                gfx.FillRectangle(new SolidBrush(shadowColor), x + 2, y + 2, sample.Width, sample.Height);
                gfx.FillRectangle(new SolidBrush(sampleColor), x, y, sample.Width, sample.Height);
                gfx.DrawRectangle(new Pen(Color.Black, 1), x, y, sample.Width, sample.Height);

                int tx = x + sample.Width + (spacingPx/4);
                int ty = y - 2;

                // (Set same Alpha for shadow)
                gfx.DrawString(
                    cVal.Name,
                    valueFont,
                    new SolidBrush(Color.Black),
                    tx + (valueFont.Size/10),
                    ty + (valueFont.Size/10),
                    stringFormat);

                gfx.DrawString(
                    cVal.Name,
                    valueFont,
                    new SolidBrush(sampleColor),
                    tx,
                    ty,
                    stringFormat);

                y += lineSize;
            }

            return drawArea;
        }

        /// <summary>
        /// Vertical adjusting. This function increases the text-size widthEditableArea the chart image size.
        /// </summary>
        /// <param name="gfx">The grap being generated.</param>
        /// <param name="maxHeight">The maximum heightEditableContent of the graph.</param>
        /// <returns>the fontsize</returns>
        internal Font VerticalFont(Graphics gfx, int maxHeight)
        {
            // Find longest value text
            string longestName = string.Empty;
            for (int i = 0; i < m_dataArray.Count; i++)
            {
                ArrayList values = (ArrayList) m_dataArray[i];
                for (int j = 0; j < values.Count; j++)
                {
                    ChartValue val = (ChartValue) values[j];
                    if (val.Name.Length > longestName.Length)
                    {
                        longestName = val.Name;
                    }
                }
            }

            int size = ChartHeadline.FontSize;
            while (gfx.MeasureString(longestName, new Font(ChartHeadline.FontName, size)).Height > maxHeight && size > 4
                )
            {
                size--;
            }

            return new Font(ChartHeadline.FontName, size);
        }

        /// <summary>
        /// Notifies the server control that an element, either XML
        /// or HTML, was parsed and adds the element to the server control's <see cref="System.Web.UI.ControlCollection">
        /// T:System.Web.UI.ControlCollection</see>
        /// object.
        /// </summary>
        /// <param name="obj">A <see cref="System.Object">T:System.Object</see> that represents the parsed element.</param>
        protected override void AddParsedSubObject(object obj)
        {
            if (!(obj is ChartValue)) return;
            ChartValue v = (ChartValue) obj;
            if (m_dataArray.Count == 0)
                m_dataArray.Add(new ArrayList());
            ((ArrayList) m_dataArray[0]).Add(v);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init">E:System.Web.UI.Control.Init</see>
        /// event.
        /// </summary>
        /// <param name="e">A <see cref="System.EventArgs">T:System.EventArgs</see> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            if (HttpContext.Current.Request.QueryString["WGCharting"] != ClientID) return;
            Page.Response.Clear();
            if (Page.Response.Filter != null)
                Page.Response.Filter = null;

            Page.Response.ContentType = "image/png";

            System.Drawing.Image tomemory = (System.Drawing.Image)HttpContext.Current.Session[ClientID + "_img"] ?? Render();
            MemoryStream memory = new MemoryStream();
            tomemory.Save(memory, ImageFormat.Png);

            memory.WriteTo(Page.Response.OutputStream);
            memory.Close();
            tomemory.Dispose();
            Page.Response.End();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender">E:System.Web.UI.Control.PreRender</see>
        /// event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs">T:System.EventArgs</see> object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            if (m_dataArray.Count == 0)
            {
                Controls.Add(new LiteralControl("No values."));
                return;
            }
            HttpContext.Current.Session[ClientID + "_img"] = Render();

            string s = Grid.GotHttpContext &&
                       HttpContext.Current.Request.Browser.Type.ToUpperInvariant().StartsWith("IE")
                           ? string.Format(
                                 "<img height=\"{0}\" width=\"{1}\"  src=\"{4}\" style=\"filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='?wgcharting={2}', sizingMethod='scale');\" alt=\"{5}\" border=\"0\" src=\"?wgcharting={3}\"/>",
                                 Unit.Pixel(Height), Unit.Pixel(Width), ClientID, ClientID, Page.ClientScript.GetWebResourceUrl(GetType(), "WebGrid.Resources.images.transparent.gif"), ChartImageTitle)
                           : string.Format(
                                 "<img height=\"{0}\" width=\"{1}\" alt=\"{3}\" border=\"0\" src=\"?wgcharting={2}\"/>",
                                 Unit.Pixel(Height), Unit.Pixel(Width), ClientID, ChartImageTitle);
            Controls.Add(new LiteralControl(s));

            base.OnPreRender(e);
        }

        #endregion Methods
    }

    internal class MyControlBuilder : ControlBuilder
    {
        #region Methods

        public override Type GetChildControlType(String tagName, IDictionary attributes)
        {
            if (tagName.ToLowerInvariant()  == "item" || tagName.ToLowerInvariant()  == "chartvalue" || tagName.ToLowerInvariant()  == "value")
            {
                return Type.GetType("WebGrid.Util.ChartValue", false, true);
            }
            return null;
        }

        #endregion Methods
    }
}