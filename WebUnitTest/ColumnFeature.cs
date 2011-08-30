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
using WebGrid.Data;
using WebGrid.Enums;
using WebGrid.Events;
using Decimal=WebGrid.Columns.Decimal;

namespace WebGridUnitTest
{
    [TestFixture]
    public class ColumnFeature : UnitTestCommon
    {
        private void SetupGrid(Grid grid)
        {
            grid.ID = "wggrid";
            grid.DefaultVisibility = Visibility.Both;
            grid.RecordsPerRow = 2;
            grid.PagerType = PagerType.Standard;
            grid.Width = Unit.Pixel(1000);
            grid.DataSourceId = "Categories";
            grid.ConnectionString = ConnectionAccessOleDb;
            grid.RaisePostBackEvent("NewRecordClick!");
            Testpage.Controls.Add(grid);

        }

        private int recordsdisplayed;

        private void tmp_GridRowTotalSummary(object sender, GridRowBoundEventArgs e)
        {
            recordsdisplayed++;
        }

        [Test]
        public void AllowEditInGrid()
        {
            Grid grid = new Grid();


            SetupGrid(grid);

            grid["CategoryName"].AllowEditInGrid = true;
            grid["Description"].AllowEditInGrid = true;
            grid["CategoryId"].AllowEditInGrid = true;
            grid["CategoryId"].Visibility = Visibility.Both;

            Assert.AreEqual(grid.Mode, Mode.Edit);
            grid.RaisePostBackEvent("RecordCancelClick!");

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.Mode, Mode.Grid);
            Assert.LessOrEqual(grid.SystemMessage.Count, 1);
            Assert.Greater(grid.Rows.Count, 1);
        }

        [Test]
        public void AllowEditInGridWithDebug()
        {
            Grid grid = new Grid();


            SetupGrid(grid);

            grid["CategoryName"].AllowEditInGrid = true;
            grid["Description"].AllowEditInGrid = true;
            grid["CategoryId"].AllowEditInGrid = true;
            grid["CategoryId"].Visibility = Visibility.Both;
            grid.Debug = true;
            Assert.AreEqual(grid.Mode, Mode.Edit);
            grid.RaisePostBackEvent("RecordCancelClick!");

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.Mode, Mode.Grid);
            Assert.LessOrEqual(grid.SystemMessage.Count, 1);
            Assert.Greater(grid.Rows.Count, 1);
        }

        [Test]
        public void AreUniqueValue()
        {
            Grid grid = new Grid();


            SetupGrid(grid);

            grid["CategoryName"].UniqueValueRequired = true;
            grid.MasterTable.Rows[0]["CategoryName"].Value = "Beverages"; //Value that already exists in database.
            grid.RaisePostBackEvent("RecordUpdateClick!!False");

            Assert.AreEqual(grid.Mode, Mode.Edit);
            Assert.AreEqual(grid.SystemMessage.Count, 1); 
        }

        [Test]
        public void GridDisplayTotalSummary()
        {
            Grid grid = new Grid();

            Decimal deccolumn = new Decimal("sum", grid);
            deccolumn.Sum = "[CategoryId]*50";
            deccolumn.Format = "N3";
            deccolumn.Visibility = Visibility.Both;
            deccolumn.DisplayTotalSummary = true;
            grid.MasterTable.Columns.Add(deccolumn);
            grid.GridRowBound += tmp_GridRowTotalSummary;
            SetupGrid(grid);
            grid.ID = "test34";
            grid.RaisePostBackEvent("RecordCancelClick!");

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            foreach (Row row in grid.MasterTable.Rows)
            {
                if (row["sum"].Value == null)
                    throw new ApplicationException("TotalSumSummary value for 'sum' column is null.");
            }
            Assert.AreEqual(grid.Mode, Mode.Grid);
            Assert.Greater(recordsdisplayed, 1);
        }

        [Test]
        public void GridSaveMethodEditMode()
        {
            Grid grid = new Grid();


            SetupGrid(grid);

            grid.StayEdit = true;
            grid.MasterTable.Rows[0]["CategoryName"].Value = "Beverages..."; //Value that already exists in database.
            grid.RaisePostBackEvent("RecordUpdateClick!!False");
            grid.Save();
            Assert.AreEqual(grid.Mode, Mode.Edit);
        }

        [Test]
        public void GridSaveMethodGridMode()
        {
            Grid grid = new Grid();


            SetupGrid(grid);
            grid.RaisePostBackEvent("RecordCancelClick!");
            grid.Mode = Mode.Grid;
            grid["CategoryName"].AllowEditInGrid = true;
            grid["Description"].AllowEditInGrid = true;
            grid["CategoryId"].AllowEditInGrid = true;
            grid["CategoryId"].Visibility = Visibility.Both;
            grid.MasterTable.Rows[0]["CategoryName"].Value = "Beverages..."; //Value that already exists in database.
            grid.Save();

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);

            Assert.AreEqual("Beverages...", grid.MasterTable.Rows[0]["CategoryName"].Value.ToString());

            Assert.AreEqual(grid.Mode, Mode.Grid);
        }

        [Test]
        public void GridSearchFoundSearch()
        {
            Grid grid = new Grid();


            SetupGrid(grid);

            grid["CategoryName"].Searchable = true;
            grid["Description"].Searchable = true;
            grid["CategoryId"].Searchable = false;

            Assert.AreEqual(grid.Mode, Mode.Edit);
            grid.RaisePostBackEvent("RecordCancelClick!");
            grid.Search = "fruit";

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.DataSourceType, DataSourceControlType.InternalDataSource);
        
            Assert.AreEqual(grid.Mode, Mode.Grid);
            Assert.LessOrEqual(grid.SystemMessage.Count, 1); // trial message...
            Assert.AreEqual(grid.MasterTable.Rows.Count, 0); // one "fruit" text should exist in table..
        }

        [Test]
        public void GridSearchNoSearchable()
        {
            Grid grid = new Grid();


            SetupGrid(grid);

            grid["CategoryName"].Searchable = true;
            grid["Description"].Searchable = true;
            grid["CategoryId"].Searchable = false;

            Assert.AreEqual(grid.Mode, Mode.Edit);
            grid.RaisePostBackEvent("RecordCancelClick!");
            grid.Search = "doesnotexist";

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.Mode, Mode.Grid);
            Assert.LessOrEqual(grid.SystemMessage.Count, 1); // trial message...
            Assert.AreEqual(grid.MasterTable.Rows.Count, 0);
        }

        [Test]
        public void GridCopyRowCancel()
        {
            Grid grid = new Grid();


            SetupGrid(grid);
            grid.AllowCopy = true;
            grid.CopyRow += tmp_CopyRow;
            Assert.AreEqual(grid.Mode, Mode.Edit);
            grid.RaisePostBackEvent("RecordCancelClick!");
            int rows = grid.Rows.Count;
            grid.RaisePostBackEvent("RecordCopyClick!!" + grid.Rows[0]["CategoryId"].Value);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.Mode, Mode.Grid);
            Assert.AreEqual(grid.SystemMessage.Count, 1); // trial message...
            Assert.AreEqual(grid.MasterTable.Rows.Count, rows);
        }

        static void tmp_CopyRow(object sender, CopyRowEventArgs e)
        {
            e.AcceptChanges = false;
        }

        [Test]
        public void GridCopyRowChangeColumn()
        {
            Grid grid = new Grid();


            SetupGrid(grid);
            grid.AllowCopy = true;
            grid.CopyRow += tmp_CopyRowChange;
            Assert.AreEqual(grid.Mode, Mode.Edit);
            grid.RaisePostBackEvent("RecordCancelClick!");
            Assert.AreEqual(grid.Mode, Mode.Grid);
            Assert.Greater(grid.Rows.Count, 2);

            grid.RaisePostBackEvent("RecordCopyClick!!" + grid.Rows[0]["CategoryId"].Value);


            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.Mode, Mode.Edit);
            Assert.AreEqual(grid.SystemMessage.Count, 1); // trial message...
            Assert.AreEqual(grid.MasterTable.Rows.Count, 1);
            Assert.AreEqual(grid.MasterTable.Rows[0]["Description"].Value, "mytest");
        }

        static void tmp_CopyRowChange(object sender, CopyRowEventArgs e)
        {
            e.Row["Description"].Value = "mytest";
        }






        [Test]
        public void MaximumSize()
        {
            Grid grid = new Grid();


            SetupGrid(grid);

            grid["CategoryName"].MaxSize = 5;
            grid.MasterTable.Rows[0]["CategoryName"].Value = "Beverages...";
            grid.RaisePostBackEvent("RecordUpdateClick!!False");

            Assert.AreEqual(grid.SystemMessage.Count, 1);
            Assert.AreNotEqual(grid.SystemMessage[0].Message.IndexOf("can not contain more than 5 characters"), -1, grid.SystemMessage[0].Message);
            Assert.AreEqual(Mode.Edit, grid.Mode);
        }

        [Test]
        public void MinimumSize()
        {
            Grid grid = new Grid();


            SetupGrid(grid);

            grid["CategoryName"].MinSize = 50;
            grid.MasterTable.Rows[0]["CategoryName"].Value = "Beverages...";
            grid.RaisePostBackEvent("RecordUpdateClick!!False");

            Assert.AreEqual(grid.Mode, Mode.Edit);
            Assert.AreNotEqual(grid.SystemMessage[0].Message.IndexOf("must be at least 50 characters long."), -1, grid.SystemMessage[0].Message);
            Assert.AreEqual(grid.SystemMessage.Count, 1);
        }


        [Test]
        public void NewRowInEdit()
        {
            Grid grid = new Grid();


            SetupGrid(grid);

            grid["CategoryName"].NewRowInEdit = false;
            grid["Description"].NewRowInEdit = false;
            grid["CategoryId"].NewRowInEdit = false;
            grid["CategoryId"].Visibility = Visibility.Both;

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);

            Assert.AreEqual(grid.Mode, Mode.Edit);
            Assert.AreEqual(grid.SystemMessage.Count, 1);
        }

        [Test]
        public void NewRowInGrid()
        {
            Grid grid = new Grid();


            SetupGrid(grid);

            grid["CategoryName"].NewRowInGrid = true;
            grid["Description"].NewRowInGrid = true;
            grid["CategoryId"].NewRowInGrid = true;
            grid["CategoryId"].Visibility = Visibility.Both;

            Assert.AreEqual(grid.Mode, Mode.Edit);
            Assert.AreEqual(grid.MasterTable.Rows.Count, 1);
            grid.RaisePostBackEvent("RecordCancelClick!");

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);

            Assert.AreEqual(grid.Mode, Mode.Grid);
            Assert.LessOrEqual(grid.SystemMessage.Count, 1);
        }

        [Test]
        public void PropertyInGridEdit()
        {
            Grid grid = new Grid();


            SetupGrid(grid);

            grid["CategoryName"].DisplayIndex = 10;
            grid["Description"].DisplayIndex = 20;
            grid["CategoryId"].DisplayIndex = 30;
            Assert.AreEqual(grid.Mode, Mode.Edit);
            grid.RaisePostBackEvent("RecordCancelClick!");

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.Mode, Mode.Grid);
            Assert.LessOrEqual(grid.SystemMessage.Count, 1);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);

        }
    }
}