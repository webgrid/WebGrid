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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web.UI;

    using NUnit.Framework;

    using WebGrid;

    [TestFixture]
    public class DataSourceGeneric : UnitTestCommon
    {
        #region Fields

        private int BeforeGetDataExecuted;

        #endregion Fields

        #region Methods

        [Test]
        public void DataSetTest()
        {
            DataSet dataSet;

            #region DataSet setup
            //Create a SqlConnection to the Northwind database.
            using (SqlConnection connection = new SqlConnection(ConnectionSqlConnection.Replace("Test","Northwind")))
            {
                //Create a SqlDataAdapter for the Suppliers table.
                SqlDataAdapter adapter = new SqlDataAdapter();

                // A table mapping names the DataTable.
                adapter.TableMappings.Add("Table", "Suppliers");

                // Open the connection.
                connection.Open();

                // Create a SqlCommand to retrieve Suppliers data.
                SqlCommand command = new SqlCommand("SELECT SupplierID, CompanyName FROM dbo.Suppliers;",connection);
                command.CommandType = CommandType.Text;

                // Set the SqlDataAdapter's SelectCommand.
                adapter.SelectCommand = command;

                // Fill the DataSet.
                dataSet = new DataSet("Suppliers");
                adapter.Fill(dataSet);

                // Create a second Adapter and Command to get
                // the Products table, a child table of Suppliers.
                SqlDataAdapter productsAdapter = new SqlDataAdapter();
                productsAdapter.TableMappings.Add("Table", "Products");

                SqlCommand productsCommand = new SqlCommand("SELECT ProductID, SupplierID FROM dbo.Products;",connection);

                productsAdapter.SelectCommand = productsCommand;

                // Fill the DataSet.
                productsAdapter.Fill(dataSet);

                // Close the connection.
                connection.Close();
               // Create a DataRelation to link the two tables
                // based on the SupplierID.
                DataColumn parentColumn = dataSet.Tables["Suppliers"].Columns["SupplierID"];
                DataColumn childColumn = dataSet.Tables["Products"].Columns["SupplierID"];
                DataRelation relation = new DataRelation("SuppliersProducts",parentColumn, childColumn);
                dataSet.Relations.Add(relation);

            }
            #endregion

            Grid grid = new Grid();
            grid.DataSource = dataSet;
            grid.ID = "test";
             StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Rows.Count,1);
            int count = 0;

            for (int i = 0; i < grid.Columns.Count; i++)
            {
                if (grid.Columns[i].ColumnType != WebGrid.Enums.ColumnType.Foreignkey) continue;
                count++;
                Assert.Greater(((Foreignkey) grid.Columns[i]).Table.Rows.Count, 1);
            }
            Assert.AreEqual(1, count);
        }

        [Test]
        public void LoadGridWithEmployeesGenericCollection()
        {
            NorthwindEmployeeData employeedata = new NorthwindEmployeeData();

            Collection<NorthwindEmployee> employees = employeedata.GetAllEmployeesCollection("", 0, 100);
            Grid grid = new Grid();
            grid.DataSource = employees;
            grid.ID = "test";
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.Rows.Count, employees.Count);
        }

        [Test]
        public void LoadGridWithEmployeesGenericDictionary()
        {
            NorthwindEmployeeData employeedata = new NorthwindEmployeeData();

            Dictionary<int, NorthwindEmployee> employees = employeedata.GetAllEmployeesDictionary("", 0, 100);

            Grid grid = new Grid();
            grid.DataSource = employees;
            grid.ID = "test";
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.Rows.Count, employees.Count);
        }

        [Test]
        public void LoadGridWithEmployeesGenericList()
        {
            NorthwindEmployeeData employeedata = new NorthwindEmployeeData();

            List<NorthwindEmployee> employees = employeedata.GetAllEmployeesList("", 0, 100);

            Grid grid = new Grid();
            grid.DataSource = employees;
            grid.DataBind();
            grid.ID = "test";
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.Rows.Count, employees.Count);
        }

        [Test]
        public void LoadGridWithNames()
        {
            List<string> names = new List<string>();
            names.Add("Tom");
            names.Add("Dick");
            names.Add("Harry");
            names.Add("Thomas");
            names.Add("Anders");
            names.Add("Carl");
            names.Add("Trond");
            names.Add("Eli");
            names.Add("Eva");

            Grid grid = new Grid();
            grid.DataSource = names;
            grid.ID = "test";
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.Rows.Count, names.Count);
        }

        [Test]
        public void LoadGridWithNamesDictionary()
        {
            Dictionary<string, string> names = new Dictionary<string, string>();
            names.Add("Tom", "Tom");
            names.Add("Dick", "Dick");
            names.Add("Harry", "Harry");
            names.Add("Thomas", "Thomas");
            names.Add("Anders", "Anders");
            names.Add("Carl", "Carl");
            names.Add("Trond", "Trond");
            names.Add("Eli", "Eli");
            names.Add("Eva", "Eva");

            Grid grid = new Grid();
            grid.DataSource = names;
            grid.ID = "test";
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.Rows.Count, names.Count);
        }

        [Test]
        public void LoadGridWithNamesDictionaryUpdate()
        {
            Dictionary<string, string> names = new Dictionary<string, string>();
            names.Add("Tom", "Tom");
            names.Add("Dick", "Dick");
            names.Add("Harry", "Harry");
            names.Add("Thomas", "Thomas");
            names.Add("Anders", "Anders");
            names.Add("Carl", "Carl");
            names.Add("Trond", "Trond");
            names.Add("Eli", "Eli");
            names.Add("Eva", "Eva");

            Grid grid = new Grid();
            grid.DataSource = names;
            grid.ID = "test";
            grid.DisplayView = WebGrid.Enums.DisplayView.Detail;
            grid.Columns["Key"].Primarykey = true;
            grid.EditIndex = "Trond";
            grid.BeforeUpdateInsert += grid_BeforeUpdateInsert;
            grid.AfterUpdateInsert += grid_AfterUpdateInsert;
            grid.Rows[0]["Value"].Value = "mytest";
            grid.RaisePostBackEvent("RecordUpdateClick!!False");
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.Rows["Trond"]["Value"].Value, "mytest");
            Assert.AreEqual(grid.MasterTable.Rows.Count, names.Count);
        }

        [Test]
        public void LoadGridWithNamesSortedList()
        {
            SortedList<string, string> names = new SortedList<string, string>();
            names.Add("Tom", "Tom");
            names.Add("Dick", "Dick");
            names.Add("Harry", "Harry");
            names.Add("Thomas", "Thomas");
            names.Add("Anders", "Anders");
            names.Add("Carl", "Carl");
            names.Add("Trond", "Trond");
            names.Add("Eli", "Eli");
            names.Add("Eva", "Eva");

            Grid grid = new Grid();
            grid.DataSource = names;
            grid.ID = "test";
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.Rows.Count, names.Count);
        }

        [Test]
        public void LoadGridWithNeedDataDetailView()
        {
            Grid grid = new Grid();
            grid.DataBind();
            grid.ID = "test";
            grid.DisplayView = WebGrid.Enums.DisplayView.Detail;
            BeforeGetDataExecuted = 0;
            grid.BeforeGetData += grid_BeforeGetData;
            grid.Columns["EmployeeID"].ColumnType = WebGrid.Enums.ColumnType.Number;
            grid.Columns["EmployeeID"].Primarykey = true;
            grid.Columns["FirstName"].ColumnType = WebGrid.Enums.ColumnType.Text;
            grid.Columns["LastName"].ColumnType = WebGrid.Enums.ColumnType.Text;
            grid.Columns["Address"].ColumnType = WebGrid.Enums.ColumnType.Text;
            grid.Columns["BirthDay"].ColumnType = WebGrid.Enums.ColumnType.DateTime;
            grid.Columns["CustomerType"].ColumnType = WebGrid.Enums.ColumnType.Foreignkey;
            grid.Columns["CustomerType"].AllowEditInGrid = true;
            ((WebGrid.Foreignkey)grid.Columns["CustomerType"]).ValueColumn = "title";
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.Rows.Count, 1);
            Assert.AreEqual(((Foreignkey)grid.Columns["CustomerType"]).Table.Rows.Count, 3+1);//???
            Assert.AreEqual(grid.SystemMessage.Count, 0);
            Assert.AreEqual(BeforeGetDataExecuted, 1);
        }

        [Test]
        public void LoadGridWithNeedDataGridView()
        {
            NorthwindEmployeeData employeedata = new NorthwindEmployeeData();

            List<NorthwindEmployee> employees = employeedata.GetAllEmployeesList("", 0, 100);
            Grid grid = new Grid();
            grid.DataBind();
            grid.ID = "test";
            BeforeGetDataExecuted = 0;
            grid.BeforeGetData += grid_BeforeGetData;
            grid.Columns["EmployeeID"].ColumnType = WebGrid.Enums.ColumnType.Number;
            grid.Columns["EmployeeID"].Primarykey = true;
            grid.Columns["FirstName"].ColumnType = WebGrid.Enums.ColumnType.Text;
            grid.Columns["LastName"].ColumnType = WebGrid.Enums.ColumnType.Text;
            grid.Columns["Address"].ColumnType = WebGrid.Enums.ColumnType.Text;
            grid.Columns["BirthDay"].ColumnType = WebGrid.Enums.ColumnType.DateTime;
            grid.Columns["CustomerType"].ColumnType = WebGrid.Enums.ColumnType.Foreignkey;
            grid.Columns["CustomerType"].AllowEditInGrid = true;
            grid.Columns["CustomerType2"].ColumnType = WebGrid.Enums.ColumnType.Foreignkey;
            grid.Columns["CustomerType2"].AllowEditInGrid = true;
            ((WebGrid.Foreignkey)grid.Columns["CustomerType"]).ValueColumn = "title";
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(BeforeGetDataExecuted, 1);
            Assert.AreEqual(grid.MasterTable.Rows.Count, employees.Count);
            Assert.AreEqual( ((WebGrid.Foreignkey)grid.Columns["CustomerType"]).Table.Rows.Count, 3);
            Assert.AreEqual(((WebGrid.Foreignkey)grid.Columns["CustomerType2"]).Table.Rows.Count, 9);
            Assert.AreEqual(grid.SystemMessage.Count, 0);
        }

        [Test]
        public void LoadGridWithNumbers()
        {
            List<int> numbers = new List<int>();
            numbers.Add(45);
            numbers.Add(45);
            numbers.Add(34);
            numbers.Add(34);
            numbers.Add(34);
            numbers.Add(23);
            numbers.Add(45);
            numbers.Add(45);
            numbers.Add(23);

            Grid grid = new Grid();
            grid.DataSource = numbers;
            grid.ID = "test";
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.Rows.Count, numbers.Count);
        }

        [Test]
        public void WebGridLinqTest()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            Grid grid = new Grid();
            grid.DataSource = from n in numbers where n < 5 select n;
            grid.ID = "test";
            int rows = grid.Rows.Count; // Return 5 rows.
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
            Assert.AreEqual(rows, 5);
        }

        [Test]
        public void WebGridLinqTest2()
        {
            NorthwindEmployeeData employeedata = new NorthwindEmployeeData();

            Collection<NorthwindEmployee> employees = employeedata.GetAllEmployeesCollection("", 0, 100);

            var list = from test in employees
                       where test.Region != "WA"
                       select new { test.EmployeeID, test.FirstName };

            Grid grid = new Grid();
            grid.DataSource = list;
            grid.ID = "test";
            int rows = grid.Rows.Count; // Return 5 rows.
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
            Assert.AreEqual(rows, 4);
            foreach (WebGrid.Data.Row row in grid.Rows)
            {
                Assert.IsNotNull(row["EmployeeID"].Value);
                Assert.IsNotNull(row["FirstName"].Value);
                Assert.AreEqual(row["FirstName"].Value.GetType(), typeof(string));
                // WebGrid is using int64
                Assert.AreEqual(row["EmployeeID"].Value.GetType(), typeof(Int64));
            }
        }

        void grid_AfterUpdateInsert(object sender, WebGrid.Events.AfterUpdateInsertEventArgs e)
        {
            Assert.IsNotNull(e.DataRow);
            Assert.AreEqual(e.DataRow["Value"], "mytest");
        }

        void grid_BeforeGetData(object sender, WebGrid.Events.BeforeGetDataEventArgs e)
        {
            if (e.Table == ((Grid)sender).MasterTable)
            {
                BeforeGetDataExecuted++;
                NorthwindEmployeeData employeedata = new NorthwindEmployeeData();

                List<NorthwindEmployee> employees = employeedata.GetAllEmployeesList("", 0, 100);
                e.Table.DataSource = employees;
            }
            else if (e.Table == ((WebGrid.Foreignkey)((Grid)sender).Columns["CustomerType"]).Table)
            {
                e.Table.Columns["CustomerType"].ColumnType = WebGrid.Enums.ColumnType.Number;
                e.Table.Columns["Title"].ColumnType = WebGrid.Enums.ColumnType.Text;

                WebGrid.Data.Row row = new WebGrid.Data.Row(e.Table);

                row["CustomerType"].Value = 1;
                row["Title"].Value = "Title";
                e.Table.Rows.Add(row);

                row = new WebGrid.Data.Row(e.Table);
                row["CustomerType"].Value = 2;
                row["Title"].Value = "Title";
                e.Table.Rows.Add(row);

                row = new WebGrid.Data.Row(e.Table);
                row["CustomerType"].Value = 3;
                row["Title"].Value = "Title";
                e.Table.Rows.Add(row);
            }
            else if (e.Table == ((WebGrid.Foreignkey)((Grid)sender).Columns["CustomerType2"]).Table)
            {
                NorthwindEmployeeData employeedata = new NorthwindEmployeeData();

                List<NorthwindEmployee> employees = employeedata.GetAllEmployeesList("", 0, 100);
                e.Table.DataSource = employees;
            }
        }

        void grid_BeforeUpdateInsert(object sender, WebGrid.Events.BeforeUpdateInsertEventArgs e)
        {
            Assert.IsNotNull(e.Row["Value"].Value);
            Assert.AreEqual(e.Row["Value"].Value, "mytest");
        }

        #endregion Methods
    }
}