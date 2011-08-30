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

namespace WebGrid.Design
{
    using System.Text;
    using System.Web;
    using System.Web.UI;

    using WebGrid.Enums;

    /// <summary>
    /// An error generated either as a system failure or by invalid data input by the user.
    /// </summary>
    public class SystemMessage
    {
        #region Fields

        internal string RowColumnID;

        private SystemMessageStyle _systemMessageStyle = SystemMessageStyle.WebGrid;
        private bool _userFriendly = true;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemMessage">error</see> class.
        /// </summary>
        /// <param name="systemMessage">The error to be shown.</param>
        /// <param name="critical">Critical errors are errors not caused by users. Critical errors halt most actions after the error occurred.</param>
        /// <param name="systemMessageStyle">Determines how the error should be displayed.</param>
        /// <param name="rowColumnId">Determines which row and column the error should be displayed in.</param>
        public SystemMessage(string systemMessage, bool critical, SystemMessageStyle systemMessageStyle,
            string rowColumnId)
        {
            AddSystemMessage(systemMessage, critical, systemMessageStyle, rowColumnId);
        }

        /// <summary>
        /// Constructs a new error.
        /// </summary>
        /// <param name="systemMessage">The error to be shown.</param>
        /// <param name="systemMessageStyle">Determines how the error should be displayed.</param>
        /// <param name="rowColumnId">Determines which row and column the error should be displayed in.</param>
        public SystemMessage(string systemMessage, SystemMessageStyle systemMessageStyle,
            string rowColumnId)
        {
            AddSystemMessage(systemMessage, false, systemMessageStyle, rowColumnId);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets whether the error is critical.
        /// Critical errors are errors not caused by users. Critical systemMessages
        /// halt most actions after the error occurred.
        /// </summary>
        /// <value><c>true</c> if critical; otherwise, <c>false</c>.</value>
        public bool Critical
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether extra debug information should be displayed.
        /// </summary>
        /// <value><c>true</c> if debug; otherwise, <c>false</c>.</value>
        public bool Debug
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>The error.</value>
        public string Message
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets how the error should be displayed.
        /// </summary>
        /// <value>The error style.</value>
        public SystemMessageStyle SystemMessageStyle
        {
            get { return _systemMessageStyle; }
            set { _systemMessageStyle = value; }
        }

        /// <summary>
        /// Gets or sets whether the error should be displayed to the user.
        /// </summary>
        /// <value><c>true</c> if [user friendly]; otherwise, <c>false</c>.</value>
        public bool UserFriendly
        {
            get { return _userFriendly; }
            set { _userFriendly = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Queries for debug information.
        /// </summary>
        internal static string QueryDebug()
        {
            if (!Grid.GotHttpContext || HttpContext.Current.Request.Cookies == null)
                return null;
            StringBuilder debugBuilder = new StringBuilder("<table width=400>");

            debugBuilder.Append("<tr><td colspan=\"2\"><b>Server variables</b></td>\r\n");

            foreach (string name in HttpContext.Current.Request.ServerVariables)
            {
                debugBuilder.AppendFormat("<tr><td width=200>{0}</td>", name);
                debugBuilder.AppendFormat("<td>{0}</td></tr>\r\n", HttpContext.Current.Request.ServerVariables[name]);
            }

            debugBuilder.AppendFormat("<tr><td colspan=\"2\"><b>Server sessions (There are {0} Session variables)</b></td>\r\n", HttpContext.Current.Session.Contents.Count);
            foreach (string name in HttpContext.Current.Session.Contents)
            {
                debugBuilder.AppendFormat("<tr><td width=200>{0}</td>", name);
                debugBuilder.AppendFormat("<td>{0}</td></tr>\r\n", (object[])HttpContext.Current.Session[name]);
            }
            debugBuilder.AppendFormat("<tr><td colspan=\"2\"><b>Cookies (There are {0} cookies)</b></td>\r\n", HttpContext.Current.Request.Cookies.Count);
            foreach (string name in HttpContext.Current.Request.Cookies.Keys)
            {
                if (HttpContext.Current.Request.Cookies != null)
                {
                    if ( string.IsNullOrEmpty(name) || HttpContext.Current.Request.Cookies[name] == null)
                        continue;
                    debugBuilder.AppendFormat("<tr><td width=200>{0}({1})</td>", name,
                                              HttpContext.Current.Request.Cookies[name]);

            // ReSharper disable PossibleNullReferenceException
                        debugBuilder.AppendFormat("<td>{0}</td></tr>\r\n", HttpContext.Current.Request.Cookies[name].Value);
            // ReSharper restore PossibleNullReferenceException
                }
            }
            return debugBuilder.ToString();
        }

        /// <summary>
        /// Adds the systemMessage.
        /// </summary>
        /// <param name="systemMessage">The error.</param>
        /// <param name="critical">if set to <c>true</c> [critical].</param>
        /// <param name="systemMessageStyle">The error style.</param>
        /// <param name="rowColumnId">The row column ColumnId.</param>
        private void AddSystemMessage(string systemMessage, bool critical,
            SystemMessageStyle systemMessageStyle, string rowColumnId)
        {
            Message = systemMessage;
            Critical = critical;
            SystemMessageStyle = systemMessageStyle;
            RowColumnID = rowColumnId;
        }

        #endregion Methods
    }
}