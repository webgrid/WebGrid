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
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using NUnit.Framework;

    using WebGrid;
    using WebGrid.Enums;
    using SystemColumn = WebGrid.Enums.SystemColumn;
    using File = WebGrid.File;
    using Foreignkey = WebGrid.Foreignkey;
    using ManyToMany = WebGrid.ManyToMany;

    [TestFixture]
    public class Properties : UnitTestCommon
    {
        #region Methods

        [Test]
        public void GetAccessOleDBAllowEditInGridTFalse()
        {
            var grid = new Grid
                           {
                               Sql = "SELECT * FROM Employees",
                               ID = "test",
                               DefaultVisibility = Visibility.Both,
                               ConnectionString = ConnectionAccessOleDb
                           };
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.AllowEditInGrid, false);
        }

        [Test]
        public void GetAccessOleDBAllowEditInGridTrue()
        {
            var grid = new Grid {Sql = "SELECT * FROM Employees", ID = "test"};
            grid["FirstName"].AllowEditInGrid = true;
            grid["LastName"].AllowEditInGrid = true;
            grid.DefaultVisibility = Visibility.Both;
            grid.ConnectionString = ConnectionAccessOleDb;
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.AllowEditInGrid, true);
        }

        [Test]
        public void GetAccessOleDBDisplayFalse()
        {
            var grid = new Grid
                           {
                               Sql = "SELECT * FROM Employees",
                               ID = "test",
                               DisplayHeader = false,
                               DisplayRequiredColumn = false,
                               DisplaySearchBox = Position.Hide,
                               DisplaySlaveGridMenu = false,
                               DisplaySortIcons = false,
                               DisplayTitle = false,
                               DefaultVisibility = Visibility.Both,
                               ConnectionString = ConnectionAccessOleDb
                           };
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
        }

        [Test]
        public void GetAccessOleDBEditMode()
        {
            var grid = new Grid {Sql = "SELECT * FROM Employees", ID = "test", EditIndex = "3"};
            grid["Photo"].Visibility = Visibility.None; // File columns requires Form element therefore No visibility.
            grid.DisplayView = DisplayView.Detail;
            grid.DefaultVisibility = Visibility.Both;
            grid.ConnectionString = ConnectionAccessOleDb;
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.Rows.Count, 1);
        }

        [Test]
        public void GetAccessOleDBForeignkeyEditInEditMode()
        {
            var grid = new Grid { Sql = "SELECT * FROM Employees", ID = "test", EditIndex = "3" };
            grid["Photo"].Visibility = Visibility.None; // File columns requires Form element therefore No visibility.
            grid["ReportsTo"].ColumnType = ColumnType.Foreignkey;
            grid["ReportsTo"].DataSourceId = "Employees";
            grid.DisplayView = DisplayView.Detail;
            grid.DefaultVisibility = Visibility.Both;
            grid.ConnectionString = ConnectionAccessOleDb;
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.Rows.Count, 1);
            Assert.Greater(((Foreignkey)grid.MasterTable.Columns["ReportsTo"]).Table.RecordCount, 1);
        }

        [Test]
        public void GetAccessOleDBForeignkeyEditInGridMode()
        {
            var grid = new Grid {Sql = "SELECT * FROM Employees", ID = "test"};
            grid["ReportsTo"].AllowEditInGrid = true;
            grid["ReportsTo"].ColumnType = ColumnType.Foreignkey;
            grid["ReportsTo"].DataSourceId = "Employees";
            grid.DefaultVisibility = Visibility.Both;
            grid.ConnectionString = ConnectionAccessOleDb;

            Assert.Greater(grid.MasterTable.RecordCount, 1);
            Assert.Greater(((Foreignkey)grid.MasterTable.Columns["ReportsTo"]).Table.RecordCount, 1);
        }

        [Test]
        public void GetAccessOleDBForeignkeyEditInGridModeViewDataSource()
        {
            var grid = new Grid {Sql = "SELECT * FROM Employees", ID = "test"};
            grid["ReportsTo"].AllowEditInGrid = true;
            grid["ReportsTo"].ColumnType = ColumnType.Foreignkey;
            grid["ReportsTo"].DataSourceId = "Employees";
            grid.ConnectionString = ConnectionAccessOleDb;
            ((Foreignkey) grid["ReportsTo"]).IdentityColumn = "EmployeeID";
            grid.DefaultVisibility = Visibility.Both;

            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.RecordCount, 1);
            Assert.Greater(((Foreignkey)grid.MasterTable.Columns["ReportsTo"]).Table.RecordCount, 1);
        }

        [Test]
        public void GetAccessOleDBForeignkeyTreeAllowEditInGrid()
        {
            var grid = new Grid { DataSourceId = "Orders", ID = "test" };
            grid["CustomerID"].ColumnType = ColumnType.Number;
            grid["EmployeeId"].ColumnType = ColumnType.Foreignkey;
            grid["EmployeeId"].DataSourceId = "Employees";
            grid["EmployeeId"].AllowEditInGrid = true;
            ((Foreignkey)grid["EmployeeId"]).SortExpression = "FirstName +' '+LastName";
            ((Foreignkey)grid["EmployeeID"]).ValueColumn = "FirstName +' '+LastName";
            grid["EmployeeId"].TreeIndentText = "--";
            ((Foreignkey)grid["EmployeeId"]).TreeParentId = "ReportsTo";
            grid.DisplayView = DisplayView.Detail;
            grid.DefaultVisibility = Visibility.Both;
            grid.ConnectionString = ConnectionAccessOleDb;
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.Rows.Count, 1);
            Assert.Greater(sb.ToString().IndexOf("<option value=\"5\">--Steven Buchanan</option>"), -1,sb.ToString());
            Assert.Greater(((Foreignkey)grid.MasterTable.Columns["EmployeeId"]).Table.RecordCount, 1);
        }

        [Test]
        public void GetAccessOleDBForeignkeyWhereEditInEditMode()
        {
            var grid = new Grid
                           {
                               Sql = "SELECT EmployeeId FROM Employees",
                               ID = "uniquetest",
                               DisplayView = DisplayView.Detail,
                               EditIndex = "3",
                               CacheGridStructure = false,
                           };
            //      grid["EmployeeId"].Primarykey = true;
            grid["Photo"].Visibility = Visibility.None; // File columns requires Form element therefore No visibility.
            grid["ReportsTo"].ColumnType = ColumnType.Foreignkey;
            grid["ReportsTo"].DataSourceId = "Employees";
            ((Foreignkey) grid["ReportsTo"]).TreeParentId = "EmployeeId";

            ((Foreignkey) grid["ReportsTo"]).FilterExpression = "[Employees].[EmployeeId] = -1";
            grid.DisplayView = DisplayView.Detail;
            grid.DefaultVisibility = Visibility.Both;
            grid.ConnectionString = ConnectionAccessOleDb;
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.Rows.Count, 1);
            Assert.AreEqual(((Foreignkey)grid.MasterTable.Columns["ReportsTo"]).Table.RecordCount, 0);
        }

        [Test]
        public void GetAccessOleDBLoadXmlWebGridMessages()
        {
            var grid = new Grid
                           {
                               Sql = "SELECT * FROM Employees",
                               ID = "test",
                               Language = SystemLanguage.Norwegian,
                               SystemMessageDataFile = (Path + "\\WebGridMessages.xml"),
                               ConnectionString = ConnectionAccessOleDb
                           };
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
        }

        [Test]
        public void GetAccessOleDBManyToManyWhereEditInEditMode()
        {
            var grid = new Grid { Sql = "SELECT * FROM Employees", ID = "test", EditIndex = "3" };
            grid["Photo"].Visibility = Visibility.None; // File columns requires Form element therefore No visibility.
            grid["Territories"].ColumnType = ColumnType.ManyToMany;
            ((ManyToMany) grid["Territories"]).ForeignDataSource = "Territories";
            ((ManyToMany) grid["Territories"]).MatrixIdentityColumn = "EmployeeID";
            ((ManyToMany) grid["Territories"]).ValueColumn = "TerritoryDescription";
            ((ManyToMany) grid["Territories"]).MatrixDataSource = "EmployeeTerritories";
            ((ManyToMany) grid["Territories"]).ForeignIdentityColumn = "TerritoryID";
            ((ManyToMany) grid["Territories"]).ForeignDataSource = "Territories";
            ((ManyToMany) grid["Territories"]).FilterExpression = "RegionID = 3";
            grid.DisplayView = DisplayView.Detail;
            grid.DefaultVisibility = Visibility.Both;
            grid.ConnectionString = ConnectionAccessOleDb;
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.Rows.Count, 1);
            Assert.Greater(((ManyToMany)grid.MasterTable.Columns["Territories"]).Items.Count, 1);
        }

        [Test]
        public void GetAccessOleDBRecordCount()
        {
            var grid = new Grid
                           {
                               Sql = "SELECT * FROM Employees",
                               ID = "test",
                               DefaultVisibility = Visibility.Both,
                               ConnectionString = ConnectionAccessOleDb
                           };

            Assert.Greater(grid.MasterTable.RecordCount, 1);
        }

        [Test]
        public void GetAccessOleDBSearch()
        {
            var grid = new Grid
                           {
                               DataSourceId = "Employees",
                               ID = "test",
                               Search = "bo",
                               SortExpression = "[Employees].[LastName], [Employees].[FirstName]",
                               ConnectionString = ConnectionAccessOleDb
                           };
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.Rows.Count, 1);
        }

        [Test]
        public void GetAccessOleDBSortExpression()
        {
            var grid = new Grid
                           {
                               Sql = "SELECT * FROM Employees",
                               ID = "test",
                               SortExpression = "[Employees].[LastName], [Employees].[FirstName]",
                               ConnectionString = ConnectionAccessOleDb
                           };
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
        }

        [Test]
        public void GetAccessOleDBWhereAndSortExpression()
        {
            var grid = new Grid
                           {
                               Sql = "SELECT * FROM Employees",
                               ID = "test",
                               FilterExpression = "[Employees].[EmployeeID] < 1000 AND [Employees].[FirstName] NOT LIKE '%bo%'",
                               SortExpression = "[Employees].[LastName], [Employees].[FirstName]",
                               ConnectionString = ConnectionAccessOleDb
                           };
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
        }

        [Test]
        public void GetAccessOleDBWhereAndSortExpressionAndPager()
        {
            var grid = new Grid
                           {
                               Sql = "SELECT * FROM Employees",
                               ID = "test",
                               PageSize = 3,
                               FilterExpression = "[Employees].[EmployeeID] < 1000 AND [Employees].[FirstName] NOT LIKE '%bo%'",
                               SortExpression = "[Employees].[LastName], [Employees].[FirstName]",
                               ConnectionString = ConnectionAccessOleDb
                           };
            grid.PagerSettings.PagerType = PagerType.Undefined;
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.Rows.Count, 3);
        }

        [Test]
        public void GetAccessOleDbWhere()
        {
            var grid = new Grid
                           {
                               Sql = "SELECT * FROM Employees",
                               ID = "test",
                               FilterExpression = "[Employees].[EmployeeID] < 1000 AND [Employees].[FirstName] NOT LIKE '%bo%'",
                               ConnectionString = ConnectionAccessOleDb
                           };
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
        }

        [Test]
        public void GetEmployeesAccessAllowFalse()
        {
            var grid = new Grid
                           {
                               DataSourceId = "Employees",
                               ID = "test",
                               PageSize = 34,
                               AllowCancel = false,
                               AllowCopy = false,
                               AllowDelete = false,
                               AllowNew = false,
                               AllowSort = false,
                               AllowEdit = false,
                               AllowUpdate = false,
                               ConnectionString = ConnectionAccessOleDb
                           };
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            string content = sb.ToString();
            Assert.AreEqual(content.IndexOf("delete.gif"), -1);
            Assert.AreEqual(content.IndexOf("edit.gif"), -1);
            Assert.AreEqual(content.IndexOf("SearchClick"), -1);
            Assert.AreEqual(content.IndexOf("UpdateRowsClick"), -1);
            Assert.AreEqual(content.IndexOf("ErrorReportClick"), -1);
            Assert.AreEqual(content.IndexOf("ColumnHeaderClick"), -1);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
        }

        [Test]
        public void GetEmployeesAccessPager()
        {
            var grid = new Grid
                           {
                               DataSourceId = "Employees",
                               ID = "test",
                               PageSize = 3,
                               ConnectionString = ConnectionAccessOleDb
                           };
            grid.PagerSettings.PagerType = PagerType.DropDown;
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.Rows.Count, 3);
        }

        [Test]
        public void GetEmployeesAccessSystemMessage()
        {
            var grid = new Grid {DataSourceId = "Employees", ID = "test90"};
            grid.SystemMessage.Add("blahblah");
            grid.SystemMessage.Add("This is an test", false);
            grid.SystemMessage.Add("blah", SystemMessageStyle.ColumnBottom);
            grid.SystemMessage.Add("blah2", SystemMessageStyle.ColumnLeft);
            grid.SystemMessage.Add("blah3", SystemMessageStyle.ColumnTop);
            grid.SystemMessage.Add("blah4", SystemMessageStyle.WebGrid);
            grid.ConnectionString = ConnectionAccessOleDb;
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.SystemMessage.Count, 6);

            Assert.Greater(grid.MasterTable.Rows.Count, 1);
        }

        [Test]
        public void GetEmployeesAccessSystemMessageCritical()
        {
            var grid = new Grid {DataSourceId = "Employees", ID = "test"};
            grid.SystemMessage.Add("blahblah");
            grid.SystemMessage.Add("This is an test", true);
            grid.ConnectionString = ConnectionAccessOleDb;
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.Rows.Count, 0);
        }

        [Test]
        public void GetHideIfEmpty()
        {
            var grid = new Grid
            {
                ID = "test",
                PageSize = 34,
                AllowCancel = false,
                AllowCopy = false,
                AllowDelete = false,
                AllowNew = false,
                AllowSort = false,
                AllowEdit = false,
                AllowUpdate = false,
                ConnectionString = ConnectionAccessOleDb
            //    ,HideIfEmptyGrid = true
            };
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            string content = sb.ToString();
            Assert.Greater(content.Length, 250);

            grid = new Grid
            {
                ID = "test",
                PageSize = 34,
                AllowCancel = false,
                AllowCopy = false,
                AllowDelete = false,
                AllowNew = false,
                AllowSort = false,
                AllowEdit = false,
                AllowUpdate = false,
                ConnectionString = ConnectionAccessOleDb
                 ,HideIfEmptyGrid = true
            };
             sb = new StringBuilder();
             sw = new StringWriter(sb);
             gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
             content = sb.ToString();
            Assert.AreEqual(content, string.Empty);
        }

        [Test]
        public void GridFilterByColumn()
        {
            var grid = new Grid {DataSourceId = "test"};

            Testpage.Controls.Add(grid);

            grid.ID = "test";
            grid.ConnectionString = ConnectionSqlConnection;
            grid["imgtest"].ColumnType = ColumnType.File;
            grid["imgtest"].Visibility = Visibility.None;
            grid["dtmDate"].FilterByColumn = true;
            grid["inttest"].FilterByColumn = true;
            ((File) grid["imgtest"]).Directory = Path;
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
            if (sb.ToString().IndexOf("ddltest__inttest", StringComparison.OrdinalIgnoreCase) == -1)
                Console.WriteLine("Search options is not being rendered. (HttpContext was missing)");
        }

        [Test]
        public void GridSelectableRows()
        {
            var tmp = new Grid();

            var column = new WebGrid.SystemColumn("column", SystemColumn.SelectColumn,
                                                          tmp.MasterTable) {Visibility = Visibility.Both};

            tmp.MasterTable.Columns.Add(column);

            tmp.ID = "wggrid";
            tmp.DefaultVisibility = Visibility.Both;
            tmp.RecordsPerRow = 2;
            tmp.PagerSettings.PagerType = PagerType.None;
            tmp.Width = Unit.Pixel(1000);

            Testpage.Controls.Add(tmp);

            tmp.DisplayView = DisplayView.Grid;
            tmp.DataSourceId = "Categories";
            tmp.ConnectionString = ConnectionAccessOleDb;
            tmp.SetSelectedRows = "1!3!5";
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var gridwriter = new HtmlTextWriter(sw);
            tmp.RenderControl(gridwriter);
            Assert.Greater(tmp.MasterTable.Rows.Count, 0);
            Assert.LessOrEqual(tmp.SystemMessage.Count, 1); // 15-day license key message
            Assert.AreEqual(tmp.DisplayView, DisplayView.Grid);
            Assert.AreEqual(tmp.GetGetSelectedRows().Length, 3);
        }

        #endregion Methods
    }
}