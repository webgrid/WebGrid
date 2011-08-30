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
    public enum ContextMenuType
    {
        /// <summary>
        /// Print Menu
        /// </summary>
        Undefined = -1,

        /// <summary>
        /// Print Menu
        /// </summary>
        Print = 0,

        /// <summary>
        /// Export data
        /// </summary>
        Export = 1,

        /// <summary>
        /// Refresh grid
        /// </summary>
        Refresh = 2,

        /// <summary>
        /// Help
        /// </summary>
        Help = 3,
        /// <summary>
        /// About WebGrid
        /// </summary>
        About = 4,

        /// <summary>
        /// Collapse Grid
        /// </summary>
        CollapsedGrid = 5,

        /// <summary>
        /// Active slave grid
        /// </summary>
        ActiveSlaveGrid = 6
    }

    #endregion Enumerations
}