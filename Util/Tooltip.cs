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

    /// <summary>
    /// Tooltip class is used to display tooltip over a selected web control or text on a web page.
    /// The tooltip is displayed when the mouse is over the text.
    /// </summary>
    public sealed class Tooltip
    {
        #region Methods

        /// <summary>
        /// The tooltip is displayed when the mouse is over the text.
        /// </summary>
        /// <param name="displayText">The text that triggers the tooltip</param>
        /// <param name="tooltip">The tooltip text itself</param>
        public static string Add(string displayText, string tooltip)
        {
            if (string.IsNullOrEmpty(tooltip))
                return displayText;

            tooltip = tooltip.Replace("\r\n", "<br/>").Replace("\n", "<br/>").Replace("'",string.Empty);

            return
                String.Format("<span class=\"wgtool\">{0}<span class=\"wgtip\">{1}</span></span>", displayText, tooltip);
        }

        #endregion Methods
    }
}