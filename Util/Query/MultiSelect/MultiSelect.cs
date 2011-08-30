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

namespace WebGrid.Util.MultiSelect
{
    using System;
    using System.Drawing;
    using System.Text;
    using System.Web;

    /// <summary>
    /// This class contain properties, methods, and scripts that are used by
    /// </summary>
    internal sealed class SelectableRows
    {
        #region Fields

        /// <summary>
        /// Javascripts required for the use of multiselect feature.
        /// </summary>
        /// <returns>The javascript</returns>
        internal static string JavaScript = 
            @"
        ";

        #endregion Fields

        #region Methods

        /// <summary>
        /// Creates a checkbox in the header of WebGrid.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="identifier">a unique identifier for the header checkbox</param>
        /// <returns>The html for this checkbox</returns>
        internal static string GetHeaderCheckbox(Grid grid, string identifier)
        {
            return
                string.Format(
                    "<input type=\"checkbox\" id=\"{0}_{1}\" name=\"{2}_{3}\" onclick=\"SelectAllCheckboxes(this,'{4}');\" {5}/>",
                    grid.ClientID, identifier, grid.ClientID, identifier, grid.ClientID, IsInCookie(grid, identifier));
        }

        /// <summary>
        /// Creates a checkbox foreach row in grid view.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="identifier">a unique identifier for the item checkbox</param>
        /// <param name="argument">The argument.</param>
        /// <param name="color">The color.</param>
        /// <returns>The html for this checkbox</returns>
        internal static string GetItemCheckbox(Grid grid, object identifier, string argument, string color)
        {
            return
                string.Format(
                    "<input type=\"checkbox\" id=\"{0}_{1}\"  {2}  name=\"{3}_{4}\" onclick=\"WGCheckboxhighlight(this,'{5}');\" {6}/>",
                    grid.ClientID, identifier, argument, grid.ClientID, identifier, color, IsInCookie(grid, identifier));
        }

        internal static void GetSelectedRows(Grid grid)
        {
            if (HttpContext.Current.Request[grid.ClientID + "checkboxes"] == null)
                return;

            StringBuilder oldRowsBuilder = new StringBuilder();

            if (grid.GetState("SelectedRows") != null)
                oldRowsBuilder.Append(grid.GetState("SelectedRows").ToString());
            StringBuilder selectedRowsBuilder = new StringBuilder();

            foreach (string request in HttpContext.Current.Request[grid.ClientID + "checkboxes"].Split('!'))
            {
                if (string.IsNullOrEmpty(request) == false)
                    oldRowsBuilder = new StringBuilder(oldRowsBuilder.ToString().Replace(request + "!", string.Empty));

                if (Grid.GotHttpContext && HttpContext.Current.Request[string.Format("{0}_{1}", grid.ClientID, request)] != null)
                    selectedRowsBuilder.AppendFormat("{0}!", request);
            }
            grid.State("SelectedRows", oldRowsBuilder + selectedRowsBuilder.ToString());
        }

        internal static void GetUnSelectedRows(Grid grid)
        {
            if (grid.SelectableRows == false)
                return;
        }

        /// <summary>
        /// Reads cookie for multiselect feature.
        /// If cookie exists and the cookie has a key value like the row identifier for WebGrid
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="identifier">a unique identifier for a multiselect row.</param>
        /// <returns>
        /// Returns true if row is selected in the cookie, else false.
        /// </returns>
        internal static string IsInCookie(Grid grid, object identifier)
        {
            if (grid.GetState("SelectedRows") != null &&
                grid.GetState("SelectedRows").ToString().IndexOf(identifier + "!") > -1)
            {
                return grid.SelectRowColor != Color.Empty
                           ? string.Format("checked=\"checked\" onload=\"WGCheckboxhighlight(this,'{0}');\"",
                                           Grid.ColorToHtml(grid.SelectRowColor))
                           : "checked=\"checked\"";
            }
            return string.Empty;
        }

        /// <summary>
        /// ItemCheckboxChanged is an event used by the ItemCheckBox ( GetItemCheckbox ) to
        /// submit changes to the cookie, when post back is raised.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        internal static void ItemCheckboxChanged(object sender, EventArgs e)
        {
            //	CheckBox chkTemp = ((CheckBox)sender);
            //chkTemp.page.Trace("ItemCheckboxChanged event called");
            /*	WebGrid.Grid Grid;
                Grid = ((DataGridItem)chkTemp.Parent.Parent);
                if ((chkTemp.Checked))
                {
                    dgi.BackColor = grdEmployees.SelectedItemStyle.BackColor;
                    dgi.ForeColor = grdEmployees.SelectedItemStyle.ForeColor;
                }
                else
                {
                    dgi.BackColor = grdEmployees.ItemStyle.BackColor;
                    dgi.ForeColor = grdEmployees.ItemStyle.ForeColor;

                } */
        }

        #endregion Methods
    }
}