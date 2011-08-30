/*
Copyright ©  Olav Christian Botterli.

Dual licensed under the MIT or GPL Version 2 licenses.

Date: 30.08.2011, Norway.

http://www.webgrid.com
*/
#undef NET35

#region Header

/*
Copyright ©  Olav Christian Botterli. 

Dual licensed under the MIT or GPL Version 2 licenses.

Date: 30.08.2011, Norway.

http://www.webgrid.com
*/

#endregion Header

namespace WebGrid.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.OleDb;
    using System.Data.SqlClient;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Xml;

    using Collections;
    using Database;
    using SqlConnection = Database.SqlConnection;
    using Design;
    using Enums;
    using Events;
    using Sort;
    using Util;

    /// <summary>
    /// This class typically represents a table from the data source. A table instance is 
    /// a placeholder for <see cref="Row">WebGrid.Row</see> (records from a data source).
    /// </summary>
    [Serializable]
    public class Table
    {
        #region Fields

        /// <summary>
        /// Indicates if schema should be loaded (if not already loaded)
        /// Schema is always loaded when column collection is external use, but in some cases it should not be 
        /// loaded when queried internally.
        /// </summary>
        internal const bool m_Loadschema = true;

        internal ColumnCollection m_Columns;
        internal DataTable m_DataSourceColumns;
        internal DataSourceControlType m_DataSourceType = DataSourceControlType.Undefined;

        /// <summary>
        /// Indicates if the table has retrieved data.
        /// </summary>
        internal bool m_GotData;
        internal bool m_GotPostBackData;
        internal bool m_GotSchema;
        internal Grid m_Grid;
        internal bool m_LoadedTableSchema;

        /// <summary>
        /// Indicates if 'filter' sql statement is loaded for the table. (Triggers only at at PreRender event.)
        /// </summary>
        [NonSerialized]
        internal string m_Loadfilter;
        internal string m_OldEditIndex = "-1";
        internal string m_XmlDataDocument;
        internal DataSet m_XmlDataSet;
        internal string m_Xmlxpath;

        private DbParameterCollection dbparam;
        private string m_CacheKey;
        private string m_ConnectionString;
        private string m_DataMember;
        private string m_DataSourceId;
        private Interface m_DbInterface;
        private string m_FilterExpression = string.Empty;
        private string m_GroupByColumns = string.Empty;
        private string m_OrderBy = string.Empty;
        private int m_RecordCount = -1;
        private bool m_RetrieveForeignkeys = true;
        private RowCollection m_Rows;
        private string m_Search;
        private bool m_beforeGetDataEventExecuted;
        private Control m_controlDataSource;
        object m_dataSource;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Table">Table</see> class.
        /// </summary>
        /// <param name="mygrid">The Grid that is the parent object of the table.</param>
        /// <param name="gotcache">if set to <c>true</c> [m_ gotcache].</param>
        public Table(Grid mygrid, bool gotcache)
        {
            m_Grid = mygrid;
            GotCache = gotcache;
            m_Rows = new RowCollection(this);
            m_Columns = new ColumnCollection(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Table">Table</see> class.
        /// </summary>
        internal Table(Grid mygrid)
        {
            m_Grid = mygrid;
            //DBInterface = new Database.SqlConnection();
            m_Rows = new RowCollection(this);
            m_Columns = new ColumnCollection(this);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Indicates whether the table should cache database-design information.
        /// Cache database-design is automatic for MSSQL server, and default disabled
        /// for other SQL servers.
        /// </summary>
        /// <value><c>true</c> if [cache structure]; otherwise, <c>false</c>.</value>
        public bool CacheDatasourceStructure
        {
            get { return DataBaseInterface.Cachable && m_Grid.CacheGridStructure && m_Grid.Page != null; }
        }

        /// <summary>
        /// Gets the cache key for this table. (Only valid for SqlConnection and OleDb Data Interface)
        /// </summary>
        /// <value>The cache key.</value>
        public string CacheKey
        {
            get
            {
                return m_CacheKey ??
                       (m_CacheKey =
                        m_RetrieveForeignkeys == false
                            ? string.Format("{0}_{1}_{2}_noForeignKeys", m_Grid.Trace.ClientID, m_Grid.ClientID,
                                            DataSourceId)
                            : string.Format("{0}_{1}_{2}", m_Grid.Trace.ClientID, m_Grid.ClientID, DataSourceId));
            }
        }

        /// <summary>
        /// Gets or sets the settings for each column in the table.
        /// </summary>
        /// <value>A column collection of table-column settings.</value>
        public ColumnCollection Columns
        {
            get
            {
                if (m_GotSchema == false)
                    GetSchema();
                return m_Columns;
            }
            set
            {
                if (m_GotSchema == false)
                    GetSchema();
                m_Columns = value;
            }
        }

        /// <summary>
        /// Gets or sets the connection string to be used for retrieving data for this table.
        /// </summary>
        /// <value>The OleDB connection string.</value>
        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(m_ConnectionString))
                {
                    if (m_Grid != null)
                        m_ConnectionString = m_Grid.ActiveConnectionString;
                    else
                        return null;
                }
                return m_ConnectionString;
            }
            set { m_ConnectionString = value; }
        }

        /// <summary>
        /// Gets the control data source.
        /// </summary>
        /// <value>The control data source.</value>
        public Control ControlDataSource
        {
            get { return m_controlDataSource; }
        }

        /// <summary>
        /// Gets or sets the data interface to use when communicating with the database.
        /// </summary>
        /// <value>The DB interface.</value>
        public Interface DataBaseInterface
        {
            get
            {
                if (m_DbInterface == null)
                {
                    switch (m_Grid.DatabaseConnectionType)
                    {
                        case DataBaseConnectionType.SqlConnection:
                            m_DbInterface = new SqlConnection();
                            break;
                        case DataBaseConnectionType.OleDB:
                            m_DbInterface = new OleDB();
                            break;
                       /* case DataBaseConnectionTypes.MySql:
                            m_DbInterface = new MySQL();
                            break;*/
                        default:
                            m_DbInterface = new SqlConnection();
                            break;
                    }
                }

                if (m_DbInterface == null)
                    throw new GridException("WebGrid could not find a valid data interface (data interface is null)");

                return m_DbInterface;
            }
            set { m_DbInterface = value; }
        }

        /// <summary>
        /// Gets or sets the name of the list of data that the data-bound control binds to, in cases where the data source contains more than one distinct list of data items.
        /// </summary>
        public string DataMember
        {
            get { return m_DataMember; }
            set { m_DataMember = value; }
        }

        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public object DataSource
        {
            set
            {

                if (this == m_Grid.MasterTable && Grid.GotHttpContext)
                {
                    HttpContext.Current.Session.Remove(m_Grid.UniqueID + "_Datasourcedotnet");
                    HttpContext.Current.Session.Remove(m_Grid.UniqueID + "_DataTableSettings");
                    if (m_Grid.Debug)
                        m_Grid.m_DebugString.AppendLine("<b>"+m_Grid.ID + ":</b> Cleared Data source and data source settings.<br/>");
                }

                if (value is XmlDataDocument)
                {
                    m_dataSource = value;
                    return;
                }
                if (value is DataSet)
                    SetDataSource((DataSet) value);
                else if (value is DataTable)
                    SetDataSource((DataTable)value);
                else if (value is DataView)
                    SetDataSource((DataView)value);
                else if (value is OleDbDataAdapter)
                {
                    DataSourceType = DataSourceControlType.InternalDataSource;
                    SetDataSource((OleDbDataAdapter)value);
                }
                else if (value is IEnumerable)
                    SetDataSource((IEnumerable)value);
                else if (IsGenericCollection(value))
                    SetGenericDataSource(value as Collection<object>);
                else
                    throw new ArgumentException(@"Invalid object. Object must implement IList or IEnumerable", "value");

                DataSourceType = DataSourceControlType.EnumerableDataSource;

             }
            get
            {
                return m_dataSource;
            }
        }

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string DataSourceId
        {
            get
            {
                return m_DataSourceId;
            }
            set { m_DataSourceId = value; }
        }

        /// <summary>
        /// Gets the type of the data source used.
        /// </summary>
        /// <value>The type of the data source.</value>
        public DataSourceControlType DataSourceType
        {
            get
            {
                if (m_DataSourceType == DataSourceControlType.Undefined)
                    m_DataSourceType = FindDataSourceType(DataSourceId);
                return m_DataSourceType;

            }
            internal set { m_DataSourceType = value; }
        }

        /// <summary>
        /// Gets or sets the extra 'filter' query when retrieving data.
        /// </summary>
        /// <value>The SQL 'filter' query.</value>
        public string FilterExpression
        {
            get { return m_FilterExpression; }
            set
            {
                if (value != m_FilterExpression)
                {
                    m_OldEditIndex = "-1";
                    m_Loadfilter = null;
                    m_GotData = false;
                }
                m_FilterExpression = value;
            }
        }

        ///<summary>
        /// Gets or sets grouping expression for this data table.
        ///</summary>
        public string GroupByExpression
        {
            get { return m_GroupByColumns; }
            set
            {

                m_GroupByColumns = value;
            }
        }
        /// <summary>
        /// Retrieve only unused data for this unique data-column.
        /// </summary>
        /// <value>The column name (the column must be in the database).</value>
        public string GetUniqueDataColumn
        {
            get; set;
        }

        /// <summary>
        /// Gets a value indicating whether [got data source cache].
        /// </summary>
        /// <value><c>true</c> if [got data source cache]; otherwise, <c>false</c>.</value>
        public bool GotCache
        {
            get; private set;
        }

        /// <summary>
        /// Indicates if the table has loaded data for the rows.
        /// </summary>
        public bool GotData
        {
            get { return m_GotData; }
            set {  m_GotData = value; }
        }

        /// <summary>
        /// Gets or a value indicating whether got data source schema.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [got data source schema]; otherwise, <c>false</c>.
        /// </value>
        public bool GotSchema
        {
            get { return m_GotSchema; }
        }

        /// <summary>
        /// Gets if this table has custom paging (used by objectdatasource)
        /// </summary>
        public bool IsCustomPaging
        {
            get; private set;
        }

        /// <summary>
        /// Adds a sorting statement for the data source.
        /// </summary>
        /// <value>The statement.</value>
        internal string OrderByAdd
        {
            set
            {
                if (this == m_Grid.MasterTable)
                {
                    SortList sl = new SortList(m_Grid.SortExpression);
                    sl.Add(value);
                    m_Grid.SortExpression = sl.ToString();
                }
                else
                {
                    SortList sl = new SortList(m_OrderBy);
                    sl.Add(value);
                    m_OrderBy = sl.ToString();
                }
            }
        }

        // Primarykeys for this column collections
        /// <summary>
        /// Gets a Column vollection of all primary keys for this Table
        /// </summary>
        public List<Column> Primarykeys
        {
            get
            {
                return Columns != null ? Columns.Primarykeys : null;
            }
        }

        /// <summary>
        /// Gets the number of records in the table.
        /// </summary>
        /// <value>The record count.</value>
        public int RecordCount
        {
            get
            {
                if (m_Grid.InternalId == m_OldEditIndex)
                    return m_RecordCount;

                if (IsCustomPaging || DataSource != null && this == m_Grid.MasterTable)
                    return m_RecordCount;
                if (m_Grid.DataSourceId == null || (m_Grid.DataSourceId != null && m_Grid.ConnectionString == null))
                    return 0;
                m_RecordCount = Interface.RecordCount(this);
                m_OldEditIndex = m_Grid.InternalId;
                if (m_Grid.Trace.IsTracing)
                    m_Grid.Trace.Trace("{0} : Table.RecordCount ({1})", m_Grid.ID, DataSourceId);
                return m_RecordCount;
            }
            internal set
            {
                m_RecordCount = value;
            }
        }

        /// <summary>
        /// Whether foreignkeys also should be loaded.
        /// </summary>
        /// <value><c>true</c> if [get foreign keys]; otherwise, <c>false</c>.</value>
        public bool RetrieveForeignkeys
        {
            get { return m_RetrieveForeignkeys; }
            set { m_RetrieveForeignkeys = value; }
        }

        /// <summary>
        /// Gets or sets the rows in this table.
        /// </summary>
        /// <value>The rows inside this table.</value>
        public RowCollection Rows
        {
            get
            {
                if (!m_GotData)
                    GetData(false);
                return m_Rows;
            }
            set
            {
                if (!m_GotData)
                    GetData(false);
                m_Rows = value;
            }
        }

        /// <summary>
        /// Gets or sets filters used when retrieving data from the database.
        /// </summary>
        /// <value>The search value for this table.</value>
        public string Search
        {
            get { return m_Search ?? (m_Search = this == m_Grid.MasterTable ? m_Grid.Search : string.Empty); }
            set { m_Search = value; }
        }

        /// <summary>
        /// Sets or gets the sorting statement for the data source.
        /// </summary>
        /// <value>The SQL sorting statement.</value>
        public string SortExpression
        {
            set
            {
                if (this == m_Grid.MasterTable)
                    m_Grid.SortExpression = value;
                else
                    m_OrderBy = value;
            }
            get { return this == m_Grid.MasterTable ? m_Grid.SortExpression : m_OrderBy; }
        }

        /// <summary>
        /// Gets or sets a Custom Sql for WebGrid to retrieve data.
        /// </summary>
        /// <value>The SQL query</value>
        public string Sql
        {
            get; set;
        }

        /// <summary>
        /// Gets the order by sort list.
        /// </summary>
        /// <value>The sort list.</value>
        internal SortList OrderBySortList
        {
            get { return this == m_Grid.MasterTable ? new SortList(m_Grid.SortExpression) : new SortList(m_OrderBy); }
        }

        #endregion Properties

        #region Indexers

        /// <summary>
        /// Returns the row at the specified index in this table.
        /// </summary>
        /// <value>Return specified (by index value) row within this table.</value>
        public Row this[int index]
        {
            get
            {
                if (!m_GotData)
                    GetData(false);
                return Rows[index];
            }
            set
            {
                if (!m_GotData)
                    GetData(false);
                Rows[index] = value;
            }
        }

        // 2004.01.09 - jorn : String.Compare
        /// <summary>
        /// Returns the row at the specified primary key(s) in the table.
        /// </summary>
        /// <value>Return specified (by primary keys) row within this table.</value>
        /// <remarks>
        /// If a row with the specified primary key(s) does not exist in the table then null is returned.
        /// </remarks>
        public Row this[string primarykeys]
        {
            get
            {
                return Rows.Find(row => String.Compare(row.PrimaryKeyUpdateValues, primarykeys, false) == 0
                    );

            }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Finds data source web control.
        /// </summary>
        /// <param name="dataSourceId"></param>
        /// <returns></returns>
        public DataSourceControlType FindDataSourceType(string dataSourceId)
        {
            if (m_Grid.Page == null || Grid.FindControl<Control>(m_Grid.Parent, dataSourceId) == null)
            {
                m_DataSourceType = DataSourceControlType.InternalDataSource;

                return m_DataSourceType;
            }

            Control control = Grid.FindControl<Control>(m_Grid.Page, dataSourceId);
            if (control == null)
                return DataSourceControlType.Undefined;
            if (control is AccessDataSource)
                m_DataSourceType = DataSourceControlType.AccessDataSource;
            else if (control is SqlDataSource)
                m_DataSourceType = DataSourceControlType.SqlDataSource;
            else if (control is XmlDataSource)
                m_DataSourceType = DataSourceControlType.XmlDataSource;
            else if (control is ObjectDataSource)
                m_DataSourceType = DataSourceControlType.ObjectDataSource;
             #if NET35  
            else if (control is LinqDataSource)
                m_DataSourceType = DataSourceControlType.LinqDataSource;
            else if (control is EntityDataSource)
                m_DataSourceType = DataSourceControlType.EntityDataSource;
            #endif
            else
                m_DataSourceType = DataSourceControlType.InternalDataSource;

            //Create a reference to the data control we are using.
            if (m_DataSourceType != DataSourceControlType.InternalDataSource)
                m_controlDataSource = control;

            return m_DataSourceType;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="forceGetData">if set to <c>true</c> [force get data].</param>
        public void GetData(bool forceGetData)
        {
            GetData(forceGetData, false);
        }

        /// <summary>
        /// Gets WebGrid to load data
        /// </summary>
        /// <param name="forceGetData">if true webgrid will reload data even if already loaded.</param>
        /// <param name="getAllRows">load all records from the data source</param>
        public void GetData(bool forceGetData, bool getAllRows)
        {
            if (!m_beforeGetDataEventExecuted)
            {
                m_beforeGetDataEventExecuted = true;
                BeforeGetDataEventArgs res = m_Grid.BeforeGetDataEvent(this);
                if (res != null)
                {
                    forceGetData = res.ForceGetData;
                    getAllRows = res.GetAllRows;
                }
            }
            if (forceGetData)
                m_GotData = false;
            if (m_GotData)
                return;
            if (this == m_Grid.MasterTable)
                m_Grid.SetupFilterByColumns();

            if (m_GotSchema == false)
                GetSchema();

            if (m_Grid.SystemMessage.CriticalCount > 0 || m_GotData)
                return;
            m_GotData = true;

            RetreiveDataSourceSession();
            LoadDataSourceControl(m_DataSourceId);

            if (DataSourceId == null && Sql == null && DataSource == null && m_XmlDataDocument == null)
            {
                m_GotData = true;

                if (m_Grid.DisplayView == DisplayView.Detail && Rows.Count == 0)
                    Interface.GetEmptyData(this);
                  GetPostBackData();
                return;
            }

            // 20050404 - jorn - don't use connection string for  table
            if (DataSourceId != null && string.IsNullOrEmpty(ConnectionString) && DataSource == null)
            {
                //We were unable to detect connectionstring.
                if (m_Grid.IsDesignTime)
                {

                    m_Grid.SystemMessage.Add(string.Format("Unable to detect a data source object with Id '{0}'. You are either missing connectionstring for a direct-table reference or move WebGrid *after* any data source controls in your design-source and refresh.", m_Grid.DataSourceId));
                    return;
                }

                throw new GridException(
                    string.Format(
                        "A standard OLEDB or SqlConnection(.Net)connection string property has not been set for DatasourceId: {0}",
                        m_DataSourceId), null);
            }

            if (m_Grid.Debug)
            {
                m_Grid.m_DebugString.AppendFormat("<b>{0}: GetData ({1})</b> - force data is {2}<br/>", m_Grid.ID,
                                                  DataSourceId,
                                                  forceGetData);
                if (m_Grid.Trace.IsTracing)
                    m_Grid.Trace.Trace(
                        string.Format("GetData ({0}) - force data is {1}", DataSourceId,
                                      forceGetData));
            }
            Rows.Clear();

            // Get empty data for NEW RECORD / INSERT
            if (this == m_Grid.MasterTable && m_Grid.DisplayView == DisplayView.Detail && m_Grid.InternalId == null)
            {
                if (m_Grid.Trace.IsTracing)
                    m_Grid.Trace.Trace("{0} : Start (GetData) GetEmptyData({1});", m_Grid.ID, DataSourceId);
                Interface.GetEmptyData(this);
                if (m_Grid.Trace.IsTracing)
                    m_Grid.Trace.Trace("{0} : Finish GetEmptyData({1});", m_Grid.ID, DataSourceId);
            }
            else
            // 16.10.2004, jorn - Fixet slik at denne ikke kjrte om vi allerede hadde kjrt GetEmptyData()
            {
                if (m_Grid.Trace.IsTracing)
                    m_Grid.Trace.Trace("{0} : Start GetData({1},{2});", m_Grid.ID, DataSourceId, forceGetData);
                int oldCurrentPage = m_Grid.PageIndex;
                if (getAllRows)
                    m_Grid.PageIndex = 0;

                if (DataSource != null || m_XmlDataDocument != null)
                    EnumDataSource.LoadDatasource(DataSource, m_DataSourceColumns, false, this);
                else if ((!string.IsNullOrEmpty(DataSourceId) || !string.IsNullOrEmpty(Sql)))
                    DataBaseInterface.GetData(this);

                // Here we're going to check for post back-data...
                // And load values from state in detail view
                if (m_Grid.MasterTable != this)
                    return;

                
                m_Grid.PageIndex = oldCurrentPage;
                m_Grid.m_IsOneToOneRelationNewRecord = false;
                if (m_Grid.Trace.IsTracing) m_Grid.Trace.Trace("{0} : Finish GetData();", m_Grid.ID);

                // Couldn't we find the record we selected? Ooops.
                if (Rows.Count == 0 && m_Grid.DisplayView == DisplayView.Detail)
                    if (!m_Grid.m_IsOneToOneRelationGrid)
                    {
                        m_Grid.SystemMessage.Add(string.Format("{0} ({1})",
                                                               m_Grid.GetSystemMessage("SystemMessage_NoRecord"),
                                                               m_Grid.InternalId));
                        m_Grid.InternalId = null;
                        Interface.GetEmptyData(this);
                    }
                    else
                    {
                        Interface.GetEmptyData(this);
                        m_Grid.m_IsOneToOneRelationNewRecord = true;
                    }
            }

            // This is cool; If we're jumping into detail view, values are auto-populated from last insert... but does it work??
            if (m_Grid.LastMode == DisplayView.Grid && (m_Grid.DisplayView == DisplayView.Detail))
            {
                if (m_Grid.Trace.IsTracing) m_Grid.Trace.Trace("{0} : Start load state();", m_Grid.ID);
                for (int i = 0; i < Columns.Count; i++)
                {
                    if (Columns[i].m_SaveValueToViewState == false ||
                        (Columns[i].Visibility == Visibility.Grid ||
                         Columns[i].Visibility == Visibility.None) ||
                        m_Grid.GetState(string.Format("{0}_value", Columns[i].ColumnId)) == null ||
                        Rows[0].Cells[i].Value != null || (Columns[i].AllowEmpty))
                        continue;
                    Rows[0].Cells[i].Value = m_Grid.GetState(string.Format("{0}_value", Columns[i].ColumnId));
                }
                if (m_Grid.Trace.IsTracing) m_Grid.Trace.Trace("{0} : Finish load state();", m_Grid.ID);
            }
            GetPostBackData();
           
            if (m_Grid.DisplayView != DisplayView.Detail) return;
            for (int i = 0; i < Columns.Count; i++)
            {
                if (Columns[i].m_SaveValueToViewState == false ||
                    Columns[i].Visibility == Visibility.Grid ||
                    Columns[i].Visibility == Visibility.None)
                    continue;
                if (Rows.Count > 0)
                    m_Grid.State(string.Format("{0}_value", Columns[i].ColumnId), Rows[0][i].Value);
            }
        }

        /// <summary>
        /// Gets postback data for cells in this table, even if they have received postback.
        /// </summary>
        public void GetPostBackData()
        {
            // 16.10.2004, jorn - Added support for AllowEditInGrid and more than 1 row.
            if (m_GotPostBackData || !m_Grid.GetColumnsPostBackData)
                return;

             m_GotPostBackData = true;

            if (m_Grid.DisplayView == DisplayView.Grid && m_Grid.AllowEditInGrid == false)
            {
                if (m_Grid.Trace.IsTracing)
                    m_Grid.Trace.Trace(
                        "{0} : Skipped GetPostBackData cause displyview is gridview and no editable cells.", m_Grid.ID);
                return;

            }

            if (m_Grid.Trace.IsTracing) m_Grid.Trace.Trace("{0} : Start GetPostBackData();", m_Grid.ID);

            for (int j = 0; j < Rows.Count; j++)
                for (int i = 0; i < Rows[j].Cells.Count; i++)
                    Rows[j][i].GetCellPostBackData();
            if (m_Grid.Trace.IsTracing) m_Grid.Trace.Trace("{0} : Finish GetPostBackData();", m_Grid.ID);
        }

        /// <summary>
        /// Set XML data document file as dataprovider for WebGrid. WebGrid will automatically insert/update your xml file and detect schema settings for xml data document.
        /// </summary>
        /// <param name="xmlfile">XML data document file (full path for the file is required)</param>
        /// <param name="sectionPath">Name of the section path (Example: "\\configuration\appsettings").</param>
        /// <remarks>
        /// When selecting nodes with child nodes, then WebGrid is using the first selected node as template for loading column settings, and uses
        /// these column settings for updating and inserting nodes into your XML file. If selecting nodes with child nodes you are not able to create new nodes 
        /// (reason for this is your XML document is mapped to WebGrid.) 
        /// </remarks>
        public void SetDataSource(string xmlfile, string sectionPath)
        {
            m_dataSource = new XmlDataDocument();
            if (xmlfile != null && xmlfile.IndexOf("|DataDirectory|", StringComparison.OrdinalIgnoreCase) > -1 &&
                Grid.GotHttpContext)
                xmlfile = xmlfile.Replace("|DataDirectory|", HttpContext.Current.Server.MapPath("~"));
            m_XmlDataDocument = xmlfile;
            m_Xmlxpath = sectionPath;
            DataSourceType = DataSourceControlType.EnumerableDataSource;
        }

        /// <summary>
        /// Triggers cancel events.
        /// </summary>
        internal void Cancel()
        {
            // The column is responsible for saving form element names to view state/etc
            // for use in AfterUpdate.
            for (int i = 0; i < Columns.Count; i++)
                Columns[i].AfterCancel();

            if (m_Grid.MasterWebGrid != null && m_Grid.MasterWebGrid.ActiveMenuSlaveGrid == m_Grid &&
                m_Grid.StayInDetail == false &&
                m_Grid.LastMode != DisplayView.Detail)
            {
                m_Grid.MasterWebGrid.ActiveMenuSlaveGrid = null;
            }
        }

        internal void ClearDataSourceSession()
        {
            if (!Grid.GotHttpContext || DataSource == null)
                return;
            if (HttpContext.Current.Session[DataSource.GetHashCode() + "_Datasourcedotnet"] != null)
                HttpContext.Current.Session.Remove(DataSource.GetHashCode() + "_Datasourcedotnet");
            if (HttpContext.Current.Session[DataSource.GetHashCode() + "_DataTableSettings"] != null)
                HttpContext.Current.Session.Remove(DataSource.GetHashCode() + "_DataTableSettings");
            if (m_Grid.Debug)
                m_Grid.m_DebugString.AppendFormat("<b>{0}:</b> Cleared Data source and data source settings (by 'ClearDataSourceSession' method.<br/>", m_Grid.ID);
        }

        /// <summary>
        /// Deletes the row.
        /// </summary>
        /// <param name="rowID">The row ID.</param>
        internal void DeleteRow(string rowID)
        {
            if (this == m_Grid.MasterTable)
                if (m_Grid.BeforeDeleteEvent(rowID) == false)
                    return;
            if (string.IsNullOrEmpty(rowID))
                throw new GridException(
                    "A empty or null row identifier was passed to DeleteRow method. Make sure WebGrid has a primary key.");
            bool res = Interface.DeleteRow(this, rowID);
            if (res)
                m_Grid.ReLoadData = true;
            if (this == m_Grid.MasterTable)
                m_Grid.AfterDeleteEvent(rowID);
            if (res)
                m_Grid.AddClientNotificationMessage(m_Grid.GetSystemMessage("ClientNotification_Delete"));
        }

        /// <summary>
        /// Gets the data source schema for this table.
        /// </summary>
        internal void GetSchema()
        {
            //  if (!datasourceloaded || m_Grid.ReLoadData)
            if (m_GotSchema || m_Grid.SystemMessage.CriticalCount > 0 ||
                m_Grid.m_EventRanInit == false && this == m_Grid.MasterTable)
                return;

            m_GotSchema = true;
            LoadDataSourceControl(m_DataSourceId);

            // Load cached structure?
            if (CacheDatasourceStructure && DataBaseInterface.ValidCacheStructure(this) && HttpRuntime.Cache.Get(
                   string.Format("{0}", CacheKey)) != null)
            {
                if (m_Grid.Trace.IsTracing) m_Grid.Trace.Trace(
                        string.Format("Table - GetSchema from Cache: {0} DataSource: {1} ", m_Grid.ID,
                                      DataSourceId));
                RetrieveColumnCache();
                RetreiveColumnsSession();
            }
            else
            {

                RetreiveColumnsSession();
                if (string.IsNullOrEmpty(DataSourceId) && (m_DataSourceColumns == null && DataSource == null))
                    return;

                if (string.IsNullOrEmpty(m_Grid.Sql) == false && m_Grid.DatabaseConnectionType != DataBaseConnectionType.OleDB)
                    throw new GridException(string.Format("{0} : Grid 'Sql' property is only supported by connection string of provider type OleDb",m_Grid.ID));

                if (m_Grid.Trace.IsTracing) m_Grid.Trace.Trace("{0} : Start GetSchema({1});", m_Grid.ID, DataSourceId);

                if (!string.IsNullOrEmpty(DataSourceId) && DataSource == null && !string.IsNullOrEmpty(ConnectionString))
                    DataBaseInterface.GetSchema(this, RetrieveForeignkeys);
                if ( DataSource != null || m_DataSourceColumns != null)
                    EnumDataSource.LoadDatasource(DataSource, m_DataSourceColumns, true, this);
            }
            if (m_Grid.SystemMessage.CriticalCount > 0)
                return;

            for (int i = 0; i < Columns.Count; i++)
            {
                Columns[i].Grid = m_Grid;
                Columns[i].m_Table = this;
            }

            if (this == m_Grid.MasterTable)
                m_Grid.ProcessSystemColumns();
            else
                SaveDataStructure();

            if (m_Grid.Trace.IsTracing) m_Grid.Trace.Trace("{0} : Finish GetSchema();", m_Grid.ID);
        }

        internal bool Insert(ref string editIndex)
        {
            m_GotData = true;
            if (m_Grid.Trace.IsTracing) m_Grid.Trace.Trace("{0} : (Insert) Start GetEmptyData();", m_Grid.ID);
               // Interface.GetEmptyData(this);

            // Populate foreignkeys to match MasterGrid
            if (m_Grid.MasterWebGrid != null)
            {
                foreach (Column column in m_Grid.MasterWebGrid.MasterTable.Columns.Primarykeys)
                {
                    if (Columns[column.ColumnId] == null || (!Columns[column.ColumnId].Primarykey && (
                     Rows[0][column.ColumnId].Value != null ||
                     !Rows[0].Columns[column.ColumnId].Required)))
                        continue;

                    Rows[0][column.ColumnId].Value =
                        m_Grid.MasterWebGrid.MasterTable.Rows[0][column.ColumnId].Value;
                }
            }

            if (m_Grid.Trace.IsTracing) m_Grid.Trace.Trace("{0} : Finish GetEmptyData();", m_Grid.ID);
            GetPostBackData();

            if (m_Grid.SystemMessage.Count == 0)
            {
                if (Interface.InsertUpdateRow(ref editIndex, Rows[0], true) == false)
                    return false;

                for (int i = 0; i < Columns.Count; i++)
                    Columns[i].AfterUpdate(editIndex, Rows[0][Columns[i].ColumnId]);
            }
            return true;
        }

        internal bool InsertUpdate(ref string editIndex)
        {
            return InsertUpdate(ref editIndex, null);
        }

        internal bool InsertUpdate(ref string editIndex, Row row)
        {
            // 16.10.2004, jorn - Added support for multiple rows and UPDATEs from grid view.

            bool insert = ((row == null && m_Grid.InternalId == null) || (m_Grid.m_IsOneToOneRelationNewRecord));
            if (insert)
            {
                if (m_Grid.Trace.IsTracing) m_Grid.Trace.Trace("{0} : Start Insert()", m_Grid.ID);
                bool result = Insert(ref editIndex);
                if (m_Grid.Trace.IsTracing) m_Grid.Trace.Trace("{0} : Finish Insert({1})", m_Grid.ID, editIndex);
                return result;
            }
            if (m_Grid.Trace.IsTracing)
                m_Grid.Trace.Trace("{0} : Start Update({1})", m_Grid.ID,
                                    (row == null ? editIndex : row.PrimaryKeyValues));

            if (row == null)
                row = Rows[0];

            string res = m_Grid.DisplayView == DisplayView.Detail ? m_Grid.InternalId : row.PrimaryKeyUpdateValues;

            if (m_Grid.SystemMessage.Count == 0)
            {
                if (Interface.InsertUpdateRow(ref res, row, false) == false)
                    return false;
                editIndex = res;

                if (m_Grid.SystemMessage.Count == 0)
                    for (int i = 0; i < row.Columns.Count; i++)
                        row.Columns[i].AfterUpdate(editIndex, row[Columns[i].ColumnId]);
            }
            return true;
        }

        internal void LoadDataSourceControl(string dataSourceId)
        {
            if (m_DataSourceId == null)
                return;

            if (DataSourceType == DataSourceControlType.InternalDataSource)
                return;
            switch (DataSourceType)
            {
                case DataSourceControlType.AccessDataSource:
                    if (!m_Grid.GridDesignMode)
                    {
                        AccessDataSource accessds =
                            (AccessDataSource) Grid.FindControl<Control>(m_Grid.Page, dataSourceId);

                        if (accessds != null)
                        {
                            accessds.Selecting += datacontrol_Selecting;
                            accessds.Select(DataSourceSelectArguments.Empty);

                            OleDbDataAdapter sda = new OleDbDataAdapter(accessds.SelectCommand,
                                                                        accessds.ConnectionString);
                            bool res = ValidateParameters(sda);

                            if (res)
                            {
                                DataTable dt = new DataTable();
                                sda.Fill(dt);
                                sda.Dispose();
                                SetDataSource(dt);
                            }
                            else
                                SetDataSource(new DataTable());

                            m_Grid.ConnectionString = accessds.ConnectionString;
                        }
                    }
                    else
                    {
                        string path = Util.ConnectionString.GetPhysicalPath(m_Grid);
                        if (!string.IsNullOrEmpty(path))
                        {
                            AccessDataSource accessds =
                                (AccessDataSource) Grid.FindControl<Control>(m_Grid.Page, dataSourceId);
                            if (accessds != null && !string.IsNullOrEmpty(accessds.DataFile))
                            {

                                accessds.DataFile = accessds.DataFile.Replace("~", path);

                                accessds.Selecting += datacontrol_Selecting;
                                accessds.Select(DataSourceSelectArguments.Empty);

                                OleDbDataAdapter sda = new OleDbDataAdapter(accessds.SelectCommand,
                                                                            accessds.ConnectionString);
                                bool res = ValidateParameters(sda);

                                if (res)
                                {
                                    DataTable dt = new DataTable();
                                    sda.Fill(dt);
                                    sda.Dispose();
                                    SetDataSource(dt);
                                }
                                else
                                    SetDataSource(new DataTable());

                                m_Grid.ConnectionString = accessds.ConnectionString;

                            }
                            else
                                SetDataSource(new DataTable());
                        }
                        else

                            SetDataSource(new DataTable());
                    }
                    break;
                case DataSourceControlType.SqlDataSource:
                    if (!m_Grid.GridDesignMode)
                    {
                        SqlDataSource sqlds = (SqlDataSource) Grid.FindControl<Control>(m_Grid.Page, dataSourceId);
                        if (sqlds != null)
                        {
                            sqlds.Selecting += datacontrol_Selecting;
                            sqlds.Select(DataSourceSelectArguments.Empty);

                            SqlDataAdapter sda = new SqlDataAdapter(sqlds.SelectCommand, sqlds.ConnectionString);
                            bool res = ValidateParameters(sda);

                            if (res)
                            {
                                DataTable dt = new DataTable();
                                sda.Fill(dt);
                                sda.Dispose();
                                SetDataSource(dt);
                            }
                            else
                                SetDataSource(new DataTable());

                            m_Grid.ConnectionString = sqlds.ConnectionString;
                        }
                    }
                    else
                    {
                        SqlDataSource sqlds = (SqlDataSource) Grid.FindControl<Control>(m_Grid.Page, dataSourceId);
                        if (sqlds != null)
                            if (!string.IsNullOrEmpty(sqlds.ConnectionString))
                            {
                                sqlds.Selecting += datacontrol_Selecting;
                                sqlds.Select(DataSourceSelectArguments.Empty);

                                SqlDataAdapter sda = new SqlDataAdapter(sqlds.SelectCommand, sqlds.ConnectionString);
                                bool res = ValidateParameters(sda);

                                if (res)
                                {
                                    DataTable dt = new DataTable();
                                    sda.Fill(dt);
                                    sda.Dispose();
                                    SetDataSource(dt);
                                }
                                else
                                    SetDataSource(new DataTable());

                                m_Grid.ConnectionString = sqlds.ConnectionString;
                            }
                            else
                                SetDataSource(new DataTable());
                    }
                    break;
                case DataSourceControlType.XmlDataSource:
                    {
                        string datafile;
                        if (m_Grid.GridDesignMode == false)
                        {
                            XmlDataSource xmlds = (XmlDataSource) Grid.FindControl<Control>(m_Grid.Page, dataSourceId);

                            datafile = Grid.GotHttpContext
                                           ? HttpContext.Current.Server.MapPath(xmlds.DataFile)
                                           : xmlds.DataFile;

                            if (!string.IsNullOrEmpty(datafile))
                            {
                                string xpath = xmlds.XPath;
                                SetDataSource(datafile, xpath);
                            }
                            else
                                SetDataSource(new DataTable());
                        }
                        else
                        {
                            string path = Util.ConnectionString.GetPhysicalPath(m_Grid);
                            if (!string.IsNullOrEmpty(path))
                            {
                                XmlDataSource xmlds =
                                    (XmlDataSource) Grid.FindControl<Control>(m_Grid.Page, dataSourceId);
                                datafile = xmlds.DataFile.Replace("~", path);
                                string xpath = xmlds.XPath;
                                SetDataSource(datafile, xpath);
                            }
                            else
                            {
                                SetDataSource(new DataTable());
                            }
                        }
                    }
                    break;
                case DataSourceControlType.ObjectDataSource:
                    if (m_Grid.GridDesignMode == false)
                    {
                        ObjectDataSource objectds =
                            (ObjectDataSource) Grid.FindControl<Control>(m_Grid.Page, dataSourceId);
                        objectds.Selecting += objectds_Selecting;
                        objectds.Selected += objectds_Selected;
                        objectds.Updating += objectds_Updating;
                        objectds.Inserting += objectds_Inserting;
                        objectds.Deleting += objectds_Deleting;
                        SetDataSource((objectds.Select()));
                    }
                    else
                    {
                        ObjectDataSource objectds =
                            (ObjectDataSource) Grid.FindControl<Control>(m_Grid.Page, dataSourceId);
                        objectds.Selecting += objectds_Selecting;
                        try
                        {
                            if (objectds.Select() != null)
                                SetDataSource(objectds.Select());
                            else
                                SetDataSource(new DataTable());
                        }
                        catch (Exception ee)
                        {
                            m_Grid.SystemMessage.Add(
                                string.Format(
                                    "There was an error executing '{0}' in design time. Set 'Debug=true' to display error message.",
                                    dataSourceId));
                            if (m_Grid.Debug)
                                m_Grid.SystemMessage.Add(ee.Message);
                            SetDataSource(new DataTable());
                        }

                    }
                    break;
             #if NET35  
                case DataSourceControlType.LinqDataSource:
                    if (!m_Grid.GridDesignMode)
                    {
                        LinqDataSource linqds = (LinqDataSource)Grid.FindControl<Control>(m_Grid.Page, dataSourceId);
                        if (linqds != null)
                            linqds.Selected += linqds_Selected;
                    }
                    else
                    {
                        LinqDataSource linqds = (LinqDataSource)Grid.FindControl<Control>(m_Grid.Page, dataSourceId);
                        if (linqds != null)
                            linqds.Selected += linqds_Selected;
                    }
                    break;
                case DataSourceControlType.EntityDataSource:
                    if (!m_Grid.GridDesignMode)
                    {
                        EntityDataSource Entityds = (EntityDataSource)Grid.FindControl<Control>(m_Grid.Page, dataSourceId);
                        if (Entityds != null)
                            Entityds.Selected += Entityds_Selected;
                    }
                    else
                    {
                        EntityDataSource Entityds = (EntityDataSource)Grid.FindControl<Control>(m_Grid.Page, dataSourceId);
                        if (Entityds != null)
                            Entityds.Selected += Entityds_Selected;
                    }
                    break;
            #endif
            }
        }

         #if NET35  
          void Entityds_Selected(object sender, EntityDataSourceSelectedEventArgs e)
        {
            if (e.Exception != null) return;
            SetDataSource(e.Results);
            RecordCount = e.TotalRowCount;
        }

        void linqds_Selected(object sender, LinqDataSourceStatusEventArgs e)
        {
            if (e.Exception != null) return;
            SetDataSource(e.Result as IEnumerable);
            RecordCount = e.TotalRowCount;
        }
        #endif

        internal void RetreiveColumnsSession()
        {
            if (!m_Grid.m_BindDataSource || !Grid.GotHttpContext ||
                HttpContext.Current.Session[m_Grid.UniqueID + "_DataTableSettings"] == null)
                return;
            m_DataSourceColumns = HttpContext.Current.Session[m_Grid.UniqueID + "_DataTableSettings"] as DataTable;
            return;
        }

        internal void RetreiveDataSourceSession()
        {
            if (!m_Grid.m_BindDataSource || !Grid.GotHttpContext ||
                HttpContext.Current.Session[m_Grid.UniqueID + "_Datasourcedotnet"] == null)
                return;

            /*m_Grid.m_Datasourcedotnet =*/
            m_dataSource = HttpContext.Current.Session[m_Grid.UniqueID + "_Datasourcedotnet"];
        }

        /// <summary>
        /// Retrieves the column cache.
        /// </summary>
        internal void RetrieveColumnCache()
        {
            try
            {

                if (GotCache || HttpRuntime.Cache.Get(CacheKey) == null)
                    return;

                ColumnCollection cacheData =
                    (ColumnCollection)
                    HttpRuntime.Cache.Get(CacheKey);
                Columns = new ColumnCollection(this);

                cacheData.ForEach(delegate(Column column)
                                      {
                                          if (column == null)
                                              return;
                                          Columns.Add(column.Duplicate());
                                      });

                if (m_Grid.Trace.IsTracing)
                    m_Grid.Trace.Trace("{0} : Started datasource information from cache object: {1}", m_Grid.ClientID,
                                       CacheKey);

                for (int i = 0; i < Columns.Count; i++) // Clear some values
                {

                    switch (Columns[i].ColumnType)
                    {
                        case ColumnType.Foreignkey:
                            if (((Foreignkey) Columns[i]).Table.Columns.Primarykeys == null)
                            {
                                ((Foreignkey) Columns[i]).Table.m_LoadedTableSchema = false;
                                ((Foreignkey) Columns[i]).Table.m_GotData = false;
                                DataBaseInterface.LoadColumns(((Foreignkey) Columns[i]).Table);
                            }
                            Columns[i].m_ColumnType = ColumnType.Foreignkey;
                            break;
                        case ColumnType.ManyToMany:
                            if (
                                ((ManyToMany) Columns[i]).m_ForeignDataSource.Columns.Primarykeys ==
                                null)
                            {
                                ((ManyToMany) Columns[i]).m_ForeignDataSource.m_LoadedTableSchema = false;
                                DataBaseInterface.LoadColumns(((ManyToMany) Columns[i]).m_ForeignDataSource);
                            }
                            Columns[i].m_ColumnType = ColumnType.ManyToMany;
                            ((ManyToMany) Columns[i]).Clear();
                            ((ManyToMany) Columns[i]).ReLoad = true;
                            break;
                        case ColumnType.GridColumn:
                            Columns[i].m_ColumnType = ColumnType.GridColumn;
                            ((GridColumn) Columns[i]).GridObject = null;
                            break;
                        case ColumnType.ColumnTemplate:
                            Control m_templateContainer =
                                m_Grid.FindControl(string.Format("{0}_{1}", m_Grid.ClientID, Columns[i].ColumnId));
                            if (m_templateContainer != null)
                                ((ColumnTemplate) Columns[i]).DetailTemplateContainer = m_templateContainer;
                            break;
                    }
                    Columns[i].Grid = m_Grid;
                    Columns[i].m_Table = this;
                }

                GotCache = true;

                if (m_Grid.Debug)
                    m_Grid.m_DebugString.AppendFormat(
                        "<b>Cache</b> - Loaded datasource information from cache object : {0}<br/>", CacheKey);
                if (m_Grid.Trace.IsTracing)
                    m_Grid.Trace.Trace("{0} : Finished datasource information from cache object: {1}", m_Grid.ClientID,
                                       CacheKey);
            }
            catch (Exception ee)
            {
                throw new GridException(
                    string.Format("Error loading datasource information from cache object: {0} and DataSourceId: {1}",
                                  CacheKey, DataSourceId), ee);
            }
        }

        internal void SaveDataStructure()
        {
            // Save cached structure
            if (!CacheDatasourceStructure || HttpRuntime.Cache.Get(CacheKey) != null ||
                Columns.Primarykeys == null || Columns.Primarykeys.Count == 0
                ||  DataSource != null || string.IsNullOrEmpty(m_Grid.ActiveConnectionString))
                return;

            try
            {
                ColumnCollection cacheTable = new ColumnCollection(this);
                Columns.ForEach(delegate(Column column)
                                    {
                                        Column c = column.Duplicate();
                                        cacheTable.Add(c);
                                    });

                if (DataBaseInterface is SqlConnection)
                {
                    object date =
                        Query.ExecuteScalar("SELECT TOP 1 refdate FROM sysobjects ORDER BY refdate DESC",
                                            m_Grid.ActiveConnectionString);
                    if (date is DateTime)
                        HttpRuntime.Cache[
                            string.Format("{0}_dtm", CacheKey)] = ((DateTime)date).Ticks;
                    else if (m_Grid.Debug)
                        m_Grid.m_DebugString.AppendFormat(
                            "Unable to Get 'refdate' value from sysobjects for Table '{0}'", DataSourceId);

                }
                else if (DataBaseInterface is OleDB)
                {
                    OleDbConnection conn = new OleDbConnection(m_Grid.ActiveConnectionString);
                    conn.Open();

                    DataTable schema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (schema.Rows.Count > 0)
                    {
                        object date = schema.Select(null, "DATE_MODIFIED DESC")[0]["DATE_MODIFIED"];
                        conn.Close();
                        if (date is DateTime)
                            HttpRuntime.Cache[
                                string.Format("{0}_dtm", CacheKey)] = ((DateTime)date).Ticks;
                        else if (m_Grid.Debug)
                            m_Grid.m_DebugString.AppendFormat(
                                "Unable to Get 'DATE_MODIFIED' value from 'GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null)' for Table '{0}'",
                                DataSourceId);

                    }
                }
                // Hash code for current page (aspx/ascx page). Used for ValidPageHashCode property
                HttpRuntime.Cache[m_Grid.Trace.ClientID + m_Grid.ClientID] = m_Grid.Page.GetType().GetHashCode();
                HttpRuntime.Cache[CacheKey] = cacheTable;
                if (m_Grid.Trace.IsTracing)
                    m_Grid.Trace.Trace("Created cache for datasource '{0}' in grid '{1}'", DataSourceId, m_Grid.ID);
            }
            catch (Exception ex)
            {
                throw new GridException(
                    string.Format(
                        "Unable to Save Grid CacheStructure ({0}). Remove this error by 'CacheGridStructure=false'",
                        m_Grid.ID), ex);
            }
        }

        /// <summary>
        /// Set table as data source for WebGrid. 
        /// </summary>
        /// <param name="datasource">Control dataprovider object</param>
        internal void SetDataSource(Control datasource)
        {
            m_dataSource = datasource != null ? (object)datasource : new DataTable();
        }

        /// <summary>
        /// Simple iteration over a non-generic collection
        /// </summary>
        /// <param name="datasource">The datasource.</param>
        internal void SetDataSource(IEnumerable datasource)
        {
            m_dataSource = datasource != null ? Parser.ObtainDataTableFromIEnumerable(datasource) : new DataTable();
        }

        /// <summary>
        /// Set table as data source for WebGrid. 
        /// </summary>
        /// <param name="datasource">Table dataprovider object</param>
        internal void SetDataSource(DataTable datasource)
        {
            ClearDataSourceSession();

            m_dataSource = datasource ?? new DataTable();
        }

        /// <summary>
        /// Set table as data source for WebGrid. 
        /// </summary>
        /// <param name="datasource">table dataprovider object</param>
        /// <param name="columnSettings">Column settings for your data source (Optional).</param>
        internal void SetDataSource(DataTable datasource, DataTable columnSettings)
        {
            ClearDataSourceSession();

            m_dataSource = datasource ?? new DataTable();
            m_DataSourceColumns = columnSettings;
        }

        /// <summary>
        /// Set DataView as data source for WebGrid. 
        /// </summary>
        /// <param name="datasource">DataView dataprovider object</param>
        internal void SetDataSource(DataView datasource)
        {
            ClearDataSourceSession();
            m_dataSource = datasource != null ? (object) datasource : new DataTable();
        }

        /// <summary>
        /// Set DataView as data source for WebGrid. 
        /// </summary>
        /// <param name="datasource">DataView dataprovider object</param>
        /// <param name="columns">Column settings for your data source.</param>
        internal void SetDataSource(DataView datasource, DataTable columns)
        {
            ClearDataSourceSession();
            m_dataSource = datasource != null ? (object) datasource : new DataTable();
            m_DataSourceColumns = columns;
        }

        /// <summary>
        /// Set OleDbDataAdapter as data source for WebGrid. OleDbDataAdapter will automatically insert/update your data source and detect schema settings. 
        /// </summary>
        /// <param name="datasource">OleDbDataAdapter dataprovider object</param>
        internal void SetDataSource(OleDbDataAdapter datasource)
        {
            m_dataSource = datasource;
        }

        /// <summary>
        /// Set OleDbDataAdapter as data source for WebGrid. OleDbDataAdapter will automatically insert/update your data source and detect schema settings. 
        /// </summary>
        /// <param name="datasource">OleDbDataAdapter dataprovider object</param>
        /// <param name="columns">Column settings for your data source (Optional).</param>
        internal void SetDataSource(OleDbDataAdapter datasource, DataTable columns)
        {
            m_dataSource = datasource != null ? (object) datasource : new DataTable();
            m_DataSourceColumns = columns;
        }

        /// <summary>
        /// Represents a strongly typed list of objects that can be accessed by index.
        /// </summary>
        /// <param name="datasource">The datasource.</param>
        internal void SetGenericDataSource(Collection<object> datasource)
        {
            DataTable dt = datasource != null ? Parser.ToDataTable(datasource) : new DataTable();

            m_dataSource = dt;
        }

        internal void UpdateSchema()
        {
            m_GotSchema = false;
            GetSchema();
        }

        private static bool IsGenericCollection(object value)
        {
            var type = value.GetType();
            return type.IsGenericType
                   && typeof(IList<>) == type.GetGenericTypeDefinition();
        }

        private void SetDataSource(DataSet datasource)
        {
            m_dataSource = datasource ?? new DataSet();
        }

        private bool ValidateParameters(DbDataAdapter sda)
        {
            if (dbparam != null && dbparam.Count > 0)
                foreach (DbParameter d in dbparam)
                {
                    //Invalid Select Parameters;
                    if (!d.IsNullable && (d.Value == null || d.Value == DBNull.Value))
                    {
                        sda.SelectCommand.Parameters.Clear();
                        return false;
                    }
                    DbParameter a = sda.SelectCommand.CreateParameter();

                    a.DbType = d.DbType;
                    a.ParameterName = d.ParameterName;
                    a.Value = d.Value;
                    a.Direction = d.Direction;
                    sda.SelectCommand.Parameters.Add(a);
                }
            return true;
        }

        void datacontrol_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
            e.Cancel = true;
            dbparam = e.Command.Parameters;
        }

        private void objectds_Deleting(object sender, ObjectDataSourceMethodEventArgs e)
        {
            Parser.SetIOrderedDictionaryFromDataRow(Rows[0].DataRow, e.InputParameters);
        }

        void objectds_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
        {
            Parser.SetIOrderedDictionaryFromDataRow(Rows[0].DataRow, e.InputParameters);
        }

        void objectds_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            RecordCount = 0;
            if (e.ReturnValue == null) return;
            if (e.ReturnValue is int)
                RecordCount = (int)e.ReturnValue;
            else if (e.ReturnValue is ICollection)
                RecordCount = ((ICollection)e.ReturnValue).Count;
        }

        private void objectds_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.Arguments.MaximumRows = m_Grid.PageSize;
            e.Arguments.StartRowIndex = (m_Grid.PageIndex - 1)*m_Grid.PageSize;

            if (!e.ExecutingSelectCount)
            {
                e.Arguments.RetrieveTotalRowCount = true;
                e.Arguments.SortExpression = m_Grid.SortExpression.Replace("[", string.Empty).Replace("]", string.Empty);
            }
            else
            {
                IsCustomPaging = true;
            }
        }

        private void objectds_Updating(object sender, ObjectDataSourceMethodEventArgs e)
        {
            Parser.SetIOrderedDictionaryFromDataRow(Rows[0].DataRow, e.InputParameters);
        }

        #endregion Methods
    }
}