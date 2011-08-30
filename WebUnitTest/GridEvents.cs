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
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using NUnit.Framework;
using WebGrid;
using WebGrid.Enums;
using WebGrid.Events;
using WebGrid.Util;
using DateTime=System.DateTime;

namespace WebGridUnitTest
{
    [TestFixture]
    public class GridEvents : UnitTestCommon
    {
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
            grid.MasterTable.Rows[0]["Description"].Value = "Unique ID:" + DateTime.Now.Ticks;
            return grid;
        }


        private bool executedbeforeevent;

        private void CreateRecordBeforeAfterEvent_AfterUpdateInsert(object sender, AfterUpdateInsertEventArgs e)
        {
            executedbeforeevent = true;
        }

        private bool executeddisallowed;

        private void CreateRecordBeforeDisallowedEvent_BeforeUpdateInsert(object sender,
                                                                          BeforeUpdateInsertEventArgs e)
        {
            executeddisallowed = true;
            e.AcceptChanges = false;
        }


        private bool executedafterevent;

        private void CreateRecordBeforeAfterEvent_BeforeUpdateInsert(object sender,
                                                                     BeforeUpdateInsertEventArgs e)
        {
            e.Row["CategoryName"].Value = "myCategoryName1";
            executedafterevent = true;
        }

        private bool executedafterdeleteevents;

        private void grid_AfterDeleteEvents(object sender, AfterDeleteEventArgs e)
        {
            executedafterdeleteevents = true;
        }

        private bool executedbeforedeleteevents;

        private void grid_BeforeDeleteEvents(object sender, BeforeDeleteEventArgs e)
        {
            executedbeforedeleteevents = true;
        }

        private bool executedbeforedeleteeventsdisallowed;

        private void grid_BeforeDeleteDisallowed(object sender, BeforeDeleteEventArgs e)
        {
            executedbeforedeleteeventsdisallowed = true;
            e.AcceptChanges = false;
        }

        private int rows;

        private void grid_GridRowEvent(object sender, GridRowBoundEventArgs e)
        {
            Assert.IsNotEmpty(e.Row.PrimaryKeyValues);
            Assert.AreEqual(e.Row.PrimaryKeyValues.IndexOf(','),-1);
            rows++;
        }

        private bool gridrowClickedDisallowed;

        private void grid_RowClickDisallowed(object sender, RowClickEventArgs e)
        {
            gridrowClickedDisallowed = true;
            e.AcceptChanges = false;
        }

        private bool gridrowClicked;

        private void grid_RowClickEvents(object sender, RowClickEventArgs e)
        {
            gridrowClicked = true;
        }

        private bool executedgridcolumnpostbackevent;

        private void grid_ColumnPostBackEvent(object sender, ColumnPostBackEventArgs e)
        {
            if (string.IsNullOrEmpty(e.ColumnName) == false)
                executedgridcolumnpostbackevent = true;
            if (!string.IsNullOrEmpty(e.CurrentId))
            {
                Assert.IsNotNull(e.Row);
                Assert.AreEqual(e.Row.PrimaryKeyValues, e.CurrentId);
            }
            else
                Assert.Fail("CurrentId is null for column post back");
        }

        private bool gridpostbackevent;

        private void grid_GridPostBackEvent(object sender, GridPostBackEventArgs e)
        {
            if (e.GetEventArguments() != null && e.GetEventArguments().Length > 1 && string.IsNullOrEmpty(e.EventName) == false)
                gridpostbackevent = true;
        }

        

        private bool executedgridlifeprerender;

        private void grid_PreRenderEvents(object sender, EventArgs e)
        {
            executedgridlifeprerender = true;
        }

        private bool executedgridlifeloadevents;

        private void grid_LoadEvents(object sender, EventArgs e)
        {
            executedgridlifeloadevents = true;
        }

        private bool executedgridlifecompleteevents;

        private void grid_CompleteEvents(object sender)
        {
            executedgridlifecompleteevents = true;
        }

        private bool gridlifeafterrowsevents;

        private string grid_AfterRowsEvents(object sender)
        {
            gridlifeafterrowsevents = true;
            return null;
        }

        private bool gridlifebeforeevents;

        private string grid_BeforeRowsEvents(object sender)
        {
            gridlifebeforeevents = true;
            return null;
        }

        private static void grid_RowClickEventsAccepted(object sender, RowClickEventArgs e)
        {
            e.AcceptChanges = false;
        }

        private static void grid_RowClickEventsCancel(object sender, RowClickEventArgs e)
        {
            e.AcceptChanges = true;
        }

        private static void grid_EditRowSuccess(object sender, EditRowEventArgs e)
        {
            e.AcceptChanges = true;
        }

        private static void grid_EditRowCodeCancel(object sender, EditRowEventArgs e)
        {
            e.AcceptChanges = false;
        }

        private static void grid_EditRowPostBackCancel(object sender, EditRowEventArgs e)
        {
            e.AcceptChanges = false;
        }

        private bool searcheventrunnned;

        private void grid_SearchChangedEvents(object sender, SearchChangedEventArgs e)
        {
            if (e.SearchString.Equals(e.OldSearchString))
                throw new ApplicationException("Grid 'SearchChanged' event did not trigger correctly.");
            if (string.IsNullOrEmpty(e.SearchString))
                throw new ApplicationException("Grid 'SearchChanged' and property 'SearchString' is EmptyOrNull");
            searcheventrunnned = true;
        }

        private bool gridrecordupdateclickdisallow;

        private void grid_RecordUpdateClickDisallow(object sender, UpdateClickEventArgs e)
        {
            e.AcceptChanges = false;
            gridrecordupdateclickdisallow = true;
        }

        private bool recordcancelclickrunned;

        private void grid_RecordCancelClickEvents(object sender, CancelClickEventArgs e)
        {
            recordcancelclickrunned = true;
            e.AcceptChanges = false;
        }

        private bool newrecordclickdisallow;

        private void grid_NewRecordClickNewRecordDisallow(object sender, NewRecordClickEventArgs e)
        {
            newrecordclickdisallow = true;
            e.AcceptChanges = false;
        }

        private bool headerclickrunned;

        private void grid_HeaderClickEvents(object sender, HeaderClickEventArgs e)
        {
            headerclickrunned = true;
            e.AcceptChanges = false;
        }

        private bool pagerclickrunned;

        private void grid_PagerClickEvents(object sender, PagerClickEventArgs e)
        {
            pagerclickrunned = true;
            if (e.NewCurrentPage.Equals(e.OldCurrentPage))
                throw new ApplicationException("PagerClick 'event' error: NewCurrentPage == OldCurrentPage");
            e.AcceptChanges = false;
        }

        private int updaterowseventscount;

        private void tmp_BeforeUpdateInsertUpdateRow(object sender, BeforeUpdateInsertEventArgs e)
        {
            e.Row["Description"].Value = string.Format("UpdateEvent{0}", DateTime.Now.Ticks);
            updaterowseventscount++;
        }

        private bool updaterowsclickevents;

        private void tmp_UpdateRowsClickEvents(object sender, UpdateRowsClickEventArgs e)
        {
            updaterowsclickevents = true;
        }

        private bool exportclickeventrunned;

        private void grid_ExportClickEvents(object sender, ExcelExportClickEventArgs e)
        {
            if (string.IsNullOrEmpty(e.FileName))
                throw new ApplicationException("ExportClick event (default) file name is Null or Empty");
            exportclickeventrunned = true;
        }

        [Test]
        public void CreateAndDeleteRecordBeforeAfterDeleteSucccess()
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
            grid.BeforeDelete += grid_BeforeDeleteEvents;
            grid.AfterDelete += grid_AfterDeleteEvents;
            grid.RaisePostBackEvent("RecordDeleteClick!" + CategoryId);

            sb = new StringBuilder();
            sw = new StringWriter(sb);
            gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);

            Assert.Less(grid.MasterTable.Rows.Count, records);
            Assert.AreEqual(grid.Mode, Mode.Grid);
            Assert.IsTrue(executedafterdeleteevents);
            Assert.IsTrue(executedbeforedeleteevents);
        }

        [Test]
        public void CreateAndDeleteRecordBeforeDeleteDisallowed()
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
            grid.BeforeDelete += grid_BeforeDeleteDisallowed;
            grid.RaisePostBackEvent("RecordDeleteClick!" + CategoryId);

            sb = new StringBuilder();
            sw = new StringWriter(sb);
            gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);

            Assert.AreEqual(grid.MasterTable.Rows.Count, records);
            Assert.AreEqual(grid.Mode, Mode.Grid);
            Assert.IsTrue(executedbeforedeleteeventsdisallowed);
        }

        [Test]
        public void CreateRecordBeforeAfterEventSuccess()
        {
            Grid grid = new Grid();
            grid = CreateNewCategory(grid);
            beforerowsrendered = 0;
            int records = grid.MasterTable.Rows.Count;
            executedafterevent = false;
            executedbeforeevent = false;
            executedbeforevalidate = false;
            Assert.AreEqual(grid.Mode, Mode.Edit);
            grid.BeforeUpdateInsert += CreateRecordBeforeAfterEvent_BeforeUpdateInsert;
            grid.BeforeValidate += grid_BeforeValidate;
            grid.BeforeRowRender += grid_BeforeRowRender;
            grid.AfterUpdateInsert += CreateRecordBeforeAfterEvent_AfterUpdateInsert;
            grid.RaisePostBackEvent("RecordUpdateClick!!False");
            Assert.AreEqual(grid.Rows[0]["Description"].Value, "mydescription1");
            Assert.AreEqual(grid.Rows[0]["CategoryName"].Value, "myCategoryName1");
          
            Assert.AreEqual(grid.SystemMessage.Count, 0);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Rows.Count, records);
            Assert.AreEqual(grid.MasterTable.Rows.Count, beforerowsrendered);
            Assert.AreEqual(grid.Mode, Mode.Grid);
            Assert.IsTrue(executedafterevent);
            Assert.IsTrue(executedbeforeevent);
            Assert.IsTrue(executedbeforevalidate);
        }

        private int beforerowsrendered;
        void grid_BeforeRowRender(object sender, BeforeRenderRowArgs e)
        {
            beforerowsrendered++;
        }

        bool executedbeforevalidate;
        void grid_BeforeValidate(object sender, BeforeValidateEventArgs e)
        {
            executedbeforevalidate = true;
            
            e.Row["Description"].Value = "mydescription1";
        }

        [Test]
        public void CreateRecordBeforeDisallowedEvent()
        {
            Grid grid = new Grid();
            grid = CreateNewCategory(grid);

            int records = grid.MasterTable.Rows.Count;

            Assert.AreEqual(grid.Mode, Mode.Edit);
            grid.BeforeUpdateInsert += CreateRecordBeforeDisallowedEvent_BeforeUpdateInsert;
            grid.RaisePostBackEvent("RecordUpdateClick!!False");
            Assert.AreEqual(grid.SystemMessage.Count, 0);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.Mode, Mode.Edit);
            Assert.IsTrue(executeddisallowed);
            Assert.AreEqual(grid.Rows.Count, records);
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


            mastergrid.RaisePostBackEvent("SlaveGridClick!test2");

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
            Assert.IsTrue(mastergrid.ActiveMenuSlaveGrid != null &&
                          mastergrid.ActiveMenuSlaveGrid.ID.Equals(slavegrid.ID));
        }

        [Test]
        public void GridCancelEventClickDisallow()
        {
            Grid grid = new Grid();

            CreateNewCategory(grid);
            grid.RecordCancelClick += grid_RecordCancelClickEvents;
            grid.RaisePostBackEvent("RecordCancelClick!");


            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);
            grid.Dispose();

            Assert.AreEqual(grid.MasterTable.Rows.Count, 1);
            Assert.AreEqual(grid.Mode, Mode.Edit);
            Assert.IsTrue(recordcancelclickrunned);
        }

        [Test]
        public void GridEditRowEvents()
        {
            Grid grid = new Grid();
            grid.EditRow += grid_EditRowPostBackCancel;

            grid.DataSourceId = "Categories";
            grid.ID = "test";
            grid.ConnectionString = ConnectionAccessOleDb;
            grid.DefaultVisibility = Visibility.Both;
            grid.RecordsPerRow = 2;
            grid.PagerType = PagerType.None;
            grid.Width = Unit.Pixel(1000);
            grid.RaisePostBackEvent("RecordClick!!6");

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);
            grid.Dispose();
            Assert.AreEqual(grid.Mode, Mode.Grid);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);


            grid = new Grid();

            CreateNewCategory(grid);
            grid.RaisePostBackEvent("RecordCancelClick!");

            grid.EditRow += grid_EditRowCodeCancel;
            grid.CurrentId = "6";
            grid.Mode = Mode.Edit;
            sb = new StringBuilder();
            sw = new StringWriter(sb);
            gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);
            grid.Dispose();
            Assert.AreEqual(grid.Mode, Mode.Grid);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);

            grid = new Grid();

            CreateNewCategory(grid);
            grid.RaisePostBackEvent("RecordCancelClick!");

            grid.EditRow += grid_EditRowSuccess;
            grid.RaisePostBackEvent("RecordClick!!6");
            sb = new StringBuilder();
            sw = new StringWriter(sb);
            gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);
            grid.Dispose();
            Assert.AreEqual(grid.Mode, Mode.Edit);
            Assert.AreEqual(grid.MasterTable.Rows.Count, 1);
        }

        [Test]
        public void GridExportClickEvents()
        {
            Grid grid = new Grid();

            CreateNewCategory(grid);
            grid.ExportClick += grid_ExportClickEvents;
            grid.RaisePostBackEvent("RecordCancelClick!");
            grid.RaisePostBackEvent("ExcelExportClick!");
            Assert.IsTrue(exportclickeventrunned);
        }

        [Test]
        public void GridHeaderClickEvent()
        {
            Grid grid = new Grid();
            grid.DataSourceId = "Categories";
            grid.ID = "test";
            grid.ConnectionString = ConnectionAccessOleDb;
            grid.DefaultVisibility = Visibility.Both;
            grid.RecordsPerRow = 2;
            grid.PagerType = PagerType.None;
            grid.Width = Unit.Pixel(1000);
            grid.HeaderClick += grid_HeaderClickEvents;
            grid.RaisePostBackEvent("ColumnHeaderClick![Categories].[CategoryName]");
            grid.RaisePostBackEvent("ColumnHeaderClick![Categories].[Description]");
            grid.RaisePostBackEvent("ColumnHeaderClick![Categories].[CategoryName]");
            grid.RaisePostBackEvent("ColumnHeaderClick![Categories].[CategoryName]");
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
            Assert.AreEqual(grid.Mode, Mode.Grid);
            Assert.IsTrue(headerclickrunned);
            Assert.IsEmpty(grid.SortExpression);
        }

        [Test]
        public void GridLifeCycleEvents()
        {
            Grid grid = new Grid();
            

            CreateNewCategory(grid);
            grid.RaisePostBackEvent("RecordCancelClick!");
            grid.BeforeRows += grid_BeforeRowsEvents;
            grid.AfterRows += grid_AfterRowsEvents;
            grid.Complete += grid_CompleteEvents;
            grid.Load += grid_LoadEvents;
            grid.PreRender += grid_PreRenderEvents;
            grid.RaisePostBackEvent("ElementPostBack!CategoryName");

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);
            grid.Dispose();
            Assert.AreEqual(grid.Mode, Mode.Grid);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);


            Assert.IsTrue(executedgridlifeprerender);
            Assert.IsTrue(executedgridlifeloadevents);


            Assert.IsTrue(executedgridlifecompleteevents);
            Assert.IsTrue(gridlifeafterrowsevents);
            Assert.IsTrue(gridlifeafterrowsevents);
            Assert.IsTrue(gridlifebeforeevents);
        }

        [Test]
        public void GridNewRecordEventClickDisallow()
        {
            Grid grid = new Grid();
            grid.NewRecordClick += grid_NewRecordClickNewRecordDisallow;
            CreateNewCategory(grid);


            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);
            grid.Dispose();

            Assert.Greater(grid.MasterTable.Rows.Count, 1);
            Assert.AreEqual(grid.Mode, Mode.Grid);
            Assert.IsTrue(newrecordclickdisallow);
        }

        [Test]
        public void GridPagerClickEventDisallow()
        {
            Grid grid = new Grid();
            grid.DataSourceId = "Categories";
            grid.ID = "test";
            grid.ConnectionString = ConnectionAccessOleDb;
            grid.DefaultVisibility = Visibility.Both;
            grid.RecordsPerRow = 2;
            grid.PagerType = PagerType.None;
            grid.Width = Unit.Pixel(1000);
            grid.PagerClick += grid_PagerClickEvents;
            grid.RaisePostBackEvent("PagerClick!ALL");
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
            Assert.AreEqual(grid.Mode, Mode.Grid);
            Assert.IsTrue(pagerclickrunned);
        }

        [Test]
        public void GridPostBackEvents()
        {
            Grid grid = new Grid();

            CreateNewCategory(grid);
            grid.RaisePostBackEvent("RecordCancelClick!");
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
            grid.GridPostBack += grid_GridPostBackEvent;
            grid.ColumnPostBack += grid_ColumnPostBackEvent;
            grid.RaisePostBackEvent("ElementPostBack!CategoryName!6");

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);

            Assert.AreEqual(grid.Mode, Mode.Grid);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);

            Assert.IsTrue(executedgridcolumnpostbackevent);
            Assert.IsTrue(gridpostbackevent);
        }

        [Test]
        public void GridRowClickDisallowed()
        {
            Grid grid = new Grid();

            CreateNewCategory(grid);
            grid.RaisePostBackEvent("RecordCancelClick!");
            grid.RowClick += grid_RowClickDisallowed;
            grid.RaisePostBackEvent("RecordClick!!6");
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);

            Assert.AreEqual(grid.Mode, Mode.Grid);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);

            Assert.IsTrue(gridrowClickedDisallowed);
        }

        [Test]
        public void GridRowLimitAddRow()
        {
            Grid grid = new Grid();
            rowsadded = 0;
            CreateNewCategory(grid);
            grid.RaisePostBackEvent("RecordCancelClick!");
            grid.GridRowBound += grid_GridRowBound;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);

            Assert.AreEqual(grid.Mode, Mode.Grid);
            Assert.AreEqual(grid.MasterTable.Rows.Count, 50);
            string content = sb.ToString();

            Assert.AreNotEqual(content.IndexOf("id=\"wgrh_test_19\"", StringComparison.InvariantCultureIgnoreCase), -1);
            Assert.AreNotEqual(content.IndexOf("id=\"wgrh_test_1\"", StringComparison.InvariantCultureIgnoreCase), -1);
            Assert.AreNotEqual(content.IndexOf("id=\"wgrh_test_25\"", StringComparison.InvariantCultureIgnoreCase), -1);
            Assert.AreEqual(content.IndexOf("id=\"wgrh_test_52\"", StringComparison.InvariantCultureIgnoreCase), -1);
        }

        private int rowsadded;
        void grid_GridRowBound(object sender, GridRowBoundEventArgs e)
        {
            rowsadded++;
            if (rowsadded > 50)
                e.AcceptChanges = false;
        }

        [Test]
        public void GridRowLimitRowsRendered()
        {
            Grid grid = new Grid();
            rowsrendered = 0;
            CreateNewCategory(grid);
            grid.RaisePostBackEvent("RecordCancelClick!");
            grid.BeforeRowRender += new BeforeRowRenderHandler(grid_LimitBeforeRowsRender);
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);
            string content = sb.ToString();

            Assert.AreNotEqual(content.IndexOf("id=\"wgrh_test_19\"", StringComparison.InvariantCultureIgnoreCase), -1);
            Assert.AreNotEqual(content.IndexOf("id=\"wgrh_test_1\"", StringComparison.InvariantCultureIgnoreCase), -1);
            Assert.AreEqual(content.IndexOf("id=\"wgrh_test_25\"", StringComparison.InvariantCultureIgnoreCase), -1);
            Assert.AreEqual(grid.Mode, Mode.Grid);
         }

        private int rowsrendered;
        void grid_LimitBeforeRowsRender(object sender, BeforeRenderRowArgs e)
        {
            rowsrendered++;
            if (rowsrendered > 10)
                e.RenderThisRow = false;
        }


        [Test]
        public void GridRowClickEvents()
        {
            Grid grid = new Grid();

            CreateNewCategory(grid);
            grid.RaisePostBackEvent("RecordCancelClick!");

            grid.RowClick += grid_RowClickEventsAccepted;
            grid.RaisePostBackEvent("RecordClick!!6");
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);
            grid.Dispose();
            Assert.AreEqual(grid.Mode, Mode.Grid);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);

            grid = new Grid();

            CreateNewCategory(grid);
            grid.RaisePostBackEvent("RecordCancelClick!");

            grid.RowClick += grid_RowClickEventsCancel;
            grid.RaisePostBackEvent("RecordClick!!6");
            sb = new StringBuilder();
            sw = new StringWriter(sb);
            gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);
            grid.Dispose();
            Assert.AreEqual(grid.Mode, Mode.Edit);
            Assert.AreEqual(grid.MasterTable.Rows.Count, 1);
        }

        [Test]
        public void GridRowClickSuccess()
        {
            Grid grid = new Grid();

            CreateNewCategory(grid);
            grid.RaisePostBackEvent("RecordCancelClick!");
            grid.RowClick += grid_RowClickEvents;
            grid.RaisePostBackEvent("RecordClick!!6");
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);

            Assert.AreEqual(grid.Mode, Mode.Edit);
            Assert.AreEqual(grid.MasterTable.Rows.Count, 1);

            Assert.IsTrue(gridrowClicked);
        }

        [Test]
        public void GridRowEventSuccess()
        {
            Grid grid = new Grid();

            CreateNewCategory(grid);
            grid.RaisePostBackEvent("RecordCancelClick!");
            grid.GridRowBound += grid_GridRowEvent;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);

            Assert.AreEqual(grid.Mode, Mode.Grid);
            Assert.AreEqual(rows, grid.Rows.Count);
        }

        [Test]
        public void GridSearchEvents()
        {
            Grid grid = new Grid();
            grid.SearchChanged += grid_SearchChangedEvents;
            CreateNewCategory(grid);
            grid.Page = Testpage;
            grid.Search = "DoesNotExist";
            grid.RaisePostBackEvent("RecordCancelClick!");

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);
            grid.Dispose();
            Assert.AreEqual(grid.Mode, Mode.Grid);
            Assert.AreEqual(grid.MasterTable.Rows.Count, 0);
            //Assert.IsTrue(searcheventrunnned);
            // Is not triggered because HttpContext is missing during the test phase.
        }

        [Test]
        public void GridUpdateGridRowsClickEvent()
        {
            Grid grid = new Grid();


            grid.DataSourceId = "Categories";
            grid.ID = "test";
            grid.ConnectionString = ConnectionAccessOleDb;
            grid.DefaultVisibility = Visibility.Both;
            grid.RecordsPerRow = 2;
            grid.PagerType = PagerType.Standard;
            grid.Width = Unit.Pixel(1000);

            Assert.IsNull(grid.CurrentId);
            grid.UpdateRowsClick += tmp_UpdateRowsClickEvents;
            grid.BeforeUpdateInsert += tmp_BeforeUpdateInsertUpdateRow;
            grid.RaisePostBackEvent("UpdateRowsClick!");
            grid["CategoryName"].AllowEditInGrid = true;
            grid["Description"].AllowEditInGrid = true;
            grid["CategoryId"].AllowEditInGrid = true;
            grid["CategoryId"].Visibility = Visibility.Both;

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.Mode, Mode.Grid);
            Assert.LessOrEqual(grid.SystemMessage.Count, 1);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
            Assert.AreEqual(grid.MasterTable.Rows.Count, updaterowseventscount);
            //  foreach (WebGrid.Row row in tmp.MasterTable.Rows)
            //      Console.WriteLine(row["Description"].Value);
            Assert.IsTrue(updaterowsclickevents);
        }


        [Test]
        public void GridUpdateGridRowsClick()
        {
            Grid grid = new Grid();


            grid.DataSourceId = "Categories";
            grid.ID = "test";
            grid.ConnectionString = ConnectionAccessOleDb;
            grid.DefaultVisibility = Visibility.Both;
            grid.RecordsPerRow = 2;
            grid.PageLength = 34;
            grid.PagerType = PagerType.Standard;
            grid.Width = Unit.Pixel(1000);

            Assert.IsNull(grid.CurrentId);
            grid["CategoryName"].AllowEditInGrid = true;
            grid["Description"].AllowEditInGrid = true;
            grid["CategoryId"].AllowEditInGrid = true;
            grid["CategoryId"].Primarykey = true;
            grid["CategoryId"].Visibility = Visibility.Both;
            grid["Description"].MaxSize = 10;

            grid.RaisePostBackEvent("UpdateRowsClick!");
            int loadData = grid.Rows.Count;
            grid.Rows[0]["Description"].Value = "bla bla bla blah";
            grid.RaisePostBackEvent("UpdateRowsClick!");
            
            
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.Mode, Mode.Grid);
            Assert.LessOrEqual(grid.SystemMessage.Count, 2);
            Assert.AreEqual("Value must be at least 10 characters.", grid.SystemMessage[0].Message);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
            Assert.AreEqual(grid.MasterTable.Rows.Count, loadData);
            Assert.AreEqual(34, loadData);
       
        }

        [Test]
        public void GridUpdateRecordEvent()
        {
            Grid grid = new Grid();
            grid = CreateNewCategory(grid);
            grid.CurrentId = "6";
            int records = grid.MasterTable.Rows.Count;

            Assert.AreEqual(grid.Mode, Mode.Edit);
            grid.RecordUpdateClick += grid_RecordUpdateClickDisallow;
            grid.RaisePostBackEvent("RecordUpdateClick!!False");
            Assert.AreEqual(grid.SystemMessage.Count, 0);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.ReLoadData = true;
            grid.RenderControl(gridwriter);

            Assert.IsTrue(gridrecordupdateclickdisallow);
            Assert.AreEqual(grid.MasterTable.Rows.Count, records);
            Assert.AreEqual(grid.Mode, Mode.Edit);
        }
    }
}