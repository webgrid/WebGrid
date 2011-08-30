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
    /// How to resize an image.
    /// </summary>
    public enum ResizeMode
    {
        /// <summary>
        /// Undefined.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// New widthEditableArea and heightEditableContent are absolute.
        /// </summary>
        Absolute = 1,

        /// <summary>
        /// WidthEditableColumn and heightEditableContent are maximum values. Image is resized with the correct
        /// proportions to fit into widthEditableArea and heightEditableContent.
        /// </summary>
        Proportional = 2,

        /// <summary>
        /// WidthEditableColumn and heightEditableContent are percentage.
        /// </summary>
        Percent = 3
    }

    #endregion Enumerations
}