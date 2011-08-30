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
using NUnit.Framework;
using WebGrid;
using WebGrid.Enums;
using WebGridUnitTest;

namespace WebGridUnitTest
{
    [TestFixture]
    public class GridPart : UnitTestCommon
    {
        [Test]
        public void GetAccessOleDb()
        {
            Grid grid = new Grid();
            grid.Sql = "SELECT * FROM Employees";
            grid.ID = "test";
            grid.Page = Testpage;
            grid.PageLength = 2;
            grid.ConnectionString = ConnectionAccessOleDb;

            WebGrid.GridPart part1 = new WebGrid.GridPart();
            part1.Page = Testpage;
            part1.MasterGrid = grid.ID;
            part1.GridPartType = WebGrid.Enums.GridPart.NewRecord;
            part1.ID = "part1";
            part1.Html = "New Record Button";
            Testpage.Controls.Add(part1);


            WebGrid.GridPart part2 = new WebGrid.GridPart();
            part1.Page = Testpage;
            part1.MasterGrid = grid.ID;
            part1.GridPartType = WebGrid.Enums.GridPart.Pager;
            part1.ID = "part2";
            Testpage.Controls.Add(part2);

            WebGrid.GridPart part3 = new WebGrid.GridPart();
            part1.Page = Testpage;
            part1.MasterGrid = grid.ID;
            part1.GridPartType = WebGrid.Enums.GridPart.SearchField;
            part1.ID = "part3";
            Testpage.Controls.Add(part3);


            WebGrid.GridPart part4 = new WebGrid.GridPart();
            part1.Page = Testpage;
            part1.MasterGrid = grid.ID;
            part1.GridPartType = WebGrid.Enums.GridPart.UpdateRecords;
            part1.ID = "part4";
            Testpage.Controls.Add(part4);

            Testpage.Controls.Add(grid);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter gridwriter = new HtmlTextWriter(sw);
            Testpage.RenderControl(gridwriter);
            Console.WriteLine(sb.ToString());
        }
    }
}