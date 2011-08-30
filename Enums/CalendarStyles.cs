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
    /// The different modes for a calendar that is used by <see cref="WebGrid.DateTime">WebGrid.DateTime</see>.
    /// </summary>
    public enum CalendarStyle
    {
        /// <summary>
        /// Undefined.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Only shows a textbox.
        /// </summary>
        Textbox = 1,

        /// <summary>
        /// Shows a date picker.
        /// </summary>
        Calendar = 2,

        /// <summary>
        /// Shows both a date picker and a textbox for "manual" input of the date and time.
        /// </summary>
        TextboxAndCalendar = 3
    }

    #endregion Enumerations
}