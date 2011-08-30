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
    /// GridPart is a common web control for WebGrid to make items outside the DataGrid movable.
    /// </summary>
    public enum GridPart
    {
        /// <summary>
        /// Undefined.
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Renders the Pager control where this web control is located.
        /// </summary>
        Pager = 1,

        /// <summary>
        /// Renders the "SearchField" control where this web control is located.
        /// </summary>
        SearchField = 2,
        /// <summary>
        /// Renders the "NewRecord" control where this web control is located.
        /// </summary>
        NewRecord = 3,
        /// <summary>
        /// Renders the "UpdateRecords" control where this web control is located.
        /// </summary>
        UpdateRecords = 4,
    }

    #endregion Enumerations
}