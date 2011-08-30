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
    using System.Net;
    using System.Text;
    using System.Web;

    using WebGrid.Config;

    /// <summary>
    /// this class contains WebGrid Sms (Short Message Service) service for sending 
    /// messages of up to 160 characters in each message to mobile phones that use 
    /// Global System for Mobile (GSM) communication. 
    /// Webgrid Sms service uses clicktell (http://clickatell.com)
    /// as service provider for sending Sms messages around the world.
    /// out Sms network supports more then 180 countries, and we support outgoing non-premium Sms messages.
    /// Non-premium messages is free of charge for the receiver.
    /// If you need access to this service please use the contact form at http://www.webgrid.com,
    /// and state your request there.
    /// </summary>
    /// <remarks>
    /// To access WebGrid Sms Service you must have a customerId and password, by this information you
    /// can use <see cref="WebGrid.Util.Sms"/> class to access WebGrid Sms Service Network.
    /// you can also access your account at http://sms.webgrid.com and login into your account and
    /// view credit balance and message archive. When you send an Sms message you will be charged by
    /// credits, and soon as your account hits zero credits your will loose access to WebGrid Sms Service Network.
    /// Credits can be bought at http://sms.webgrid.com.
    /// </remarks>
    public class Sms
    {
        #region Fields

        private readonly Url webgridurl = new Url();

        private string customerId;
        private string password;

        #endregion Fields

        #region Properties

        /// <summary>
        /// The customerId provided by the WebGrid Sms Service.
        /// </summary>
        public string CustomerId
        {
            get { return customerId; }

            set { customerId = value; }
        }

        /// <summary>
        /// Your password used to access WebGrid Sms Service.
        /// </summary>
        public string Password
        {
            get { return password; }

            set { password = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Method to access WebGrid Sms Service and sending out sms asynchronously (threadpool), to access this service you must provide
        /// a customerId and password. For further information please read documentation at
        /// <see cref="WebGrid.Util.Sms"/>.
        /// This method support also Web.Config to access customerId and password information for WebGrid Sms
        /// the web.config keys are "WGSMSCUSTOMERID" and "WGSMSPASSWORD".
        /// </summary>
        /// <param name="sender">Sender address, must be alfanumeric string with no more then 11 characters and no special characters.</param>
        /// <param name="receiver">The receivers cellphone number, can be Separated by ";" and all numbers must be written with international number. See documentation for further information.</param>
        /// <param name="message">The message you want to send. All messages with more then 160 characters will be split.</param>
        /// <param name="eventhandler">The event handler</param>
        /// <param name="userToken">The user token for this message</param>
        /// <remarks>
        /// Using the receiver parameter you must include country number, area number, and local number. Example could be "4712345678", where
        /// "47" is the norwegian country code and 12345678 is area and local number.
        /// </remarks>
        public void SendSms(string sender, string receiver, string message, UploadDataCompletedEventHandler eventhandler,
            object userToken)
        {
            if (string.IsNullOrEmpty(customerId))
                customerId = GridConfig.Get("WGSMSCUSTOMERID", "0");
            if (string.IsNullOrEmpty(password))
                password = GridConfig.Get("WGSMSPASSWORD", null as string);

            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(customerId))
                throw (new ApplicationException(
                    "customerID or password is missing for WebGrid.Util.Sms (Could not find web.config keys WGSMSCUSTOMERID and WGSMSPASSWORD"));

            internalSendSms(sender, receiver, message, eventhandler, userToken);
        }

        /// <summary>
        /// Method to access WebGrid Sms Service. Internal use only..
        /// </summary>
        /// <param name="sender">Sender address, must be alfanumeric string with no more then 11 characters and no special characters.</param>
        /// <param name="receiver">The receivers cellphone number, can be Separated by ";" and all numbers must be written with international number. See documentation for further information.</param>
        /// <param name="message">The message you want to send. All messages with more then 160 characters will be split.</param>
        /// <param name="eventhandler">The eventhandler.</param>
        /// <param name="userToken">The user token.</param>
        /// <remarks>
        /// Using the receiver parameter you must include country number, area number, and local number. Example could be "4712345678", where
        /// "47" is the norwegian country code and 12345678 is area and local number.
        /// Any errors are logged at Windows EventLog (Application category). 
        /// </remarks>
        private void internalSendSms(string sender, string receiver, string message,
            UploadDataCompletedEventHandler eventhandler, object userToken)
        {
            const string webgridSMSService = "http://traffic.webgrid.com/send.aspx";

            string urlRequestform = string.Format("customerID={0}", HttpUtility.UrlEncode(customerId, Encoding.Default));
            if (string.IsNullOrEmpty(password) == false)
                urlRequestform += string.Format("&password={0}", HttpUtility.UrlEncode(password, Encoding.Default));
            if (string.IsNullOrEmpty(sender) == false)
                urlRequestform += string.Format("&sender={0}", HttpUtility.UrlEncode(sender, Encoding.Default));
            if (string.IsNullOrEmpty(receiver) == false)
                urlRequestform += string.Format("&receiver={0}", HttpUtility.UrlEncode(receiver, Encoding.Default));
            if (string.IsNullOrEmpty(message) == false)
                urlRequestform += string.Format("&message={0}", HttpUtility.UrlEncode(message, Encoding.Default));
            urlRequestform += "&messagetypeID=1&__Active=true";

            webgridurl.OpenUrl(webgridSMSService, urlRequestform, eventhandler, userToken);
        }

        #endregion Methods
    }
}