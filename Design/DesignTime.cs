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

using System.Text;
using System.Web;

namespace WebGrid
{
    internal class GridTrace
    {
        #region Fields

        internal readonly Grid m_Grid;

        internal StringBuilder TraceList = new StringBuilder();

        private bool? trace_grid;

        #endregion Fields

        #region Constructors

        internal GridTrace()
        {
        }

        internal GridTrace(Grid grid)
        {
            m_Grid = grid;
        }

        #endregion Constructors

        #region Properties

        internal string ClientID
        {
            get { return m_Grid.Page != null ? string.Format("{0}_{1}", m_Grid.Page.GetType().Name, m_Grid.Page.ClientID) : null; }
        }

        /// <summary>
        /// Trace grid
        /// </summary>
        internal bool IsTracing
        {
            get
            {
                if (trace_grid != null)
                    return (bool)trace_grid;
                if (trace_grid == null && (m_Grid.GridDesignMode || !Grid.GotHttpContext))
                    trace_grid = false;
                if (trace_grid == null)
                    trace_grid = (HttpContext.Current.Trace.IsEnabled ? true : false);
                return (bool)trace_grid;
            }
        }

        #endregion Properties

        #region Methods

        internal void Trace(string text)
        {
            if (!IsTracing)
                return;

            HttpContext.Current.Trace.Warn(text);
        }

        internal void Trace(string text, params object[] args)
        {
            if (!IsTracing)
                return;
            StringBuilder tmp = new StringBuilder();
            tmp.AppendFormat(text, args);
            Trace(tmp);
        }

        internal void Trace(StringBuilder text)
        {
            Trace(text.ToString());
        }

        #endregion Methods
    }
}

namespace WebGrid.Design
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Configuration;
    using System.Data;
    using System.Data.OleDb;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Web.UI;
    using Control = System.Web.UI.Control;
    using System.Web.UI.Design;
    using System.Web.UI.WebControls;
    using System.Windows.Forms;

    using Collections;
    using Config;
    using Enums;
    using Sentences;
    using Toolbar;
    using Util;

    /// <summary>
    /// This class contains methods and properties for WebGrid to render it at design time.
    /// </summary>
    public class WebGridDesignTime : ControlDesigner
    {
        #region Fields

        private const string HeaderPrefix = "Header";

        private DesignerActionListCollection al;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the action list collection for the control designer.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.ComponentModel.Design.DesignerActionListCollection"></see> that contains the <see cref="T:System.ComponentModel.Design.DesignerActionList"></see> items for the control designer.</returns>
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (al == null)
                {
                    al = new DesignerActionListCollection();

                    al.AddRange(base.ActionLists);

                    // Add a custom DesignerActionList
                    al.Add(new WebGridDesignerActionList(this));

                }
                return al;
            }
        }

        /// <summary>
        /// Gets the design-time verbs supported by the component that is associated with the designer.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.ComponentModel.Design.DesignerVerbCollection"></see> of <see cref="T:System.ComponentModel.Design.DesignerVerb"></see> objects, or null if no designer verbs are available. This default implementation always returns null.</returns>
        public override DesignerVerbCollection Verbs
        {
            get
            {
                try
                {
                    if (Component is Grid)
                    {
                        Grid grid = (Grid)Component;

                        DesignerVerbCollection actions = new DesignerVerbCollection();
                        if (!grid.Toolbars.Exists(toolbar => toolbar is ToolbarDetailBottom))
                            actions.Add(new DesignerVerb("Serialize the default Toolbar Detail-Bottom",
                                                         PopulateToolbarBottomDetailIntoGrid));
                        if (!grid.Toolbars.Exists(toolbar => toolbar is ToolbarGridBottom))
                            actions.Add(new DesignerVerb("Serialize the default Toolbar Grid-Bottom",
                                                         PopulateToolbarBottomGridIntoGrid));

                        foreach (Column column in grid.MasterTable.Columns)
                            if (column.IsCreatedByWebGrid)
                                actions.Add(
                                    new DesignerVerb(
                                        string.Format("Add field '{0}' ({1}) from data source.", column.ColumnId,
                                                      column.ColumnType), PopulateColumnInGrid));

                        return actions;
                    }
                    return base.Verbs;
                }
                catch (Exception ee)
                {
                    MessageBox.Show(string.Format("Error creating import fields. Error reason: {0}", ee),
                                    "WebGrid import",
                                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return base.Verbs;
                }
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Retrieves the HTML markup to display the control and populates the collection with the current control designer regions.
        /// </summary>
        /// <param name="regions"></param>
        /// <returns>
        /// The design-time HTML markup for the associated control, including all control designer regions.
        /// </returns>
        public override String GetDesignTimeHtml(DesignerRegionCollection regions)
        {
            try
            {

                Grid grid = (Grid) Component;
                grid.m_EventRanDoRender = false;
                GridConfig.m_Site = grid.Site;

                string renderDesignTime = GridConfig.Get("WGDesignTime");
                if (!string.IsNullOrEmpty(renderDesignTime) && renderDesignTime.Equals("disabled"))
                    return "WebGrid - The plug and play grid";
                if (renderDesignTime == "notavailable")
                    return
                        "WebGrid - The plug and play grid<br/>Design-Time support is not available for this project. Please contact support@webgrid.com for further help.";

                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);
                HtmlTextWriter writer = new HtmlTextWriter(sw);
                grid.IsDesignTimeRender = true;
                grid.RenderControl(writer);
                grid.IsDesignTimeRender = false;
                if (string.IsNullOrEmpty(sb.ToString()))
                    if (string.IsNullOrEmpty(grid.MasterGrid) == false)
                        return string.Format("{0} slavegrid for '{1}'", grid.ID, grid.MasterGrid);
                return sb.ToString();
            }
            catch (Exception ee)
            {
                throw new GridException(string.Format("Error generating design time environment {0}", ee), ee);
            }
        }

        /// <summary>
        /// Workaround for adding custom html tags into designtime.
        /// This is done because I'm unable to add the html tags using 'RootDesigner.AddControlToDocument' method.
        /// </summary>
        /// <returns></returns>
        public override string GetPersistenceContent()
        {
            string text = base.GetPersistenceContent();

            if (text.IndexOf("SkinID=\"wgToolbarDetailBottomreplace\">") > -1)
                text = text.Replace("SkinID=\"wgToolbarDetailBottomreplace\">", ">" + Grid.ToolbarDetailBottom);
            if (text.IndexOf("SkinID=\"wgToolbarGridBottomreplace\">") > -1)
                text = text.Replace("SkinID=\"wgToolbarGridBottomreplace\">", ">" + Grid.ToolbarGridBottom);
            return text;
        }

        /// <summary>
        /// Initializes the control designer and loads the specified component.
        /// </summary>
        /// <param name="component">The control being designed.</param>
        public override void Initialize(IComponent component)
        {
            // Throw an exception if the designer is attached
            // to a control for which it is not intended
            if (!(component is Grid))
            {
                throw new InvalidOperationException(
                    String.Format("{0} only supports controls derived from WebGrid.Grid", GetType().FullName)
                    );
            }
            base.Initialize(component);
        }

        /// <summary>
        /// Called when the associated control changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="ce">A <see cref="T:System.ComponentModel.Design.ComponentChangedEventArgs"></see> that contains the event data.</param>
        public override void OnComponentChanged(object sender, ComponentChangedEventArgs ce)
        {
            base.OnComponentChanged(sender, ce);
            UpdateDesignTimeHtml();
        }

        internal static void SaveGridState(DesignerTransaction dt, Grid grid, IComponentChangeService service)
        {
            service.OnComponentChanging(grid, null);
            service.OnComponentChanged(grid, null, null, null);
            dt.Commit();
        }

        /// <summary>
        /// Serialize column from data source to grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void PopulateColumnInGrid(object sender, EventArgs e)
        {
            try
            {
                int firstindex = sender.ToString().IndexOf("'");
                int lastindex = sender.ToString().IndexOf("'", firstindex + 1) - firstindex;
                string columnID = sender.ToString().Substring(firstindex + 1, lastindex - 1);

                if (firstindex == -1 || lastindex == -1)
                {
                    MessageBox.Show("WebGrid was unable to extract field ID from your data source.", "WebGrid import",
                                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                Grid grid = (Grid) Component;

                if (!grid.MasterTable.Columns.Contains(columnID))
                {
                    MessageBox.Show(String.Format("WebGrid could not find '{0}' in the data source.", columnID),
                                    "WebGrid import",
                                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                grid.MasterTable.Columns[columnID].IsCreatedByWebGrid = false;

                grid.IsDesignTimeRender = true;
                Column column = grid.MasterTable.Columns[columnID];
                IDesignerHost designerHost = (IDesignerHost) GetService(typeof (IDesignerHost));
                IComponentChangeService componentChangeService =
                    (IComponentChangeService) GetService(typeof (IComponentChangeService));

                DesignerTransaction designerTransaction =
                    designerHost.CreateTransaction("Add column for column collection");

                switch (column.ColumnType)
                {
                    case ColumnType.Checkbox:
                        column = (Checkbox) designerHost.CreateComponent(typeof (Checkbox));
                        break;
                    case ColumnType.DateTime:
                        column = (WebGrid.DateTime) designerHost.CreateComponent(typeof (DateTime));
                        break;
                    case ColumnType.Decimal:
                        column = (WebGrid.Decimal)designerHost.CreateComponent(typeof(Decimal));
                        break;
                    case ColumnType.File:
                        column = (WebGrid.File)designerHost.CreateComponent(typeof(File));
                        break;
                    case ColumnType.Foreignkey:
                        column = (Foreignkey) designerHost.CreateComponent(typeof (Foreignkey));
                        break;
                    case ColumnType.ManyToMany:
                        column = (ManyToMany) designerHost.CreateComponent(typeof (ManyToMany));
                        break;
                    case ColumnType.Number:
                        column = (Number) designerHost.CreateComponent(typeof (Number));
                        break;
                    default:
                        column = (Text) designerHost.CreateComponent(typeof (Text));
                        break;
                }

                column.CopyFrom(grid.MasterTable.Columns[columnID]);

                SaveGridState(designerTransaction, grid, componentChangeService);
                grid.IsDesignTimeRender = false;

                base.UpdateDesignTimeHtml();
                Invalidate();
                MessageBox.Show(
                    string.Format("Column '{0}' has successfully registered on the grid '{1}'.", column.Title,
                                  grid.Title),
                    "WebGrid import", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        internal void PopulateToolbarBottomDetailIntoGrid(object sender, EventArgs e)
        {
            PouplateToolbar(typeof(ToolbarDetailBottom));
        }

        internal void PopulateToolbarBottomGridIntoGrid(object sender, EventArgs e)
        {
            PouplateToolbar(typeof(ToolbarGridBottom));
        }

        /// <summary>
        /// Called by the design host when the user clicks the associated control at design time.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Web.UI.Design.DesignerRegionMouseEventArgs"></see> that specifies the location and, possibly, the control designer region that the user clicked.</param>
        protected override void OnClick(DesignerRegionMouseEventArgs e)
        {
            if (e.Region == null)
                return;
            Grid grid = (Grid)Component;

            // If the clicked region is not a header, return
            if (e.Region.Name.IndexOf(HeaderPrefix) != 0)
                return;

            // Switch the current view if required
            //only when the clicked region is different than the active region.
            if (e.Region.Name.Substring(HeaderPrefix.Length) == grid.CurrentDesignColumn.ToString()) return;
            //extract the index of the design region, and set the CurrentDesignTab index
            grid.CurrentDesignColumn = int.Parse(e.Region.Name.Substring(HeaderPrefix.Length));

            //then after update the design time HTML
            base.UpdateDesignTimeHtml();
        }

        private void PouplateToolbar(Type toolbar)
        {
            Grid grid = (Grid) Component;

            IDesignerHost designerHost = (IDesignerHost) GetService(typeof (IDesignerHost));
            IComponentChangeService componentChangeService =
                (IComponentChangeService) GetService(typeof (IComponentChangeService));

            DesignerTransaction designerTransaction =
                designerHost.CreateTransaction("Add toolbar for toolbars collection");

            ToolbarItem mytoolbar = (ToolbarItem) designerHost.CreateComponent(toolbar);

            grid.Toolbars.Add(mytoolbar);

            SaveGridState(designerTransaction, grid, componentChangeService);

            base.UpdateDesignTimeHtml();
            Invalidate();

            MessageBox.Show(
                string.Format("Default toolbar settings as successfully registered on the grid '{0}'.", grid.Title),
                "WebGrid import", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion Methods
    }

    internal class WebGridColumnCollectionConverter : CollectionConverter
    {
        #region Methods

        /// <summary>
        /// Converts the given value object to the specified destination type.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="culture">The culture to which value will be converted.</param>
        /// <param name="value">The <see cref="T:System.Object"></see> to convert. This parameter must inherit from <see cref="T:System.Collections.ICollection"></see>.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"></see> to convert the value to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">destinationType is null. </exception>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == typeof (string))
            {
                if (value is ICollection)
                {
                    return "Add, remove, and edit WebGrid columns.";
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion Methods
    }

    internal class WebGridColumnCollectionEditor : CollectionEditor
    {
        #region Fields

        private readonly Type[] types;

        #endregion Fields

        #region Constructors

        public WebGridColumnCollectionEditor(Type type)
            : base(type)
        {
            types = new[]
                        {
                            typeof (Text),
                            typeof (Number),
                            typeof (Decimal),
                            typeof (DateTime),
                            typeof (Checkbox),
                            typeof (Foreignkey),
                            typeof (File),
                            typeof (Image),
                            typeof (ManyToMany),
                            typeof (Chart),
                            typeof (SystemColumn),
                            typeof (GridColumn),
                            typeof (ColumnTemplate),
                            typeof (ToolTipColumn),
                        };
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Creates a new instance of the specified collection item type.
        /// </summary>
        /// <param name="itemType">The type of item to create.</param>
        /// <returns>A new instance of the specified object.</returns>
        protected override Object CreateInstance(Type itemType)
        {
            Grid grid = (Grid)Context.Instance;

            if (grid.Columns.GetIndex("columnID") > 0)
            {
                MessageBox.Show("An unedited column has already has already been registered in the grid.",
                                "WebGrid column registration",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

            Column x = (Column)base.CreateInstance(itemType);
             x.Grid = grid;
            x.ColumnId = "columnID";
            x.ColumnType = (ColumnType)Enum.Parse(typeof(ColumnType), x.ToString().Replace("WebGrid.",string.Empty), true);
            return x;
        }

        /// <summary>
        /// Gets the data types that this collection editor can contain.
        /// </summary>
        /// <returns>
        /// An array of data types that this collection can contain.
        /// </returns>
        protected override Type[] CreateNewItemTypes()
        {
            return types;
        }

        /// <summary>
        /// Retrieves the display text for the given list item.
        /// </summary>
        /// <param name="value">The list item for which to retrieve display text.</param>
        /// <returns>The display text for value.</returns>
        protected override string GetDisplayText(object value)
        {
            if (value is Column)
            {
                Grid grid = (Grid)Context.Instance;

                Column column = (Column)value;
                if (column.IsCreatedByWebGrid)
                    return null;
                return grid.MasterTable.Columns[column.ColumnId] != null ?
                    string.Format("{0} ({1})", grid.MasterTable.Columns[column.ColumnId].Title,
                    grid.MasterTable.Columns[column.ColumnId].ColumnType) : string.Format("{0} ({1})", column.Title, column.ColumnType);
            }
            return base.GetDisplayText(value);
        }

        #endregion Methods
    }

    internal class WebGridConnectionStringConverter : StringConverter
    {
        #region Fields

        private ArrayList values = new ArrayList();

        #endregion Fields

        #region Methods

        /// <summary>
        /// Gets a value indicating whether this converter can convert an object in the given source type to a string using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"></see> that represents the type you wish to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof (string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof (string) || base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the specified value object to a <see cref="T:System.String"></see> object.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"></see> to use.</param>
        /// <param name="value">The <see cref="T:System.Object"></see> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion could not be performed. </exception>
        public override object ConvertFrom(ITypeDescriptorContext context,
            CultureInfo culture, object value)
        {
            if (value != null && value.GetType() == typeof (string))
            {
                string _value = (string) value;
                if (values != null && !values.Contains(_value))
                    values.Add(_value);
                return _value;
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (value != null && value.GetType() == typeof (string))
            {
                string _value = (string) value;
                if (values != null && !values.Contains(_value))
                    values.Add(_value);
                return _value;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// Returns a collection of standard values for the data type this type converter is designed for when provided with a format context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context that can be used to extract additional information about the environment from which this converter is invoked. This parameter or properties of this parameter can be null.</param>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection"></see> that holds a standard set of valid values, or null if the data type does not support a standard set of values.
        /// </returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context == null)
                return null;
            try
            {
                StandardValuesCollection _values = GetValues(context);
                return _values;
            }
            catch (Exception ee)
            {
                MessageBox.Show(
                    string.Format("Error retrieving connection string values (Try restart your IDE for updating WebGrid reference):\n{0}", ee));
            }
            return base.GetStandardValues(context);
        }

        /// <summary>
        /// Returns whether the collection of standard values returned from <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"></see> is an exclusive list of possible values, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <returns>
        /// true if the <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection"></see> returned from <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"></see> is an exhaustive list of possible values; false if other values are possible.
        /// </returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        /// <summary>
        /// Returns whether this object supports a standard set of values that can be picked from a list, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <returns>
        /// true if <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"></see> should be called to find a common set of values the object supports; otherwise, false.
        /// </returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        private void AddConnectionStrings(IEnumerable<string> mdbrootfiles)
        {
            try
            {
                foreach (string _file in mdbrootfiles)
                {
                    if (_file.EndsWith(".mdb", StringComparison.OrdinalIgnoreCase))
                    {
                        if (string.IsNullOrEmpty(_file))
                            continue;
                        string file = string.Format("provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", _file);
                        if (values.Contains(file) == false)
                            values.Add(file);
                    }
                    else if (_file.EndsWith(".mdf", StringComparison.OrdinalIgnoreCase))
                    {
                        string file =
                            string.Format(
                                @"data source=.\SQLEXPRESS;Integrated Security=SSPI;AttachDBFilename={0};User Instance=True",
                                _file);
                        if (values.Contains(file) == false)
                            values.Add(file);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private StandardValuesCollection GetValues(ITypeDescriptorContext context)
        {
            try
            {
                Grid grid;
                if (context.Instance is Grid)
                    grid = (Grid) context.Instance;
                else if (context.Instance is WebGridDesignerActionList)
                    grid = (Grid) ((WebGridDesignerActionList) context.Instance).Component;
                else
                {
                    WebGridDesignerActionList tmp = (WebGridDesignerActionList) context.Instance;
                    grid = tmp.WebControl;
                }
                // Get the connectionStrings key,value pairs collection.
                ConnectionStringSettingsCollection connectionStrings =
                    ConfigurationManager.ConnectionStrings;
                IEnumerator connectionStringsEnum;
                int i;
                values = new ArrayList();
                if (connectionStrings != null)
                {
                    // Get the collection enumerator.
                    connectionStringsEnum = connectionStrings.GetEnumerator();

                    // Loop through the collection and
                    // display the connectionStrings key, value pairs.
                    i = 0;
                    while (connectionStringsEnum.MoveNext())
                    {
                        values.Add(connectionStrings[i].ConnectionString);
                        i += 1;
                    }
                }

                IWebApplication app = (IWebApplication) grid.Site.GetService(typeof (IWebApplication));
                if (app != null)
                {
                    Configuration config = app.OpenWebConfiguration(false);
                    if (config != null)
                    {
                        if (config.ConnectionStrings != null)
                        {
                            connectionStrings = config.ConnectionStrings.ConnectionStrings;
                            connectionStringsEnum = connectionStrings.GetEnumerator();

                            // Loop through the collection and
                            // display the connectionStrings key, value pairs.
                            i = 0;
                            while (connectionStringsEnum.MoveNext())
                            {
                                if (values.Contains(connectionStrings[i].ConnectionString) == false)
                                    values.Add(connectionStrings[i].ConnectionString);

                                i += 1;
                            }
                        }

                        // Find data source files in root directory of the project
                        string rootpath = app.RootProjectItem.PhysicalPath;

                        string[] mdbrootfiles =
                            Directory.GetFiles(rootpath, "*.mdb", SearchOption.TopDirectoryOnly);
                        string[] mdfrootfiles =
                            Directory.GetFiles(rootpath, "*.mdf", SearchOption.TopDirectoryOnly);
                        AddConnectionStrings(mdbrootfiles);
                        AddConnectionStrings(mdfrootfiles);

                        // Find data source files in "app_data" directory of the project
                        if (Directory.Exists(Path.Combine(rootpath, "App_Data")))
                        {
                            string[] mdbappdatafiles =
                                Directory.GetFiles(Path.Combine(rootpath, "App_Data"), "*.mdb",
                                                   SearchOption.TopDirectoryOnly);
                            string[] mdfappdatafiles =
                                Directory.GetFiles(Path.Combine(rootpath, "App_Data"), "*.mdf",
                                                   SearchOption.TopDirectoryOnly);
                            AddConnectionStrings(mdbappdatafiles);
                            AddConnectionStrings(mdfappdatafiles);
                        }

                        // Find data source files in "Bin" directory of the project
                        if (Directory.Exists(Path.Combine(rootpath, "Bin")))
                        {
                            string[] mdbbinfiles =
                                Directory.GetFiles(Path.Combine(rootpath, "Bin"), "*.mdb",
                                                   SearchOption.TopDirectoryOnly);
                            string[] mdfbinfiles =
                                Directory.GetFiles(Path.Combine(rootpath, "Bin"), "*.mdf",
                                                   SearchOption.TopDirectoryOnly);
                            AddConnectionStrings(mdbbinfiles);
                            AddConnectionStrings(mdfbinfiles);
                        }

                        if (grid.Parent != null && grid.Parent != null)
                        {
                            Control parentControl = grid.Parent;

                            foreach (Control control in parentControl.Controls)
                            {
                                if (control is AccessDataSource)
                                {

                                    string datafile = ((AccessDataSource) control).ConnectionString;
                                    if (string.IsNullOrEmpty(datafile) == false)
                                    {
                                        // string file = string.Format("provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", datafile);
                                        string file = datafile;
                                        if (values.Contains(file) == false)
                                            values.Add(file);
                                    }

                                }
                                else if (control is SqlDataSource)
                                {
                                    string connectionString = ((SqlDataSource) control).ConnectionString;

                                    if (string.IsNullOrEmpty(connectionString) == false &&
                                        values.Contains(connectionString) == false) values.Add(connectionString);
                                }
                            }
                        }
                    }
                }

                values.Sort();
                StandardValuesCollection _values = new StandardValuesCollection(values);
                return _values;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        #endregion Methods
    }

    internal class WebGridContextMenuCollectionConverter : CollectionConverter
    {
        #region Methods

        /// <summary>
        /// Converts the given value object to the specified destination type.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="culture">The culture to which value will be converted.</param>
        /// <param name="value">The <see cref="T:System.Object"></see> to convert. This parameter must inherit from <see cref="T:System.Collections.ICollection"></see>.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"></see> to convert the value to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">destinationType is null. </exception>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value is ICollection)
                    return "Add, remove, and edit WebGrid ContextMenu.";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion Methods
    }

    internal class WebGridGroupByColumnCollectionEditor : CollectionEditor
    {
        #region Fields

        private readonly Type[] types;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WebGridContextMenuCollectionEditor"/> class.
        /// </summary>
        /// <param name="type">The type of the collection for this editor to edit.</param>
        public WebGridGroupByColumnCollectionEditor(Type type)
            : base(type)
        {
            types = new[]
                        {
                            typeof (GroupingColumn)
                        };
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Creates a new instance of the specified collection item type.
        /// </summary>
        /// <param name="itemType">The type of item to create.</param>
        /// <returns>A new instance of the specified object.</returns>
        protected override Object CreateInstance(Type itemType)
        {
            if (itemType.BaseType == typeof(GroupingColumn))
            {
                GroupingColumn x = (GroupingColumn)base.CreateInstance(itemType);

                return x;
            }

            return base.CreateInstance(itemType);
        }

        protected override Type[] CreateNewItemTypes()
        {
            return types;
        }

        /// <summary>
        /// Retrieves the display text for the given list item.
        /// </summary>
        /// <param name="value">The list item for which to retrieve display text.</param>
        /// <returns>The display text for value.</returns>
        protected override string GetDisplayText(object value)
        {
            if (value is GroupingColumn) return "Grouping Column";
            return base.GetDisplayText(value);
        }

        #endregion Methods
    }


    internal class WebGridGroupByColumnCollectionConverter : CollectionConverter
    {
        #region Methods

        /// <summary>
        /// Converts the given value object to the specified destination type.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="culture">The culture to which value will be converted.</param>
        /// <param name="value">The <see cref="T:System.Object"></see> to convert. This parameter must inherit from <see cref="T:System.Collections.ICollection"></see>.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"></see> to convert the value to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">destinationType is null. </exception>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value is ICollection)
                    return "Add, remove, and edit WebGrid Grouping Columns";
            }
             return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion Methods
    }

    internal class WebGridContextMenuCollectionEditor : CollectionEditor
    {
        #region Fields

        private readonly Type[] types;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WebGridContextMenuCollectionEditor"/> class.
        /// </summary>
        /// <param name="type">The type of the collection for this editor to edit.</param>
        public WebGridContextMenuCollectionEditor(Type type)
            : base(type)
        {
            types = new[]
                        {
                            typeof (CustomMenuItem),
                            typeof (SystemMenuItem),
                        };
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Creates a new instance of the specified collection item type.
        /// </summary>
        /// <param name="itemType">The type of item to create.</param>
        /// <returns>A new instance of the specified object.</returns>
        protected override Object CreateInstance(Type itemType)
        {
            if (itemType.BaseType == typeof(ContextMenuItem))
            {
                ContextMenuItem x = (ContextMenuItem)base.CreateInstance(itemType);

                return x;
            }

            return base.CreateInstance(itemType);
        }

        protected override Type[] CreateNewItemTypes()
        {
            return types;
        }

        /// <summary>
        /// Retrieves the display text for the given list item.
        /// </summary>
        /// <param name="value">The list item for which to retrieve display text.</param>
        /// <returns>The display text for value.</returns>
        protected override string GetDisplayText(object value)
        {
            if (value is SystemMenuItem) return "System Menu Item";
            if (value is CustomMenuItem) return "Custom Menu Item";
            return base.GetDisplayText(value);
        }

        #endregion Methods
    }

    internal class WebGridControlBuilder : ControlBuilder
    {
        #region Methods

        public override Type GetChildControlType(String tagName, IDictionary attributes)
        {
            if (tagName.ToLowerInvariant() .IndexOf("columns") > -1)
            {

                return base.GetChildControlType(tagName, attributes);
            }
            throw new ApplicationException(tagName);
        }

        #endregion Methods
    }

    internal class WebGridDataSourceConverter : StringConverter
    {
        #region Fields

        private ArrayList values = new ArrayList();

        #endregion Fields

        #region Methods

        /// <summary>
        /// Gets a value indicating whether this converter can convert an object in the given source type to a string using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"></see> that represents the type you wish to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the specified value object to a <see cref="T:System.String"></see> object.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"></see> to use.</param>
        /// <param name="value">The <see cref="T:System.Object"></see> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion could not be performed. </exception>
        public override object ConvertFrom(ITypeDescriptorContext context,
            CultureInfo culture, object value)
        {
            if (value != null && value.GetType() == typeof(string))
            {
                string _value = (string)value;
                if (values != null && !values.Contains(_value))
                    values.Add(_value);

                return _value;
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (value != null && value.GetType() == typeof(string))
            {
                string _value = (string)value;
                if (values != null && !values.Contains(_value))
                    values.Add(_value);
                return _value;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// Returns a collection of standard values for the data type this type converter is designed for when provided with a format context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context that can be used to extract additional information about the environment from which this converter is invoked. This parameter or properties of this parameter can be null.</param>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection"></see> that holds a standard set of valid values, or null if the data type does not support a standard set of values.
        /// </returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context == null)
                return null;
            StandardValuesCollection _values = GetDataSourceObjects(context);
            try
            {
                return _values;
            }
            catch (Exception ee)
            {
                MessageBox.Show(
                    string.Format("Error retrieving data source values (Try restart your IDE for updating WebGrid reference):\n{0}", ee));
            }
            return base.GetStandardValues(context);
        }

        /// <summary>
        /// Returns whether the collection of standard values returned from <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"></see> is an exclusive list of possible values, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <returns>
        /// true if the <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection"></see> returned from <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"></see> is an exhaustive list of possible values; false if other values are possible.
        /// </returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        /// <summary>
        /// Returns whether this object supports a standard set of values that can be picked from a list, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <returns>
        /// true if <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"></see> should be called to find a common set of values the object supports; otherwise, false.
        /// </returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        private StandardValuesCollection GetDataSourceObjects(ITypeDescriptorContext context)
        {
            string connectionString = null;

            values = new ArrayList();
            Grid grid;
            try
            {
                if (context.Instance is Grid)
                {
                    grid = (Grid)context.Instance;
                    connectionString = grid.ConnectionString;
                }
                else if (context.Instance is WebGridDesignerActionList)
                {
                    connectionString = ((WebGridDesignerActionList)context.Instance).ConnectionString;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            try
            {
                if (context.Instance is WebGridDesignerActionList)
                {
                    grid = (Grid) ((WebGridDesignerActionList) context.Instance).Parent.Component;
                    if (grid.Page != null && grid.Page.Controls != null)

                        foreach (Control control in grid.Page.Controls)
                        {
                            if (control is AccessDataSource)
                                values.Add(string.Format("{0} (AccessDataSource)", control.ID));
                            else if (control is SqlDataSource)
                                values.Add(string.Format("{0} (SqlDataSource)", control.ID));
                            else if (control is XmlDataSource)
                                values.Add(string.Format("{0} (XmlDataSource)", control.ID));
                            else if (control is ObjectDataSource)
                                values.Add(string.Format("{0} (ObjectDataSource)", control.ID));
                        }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            if (!string.IsNullOrEmpty(connectionString))
            {

                DataBaseConnectionType databaseType = ConnectionType.FindConnectionType(connectionString);
                DataTable tables;
                DataTable views;

                switch (databaseType)
                {
                    case DataBaseConnectionType.OleDB:
                        {
                            try
                            {
                                OleDbConnection conn = new OleDbConnection(connectionString);
                                conn.Open();
                                tables = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables,
                                                                  new Object[] {null, null, null, "TABLE"});
                                if (conn.Provider.Equals("sqloledb", StringComparison.OrdinalIgnoreCase) == false)
                                {
                                    views = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Views, null);
                                    for (int i = 0; i < views.Rows.Count; i++)
                                    {
                                        values.Add(string.Format("{0} (view)", views.Rows[i].ItemArray[2]));
                                    }
                                    /*procedures = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Procedures, null);
                                    for (int i = 0; i < procedures.Rows.Count; i++)
                                        values.Add(string.Format("{0} (procedure)", procedures.Rows[i].ItemArray[2]));*/
                                }
                                for (int i = 0; i < tables.Rows.Count; i++)
                                    values.Add(string.Format("{0} (table)", tables.Rows[i].ItemArray[2]));

                                conn.Close();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                        break;
                    case DataBaseConnectionType.SqlConnection:
                        {
                            try
                            {
                                // Adding Tables
                                string sql =
                                    "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME";
                                SqlDataAdapter schemaDA = new SqlDataAdapter(sql, connectionString);

                                tables = new DataTable();

                                schemaDA.Fill(tables);

                                // Adding Views
                                sql =
                                    "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.Views WHERE TABLE_SCHEMA = 'INFORMATION_SCHEMA' ORDER BY TABLE_NAME;";

                                schemaDA = new SqlDataAdapter(sql, connectionString);

                                views = new DataTable();

                                schemaDA.Fill(views);
                                schemaDA.Dispose();

                                // Adding Procedures
                                sql =
                                    "SELECT SPECIFIC_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'PROCEDURE'";

                                schemaDA = new SqlDataAdapter(sql, connectionString);

                                DataTable procedures = new DataTable();

                                schemaDA.Fill(procedures);
                                schemaDA.Dispose();

                                for (int i = 0; i < tables.Rows.Count; i++)
                                {
                                    values.Add(string.Format("{0} (table)", tables.Rows[i].ItemArray[0]));
                                }
                                for (int i = 0; i < views.Rows.Count; i++)
                                    values.Add(views.Rows[i].ItemArray[0] + " (view)");

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);

                            }
                            /*for (int i = 0; i < procedures.Rows.Count; i++)
                                values.Add(procedures.Rows[i].ItemArray[0] + " (procedure)");*/
                        }
                        break;
                    default:
                        MessageBox.Show(
                            string.Format(
                                "Database connection string '{0}' type is not supported by WebGrid. Please contact support@webgrid.com for help.",
                                connectionString));
                        break;
                }
            }
            values.Sort();
            StandardValuesCollection _values = new StandardValuesCollection(values);
            return _values;
        }

        #endregion Methods
    }

    internal class WebGridFileImageSizeCollectionConverter : CollectionConverter
    {
        #region Methods

        /// <summary>
        /// Converts the given value object to the specified destination type.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="culture">The culture to which value will be converted.</param>
        /// <param name="value">The <see cref="T:System.Object"></see> to convert. This parameter must inherit from <see cref="T:System.Collections.ICollection"></see>.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"></see> to convert the value to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">destinationType is null. </exception>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value is ICollection)
                    return "Add, remove, and edit WebGrid images sizes (triggers on upload)";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion Methods
    }

    internal class WebGridFileImageSizeCollectionEditor : CollectionEditor
    {
        #region Fields

        private readonly Type[] types;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WebGridFileImageSizeCollectionEditor"/> class.
        /// </summary>
        /// <param name="type">The type of the collection for this editor to edit.</param>
        public WebGridFileImageSizeCollectionEditor(Type type)
            : base(type)
        {
            types = new[]
                        {
                            typeof (ImageSize)
                        };
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Creates a new instance of the specified collection item type.
        /// </summary>
        /// <param name="itemType">The type of item to create.</param>
        /// <returns>A new instance of the specified object.</returns>
        protected override Object CreateInstance(Type itemType)
        {
            if (itemType.BaseType == typeof(ImageSize))
            {
                ImageSize x = (ImageSize)base.CreateInstance(itemType);
                return x;
            }
            return base.CreateInstance(itemType);
        }

        protected override Type[] CreateNewItemTypes()
        {
            return types;
        }

        /// <summary>
        /// Retrieves the display text for the given list item.
        /// </summary>
        /// <param name="value">The list item for which to retrieve display text.</param>
        /// <returns>The display text for value.</returns>
        protected override string GetDisplayText(object value)
        {
            if (value is ImageSize) return "Image Size";
            return base.GetDisplayText(value);
        }

        #endregion Methods
    }

    internal class WebGridSystemMessageCollectionConverter : CollectionConverter
    {
        #region Methods

        /// <summary>
        /// Converts the given value object to the specified destination type.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="culture">The culture to which value will be converted.</param>
        /// <param name="value">The <see cref="T:System.Object"></see> to convert. This parameter must inherit from <see cref="T:System.Collections.ICollection"></see>.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"></see> to convert the value to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">destinationType is null. </exception>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == typeof (string))
            {
                if (value is ICollection)
                    return "Add, remove, and edit WebGrid system messages.";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion Methods
    }

    internal class WebGridSystemMessageCollectionEditor : CollectionEditor
    {
        #region Fields

        private readonly Type[] types;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WebGridSystemMessageCollectionEditor"/> class.
        /// </summary>
        /// <param name="type">The type of the collection for this editor to edit.</param>
        public WebGridSystemMessageCollectionEditor(Type type)
            : base(type)
        {
            types = new[]
                        {
                            typeof (NewSystemMessage),
                            typeof (OverrideSystemMessage),
                        };
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Creates a new instance of the specified collection item type.
        /// </summary>
        /// <param name="itemType">The type of item to create.</param>
        /// <returns>A new instance of the specified object.</returns>
        protected override Object CreateInstance(Type itemType)
        {
            if (itemType.BaseType == typeof (SystemMessageItem))
            {
                SystemMessageItem x = (SystemMessageItem) base.CreateInstance(itemType);
                return x;
            }
            return base.CreateInstance(itemType);
        }

        protected override Type[] CreateNewItemTypes()
        {
            return types;
        }

        /// <summary>
        /// Retrieves the display text for the given list item.
        /// </summary>
        /// <param name="value">The list item for which to retrieve display text.</param>
        /// <returns>The display text for value.</returns>
        protected override string GetDisplayText(object value)
        {
            return value is NewSystemMessage
                       ? "New System Message"
                       : (value is OverrideSystemMessage ? "Override System Message" : base.GetDisplayText(value));
        }

        #endregion Methods
    }

    internal class WebGridToolbarCollectionConverter : CollectionConverter
    {
        #region Methods

        /// <summary>
        /// Converts the given value object to the specified destination type.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="culture">The culture to which value will be converted.</param>
        /// <param name="value">The <see cref="T:System.Object"></see> to convert. This parameter must inherit from <see cref="T:System.Collections.ICollection"></see>.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"></see> to convert the value to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">destinationType is null. </exception>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value is ICollection)
                    return "Add, remove, and edit WebGrid toolbars.";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion Methods
    }

    internal class WebGridToolbarCollectionEditor : CollectionEditor
    {
        #region Fields

        private readonly Type[] types;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WebGridToolbarCollectionEditor"/> class.
        /// </summary>
        /// <param name="type">The type of the collection for this editor to edit.</param>
        public WebGridToolbarCollectionEditor(Type type)
            : base(type)
        {
            types = new[]
                        {
                            typeof (ToolbarDetailBottom),
                            typeof (ToolbarDetailTop),
                            typeof (ToolbarGridBottom),
                            typeof (ToolbarGridTop),
                        };
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Creates a new instance of the specified collection item type.
        /// </summary>
        /// <param name="itemType">The type of item to create.</param>
        /// <returns>A new instance of the specified object.</returns>
        protected override Object CreateInstance(Type itemType)
        {
            if (itemType.BaseType == typeof(ToolbarItem))
            {
                ToolbarItem x = (ToolbarItem)base.CreateInstance(itemType);

                return x;
            }
            return base.CreateInstance(itemType);
        }

        protected override Type[] CreateNewItemTypes()
        {
            return types;
        }

        /// <summary>
        /// Retrieves the display text for the given list item.
        /// </summary>
        /// <param name="value">The list item for which to retrieve display text.</param>
        /// <returns>The display text for value.</returns>
        protected override string GetDisplayText(object value)
        {
            if (value is ToolbarDetailBottom) return "Toolbar for Detail-Bottom";
            if (value is ToolbarDetailTop) return "Toolbar for Detail-Top";
            if (value is ToolbarGridBottom) return "Toolbar for Grid-Bottom";
            if (value is ToolbarGridTop) return "Toolbar for Grid-Top";
            return base.GetDisplayText(value);
        }

        #endregion Methods
    }

    internal class WebGridTreeParentSelector : StringConverter
    {
        #region Fields

        private ArrayList values = new ArrayList();

        #endregion Fields

        #region Methods

        /// <summary>
        /// Gets a value indicating whether this converter can convert an object in the given source type to a string using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"></see> that represents the type you wish to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof (string) || base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type"></see> that represents the type you want to convert to.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof (string) || base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the specified value object to a <see cref="T:System.String"></see> object.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"></see> to use.</param>
        /// <param name="value">The <see cref="T:System.Object"></see> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion could not be performed. </exception>
        public override object ConvertFrom(ITypeDescriptorContext context,
            CultureInfo culture, object value)
        {
            if (value != null && value.GetType() == typeof (string))
            {
                string _value = (string) value;
                if (values != null && !values.Contains(_value))
                    values.Add(_value);
                return _value;
            }
            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"></see>. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object"></see> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"></see> to convert the value parameter to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        /// <exception cref="T:System.ArgumentNullException">The destinationType parameter is null. </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (value != null && value.GetType() == typeof (string))
            {
                string _value = (string) value;
                if (values != null && !values.Contains(_value))
                    values.Add(_value);
                return _value;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// Returns a collection of standard values for the data type this type converter is designed for when provided with a format context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context that can be used to extract additional information about the environment from which this converter is invoked. This parameter or properties of this parameter can be null.</param>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection"></see> that holds a standard set of valid values, or null if the data type does not support a standard set of values.
        /// </returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context == null)
                return null;
            try
            {
                StandardValuesCollection _values = GetValues(context);
                return _values;
            }
            catch (Exception ee)
            {
                MessageBox.Show(
                    string.Format("Error retrieving column data (Try restart your IDE for updating WebGrid reference): \n{0}", ee));
            }
            return base.GetStandardValues(context);
        }

        /// <summary>
        /// Returns whether the collection of standard values returned from <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"></see> is an exclusive list of possible values, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <returns>
        /// true if the <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection"></see> returned from <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"></see> is an exhaustive list of possible values; false if other values are possible.
        /// </returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        /// <summary>
        /// Returns whether this object supports a standard set of values that can be picked from a list, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <returns>
        /// true if <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"></see> should be called to find a common set of values the object supports; otherwise, false.
        /// </returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        private StandardValuesCollection GetValues(ITypeDescriptorContext context)
        {
            try
            {
                values = new ArrayList();
                Grid grid;
                if (context.Instance is Grid)
                    grid = (Grid) context.Instance;
                else if (context.Instance is WebGridDesignerActionList)
                    grid = (Grid) ((WebGridDesignerActionList) context.Instance).Component;
                else if (context.Instance is Column)
                    grid = ((Column) context.Instance).Grid;
                else
                {
                    WebGridDesignerActionList tmp = (WebGridDesignerActionList) context.Instance;
                    grid = tmp.WebControl;
                    //   grid = (Grid)((WebGridDesignerActionList)context.Instance).Component;
                }

                // Get the connectionStrings key,value pairs collection.
                ColumnCollection gridColumns = grid.MasterTable.Columns;

                foreach (Column column in gridColumns)
                    values.Add(column.ColumnId);
                values.Sort();
                StandardValuesCollection _values = new StandardValuesCollection(values);
                return _values;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        #endregion Methods
    }
}