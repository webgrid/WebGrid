/*
Copyright ©  Olav Christian Botterli.

Dual licensed under the MIT or GPL Version 2 licenses.

Date: 30.08.2011, Norway.

http://www.webgrid.com
*/



using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using NUnit.Framework;
using WebGrid;
using WebGrid.Enums;

namespace WebGridUnitTest
{
    [TestFixture]
    public class Grouping : UnitTestCommon
    {

        #region Methods

        [Test]
        public void AccessGrouping()
        {
            Grid grid = new Grid();
            SetupAccessGrid(grid);
            grid.GroupByExpression = "City";
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
        }

        [Test]
        public void SqlServerGrouping()
        {
            Grid grid = new Grid();
            SetupSqlServerGrid(grid);
            grid.GroupByExpression = "ProductID";
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
        }

        private void SetupSqlServerGrid(Grid grid)
        {
            grid.ID = "wggrid";
            grid.DefaultVisibility = Visibility.Both;
            grid.RecordsPerRow = 2;
            grid.PagerSettings.PagerType = PagerType.None;
            grid.Width = Unit.Pixel(1000);
            grid.DataSourceId = "Order Details";
            grid.ConnectionString = ConnectionSqlConnection;
        
            Testpage.Controls.Add(grid);
        }

        private void SetupAccessGrid(Grid grid)
        {
            grid.ID = "wggrid";
            grid.DefaultVisibility = Visibility.Both;
            grid.RecordsPerRow = 2;
            grid.PagerSettings.PagerType = PagerType.None;
            grid.Width = Unit.Pixel(1000);
            grid.DataSourceId = "Customers";
            grid.ConnectionString = ConnectionAccessOleDb;
       
            Testpage.Controls.Add(grid);
        }

        #endregion Methods
    }
}
