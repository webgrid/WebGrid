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
using System.Web.UI.WebControls;
using NUnit.Framework;
using WebGrid;
using WebGrid.Enums;
using WebGrid.Util;
using WebGridUnitTest;
using DateTime=System.DateTime;

namespace WebGridUnitTest
{
    [TestFixture]
    public class PostBackClick : UnitTestCommon
    {
        private Grid CreateNewCategory(Grid grid)
        {
            grid.DataSourceId = "Categories";
            grid.ID = "test";
            grid.ConnectionString = ConnectionAccessOleDb;
            grid.DefaultVisibility = Visibility.Both;
            grid.RecordsPerRow = 2;
            grid.PagerType = PagerType.None;
            grid.Width = Unit.Pixel(1000);

            grid.RaisePostBackEvent("NewRecordClick!");
            grid.MasterTable.Rows[0]["CategoryName"].Value = "Category name";
            grid.MasterTable.Rows[0]["Description"].Value = string.Format("Unique ID:{0}", DateTime.Now.Ticks);
            return grid;
        }

        private Grid CreateNewCategorySelect(Grid grid)
        {
            grid.Sql = "SELECT * FROM Categories";
            grid.ID = "test";
            grid.ConnectionString = ConnectionAccessOleDb;
            grid.DefaultVisibility = Visibility.Both;
            grid.RecordsPerRow = 2;
            grid.PagerType = PagerType.None;
            grid.Width = Unit.Pixel(1000);

            grid.RaisePostBackEvent("NewRecordClick!");
            grid.MasterTable.Rows[0]["CategoryName"].Value = "Category name";
            grid.MasterTable.Rows[0]["Description"].Value = "test test";
            return grid;
        }

        [Test]
        public void CreateAndDeleteRecord()
        {
            CreateRecord();

            string CategoryId =
                Query.ExecuteScalar("SELECT TOP 1 CategoryID FROM Categories ORDER BY CategoryID DESC",
                                    ConnectionAccessOleDb).ToString();


            Grid grid = new Grid();
            grid.DataSourceId = "Categories";
            grid.ID = "test";
            grid.ConnectionString = ConnectionAccessOleDb;
            grid.DefaultVisibility = Visibility.Both;
            grid.RecordsPerRow = 2;
            grid.PagerType = PagerType.None;
            grid.Width = Unit.Pixel(1000);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            int records = grid.MasterTable.Rows.Count;

            grid = new Grid();
            grid.DataSourceId = "Categories";
            grid.ID = "test";
            grid.ConnectionString = ConnectionAccessOleDb;
            grid.DefaultVisibility = Visibility.Both;
            grid.RecordsPerRow = 2;
            grid.PagerType = PagerType.None;
            grid.Width = Unit.Pixel(1000);
            grid.RaisePostBackEvent("RecordDeleteClick!" + CategoryId);

            sb = new StringBuilder();
            sw = new StringWriter(sb);
            gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);

            Assert.Less(grid.MasterTable.Rows.Count, records);
            Assert.AreEqual(grid.Mode, Mode.Grid);
        }

        [Test]
        public void CreateAndDeleteRecordSelectSql()
        {
            CreateRecord();

            string CategoryId =
                Query.ExecuteScalar("SELECT TOP 1 CategoryID FROM Categories ORDER BY CategoryID DESC",
                                    ConnectionAccessOleDb).ToString();


            Grid grid = new Grid();
            grid.Sql = "SELECT * FROM Categories";
            grid.ID = "test";
            grid.ConnectionString = ConnectionAccessOleDb;
            grid.DefaultVisibility = Visibility.Both;
            grid.RecordsPerRow = 2;

            grid.PagerType = PagerType.None;
            grid.Width = Unit.Pixel(1000);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            int records = grid.MasterTable.Rows.Count;

            grid = new Grid();
            grid.Sql = "SELECT * FROM Categories";
            grid.ID = "test345";
            grid.ConnectionString = ConnectionAccessOleDb;
            grid.DefaultVisibility = Visibility.Both;
            grid.RecordsPerRow = 2;
            grid.PagerType = PagerType.None;
            grid.Width = Unit.Pixel(1000);
            grid.RaisePostBackEvent("RecordDeleteClick!" + CategoryId);
            Assert.AreEqual(grid.MasterTable.DataSourceType, DataSourceControlType.InternalDataSource);
              sb = new StringBuilder();
            sw = new StringWriter(sb);
            gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);

            Assert.Less(grid.MasterTable.Rows.Count, records);
            Assert.AreEqual(grid.Mode, Mode.Grid);
        }

        [Test]
        public void CreateRecord()
        {
            Grid grid = new Grid();
            grid = CreateNewCategory(grid);

            int records = grid.MasterTable.Rows.Count;

            Assert.AreEqual(grid.Mode, Mode.Edit);
            grid.RaisePostBackEvent("RecordUpdateClick!!False");
            Assert.AreEqual(grid.SystemMessage.Count, 0);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Rows.Count, records);
            Assert.AreEqual(grid.Mode, Mode.Grid);
        }

        [Test]
        public void CreateRecordFieldRequired()
        {
            Grid grid = new Grid();
            grid["CategoryName"].Required = true;
            grid = CreateNewCategory(grid);
            grid["CategoryName"].Required = true;
            grid.MasterTable.Rows[0]["CategoryName"].Value = "";
            int records = grid.MasterTable.Rows.Count;

            Assert.AreEqual(grid.Mode, Mode.Edit);
            grid.RaisePostBackEvent("RecordUpdateClick!!False");
            Assert.AreEqual(grid.SystemMessage.Count, 1);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.Rows.Count, records);
            Assert.AreEqual(grid.Mode, Mode.Edit);
        }

        [Test]
        public void CreateRecordSelect()
        {
            Grid grid = new Grid();
            grid = CreateNewCategorySelect(grid);

            int records = grid.MasterTable.Rows.Count;

            Assert.AreEqual(grid.Mode, Mode.Edit);
            grid.RaisePostBackEvent("RecordUpdateClick!!False");
            Assert.AreEqual(grid.SystemMessage.Count, 0);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Rows.Count, records);
            Assert.AreEqual(grid.Mode, Mode.Grid);
        }

        [Test]
        public void CreateRecordSelectFieldRequired()
        {
            Grid grid = new Grid();
            grid["CategoryName"].Required = true;
            grid = CreateNewCategorySelect(grid);
            grid["CategoryName"].Required = true;
            grid.MasterTable.Rows[0]["CategoryName"].Value = "";
            int records = grid.MasterTable.Rows.Count;

            Assert.AreEqual(grid.Mode, Mode.Edit);
            grid.RaisePostBackEvent("RecordUpdateClick!!False");
            Assert.AreEqual(grid.SystemMessage.Count, 1);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.Rows.Count, records);
            Assert.AreEqual(grid.Mode, Mode.Edit);
        }

        [Test]
        public void NewRecord()
        {
            Grid grid = new Grid();
            grid.DataSourceId = "Employees";
            grid.ID = "test";
            grid.ConnectionString = ConnectionAccessOleDb;
            grid.DefaultVisibility = Visibility.Both;
            grid.RecordsPerRow = 2;
            grid.PagerType = PagerType.None;
            grid.Width = Unit.Pixel(1000);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
            Assert.AreEqual(grid.Mode, Mode.Grid);
            grid.RaisePostBackEvent("NewRecordClick!");
            Assert.AreEqual(grid.MasterTable.Rows.Count, 1);
            Assert.AreEqual(grid.Mode, Mode.Edit);
        }

        [Test]
        public void NewRecordAndCancelRecord()
        {
            Grid grid = new Grid();
            grid.DataSourceId = "Employees";
            grid.ID = "test";
            grid.ConnectionString = ConnectionAccessOleDb;
            grid.DefaultVisibility = Visibility.Both;
            grid.RecordsPerRow = 2;
            grid.PagerType = PagerType.None;
            grid.Width = Unit.Pixel(1000);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
            Assert.AreEqual(grid.Mode, Mode.Grid);
            grid.RaisePostBackEvent("NewRecordClick!");
            Assert.AreEqual(grid.Mode, Mode.Edit);
            Assert.AreEqual(grid.MasterTable.Rows.Count, 1);
            grid.RaisePostBackEvent("RecordCancelClick!");
            Assert.AreEqual(grid.Mode, Mode.Grid);
            grid.MasterTable.GetData(false);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
        }

        [Test]
        public void NewRecordMaximumRecordsLimit()
        {
            Grid grid = new Grid();
            grid.DataSourceId = "Employees";
            grid.ID = "test";
            grid.ConnectionString = ConnectionAccessOleDb;
            grid.DefaultVisibility = Visibility.Both;
            grid.RecordsPerRow = 2;
            grid.MaximumRecords = 2;
            grid.PagerType = PagerType.None;
            grid.Width = Unit.Pixel(1000);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(false, grid.AllowNew);
        }

        [Test]
        public void OrderBy()
        {
            Grid grid = new Grid();
            grid.DataSourceId = "Categories";
            grid.ID = "test";
            grid.ConnectionString = ConnectionAccessOleDb;
            grid.DefaultVisibility = Visibility.Both;
            grid.RecordsPerRow = 2;
            grid.PagerType = PagerType.None;
            grid.Width = Unit.Pixel(1000);
            grid.RaisePostBackEvent("ColumnHeaderClick![Categories].[CategoryName]");
            grid.RaisePostBackEvent("ColumnHeaderClick![Categories].[Description]");
            grid.RaisePostBackEvent("ColumnHeaderClick![Categories].[CategoryName]");
            grid.RaisePostBackEvent("ColumnHeaderClick![Categories].[CategoryName]");
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
            Assert.AreEqual(grid.Mode, Mode.Grid);
            Assert.IsNotEmpty(grid.SortExpression);
        }


        [Test]
        public void UpdateRecord()
        {
            Grid grid = new Grid();
            grid = CreateNewCategory(grid);
            grid.CurrentId = "6";
            int records = grid.MasterTable.Rows.Count;

            Assert.AreEqual(grid.Mode, Mode.Edit);
            grid.RaisePostBackEvent("RecordUpdateClick!!False");
            Assert.AreEqual(grid.SystemMessage.Count, 0);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);

            Assert.Greater(grid.MasterTable.Rows.Count, records);
            Assert.AreEqual(grid.Mode, Mode.Grid);
        }

        [Test]
        public void UpdateRecordSelect()
        {
            Grid grid = new Grid();
            grid = CreateNewCategorySelect(grid);
            grid.CurrentId = "6";
            int records = grid.MasterTable.Rows.Count;

            Assert.AreEqual(grid.Mode, Mode.Edit);
            grid.RaisePostBackEvent("RecordUpdateClick!!False");
            Assert.AreEqual(grid.SystemMessage.Count, 0);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);

            Assert.Greater(grid.MasterTable.Rows.Count, records);
            Assert.AreEqual(grid.Mode, Mode.Grid);
        }
    }
}