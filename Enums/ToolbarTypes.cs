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
    /// The different toolbar types supported by  <see cref="WebGrid.Grid">WebGrid.Grid</see>.
    /// </summary>
    public enum ToolbarType
    {
        /// <summary>
        /// Toolbar rendered after rows in detail view
        /// </summary>
        ToolbarDetailBottom = 0,

        /// <summary>
        /// Toolbar rendered before rows in detail view
        /// </summary>
        ToolbarDetailTop = 1,

        /// <summary>
        /// Toolbar rendered after rows in grid view
        /// </summary>
        ToolbarGridBottom = 2,

        /// <summary>
        /// Toolbar rendered before rows in grid view
        /// </summary>
        ToolbarGridTop = 3
    }

    #endregion Enumerations
}