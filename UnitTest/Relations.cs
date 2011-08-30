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
    using System.Collections.Specialized;
    using System.Data.OleDb;
    using System.IO;
    using System.Text;
    using System.Web.UI;

    using NUnit.Framework;

    using WebGrid;
    using WebGrid.Enums;

    [TestFixture]
    public class Relations : UnitTestCommon
    {
        #region Methods

        [Test]
        public void GetEmployeesAccessMasterGrid()
        {
            Grid mastergrid = new Grid
                                  {
                                      DataSourceId = "Employees",
                                      ID = "test",
                                      Page = Testpage,
                                      ConnectionString = ConnectionAccessOleDb
                                  };
            Testpage.Controls.Add(mastergrid);

            Grid slavegrid = new Grid
                                 {
                                     DataSourceId = "Territories",
                                     ID = "test2",
                                     Page = Testpage,
                                     MasterGrid = "test",
                                     ConnectionString = ConnectionAccessOleDb
                                 };
            Testpage.Controls.Add(slavegrid);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            mastergrid.RenderControl(gridwriter);

            sb = new StringBuilder();
            sw = new StringWriter(sb);
            gridwriter = new HtmlTextWriter(sw);
            slavegrid.RenderControl(gridwriter);

            Assert.Greater(mastergrid.MasterTable.Rows.Count, 1);
            Assert.IsTrue(mastergrid.MasterTable.GotSchema);
            Assert.IsFalse(slavegrid.MasterTable.GotSchema);
        }

        [Test]
        public void GetEmployeesAccessMasterGridEdit()
        {
            Grid mastergrid = new Grid();
            mastergrid.DataSourceId = "Employees";
            mastergrid.ID = "test";
            mastergrid.Page = Testpage;
            mastergrid.EditIndex = "3";
            mastergrid["Photo"].Visibility = Visibility.None; //HtmlForm is required for File columns.
            mastergrid.DisplayView = DisplayView.Detail;
            mastergrid.ConnectionString = ConnectionAccessOleDb;
            Testpage.Controls.Add(mastergrid);

            Grid slavegrid = new Grid();
            slavegrid.DataSourceId = "Territories";
            slavegrid.ID = "test2";
            slavegrid.Page = Testpage;
            slavegrid.MasterGrid = "test";
            slavegrid.ConnectionString = ConnectionAccessOleDb;
            Testpage.Controls.Add(slavegrid);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            mastergrid.RenderControl(gridwriter);

            sb = new StringBuilder();
            sw = new StringWriter(sb);
            gridwriter = new HtmlTextWriter(sw);
            slavegrid.RenderControl(gridwriter);

            Assert.AreEqual(mastergrid.MasterTable.Rows.Count, 1);
            Assert.IsFalse(slavegrid.MasterTable.GotSchema);
        }

        [Test]
        public void GetEmployeesAccessMasterGridEditActiveSlaveGrid()
        {
            Grid mastergrid = new Grid {DataSourceId = "Employees", ID = "test", Page = Testpage, EditIndex = "3"};
            mastergrid["Photo"].Visibility = Visibility.None; //HtmlForm is required for File columns.
            mastergrid.DisplayView = DisplayView.Detail;
            mastergrid.ConnectionString = ConnectionAccessOleDb;
            Testpage.Controls.Add(mastergrid);

            Grid slavegrid = new Grid();
            slavegrid.DataSourceId = "EmployeeTerritories";
            slavegrid.ID = "test2";
            slavegrid.Page = Testpage;
            slavegrid.MasterGrid = "test";
            slavegrid.ConnectionString = ConnectionAccessOleDb;
            Testpage.Controls.Add(slavegrid);

            mastergrid.ActiveMenuSlaveGrid = slavegrid;

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            mastergrid.RenderControl(gridwriter);

            sb = new StringBuilder();
            sw = new StringWriter(sb);
            gridwriter = new HtmlTextWriter(sw);
            slavegrid.RenderControl(gridwriter);

            Assert.AreEqual(mastergrid.MasterTable.Rows.Count, 1);
            Assert.Greater(slavegrid.MasterTable.Rows.Count, 1);
            Assert.IsTrue(slavegrid.MasterTable.GotSchema);
        }

        [Test]
        public void GetEmployeesAccessMasterGridEditActiveSlaveGridThenInActiveSlaveGrid()
        {
            Grid mastergrid = new Grid();
            mastergrid.DataSourceId = "Employees";
            mastergrid.ID = "test";
            mastergrid.Page = Testpage;
            mastergrid.EditIndex = "3";
            mastergrid["Photo"].Visibility = Visibility.None; //HtmlForm is required for File columns.
            mastergrid.DisplayView = DisplayView.Detail;
            mastergrid.ConnectionString = ConnectionAccessOleDb;
            Testpage.Controls.Add(mastergrid);

            Grid slavegrid = new Grid();
            slavegrid.DataSourceId = "Territories";
            slavegrid.ID = "test2";
            slavegrid.Page = Testpage;
            slavegrid.MasterGrid = "test";
            slavegrid.ConnectionString = ConnectionAccessOleDb;
            Testpage.Controls.Add(slavegrid);

            mastergrid.ActiveMenuSlaveGrid = slavegrid;
            mastergrid.ActiveMenuSlaveGrid = null;

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            mastergrid.RenderControl(gridwriter);

            sb = new StringBuilder();
            sw = new StringWriter(sb);
            gridwriter = new HtmlTextWriter(sw);
            slavegrid.RenderControl(gridwriter);

            Assert.AreEqual(mastergrid.MasterTable.Rows.Count, 1);
            Assert.IsFalse(slavegrid.MasterTable.GotSchema);
        }

        [Test]
        public void GetEmployeesAccessMasterGridEditActiveSlaveGridThreeSlaveGrids()
        {
            Grid mastergrid = new Grid();
            mastergrid.DataSourceId = "Employees";
            mastergrid.ID = "test";
            mastergrid.Page = Testpage;
            mastergrid.EditIndex = "3";
            mastergrid["Photo"].Visibility = Visibility.None; //HtmlForm is required for File columns.
            mastergrid.DisplayView = DisplayView.Detail;
            mastergrid.ConnectionString = ConnectionAccessOleDb;
            Testpage.Controls.Add(mastergrid);

            Grid slavegrid = new Grid();
            slavegrid.DataSourceId = "EmployeeTerritories";
            slavegrid.ID = "test2";
            slavegrid.Page = Testpage;
            slavegrid.MasterGrid = "test";
            slavegrid.ConnectionString = ConnectionAccessOleDb;
            Testpage.Controls.Add(slavegrid);

            Grid slavegrid2 = new Grid();
            slavegrid2.DataSourceId = "EmployeeTerritories";
            slavegrid2.ID = "test3";
            slavegrid2.Page = Testpage;
            slavegrid2.MasterGrid = "test";
            slavegrid2.ConnectionString = ConnectionAccessOleDb;
            Testpage.Controls.Add(slavegrid2);

            Grid slavegrid3 = new Grid();
            slavegrid3.DataSourceId = "EmployeeTerritories";
            slavegrid3.ID = "test3";
            slavegrid3.Page = Testpage;
            slavegrid3.MasterGrid = "test";
            slavegrid3.ConnectionString = ConnectionAccessOleDb;
            Testpage.Controls.Add(slavegrid3);

            mastergrid.ActiveMenuSlaveGrid = slavegrid;

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            mastergrid.RenderControl(gridwriter);

            sb = new StringBuilder();
            sw = new StringWriter(sb);
            gridwriter = new HtmlTextWriter(sw);
            slavegrid.RenderControl(gridwriter);

            Assert.AreEqual(mastergrid.MasterTable.Rows.Count, 1);
            Assert.Greater(slavegrid.MasterTable.Rows.Count, 1);
            Assert.IsTrue(slavegrid.MasterTable.GotSchema);
            Assert.IsFalse(slavegrid2.MasterTable.GotSchema);
            Assert.IsFalse(slavegrid3.MasterTable.GotSchema);
        }

        [Test]
        public void GetEmployeesAccessMasterGridEditDefaultSlaveGridThreeSlaveGrids()
        {
            Testpage.Controls.Clear();

            Grid mastergrid = new Grid();
            mastergrid.DataSourceId = "Employees";
            mastergrid.ID = "test";
            mastergrid.Page = Testpage;
            mastergrid.EditIndex = "3";
            mastergrid["Photo"].Visibility = Visibility.None; //HtmlForm is required for File columns.
            mastergrid.DisplayView = DisplayView.Detail;
            mastergrid.ConnectionString = ConnectionAccessOleDb;
            Testpage.Controls.Add(mastergrid);

            Grid slavegrid = new Grid();
            slavegrid.DataSourceId = "EmployeeTerritories";
            slavegrid.ID = "test2";
            slavegrid.Page = Testpage;
            slavegrid.MasterGrid = "test";
            slavegrid.ConnectionString = ConnectionAccessOleDb;
            Testpage.Controls.Add(slavegrid);

            Grid slavegrid2 = new Grid();
            slavegrid2.DataSourceId = "EmployeeTerritories";
            slavegrid2.ID = "test3";
            slavegrid2.Page = Testpage;
            slavegrid2.MasterGrid = "test";
            slavegrid2.ConnectionString = ConnectionAccessOleDb;
            Testpage.Controls.Add(slavegrid2);

            Grid slavegrid3 = new Grid();
            slavegrid3.DataSourceId = "EmployeeTerritories";
            slavegrid3.ID = "test3";
            slavegrid3.Page = Testpage;
            slavegrid3.MasterGrid = "test";
            slavegrid3.ConnectionString = ConnectionAccessOleDb;
            Testpage.Controls.Add(slavegrid3);

            mastergrid.DefaultSlaveGrid = "test2";

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            mastergrid.RenderControl(gridwriter);

            sb = new StringBuilder();
            sw = new StringWriter(sb);
            gridwriter = new HtmlTextWriter(sw);
            slavegrid.RenderControl(gridwriter);

            Assert.AreEqual(mastergrid.MasterTable.Rows.Count, 1);
            Assert.Greater(slavegrid.MasterTable.Rows.Count, 1);
            Assert.IsTrue(slavegrid.MasterTable.GotSchema);
            Assert.IsFalse(slavegrid2.MasterTable.GotSchema);
            Assert.IsFalse(slavegrid3.MasterTable.GotSchema);
            Assert.GreaterOrEqual(mastergrid.SlaveGrid.Count, 3);
            Assert.AreEqual(mastergrid.ActiveMenuSlaveGrid, slavegrid);
        }

        [Test]
        public void GetEmployeesAccessMasterGridEditSetDataSource()
        {
            //Setting up a standard OleDBDataAdapter datasource.
            OleDbConnection myConn = new OleDbConnection(ConnectionAccessOleDb);

            OleDbDataAdapter myCmdEmployee = new OleDbDataAdapter("SELECT * FROM [Employees]", myConn);
            OleDbDataAdapter myCmdTerritories = new OleDbDataAdapter("SELECT * FROM [Territories]", myConn);

            Grid mastergrid = new Grid();
            mastergrid.DataSource = myCmdEmployee;
            mastergrid.ID = "test";
            mastergrid.Page = Testpage;
            mastergrid.EditIndex = "3";
            mastergrid["Photo"].Visibility = Visibility.None; //HtmlForm is required for File columns.
            mastergrid.DisplayView = DisplayView.Detail;
            Testpage.Controls.Add(mastergrid);

            Grid slavegrid = new Grid();
            slavegrid.DataSource = myCmdTerritories;
            slavegrid.ID = "test2";
            slavegrid.Page = Testpage;
            slavegrid.MasterGrid = "test";
            Testpage.Controls.Add(slavegrid);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            mastergrid.RenderControl(gridwriter);

            sb = new StringBuilder();
            sw = new StringWriter(sb);
            gridwriter = new HtmlTextWriter(sw);
            slavegrid.RenderControl(gridwriter);

            Assert.AreEqual(mastergrid.MasterTable.Rows.Count, 1);
            Assert.IsFalse(slavegrid.MasterTable.GotSchema);
        }

        [Test]
        public void GetEmployeesAccessMasterGridSetDataSource()
        {
            //Setting up a standard OleDBDataAdapter datasource.
            OleDbConnection myConn = new OleDbConnection(ConnectionAccessOleDb);

            OleDbDataAdapter myCmdEmployee = new OleDbDataAdapter("SELECT * FROM [Employees]", myConn);
            OleDbDataAdapter myCmdTerritories = new OleDbDataAdapter("SELECT * FROM [Territories]", myConn);

            Grid mastergrid = new Grid();
            mastergrid.DataSource = myCmdEmployee;
            mastergrid.ID = "test";
            mastergrid.Page = Testpage;
            Testpage.Controls.Add(mastergrid);

            Grid slavegrid = new Grid();
            slavegrid.DataSourceId = "Territories";
            slavegrid.DataSource =myCmdTerritories;
            slavegrid.ID = "test2";
            slavegrid.Page = Testpage;
            slavegrid.MasterGrid = "test";
            Testpage.Controls.Add(slavegrid);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            mastergrid.RenderControl(gridwriter);

            sb = new StringBuilder();
            sw = new StringWriter(sb);
            gridwriter = new HtmlTextWriter(sw);
            slavegrid.RenderControl(gridwriter);

            Assert.Greater(mastergrid.MasterTable.Rows.Count, 1);
            Assert.IsTrue(mastergrid.MasterTable.GotSchema);
            Assert.IsFalse(slavegrid.MasterTable.GotSchema);
        }

        [Test]
        public void SetDataSourceForeignKey()
        {
            Grid mastergrid = new Grid();

            mastergrid.ID = "test";
            mastergrid.Page = Testpage;

            mastergrid.DisplayView = DisplayView.Detail;
            Testpage.Controls.Add(mastergrid);

            Foreignkey column = new Foreignkey("ForeignkeyItems", mastergrid.MasterTable);
            column.NullText = "Select Category";
            column.Required = true;
            mastergrid.MasterTable.Columns.Add(column);

            WebGrid.Data.Table fktable = new WebGrid.Data.Table(mastergrid, true);
            StringDictionary tmp = new StringDictionary();

            tmp.Add("Category 1", "1");
            tmp.Add("Category 2", "2");
            tmp.Add("Category 3", "3");
            tmp.Add("Category 4", "4");
            tmp.Add("Category 5", "5");
            tmp.Add("Category 6", "6");
            tmp.Add("Category 7", "7");
            fktable.DataSource = tmp;

            ((Foreignkey)mastergrid["ForeignkeyItems"]).Table = fktable;
            ((Foreignkey) mastergrid["ForeignkeyItems"]).ValueColumn = "Value";
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            mastergrid.RenderControl(gridwriter);

            Assert.AreEqual(mastergrid.MasterTable.Rows.Count, 1);
            Assert.IsTrue(mastergrid.MasterTable.GotSchema);
            Assert.AreEqual(((Foreignkey)mastergrid["ForeignkeyItems"]).Table.Rows.Count, 7);
        }

        [Test]
        public void SetDataSourceForeignKeyGetDataForce()
        {
            Grid mastergrid = new Grid();

            mastergrid.ID = "test";
            mastergrid.Page = Testpage;

            mastergrid.DisplayView = DisplayView.Detail;
            Testpage.Controls.Add(mastergrid);

            Foreignkey column = new Foreignkey("ForeignkeyItems", mastergrid.MasterTable);
            column.NullText = "Select Category";
            column.Required = true;
            mastergrid.MasterTable.Columns.Add(column);

            WebGrid.Data.Table fktable = new WebGrid.Data.Table(mastergrid, true);
            StringDictionary tmp = new StringDictionary();

            tmp.Add("Category 1", "1");
            tmp.Add("Category 2", "2");
            tmp.Add("Category 3", "3");
            tmp.Add("Category 4", "4");
            tmp.Add("Category 5", "5");
            tmp.Add("Category 6", "6");
            tmp.Add("Category 7", "7");
            fktable.DataSource = tmp;

            ((Foreignkey)mastergrid["ForeignkeyItems"]).Table = fktable;
            ((Foreignkey)mastergrid["ForeignkeyItems"]).ValueColumn = "Value";
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            mastergrid.RenderControl(gridwriter);

            Assert.AreEqual(mastergrid.MasterTable.Rows.Count, 1);
            Assert.IsTrue(mastergrid.MasterTable.GotSchema);
            Assert.AreEqual(((Foreignkey)mastergrid["ForeignkeyItems"]).Table.Rows.Count, 7);
            ((Foreignkey) mastergrid["ForeignkeyItems"]).Table.FilterExpression = "Value = 3";
         
            Assert.AreEqual(((Foreignkey)mastergrid["ForeignkeyItems"]).Table.Rows.Count, 1);
        }

        [Test]
        public void SetDataSourceForeignKeyStringArray()
        {
            Grid mastergrid = new Grid();

            mastergrid.ID = "test";
            mastergrid.Page = Testpage;

            mastergrid.DisplayView = DisplayView.Detail;
            Testpage.Controls.Add(mastergrid);

            Foreignkey column = new Foreignkey("ForeignkeyItems", mastergrid.MasterTable);
            column.NullText = "Select Category";
            column.Required = true;
            mastergrid.MasterTable.Columns.Add(column);

            WebGrid.Data.Table fktable = new WebGrid.Data.Table(mastergrid, true);
            string[] names = new[] { "Tom", "Dick", "Harry", "Thomas", "Anders", "Carl", "Trond", "Eli", "Eva" };
            fktable.DataSource = names;

            ((Foreignkey)mastergrid["ForeignkeyItems"]).Table = fktable;

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            mastergrid.RenderControl(gridwriter);

            Assert.AreEqual(mastergrid.MasterTable.Rows.Count, 1);
            Assert.IsTrue(mastergrid.MasterTable.GotSchema);
            Assert.AreEqual(((Foreignkey)mastergrid["ForeignkeyItems"]).Table.Rows.Count, names.Length);
        }

        #endregion Methods
    }
}