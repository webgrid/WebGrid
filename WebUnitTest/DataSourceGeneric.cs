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


using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Web.UI;
using NUnit.Framework;
using WebGrid;

namespace WebGridUnitTest 
{
    [TestFixture]
    public class DataSourceGeneric : UnitTestCommon
    {
        [Test]
        public void LoadGridWithEmployeesGenericList()
        {
            NorthwindEmployeeData employeedata = new NorthwindEmployeeData();

            List<NorthwindEmployee> employees = employeedata.GetAllEmployeesList("", 0, 100);

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

            Dictionary<int,NorthwindEmployee> employees = employeedata.GetAllEmployeesDictionary("", 0, 100);
            
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
        public void LoadGridWithNamesSortedList()
        {
           SortedList<string, string> names = new SortedList<string,string>();
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
    }
}
