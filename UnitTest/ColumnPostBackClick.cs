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
    using Checkbox = WebGrid.Checkbox;
    using DateTime = WebGrid.DateTime;
    using Decimal = WebGrid.Decimal;
    using WebGrid.Design;
    using WebGrid.Enums;
    using Foreignkey = WebGrid.Foreignkey;
    using GridColumn = WebGrid.GridColumn;
    using ManyToMany = WebGrid.ManyToMany;
    using Number = WebGrid.Number;
    using SystemColumn = WebGrid.SystemColumn;
    using Text = WebGrid.Text;

    [TestFixture]
    public class ColumnPostBackClick : UnitTestCommon
    {
        #region Methods

        [Test]
        public void CheckboxColumn()
        {
            Grid grid = new Grid();

            Checkbox column = new Checkbox("column", grid.MasterTable);
            column.Required = true;
            column.Primarykey = true;
            Checkbox column2 = new Checkbox("column2", grid.MasterTable);
            Checkbox column3 = new Checkbox("column3", grid.MasterTable);
            Checkbox column4 = new Checkbox("column4", grid.MasterTable);

            grid.MasterTable.Columns.Add(column);
            grid.MasterTable.Columns.Add(column2);
            grid.MasterTable.Columns.Add(column3);
            grid.MasterTable.Columns.Add(column4);

            SetupGrid(grid);

            // Not allowed content
            grid.MasterTable.Rows[0]["column"].Value = "1";
            grid.MasterTable.Rows[0]["column2"].Value = "sdf30";
            grid.MasterTable.Rows[0]["column3"].Value = "TRUE";
            grid.MasterTable.Rows[0]["column4"].Value = "";

            Assert.AreEqual(grid.DisplayView, DisplayView.Detail);
            grid.RaisePostBackEvent("RecordUpdateClick!!False");
            Assert.AreEqual(grid.SystemMessage.Count, 0);
        }

        [Test]
        public void DateColumn()
        {
            Grid grid = new Grid();

            DateTime column = new DateTime("column", grid.MasterTable);
            column.Required = true;
            column.Primarykey = true;
            DateTime column2 = new DateTime("column2", grid.MasterTable);
            DateTime column3 = new DateTime("column3", grid.MasterTable);
            DateTime column4 = new DateTime("column4", grid.MasterTable);

            DateTime column5 = new DateTime("column5", grid.MasterTable);
            column5.Format = "dd.MM.yyyy";
            DateTime column6 = new DateTime("column6", grid.MasterTable);
            column6.Format = "dd/MM/yyyy";
            DateTime column7 = new DateTime("column7", grid.MasterTable);
            column7.Format = "dd/MM/yy";
            DateTime column8 = new DateTime("column8", grid.MasterTable);
            column8.Format = "MM/dd/yy";
            grid.Culture = new System.Globalization.CultureInfo("nb-NO");
            grid.MasterTable.Columns.Add(column);
            grid.MasterTable.Columns.Add(column2);
            grid.MasterTable.Columns.Add(column3);
            grid.MasterTable.Columns.Add(column4);
            grid.MasterTable.Columns.Add(column5);
            grid.MasterTable.Columns.Add(column6);
            grid.MasterTable.Columns.Add(column7);
            grid.MasterTable.Columns.Add(column8);

            SetupGrid(grid);

            // Not allowed content
            grid.MasterTable.Rows[0]["column"].Value = "";
            grid.MasterTable.Rows[0]["column2"].Value = "mm.YYYY.333";
            grid.MasterTable.Rows[0]["column3"].Value = "35.34.23";
            grid.MasterTable.Rows[0]["column4"].Value = "test test";

            // Allowed Content
            grid.MasterTable.Rows[0]["column5"].Value = "21.12.2007";
            grid.MasterTable.Rows[0]["column6"].Value = "21/12/2007";
            grid.MasterTable.Rows[0]["column7"].Value = "21/12/07";
            grid.MasterTable.Rows[0]["column8"].Value = "12/21/07";

            Assert.AreEqual(grid.DisplayView, DisplayView.Detail);
            grid.RaisePostBackEvent("RecordUpdateClick!!False");

            Assert.AreSame("", grid.MasterTable.Rows[0]["column"].Value.ToString());
            Assert.AreSame("mm.YYYY.333", grid.MasterTable.Rows[0]["column2"].Value.ToString());
            Assert.AreSame("35.34.23", grid.MasterTable.Rows[0]["column3"].Value.ToString());
            Assert.AreSame("test test", grid.MasterTable.Rows[0]["column4"].Value.ToString());

            Assert.AreEqual(grid.SystemMessage.Count, 4);
        }

        [Test]
        public void DecimalColumn()
        {
            Grid grid = new Grid();

            Decimal column = new Decimal("column", grid.MasterTable);
            column.Required = true;
            column.Primarykey = true;
            Decimal column2 = new Decimal("column2", grid.MasterTable);
            Decimal column3 = new Decimal("column3", grid.MasterTable);
            Decimal column4 = new Decimal("column4", grid.MasterTable);
            Decimal column5 = new Decimal("column5", grid.MasterTable);
            Decimal column6 = new Decimal("column6", grid.MasterTable);
            Decimal column7 = new Decimal("column7", grid.MasterTable);
            Decimal column8 = new Decimal("column8", grid.MasterTable);

            grid.MasterTable.Columns.Add(column);
            grid.MasterTable.Columns.Add(column2);
            grid.MasterTable.Columns.Add(column3);
            grid.MasterTable.Columns.Add(column4);
            grid.MasterTable.Columns.Add(column5);
            grid.MasterTable.Columns.Add(column6);
            grid.MasterTable.Columns.Add(column7);
            grid.MasterTable.Columns.Add(column8);

            SetupGrid(grid);

            // Not allowed content
            grid.MasterTable.Rows[0]["column"].Value = "";
            grid.MasterTable.Rows[0]["column2"].Value = "test";
            grid.MasterTable.Rows[0]["column3"].Value = "<'.3%&#'34,'5>";

            // Allowed Content
            grid.MasterTable.Rows[0]["column4"].Value = "0,5";
            grid.MasterTable.Rows[0]["column5"].Value = "34";
            grid.MasterTable.Rows[0]["column6"].Value = "1034, 5";
            grid.MasterTable.Rows[0]["column7"].Value = "233 45,5";
            grid.MasterTable.Rows[0]["column8"].Value = "0,45";

            grid.MasterTable.Rows[0]["column8"].Value = 45345;

            Assert.AreEqual(grid.DisplayView, DisplayView.Detail);
            grid.RaisePostBackEvent("RecordUpdateClick!!False");

            Assert.AreSame("", grid.MasterTable.Rows[0]["column"].Value.ToString());
            Assert.AreSame("test", grid.MasterTable.Rows[0]["column2"].Value.ToString());
            Assert.AreSame("<'.3%&#'34,'5>", grid.MasterTable.Rows[0]["column3"].Value.ToString());
            Assert.AreEqual(grid.DisplayView, DisplayView.Detail);

            Assert.AreEqual(grid.SystemMessage.Count, 3);
        }

        [Test]
        public void ForeignKeyColumn()
        {
            Grid grid = new Grid();

            Foreignkey column = new Foreignkey("column", grid.MasterTable);
            column.ForeignkeyType = ForeignkeyType.Radiobuttons;
            column.DataSourceId = "Employees";
            column.NullText = "Select Employees";
            column.Required = true;

            Foreignkey column2 = new Foreignkey("column2", grid.MasterTable);
            column2.ForeignkeyType = ForeignkeyType.Select;
            column2.DataSourceId = "Employees";
            column2.NullText = "Select Employees";

            Foreignkey column3 = new Foreignkey("column3", grid.MasterTable);
            column3.ForeignkeyType = ForeignkeyType.Radiobuttons;
            column3.RecordsPerRow = 2;
            column3.DataSourceId = "Employees";
            Foreignkey column4 = new Foreignkey("column4", grid.MasterTable);
            column4.TreeIndentText = "ReportsTo";
            column4.DataSourceId = "Employees";
            Foreignkey column5 = new Foreignkey("column5", grid.MasterTable);
            column5.TreeIndentText = "ReportsTo";
            column5.DataSourceId = "Employees";
            column5.NullText = "Select Employees";

            Foreignkey column6 = new Foreignkey("column6", grid.MasterTable);
            column6.TreeIndentText = "ReportsTo";
            column6.DataSourceId = "Employees";
            column6.FilterExpression = "[Employees].[ReportsTo] IS NOT NULL";
            column6.NullText = "Select Employees";

            Foreignkey column7 = new Foreignkey("column7", grid.MasterTable);
            column7.TreeIndentText = "ReportsTo";
            column7.DataSourceId = "Employees";
            column7.FilterExpression = "[Employees].[ReportsTo] IS NOT NULL";
            column7.NullText = "Select Employees";
            column7.ForeignkeyType = ForeignkeyType.Radiobuttons;

            Foreignkey column8 = new Foreignkey("column8", grid.MasterTable);
            column8.DataSourceId = "Employees";
            column8.FilterExpression = "[Employees].[ReportsTo] IS NOT NULL";
            column8.NullText = "Select Employees";
            column8.ForeignkeyType = ForeignkeyType.Radiobuttons;

            grid.MasterTable.Columns.Add(column);
            grid.MasterTable.Columns.Add(column2);
            grid.MasterTable.Columns.Add(column3);
            grid.MasterTable.Columns.Add(column4);
            grid.MasterTable.Columns.Add(column5);
            grid.MasterTable.Columns.Add(column6);
            grid.MasterTable.Columns.Add(column7);
            grid.MasterTable.Columns.Add(column8);

            SetupGrid(grid);

            grid.DisplayView = DisplayView.Grid;
            grid.DataSourceId = "Employees";
            grid.ConnectionString = ConnectionAccessOleDb;

            // Allowed Content
            grid.MasterTable.Rows[0]["column"].DisplayValue = "";
             grid.MasterTable.Rows[0]["column2"].DisplayValue = "3";
            grid.MasterTable.Rows[0]["column3"].DisplayValue = "3";
             grid.MasterTable.Rows[0]["column4"].DisplayValue = "3";
            grid.MasterTable.Rows[0]["column5"].DisplayValue = "3";
            grid.MasterTable.Rows[0]["column7"].DisplayValue = "3";
            grid.MasterTable.Rows[0]["column8"].DisplayValue = "3";

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
            Assert.LessOrEqual(grid.SystemMessage.Count, 1); // 15-day license key message
            Assert.AreEqual(grid.DisplayView, DisplayView.Grid);
        }

        [Test]
        public void GridColumn()
        {
            Grid GridSecond = new Grid();
            Testpage.Controls.Add(GridSecond);
            Grid gridPrimary = new Grid();

            GridSecond.ID = "gridslave";
            GridSecond.DataSourceId = "Orders";
            GridSecond.ConnectionString = ConnectionAccessOleDb;
            GridSecond.MasterGrid = "wggrid";

            GridColumn column = new GridColumn("column", gridPrimary.MasterTable);
            column.UseAllRows = true;
            column.ColumnId = "wgMasterDetails";
            column.HideDetailTitle = true;
            column.GridId = "gridslave";

            gridPrimary.MasterTable.Columns.Add(column);

            SetupGrid(gridPrimary);

            gridPrimary.DisplayView = DisplayView.Detail;
            gridPrimary.EditIndex = "4";
            gridPrimary.DataSourceId = "Employees";
            gridPrimary.ConnectionString = ConnectionAccessOleDb;

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            gridPrimary.RenderControl(gridwriter);
            Assert.AreEqual(gridPrimary.MasterTable.Rows.Count, 1);
            Assert.Greater(GridSecond.MasterTable.Rows.Count, 1);
            Assert.LessOrEqual(gridPrimary.SystemMessage.Count, 1); // 15-day license key message
            Assert.AreEqual(gridPrimary.DisplayView, DisplayView.Detail);
            Assert.AreEqual(GridSecond.DisplayView, DisplayView.Grid);
        }

        [Test]
        public void ManyToMany()
        {
            Testpage.Controls.Clear();
               Grid grid = new Grid();

            ManyToMany column = new ManyToMany("column", grid.MasterTable);
            column.MatrixIdentityColumn = "EmployeeID";
            column.MatrixDataSource = "EmployeeTerritories";
            column.FilterExpression = "RegionID = 3";
            column.ValueColumn = "TerritoryDescription";
            column.ForeignDataSource = "Territories";
            column.ForeignIdentityColumn = "TerritoryID";
            column.RecordsPerRow = 4;
            column.ManyToManyType = ManyToManyType.Checkboxes;

            ManyToMany column2 = new ManyToMany("column2", grid.MasterTable);
            column2.MatrixIdentityColumn = "EmployeeID";
            column2.MatrixDataSource = "EmployeeTerritories";
            column2.FilterExpression = "RegionID = 2";
            column2.ValueColumn = "TerritoryDescription";
            column2.ForeignDataSource = "Territories";
            column2.ForeignIdentityColumn = "TerritoryID";
            column2.ManyToManyType = ManyToManyType.Multiselect;

            grid.MasterTable.Columns.Add(column);
            grid.MasterTable.Columns.Add(column2);

            SetupGrid(grid);
            grid.DisplayView = DisplayView.Detail;
            grid.StayInDetail = true;
            grid.EditIndex = "4";
            grid.DataSourceId = "Employees";
            grid.ConnectionString = ConnectionAccessOleDb;

            grid.RaisePostBackEvent("RecordUpdateClick!!False");
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.AreEqual(grid.MasterTable.Rows.Count, 1);
            Assert.Greater(column.Items.Count, 1);
            Assert.Greater(column2.Items.Count, 1);
            if (grid.SystemMessage.Count > 2)
                foreach (SystemMessage message in grid.SystemMessage)
                    Console.WriteLine(message.Message);

            //When running bulk tests we get an extra systemmessage because Grid.ID stored in TestPage object.
            Assert.LessOrEqual(grid.SystemMessage.Count, 2); // 15-day license key message
            Assert.AreEqual(grid.DisplayView, DisplayView.Detail);

            string[] triggerStrings = new[]
                                          {
                                              "type=\"checkbox\"",
                                              "id=\"cb_wggrid_4_column_01581\"",
                                          "for=\"cb_wggrid_4_column_01581\"",
                                              "type=\"checkbox\" id=\"cb_wggrid_4_column_55439\""
                                          };
            string content = sb.ToString();
            foreach (string part in triggerStrings)
            {
                int res = content.IndexOf(part);
                Assert.Greater(res, -1, part + Environment.NewLine + content);
            }
        }

        [Test]
        public void NumberColumn()
        {
            Grid grid = new Grid();

            Number column = new Number("column", grid.MasterTable);
            column.Required = true;
            column.Primarykey = true;

            Number column2 = new Number("column2", grid.MasterTable);
            Number column3 = new Number("column3", grid.MasterTable);
            Number column4 = new Number("column4", grid.MasterTable);
            Number column5 = new Number("column5", grid.MasterTable);
            Number column6 = new Number("column6", grid.MasterTable);
            Number column7 = new Number("column7", grid.MasterTable);
            Number column8 = new Number("column8", grid.MasterTable);

            grid.MasterTable.Columns.Add(column);
            grid.MasterTable.Columns.Add(column2);
            grid.MasterTable.Columns.Add(column3);
            grid.MasterTable.Columns.Add(column4);
            grid.MasterTable.Columns.Add(column5);
            grid.MasterTable.Columns.Add(column6);
            grid.MasterTable.Columns.Add(column7);
            grid.MasterTable.Columns.Add(column8);

            SetupGrid(grid);

            // Not allowed content
            grid.MasterTable.Rows[0]["column"].Value = "";
            grid.MasterTable.Rows[0]["column2"].Value = "test";
            grid.MasterTable.Rows[0]["column3"].Value = "<'.3%&#'34,'5>";

            // Allowed Content
            grid.MasterTable.Rows[0]["column4"].Value = "05";
            grid.MasterTable.Rows[0]["column5"].Value = "34";
            grid.MasterTable.Rows[0]["column6"].Value = "1 035 000";
            grid.MasterTable.Rows[0]["column7"].Value = "1 000";
            grid.MasterTable.Rows[0]["column8"].Value = "123 000 000";

            Assert.AreEqual(grid.DisplayView, DisplayView.Detail);
            grid.RaisePostBackEvent("RecordUpdateClick!!False");

            Assert.AreSame("", grid.MasterTable.Rows[0]["column"].Value.ToString());
            Assert.AreSame("test", grid.MasterTable.Rows[0]["column2"].Value.ToString());
            Assert.AreSame("<'.3%&#'34,'5>", grid.MasterTable.Rows[0]["column3"].Value.ToString());

            Assert.AreEqual(grid.SystemMessage.Count, 3);
        }

        [Test]
        public void SystemColumn()
        {
            Grid grid = new Grid();

            SystemColumn column = new SystemColumn("column", WebGrid.Enums.SystemColumn.SelectColumn, grid.MasterTable);
            column.Required = true;

            SystemColumn column2 = new SystemColumn("column2", WebGrid.Enums.SystemColumn.DeleteColumn, grid.MasterTable);

            SystemColumn column3 = new SystemColumn("column3", WebGrid.Enums.SystemColumn.CopyColumn, grid.MasterTable);
             SystemColumn column5 = new SystemColumn("column5", WebGrid.Enums.SystemColumn.NewRecordColumn, grid.MasterTable);
            SystemColumn column6 = new SystemColumn("column6", WebGrid.Enums.SystemColumn.RowColumn, grid.MasterTable);
            SystemColumn column7 = new SystemColumn("column7", WebGrid.Enums.SystemColumn.DeleteColumn, grid.MasterTable);
            SystemColumn column8 = new SystemColumn("column8", WebGrid.Enums.SystemColumn.UpdateGridRecordsColumn, grid.MasterTable);

            grid.MasterTable.Columns.Add(column);
            grid.MasterTable.Columns.Add(column2);
            grid.MasterTable.Columns.Add(column3);
             grid.MasterTable.Columns.Add(column5);
            grid.MasterTable.Columns.Add(column6);
            grid.MasterTable.Columns.Add(column7);
            grid.MasterTable.Columns.Add(column8);

            SetupGrid(grid);
            grid.DisplayView = DisplayView.Grid;
            grid.DataSourceId = "Categories";
            grid.ConnectionString = ConnectionAccessOleDb;

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
            Assert.LessOrEqual(grid.SystemMessage.Count, 1); // 15-day license key message
            Assert.AreEqual(grid.DisplayView, DisplayView.Grid);
        }

        [Test]
        public void TextColumn()
        {
            Grid grid = new Grid();

            Text column = new Text("column", grid.MasterTable);
            column.Required = true;
            column.Primarykey = true;
            Text column2 = new Text("column2", grid.MasterTable);
            column2.IsEmail = true;

            Text column3 = new Text("column3", grid.MasterTable);
            column3.IsEmail = true;

            Text column4 = new Text("column4", grid.MasterTable);
            column4.IsUrl = true;
            column4.Required = true;

            Text column5 = new Text("column5", grid.MasterTable);
            column.IsPassword = true;

            Text column6 = new Text("column6", grid.MasterTable);
            Text column7 = new Text("column7", grid.MasterTable);
            Text column8 = new Text("column8", grid.MasterTable);

            grid.MasterTable.Columns.Add(column);
            grid.MasterTable.Columns.Add(column2);
            grid.MasterTable.Columns.Add(column3);
            grid.MasterTable.Columns.Add(column4);
            grid.MasterTable.Columns.Add(column5);
            grid.MasterTable.Columns.Add(column6);
            grid.MasterTable.Columns.Add(column7);
            grid.MasterTable.Columns.Add(column8);

            SetupGrid(grid);
            // Not allowed content
            grid.MasterTable.Rows[0]["column"].Value = "";
            grid.MasterTable.Rows[0]["column4"].Value = "";
            grid.MasterTable.Rows[0]["column3"].Value = "invalid@email";

            // Allowed Content
            grid.MasterTable.Rows[0]["column2"].Value = "valid@email.com";
            grid.MasterTable.Rows[0]["column5"].Value = "somepassword";
            grid.MasterTable.Rows[0]["column6"].Value = "<validtext>";
            grid.MasterTable.Rows[0]["column7"].Value = "<%valid text%>";
            grid.MasterTable.Rows[0]["column8"].Value = "valid text";

            Assert.AreEqual(grid.DisplayView, DisplayView.Detail);
            grid.RaisePostBackEvent("RecordUpdateClick!!False");

            Assert.AreSame("", grid.MasterTable.Rows[0]["column"].Value.ToString());
            Assert.AreSame("", grid.MasterTable.Rows[0]["column4"].Value.ToString());
            Assert.AreSame("invalid@email", grid.MasterTable.Rows[0]["column3"].Value.ToString());

            Assert.AreEqual(grid.SystemMessage.Count, 3);
        }

        private void SetupGrid(Grid grid)
        {
            grid.ID = "wggrid";
            grid.DefaultVisibility = Visibility.Both;
            grid.RecordsPerRow = 2;
            grid.PagerSettings.PagerType = PagerType.None;
            grid.Width = Unit.Pixel(1000);

            grid.RaisePostBackEvent("NewRecordClick!");
            Testpage.Controls.Add(grid);
        }

        #endregion Methods
    }
}