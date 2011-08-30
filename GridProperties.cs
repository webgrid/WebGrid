/*
Copyright ©  Olav Christian Botterli.

Dual licensed under the MIT or GPL Version 2 licenses.

Date: 30.08.2011, Norway.

http://www.webgrid.com
*/


/*
Copyright ©  Olav Christian Botterli.

Dual licensed under the MIT or GPL Version 2 licenses.

Date: 30.08.2011, Norway.

http://www.webgrid.com
*/


using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Web;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Xml;
using WebGrid.Collections;

using WebGrid.Config;
using WebGrid.Design;
using WebGrid.Enums;
using WebGrid.Toolbar;
using WebGrid.Util;
using System.Globalization;
using MenuItemCollection = WebGrid.Collections.MenuItemCollection;
using System.Collections.Generic;

namespace WebGrid
{
    public partial class Grid
    {

        internal XmlDocument WebConfigAppSettings { get; set; }
        internal string WebConfigFile { get; set; }
        internal System.DateTime NextConfigCheck { get; set; }


        #region CodeBehind

        /// <summary>
        /// MasterTable for WebGrid. The MasterTable is same datasource as the property <see cref="DataSourceId"/>
        /// </summary>
        [
            Description(@"Default datatable for WebGrid. The MasterTable is same data source as the property table."),
            Browsable(false),
        ]
        public Data.Table MasterTable
        {
            get { return m_MasterTable; }
            set { m_MasterTable = value; }
        }

        /// <summary>
        /// A collection of all the tables used with the grid (data table, schemas, foreignkeys etc.)
        /// If you activate cache data structures, some of these data will be put in cache. (For improved performance)
        /// </summary>
        [
            Description(
                @"A collection of all the tables used with the grid (data table, schemas, foreignkeys etc.) If you activate cache data structures, some of these data will be put in cache. (For improved performance)"
                ),
            Browsable(false),
        ]
        internal TableCollection Tables
        {
            get { return m_Tables;}

        }

        /// <summary>
        /// Sets or gets the clientObjectId for this WebGrid. Default is <see cref="System.Web.UI.Control.ID"/>.
         /// </summary>
        public string ClientObjectId { get; set; }

       

        /// <summary>
        /// Gets column elements at specified by string.
        /// </summary>
        [
            Description(@"Gets column elements at specified by string."),
            Browsable(false),
        ]
        public Column this[string columnName]
        {
            get { return MasterTable.Columns[columnName]; }
            set { MasterTable.Columns[columnName] = value; }
        }


        /// <summary>
        /// Gets column elements at specified by index value.
        /// </summary>
        [
            Description(@"Gets column elements at specified by index value."),
            Browsable(false),
        ]
        public Column this[int index]
        {
            get { return MasterTable.Columns[index]; }
            set { MasterTable.Columns[index] = value; }
        }


        private readonly TableCollection m_Tables = new TableCollection();

        private Data.Table m_MasterTable;

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeMasterTable()
        {
            return false;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeTables()
        {
            return false;
        }

        #endregion
        #region Design



       
        /// <summary>
        /// Display any html if the grid is triggered by the "HideIfEmpty" property
        /// </summary>
        /// <value>The html</value>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [ Category("Appearance")]
        [ DefaultValue(null)]
        [Description(@"Render any HTML before the grid start rendering. Applies to both grid and detail view.")]
       [NotifyParentProperty(true)]
        public PlaceHolder PreGridTemplate
        {
            get { return m_PreGridTemplate ?? (m_PreGridTemplate = new PlaceHolder()); }
            set
            {
                m_PreGridTemplate = value;

            }
        }


        /// <summary>
        /// Gets or sets the name of the list of data that the data-bound control binds to, in cases where the data source contains more than one distinct list of data items.
        /// </summary>
        public string DataMember
        {
            get
            {
                return MasterTable != null ? MasterTable.DataMember : null;
            }
            set { if (MasterTable != null) MasterTable.DataMember = value; }
        }

        private PlaceHolder m_PreGridTemplate;
         /// <summary>
        /// Sets or gets the colour that should be shown when the content of column is changed from the web interface.
        /// Default color is LightSteelBlue.
        /// </summary>
        /// <value>The column changed colour.</value>
        [Bindable(true),
         Category("Row"),
         DefaultValue(typeof(Color), "LightSteelBlue"),
         Description(
             @"Sets or gets the colour that should be shown when the content of column is changed from the web interface. Default color is LightSteelBlue."
             )
        ]
        public Color ColumnChangedColour
        {
            get { return m_ColumnChangedColour; }
            set { m_ColumnChangedColour = value; }
        }


        /// <summary>
        /// Culture for WebGrid
        /// </summary>
        /// <value>Culture for WebGrid  </value>
        [Bindable(true),
         Category("Misc"),
         Description(
             @"Sets or gets current culture settings for WebGrid. Default is Thread.CurrentThread.CurrentCulture"
             )
        ]
        public CultureInfo Culture
        {
            get { return m_GridCulture ?? (m_GridCulture = System.Threading.Thread.CurrentThread.CurrentCulture); }
            set { m_GridCulture = value; }
        }

        private CultureInfo m_GridCulture;

        /// <summary>
        /// Sets or gets the heightEditableContent of the grid.
        /// </summary>
        /// <value>The heightEditableContent of the grid.</value>
        [Bindable(true),
         Category("Appearance"),
         Description(@"Sets or gets the height of the grid.")
        ]
        public override Unit Height
        {
            get { return m_Height; }
            set { m_Height = value; }
        }

        /// <summary>
        /// Sets or gets the absolute path for images used in the Grid.
        /// Web.Config key for this property is "WG" + property name. Default image path is "./images/"
        /// </summary>
        /// <value>The image path.</value>
        [Bindable(true),
         Category("Data"),
         Description(
             @"Sets or gets the absolute path for images used in the grid. Web.Config key for this property is ""WG"" + property name. Default image path is ""./images"""
             )
        ]
        public string ImagePath
        {
            get
            {
                if (m_ImagePath == null)
                    return GridConfig.Get("wgimagePath", "./images/");
                if (m_ImagePath.EndsWith("/") == false)
                    m_ImagePath += "/";
                return m_ImagePath;
            }
            set
            {
                m_ImagePath = value;
                if (m_ImagePath.EndsWith("/") == false)
                    m_ImagePath += "/";
            }
        }

        /// <summary>
        /// Sets or gets the colour that should be shown when you focus on one specific column.
        /// </summary>
        /// <value>The input highlight colour.</value>
        [Bindable(true),
         Category("Row"),
         DefaultValue(typeof(Color), "Empty"),
         Description(@"Sets or gets the colour that should be shown when you focus on one specific column.")
        ]
        public Color InputHighLight
        {
            get { return m_InputHighLight; }
            set { m_InputHighLight = value; }
        }

        /// <summary>
        /// Sets or gets the maximum number of records allowed before "New record" button is removed. This property is disabled by the value 0 or less.
        /// </summary>
        /// <remarks>
        /// MaximumRecords lowest value used is 1, if less the property is disabled.
        /// </remarks>
        /// <value>The records per row.</value>
        [Bindable(true),
         Category("Row"),
         DefaultValue(0),
         Description(
             @"Sets or gets the maximum number of records allowed before ""New record"" button is removed. This property is disabled by the value 0 or less."
             )
        ]
        public int MaximumRecords
        {
            get { return m_MaximumRecords; }
            set
            {
                m_MaximumRecords = value < 1 ? 1 : value;
            }
        }

        /// <summary>
        /// Indicates whether the all columns should be non breakable by default. Default is false.
        /// This property can be overridden by the column property "NonBreaking"
        /// Web.Config key for this property is "WG" + property name.
        /// </summary>
        /// <value><c>true</c> if [non breaking columns]; otherwise, <c>false</c>.</value>
        [Bindable(true),
         Category("Behavior"),
         Description(
             @"Indicates whether the all columns should be non breakable by default. Default is false. This property can be overridden by the column property ""NonBreaking"" Web.Config key for this property is ""WG"" + property name"
             )
        ]
        public bool NonBreakingColumns
        {
            get
            {
                if (m_NonBreakingColumns == null)
                {
                    m_NonBreakingColumns = GridConfig.Get("WGNonBreakingColumns", false);
                }
                return TristateBoolType.ToBool(m_NonBreakingColumns, false);
            }
            set { m_NonBreakingColumns = value; }
        }

        /// <summary>
        /// Indicates whether the grid's headers should be non breakable. Default is false.
        /// Web.Config key for this property is "WG" + property name.
        /// </summary>
        /// <value><c>true</c> if [non breaking headers]; otherwise, <c>false</c>.</value>
        [Bindable(true),
         Category("Behavior"),
         Description(
             @"Indicates whether the grid's headers should be non breakable. Default is true. Web.Config key for this property is ""WG"" + property name"
             )
        ]
        public bool NonBreakingHeaders
        {
            get
            {
                return m_NonBreakingHeaders == null ? GridConfig.Get("WGNonBreakingHeaders", true) : TristateBoolType.ToBool(m_NonBreakingHeaders, true);
            }
            set { m_NonBreakingHeaders = value; }
        }



        /// <summary>
        /// Sets or gets how many records we should display per row on the web page. Example by setting RecordsPerRow = 4 it will render 4 database records per row on your web page.
        /// </summary>
        /// <remarks>
        /// Minimum rows per record is one.
        /// </remarks>
        /// <value>The records per row.</value>
        [Bindable(true),
         Category("Row"),
         DefaultValue(1),
         Description(
             @"Sets or gets how many records we should display per row on the web page. Example by setting RecordsPerRow = 4 it will render 4 database records per row on your web page."
             )
        ]
        public int RecordsPerRow
        {
            get { return m_RecordsPerRow; }
            set
            {
                m_RecordsPerRow = value < 1 ? 1 : value;
            }
        }

        /// <summary>
        /// Sets or gets the colour that should be shown at a row when the mouse hovers over it.
        /// </summary>
        /// <value>The row highlight colour.</value>
        [Bindable(true),
         Category("Row"),
         DefaultValue(typeof(Color), "Empty"),
         Description(@"Sets or gets the colour that should be shown at a row when the mouse hovers over it.")
        ]
        public Color RowHighLight
        {
            get { return m_RowHighLight; }
            set { m_RowHighLight = value; }
        }

        /// <summary>
        /// Sets or gets the colour that should be shown at a row when you use WebGrid.Enums.SystemColumn.SelectColumn (Applies to WebGrid.SystemColumn)
        /// Default colour is "WhiteSmoke"
        /// </summary>
        /// <value>The colour of a selected row.</value>
        [Bindable(true),
         Category("Row"),
         DefaultValue(typeof(Color), "Silver"),
         Description(@"Sets or gets the colour that should be shown when you select on one specific column.")
        ]
        public Color SelectRowColor
        {
            get { return m_SelectRowColor; }
            set { m_SelectRowColor = value; }
        }

        /// <summary>
        /// Sets or gets the predefined styles provided with WebGrid.
        /// Web.Config key for this property is "WG" + property name.
        /// </summary>
        /// <value>The name of the style (predefined styles)</value>
        [Bindable(true),
         Category("Layout"),
         Browsable(true),
         Description(
             @"Sets or gets the predefined themes provided with WebGrid. You can also set this property in Web.Config by the key ""WGSkinType""."
             )
        ]
        public SkinType SkinType
        {
            get
            {

                if (m_SkinType == null)
                {
                    string type = GridConfig.Get("WGSkinType", "smoothness");
                    m_SkinType = Enum.IsDefined(typeof(SkinType), type)
                                          ? (SkinType)Enum.Parse(typeof(SkinType), type, true)
                                          : SkinType.Smoothness;

                }
                return m_SkinType != null ? (SkinType)m_SkinType : SkinType.Smoothness;
            }
            set
            {
                m_SkinType = value;
            }
        }

        /// <summary>
        /// Sets or gets the widthEditableArea of the grid. If WidthEditableColumn is not set and the Grid has a <see cref="WebGrid.Grid.MasterGrid"/> it will inherit widthEditableArea.
        /// Web.Config key for this property is "WG" + property name.
        /// </summary>
        /// <value>The widthEditableArea of the grid.</value>
        [Bindable(true),
         Category("Appearance"),
         Description(@"Sets or gets the width of the grid.")
        ]
        public override Unit Width
        {
            get
            {
                if (m_Width != Unit.Empty)
                    return m_Width;
                if (m_Width == Unit.Empty && MasterWebGrid == null)
                    return Unit.Parse(GridConfig.Get("WGWidth", "750px"));
                if (m_Width == Unit.Empty && MasterWebGrid != null)
                    return MasterWebGrid.Width;
                return m_Width;
            }
            set { m_Width = value; }
        }

        /// <summary>
        /// Converts color to html
        /// </summary>
        /// <param name="color">System's color code.</param>
        /// <returns>Html for the color</returns>
        internal static string ColorToHtml(Color color)
        {
            return ColorTranslator.ToHtml(color);

        }

       

        internal bool IsDesignTime
        {
            get { return DesignMode; 
             }
        }

        internal bool IsDesignTimeRender;

        private Color m_ColumnChangedColour = Color.LightSteelBlue;

        private Unit m_Height = Unit.Empty;


        private string m_ImagePath;

        private Color m_InputHighLight = Color.Empty;

        private int m_MaximumRecords;


        private bool? m_NonBreakingColumns;


        private bool? m_NonBreakingHeaders;


        private int m_RecordsPerRow = 1;

        private Color m_RowHighLight = Color.Empty; 


        private Color m_SelectRowColor = Color.Silver;


        private SkinType? m_SkinType;


        private Unit m_Width;


        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeColumnChangedColour()
        {
            return m_ColumnChangedColour != Color.Empty && m_ColumnChangedColour != Color.LightSteelBlue;
        }

        
        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeSkinType()
        {
            switch (m_SkinType)
            {
                case SkinType.Smoothness:
                     return false;
            }
            return true;
        }


        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeHeight()
        {
            return m_Height != Unit.Empty;
        }


        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeImagePath()
        {
            return m_ImagePath != null;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeInputHighLight()
        {
            return m_InputHighLight != Color.Empty;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeNonBreakingColumns()
        {
            return m_NonBreakingColumns != null && m_NonBreakingColumns != false;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeNonBreakingHeaders()
        {
            return m_NonBreakingHeaders != null;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeRowHighLight()
        {
            return m_RowHighLight != Color.Empty;
        }



        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeWidth()
        {
            return m_Width != Unit.Empty;
        }

        #endregion

        /// <summary>
        /// A standard OleDB connection string for Access and SQLClient connection for MS SQL.
        /// Example how a connection string looks:
        /// "Data Source='DataSorce';Initial Catalog='Databasename';User ColumnId='userID';Password='password'";
        /// Web.Config key for this property is "WG" + property name.
        /// </summary>
        /// <value>The connection string for the grid.</value>
        [Bindable(true),
         Category("Data"),
         DefaultValue("Data Source='DataServer';Initial Catalog='Catalog';User ID='myuser';Password='mypassword';"),
         SettingsBindable(true),
            //    EditorAttribute(typeof(ConnectionStringEditor),
            //    typeof(UITypeEditor)),
         Description(
             @"A standard database connection string. Example how aconnection stringlooks: ""Data Source='DataSorce';Initial Catalog='Databasename';User ID='userID';Password='password'"" Web.Config key for this property is ""WG"" + property name"
             )
        ]
        [TypeConverter(typeof(WebGridConnectionStringConverter))]
        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(m_ConnectionString))
                    m_ConnectionString = GridConfig.Get("WGConnectionString", null as string);
                return m_ConnectionString;
            }
            set
            {
                try
                {
                    if (string.IsNullOrEmpty(value) == false)
                    {
                        m_ConnectionString = value;
                        DatabaseConnectionType = DataBaseConnectionType.Auto;
                        MasterTable.ConnectionString = null;
                        MasterTable.DataBaseInterface = null;

                        #region Designtime connectionString

                        if (Site != null && DesignMode)
                        {
                            
                            IWebApplication app = (IWebApplication)Site.GetService(typeof(IWebApplication));
                            if (app != null)
                            {
                                Configuration config = app.OpenWebConfiguration(false);
                                if (config != null && Properties.Settings.Default["ConnectionSet"] == null)
                                {
                                    DialogResult result = config.AppSettings.Settings["WGConnectionString"] == null
                                                                                   ? MessageBox.Show(
                                                                                         "No default connection string was detected for WebGrid, do you want to use this one as default ? (Close the web configuration file before applying, you must also re-open this dialog window)",
                                                                                         "Create connection string",
                                                                                         MessageBoxButtons.YesNoCancel,
                                                                                         MessageBoxIcon.Information)
                                                                                   : MessageBox.Show(
                                                                                         "Do you want to use this connection string as default for WebGrid controls ? (Close the web configuration file before applying, you must also re-open this dialog window)",
                                                                                         "Create connection string",
                                                                                         MessageBoxButtons.YesNoCancel,
                                                                                         MessageBoxIcon.Information);

                                    Properties.Settings.Default["ConnectionSet"] = "yes";
                                    switch (result)
                                    {
                                        case DialogResult.No:
                                            m_ShouldSerializeConnectionString = true;
                                            m_ActiveConnectionString = null;
                                             return;
                                        default:
                                            m_ShouldSerializeConnectionString = false;
                                            if (result == DialogResult.Yes)
                                            {
                                                config.AppSettings.Settings.Remove("WGConnectionString");
                                                config.AppSettings.Settings.Add("WGConnectionString", value);
                                                config.Save(ConfigurationSaveMode.Minimal, true);
#pragma warning disable 168
                                                string x = ConnectionString;
#pragma warning restore 168
                                            }
                                            m_ConnectionString = null;
                                            break;
                                    }
                                }
                            }
                        }

                        #endregion
                    }
                    else
                        m_ConnectionString = null;
                    m_ActiveConnectionString = null;
                }
                catch (Exception e)
                {
                    throw new GridException("Error in connection string", e);
                }
            }
        }

        /// <summary>
        /// Sets or gets the data interface for the grid. Default is Automatic.
        /// </summary>
        /// <value>The type of the database connection.</value>
        [Bindable(true),
         Category("Data"),
         Browsable(false),
         DefaultValue(typeof(DataBaseConnectionType), "Auto"),
         Description(@"Sets or gets the data interface for the grid. Default is Automatic.")
        ]
        public DataBaseConnectionType DatabaseConnectionType
        {
            get
            {
                if (m_DbConnectionType == DataBaseConnectionType.Auto)
                {
                    m_DbConnectionType = ConnectionType.FindConnectionType(ConnectionString);
                }
                return m_DbConnectionType;
            }
            set { m_DbConnectionType = value; }
        }


        /// <summary>
        /// Sets or gets a data source (data-table, view, or stored procedure, or write your own SQL sentence) you want to use from your data-source.
        /// With WebGrid you can also access other databases from your data-source (granted you have access for this)
        /// Example is:
        /// [NorthWind].dbo.[Customers]
        /// In the example above you are accessing the NorthWind database and the Customers table.
        /// </summary>
        /// <value>The table name (name of the table in your data source)</value>
        [Bindable(true),
         Category("Data"),
         Description(
             @"Sets or gets the datatable (datatable, view, or stored procedure) you want to use from your data-source. With WebGrid you can also access other databases from your data-source (granted you have access for this) Example is: [NorthWind].dbo.[Customers] In the example above you are accessing the NorthWind database and the Customers table."
             )
        ]
        [TypeConverter(typeof(WebGridDataSourceConverter))]
        [RefreshProperties(RefreshProperties.All)]
        public string DataSourceId
        {
            get
            {
                if (DesignMode && string.IsNullOrEmpty(MasterTable.DataSourceId) == false &&
                    MasterTable.DataSourceId.Equals(DATASOURCEID_NULL))
                    return string.Empty;
                return MasterTable.DataSourceId;
            }

            set { MasterTable.DataSourceId = string.IsNullOrEmpty(value) ? null : value; }
        }

        /// <summary>
        /// Sets or gets the ParentID (Parent identifier) for a treestructure.
        /// Remember: Setting this property makes the grid render as a treestructure in grid view.
        /// </summary>
        /// <value>The tree parent ColumnId.</value>
        [Bindable(true), Category("Behavior"), DefaultValue(null), TypeConverter(typeof(WebGridTreeParentSelector)), Description(
            @"Sets or gets the ParentID (Parent identifier) for a treestructure. NOTE: Setting this property makes the grid render as a treestructure in grid view."
            )]
        public string TreeParentId { get; set; }



        // 23.10.2004, jorn
        /// <summary>
        /// Determines whether the specified data type is bindable to WebGrid.
        /// </summary>
        /// <param name="type">A System.ForeignkeyType object that contains the data type to test.</param>
        /// <returns> true if the specified data type is bindable to WebGrid; otherwise, false.  </returns>
        internal static bool IsBindable(Type type)
        {
            return type.Equals(typeof(DataTable));

        }

        internal HtmlForm FindForm
        {
            get
            {
                return Page == null ? null : Page.Form;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [grid design mode].
        /// </summary>
        /// <value><c>true</c> if [grid design mode]; otherwise, <c>false</c>.</value>
        internal bool GridDesignMode
        {
            get { return DesignMode; }
        }

 

        /// <summary>
        /// Sorted columns by priority. (This is created after the grid has been rendered.)
        /// </summary>
        internal int[] m_VisibleColumnsByIndex;

        internal int[] VisibleSortedColumns
        {

            get { return m_VisibleColumnsByIndex ?? (m_VisibleColumnsByIndex = MasterTable.Columns.VisibleColumnsByIndex); }
//            RenderingMethods.GetSortedColumns(this)); }
        }


        /// <summary>
        /// Gets a value indicating whether [valid page hash code]. Hash code is used to detect changes on web pages (aspx/ascx). 
        /// If a page is registered and changes to it is done it will return false. 
        /// </summary>
        /// <value><c>true</c> if [valid page hash code]; otherwise, <c>false</c>.</value>
        [Description("Indicating whether [valid page hash code]. Hash code is used to detect changes on web pages. This is used for cache of web page settings")]
        [Category("Misc")]
        public bool ValidPageHashCode
        {
            get
            {
                if (HttpRuntime.Cache.Get(Trace.ClientID + ClientID) == null)
                    return true;
                int registeredHashcode = (int)HttpRuntime.Cache.Get(Trace.ClientID + ClientID);
                int pageHash = Page.GetType().GetHashCode();
                return registeredHashcode == pageHash;
            }
        }
       
        
        /// <summary>
        /// Returns true if there are any columns in the grid that can be edited in grid view.
        /// </summary>
        /// <value><c>true</c> if [grid view edit]; otherwise, <c>false</c>.</value>
        [Bindable(true),
         Browsable(false),
         Description(@"Returns true if there are any columns in the grid that can be edited in grid view.")
        ]
        public bool AllowEditInGrid
        {
            // 15.10.2004, jorn
            get
            {
                if (DisplayView == DisplayView.Detail)
                    return false;

                if (m_StoredAllowEditInGrid != null)
                    return TristateBoolType.ToBool(m_StoredAllowEditInGrid, false);
                for (int i = 0; i < MasterTable.Rows.Count; i++)
                    for (int ii = 0; ii < MasterTable.Rows[i].Columns.Count; ii++)
                        if (MasterTable.Rows[i].Columns[ii].AllowEditInGrid &&
                            MasterTable.Rows[i].Columns[ii].AllowEdit &&
                            (MasterTable.Rows[i].Columns[ii].Visibility == Visibility.Both ||
                             MasterTable.Rows[i].Columns[ii].Visibility == Visibility.Grid))
                        {
                            if (Equals(m_EventRanpreRender, true))
                                m_StoredAllowEditInGrid = true;
                            return true;
                        }


                if (Equals(m_EventRanpreRender, true))
                    m_StoredAllowEditInGrid = false;
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the call back loading to be displayed when WebGrid is loading data on callback (ajax).
        /// </summary>
        /// <value>The call back loading text.</value>
        /// <remarks>This feature works only if PreCallBackFunction and PostCallBackFunction method are not in use.</remarks>
        public string CallBackLoadingText
        {
            get { return m_CallBackLoadingText; }
            set { m_CallBackLoadingText = value; }
        }

        /// <summary>
        /// Sets or gets if the grid should only render menu and title if it has an active (visibility) slave grid.
        /// </summary>
        /// <value><c>true</c> if [hide if slave grid]; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// Returns false if the grid has no slave grids.
        /// </remarks>
        [Bindable(true),
         Category("Menu"),
         Description(
             @"Sets or gets if the grid should only render menu and title if it has an active (visible) slave grid.")
        ]
        public bool CollapseIfSlaveGrid
        {
            get
            {
                if (m_CollapseIfSlaveGrid == true && SlaveGrid.Count == 0 || DisplayView == DisplayView.Grid)
                    return false;
                return TristateBoolType.ToBool(m_CollapseIfSlaveGrid, false);
            }
            set
            {
                if (value)
                    DisplaySlaveGridMenu = true;
                m_CollapseIfSlaveGrid = value;
            }
        }
        
        /// <summary>
        /// Get or set the design time active tab.
        /// </summary>
        [Browsable(false),
         PersistenceMode(PersistenceMode.InnerProperty),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        internal int CurrentDesignColumn { get; set; }

        

        /// <summary>
        /// Make a distinct (unique) sql query. Default is false.
        /// </summary>
        /// <value><c>true</c> if distinct; otherwise, <c>false</c>.</value>
        [
         Category("Data"),
         DefaultValue(false),
         Description(@"Make a distinct (unique) sql query. Default is false.")
        ]
        public bool Distinct
        {
            get { return m_Distinct; }
            set
            {
                m_Distinct = value;
            }
        }

        private string m_DialogTitle;
        /// <summary>
        /// Title for Grid user interface dialogs. Default is Grid Title
        /// </summary>
        [
         Category("Appearance"),
         DefaultValue(false),
         Description(@"Title for Grid user interface dialogs.")
        ]
        public string DialogTitle
        {
            get
            {
                return m_DialogTitle ?? (Title ?? string.Empty);
            }
            set
            {
                m_DialogTitle = value;
            }
        }

        /// <summary>
        /// Gets the real connection string.
        /// </summary>
        /// <value>The real connection string.</value>
        internal string ActiveConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(m_ActiveConnectionString))
                {
                    m_ActiveConnectionString = Equals(DesignMode, true) ? Util.ConnectionString.FindConnectionString(ConnectionString, this) : Util.ConnectionString.FindConnectionString(ConnectionString);
                    DatabaseConnectionType = ConnectionType.FindConnectionType(m_ActiveConnectionString);
                }
                return m_ActiveConnectionString;
            }
        }

        /// <summary>
        /// Indicates if post back-data should be retrieved from columns. (Activated by "UpdateRowsClick", "RecordUpdateClick", and "UpdateRowClick" events)
        /// </summary>
        public bool GetColumnsPostBackData
        {
            get { return TristateBoolType.ToBool(m_GetColumnsPostBackData, true); }
            set
            {
                if (value)
                    MasterTable.m_GotPostBackData = false;
                m_GetColumnsPostBackData = value;
            }
        }

        /// <summary>
        /// Gets all the selected rows in the grid (applies to WebGrid.Enums.SystemColumn.SelectColumn )
        /// </summary>
        /// <value>The get selected rows.</value>
        /// <returns>
        /// An array with all selected rows in the grid, and null is returned if no rows are selected.
        /// </returns>
        public string[] GetGetSelectedRows()
        {
            string selectedRows = null;
            if (GetState("SelectedRows") == null)
                return null;
            foreach (string row in GetState("SelectedRows").ToString().Split('!'))
                if (row.StartsWith("heading") == false && row.Trim() != string.Empty)
                {
                    if (selectedRows == null)
                        selectedRows = row;
                    else
                        selectedRows += string.Format("!{0}", row);
                }
            return selectedRows == null ? null : selectedRows.Split('!');
        }

        /// <summary>
        /// Gets or sets the help text content for the grid help icon. By setting this you will 
        /// override default help description.
        /// </summary>
        /// <value>The html</value>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Description("Help text (Displayed in grid help icon)")]
        [NotifyParentProperty(true)]
        public PlaceHolder HelpText
        {
            get {
                return m_helptext_public; 
            }
            set
            {
                m_helptext_public = value;
               
            }
        }

        /// <summary>
        /// Display any html if the grid is triggered by the "HideIfEmpty" property
        /// </summary>
        /// <value>The html</value>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Description("Help text (Displayed in grid help icon)")]
        [NotifyParentProperty(true)]
        public PlaceHolder EmptyDataTemplate
        {
            get { return m_EmptyDataTemplate; }
            set
            {
                m_EmptyDataTemplate = value;

            }
        }

       

        /// <summary>
        /// Display any custom html when doing asynchronous loading against the web server.
        /// </summary>
        /// <value>The html</value>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Description("Help text (Displayed in grid help icon)")]
        [NotifyParentProperty(true)]
        public PlaceHolder AjaxLoaderTemplate
        {
            get { return m_AjaxLoaderTemplate; }
            set
            {
                m_AjaxLoaderTemplate = value;

            }
        }

        

        /// <summary>
        /// Indicates whether the Grid should turn invisible if no records.
        /// As Default it will hide the grid when there are 0 rows and does not allowing new records.
        /// </summary>
        /// <value><c>true</c> if [hide if empty]; otherwise, <c>false</c>.</value>
        [Bindable(true),
         Category("Behavior"),
         Description(
             @"Indicates whether the grid should turn invisible if no records. As Default it will hide the grid when there are 0 rows and does not allowing new records."
             )
        ]
        public bool HideIfEmptyGrid
        {
            get
            {
                if (string.IsNullOrEmpty(Search) == false)
                    return false;
                if (null == m_HideIfemptyGrid)
                {
                    if (AllowNew == false && DisplayView == DisplayView.Grid && MasterTable.Rows.Count == 0 && MasterGrid != null)
                        return true;
                }
                return TristateBoolType.ToBool(m_HideIfemptyGrid, false);
            }
            set { m_HideIfemptyGrid = value; }
        }

        /// <summary>
        /// Sets or gets the color for this inactive menu items for this grid. Web.Config key for this property is ""WG"" + property name
        /// </summary>
        /// <value>The menu colour.</value>
        /// <remarks>
        /// If this property is undefined and a <see cref="SkinType">SkinType</see> is set it will automatically inherit color from this styletype.
        /// </remarks>
        [Bindable(true),
         Category("Menu"),
         Description(
             @"Sets or gets the color for this active menu items for this grid. Web.Config key for this property is ""WG"" + property name"
             )
        ]
        internal static Color MenuActiveColor
        {
            get
            {

                return Color.Empty;
            }
        }

        /// <summary>
        /// Sets or gets the color for this inactive menu items for this grid. Web.Config key for this property is ""WG"" + property name
        /// </summary>
        /// <value>The menu colour.</value>
        /// <remarks>
        /// If this property is undefined and a <see cref="SkinType">StyleType</see> is set it will automatically inherit color from this styletype.
        /// </remarks>
        [Bindable(true),
         Category("Menu"),
         Description(
             @"Sets or gets the color for this inactive menu items for this grid. Web.Config key for this property is ""WG"" + property name"
             )
        ]
        internal static Color MenuInactiveColor
        {
            get
            {
                return Color.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the pager for the grid
        /// </summary>
        /// <value>The pager settings.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Description("Gets or sets the pager for the grid")]
        [NotifyParentProperty(true)]
        [MergableProperty(false)]
        [Category("Navigation")]
        public Pager PagerSettings
        {
            get
            {
                if (m_PagerSettings.m_PagerGrid == null)
                    m_PagerSettings.m_PagerGrid = this;
                return m_PagerSettings;
            }
            set
            {
                m_PagerSettings = value;
            }
        }


        /// <summary>
        /// Sets all the selected rows in the grid (applies to WebGrid.Enums.SystemColumn.SelectColumn )
        /// </summary>
        /// <value>The selected rows and seperated by '!'</value>
        /// <returns>
        /// An array with all selected rows in the grid, and null if no rows are selected (system default is null).
        /// </returns>
        public string SetSelectedRows
        {
            set { State("SelectedRows", value); }
        }

       

        /// <summary>
        /// Gets or sets the SQL, this property overrides 'DataSourceID' property.
        /// </summary>
        /// <value>The SQL.</value>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Description("Sql statement for this grid object")]
        [NotifyParentProperty(true)]
        public string Sql
        {
            get { return m_Sql; }
            set
            {
                if (string.IsNullOrEmpty(DataSourceId))
                    DataSourceId = DATASOURCEID_NULL;


                m_Displaysearchbox = Position.Hide;
                m_Sql = value;
            }
        }

       
        private ClientNotification m_ClientNotification;
        /// <summary>
        /// Client Notification raises unobtrusive messages within the browser, similar to the way that OS X's Growl Framework works
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Description("Client Notification raises unobtrusive messages within the browser, similar to the way that OS X's Growl Framework works")]
        [NotifyParentProperty(true)]
        [MergableProperty(false)]
        [Category("Misc")]
        public ClientNotification ClientNotification
        {
            get
            {
                return m_ClientNotification;
            }
            set
            {
                m_ClientNotification = value;
            }
        }

        private GridFormMail m_GridFormMail;
        /// <summary>
        /// Settings for sending webgrid as an e-mail.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Description("Settings for sending webgrid as an e-mail.")]
        [NotifyParentProperty(true)]
        [MergableProperty(false)]
        [Category("Misc")]
         public GridFormMail MailForm
        {
            get { 
                return m_GridFormMail; 
            }
            set
            {
                m_GridFormMail = value;
            }
        }

        private MaskedInput m_MaskedInput;
        /// <summary>
        /// grid settings for masked input.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Description("grid settings for masked input.")]
        [NotifyParentProperty(true)]
        [MergableProperty(false)]
        [Category("Misc")]
        public MaskedInput MaskedInput
        {
            get
            {
                return m_MaskedInput;
            }
            set
            {
                m_MaskedInput = value;
            }
        }


        private PopupExtender m_PopupExtender;
        /// <summary>
        /// Popup extender is an ASP.NET extender that can be attached to WebGrid control in order to open a popup window that displays additional content. 
        /// This popup window will be interactive and will  be within an  external web page, so it will be able to perform complex server-based processing (including postbacks) 
        /// without affecting the rest of the page. 
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Description("Popup extender is an ASP.NET extender that can be attached to WebGrid control in order to open a popup window that displays additional content.")]
        [NotifyParentProperty(true)]
        [MergableProperty(false)]
        [Category("Misc")]
        public PopupExtender PopupExtender
        {
            get {
                return m_PopupExtender; 
            }
            set
            {
                m_PopupExtender = value;
            }
        }


        private ScriptSettings m_GridScripts;
        /// <summary>
        /// Disable Javascript resources being generated by WebGrid.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Description("Disable Javascript resources being generated by WebGrid.")]
        [NotifyParentProperty(true)]
        [MergableProperty(false)]
        [Category("Misc")]
        public ScriptSettings Scripts
        {
            get
            {
                return m_GridScripts;
            }
            set
            {
                m_GridScripts = value;
            }
        }


          /// <summary>
        /// A collection of customized toolbar that can be set to override the default toolbars used by WebGrid.
        /// Right click on the grid and select 'serialize toolbar' to see default toolbars.
        /// </summary>
        /// <remarks>Right click on the grid and select 'serialize toolbar' to see default toolbars.
        /// </remarks>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Description("Add or edit grid toolbars.")]
        [Browsable(true)]
        [TypeConverter(typeof(WebGridToolbarCollectionConverter))]
        [Editor(typeof(WebGridToolbarCollectionEditor), typeof(UITypeEditor))]

        public ToolbarCollection Toolbars
        {
            get { return m_CustomToolbars ?? (m_CustomToolbars = new ToolbarCollection()); }
        }



        /// <summary>
        /// A collection of customized toolbar that can be set to override the default toolbars used by WebGrid.
        /// Right click on the grid and select 'serialize toolbar' to see default toolbars.
        /// </summary>
        /// <remarks>Right click on the grid and select 'serialize toolbar' to see default toolbars.
        /// </remarks>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Description("Add or edit default column grouping.")]
        [Browsable(true)]
        [TypeConverter(typeof(WebGridGroupByColumnCollectionConverter))]
        [Editor(typeof(WebGridGroupByColumnCollectionEditor), typeof(UITypeEditor))]

        public GroupColumnCollection GroupByColumns
        {
            get { return m_GroupColumns ?? (m_GroupColumns = new GroupColumnCollection()); }
        }


        private GroupColumnCollection m_GroupColumns;



        /// <summary>
        /// A collection of customized toolbar that can be set to override the default toolbars used by WebGrid.
        /// Right click on the grid and select 'serialize toolbar' to see default toolbars.
        /// </summary>
        /// <remarks>Right click on the grid and select 'serialize toolbar' to see default toolbars.
        /// </remarks>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Description("Add or edit grid contextmenu.")]
        [Browsable(true)]
        [TypeConverter(typeof(WebGridContextMenuCollectionConverter))]
        [Editor(typeof(WebGridContextMenuCollectionEditor), typeof(UITypeEditor))]

        public MenuItemCollection ContextMenu
        {
            get { return m_ContextMenu ?? (m_ContextMenu = new MenuItemCollection()); }
        }


        private MenuItemCollection m_ContextMenu;

        internal void GetContextMenuHtml(WebGridHtmlWriter writer)
        {
            if (Page == null)
                return;
            writer.Write(@"<ul id=""{0}_Menu"" class=""wgcontextMenu"">", ID);

            if (m_ContextMenu == null || !m_ContextMenu.Contains(ContextMenuType.Help))
            {
                AddClientScript(writer, "$(document).ready(function() {$(\".wghelpiframe\").colorbox({ width: \"400px\", height: \"675px\", iframe: true });});");
                writer.Write(@"<li class=""wgcontextmenuhelp""><a {0}>Display Help</a></li>",
                             AddMenuAction(ContextMenuType.Help));
            }
            if (m_ContextMenu == null || !m_ContextMenu.Contains(ContextMenuType.Export))
                writer.Write(@"<li class=""wgcontextmenuexport separator""><a {0}>Export Data</a></li>",
                             AddMenuAction(ContextMenuType.Export));
            if (m_ContextMenu == null || !m_ContextMenu.Contains(ContextMenuType.Refresh))
                writer.Write(@"<li class=""wgcontextmenurefresh""><a {0}>Refresh Data</a></li>",
                             AddMenuAction(ContextMenuType.Refresh));
            if (m_ContextMenu == null || !m_ContextMenu.Contains(ContextMenuType.CollapsedGrid))
            {
                string text = CollapsedGrid ? "Expand Grid" : "Collapse Grid";
                writer.Write(@"<li class=""wgcontextmenucollapse separator""><a {0}>{1}</a></li>",
                             AddMenuAction(ContextMenuType.CollapsedGrid), text);
            }


            if (ActiveMenuSlaveGrid != null)
                writer.Write(@"<li class=""wgcontextmenucollapse""><a {0}>Close slave grid</a></li>",
                             AddMenuAction(ContextMenuType.ActiveSlaveGrid));

            if (m_ContextMenu == null || !m_ContextMenu.Contains(ContextMenuType.Print))
                writer.Write(@"<li class=""wgcontextmenuprint""><a {0}>Print this grid</a></li>",
                             AddMenuAction(ContextMenuType.Print));

            if (m_ContextMenu == null || !m_ContextMenu.Contains(ContextMenuType.About))
            {
                AddClientScript(writer, "$(document).ready(function() {$(\".wgaboutiframe\").colorbox({ width: \"475px\", height: \"300px\", iframe: true });});");
                writer.Write(@"<li class=""wgcontextmenuhelp"" class=""wgcontextmenuabout""><a {0}>About WebGrid</a></li>",
                             AddMenuAction(ContextMenuType.About));
            }
            if (IsDemo)
                writer.Write(@"<li><a onclick=""alert('For full version register at www.webgrid.com');"">WebGrid Demo</a></li>");
            if (m_ContextMenu != null)
                foreach (ContextMenuItem item in ContextMenu)
                {
                    if (!item.Visible || string.IsNullOrEmpty(item.Text))
                        continue;
                    writer.Write("<li ");
                    if (item.CssClass != null)
                        writer.Write(@" class=""{0}""", item.CssClass);
                    if (item.Id != null)
                        writer.Write(@" id=""{0}""", item.Id);
                    writer.Write(@"><a {1}>{0}</a></li>", item.Text,
                                 item.InternalMenuType != ContextMenuType.Undefined
                                     ? AddMenuAction(item.InternalMenuType)
                                     : ((CustomMenuItem) item).Attributes);
                }
            writer.Write("</ul>");
        }

        private string AddMenuAction(ContextMenuType menuType)
        {
            switch (menuType)
            {
                case ContextMenuType.Export:
                    return string.Format("href=\"#\" onclick=\"{0};return false;\"",
                                         Page.ClientScript.GetPostBackEventReference(this, "ExcelExportClick"));
                case ContextMenuType.About:
                return string.Format(
                        "class=\"wgaboutiframe\" title=\"About WebGrid\" href=\"http://webgrid.com/aboutwebgrid.htm\"");
                  
                case ContextMenuType.Help:
                    return string.Format(
                        "class=\"wghelpiframe\" title=\"WebGrid Help\" href=\"?showhelp=true&amp;gridid={0}\"",ID);
                case ContextMenuType.Print:
                    return string.Format(
                        "href=\"#\" onclick=\"javascript:$('#wgContent_{0}').jqprint();return false;\"", ID);

                case ContextMenuType.CollapsedGrid:

                    string collapsedGridScript = EnableCallBack
                                                     ? Asynchronous.GetCallbackEventReference(this, "CollapseGridClick",
                                                                                              false,
                                                                                              string.Empty,
                                                                                              string.Empty)
                                                     : Page.ClientScript.GetPostBackEventReference(this,
                                                                                                   "CollapseGridClick");
                    return string.Format("href=\"#\" onclick=\"{0};return false;\"", collapsedGridScript);
                case ContextMenuType.Refresh:
                    string refreshScript = EnableCallBack
                                               ? Asynchronous.GetCallbackEventReference(this, "refresh", false,
                                                                                        string.Empty,
                                                                                        string.Empty)
                                               : Page.ClientScript.GetPostBackEventReference(this, "refresh");
                    return string.Format("href=\"#\" onclick=\"{0};return false;\"", refreshScript);
                case ContextMenuType.ActiveSlaveGrid:
                    if (ActiveMenuSlaveGrid == null)
                        return "N/A";
                    string activeSlaveGridScript = EnableCallBack
                                               ? Asynchronous.GetCallbackEventReference(this, "SlaveGridClick!"+ActiveMenuSlaveGrid.ID, false,
                                                                                        string.Empty,
                                                                                        string.Empty)
                                               : Page.ClientScript.GetPostBackEventReference(this, "SlaveGridClick!" + ActiveMenuSlaveGrid.ID);
                    return string.Format("href=\"#\" onclick=\"{0};return false;\"", activeSlaveGridScript);
               }
            return null;
        }

        internal string GetToolbarHtml(ToolbarType toolbarType)
        {

            if (m_CustomToolbars == null || m_CustomToolbars.Count == 0)
            {
                switch (toolbarType)
                {
                    case ToolbarType.ToolbarDetailBottom:
                        return ToolbarDetailBottom;
                    case ToolbarType.ToolbarGridBottom:
                        return ToolbarGridBottom;
                    case ToolbarType.ToolbarDetailTop:
                    case ToolbarType.ToolbarGridTop:
                        return null;
                }
            }

            if (m_CustomToolbars != null)
            {
                ToolbarItem mytoolbar = m_CustomToolbars.Find(delegate(ToolbarItem toolbar)
                                                                  {
                                                                      return toolbar != null && toolbar.ToolbarType == toolbarType;
                                                                  });
                if (mytoolbar != null)
                    return mytoolbar.GetControlsContent();
            }
            switch (toolbarType)
            {
                case ToolbarType.ToolbarDetailBottom:
                    return ToolbarDetailBottom;
                case ToolbarType.ToolbarGridBottom:
                    return ToolbarGridBottom;
                case ToolbarType.ToolbarDetailTop:
                case ToolbarType.ToolbarGridTop:
                    return null;
            }
            return null;
        }


        private ToolbarCollection m_CustomToolbars;

        /// <summary>
        /// A collection of customized system messages that either override system messages, or 
        /// add new system messages for this grid.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [MergableProperty(true)]
        [Description("Add or edit grid system messages.")]
        [Browsable(true)]
        [TypeConverter(typeof(WebGridSystemMessageCollectionConverter))]
        [Editor(typeof(WebGridSystemMessageCollectionEditor), typeof(UITypeEditor))]
        public GridSystemMessageCollection SystemMessages
        {
            get { return m_CustomSystemMessages ?? (m_CustomSystemMessages = new GridSystemMessageCollection()); }
        }

        /// <summary>
        /// Sets or gets custom information about the grid.
        /// Use this property if you need to pass custom-made information with the grid.
        /// </summary>
        [Bindable(true),
         Browsable(false),
         Description(
             @"Sets or gets custom information about the grid. Use this property if you need to pass custom-made information with the grid."
             )
        ]
        public NameValueCollection Tag
        {
            get { return m_Tag; }
            set { m_Tag = value; }
        }

        

      

       

        #region FeatureSettings




        //       private bool activeMenuSlaveGridset = false;
        /// <summary>
        /// Returns active slave grid from a slave grid menu. Null is returned if there is no Active slave grid.
        /// </summary>
        /// <value>The active menu slave grid.</value>
        [
            Browsable(false),
            Category("Menu"),
            Description(
                @"Sets or gets default slave grid for this object. default slave grid opens automatically when this object enters detail view."
                )
        ]
        public Grid ActiveMenuSlaveGrid
        {
            get
            {
                //   if (activeMenuSlaveGridset && eventRan_preRender)
                //        return storedActiveMenuSlaveGrid;

                if (GetState("ActiveMenuSlaveGrid") != null)
                {
                    foreach (Grid slavegrid in SlaveGrid)
                        if (slavegrid.ID == (string) GetState("ActiveMenuSlaveGrid"))
                            return slavegrid;
                }
                else if (DefaultSlaveGrid != null && DisplaySlaveGridMenu &&
                         GetState(ClientID + "_defaultSlaveGrid") == null)
                {
                    State(ClientID + "_defaultSlaveGrid", true);
                    State("ActiveMenuSlaveGrid", DefaultSlaveGrid);
                    if (m_EventRanpreRender)
                    {
                        m_StoredActiveMenuSlaveGrid = DefaultSlaveGridObject;
                        //                 activeMenuSlaveGridset = true;
                    }
                    return m_StoredActiveMenuSlaveGrid;
                }
                //               activeMenuSlaveGridset = true;
                return null;
            }
            set
            {
                //                activeMenuSlaveGridset = false;
                //                storedActiveMenuSlaveGrid = null;
                if (value != null)
                    State("ActiveMenuSlaveGrid", value.ID);
                else
                    State("ActiveMenuSlaveGrid", null);
            }
        }

        /// <summary>
        /// Sets or Gets whether cancel of an row update is allowed from the web interface. Default is true.
        /// If AllowCancel is false, then the 'Cancel' button in detail view is disabled.
        /// </summary>
        /// <value><c>true</c> if [allow cancel]; otherwise, <c>false</c>.</value>
        [Bindable(true),
         Category("Behavior"),
         DefaultValue(true),
         Description(
             @"Sets or Gets whether cancel of an row update is allowed from the web interface. Default is true.")
        ]
        public bool AllowCancel
        {
            get { return m_AllowCancel; }
            set { m_AllowCancel = value; }
        }

        /// <summary>
        /// Sets or Gets whether row copy is allowed from the web interface. Default is false.
        /// </summary>
        /// <value><c>true</c> if [allow copy]; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// This property must be true if you want to set allowCopy on row-level. 
        /// </remarks>
        [Bindable(true),
         Category("Behavior"),
        Description(@"Sets or Gets whether row copy is allowed from the web interface. Default is false.")
        ]
        public bool AllowCopy
        {
            get { return TristateBoolType.ToBool(m_AllowCopy, false); }
            set { m_AllowCopy = value; }
        }

        /// <summary>
        /// Sets or Gets whether row delete is allowed from the web interface. Default is true.
        /// </summary>
        /// <value><c>true</c> if [allow delete]; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// This property must be true if you want to set allowDelete on row-level. 
        /// </remarks>
        [Bindable(true),
         Category("Behavior"),
         DefaultValue(true),
         Description(@"Sets or Gets whether row delete is allowed from the web interface. Default is true.")
        ]
        public bool AllowDelete
        {
            get { return m_AllowDelete; }
            set { m_AllowDelete = value; }
        }

        /// <summary>
        /// Sets or Gets whether row edit is allowed from the web interface. Default is true.
        /// </summary>
        /// <value><c>true</c> if [allow edit]; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// This property must be true if you want to set allowEdit on row-level. 
        /// </remarks>
        [Bindable(true),
         Category("Behavior"),
         DefaultValue(true),
         Description(@"Sets or Gets whether row edit is allowed from the web interface. Default is true.")
        ]
        public bool AllowEdit
        {
            get { return m_AllowEdit; }
            set
            {
                m_Forceeditablecolumns = value;
                m_AllowEdit = value;
            }
        }

        /// <summary>
        /// Sets or Gets whether the user is allowed to create a new data record from the web interface. Default is true.
        /// </summary>
        /// <value><c>true</c> if [allow new]; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// This property returns false if <see cref="MaximumRecords">Grid.MaximumRecords</see> is larger or equal to MasterTable.Rows.Count.
        /// </remarks>
        [Bindable(true),
         Category("Behavior"),
         DefaultValue(true),
         Description(
             @"Sets or Gets whether the user is allowed to create a new data record from the web interface. Default is true."
             )
        ]
        public bool AllowNew
        {
            get
            {
                if (m_MaximumRecords > 0 && MasterTable.Rows.Count >= m_MaximumRecords)
                    return false;
                return m_AllowNew;
            }
            set { m_AllowNew = value; }
        }

        /// <summary>
        /// Sets or Gets whether sorting from the headers in grid view is allowed. Default is true.
        /// Some column properties might disable sorting automatically.
        /// </summary>
        /// <value><c>true</c> if [allow sort]; otherwise, <c>false</c>.</value>
        [Bindable(true),
         Category("Behavior"),
         DefaultValue(true),
         Description(@"Sets or Gets whether sorting from the headers in grid view is allowed. Default is true.")
        ]
        public bool AllowSort
        {
            get { return m_AllowSort; }
            set { m_AllowSort = value; }
        }

        /// <summary>
        /// Sets or Gets whether row update is allowed from the web interface. Default is true.
        /// If AllowUpdate is false, then you are not allowed to update already existings data.
        /// </summary>
        /// <value><c>true</c> if [allow update]; otherwise, <c>false</c>.</value>
        [Bindable(true),
         Category("Behavior"),
         DefaultValue(true),
         Description(@"Sets or Gets whether row update is allowed from the web interface. Default is true.")
        ]
        public bool AllowUpdate
        {
            get { return allowUpdate; }
            set { allowUpdate = value; }
        }

        /// <summary>
        /// Sets or Gets if the grid should render a basic HTML interface.
        /// Useful feature if you want to customise the layout.
        /// Web.Config key for this property is "WG" + property name.
        /// </summary>
        /// <value><c>true</c> if [basic edit layout]; otherwise, <c>false</c>.</value>
        [Bindable(true),
         Category("Default"),
         Browsable(false),
         Description(
             @"Sets or Gets if the grid should render a basic HTML interface. Useful feature if you want to customise the layout. Web.Config key for this property is ""WG"" + property name"
             )
        ]
        public bool BasicDetailLayout
        {
            set { m_BasicDetailLayout = value; }
            get
            {
                return m_BasicDetailLayout == null ? GridConfig.Get("WGBasicLayout", false) :
                    TristateBoolType.ToBool(m_BasicDetailLayout, false);
            }
        }

        /// <summary>
        /// Sets or Gets whether the grid should cache data structures, foreign keys etc. to minimize the dataload from the data source.
        /// Web.Config key for this property is "WG" + property name.
        /// </summary>
        [Bindable(true),
         Category("Data"),
         DefaultValue("True")]
        public bool CacheGridStructure
        {
            get
            {
                if (DesignMode)
                    return false;
                if (m_CacheGridStructure == null)
                    m_CacheGridStructure = GridConfig.Get("WGCacheGridStructure", true);

                return TristateBoolType.ToBool(m_CacheGridStructure, true);
            }
            set
            {
                m_CacheGridStructure = value;
                m_CacheGridStructureset = true;
            }
        }

     

        /// <summary>
        /// Gets or sets a value indicating whether the grid is collapsed.
        /// </summary>
        /// <value><c>true</c> if [collapsed grid]; otherwise, <c>false</c>.</value>
        public bool CollapsedGrid
        {
            get
            {
                if (ViewState["CollapseGrid"] != null)
                    return (bool)ViewState["CollapseGrid"];
               return (bool) (m_CollapsedGrid = GridConfig.Get("WGCollapseGrid", false));
            }
            set
            {
                if (m_CollapsedGrid != null) return;
                m_CollapsedGrid = value;
                ViewState["CollapseGrid"] = value;
            }
        }

        /// <summary>
        /// Sets or gets the current record being edited. By setting this manually you can force the Grid to be locked at one specific record.
        /// </summary>
        /// <value>The current row identifier (Usually identity or primary keys from data source).</value>
        [Bindable(true),
         Browsable(true),
         Category("Data"),
         DefaultValue(null),
          Description(
             @"Sets or gets the current record being edited. By setting this manually you can force the grid to be locked at one specific record."
             )
        ]
        public string EditIndex
        {
            get
            {
               return InternalId;
            }
            set {
                InternalId = value;
            }
        }

        /// <summary>
        /// Sets of gets the lookup key for use in HTTP query string variables upon non-postback. If found it sets this as EditIndex and goes directly to DetailMode.
        /// </summary>
        /// <value>The key to lookup for.</value>
        [Bindable(true),
         Browsable(true),
         Category("Data"),
         DefaultValue(null),
          Description(
             @"Sets of gets the lookup key for use in HTTP query string variables upon non-postback. If found it sets this as EditIndex and goes directly to DetailMode."
             )
        ]
        public string EditIndexRequest
        {
            get
            {
                return m_EditIndexRequest;
            }
            set
            {
                m_EditIndexRequest = value;
            }
        }

        private string m_EditIndexRequest;

        /// <summary>
        /// Gets group data information
        /// </summary>
        internal Dictionary<string, GroupInfo> GroupState
        {
            get
            {
                if (m_groupState != null)
                    return m_groupState;

                if (ViewState[ClientID + "GroupData"] == null)
                    m_groupState = new Dictionary<string, GroupInfo>();
                else
                {
                 
                    m_groupState = ViewState[ClientID + "GroupData"] as Dictionary<string, GroupInfo>;
                }
                return m_groupState;
            }
            set
            {
                m_groupState = value;
                ViewState[ClientID + "GroupData"] = value;
            }
        }

        internal string InternalId
        {
            get
            {
                if (m_EditIndexset)
                    return m_EditIndex;

                if (ViewState[ClientID + "EditIndex"] == null)
                    m_EditIndex = null;
                else
                    m_EditIndex = (string) ViewState[ClientID + "EditIndex"];
                if (m_EventRanLoad)
                    m_EditIndexset = true;
                return m_EditIndex;
            }
            set
            {
                
                if (value == null && ActiveMenuSlaveGrid != null && ActiveMenuSlaveGrid.AllowCancel)
                {
                    ActiveMenuSlaveGrid.InternalId = null;
                    ActiveMenuSlaveGrid.DisplayView = DisplayView.Grid;
                }
                m_EditIndex = value;
                ViewState[ClientID + "EditIndex"] = value;
                m_EditIndexset = true;
            }
        }

        /// <summary>
        /// Gets a dictionary of state information that allows you to save and restore the view state of a server control across multiple requests for the same page.
        /// </summary>
        /// <returns>
        /// An instance of the <see cref="T:System.Web.UI.StateBag"/> class that contains the server control's view-state information.
        /// </returns>
        protected override StateBag ViewState
        {
            get
            {
                if (base.ViewState == null)
                    return null;
                return base.ViewState;
            }
        }
        /// <summary>
        /// Sets or Gets whether the grid is in debug mode.
        /// Extended debug information is shown if you set trace="true" on your aspx file.
        /// </summary>
        /// <value><c>true</c> if debug; otherwise, <c>false</c>.</value>
        [Bindable(true),
         Category("Appearance"),
         DefaultValue(false),
         Description(@"Sets or Gets whether the grid is in debug mode.")
        ]
        public bool Debug
        {
            get
            {
                if (m_Debug == null)
                    m_Debug = GridConfig.Get("WGDebug", false);
                return TristateBoolType.ToBool(m_Debug, false);
            }
            set { m_Debug = value; }
        }

        /// <summary>
        /// Sets or gets default date/datetime format of the column. Default date format is retrieved from current thread datetimeformat (shortdatepattern))
        /// </summary>
        /// <value>The date time format.</value>
        [
            Category("Data"),
           Description(
                @"Sets or gets default date/datetime format of the column. Default date format is retrieved from current thread datetimeformat (shortdatepattern)"
                )
        ]
        public string DefaultDateTimeFormat
        {
            get { return m_DefaultDateTimeFormat ?? (m_DefaultDateTimeFormat = Culture.DateTimeFormat.ShortDatePattern); }
            set { m_DefaultDateTimeFormat = value; }
        }

        /// <summary>
        /// Sets or gets default slave grid for <see cref="WebGrid.Grid.DisplaySlaveGridMenu">WebGrid.Grid.DisplaySlaveGridMenu</see>.
        /// default slave grid opens automatically when the <see cref="WebGrid.Grid.MasterGrid"/> enters <see cref="WebGrid.Enums.DisplayView.Detail"/> mode.
        /// </summary>
        [
            Category("Menu"),
            DefaultValue(null),
            Description(
                @"Sets or gets default slave grid for this object. default slave grid opens automatically when this object enters detail view."
                )
        ]
        public string DefaultSlaveGrid { get; set; }

        /// <summary>
        /// Sets or gets the default visiblity for the columns in the grid. Default is Undefined.
        /// Web.Config key for this property is "WG" + property name.
        /// </summary>
        /// <value>The default visibility.</value>
        [Bindable(true),
         Category("Appearance"),
         Description(
             @"Sets or gets the default visiblity for the columns in the grid. Web.Config key for this property is ""WG"" + property name"
             )
        ]
        public Visibility DefaultVisibility
        {
            get
            {
                if (m_DefaultVisibility != Visibility.Undefined)
                    return m_DefaultVisibility;
                if (m_StoredVisibility != Visibility.Undefined)
                    return m_StoredVisibility;

                string vis;
                if ((vis = GridConfig.Get("WGDefaultVisibility")) != null)
                {
                    try
                    {
                        m_StoredVisibility = (Visibility)Enum.Parse(typeof(Visibility), vis, true);
                    }
                    catch
                    {
                        m_StoredVisibility = Visibility.Both;
                    }
                }
                else
                    m_StoredVisibility = Visibility.Both;
                return m_StoredVisibility;
            }
            set { m_DefaultVisibility = value; }
        }

       

       

        /// <summary>
        /// Sets or gets if the title bar should be displayed. Sorting ability will be inaccessable if the header is hidden.
        /// Web.Config key for this property is "WG" + property name.
        /// </summary>
        /// <value><c>true</c> if [display header]; otherwise, <c>false</c>.</value>
        [
            Category("Appearance"),
            Description(
                @"Sets or gets if the title bar should be displayed. Sorting ability will be inaccessable if the header is hidden. Web.Config key for this property is ""WG"" + property name"
                )
        ]
        public bool DisplayHeader
        {
            get
            {
                if (m_DisplayHeader == null && RecordsPerRow > 1)
                    return false;
                return m_DisplayHeader == null
                           ? GridConfig.Get("WGDisplayHeader", true)
                           : TristateBoolType.ToBool(m_DisplayHeader, true);
            }
            set { m_DisplayHeader = value; }
        }

      
    

      

        /// <summary>
        /// Sets or Gets whether the grid should show required icon for required fields in detail view.
        /// Remember: This property applies only to detail view. Required Icon is not used in grid view.
        /// Web.Config key for this property is "WG" + property name.
        /// </summary>
        /// <value><c>true</c> if [display required icon]; otherwise, <c>false</c>.</value>
        [Bindable(true),
         Category("Appearance"),
          Description(@"Sets or Gets whether the grid should show required icon for required fields in detail view.")
        ]
        public bool DisplayRequiredColumn
        {
            set { m_DisplayRequiredColumn = value; }
            get
            {
                if (BasicDetailLayout || (m_DisplayRequiredColumn == null && AllowEdit == false))
                    return false;
                return m_DisplayRequiredColumn == null ? GridConfig.Get("WGDisplayRequiredIcon", true) :
                    TristateBoolType.ToBool(m_DisplayRequiredColumn, true);
            }
        }
        /// <summary>
        /// Sets or gets the position for the search box, fixed around the grid.
        /// Web.Config key for this property is "WG" + property name.
        /// </summary>
        /// <value>The search box position</value>
        [Bindable(true),
         Category("Appearance"),
         Description(
             @"Sets or gets the position for the search box, fixed around the grid. Web.Config key for this property is ""WG"" + property name"
             )
        ]
        public Position DisplaySearchBox
        {
            get
            {
                if (m_Displaysearchbox == Position.Undefined)
                {
                    string sb = GridConfig.Get("DisplayGridSearchBox", string.Empty);
                    if (sb.Equals("hidden", StringComparison.OrdinalIgnoreCase))
                        m_Displaysearchbox = Position.Hide;
                    m_Displaysearchbox = Position.Visible;
                    m_Displaysearchbox_set = true;
                }
                return m_Displaysearchbox;
            }
            set { m_Displaysearchbox = value; }
        }

        /// <summary>
        /// Indicates whether the grid should use built-in menu for slave-grids.
        /// If undefined this property activates if there are two or more visibility slave-grids
        /// for this web control.
        /// </summary>
        /// <value><c>true</c> if [slave grid menu]; otherwise, <c>false</c>.</value>
        [
            Category("Menu"),
            Description(
                @"Indicates whether the grid should use built-in menu for slave-grids. If undefined this property activates if there are two or more visible slave-grids for this web control."
                )
        ]
        public bool DisplaySlaveGridMenu
        {
            get
            {
                return TristateBoolType.ToBool(m_DisplaySlaveGridMenu, true);
            }
            set { m_DisplaySlaveGridMenu = value; }
        }

        /// <summary>
        /// Sets or Gets whether sorting icons in the header should be visibility.
        /// </summary>
        /// <value><c>true</c> if [display sort icons]; otherwise, <c>false</c>.</value>
        [Bindable(true),
         Category("Appearance"),
         DefaultValue(true),
        Description(@"Sets or Gets whether sorting icons in the header should be visible.")
        ]
        public bool DisplaySortIcons
        {
            get { return m_DisplaySortIcons; }
            set { m_DisplaySortIcons = value; }
        }

        /// <summary>
        /// Sets or gets if the title bar should be displayed. 
        /// Hiding the title bar will also make the support buttons on the right inaccessable.
        /// Web.Config key for this property is "WG" + property name.
        /// </summary>
        /// <value><c>true</c> if [display title]; otherwise, <c>false</c>.</value>
        [
            Category("Appearance"),
            Description(
                @"Sets or gets if the title bar should be displayed. Hiding the title bar will also make the support buttons on the right inaccessable. Web.Config key for this property is ""WG"" + property name"
                ),
        ]
        public bool DisplayTitle
        {
            get
            {
                return m_DisplayTitle == null ? GridConfig.Get("WGDisplayTitle", true) : TristateBoolType.ToBool(m_DisplayTitle, true);
            }
            set { m_DisplayTitle = value; }
        }

        
      

        /// <summary>
        /// The filename of the file to be exported. Default is "Export.xls"
        /// </summary>
        /// <value>The name of the excel file.</value>
        [
            Category("Data"),
            DefaultValue("Export.xls"),
            Description(@"The filename of the file to be exported. Default is ""Export.xls""")
        ]
        public string ExportFileName
        {
            get { return m_ExportFileName; }
            set { m_ExportFileName = value; }
        }

        /// <summary>
        /// Sets or gets one of the supported languages. Default language is english.
        /// Web.Config key for this property is "WG" + property name.
        /// You can customize system language by using <see cref="WebGrid.Grid.SetSystemMessage">SetSystemMessage</see>.
        /// </summary>
        /// <value>The system language</value>
        [Bindable(true),
         Category("Default"),
         Description(
             @"Sets or gets langauge for the program. Default language is english. Web.Config key for this property is ""WG"" + property name"
             )
        ]
        public SystemLanguage Language
        {
            get
            {
                if (m_Language == SystemLanguage.Undefined)
                {
                    string webConfigLanguage = GridConfig.Get("WGLanguage", "english");

                    if (webConfigLanguage != null)
                        return (SystemLanguage)Enum.Parse(typeof(SystemLanguage), webConfigLanguage, true);
                }
                return m_Language;
            }
            set { m_Language = value; }
        }


        /// <summary>
        /// Sets or gets the name of the grid required to open this grid.
        /// Remember: The Master grid must have a valid primary key.
        /// </summary>
        /// <value>The master grid ColumnId.</value>
        [Bindable(true),
         Category("Menu"),
         DefaultValue(null),
         Description(
             @"Sets or gets the name of the grid required to open this grid. NOTE: The Master editor must have a valid primary key."
             )
        ]
        public string MasterGrid
        {
            get
            {
                return ViewState["GridIdentifier"] == null ? null : ViewState["GridIdentifier"].ToString();
            }
            set { ViewState["GridIdentifier"] = value; }
        }

        /// <summary>
        /// Gets or sets WebGrid.Grid object of Master grid for this grid.
        /// null is returned if this Grid has no MasterWebGrid.
        /// </summary>
        /// <value>The master webgrid object.</value>
        [
            Browsable(false)
        ]
        public Grid MasterWebGrid
        {
            get
            {
                if (string.IsNullOrEmpty(MasterGrid) || Page == null)
                    return null;
                if (m_MasterWebGrid != null)
                    return m_MasterWebGrid;
                m_MasterWebGrid = Page.Parent != null ? FindGrid(Page.Parent, MasterGrid) : FindGrid(Page, MasterGrid);

                if (m_MasterWebGrid == null)
                {
                    if (DesignMode)
                        return null;
                    throw new GridException(
                        (String.Format("'{0}' grid was unable to detect mastergrid '{1}'", ID, MasterGrid)));
                }
                if (
                    m_MasterWebGrid.DisplaySlaveGridMenu
                    && ((m_MasterWebGrid.ActiveMenuSlaveGrid != null && m_MasterWebGrid.ActiveMenuSlaveGrid !=
                                                                        this || m_MasterWebGrid.ActiveMenuSlaveGrid == null)))
                    return m_MasterWebGrid;

                // Detect if all primarykeys is found in the slavegrid, else initilize.
                if (m_MasterWebGrid.MasterTable.Columns.Primarykeys != null)
                    foreach (Column column in m_MasterWebGrid.MasterTable.Columns.Primarykeys)
                        if (MasterTable.m_Columns.GetIndex(column.ColumnId) == -1)
                            this[column.ColumnId].Visibility = Visibility.None;
                        /*else
                        {
                            if (m_MasterWebGrid != null)
                                m_MasterWebGrid.SystemMessage.Add(
                                    string.Format(
                                        "{0}' grid was missing primary key column '{2}' and 'MasterGrid' property for grid '{1}' is invalid",
                                        m_MasterWebGrid.ClientID, ID, column.ColumnId), true);
                            m_MasterWebGrid = null;
                        }*/
                return m_MasterWebGrid;
            }
        }

        /// <summary>
        /// Gets or Sets Menu text used in master grid.
        /// </summary>
        [Category("Appearance")]
        [
            Description(
                "Gets or sets Menu text used for this grid mastergrid. Default is this grid's title."
                )]
        public string MenuText
        {
            get
            {
                return string.IsNullOrEmpty(m_MenuText) ? Title : m_MenuText;
            }
            set { m_MenuText = value; }
        }

        /// <summary>
        /// Sets or gets the mode for the grid. Default is Grid.
        /// grid view is overview window showing several records.
        /// detail view is detail windows showing one specific record.
        /// </summary>
        /// <value>the grid view.</value>
        [Bindable(true),
         Category("Behavior"),
         DefaultValue(typeof(DisplayView), "Grid"),
         Description(
             @"Sets or gets the mode for the grid. Default is Gris. grid view is overview window showing several records. detail view is detail windows showing one specific record."
             )
        ]
        public DisplayView DisplayView
        {
            get
            {
                if (Equals(m_Gridmodeset, true))
                    return m_Gridmode;
                if (ViewState["Mode"] != null)
                    m_Gridmode = (DisplayView)ViewState["Mode"];
                m_Gridmodeset = true;
                return m_Gridmode;
            }
            set
            {
                /*if (value == DisplayView.Grid)
                    InternalId = null;*/
                ViewState["Mode"] = value;
                m_Gridmode = value;
            }
        }

        /// <summary>
        /// Gets the record count from your data source.
        /// </summary>
        /// <value>The record count.</value>
        [
            Browsable(false),
          ]
        public int RecordCount
        {
            get
            {
                return MasterTable == null ? 0 : MasterTable.RecordCount;
            }
        }

        private bool m_reLoadData;

        /// <summary>
        /// Indicates whether the Grid should reload data from data source. The Grid normally forces reload
        /// at insert,update, and delete events. This property applies to grid and detail view (data is reloaded at PreRender event.)
        /// </summary>
        /// <value><c>true</c> if [reload data]; otherwise, <c>false</c>.</value>
        [
            Browsable(false),
            DefaultValue(false),
        ]
        internal bool ReLoadData
        {
            get { return m_reLoadData; }
            set
            {
                m_reLoadData = value;
            }
        }

        /// <summary>
        /// Sets or gets a search string used to filter the grid's content.
        /// Works only with searchable columns.
        /// </summary>
        /// <value>The search string.</value>
        [
            Browsable(true),
            Category("Data"),
           Description(
                "Sets or gets a search string used to filter the grid's content. Works only with searchable columns."
                )
        ]
        public string Search
        {
            get
            {
                if (ViewState[ClientID + "GridSearch"] == null)
                    return string.Empty;
                return (string)ViewState[ClientID + "GridSearch"];

            }
            set
            {
                ViewState[ClientID + "GridSearch"] = value;
            }
        }





        /// <summary>
        /// Indicates whether SelectableRows is enabled.
        /// If you are using Charts then are row interaction disabled (like rowhighlight and selectable rows).
        /// the reason is that tbody is effected by rowspan for this column.
        /// Web.Config key for this property is "WG" + property name.
        /// </summary>
        /// <value><c>true</c> if [selectable rows]; otherwise, <c>false</c>.</value>
        [
            Category("Row"),
            Description(
                @"Indicates whether SelectableRows is enabled. If you are using Charts then are row interaction disabled (like rowhighlight and selectable rows) the reason is that thead is effected by rowspan for this column. Web.Config key for this property is ""WG"" + property name"
                )
        ]
        public bool SelectableRows
        {
            get
            {
                return m_SelectableRows == null ? GridConfig.Get("WGSelectableRows", true) : TristateBoolType.ToBool(m_SelectableRows, true);
            }
            set { m_SelectableRows = value; }
        }

        /// <summary>
        /// Gets a collection of all slave grid objects. (Grid's that requires this grid to open)
        /// </summary>
        /// <value>The slave grid.</value>
        [  Browsable(false)  ]
        public GridCollection SlaveGrid
        {
            get
            {
                // Optimize...
                if (m_StoredSlaveGrid != null)
                    return m_StoredSlaveGrid;

                GridCollection grids = new GridCollection();
                GridCollection slavegrids = new GridCollection();

                if (Page != null)
                    FindEditorControls(ref grids, Page.Controls);
                for (int i = 0; i < grids.Count; i++)
                    if (grids[i].MasterGrid != null && Equals(grids[i].Visible, true) &&
                        String.Compare(grids[i].MasterGrid, ID, true) == 0 &&
                        m_GridsInGrid.Contains(grids[i].ID) == false)
                        slavegrids.Add(grids[i]);

                grids.Clear();

                if (Equals(m_EventRanpreRender, true))
                    m_StoredSlaveGrid = slavegrids;
                return slavegrids;
            }
        }

        /// <summary>
        /// Sets or gets fields from the data source to specify data ordering.
        /// If you have more then one table in your data source then both table name and field name is required.
        /// Example: [tblCustomers].[CustomerId]
        /// </summary>
        /// <value>The SQL order by.</value>
        [Bindable(true),
         Category("Data"),
         DefaultValue(null),
        Description(
             @"Sets or gets fields from the data source to specify data ordering. If you have more then one datatable in your datasource then both table name and field name is required. Example: [tblCustomers].[CustomerID]"
             )
        ]
        public string SortExpression
        {
            get
            {
                string orderby = string.Empty;
                if (ViewState["SortExpression"] != null) orderby = ViewState["SortExpression"].ToString();
                return orderby;

            }
            set { ViewState["SortExpression"] = value; }
        }


        

        /// <summary>
        /// Sets or Gets whether the grid should stay in detail view after update/insert.
        /// It stays by default in detail view if it has one or more Slave grids.
        /// </summary>
        /// <value><c>true</c> if [stay edit]; otherwise, <c>false</c>.</value>
        [
            Category("Behavior"),
             Description(
                @"Sets or Gets whether the grid should stay in detail view after update/insert. It stays by default in detail view if it has one or more Slave grids."
                )
        ]
        public bool StayInDetail
        {
            get
            {
                if (!AllowCancel || (m_StayInDetail == null && SlaveGrid.Count > 0))
                    return true;
                return TristateBoolType.ToBool(m_StayInDetail, false);
            }
            set { m_StayInDetail = value; }
        }

        /// <summary>
        /// Sets or gets if the grid should submit when 'enter' key is hit within the grid. (Does not affect the html editor)
        /// Web.Config key for this property is "WG" + property name.
        /// </summary>
        /// <remarks>
        /// This feature uses ASP.NET Attributes 'onkeydown' for the grid, if this already is set by the user it will not apply.
        /// </remarks>
        /// <value>The title.</value>
        [Bindable(true),
         Category("Behavior"),
         Description(
            @"Sets or gets if the grid should submit when 'enter' key is hit within the grid. (Does not affect the html editor)")
        ]
        public bool UseSubmitBehavior
        {
            get
            {
                return m_UseSubmitBehavior == null ? GridConfig.Get("WGUseSubmitBehavior", true) : TristateBoolType.ToBool(m_UseSubmitBehavior, true);
            }
            set { m_UseSubmitBehavior = value; }
        }

        /// <summary>
        /// Sets or gets the xml file for system messages.
        /// Web.Config key for this property is "WG" + property name.
        /// </summary>
        /// <value>The language as XML file</value>
        [Bindable(true),
         Category("Data"),
         Description("Sets or gets the xml file for your language.")
        ]
        [EditorAttribute(typeof(XmlUrlEditor), typeof(UITypeEditor))]
        public string SystemMessageDataFile
        {
            set { m_SystemMessageDataFile = value; }

            get
            {
                return m_SystemMessageDataFile ?? GridConfig.Get("WGSystemMessageDataFile");
            }
        }

        /// <summary>
        /// Sets or gets the title of the grid. If no title is provided it will by default use data source name.
        /// </summary>
        /// <value>The title.</value>
        [Bindable(true),
         Category("Appearance"),
         Description(
             @"Sets or gets the title of the grid. If no title is provided it will by default use datasource name.")
        ]
        public string Title
        {
            get { return m_Title == null && string.IsNullOrEmpty(Sql) ? DataSourceId : m_Title; }
            set { m_Title = value; }
        }


        ///<summary>
        /// Gets or sets grouping expression for this data table.
        ///</summary>
        public string GroupByExpression
        {
            get
            {
                return MasterTable != null ? MasterTable.GroupByExpression : null;
            }
            set
            {
                if (MasterTable == null)
                    return;
                MasterTable.GroupByExpression = value;
            }
        }
      

        /// <summary>
        /// Sets or gets a filter for the data source.
        /// "WHERE" should not be written. Only the statement.
        /// If your data source contains more then one table, the
        /// where statement might require you include both table name and table field.
        /// Example is: [Customers].[CustomerId]
        /// Where Customers is table name and CustomerId is table field.
        /// </summary>
        /// <value>The SQL where</value>
        [Bindable(true),
         Category("Data"),
         Description(
             @"Sets or gets a filter for the data source. ""WHERE"" should not be written. Only the statement. If your data source contains more then one datatable, the where statement might require you include both datatable name and datatable field. Example is: [Customers].[CustomerID] Where Customers is datatable name and CustomerID is datatable field."
             )
        ]
        public string FilterExpression
        {
            get
            {
                if (!string.IsNullOrEmpty(m_FilterExpression))
                    return m_FilterExpression;

                // Implementing column filter.
                string selectedFilterExpression = null;
                if (m_EventRanLoad && GotHttpContext)
                    selectedFilterExpression += BuildFilterByColumnSql();

                if (string.IsNullOrEmpty(selectedFilterExpression))
                {
                    if (m_EventRanpreRender)
                        m_FilterExpression = m_UserFilterExpression;
                    return m_UserFilterExpression;
                }
                switch (string.IsNullOrEmpty(m_UserFilterExpression))
                {
                    case false:
                        if (m_EventRanpreRender)
                            m_FilterExpression = string.Format("{0} AND {1}", m_UserFilterExpression, selectedFilterExpression);
                        return string.Format("{0} AND {1}", m_UserFilterExpression, selectedFilterExpression);
                    default:
                        if (Equals(m_EventRanpreRender, true))
                            m_FilterExpression = selectedFilterExpression;
                        return selectedFilterExpression;
                }
            }
            set
            {
                m_FilterExpression = null;
                if (value != m_UserFilterExpression)
                    MasterTable.m_OldEditIndex = "-1";
                m_UserFilterExpression = value;
              
            }
        }
        internal bool m_Forceeditablecolumns;

        internal Grid m_MasterWebGrid;




        /// <summary>
        /// Sets or Gets whether "update rows" button should be visibility from the web interface. Default is true. Applies to grid view.
        /// </summary>
        /// <value><c>true</c> if [allow update rows]; otherwise, <c>false</c>.</value>
        [Bindable(true),
         Category("Behavior"),
         Description(
             @"Sets or Gets whether ""update rows"" button should be visible from the web interface. Default is true. Applies to grid view."
             )
        ]
        internal bool AllowUpdateRows
        {
            get
            {
                if (DisplayView == DisplayView.Detail || AllowEditInGrid == false)
                    return false;

                return TristateBoolType.ToBool(m_AllowUpdateRows, true);
            }
            set { m_AllowUpdateRows = value; }
        }

        /// <summary>
        /// Sets or Gets if the columns title should have their "ColumnId" in their names removed. Default is false.
        /// Web.Config key for this property is "WG" + property name.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [automatically remove ColumnId]; otherwise, <c>false</c>.
        /// </value>
        [Bindable(true),
         Category("Default"),
         DefaultValue("false"),
         Description(
             @"Sets or Gets if the columns title should have their ""ID"" in their names removed. Default is false. Web.Config key for this property is ""WG"" + property name"
             )
        ]
        internal bool AutomaticallyRemoveID
        {
            get
            {
                return m_AutomaticallyRemoveID == null
                           ? GridConfig.Get("WGAutomaticallyRemoveID", false)
                           : TristateBoolType.ToBool(m_AutomaticallyRemoveID, false);
            }
            set { m_AutomaticallyRemoveID = value; }
        }

        /// <summary>
        /// Sets or gets supported back/reset choice in the menu for slavegrids. Default property is AlwaysVisible
        /// Web.Config key for this property is "WG" + property name.
        /// </summary>
        [
            Category("Menu"),
            Description(
                @"Sets or gets supported back/reset choice in the menu for slavegrids. Default property is AlwaysVisible. Web.Config key for this property is ""WG"" + property name"
                )
        ]


        internal Grid DefaultSlaveGridObject
        {
            get
            {
                if (m_DefaultSlaveGridObject != null)
                    return m_DefaultSlaveGridObject;

                if (string.IsNullOrEmpty(DefaultSlaveGrid) || InternalId == null)
                    return null;

                m_DefaultSlaveGridObject = Page.Parent != null ? FindGrid(Page.Parent, DefaultSlaveGrid) : FindGrid(Page, DefaultSlaveGrid);


                if (m_DefaultSlaveGridObject == null && m_EventRanpreRender)
                    throw new ApplicationException(
                        String.Format("'{0}' grid was unable to detect DefaultSlaveGrid '{1}'", ID, DefaultSlaveGrid));

                return m_DefaultSlaveGridObject;
            }
        }


        /// <summary>
        /// Gets the last mode.
        /// </summary>
        /// <value>The last mode.</value>
        internal DisplayView LastMode
        {
            get
            {
                if (ViewState["LastGridMode"] == null)
                    return DisplayView;
                return (DisplayView)ViewState["LastGridMode"];
            }
        }

        /// <summary>
        /// Indicates the position of the grid within the menu
        /// </summary>
        /// <value>The menu priority at MasterWebGrid.</value>
        [
            Category("Menu"),
            Description(@"Indicates the position of the grid within the menu.")
        ]
        internal int Menupriority
        {
            get { return m_MenuPriority; }
            set { m_MenuPriority = value; }
        }

        





        private bool allowUpdate = true;

        private bool m_AllowCancel = true;


        private bool? m_AllowCopy;

        private bool m_AllowDelete = true;

        //	private bool	allowEditset = false;
        private bool m_AllowEdit = true;


        private bool m_AllowNew = true;

        private bool m_AllowSort = true;


        private bool? m_AllowUpdateRows;


        private bool? m_AutomaticallyRemoveID;


        private bool? m_BasicDetailLayout;


        /* NOT ACTIVE YET */
        /*
				private string		_systemMessageEmail = null;
		*/
        /*
				/// <summary>
				/// Sets or gets the email address. If set the an email will be sent when critical errors occurs. 
				/// Web.Config key for this property is "WG" + property name.
				/// </summary>
				[Bindable(true),
					Category("Layout"),
					DefaultValue("")]
				private string		errorEmail
				{
					set { _systemMessageEmail = value; }
					get 
					{ 
				
						if(_systemMessageEmail == null)
							_systemMessageEmail = appConfig.Get("WGerrorEmail",null);
						return _systemMessageEmail;
		
					}
				}
		*/


        private bool? m_CacheGridStructure;


      
        private bool m_CacheGridStructureset;



        private bool? m_CollapsedGrid;

        private string m_EditIndex;
        private bool m_EditIndexset;



        private bool? m_Debug;


        private string m_DefaultDateTimeFormat;


        private Grid m_DefaultSlaveGridObject;


        private Visibility m_DefaultVisibility = Visibility.Undefined;


     
        private bool? m_DisplayHeader;


     





        private bool? m_DisplayRequiredColumn;

        private Position m_Displaysearchbox = Position.Undefined;
        private bool m_Displaysearchbox_set;


        private bool? m_DisplaySlaveGridMenu;

        private bool m_DisplaySortIcons = true;


        private bool? m_DisplayTitle;

        private string m_ExportFileName = "Export.xls";


        private DisplayView m_Gridmode = DisplayView.Grid;
        private bool m_Gridmodeset;


        private SystemLanguage m_Language = SystemLanguage.Undefined;

        private int m_MenuPriority = 10;

        private string m_MenuText;





        //  private bool? m_RememberBrowserPosition  ;


        private bool? m_SelectableRows;

      

        private bool? m_StayInDetail;

        private Grid m_StoredActiveMenuSlaveGrid;


        private GridCollection m_StoredSlaveGrid;

        private Visibility m_StoredVisibility = Visibility.Undefined;

        private string m_FilterExpression;




        private bool? m_UseSubmitBehavior;

        private string m_SystemMessageDataFile;


        private string m_Title;
        private string m_UserFilterExpression = string.Empty;


        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeAllowCopy()
        {
            return m_AllowCopy != null;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeDialogTitle()
        {
            return m_DialogTitle != null;
        }

         /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeGetColumnsPostBackData()
        {
           return false;
        }
        

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeAllowUpdateRows()
        {
            return m_AllowUpdateRows != null;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeAutomaticallyRemoveID()
        {
            return m_AutomaticallyRemoveID != null;
        }



        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeBasicDetailLayout()
        {
            return m_BasicDetailLayout != null;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeCacheGridStructure()
        {
            return m_CacheGridStructure != null && !Equals(m_CacheGridStructureset, false);
        }

        

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeCollapsedGrid()
        {
            return m_CollapsedGrid != null;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeDebug()
        {
            return m_Debug == null;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeDefaultDateTimeFormat()
        {
            return !m_DefaultDateTimeFormat.Equals(Culture.DateTimeFormat.ShortDatePattern);
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeCulture()
        {
            if (m_GridCulture == null || m_GridCulture == CultureInfo.InvariantCulture)
                return false;
            return true;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeDefaultVisibility()
        {
            return m_DefaultVisibility != Visibility.Undefined;
        }

     

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeClientObjectId()
        {

            return ClientObjectId != null;
        }

      

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeDisplayHeader()
        {
            return m_DisplayHeader != null;
        }

      

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeDisplayRequiredColumn()
        {
            return m_DisplayRequiredColumn != null;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeDisplaySearchBox()
        {
            return m_Displaysearchbox != Position.Undefined && !m_Displaysearchbox_set;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeDisplaySlaveGridMenu()
        {
            return m_DisplaySlaveGridMenu != null;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeDisplayTitle()
        {
            return m_DisplayTitle != null;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeLanguage()
        {
            return m_Language != SystemLanguage.Undefined;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeMasterGrid()
        {
            return ViewState["GridIdentifier"] != null;
        }


        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeMenuText()
        {
            return m_MenuText != null;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeDisplayView()
        {
            return m_Gridmode == DisplayView.Detail;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeSearch()
        {

            return false;

        }



        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeSelectableRows()
        {
            return m_SelectableRows != null;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeSortExpression()
        {
            return MasterTable != null && !string.IsNullOrEmpty(MasterTable.SortExpression);
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeGroupByExpression()
        {
            return MasterTable != null && !string.IsNullOrEmpty(MasterTable.GroupByExpression);
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeStayInDetail()
        {
            return m_StayInDetail != null;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeUseSubmitBehavior()
        {
            return m_UseSubmitBehavior != null;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeSystemMessageDataFile()
        {
            return m_SystemMessageDataFile != null;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeTitle()
        {
            return m_Title != null;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeFilterExpression()
        {
            return !string.IsNullOrEmpty(FilterExpression);
        }

        #endregion

      
    }

    

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal sealed class StringToObject : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.None;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            return value !=  null ? value.ToString() : null;
        }
    }
}
