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

namespace WebGrid.Design
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Reflection;
    using System.Windows.Forms;

    using WebGrid.Enums;

    /// <summary>
    /// Provides the class for types that define a list of items used to create
    ///     a smart tag panel for WebGrid DataGrid.
    /// </summary>
    public class WebGridDesignerActionList : DesignerActionList
    {
        #region Fields

        private readonly WebGridDesignTime m_Parent;

        #endregion Fields

        #region Constructors

        internal WebGridDesignerActionList(WebGridDesignTime parent)
            : base(parent.Component)
        {
            m_Parent = parent;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [allow delete].
        /// </summary>
        /// <value><c>true</c> if [allow delete]; otherwise, <c>false</c>.</value>
        public bool AllowDelete
        {
            get { return WebControl.AllowDelete; }
            set { SetProperty("AllowDelete", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow edit].
        /// </summary>
        /// <value><c>true</c> if [allow edit]; otherwise, <c>false</c>.</value>
        public bool AllowEdit
        {
            get { return WebControl.AllowEdit; }
            set { SetProperty("AllowEdit", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow new].
        /// </summary>
        /// <value><c>true</c> if [allow new]; otherwise, <c>false</c>.</value>
        public bool AllowNew
        {
            get { return WebControl.AllowNew; }
            set { SetProperty("AllowNew", value); }
        }

        /// <summary>
        /// Gets or sets the column templates.
        /// </summary>
        /// <value>The column templates.</value>
        public ColumnTemplates ColumnTemplates
        {
            get { return ColumnTemplates.None; }
            set
            {
                switch (value)
                {
                    case ColumnTemplates.None:
                        break;
                    default:
                        {
                            IDesignerHost designerHost = (IDesignerHost) GetService(typeof (IDesignerHost));
                            IComponentChangeService componentChangeService =
                                (IComponentChangeService) GetService(typeof (IComponentChangeService));

                            DesignerTransaction designerTransaction =
                                designerHost.CreateTransaction("Add column for column collection");
                            Grid grid = (Grid) Component;

                            try
                            {
                                WebGrid.GridTemplates.AddGridColumn(ref grid, ref designerHost, ref componentChangeService,
                                                                    ref designerTransaction, value);
                                MessageBox.Show(
                                    String.Format("Column have successfully been added to '{0}'", grid.Title),
                                    "WebGrid template", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception ee)
                            {
                                MessageBox.Show(string.Format("Grid column failed to be added. Error Reason:{0}", ee),
                                                "WebGrid template",
                                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        [TypeConverter(typeof (WebGridConnectionStringConverter))]
        public string ConnectionString
        {
            get { return WebControl.ConnectionString; }
            set { SetProperty("ConnectionString", value); }
        }

        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        /// <value>The data source.</value>
        [TypeConverter(typeof(WebGridDataSourceConverter))]
        public string DataSourceId
        {
            get
            {
                string datasource = WebControl.DataSourceId;
                return string.IsNullOrEmpty(datasource) == false && datasource.Equals(Grid.DATASOURCEID_NULL) ? null : datasource;
            }

            set
            {
                string _value = value;
                if (!string.IsNullOrEmpty(_value) && _value.IndexOf("(") > -1)
                {
                    _value = _value.Substring(0, _value.IndexOf("(")).Trim();
                }
                SetProperty("DataSourceId", _value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="WebGridDesignerActionList"/> is debug.
        /// </summary>
        /// <value><c>true</c> if debug; otherwise, <c>false</c>.</value>
        public bool Debug
        {
            get { return WebControl.Debug; }
            set { SetProperty("Debug", value); }
        }

        /// <summary>
        /// Gets or sets the default visibility.
        /// </summary>
        /// <value>The default visibility.</value>
        public Visibility DefaultVisibility
        {
            get { return WebControl.DefaultVisibility; }
            set { SetProperty("DefaultVisibility", value); }
        }

        /// <summary>
        /// Gets or sets the grid template.
        /// </summary>
        /// <value>The grid templates.</value>
        public GridTemplate GridTemplates
        {
            get { return GridTemplate.None; }
            set
            {
                switch (value)
                {
                    case GridTemplate.None:
                        break;
                    default:
                        {
                            DialogResult result =
                                MessageBox.Show(
                                    "Are you sure want to configure the grid and columns with selected grid template ?",
                                    "WebGrid template", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                            if (result == DialogResult.No)
                                return;

                            IDesignerHost designerHost = (IDesignerHost) GetService(typeof (IDesignerHost));
                            IComponentChangeService componentChangeService =
                                (IComponentChangeService) GetService(typeof (IComponentChangeService));

                            DesignerTransaction designerTransaction =
                                designerHost.CreateTransaction("Configure grid with template");
                            Grid grid = (Grid) Component;

                            try
                            {
                                WebGrid.GridTemplates.ConfigureGrid(ref grid, ref designerHost, ref componentChangeService,
                                                                    ref designerTransaction, value);
                                MessageBox.Show("The grid has been successfully configured.", "WebGrid template",
                                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception ee)
                            {
                                MessageBox.Show(string.Format("Grid failed to be configured. Error Reason:{0}", ee),
                                                "WebGrid template",
                                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Gets the m_parent.
        /// </summary>
        /// <value>The m_parent.</value>
        public WebGridDesignTime Parent
        {
            get { return m_Parent; }
        }

        internal Grid WebControl
        {
            get { return (Grid) Component; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Returns the collection of <see cref="T:System.ComponentModel.Design.DesignerActionItem"></see> objects contained in the list.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.Design.DesignerActionItem"></see> array that contains the items in this list.
        /// </returns>
        public override DesignerActionItemCollection GetSortedActionItems()
        {
            // Create list to store designer action items
            DesignerActionItemCollection actionItems = new DesignerActionItemCollection
                                                           {
                                                               new DesignerActionHeaderItem(
                                                                   "Welcome to WebGrid setup wizard", "Wizard"),
                                                               new DesignerActionTextItem(
                                                                   @"Step 1: Fill inn Connection string and data source below to retrieve data from the data source.",
                                                                   "Wizard"),
                                                               new DesignerActionTextItem(
                                                                   @"Step 2: Right click on WebGrid in design mode to insert default column-settings from data source field.",
                                                                   "Wizard"),
                                                               new DesignerActionTextItem(
                                                                   @"Step 3: Right click on WebGrid in design mode and select 'Properties' to customize grid/columns properties.",
                                                                   "Wizard"),
                                                               new DesignerActionTextItem(
                                                                   @"Step 4: Customize your web application using WebGrid events. (found in same window as step 3)",
                                                                   "Wizard"),
                                                               new DesignerActionHeaderItem("Data Source",
                                                                                            "DataSourceId"),
                                                               new DesignerActionPropertyItem(
                                                                   "ConnectionString",
                                                                   "Connection string (You can set this property in Web.Config)",
                                                                   "DataSourceId",
                                                                   GetDescription(WebControl, "ConnectionString")),
                                                               new DesignerActionPropertyItem(
                                                                   "DataSourceId",
                                                                   "Data source",
                                                                   "DataSourceId",
                                                                   GetDescription(WebControl, "DataSourceId")),
                                                               new DesignerActionTextItem(
                                                                   @"Use 'DataSource' method to access OleDBAdapter, DataSet, Xml files, and DataTable data sources.",
                                                                   "DataSourceId"),
                                                               new DesignerActionHeaderItem("Basic grid options",
                                                                                            "GridOptions"),
                                                               new DesignerActionTextItem(
                                                                   @"Basic grid options often used during the development process.",
                                                                   "GridOptions"),
                                                               new DesignerActionPropertyItem(
                                                                   "DefaultVisibility",
                                                                   "Default column visibility",
                                                                   "GridOptions",
                                                                   GetDescription(WebControl, "DefaultVisibility")),
                                                               new DesignerActionPropertyItem(
                                                                   "AllowNew",
                                                                   "Allow new records",
                                                                   "GridOptions",
                                                                   GetDescription(WebControl, "AllowNew")),
                                                               new DesignerActionPropertyItem(
                                                                   "AllowEdit",
                                                                   "Allow record editing (for default)",
                                                                   "GridOptions",
                                                                   GetDescription(WebControl, "AllowEdit")),
                                                               new DesignerActionPropertyItem(
                                                                   "AllowDelete",
                                                                   "Allow record delete (for default)",
                                                                   "GridOptions",
                                                                   GetDescription(WebControl, "AllowDelete")),
                                                               new DesignerActionPropertyItem(
                                                                   "Debug",
                                                                   "Render debug information",
                                                                   "GridOptions",
                                                                   GetDescription(WebControl, "Debug")),
                                                               new DesignerActionHeaderItem(
                                                                   "Grid and Column templates", "Templates"),
                                                               new DesignerActionTextItem(
                                                                   @"Templates are fully functional beta templates. Please send any feedback to support@webgrid.com if you have suggestions.",
                                                                   "Templates"),
                                                               new DesignerActionPropertyItem(
                                                                   "GridTemplates",
                                                                   "Configure grid with template ",
                                                                   "Templates",
                                                                   "Configure the grid with one of the templates available."),
                                                               new DesignerActionPropertyItem(
                                                                   "ColumnTemplates",
                                                                   "Add column template ",
                                                                   "Templates",
                                                                   "Add column to the grid with one of the templates available.")
                                                           };

            // Add Appearance category header text

            //// Add Appearance category descriptive label

            // Add Appearance category header text

            return actionItems;
        }

        private static string GetDescription(object source, string propertyName)
        {
            PropertyInfo property = source.GetType().GetProperty(propertyName);
            DescriptionAttribute attribute =
                (DescriptionAttribute) property.GetCustomAttributes(typeof (DescriptionAttribute), false)[0];
            return attribute == null ? null : attribute.Description;
        }

        // Helper method to safely set a components property
        private void SetProperty(string propertyName, object value)
        {
            // Get property
            PropertyDescriptor property = TypeDescriptor.GetProperties(WebControl)[propertyName];

            // Set property value

            property.SetValue(WebControl, value);
        }

        #endregion Methods
    }
}