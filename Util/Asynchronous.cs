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
    using System.Diagnostics;
    using System.Net;
    using System.Text;

    /// <summary>
    /// Asynchronous class is an utility for applications that perform many tasks simultaneously, 
    /// yet remain responsive to user interaction, often require a design that uses multiple threads and
    /// performs "in the background" without interrupting your application.
    /// </summary>
    public sealed class Asynchronous
    {
        #region Fields

        private static readonly AsyncCallback dynamicInvokeDone = DynamicInvokeDone;
        private static readonly DynamicInvokeShimProc dynamicInvokeShim = DynamicInvokeShim;

        #endregion Fields

        #region Delegates

        private delegate void DynamicInvokeShimProc(Delegate d, object[] args);

        #endregion Delegates

        #region Methods

        /// <summary>
        /// Fires the and forget.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The args.</param>
        public static void FireAndForget(Delegate d, params object[] args)
        {
            if (d == null || args == null)
                return;
            dynamicInvokeShim.BeginInvoke(d, args, dynamicInvokeDone, null);
        }

        /// <summary>
        /// Obtains a reference to a clinet-side javascript function that causes, when
        /// invoked, the client to callback to the server.
        /// </summary>
        internal static string GetCallbackEventReference(Grid control, string argument,
            bool causesValidation, string validationGroup,
            string imageDuringCallback)
        {
            argument = System.Web.HttpUtility.UrlEncode(argument, Encoding.Default);
            if (argument != null) argument = argument.Replace("'", "%27");
            return string.Format(
                "{{setActiveGrid('{10}');Anthem_FireCallBackEvent(this,event,'{0}','{1}',{2},'{3}','{4}','{5}',{6},{7},{8},{9},true,true)}}",
                control.UniqueID, argument
                ,
                causesValidation ? "true" : "false",
                validationGroup,
                imageDuringCallback,
                control.TextDuringCallBack,
                control.EnabledDuringCallBack ? "true" : "false",
                (string.IsNullOrEmpty(control.PreCallBackFunction))
                    ? "null"
                    : control.PreCallBackFunction,
                (string.IsNullOrEmpty(control.PostCallBackFunction))
                    ? "null"
                    : control.PostCallBackFunction,
                (string.IsNullOrEmpty(control.CallBackCancelledFunction))
                    ? "null"
                    : control.CallBackCancelledFunction
                , control.ID);
        }

        private static void DynamicInvokeDone(IAsyncResult ar)
        {
            dynamicInvokeShim.EndInvoke(ar);
        }

        private static void DynamicInvokeShim(Delegate d, object[] args)
        {
            d.DynamicInvoke(args);
        }

        #endregion Methods
    }

    /// <summary>
    /// This asynchronous class opens a WebClient and transmits data to the provided URL.
    /// Any errors are logged at Windows EventLog (Application category). 
    /// </summary>
    public class Url
    {
        #region Fields

        private WebClient wc;

        #endregion Fields

        #region Delegates

        private delegate void UrlDelegate(
            string url, string data, UploadDataCompletedEventHandler eventhandler, object userToken, bool nop);

        #endregion Delegates

        #region Methods

        /// <summary>
        /// Opens the URL sync.
        /// </summary>
        /// <param name="url">url to open</param>
        /// <param name="data">The data to be posted.</param>
        /// <param name="eventhandler">The event handler.</param>
        /// <param name="userToken">The user token.</param>
        public void OpenUrl(string url, string data, UploadDataCompletedEventHandler eventhandler, object userToken)
        {
            if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase) == false)
                url = string.Format("http://{0}", url);
            UrlDelegate dc = OpenUrl;
            Asynchronous.FireAndForget(dc, url, data, eventhandler, userToken, false);
        }

        private static void wc_UploadDataCompleted(object sender, UploadDataCompletedEventArgs e)
        {
            if (e.Error == null) return;
            EventLog logger = new EventLog();

            logger.Source = "WebGrid";
            logger.WriteEntry(e.Error.ToString(), EventLogEntryType.Error);
            logger.Dispose();
        }

        private void OpenUrl(string url, string data, UploadDataCompletedEventHandler eventhandler, object userToken,
            bool nop)
        {
            wc = new WebClient();
            wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            if (eventhandler != null)
                wc.UploadDataCompleted += eventhandler;
            byte[] postByteArray = Encoding.ASCII.GetBytes(data);
            try
            {
                wc.UploadData(new Uri(url), "POST", postByteArray);
                wc.UploadDataCompleted += wc_UploadDataCompleted;
            }
            catch (Exception e)
            {
                EventLog logger = new EventLog();

                logger.Source = "WebGrid";
                logger.WriteEntry(e.ToString(), EventLogEntryType.Error);
                logger.Dispose();
            }
        }

        #endregion Methods
    }
}