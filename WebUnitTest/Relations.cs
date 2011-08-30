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


using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Web.UI;
using NUnit.Framework;
using WebGrid;
using WebGrid.Enums;
using WebGridUnitTest;
using System.Collections.Specialized;

namespace WebGridUnitTest
{
    [TestFixture]
    public class Relations : UnitTestCommon
    {
        [Test]
        public void GetEmployeesAccessMasterGrid()
        {
            Grid mastergrid = new Grid();
            mastergrid.DataSourceId = "Employees";
            mastergrid.ID = "test";
            mastergrid.Page = Testpage;
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


            Assert.Greater(mastergrid.MasterTable.Rows.Count, 1);
            Assert.IsTrue(mastergrid.MasterTable.GotDataSourceSchema);
            Assert.IsFalse(slavegrid.MasterTable.GotDataSourceSchema);
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
            Assert.IsTrue(mastergrid.MasterTable.GotDataSourceSchema);
            Assert.IsFalse(slavegrid.MasterTable.GotDataSourceSchema);
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
            mastergrid.CurrentId = "3";
            mastergrid["Photo"].Visibility = Visibility.None; //HtmlForm is required for File columns.
            mastergrid.Mode = Mode.Edit;
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
            Assert.IsFalse(slavegrid.MasterTable.GotDataSourceSchema);
        }


        [Test]
        public void GetEmployeesAccessMasterGridEdit()
        {
            Grid mastergrid = new Grid();
            mastergrid.DataSourceId = "Employees";
            mastergrid.ID = "test";
            mastergrid.Page = Testpage;
            mastergrid.CurrentId = "3";
            mastergrid["Photo"].Visibility = Visibility.None; //HtmlForm is required for File columns.
            mastergrid.Mode = Mode.Edit;
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
            Assert.IsFalse(slavegrid.MasterTable.GotDataSourceSchema);
        }

        [Test]
        public void GetEmployeesAccessMasterGridEditActiveSlaveGrid()
        {
            Grid mastergrid = new Grid();
            mastergrid.DataSourceId = "Employees";
            mastergrid.ID = "test";
            mastergrid.Page = Testpage;
            mastergrid.CurrentId = "3";
            mastergrid["Photo"].Visibility = Visibility.None; //HtmlForm is required for File columns.
            mastergrid.Mode = Mode.Edit;
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
            Assert.IsTrue(slavegrid.MasterTable.GotDataSourceSchema);
        }

        [Test]
        public void GetEmployeesAccessMasterGridEditActiveSlaveGridThenInActiveSlaveGrid()
        {
            Grid mastergrid = new Grid();
            mastergrid.DataSourceId = "Employees";
            mastergrid.ID = "test";
            mastergrid.Page = Testpage;
            mastergrid.CurrentId = "3";
            mastergrid["Photo"].Visibility = Visibility.None; //HtmlForm is required for File columns.
            mastergrid.Mode = Mode.Edit;
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
            Assert.IsFalse(slavegrid.MasterTable.GotDataSourceSchema);
        }

        [Test]
        public void GetEmployeesAccessMasterGridEditActiveSlaveGridThreeSlaveGrids()
        {
            Grid mastergrid = new Grid();
            mastergrid.DataSourceId = "Employees";
            mastergrid.ID = "test";
            mastergrid.Page = Testpage;
            mastergrid.CurrentId = "3";
            mastergrid["Photo"].Visibility = Visibility.None; //HtmlForm is required for File columns.
            mastergrid.Mode = Mode.Edit;
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
            Assert.IsTrue(slavegrid.MasterTable.GotDataSourceSchema);
            Assert.IsFalse(slavegrid2.MasterTable.GotDataSourceSchema);
            Assert.IsFalse(slavegrid3.MasterTable.GotDataSourceSchema);
        }

        [Test]
        public void GetEmployeesAccessMasterGridEditDefaultSlaveGridThreeSlaveGrids()
        {
            Testpage.Controls.Clear();
       
            Grid mastergrid = new Grid();
            mastergrid.DataSourceId = "Employees";
            mastergrid.ID = "test";
            mastergrid.Page = Testpage;
            mastergrid.CurrentId = "3";
            mastergrid["Photo"].Visibility = Visibility.None; //HtmlForm is required for File columns.
            mastergrid.Mode = Mode.Edit;
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
            Assert.IsTrue(slavegrid.MasterTable.GotDataSourceSchema);
            Assert.IsFalse(slavegrid2.MasterTable.GotDataSourceSchema);
            Assert.IsFalse(slavegrid3.MasterTable.GotDataSourceSchema);
            Assert.GreaterOrEqual(mastergrid.SlaveGrid.Count, 3);
            Assert.AreEqual(mastergrid.ActiveMenuSlaveGrid, slavegrid);
        }

        [Test]
        public void SetDataSourceForeignKey()
        {
            Grid mastergrid = new Grid();

            mastergrid.ID = "test";
            mastergrid.Page = Testpage;
          
            mastergrid.Mode = Mode.Edit;
            Testpage.Controls.Add(mastergrid);

            WebGrid.Columns.Foreignkey column = new WebGrid.Columns.Foreignkey("ForeignkeyItems", mastergrid);
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

            ((WebGrid.Columns.Foreignkey)mastergrid["ForeignkeyItems"]).Table = fktable;

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            mastergrid.RenderControl(gridwriter);

            Assert.AreEqual(mastergrid.MasterTable.Rows.Count, 1);
            Assert.IsTrue(mastergrid.MasterTable.GotDataSourceSchema);
            Assert.AreEqual(((WebGrid.Columns.Foreignkey)mastergrid["ForeignkeyItems"]).Table.Rows.Count, 7);
        }

        [Test]
        public void SetDataSourceForeignKeyStringArray()
        {
            Grid mastergrid = new Grid();

            mastergrid.ID = "test";
            mastergrid.Page = Testpage;

            mastergrid.Mode = Mode.Edit;
            Testpage.Controls.Add(mastergrid);

            WebGrid.Columns.Foreignkey column = new WebGrid.Columns.Foreignkey("ForeignkeyItems", mastergrid);
            column.NullText = "Select Category";
            column.Required = true;
            mastergrid.MasterTable.Columns.Add(column);

            WebGrid.Data.Table fktable = new WebGrid.Data.Table(mastergrid, true);
            string[] names = new[] { "Tom", "Dick", "Harry", "Thomas", "Anders", "Carl", "Trond", "Eli", "Eva" };
            fktable.DataSource = names;

            ((WebGrid.Columns.Foreignkey)mastergrid["ForeignkeyItems"]).Table = fktable;

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            mastergrid.RenderControl(gridwriter);

            Assert.AreEqual(mastergrid.MasterTable.Rows.Count, 1);
            Assert.IsTrue(mastergrid.MasterTable.GotDataSourceSchema);
            Assert.AreEqual(((WebGrid.Columns.Foreignkey)mastergrid["ForeignkeyItems"]).Table.Rows.Count, names.Length);
        }


    }
}