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
using System.Text;
using System.Web.UI;
using NUnit.Framework;
using WebGrid;
using WebGridUnitTest;

namespace WebGridUnitTest
{
    [TestFixture]
    public class Exception : UnitTestCommon
    {
        [Test]
        [ExpectedException("WebGrid.Design.GridException")]
        public void GetEmployeesAccessInvalidMasterGrid()
        {
            Grid grid = new Grid();
            grid.DataSourceId = "Employees";
            grid.ID = "test";
            grid.Page = Testpage;
            grid.MasterGrid = "NotFound";
            grid.ConnectionString = ConnectionAccessOleDb;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
        }

        [Test]
        [ExpectedException("WebGrid.Design.GridDataSourceException")]
        public void GetEmployeesErrorWhere()
        {
            Grid grid = new Grid();
            grid.DataSourceId = "Employees";
            grid.ID = "test";
            grid.Where = "ErrorInWhere";
            grid.ConnectionString = ConnectionAccessOleDb;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
        }

        [Test]
        [ExpectedException("WebGrid.Design.GridDataSourceException")]
        public void GetUnknownDataSource()
        {
            Grid grid = new Grid();
            grid.DataSourceId = "UnKnown";
            grid.ID = "test";
            grid.Page = Testpage;
            grid.ConnectionString = ConnectionAccessOleDb;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
        }
    }
}