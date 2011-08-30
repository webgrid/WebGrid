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
    using System.Text;
    using System.Web.UI;

    /// <summary>
    /// This class contains methods for <see cref="WebGrid.Grid">WebGrid.Grid</see> web control to generate buttons, hyperlinks, and
    /// other clickable events.
    /// </summary>
    public class Buttons
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Buttons"/> class.
        /// </summary>
        internal Buttons()
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Texts the button.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="text">The text.</param>
        /// <param name="eventName">Name of the event upon post back/callback</param>
        /// <param name="eventparameters">Parameters for the event</param>
        /// <returns></returns>
        public static string TextButton(Grid grid, string text, string eventName, string[] eventparameters)
        {
            return Anchor(grid, text, eventName, eventparameters,null,null,null,null,false);
        }

        /// <summary>
        /// Texts the button.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="text">The text.</param>
        /// <param name="eventName">Name of the event upon post back/callback</param>
        /// <param name="eventparameters">Parameters for the event</param>
        /// <param name="css">The CSS .</param>
        /// <param name="confirmText">The confirm text.</param>
        /// <returns></returns>
        public static string TextButton(Grid grid, string text, string eventName, string[] eventparameters, string css,
            string confirmText)
        {
            return Anchor(grid, text, eventName, eventparameters, confirmText, null, css, null, false);
        }

        /// <summary>
        /// Anchors the specified grid.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="text">The text.</param>
        /// <param name="eventName">Name of the event upon post back/callback</param>
        /// <param name="eventparameters">Parameters for the event</param>
        /// <returns></returns>
        internal static string Anchor(Grid grid, string text, string eventName, string[] eventparameters)
        {
            return Anchor(grid, text, eventName, eventparameters, null, null, null, null, false);
        }

        /// <summary>
        /// Anchors the specified grid.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="text">The text.</param>
        /// <param name="eventName">Name of the event upon post back/callback</param>
        /// <param name="eventparameters">Parameters for the event</param>
        /// <param name="confirmText">The confirm text.</param>
        /// <returns></returns>
        internal static string Anchor(Grid grid, string text, string eventName, string[] eventparameters, string confirmText)
        {
            return Anchor(grid, text, eventName, eventparameters, confirmText, null, null, null, false);
        }

        /// <summary>
        /// Anchors the specified grid.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="text">The text.</param>
        /// <param name="eventName">Name of the event upon post back/callback</param>
        /// <param name="eventparameters">Parameters for the event</param>
        /// <param name="confirmText">The confirm text.</param>
        /// <param name="alternativeText">The alternative text.</param>
        /// <param name="css">The CSS class.</param>
        /// <returns></returns>
        /// <param name="elementid">Client identifier for the element</param>
        /// <param name="isButton"></param>
        internal static string Anchor(Grid grid, string text, string eventName, string[] eventparameters, string confirmText,
            string alternativeText, string css, string elementid, bool isButton)
        {
            StringBuilder content = new StringBuilder();

            string param = eventparameters != null ? String.Join("!", eventparameters) : string.Empty;
            string eventScript = null;
            if (grid.Page != null)
            {
                if (!grid.EnableCallBack)
                {
                    PostBackOptions linkpostback = new PostBackOptions(grid, string.Format("{0}!{1}", eventName, param))
                                                       {ClientSubmit = true};
                    eventScript = grid.Page.ClientScript.GetPostBackEventReference(linkpostback);
                }
                else
                {
                    string command = string.Format("{0}!{1}", eventName, param);
                    eventScript =
                        Asynchronous.GetCallbackEventReference(grid, command, false, string.Empty,
                                                               string.Empty);
                }
            }

            if (grid.IsUsingJQueryUICSSFramework && isButton)
            {
                css = "SlaveGridClick" == eventName
                          ? "ui-button wgbutton ui-state-default ui-corner-top " + css
                          : "ui-button wgbutton ui-state-default ui-corner-all " + css;
                content.Append("<button");
            }
            else
                content.Append("<a");

            if (string.IsNullOrEmpty(alternativeText) == false)
                content.AppendFormat(" title=\"{0}\"", alternativeText);
            if (string.IsNullOrEmpty(css) == false)
                content.AppendFormat(" class=\"{0}\"", css);

            if (string.IsNullOrEmpty(elementid) == false)
                content.AppendFormat(" id=\"{0}\"", elementid);

            content.Append(" href=\"#\" onclick=\"");
            if (string.IsNullOrEmpty(confirmText) == false)
                content.AppendFormat("if (wgconfirm('{0}',this,'{1}'))", confirmText.Replace("'", "\\'"), grid.DialogTitle.Replace("'", "\\'"));

            if (grid.IsUsingJQueryUICSSFramework && isButton)
                content.AppendFormat("{0};return false;\">{1}</button>", eventScript, text);
            else content.AppendFormat("{0};return false;\">{1}</a>", eventScript, text);
            return content.ToString();
        }

        /// <summary>
        /// Buttons the specified grid.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="buttonValue">The button value.</param>
        /// <param name="eventName">Name of the event upon post back/callback</param>
        /// <param name="eventparameters">Parameters for the event</param>
        /// <param name="htmlButtonType">HTML button type.</param>
        /// <returns></returns>
        internal static string Button(Grid grid, string buttonValue, string eventName, string[] eventparameters,
            string htmlButtonType)
        {
            if (grid.Page == null)
                return string.Empty;
            string param = String.Join("!", eventparameters);
            StringBuilder s = new StringBuilder();
            s.AppendFormat("<input type=\"{0}\" value=\"{1}\"", htmlButtonType, buttonValue);
            string link = grid.EnableCallBack ? Asynchronous.GetCallbackEventReference(grid, string.Format("{0}!{1}", eventName, param), false, string.Empty,
                                                                                       string.Empty) : grid.Page.ClientScript.GetPostBackEventReference(grid, string.Format("{0}!{1}", eventName, param));
            s.AppendFormat(" onclick=\"{0};\"/>", link);
             return s.ToString();
        }

        /// <summary>
        /// Buttons the control.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="buttonValue">The button value.</param>
        /// <param name="eventName">Name of the event upon post back/callback</param>
        /// <param name="eventparameters">Parameters for the event</param>
        /// <param name="htmlButtonType">HTML button type.</param>
        /// <returns></returns>
        internal static Control ButtonControl(Grid grid, string buttonValue, string eventName, string[] eventparameters,
            string htmlButtonType)
        {
            return new LiteralControl(Button(grid, buttonValue, eventName, eventparameters, htmlButtonType));
        }

        /// <summary>
        /// Images the specified grid.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="text">text or html for this link</param>
        /// <param name="eventName">Name of the event upon post back/callback</param>
        /// <param name="eventparameters">Parameters for the event</param>
        /// <returns></returns>
        internal static string Image(Grid grid, string text, string eventName, string[] eventparameters)
        {
            return Image(grid, text, eventName, eventparameters, null);
        }

        /// <summary>
        /// Images the specified grid.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="text">text or html for this link</param>
        /// <param name="eventName">Name of the event upon post back/callback</param>
        /// <param name="eventparameters">Parameters for the event</param>
        /// <param name="confirmText">The confirm text.</param>
        /// <returns></returns>
        internal static string Image(Grid grid, string text, string eventName, string[] eventparameters,
            string confirmText)
        {
            return Anchor(grid, text, eventName, eventparameters, confirmText, null, null, null, false);
        }

        /// <summary>
        /// Images the specified image name.
        /// </summary>
        /// <param name="imageName">ColumnId of the image.</param>
        /// <param name="alternativeText">The alternative text.</param>
        /// <returns></returns>
        internal static string Image(string imageName, string alternativeText)
        {
            return string.Format("<img style=\" border:0px\" alt=\"{0}\" src=\"{1}\" />", alternativeText, imageName);
        }

        /// <summary>
        /// Images the control.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="text">text or html for this link</param>
        /// <param name="eventName">Name of the event upon post back/callback</param>
        /// <param name="eventparameters">Parameters for the event</param>
        /// <param name="confirmText">The confirm text.</param>
        /// <returns></returns>
        internal static string ImageControl(Grid grid, string text, string eventName, string[] eventparameters,
            string confirmText)
        {
            return Image(grid, text, eventName, eventparameters, confirmText);
        }

        /// <summary>
        /// Texts the button control.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="text">The text.</param>
        /// <param name="eventName">Name of the event upon post back/callback</param>
        /// <param name="eventparameters">Parameters for the event</param>
        /// <param name="css">The CSS class.</param>
        /// <returns></returns>
        internal static string TextButtonControl(Grid grid, string text, string eventName, string[] eventparameters,
            string css)
        {
            return Anchor(grid, text, eventName, eventparameters, null, null, css, null, true);
        }

        /// <summary>
        /// Texts the button control.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="text">The text.</param>
        /// <param name="eventName">Name of the event upon post back/callback</param>
        /// <param name="eventparameters">Parameters for the event</param>
        /// <param name="css">The CSS class.</param>
        /// <param name="confirmText">The confirm text.</param>
        /// <returns></returns>
        internal static string TextButtonControl(Grid grid, string text, string eventName, string[] eventparameters,
            string css, string confirmText)
        {
            return Anchor(grid, text, eventName, eventparameters, confirmText, null, css, null, true);
        }

        /// <summary>
        /// Texts the button control.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="text">The text.</param>
        /// <param name="eventName">Name of the event upon post back/callback</param>
        /// <param name="eventparameters">Parameters for the event</param>
        /// <param name="css">The CSS class.</param>
        /// <param name="confirmText">The confirm text.</param>
        /// <returns></returns>
        /// <param name="elementId">Client identifier for the element</param>
        internal static string TextButtonControl(Grid grid, string text, string eventName, string[] eventparameters,
            string css, string confirmText, string elementId)
        {
            return Anchor(grid, text, eventName, eventparameters, confirmText, null, css, elementId, true);
        }

        #endregion Methods
    }
}