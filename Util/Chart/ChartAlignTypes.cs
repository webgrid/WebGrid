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

    #region Enumerations

    /// <summary>
    /// Alignment for the chart Headline.
    /// </summary>
    [Flags]
    public enum ChartAlignTypes
    {
        /// <summary>
        /// Left alignment for the graph to be rendered.
        /// </summary>
        Left = 0x1,
        /// <summary>
        /// Center alignment for the graph to be rendered.
        /// </summary>
        Center = 0x2,
        /// <summary>
        /// Right alignment for the graph to be rendered.
        /// </summary>
        Right = 0x4,
        /// <summary>
        /// Top alignment for the graph to be rendered.
        /// </summary>
        Top = 0x8,
        /// <summary>
        /// Middle alignment for the graph to be rendered.
        /// </summary>
        Middle = 0x10,
        /// <summary>
        /// Bottom alignment for the graph to be rendered.
        /// </summary>
        Bottom = 0x20,
    }

    #endregion Enumerations
}