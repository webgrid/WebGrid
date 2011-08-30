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
    /// Use the RowType property to determine the type of row that the WebGrid object represents.
    /// </summary>
    public enum RowType
    {
        /// <summary>
        ///The header row in the WebGrid control.
        /// </summary>
        Header = 1,

        /// <summary>
        /// A data row in the WebGrid control.
        /// </summary>
        DataRow = 2,

        /// <summary>
        /// The group row in the WebGrid control.
        /// </summary>
        Group = 3,

        /// <summary>
        /// The footer row in the WebGrid control.
        /// </summary>
        Footer = 4
    }

    #endregion Enumerations
}