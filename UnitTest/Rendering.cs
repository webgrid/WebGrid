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
    using System.IO;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using NUnit.Framework;

    using WebGrid;
    using WebGrid.Data;
    using WebGrid.Enums;
    public class WebGridEx : Grid
    {
        
    }
    [TestFixture]
    public class Rendering : UnitTestCommon
    {
        #region Methods


        [Test]
        public void GetEmployeesAccessWebGridEx()
        {
            WebGridEx grid = new WebGridEx
            {
                DataSourceId = "Employees",
                ID = "test",
                RecordsPerRow = 2,
                Width = Unit.Pixel(1000),
                Height = Unit.Pixel(1500)
            };
            grid.PagerSettings.PagerType = PagerType.None;
            PlaceHolder helptext = new PlaceHolder();
            Label label = new Label { Text = "This is an test.." };
            helptext.Controls.Add(label);
            grid.HelpText = helptext;
            grid.ConnectionString = ConnectionAccessOleDb;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
            Assert.AreNotSame(sb.ToString().IndexOf("is an test.."), -1);
        }

        [Test]
        public void GetEmployeesAccessLayOutTest()
        {
            Grid grid = new Grid
                            {
                                DataSourceId = "Employees",
                                ID = "test",
                                RecordsPerRow = 2,
                                Width = Unit.Pixel(1000),
                                Height = Unit.Pixel(1500)
                            };
            grid.PagerSettings.PagerType = PagerType.None;
            PlaceHolder helptext = new PlaceHolder();
            Label label = new Label {Text = "This is an test.."};
            helptext.Controls.Add(label);
            grid.HelpText = helptext;
            grid.ConnectionString = ConnectionAccessOleDb;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
            Assert.Greater(grid.MasterTable.Rows.Count, 1);
            Assert.AreNotSame(sb.ToString().IndexOf("is an test.."), -1);
        }

        [Test]
        public void RenderEmptyGrid()
        {
            Grid grid = new Grid {ID = "test"};

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
        }

        [Test]
        public void RenderGridWithRows()
        {
            Grid grid = new Grid {ID = "test"};

            Row newrow = new Row( grid.MasterTable);
            newrow["title"].Value = "title 1";
            newrow["Description"].Value = @"description 1";
            newrow["sampleurl"].Value = "sample url 1";
            newrow["PrimaryColumn"].Value = "1";
            grid.AddRow(newrow);

            newrow = new Row( grid.MasterTable);
            newrow["title"].Value = "title 2";
            newrow["Description"].Value = @"description 2";
            newrow["sampleurl"].Value = "sample url 2";
            newrow["PrimaryColumn"].Value = "2";
            grid.AddRow(newrow);

            newrow = new Row( grid.MasterTable);
            newrow["title"].Value = "title 3";
            newrow["Description"].Value = @"description 3";
            newrow["sampleurl"].Value = "sample url 3";
            newrow["PrimaryColumn"].Value = "3";
            grid.AddRow(newrow);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
        }

        #endregion Methods
    }
}