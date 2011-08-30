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
    /// Defines in what mode the columns should be visibility.
    /// </summary>
    public enum Visibility
    {
        /// <summary>
        /// Undefined.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Column will be visibility in both grid view and detail view.
        /// </summary>
        Both = 1,

        /// <summary>
        /// Column is only visibility in grid view.
        /// </summary>
        Grid = 2,

        /// <summary>
        /// Column is only visibility in detail view.
        /// </summary>
        Detail = 3,

        /// <summary>
        /// Column will not be rendered.
        /// </summary>
        None = 4
    }

    #endregion Enumerations
}