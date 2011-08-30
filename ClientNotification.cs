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
    /// Client Notification raises unobtrusive messages within the browser, similar to the way that OS X's Growl Framework works
    /// </summary>
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [Description("Client Notification settings for WebGrid")]
    [MergableProperty(false)]
    [Browsable(false)]
    public class ClientNotification
    {
        #region Fields

        private string _HeaderText;
        private int _lifespan = 3000;
        string _position;
        string _cssClass;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Whether or not the close-all button should be used when more then one notification appears on the screen. Optionally this property can be set to a function which will be used as a callback when the close all button is clicked.
        /// </summary>
        public bool Closer
        {
            get; set;
        }

        ///<summary>
        /// This content is used for the close-all link that is added to the bottom of a jGrowl container when it contains more than one notification.
        ///</summary>
        public string CloserTemplate
        {
            get; set;
        }

        /// <summary>
        /// Html container for the client notification. Default is none
        /// </summary>
        public string Container
        {
            get
            {
                 return _position;
            }
            set { _position = value; }
        }

        /// <summary>
        /// Default is Grid Title
        /// </summary>
        public string HeaderText
        {
            get { return _HeaderText; }
            set { _HeaderText = value; }
        }

        /// <summary>
        /// The lifespan of a non-sticky message on the screen.
        /// </summary>
        public int LifeSpan
        {
            get { return _lifespan; }
            set { _lifespan = value; }
        }

        /// <summary>
        /// When set to true a message will stick to the screen until it is intentionally closed by the user.
        /// </summary>
        public bool Sticky
        {
            get; set;
        }

        /// <summary>
        /// A CSS class designating custom styling for this particular message.
        /// </summary>
        public string CssClass
        {
            get
            {
                return _cssClass;
            }
            set { _cssClass = value; }
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