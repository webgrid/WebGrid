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
    /// Settings class for sending webgrid  forms as an e-mail.
    /// </summary>
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [Description("Settings class for sending webgrid  forms as an e-mail.")]
    [MergableProperty(false)]
    [Browsable(false)]
    public class GridFormMail
    {
        #region Properties

        /// <summary>
        /// Sets or gets the emailaddresses (e-mail addresses Separated by semicomman ";") should receive this grid form on successfully insert.
        /// </summary>
        /// <remarks>
        /// To add an email address used in your grid form use '[ColumnID]'. Example is EmailFormOnUpdate = "olav@webgrid.com;[EmailAddress]" 
        /// </remarks>
        public string EmailAdresses
        {
            get; set;
        }

        /// <summary>
        /// Sets or gets SmtpServer use this settings to override the smtpserver set in 'WGSMTPSERVER'
        /// </summary>
        public string SmtpServer
        {
            get; set;
        }

        /// <summary>
        /// Sets or gets the subjects for the e-mail, if no subject is set the title of the grid is used.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public string Subject
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