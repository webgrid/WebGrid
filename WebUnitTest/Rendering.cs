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
using WebGrid.Data;
using WebGrid.Enums;
using WebGridUnitTest;

namespace WebGridUnitTest
{
    [TestFixture]
    public class Rendering : UnitTestCommon
    {
        [Test]
        public void GetEmployeesAccessLayOutTest()
        {
            Grid grid = new Grid();
            grid.DataSourceId = "Employees";
            grid.ID = "test";
            grid.RecordsPerRow = 2;
            grid.PagerType = PagerType.None;
            grid.Width = Unit.Pixel(1000);
            grid.Height = Unit.Pixel(1500);
            PlaceHolder helptext = new PlaceHolder();
            Label label = new Label();
            label.Text = "This is an test..";
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
            Grid grid = new Grid();

            grid.ID = "test";
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            grid.RenderControl(gridwriter);
        }

        [Test]
        public void RenderGridWithRows()
        {
            Grid grid = new Grid();

            grid.ID = "test";

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
    }
}