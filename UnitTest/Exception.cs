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
    using System.IO;
    using System.Text;
    using System.Web.UI;

    using NUnit.Framework;

    using WebGrid;

    using WebGridUnitTest;

    [TestFixture]
    public class Exception : UnitTestCommon
    {
        #region Methods

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
            Grid grid = new Grid
                            {
                                DataSourceId = "Employees",
                                ID = "test",
                                FilterExpression = "ErrorInWhere",
                                ConnectionString = ConnectionAccessOleDb
                            };
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

        #endregion Methods
    }
}