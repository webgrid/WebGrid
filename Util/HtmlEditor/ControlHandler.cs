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
    using System.Web;

    /// <summary>
    /// Control handler class that handles features like writing images, image browsing, and image upload.
    /// </summary>
    public class ControlHandler : IHttpHandler
    {
        #region Properties

        bool IHttpHandler.IsReusable
        {
            get { return true; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"></see> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"></see> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            string resourceType = context.Request.QueryString["res"];
            string resourceId = context.Request.QueryString["id"];
            string cssFile = context.Request.QueryString["cssFile"];
            object src;
            //string contentType = string.Empty;
            switch (resourceType)
            {
                case "image":
                    byte[] img = ControlManager.GetImage(resourceId);
                    context.Response.ContentType = "image/gif";
                    context.Response.BinaryWrite(img);
                    break;
                case "imagebrowser":
                    src = ControlManager.GetPage(resourceType, context.Server.MapPath(resourceId), resourceId, null);
                    context.Response.ContentType = "text/html";
                    context.Response.Write(src);
                    break;
                case "upload":
                    ControlManager.UploadFile(context.Request.Files, context.Server.MapPath(resourceId));
                    src = ControlManager.GetPage("imagebrowser", context.Server.MapPath(resourceId), resourceId, null);
                    context.Response.ContentType = "text/html";
                    context.Response.Write(src);
                    break;
                default:
                    src = ControlManager.GetPage(resourceType, resourceId, GetBrowserName(context), cssFile);
                    context.Response.ContentType = "text/html";
                    context.Response.Write(src);
                    break;
            }
            context.Response.End();
        }

        private static string GetBrowserName(HttpContext context)
        {
            string userAgent = context.Request.UserAgent;
            return -1 != userAgent.IndexOf("MSIE") ? "msie" : "mozilla";
        }

        #endregion Methods
    }
}