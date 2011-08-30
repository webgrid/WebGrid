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

namespace WebGridUnitTest
{
    using System;
    using System.IO;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.UI;

    /// <summary>
    /// Used to simulate an HttpRequest.
    /// </summary>
    public class SimulatedHttpRequest : SimpleWorkerRequest
    {
        #region Fields

        string _host;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="SimulatedHttpRequest"/> instance.
        /// </summary>
        /// <param name="appVirtualDir">App virtual dir.</param>
        /// <param name="appPhysicalDir">App physical dir.</param>
        /// <param name="page">Page.</param>
        /// <param name="query">Query.</param>
        /// <param name="output">Output.</param>
        /// <param name="host">Host.</param>
        public SimulatedHttpRequest(string appVirtualDir, string appPhysicalDir, string page, string query, TextWriter output, string host)
            : base(appVirtualDir, appPhysicalDir, page, query, output)
        {
            if(string.IsNullOrEmpty(host))

            throw new ArgumentNullException("host", "Host cannot be null nor empty.");

            _host = host;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the name of the server.
        /// </summary>
        /// <returns></returns>
        public override string GetServerName()
        {
            return _host;
        }

        /// <summary>
        /// Maps the path to a filesystem path.
        /// </summary>
        /// <param name="virtualPath">Virtual path.</param>
        /// <returns></returns>
        public override string MapPath(string virtualPath)
        {
            return Path.Combine(this.GetAppPath(), virtualPath);
        }

        #endregion Methods
    }

    public class UnitTestCommon
    {
        #region Fields

        private static string path = @"C:\WebGrid\Grid\UnitTest";

        private string connectionAccessOleDb = @"provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|Nwind.mdb";
        private string connectionSqlConnection =  @"Data Source=localhost;Initial Catalog=NorthWind;Trusted_Connection=yes;";
        private string connectionSqlOleDb = @"Provider=SQLNCLI10;Server=localhost;Database=Test;Trusted_Connection=yes;";
        private Page testpage;

        #endregion Fields

        #region Properties

        public static string Path
        {
            get { return path; }
            set { path = value; }
        }

        public string ConnectionAccessOleDb
        {
            get { return connectionAccessOleDb; }
            set { connectionAccessOleDb = value; }
        }

        public string ConnectionSqlConnection
        {
            get { return connectionSqlConnection; }
            set { connectionSqlConnection = value; }
        }

        public string ConnectionSqlOleDb
        {
            get { return connectionSqlOleDb; }
            set { connectionSqlOleDb = value; }
        }

        public Page Testpage
        {
            get
            {
            if (testpage == null)
            {

              /*  Uri url = new Uri("http://localhost");
                HttpSimulator simulator = new HttpSimulator("/", @"E:\WebGrid\Grid\UnitTest\");

                  simulator.SimulateRequest(url);
                HttpContext.Current.Handler = new CustomFormHandler();

                if (HttpContext.Current == null)
                    Console.WriteLine("Warning 'HTTPCONTEXT' is null");
                */testpage = new Page();

                //   SimpleWorkerRequest request = new SimpleWorkerRequest("/dummy", @"E:\WebGrid\Grid\UnitTest\dummy.htm", "dummy.htm", null, new StringWriter());
                //   HttpContext context = new HttpContext(request);
                //    HttpContext.Current = context;
                  testpage.ClientScript.RegisterClientScriptBlock(typeof(Page), "wgScripts", "");
            }
            return testpage;
            }
            set { testpage = value; }
        }

        #endregion Properties
    }
}