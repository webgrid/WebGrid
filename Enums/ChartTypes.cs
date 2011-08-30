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
namespace WebGrid.Enums
{
    #region Enumerations

    /// <summary>
    /// All types of charts <see cref="WebGrid.Chart">WebGrid.Chart</see> can render.
    /// When using a chart it by default render over all rows in grid view.
    /// </summary>
    public enum ChartType
    {
        /// <summary>
        /// Undefined.
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// 2d Horizontal bar chart ("boxes"). Obsolete.
        /// </summary>
        HorizontalBar = 1,
        /// <summary>
        /// 2d vertical bar chart will be generated in the area of the column.
        /// </summary>
        VerticalBarChart = 2,
        /// <summary>
        /// 3d vertical bar chart will be generated in the area of the column.
        /// </summary>
        VerticalBarChart3D = 3,
        /// <summary>
        /// 2d line chart will be generated in the area of the column.
        /// </summary>
        LineChart = 4,
        /// <summary>
        /// 2d area chart will be generated in the area of the column.
        /// </summary>
        AreaChart = 5,
        /// <summary>
        /// 2d pie chart will be generated in the area of the column.
        /// </summary>
        PieChart = 6,
        /// <summary>
        /// 3d pie chart will be generated in the area of the column.
        /// </summary>
        PieChart3D = 7
    }

    #endregion Enumerations
}