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
    /// Inputmask settings for WebGrid
    /// </summary>
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [Description("Popup url")]
    [MergableProperty(false)]
    [Browsable(false)]
    public class MaskedInput
    {
        #region Fields

        private string _attribute = "alt";

        #endregion Fields

        #region Properties

        /// <summary>
        /// an attr to look for the mask name or the mask it, default is 'alt' attribute.
        /// </summary>
        public string Attribute
        {
            get { return _attribute; }
            set { _attribute = value; }
        }

        /// <summary>
        /// auto focus the next form element
        /// </summary>
        public bool EnableAutoTab
        {
            get; set;
        }

        /// <summary>
        /// selects characters on focus of the input
        /// </summary>
        public bool EnableSelectCharsOnFocus
        {
            get; set;
        }

        /// <summary>
        /// To use or not to use textAlign on the input
        /// </summary>
        public bool EnableTextAlign
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

        internal static string GetStringValue(bool value)
        {
            return value ? "true" : "false";
        }

        #endregion Methods
    }
}