/*
Copyright ©  Olav Christian Botterli. All Rights Reserved. 

Ask for permission if you want to use or change any part of the source code.

E-mail: olav@webgrid.com, olav@botterli.com (Olav Botterli)

http://www.webgrid.com
*/


/*
Copyright ©  Olav Christian Botterli. All Rights Reserved. 

Ask for permission if you want to use or change any part of the source code.

E-mail: olav@webgrid.com, olav@botterli.com (Olav Botterli)

http://www.webgrid.com
*/


using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using NUnit.Framework;
using System;
using System.Reflection;
using Subtext.TestLibrary;

namespace WebGridUnitTest
{
    
    /// <summary>

/// Used to simulate an HttpRequest.

/// </summary>

public class SimulatedHttpRequest : SimpleWorkerRequest

{

    string _host;

 

    /// <summary>

    /// Creates a new <see cref="SimulatedHttpRequest"/> instance.

    /// </summary>

    /// <param name="appVirtualDir">App virtual dir.</param>

    /// <param name="appPhysicalDir">App physical dir.</param>

    /// <param name="page">Page.</param>

    /// <param name="query">Query.</param>

    /// <param name="output">Output.</param>

    /// <param name="host">Host.</param>

    public SimulatedHttpRequest(string appVirtualDir, string appPhysicalDir, string page, string query, TextWriter output, string host) : base(appVirtualDir, appPhysicalDir, page, query, output)

    {

        if(host == null || host.Length == 0)

            throw new ArgumentNullException("host", "Host cannot be null nor empty.");

        _host = host;

    }

 

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

}





    public class UnitTestCommon
    {
        private static string path = @"E:\WebGrid\Grid\UnitTest";

        public static string Path
        {
            get { return path; }
            set { path = value; }
        }

        private string connectionAccessOleDb = @"provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|Nwind.mdb";

        public string ConnectionAccessOleDb
        {
            get { return connectionAccessOleDb; }
            set { connectionAccessOleDb = value; }
        }

        private string connectionSqlConnection =
            @"Data Source=localhost\MSSQLSERVER2008;Initial Catalog=Test;Trusted_Connection=yes;";

        public string ConnectionSqlConnection
        {
            get { return connectionSqlConnection; }
            set { connectionSqlConnection = value; }
        }

        private string connectionSqlOleDb = @"Provider=SQLNCLI10;Server=localhost\MSSQLSERVER2008;Database=Test;Trusted_Connection=yes;";

        public string ConnectionSqlOleDb
        {
            get { return connectionSqlOleDb; }
            set { connectionSqlOleDb = value; }
        }

        private Page testpage;

        public Page Testpage
        {
            get
            {
                if (testpage == null)
                {
             //       _hostName = UnitTestHelper.GenerateUniqueHost();

   // UnitTestHelper.SetHttpContextWithBlogRequest(_hostName, "MyBlog");
                  //  HttpSimulator t = new HttpSimulator();
                    Uri url = new Uri("http://localhost/Test/Test.aspx");
                    HttpSimulator simulator = new HttpSimulator("/Test", @"c:\inetpub\wwwroot\");
                    simulator.SimulateRequest(url);

                        
                    testpage = new Page();
                 //   SimpleWorkerRequest request = new SimpleWorkerRequest("/dummy", @"E:\WebGrid\Grid\UnitTest\dummy.htm", "dummy.htm", null, new StringWriter());
                 //   HttpContext context = new HttpContext(request);
                //    HttpContext.Current = context;
                     if (HttpContext.Current == null)
                        Console.WriteLine("Warning 'HTTPCONTEXT' is null");
                    testpage.ClientScript.RegisterClientScriptBlock(typeof (Page), "wgScripts", "");
                  }
                return testpage;
            }
            set { testpage = value; }
        }
    }
}