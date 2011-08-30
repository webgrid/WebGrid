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

namespace WebGrid
{
    using System.ComponentModel;
    using System.Web.UI;

    /// <summary>
    /// Popup extender is an ASP.NET extender that can be attached to WebGrid control in order to open a popup window that displays additional content. 
    /// This popup window will be interactive and will  be within an  external web page, so it will be able to perform complex server-based processing (including postbacks) 
    /// without affecting the rest of the page. 
    /// </summary>
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [Description("Popup url")]
    [MergableProperty(false)]
    [Browsable(false)]
    public class PopupExtender
    {
        #region Properties

        /// <summary>
        /// Height of the box
        /// </summary>
        public int Height
        {
            get; set;
        }

        /// <summary>
        /// Sets or gets the title for the PopupExtender. Default is no title.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public string Title
        {
            get; set;
        }

        /// <summary>
        /// Sets or gets the url for the popup box
        /// </summary>
        /// <remarks>
        /// </remarks>
        [UrlProperty("aspx")]
        public string Url
        {
            get; set;
        }

        /// <summary>
        /// Width of the box
        /// </summary>
        public int Width
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return "No design support";
        }

        #endregion Methods
    }
}