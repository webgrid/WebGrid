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
    /// Determines how foreignkeys should be represented when they are editable.
    /// </summary>
    public enum ForeignkeyType
    {
        /// <summary>
        /// Uses dropdown box to display the options.
        /// </summary>
        Select = 0,

        /// <summary>
        /// Uses radio buttons to display the options.
        /// </summary>
        Radiobuttons = 1,
        /// <summary>
        /// Select menu supporting Jquery UI Themes.
        /// </summary>
        SelectMenu = 2,
    }

    #endregion Enumerations
}