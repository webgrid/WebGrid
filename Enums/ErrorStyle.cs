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
    /// Where WebGrid should display its error messages.
    /// </summary>
    public enum SystemMessageStyle
    {
        /// <summary>
        /// Undefined. (error will appear in WebGrid.)
        /// </summary>
        Undefined = 1,

        /// <summary>
        /// error will appear in WebGrid's error box (default for detail view).
        /// </summary>
        WebGrid = 1,

        /// <summary>
        /// error will appear to the left of the column generating the message.
        /// </summary>
        ColumnLeft = 2,

        /// <summary>
        /// error will appear to the right of the column generating the message.
        /// </summary>
        ColumnRight = 3,

        /// <summary>
        /// error will appear above the column generating the error message.
        /// </summary>
        ColumnTop = 4,

        /// <summary>
        /// error will appear below the column generating the error message.
        /// </summary>
        ColumnBottom = 5
    }

    #endregion Enumerations
}