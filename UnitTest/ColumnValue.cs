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
    using DateTime = System.DateTime;
    using System.IO;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using NUnit.Framework;

    using WebGrid;
    using WebGrid.Enums;

    [TestFixture]
    public class ColumnValue : UnitTestCommon
    {
        #region Fields

        private const string dateformat = "dd.MM.yyyy HH:mm";

        #endregion Fields

        #region Methods

        [Test]
        public void GridRowBound()
        {
            Grid grid = new Grid();
            SetupTestGrid(grid);
            grid.RaisePostBackEvent("RecordCancelClick!");
            grid.GridRowBound += grid_GridRowBound;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
        }

        [Test]
        public void GridRowBoundAllowEditInRow()
        {
            //Update The record set, we are updating record set 11
            SqlConnectionUpdate();

            Grid grid = new Grid();
            SetupTestGrid(grid);
            grid.PageSize = 100;
            grid.Culture = new System.Globalization.CultureInfo("en-US");
            grid.RowHighLight = System.Drawing.Color.Azure;
            ((WebGrid.DateTime)grid["dtmDate"]).Format = dateformat;
            grid.RaisePostBackEvent("RecordCancelClick!");
            grid.GridRowBound += grid_GridRowBound;
            grid["intTest"].AllowEditInGrid = true;
            grid["decTest"].AllowEditInGrid = true;
            grid["dtmDate"].AllowEditInGrid = true;
            grid["vchtest"].AllowEditInGrid = true;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);

            // These are triggers from html being rendered.

            string[] triggerStrings = new[]
                                          {
                                               "wggrid_40_vchTest",
                                              "wggrid_40_decTest",
                                              "wggrid_40_intTest",
                                              "wggrid_40_dtmDate",
                                      //        "wggrid_40_dtmDate_trigger",
                                              "cb_wggrid_40_bitFlag",
                                              "wggrid_40_bitFlag",
                                              "onmouseover=\"wgrowhighlight",
                                        //      "calendar.gif",
                                              "wggrid_11_vchTest",
                                              "wggrid_11_decTest",
                                              "wggrid_11_intTest",
                                              "wggrid_11_dtmDate",
                                      //        "wggrid_11_dtmDate_trigger",
                                              "cb_wggrid_11_bitFlag",
                                              "wggrid_11_bitFlag",
                                              //DateTime column with today's DateTime
                                              "value=\""+DateTime.Now.ToString(dateformat)+"\" id=\"wggrid_11_dtmDate\"",
                                              //Test column with 'blah'+ DateTime
                                         //    "maxlength=\"50\"  value=\"blah" +DateTime.Now.ToString(dateformat)+"\" id=\"wggrid_11_vchTest\"",
                                           //Decimal column with value 5634 formatted default "N2"
                                            "style=\"text-align: Right;\" value=\"5,634.00\" id=\"wggrid_11_decTest\"",
                                            //Number column with 33345 formatted default with "N0"
                                            "style=\"text-align: Right;\" value=\"33,345\" id=\"wggrid_11_intTest\""
                                          };

            string content = sb.ToString();
            foreach (string part in triggerStrings)
            {
                int res = content.IndexOf(part);
                Assert.Greater(res, -1,part+Environment.NewLine+content);
            }
        }

        [Test]
        public void OleDbInsert()
        {
            Grid grid = new Grid();

            SetupGrid(grid);
            grid.StayInDetail = true;

               grid.MasterTable.Rows[0]["CategoryName"].Value = "My Test"; //Value that already exists in database.
            grid.MasterTable.Rows[0]["CategoryName"].Value = "Blah";
            grid.RaisePostBackEvent("RecordUpdateClick!!False");

            Assert.AreEqual(grid.DisplayView, DisplayView.Detail);
            Assert.AreEqual(grid.SystemMessage.Count, 0);
        }

        [Test]
        public void SqlConnectionInsert()
        {
            Grid grid = new Grid();

            SetupTestGrid(grid);

            grid.MasterTable.Rows[0]["intTest"].Value = 345;
            grid.MasterTable.Rows[0]["decTest"].Value = 5634;
            grid.MasterTable.Rows[0]["vchTest"].Value = "blah" + DateTime.Now.ToString(dateformat);
            grid.MasterTable.Rows[0]["dtmDate"].Value = DateTime.Now;
            grid.MasterTable.Rows[0]["bitFlag"].Value = true;
            grid.MasterTable.Rows[0]["bitSecondFlag"].Value = false;
            grid.RaisePostBackEvent("RecordUpdateClick!!False");

            Assert.AreEqual(grid.SystemMessage.Count, 0);
            Assert.AreEqual(grid.DisplayView, DisplayView.Grid);
        }

        [Test]
        public void SqlConnectionUpdate()
        {
            Grid grid = new Grid();

            SetupTestGrid(grid);
            grid.StayInDetail = true;
            grid.EditIndex = "11";
               grid.MasterTable.Rows[0]["intTest"].Value = 33345;
            grid.MasterTable.Rows[0]["decTest"].Value = 5634;
               grid.MasterTable.Rows[0]["vchTest"].Value = "blah" + DateTime.Now.ToString(dateformat);
            ((WebGrid.DateTime)grid.MasterTable.Columns["dtmDate"]).Format = dateformat;
            DateTime date = DateTime.Now;
            grid.MasterTable.Rows[0]["dtmDate"].Value = date;
            grid.MasterTable.Rows[0]["bitFlag"].Value = true;
            grid.MasterTable.Rows[0]["bitSecondFlag"].Value = false;
            grid.RaisePostBackEvent("RecordUpdateClick!!False");

            Assert.AreEqual(grid.SystemMessage.Count, 0);
            Assert.AreEqual(grid.DisplayView, DisplayView.Detail);

            grid = new Grid();

            SetupTestGrid(grid);
            grid.StayInDetail = true;
            grid.EditIndex = "11";
            ((WebGrid.DateTime)grid.MasterTable.Columns["dtmDate"]).Format = dateformat;
            grid.MasterTable.Rows[0]["dtmDate"].Value = DateTime.Now;
            DateTime? seconddate = grid.MasterTable.Rows[0]["dtmDate"].Value as DateTime?;

            //The grid should honour the date format and therefor returns a different date time.
            Assert.AreNotEqual(date, seconddate);

            //Date should be the same
            Assert.AreEqual(date.ToShortDateString(), seconddate.Value.ToShortDateString());
            //Hour and minutes should also be the same.
            Assert.AreEqual(date.ToShortTimeString(), seconddate.Value.ToShortTimeString());
        }

        static void grid_GridRowBound(object sender, WebGrid.Events.GridRowBoundEventArgs e)
        {
            object myvalue = e.Row["intTest"].Value;
            //int type was was retrieved from data source
            if (myvalue == null)
                return;
            Assert.AreSame(myvalue.GetType(), typeof(int));

            //Number is 'long' type
            int? inttest = e.Row["intTest"].Value as int?;
            // Since we casted the value it should now be 'long' type
              //  myvalue = e.Row["intTest"].Value;
             //   Assert.AreSame(myvalue.GetType(), typeof(long));
            Assert.IsNotNull(inttest);

            myvalue = e.Row["decTest"].Value;
            //Decimal type was was retrieved from data source
            Assert.AreSame(myvalue.GetType(), typeof(System.Decimal));

            decimal? dectest = e.Row["decTest"].Value as decimal?;
            Assert.IsNotNull(dectest);

            string txttest = e.Row["vchtest"].Value as string;
            Assert.IsNotNull(txttest);

            myvalue = e.Row["dtmDate"].Value;
            //DateTime type was was retrieved from data source
            if (myvalue != null)
            {
                Assert.AreSame(myvalue.GetType(), typeof (DateTime));
                DateTime? dtmtest = e.Row["dtmDate"].Value as DateTime?;
                Assert.IsNotNull(dtmtest);
            }
        }

        private void SetupGrid(Grid grid)
        {
            grid.ID = "wggrid";
            grid.DefaultVisibility = Visibility.Both;
            grid.RecordsPerRow = 2;
            grid.PagerSettings.PagerType = PagerType.None;
            grid.Width = Unit.Pixel(1000);
            grid.DataSourceId = "Categories";
            grid.ConnectionString = ConnectionAccessOleDb;
            grid.RaisePostBackEvent("NewRecordClick!");
            Testpage.Controls.Add(grid);
        }

        private void SetupTestGrid(Grid grid)
        {
            grid.ID = "wggrid";
            grid.DefaultVisibility = Visibility.Both;
            grid.RecordsPerRow = 2;
            grid.PagerSettings.PagerType = PagerType.None;
            grid.Width = Unit.Pixel(1000);
            grid.DataSourceId = "Test";
            grid.ConnectionString = ConnectionSqlConnection;
            grid.RaisePostBackEvent("NewRecordClick!");
            Testpage.Controls.Add(grid);
        }

        #endregion Methods
    }
}