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


using System;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Web.UI;
using NUnit.Framework;
using WebGrid;
using WebGrid.Data;
using WebGrid.Enums;
using WebGrid.Util;
using File=WebGrid.Columns.File;
using System.Collections.Specialized;
using System.Data;

namespace WebGridUnitTest
{
    [TestFixture]
    public class DataSource : UnitTestCommon
    {
        [Test]
        public void GetEmployeesAccess()
        {
            Grid grid = new Grid();
            grid.DataSourceId = "Categories";
            grid.ID = "test";
            grid.ConnectionString = ConnectionAccessOleDb;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);

            
            foreach (Row row in grid.Rows)
            {
                object description = row["Description"].Value;
                foreach (Row row2 in grid.Rows)
                {
                    if (row == row2)
                        continue;
                object description2 = row2["Description"].Value;
                    Assert.AreNotEqual(description, description2);
                }
            }
        }

        [Test]
        public void GetEmployeesAccessPrimaryKeys()
        {
            Grid grid = new Grid();
            grid.DataSourceId = "Order Details";
            grid.ID = "test";
            grid.ConnectionString = ConnectionAccessOleDb;
            grid.GridRowBound += grid_RowPrimaryKeyCheck;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.Columns.Primarykeys.Count,  2);
            Assert.AreEqual(grid.MasterTable.Columns.Primarykeys[0].Identity, false);
            Assert.AreEqual(grid.MasterTable.Columns.Primarykeys[1].Identity, false);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
        }

        static void grid_RowPrimaryKeyCheck(object sender, WebGrid.Events.GridRowBoundEventArgs e)
        {
            Assert.IsNotEmpty(e.Row.PrimaryKeyValues);
            Assert.AreEqual(e.Row.PrimaryKeyValues.Split(',').Length, 2);
        
        }

        [Test]
        public void GetEmployeesAccessNoDataSource()
        {
            Grid grid = new Grid();
            grid.ID = "test";
            grid.ConnectionString = ConnectionAccessOleDb;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.Rows.Count, 0);
        }

        [Test]
        public void GetEmployeesAccessNoVisible()
        {
            Grid grid = new Grid();
            grid.ID = "test";
            grid.DataSourceId = "Employees";
            grid.Visible = false;
            grid.ConnectionString = ConnectionAccessOleDb;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
        }

        [Test]
        public void GetAccessOleDb()
        {
            Grid grid = new Grid();
            grid.Sql = "SELECT * FROM Employees";
            grid.ID = "test";
            grid.Page = Testpage;
            grid.ConnectionString = ConnectionAccessOleDb;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
        }

        [Test]
        public void GetAccessOleDbCacheIsFalse()
        {
            Grid grid = new Grid();
            grid.Sql = "SELECT * FROM Employees";
            grid.ID = "test";
            grid.CacheGridStructure = false;
            grid.ConnectionString = ConnectionAccessOleDb;
            grid.Page = Testpage;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
            Assert.IsFalse(grid.MasterTable.CacheDatasourceStructure);
            Assert.IsFalse(grid.MasterTable.GotDataSourceCache);
        }

        [Test]
        public void GetAccessOleDbCacheIsTrue()
        {
            Grid grid = new Grid();
            grid.Sql = "SELECT * FROM Employees";
            grid.ID = "test";
            grid.Page = Testpage;
            grid.CacheGridStructure = true;
            grid.ConnectionString = ConnectionAccessOleDb;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
            Assert.IsTrue(grid.MasterTable.CacheDatasourceStructure);
            //    Assert.IsTrue(grid.MasterTable.GotDataSourceCache); // HttpRuntime.Cache is needed to actually store the data source.
        }

        [Test]
        public void GetSetDataSourceOleDbGrid()
        {
            //Setting up a standard OleDBDataAdapter datasource.
            OleDbConnection myConn = new OleDbConnection(ConnectionAccessOleDb);

            OleDbDataAdapter myCmd = new OleDbDataAdapter("SELECT * FROM [Order Details]", myConn);

            Grid grid = new Grid();
            grid.ID = "test";

            grid.DataSource = myCmd;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.Primarykeys.Count, 2);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
        }


        [Test]
        public void GetSetDataSourceOleDbEdit()
        {
            //Setting up a standard OleDBDataAdapter datasource.
            OleDbConnection myConn = new OleDbConnection(ConnectionAccessOleDb);

            OleDbDataAdapter myCmd = new OleDbDataAdapter("SELECT * FROM [Employees]", myConn);

            Grid grid = new Grid();
            grid.ID = "test";
            grid.CurrentId = "3";
            grid.Mode = Mode.Edit;
            grid.DataSource = myCmd;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.Primarykeys.Count, 1);
            Assert.Greater(sb.ToString().IndexOf("class=\"wgupdaterecord\""), -1);
            Assert.Greater(grid.MasterTable.Columns.Count, 1);
            Assert.AreEqual(grid.MasterTable.Rows.Count, 1);
        }

        [Test]
        public void GetSetDataSourceOleDbSearch()
        {
            //Setting up a standard OleDBDataAdapter datasource.
            OleDbConnection myConn = new OleDbConnection(ConnectionAccessOleDb);

            OleDbDataAdapter myCmd = new OleDbDataAdapter("SELECT * FROM [Employees]", myConn);

            Grid grid = new Grid();
            grid.ID = "test";
             grid.Search = "doesnotexit";
            grid.DataSource = myCmd;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.DataSourceType, DataSourceControlType.EnumerableDataSource);
       
            Assert.AreEqual(grid.Primarykeys.Count, 1);
            Assert.Greater(grid.MasterTable.Columns.Count, 1);
            Assert.AreEqual(grid.MasterTable.Rows.Count, 0);
        }


        [Test]
        public void GetSetDataSourceDataTableSearch()
        {
            //Setting up a standard OleDBDataAdapter datasource.
            OleDbConnection myConn = new OleDbConnection(ConnectionAccessOleDb);

            OleDbDataAdapter myCmd = new OleDbDataAdapter("SELECT * FROM [Employees]", myConn);
            DataTable dt = new DataTable();

            myCmd.Fill(dt);
        
            Grid grid = new Grid();
            grid.ID = "test";
            grid.Search = "do";
            grid.DataSource = dt;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.DataSourceType, DataSourceControlType.EnumerableDataSource);
            //Primary key is not popuplated.
            Assert.AreEqual(grid.Primarykeys.Count, 0);
            Assert.Greater(grid.MasterTable.Columns.Count, 1);
            // Should be 6 records displayed on this search.
            Assert.AreEqual(grid.MasterTable.Rows.Count, 6);
        }

        [Test]
        public void GetDataSourceDataTable()
        {
            //Setting up a standard OleDBDataAdapter datasource.
            OleDbConnection myConn = new OleDbConnection(ConnectionAccessOleDb);

            OleDbDataAdapter myCmd = new OleDbDataAdapter("SELECT * FROM [Employees]", myConn);
            DataTable dt = new DataTable();

            myCmd.Fill(dt);
            Grid grid = new Grid();
            grid.ID = "test";
            grid.CurrentId = "3";
            grid.Mode = Mode.Edit;

            WebGrid.Columns.Number column = new WebGrid.Columns.Number("EmployeeID", grid);
            column.Required = true;
            column.Primarykey = true;
            grid.Columns.Add(column);
            grid.DataSource = dt;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.DataSourceType, DataSourceControlType.EnumerableDataSource);
            Assert.AreEqual(grid.Primarykeys.Count, 1);
            Assert.Greater(grid.MasterTable.Columns.Count, 1);
            string content = sb.ToString();
            string[] triggerStrings = new[]
                                          {
                                              "test_3_HireDate_trigger",
                                              "calendar.gif",
                                              "class=\"wgeditfield\"  value=\"Ms.\"",
                                              "value=\"08/30/1963\"",
                                              "ob=true&amp;wgfilenameoption=2&amp;wgfilename=&amp;",
                                              "class=\"wgupdaterecord\""
                                          };
            foreach (string part in triggerStrings)
            {
                int res = content.IndexOf(part);
                Assert.Greater(res, -1,part+Environment.NewLine+content);
            }
            Assert.AreEqual(grid.MasterTable.Rows.Count, 1);
        }



        /// <summary>
        /// Gets the test set data source XML.
        /// </summary>
        [Test]
        public void GetSetDataSourceXml()
        {
            Grid grid = new Grid();
            grid.ID = "test";

            // IMPORTANT when selecting nodes with child nodes:
            // WebGrid uses first selected node as template for loading column settings, and uses
            // these column settings for updating and inserting nodes into your XML file.
            grid.SetDataSource( Path + "\\WebGridMessages.xml", "//Language//*");

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.DataSourceType, DataSourceControlType.EnumerableDataSource);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
        }

        [Test]
        public void GetSqlConnection()
        {
            Grid grid = new Grid();
            grid.DataSourceId = "test";
            grid.ID = "test";
            grid.ConnectionString = ConnectionSqlConnection;
            grid["imgtest"].ColumnType = ColumnType.File;
            ((File) grid["imgtest"]).Directory = Path;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
        }


        [Test]
        public void GetSqlConnectionPrimaryKeys()
        {
            Grid grid = new Grid();
            grid.DataSourceId = "test";
            grid.ID = "test";
            grid.ConnectionString = ConnectionSqlConnection;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Columns.Primarykeys.Count, 0);
            Assert.AreEqual(grid.MasterTable.Columns.Primarykeys[0].Identity, true);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
        }


        [Test]
        public void ValidateDataSourceProperty()
        {

            string[] names = new[] { "Tom", "Dick", "Harry", "Thomas", "Anders", "Carl", "Trond", "Eli", "Eva" };

            Grid grid = new Grid();
            grid.ID = "test";
            grid.DataSource = names;

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.Rows.Count, names.Length);

            StringDictionary categories = new StringDictionary();

            categories.Add("Category 1", "1");
            categories.Add("Category 2", "2");
            categories.Add("Category 3", "3");
            categories.Add("Category 4", "4");
            categories.Add("Category 5", "5");
            categories.Add("Category 6", "6");
            categories.Add("Category 7", "7");

            grid = new Grid();
            grid.ID = "test";
            grid.DataSource = categories;
            sb = new StringBuilder();
            sw = new StringWriter(sb);
            gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.Rows.Count, categories.Count);






            NameValueCollection categoriesNVC = new NameValueCollection();

            categoriesNVC.Add("Category 1", "1");
            categoriesNVC.Add("Category 2", "2");
            categoriesNVC.Add("Category 3", "3");
            categoriesNVC.Add("Category 4", "4");
            categoriesNVC.Add("Category 5", "5");
            categoriesNVC.Add("Category 6", "6");
            categoriesNVC.Add("Category 7", "7");

            grid = new Grid();
            grid.ID = "test";
            grid.DataSource = categories;
            sb = new StringBuilder();
            sw = new StringWriter(sb);
            gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.Rows.Count, categoriesNVC.Count);
        }


        [Test]
        public void TestOleDataReader()
        {
            Grid grid = new Grid();
            grid.ID = "test";

            OleDbConnection cn = new OleDbConnection();

            cn.ConnectionString = ConnectionAccessOleDb;

            cn.Open();

            OleDbCommand cmd = new OleDbCommand();

            cmd.Connection = cn;

            cmd.CommandText = "SELECT * from Categories";

            cmd.CommandType = CommandType.Text;

            OleDbDataReader sqlDR = cmd.ExecuteReader();


            grid.DataSource = sqlDR;

                StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Rows.Count,1);
        }

        [Test]
        public void TestOleDbAccessConnection()
        {
            Query tmp = Query.ExecuteReader("SELECT * FROM Customers", ConnectionAccessOleDb);

            Assert.IsNotNull(tmp);
        }

        [Test]
        public void TestOleDbConnection()
        {
            Query tmp = Query.ExecuteReader("SELECT * FROM test", ConnectionSqlOleDb);
            Assert.IsNotNull(tmp);
        }

        /// <summary>
        /// Tests the SQL connection.
        /// </summary>
        [Test]
        public void TestSqlConnection()
        {
            Query tmp = Query.ExecuteReader("SELECT * FROM test", ConnectionSqlConnection);
            Assert.IsNotNull(tmp);
        }
    }
}