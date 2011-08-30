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
    using WebGrid.Data;
    using Decimal = WebGrid.Decimal;
    using WebGrid.Enums;
    using WebGrid.Events;

    [TestFixture]
    public class ColumnFeature : UnitTestCommon
    {
        #region Fields

        private int recordsdisplayed;

        #endregion Fields

        #region Methods

        [Test]
        public void AllowEditInGrid()
        {
            Grid grid = new Grid();

            SetupGrid(grid);

            grid["CategoryName"].AllowEditInGrid = true;
            grid["Description"].AllowEditInGrid = true;
            grid["CategoryId"].AllowEditInGrid = true;
            grid["CategoryId"].Visibility = Visibility.Both;

            Assert.AreEqual(grid.DisplayView, DisplayView.Detail);
            grid.RaisePostBackEvent("RecordCancelClick!");

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.DisplayView, DisplayView.Grid);
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
            Assert.AreEqual(grid.DisplayView, DisplayView.Detail);
            grid.RaisePostBackEvent("RecordCancelClick!");

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.DisplayView, DisplayView.Grid);
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

            Assert.AreEqual(grid.DisplayView, DisplayView.Detail);
            Assert.AreEqual(grid.SystemMessage.Count, 1);
            Assert.AreNotEqual(grid.SystemMessage[0].Message.IndexOf("Similiar value in field 'CategoryName' already exists."), -1,
                               grid.SystemMessage[0].Message);
        }

        [Test]
        public void GridCopyRowCancel()
        {
            Grid grid = new Grid();

            SetupGrid(grid);
            grid.AllowCopy = true;
            grid.CopyRow += tmp_CopyRow;
            Assert.AreEqual(grid.DisplayView, DisplayView.Detail);
            grid.RaisePostBackEvent("RecordCancelClick!");
            int rows = grid.Rows.Count;
            grid.RaisePostBackEvent("RecordCopyClick!!" + grid.Rows[0]["CategoryId"].Value);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.DisplayView, DisplayView.Grid);
            Assert.AreEqual(grid.SystemMessage.Count, 0);
            Assert.AreEqual(grid.MasterTable.Rows.Count, rows);
        }

        [Test]
        public void GridCopyRowChangeColumn()
        {
            Grid grid = new Grid();

            SetupGrid(grid);
            grid.AllowCopy = true;
            grid.CopyRow += tmp_CopyRowChange;
            Assert.AreEqual(grid.DisplayView, DisplayView.Detail);
            grid.RaisePostBackEvent("RecordCancelClick!");
            Assert.AreEqual(grid.DisplayView, DisplayView.Grid);
            Assert.Greater(grid.Rows.Count, 2);

            grid.RaisePostBackEvent("RecordCopyClick!!" + grid.Rows[0]["CategoryId"].Value);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.DisplayView, DisplayView.Detail);
            Assert.AreEqual(grid.SystemMessage.Count, 0);
            Assert.AreEqual(grid.MasterTable.Rows.Count, 1);
            Assert.AreEqual(grid.MasterTable.Rows[0]["Description"].Value, "mytest");
        }

        [Test]
        public void GridDisplayTotalSummary()
        {
            Grid grid = new Grid();

            Decimal deccolumn = new Decimal("sum", grid.MasterTable)
                                    {
                                        Sum = "[CategoryId]*50",
                                        Format = "N3",
                                        Visibility = Visibility.Both,
                                        DisplayTotalSummary = true
                                    };
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
            Assert.AreEqual(grid.DisplayView, DisplayView.Grid);
            Assert.Greater(recordsdisplayed, 1);
        }

        [Test]
        public void GridSaveMethodEditMode()
        {
            Grid grid = new Grid();

            SetupGrid(grid);

            grid.StayInDetail = true;
            grid.MasterTable.Rows[0]["CategoryName"].Value = "Beverages...";
            grid.RaisePostBackEvent("RecordUpdateClick!!False");
            grid.Save();
            Assert.AreEqual(grid.DisplayView, DisplayView.Detail);
        }

        [Test]
        public void GridSaveMethodGridMode()
        {
            Grid grid = new Grid();

            SetupGrid(grid);
            grid.RaisePostBackEvent("RecordCancelClick!");
            grid.DisplayView = DisplayView.Grid;
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

            Assert.AreEqual(grid.DisplayView, DisplayView.Grid);
        }

        [Test]
        public void GridSearchFoundSearch()
        {
            Grid grid = new Grid();

            SetupGrid(grid);

            grid["CategoryName"].Searchable = true;
            grid["Description"].Searchable = true;
            grid["CategoryId"].Searchable = false;

            Assert.AreEqual(grid.DisplayView, DisplayView.Detail);
            grid.RaisePostBackEvent("RecordCancelClick!");
            grid.Search = "fruit";

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.DataSourceType, DataSourceControlType.InternalDataSource);

            Assert.AreEqual(grid.DisplayView, DisplayView.Grid);
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

            Assert.AreEqual(grid.DisplayView, DisplayView.Detail);
            grid.RaisePostBackEvent("RecordCancelClick!");
            grid.Search = "doesnotexist";

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.DisplayView, DisplayView.Grid);
            Assert.LessOrEqual(grid.SystemMessage.Count, 1); // trial message...
            Assert.AreEqual(grid.MasterTable.Rows.Count, 0);
        }

        [Test]
        public void MaximumSize()
        {
            Grid grid = new Grid();

            SetupGrid(grid);

            grid["CategoryName"].MaxSize = 5;
            grid.MasterTable.Rows[0]["CategoryName"].Value = "Beverages...";
            grid.RaisePostBackEvent("RecordUpdateClick!!False");
            Assert.AreEqual(grid.Rows.Count, 1);

            Assert.AreEqual(grid.SystemMessage.Count, 1);
            Assert.AreNotEqual(grid.SystemMessage[0].Message.IndexOf("can not contain more than 5 characters"), -1,
                               grid.SystemMessage[0].Message);
            Assert.AreEqual(DisplayView.Detail, grid.DisplayView);
        }

        [Test]
        public void MinimumSize()
        {
            Grid grid = new Grid();

            SetupGrid(grid);

            grid["CategoryName"].MinSize = 50;
            grid.MasterTable.Rows[0]["CategoryName"].Value = "Beverages...";
            grid.RaisePostBackEvent("RecordUpdateClick!!False");

            Assert.AreEqual(grid.SystemMessage.Count, 1);
            Assert.AreEqual(grid.DisplayView, DisplayView.Detail);
            Assert.AreNotEqual(grid.SystemMessage[0].Message.IndexOf("must be at least 50 characters long."), -1, grid.SystemMessage[0].Message);
            Assert.AreEqual(grid.SystemMessage.Count, 1);
        }

        [Test]
        public void NewRowInDetail()
        {
            Grid grid = new Grid();

            SetupGrid(grid);

            grid["CategoryName"].NewRowInDetail = false;
            grid["Description"].NewRowInDetail = false;
            grid["CategoryId"].NewRowInDetail = false;
            grid["CategoryId"].Visibility = Visibility.Both;

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);

            Assert.AreEqual(grid.DisplayView, DisplayView.Detail);
            Assert.AreEqual(grid.SystemMessage.Count, 0);
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

            Assert.AreEqual(grid.DisplayView, DisplayView.Detail);
            Assert.AreEqual(grid.MasterTable.Rows.Count, 1);
            grid.RaisePostBackEvent("RecordCancelClick!");

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);

            Assert.AreEqual(grid.DisplayView, DisplayView.Grid);
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
            Assert.AreEqual(grid.DisplayView, DisplayView.Detail);
            grid.RaisePostBackEvent("RecordCancelClick!");

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.DisplayView, DisplayView.Grid);
            Assert.LessOrEqual(grid.SystemMessage.Count, 1);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
        }

        [Test]
        [ExpectedException("WebGrid.Design.GridException", "WebGrid must have callback enabled to use tooltip columns, you can enable this by setting 'Grid.EnableCallBack=true'")]
        public void ToolTipColumn()
        {
            Grid grid = new Grid
            {
                DataSourceId = "Order Details",
                ID = "test",
                ConnectionString = ConnectionAccessOleDb
            };
            WebGrid.ToolTipColumn tooltipColumn = new WebGrid.ToolTipColumn("tooltipColumn", grid.MasterTable)
                                                              {
                                                                  ToolTipStyle = StyleName.green,
                                                                  ShowMethod = ShowMethod.dblclick,
                                                                  ToolTipHeight = 300,
                                                                  ToolTipWidth = 400,
                                                                  Text = "WebGrid Loading..",
                Visibility = Visibility.Both,
            };
            tooltipColumn.BeforeToolTipRender += mytest_BeforeToolTipRender;
            grid.MasterTable.Columns.Add(tooltipColumn);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);

            Assert.AreEqual(grid.MasterTable.Columns.Primarykeys.Count, 2);
            Assert.AreEqual(grid.MasterTable.Columns.Primarykeys[0].Identity, false);
            Assert.AreEqual(grid.MasterTable.Columns.Primarykeys[1].Identity, false);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
            string content = sb.ToString();
            string[] triggerStrings = new[]
                                          {
                                              "WebGrid Loading..",
                                              "show: { when: { event: 'dblclick'",
                                            "hide: { when: 'click',",
                                              "style: { name: 'green', width: 400, height: 300 },",
                                              "addClass(\"wgtooltipselected\"",
                                              "removeClass(\"wgtooltipselected\")",
                                              "$('#testtooltipColumnTP1').qtip",
                                              "$('#testtooltipColumnTP2').qtip",
                                              "$('#testtooltipColumnTP5').qtip"
                                          };
            foreach (string part in triggerStrings)
            {
                int res = content.IndexOf(part);
                Assert.Greater(res, -1, part + Environment.NewLine + content);
            }
        }

        static void mytest_BeforeToolTipRender(object sender, BeforeToolTipRenderEventHandlerArgs args)
        {
            args.ReturnData = string.Format("OrderID: {0}", args.Row["OrderId"].Value);
        }

        static void tmp_CopyRow(object sender, CopyRowEventArgs e)
        {
            e.AcceptChanges = false;
        }

        static void tmp_CopyRowChange(object sender, CopyRowEventArgs e)
        {
            e.Row["Description"].Value = "mytest";
        }

        private void SetupGrid(Grid grid)
        {
            grid.ID = "wggrid";
            grid.DefaultVisibility = Visibility.Both;
            grid.RecordsPerRow = 2;
            grid.PagerSettings.PagerType = PagerType.Standard;
            grid.Width = Unit.Pixel(1000);
            grid.DataSourceId = "Categories";
            grid.ConnectionString = ConnectionAccessOleDb;
            grid.RaisePostBackEvent("NewRecordClick!");
            Testpage.Controls.Add(grid);
        }

        private void tmp_GridRowTotalSummary(object sender, GridRowBoundEventArgs e)
        {
            recordsdisplayed++;
        }

        #endregion Methods
    }
}