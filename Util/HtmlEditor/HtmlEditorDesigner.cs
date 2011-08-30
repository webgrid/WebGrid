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
    using System.Globalization;
    using System.Web.UI.Design;

    /// <summary>
    /// 
    /// </summary>
    public class HtmlEditorDesigner : ControlDesigner
    {
        #region Methods

        /// <summary>
        /// Returns a value that indicates the HTML to display in the designer.
        /// </summary>
        /// <returns>A value that indicates the HTML for the designer.</returns>
        public override string GetDesignTimeHtml()
        {
            if (Component.Site.DesignMode == false)
                return null;
            HtmlEditor control = (HtmlEditor) Component;
            return String.Format(CultureInfo.InvariantCulture,
                                 "<div><table width=\"{0}\" height=\"{1}\" bgcolor=\"white\" bordercolor=\"black\" cellpadding=\"0\" cellspacing=\"0\" border=\"1\"><tr><td valign=\"middle\" align=\"center\">WebGrid HtmlEditor - <b>{2}</b></td></tr></table></div>",
                                 control.Width,
                                 control.Height,
                                 control.ID);
        }

        #endregion Methods
    }
}