/*
Copyright ©  Olav Christian Botterli.
Dual licensed under the MIT or GPL Version 2 licenses.
http://www.webgrid.com
Date: 30.08.2011, Norway.
*/



namespace WebGrid
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.UI;
    using Control = System.Web.UI.Control;
    using HtmlTextWriter = System.Web.UI.HtmlTextWriter;
    using System.Web.UI.WebControls;
    using DataGrid = System.Web.UI.WebControls.DataGrid;

    using Collections;
    using Data;
    using Table = Data.Table;
    using Design;
    using Help = Design.Help;
    using Enums;
    using Events;
    using Sentences;
    using Util;

    /// <summary>
    /// <see cref="WebGrid.Grid">WebGrid.Grid</see> web control creates browser-friendly HTML and uses (CSS) Cascading GridStyle Sheet for the layout.
    /// WebGrid can display nearly all kinds of database records from a data source by setting
    /// a few properties and you create advanced application features by using the existing
    /// events (<see cref="WebGrid.Events" />) Webgrid contains.
    /// </summary>
    /// <remarks>
    /// WebGrid is is using Anthem.NET which is a FREE, cross-browser AJAX toolkit for the ASP.NET development environment 
    /// that works with both ASP.NET 1.1 and 2.0. Please look at WebGrid starter kit for more information about using Anthem.NET
    /// </remarks>
    [ParseChildren(true),
    PersistChildren(false)]
    [ToolboxData("<{0}:Grid runat=\"server\"></{0}:Grid>")]
    [Description(
            @"WebGrid.Grid web control creates browser-friendly XHTML and uses (CSS) Cascading GridStyle Sheet for the layout. WebGrid can display nearly all kinds of database records from a datasource by setting a few properties and you create advanced application features by using the existing events (WebGrid.Events) Webgrid contains."
            )]
    [Designer(typeof (WebGridDesignTime))]
    [ControlBuilder(typeof (WebGridControlBuilder))]
    [ToolboxBitmap(typeof (DataGrid))]
    [Guid("915834D5-4D27-40de-8D65-2D3191537BC4")]
    [Serializable]
    public partial class Grid : WebControl, IPostBackEventHandler, INamingContainer, Anthem.ICallBackControl, Anthem.IUpdatableControl, Anthem.ICallBackContainerControl
    {
        #region Fields

          //  internal readonly Panel m_SystemMessagePanel = new Panel();
        internal static readonly Regex m_Whitespaceregexp = new Regex(@"\s+");

        // Contains script and styles to be added into the header.
        private readonly StringBuilder m_Scriptandstyles = new StringBuilder(string.Empty);

        /// <summary>
        /// Indicates we are using 'SQL' property instead of 'DataSourceId'.
        /// </summary>
        internal const string DATASOURCEID_NULL = "WG_NULLDB";

        /// <summary>
        /// This constant is used for new records
        /// </summary>
        internal const string EMPTYSTRINGCONSTANT = "[newrecord]";

        /// <summary>
        /// Data Source alias for null values
        /// </summary>
        internal const string NULLCONSTANT = "NULL";

        /// <summary>
        /// Data Source alias for false values (OleDB and SQL)
        /// </summary>
        internal const string STRINGFALSE = "0";

        /// <summary>
        /// Data Source alias for true values (OleDB and SQL)
        /// </summary>
        internal const string STRINGTRUE = "1";
        internal const string ToolbarDetailBottom = "<table cellspacing=\"0\" width=\"100%\" cellpadding=\"0\"><tr><td  align=\"left\">[UPDATE] [CLOSEWINDOW]</td></tr></table>";
        internal const string ToolbarGridBottom = 
            "<table cellspacing=\"0\" width=\"100%\" cellpadding=\"0\"><tr valign=\"middle\"><td align=\"left\">[PAGER]</td><td align=\"right\">[PAGERSTATUS]</td></tr><tr valign=\"middle\"><td>[NEWRECORD] [UPDATEROWS] [CLOSEWINDOW]</td><td align=\"right\">[SEARCHFIELD]</td></tr></table>";

        /// <summary>
        /// Default Client notification settings.
        /// </summary>
        internal static readonly ClientNotification DefaultClientNotification = new ClientNotification();

        /// <summary>
        /// JQuery UI hover effect on buttons.
        /// </summary>
        internal const string HoverButtonScript = @"  $(document).ready(function() {
            $('.ui-button').hover(function() {
                $(this).addClass(""ui-state-hover"");
            },
                function() {
                    $(this).removeClass(""ui-state-hover"");
                }
        ).mousedown(function() {
            $(this).addClass(""ui-state-active"");
        })
        .mouseup(function() {
            $(this).removeClass(""ui-state-active"");
        });
        });";

        //  private GridConfig m_AppConfig = new GridConfig();
        internal StringBuilder m_DebugString;
        internal bool m_EventRanDoRender;
        internal bool m_EventRanInit;
        private bool m_EventRanLoad;
        internal bool m_EventRanpreRender;
        internal bool m_HasGridInsideGrid;
        internal bool m_HasRendered;

        /// <summary>
        /// Indicates whether this instance of Webgrid is inside another grid.
        /// </summary>
        internal bool m_IsGridInsideGrid;

        /// <summary>
        /// Indicates whether this instance of Webgrid is member of a one to one relation.
        /// </summary>
        internal bool m_IsOneToOneRelationGrid;

        /// <summary>
        /// Indicates whether this instance of Webgrid is member of a one to one relation, and the 
        /// realation does not exist.
        /// </summary>
        internal bool m_IsOneToOneRelationNewRecord;

        private bool? m_IsUsingJQueryUICSSFramework;
        

        /// <summary>
        /// Rows added
        /// </summary>
        internal int m_RowsAdded;
        internal bool? m_StoredAllowEditInGrid;

        private readonly DesignRender m_DesignRender; // <-- takes care of rendering
        private readonly GridTrace m_GridTrace;
        private readonly StringCollection m_GridsInGrid = new StringCollection();

        /// <summary>
        /// Used for modular poup.
        /// </summary>
        private const string DetailPopupKeys = "DetailPopupKeys";

        /// <summary>
        /// Sets or gets if this is a demo WebGrid.
        /// </summary>
        private const bool IsDemo = false;

        /// <summary>
        /// WebGrid  CallBack loading message.
        /// </summary>
        private const string WebGridCallBackLoadText = "Webgrid is loading...";

        private static bool? m_GotHttpContext;

        private string m_ActiveConnectionString;
        private PlaceHolder m_AjaxLoaderTemplate;
        private string m_CallBackLoadingText = WebGridCallBackLoadText;
        private bool? m_CollapseIfSlaveGrid;
        private string m_ConnectionString;
        private GridSystemMessageCollection m_CustomSystemMessages = new GridSystemMessageCollection();
        private DataBaseConnectionType m_DbConnectionType = DataBaseConnectionType.Auto;
        private bool m_Distinct;
        private PlaceHolder m_EmptyDataTemplate;
        private bool? m_HideIfemptyGrid;
        private Pager m_PagerSettings;
        private bool m_ShouldSerializeConnectionString;
        private string m_Sql;
        private NameValueCollection m_Tag = new NameValueCollection();
        internal Dictionary<string, GroupInfo> m_groupState;

        /*
        private GridModeSettings gridModeSettings = null;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Description("Help text (Displayed in grid help icon)")]
        [NotifyParentProperty(true)]
        [MergableProperty(false)]
        [Category("Navigation")]

        internal GridModeSettings GridModeSettings
        {
            get { return gridModeSettings; }
            set
            {
                gridModeSettings = value;

            }
        }

        private EditModeSettings editModeSettings = null;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Description("Help text (Displayed in grid help icon)")]
        [NotifyParentProperty(true)]
        [MergableProperty(false)]
        internal EditModeSettings EditModeSettings
        {
            get { return editModeSettings; }
            set
            {
                editModeSettings = value;

            }
        }*/
        private PlaceHolder m_helptext_public;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Grid"/> class.
        /// </summary>
        public Grid()
        {
           
            m_DebugString = new StringBuilder(string.Empty);
            m_SystemMessages = new SystemMessageCollection(this, SystemMessageStyle.WebGrid);
            MasterTable = new Table(this);
            m_DesignRender = new DesignRender(this);
            m_GridTrace = new GridTrace(this);
            m_PagerSettings = new Pager(this);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// A collection of the columns. A collection of columns is typically used by
        /// <see cref="Row">WebGrid.Row</see> as a placeholder for the fields of a recordset from a
        /// data source.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TypeConverter(typeof(WebGridColumnCollectionConverter))]
        [Editor(typeof(WebGridColumnCollectionEditor), typeof(UITypeEditor))]
        [Description("Add, remove, and edit columns for this WebGrid grid instance")]
        [NotifyParentProperty(true)]
        [MergableProperty(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public ColumnCollection Columns
        {
            get
            {
                if (m_EventRanInit && IsDesignTime && !IsDesignTimeRender)
                {
                    Table tmptable = new Table(this);
                    ColumnCollection tmpcol = new ColumnCollection(tmptable);
                    foreach (Column mycolumn in MasterTable.Columns)
                    {
                        if (mycolumn.IsCreatedByWebGrid)
                            continue;

                        tmpcol.Add(mycolumn);
                     }
                    return tmpcol;
                }

                return MasterTable.Columns;
            }
        }

        ///<summary>
        /// Gets if WebGrid are using JQuery UI CSS Framework.
        ///</summary>
        public bool IsUsingJQueryUICSSFramework
        {
            get {

                if (m_IsUsingJQueryUICSSFramework == null)
                    m_IsUsingJQueryUICSSFramework = SkinType != SkinType.Disabled;
                return (bool) m_IsUsingJQueryUICSSFramework;

            }
           
        }

        // Primarykeys for this column collections
        /// <summary>
        /// Gets a Column vollection of all primary keys for this Table
        /// </summary>
        [Browsable(false)]
        public List<Column> Primarykeys
        {
            get
            {
                return MasterTable != null ? MasterTable.Columns.Primarykeys : null;
            }
        }

        /// <summary>
        /// Gets or sets the rows for this grid (Rows are retreived from MasterTable for the grid)
        /// </summary>
        /// <remarks>
        /// The MasterTable is same datasource as the property <see cref="DataSourceId"/> or <see cref="DataSource"/>
        /// </remarks>
        [Browsable(false)]
        public RowCollection Rows
        {
            get
            {
                return MasterTable.Rows;
            }
        }

        internal static bool GotHttpContext
        {
            get
            {
                if (m_GotHttpContext == null)
                    m_GotHttpContext = HttpContext.Current != null && HttpContext.Current.Request != null && HttpContext.Current.Session != null;
                return (bool) m_GotHttpContext;
            }
        }

        internal GridTrace Trace
        {
            get { return m_GridTrace; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Outputs server control content to a provided HtmlTextWriter object and stores tracing information about the control if tracing is enabled.
        /// </summary>
        /// <param name="writer">The HtmlTextWriter object that receives the control content.</param>
        public override void RenderControl(HtmlTextWriter writer)
        {
            if (EnableCallBack)
                base.Visible = true;
            Render(writer);
        }

        /// <summary>
        /// Executed for each row being rendered.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columns"></param>
        internal bool BeforeRowRenderEvent(ref Row row, ref ColumnCollection columns)
        {
            if (BeforeRowRender == null)
            {

                return true;
            }
            BeforeRenderRowArgs ea = new BeforeRenderRowArgs
                                         {
                                             Row = row,
                                             EditIndex = row.PrimaryKeyValues,
                                             Columns = columns
                                         };
            BeforeRowRender(this, ea);
            if (ea.AcceptChanges)
            {
                row = ea.Row;
                columns = ea.Columns;
            }
            return ea.RenderThisRow;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            // Avoid Bubble events.
            if (m_EventRanInit)
                return;

            base.OnInit(e);
            if (ID == null)
                throw new ApplicationException("WebGrid.Grid instance does not have a value in the required 'ID' property");

            #region  Setup Aspx Columns

            bool bakIsDesignTimeRender = IsDesignTimeRender;
            IsDesignTimeRender = true;
            int i = Columns.Count - 1;

            while (i >= 0)
            {
                Columns[i].Grid = this;
                Columns[i].m_Table = MasterTable;
                Columns[i].IsCreatedByWebGrid = false;
                switch (Columns[i].ColumnType)
                {
                    case ColumnType.ColumnTemplate:
                        if (((ColumnTemplate) Columns[i]).DetailViewTemplate != null)
                        {
                            Control m_templateContainer = new Control
                                                              {
                                                                  ID =
                                                                      string.Format("{0}_{1}", ClientID,
                                                                                    Columns[i].ColumnId)
                                                              };
                            ((ColumnTemplate) Columns[i]).DetailViewTemplate.InstantiateIn(m_templateContainer);
                            m_templateContainer.Visible = false;
                            Controls.Add(m_templateContainer);
                            ((ColumnTemplate) Columns[i]).DetailTemplateContainer = m_templateContainer;
                        }
                        break;
                    case ColumnType.ManyToMany:
                        if (((ManyToMany) Columns[i]).m_ForeignDataSource.m_Grid == null)
                        {
                            ((ManyToMany) Columns[i]).m_ForeignDataSource.m_Grid = this;
                            Tables.Add(((ManyToMany) Columns[i]).m_ForeignDataSource);
                        }
                        break;
                    case ColumnType.GridColumn:
                        if (((GridColumn) Columns[i]).GridObject != null)
                        {

                            ((GridColumn) Columns[i]).GridObject.MasterGrid = ID;
                            ((GridColumn) Columns[i]).GridObject.m_IsGridInsideGrid = true;
                            m_GridsInGrid.Add(((GridColumn) Columns[i]).GridObject.ID);

                        }
                        break;
                    case ColumnType.Decimal:

                        if (((Decimal) Columns[i]).DisplayTotalSummary &&
                            string.IsNullOrEmpty(((Decimal) Columns[i]).Sum))
                            ((Decimal) Columns[i]).Sum = string.Format("[{0}]", Columns[i].ColumnId);

                        break;
                }
                if (string.IsNullOrEmpty(Columns[i].ColumnId))
                {
                    if (SystemMessage.CriticalCount == 0)
                        SystemMessage.Add("One or more columns has not specified ColumnId property.", true);
                }
                i--;
            }
            IsDesignTimeRender = bakIsDesignTimeRender;

            #endregion

            m_EventRanInit = true;

            #region HttpContent requests

            if (GotHttpContext && Page != null)
            {
                if (HttpContext.Current.Request["DateFilter"] != null)
                {
                    SystemMessage.Add("This feature is not supported in WebGrid 3.0");
                    return;
                }

                if (HttpContext.Current.Request["WGChart"] != null)
                {
                    System.Drawing.Image tomemory = (System.Drawing.Image)HttpContext.Current.Session[HttpContext.Current.Request["WGChart"]];
                    if (tomemory != null)
                    {
                        Page.Response.Clear();
                        Page.Response.ContentType = "image/png";
                        MemoryStream memory = new MemoryStream();
                        tomemory.Save(memory, ImageFormat.Png);
                        memory.WriteTo(Page.Response.OutputStream);
                        Page.Response.End();
                        return;
                    }
                }
            }

            #endregion

            if (ID.StartsWith("_") || ID.IndexOf(" ") > 0)
                throw new GridException(GetSystemMessage("SystemMessage_Editor_noValidID"));

            if (Trace.IsTracing)
                Trace.Trace("{0} : Start OnInit();", ID);

            Tables.Add(MasterTable);

            if (Trace.IsTracing)
                Trace.Trace("{0} : Finish OnInit();", ID);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"></see> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            // Bubble effect..
            if (m_EventRanInit == false)
                OnInit(new EventArgs());
            if (m_EventRanLoad)
                return;

            #region Display HelpText

            if (GotHttpContext && HttpContext.Current.Request != null)
            {
                if (EditIndexRequest != null && !Page.IsPostBack && HttpContext.Current.Request[EditIndexRequest] != null)
                {
                    EditIndex = HttpContext.Current.Request[EditIndexRequest];
                    DisplayView = DisplayView.Detail;
                }
                else if (HttpContext.Current.Request[DetailPopupKeys] != null)
                {
                    EditIndex = HttpContext.Current.Request[DetailPopupKeys];
                    DisplayView = DisplayView.Detail;
                }

                if (HttpContext.Current.Request["ShowHelp"] != null)
                {
                    Page.Response.Clear();

                    Page.Response.Write("<html><head><title>WebGrid Help</title></head>");

                    string csslink = string.Format("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />",
                                                   Page.ClientScript.GetWebResourceUrl(GetType(),
                                                                                       "WebGrid.Resources.jquery-ui-smoothness.css"
                                                                                           .Replace("&",
                                                                                                    "&amp;")));
                    Page.Response.Write(csslink);

                    Page.Response.Write("<body style=\"background-color:White;\">");
                    PlaceHolder helptext = null;

                    if (m_helptext_public != null)
                        helptext = m_helptext_public;
                    else
                    {
                        Grid grid = FindGrid(Page, HttpContext.Current.Request["gridid"]);
                        if (grid != null)
                            helptext = grid.HelpText;
                    }

                    if (helptext != null)
                    {

                        StringBuilder sb = new StringBuilder();
                        StringWriter sw = new StringWriter(sb);
                        HtmlTextWriter controlwriter = new HtmlTextWriter(sw);
                        helptext.RenderControl(controlwriter);
                        Page.Response.Write(sb.ToString());
                        sw.Close();
                        sw.Dispose();
                    }
                    else
                        Page.Response.Write(Help.GenerateHelp(ImagePath));
                    Page.Response.Write("</body></html>");
                    Page.Response.End();
                    return;
                }

                Util.MultiSelect.SelectableRows.GetSelectedRows(this);

                // DB Image?
                if (HttpContext.Current.Request["wgdbimgeditorid"] != null && HttpContext.Current.Request["wgdbimgeditorid"] == ClientID)
                {
                    DBImage.Render(this);
                    if (SystemMessage.Count == 0)
                    {
                        Visible = false;
                        return;
                    }
                }
            }

            #endregion

            m_EventRanLoad = true;

            if (Trace.IsTracing)
                Trace.Trace("{0} : Start OnLoad();", ID);

            if (Trace.IsTracing)
                Trace.Trace("base.OnLoad({0})", ID);
            base.OnLoad(e);

            if (EnableCallBack)
                if (Page != null && GotHttpContext)
                {
                    Anthem.Manager.Register(this);

                }
                else if (DesignMode == false)
                    EnableCallBack = false;

            if (Trace.IsTracing)
                Trace.Trace("{0} : Finish OnLoad();", ClientID);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            // Bubble effect
            if (Equals(m_EventRanpreRender, true))
                return;
            if (Trace.IsTracing)
                Trace.Trace("Start PreRender()");

            if (m_EventRanLoad == false)
            {
                if (Trace.IsTracing)
                    Trace.Trace("Running onLoad event from onPreRender event");
                OnLoad(new EventArgs());
            }
            m_EventRanpreRender = true;

            if (MasterWebGrid != null && RequiredbyMasterWebGrid() == false)
                return;

            if (Trace.IsTracing)
                Trace.Trace("base.OnPreRender({0})", ID);

            if (m_IsRecordUpdateRows)
            {
                UpdateRowsClickEvent();
                ReLoadData = true;
            }
            else if (m_UpdateRow != null && m_UpdateRow.Length > 0)
            {
                UpdateRowClickEvent(m_UpdateRow);

                if (m_ResetEditIndex)
                    InternalId = null;

                ReLoadData = true;
            }

            base.OnPreRender(e);

            //BUG: Make SubmitGridOnEnter work without '???_wgupdaterecord' and '???_wgupdaterecords'.
            if (UseSubmitBehavior && !DesignMode)
                switch (DisplayView)
                {
                    case DisplayView.Detail:
                        Attributes.Add("onkeydown", string.Format("return SubmitGridOnEnter(event,'{0}_wgupdaterecord');", ID));
                        break;
                    default:
                        Attributes.Add("onkeydown", string.Format("return SubmitGridOnEnter(event,'{0}_wgupdaterecords');", ID));
                        break;
                }
            SetupGridRelations();
            if (CacheGridStructure)
                MasterTable.SaveDataStructure();
            if (ReLoadData)
            {

                //Clean-up
                if (InternalId != null && DisplayView == DisplayView.Grid)
                    InternalId = null;
                if (Debug)
                    m_DebugString.AppendFormat("<b>ReloadingData ({0})</b> -  DataSource: {1}<br/>", ID, DataSourceId);
                MasterTable.GetData(true);

                if (SlaveGrid.Count > 0)
                    SlaveGrid.ForEach(delegate(Grid grid) { grid.ReLoadData = true; });
            }

            ViewState["LastGridMode"] = DisplayView;
            //Saving group state;
            GroupState = GroupState;

            if (Trace.IsTracing)
                Trace.Trace("Start DisableShowRequiredIconIfThereIsNoRequiredFields({0});", ID);
            // Disable DisplayRequiredColumn if there is no required fields in the table
            DisableShowRequiredIconIfThereIsNoRequiredFields();
            if (Trace.IsTracing)
                Trace.Trace("Finished DisableShowRequiredIconIfThereIsNoRequiredFields({0});", ID);

            if (Trace.IsTracing)
                Trace.Trace("Start errors.Render({0});", ID);

             //   SystemMessage.m_Panel = m_SystemMessagePanel;

            // If Data is not loaded yet.. do it before rendering errors..
            if (m_MasterTable.m_GotData == false)
                MasterTable.GetData(false);

            if (IsDemo)
            {

                if (!Security.IsLocal && new Random().Next(1, 7) == 3)
                {
                    SystemMessage.Add(
                        "For full version register at <a href=\"http://www.webgrid.com\"><b>WebGrid Support Account</b></a>.");

                    if (DisplayView == DisplayView.Detail && Page != null && Page.ClientScript != null)
                    {
                        const string alert = "alert('For full version register at www.webgrid.com');";
                        if (EnableCallBack && Anthem.Manager.IsCallBack)
                            Anthem.Manager.AddScriptForClientSideEval(alert);
                        else
                            Page.ClientScript.RegisterClientScriptBlock(GetType(), "AlertScript", alert, true);
                    }
                }
            }

            LoadLanguage();
            SystemMessages.ForEach(delegate(SystemMessageItem key)
                                       {
                                           if (key is OverrideSystemMessage)
                                               SetSystemMessage(
                                                   ((OverrideSystemMessage)key).SystemMessageID.ToString(),
                                                   key.DisplayText,
                                                   key.CssClass);
                                       });

            // This should move in the future.
            //SetupGridFilterByColumns();
            SetupGridMessages();
            SetupGridScripts();

            m_DesignRender.Render();

            if (Trace.IsTracing)
                Trace.Trace("Finished errors.Render({0});", ID);

            UpdateDebug();

            //	bool AllRows = (System.Web.HttpContext.Current.RequestQueryString("AllRows"] != null && System.Web.HttpContext.Current.RequestQueryString("AllRows") == "1");
            if (m_DoExcelExport)
                Excel.StreamToBrowser(Page, MasterTable, ExportFileName, Excel.ExcelFile(MasterTable, true));
            BindDataSourceSession();
            if (Trace.IsTracing)
                Trace.Trace("Finish PreRender. Rows Rendered: {0}", MasterTable.Rows.Count);
        }

        /// <summary>
        /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"></see> object, which writes the content to be rendered on the client.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"></see> object that receives the server control content.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (m_EventRanDoRender)
                return;

            if (m_EventRanpreRender == false)
                OnPreRender(new EventArgs());

            if (Trace.IsTracing)
                Trace.Trace("{0} : Start Render();", ClientID);
            if (EnableCallBack && AddCallBacks && Visible)
                Anthem.Manager.AddCallBacks(this,
                                            EnabledDuringCallBack,
                                            TextDuringCallBack,
                                            PreCallBackFunction,
                                            PostCallBackFunction,
                                            CallBackCancelledFunction);
            if (!DesignMode && EnableCallBack)
                Anthem.Manager.WriteBeginControlMarker(writer, "div", this);

            if (Visible)
                DoRender(writer);

            if (!DesignMode && EnableCallBack)
                Anthem.Manager.WriteEndControlMarker(writer, "div", this);

            if (Trace.IsTracing)
                Trace.Trace(string.Format("{0} : Finish Render();", ID));
        }

        private void CompleteEvent()
        {
            if (Complete != null)
                Complete(this);
        }

        private void DoRender(HtmlTextWriter writer)
        {
            if (m_EventRanInit == false || m_EventRanDoRender || MasterWebGrid != null && RequiredbyMasterWebGrid() == false)
                return;

            m_EventRanDoRender = true;
            if (Trace.IsTracing)
                Trace.Trace(string.Format("{0} : Start RenderControl();", ID));

            // Draw all html stuff
            // base.Render(writer);

            RenderContents(writer);

            if (Equals(Debug, true))
            {
                writer.Write(m_DebugString);
            }
            if (Trace.IsTracing)
                Trace.Trace(string.Format("{0} : Finish RenderControl();", ID));

            CompleteEvent();
        }

        /*
         * if (m_IsGridInsideGrid && MasterWebGrid.EditIndex == null)
                return false;

            if (MasterWebGrid.Visible == false || (MasterWebGrid.DisplaySlaveGridMenu
                                                   &&
                                                   ((MasterWebGrid.ActiveMenuSlaveGrid != null &&
                                                     MasterWebGrid.ActiveMenuSlaveGrid !=
                                                     this || MasterWebGrid.ActiveMenuSlaveGrid == null)) ||
                                                   MasterWebGrid.EditIndex == null || MasterWebGrid.DisplayView != Modes.Edit) &&
                                                  m_IsGridInsideGrid == false)
                return false;
            // OUCH DEEP SEARCH MASTERWEBGRID SEARCH..
            if (MasterWebGrid.MasterWebGrid != null && DeepSearchMasterWebGrid(MasterWebGrid) == false
                && m_IsGridInsideGrid == false)
                return false;
            return true;
         */
        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeCallBackLoadingText()
        {
            return m_CallBackLoadingText != WebGridCallBackLoadText;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeCollapseIfSlaveGrid()
        {
            return m_CollapseIfSlaveGrid != null ;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeDistinct()
        {
            return m_Distinct;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeHideIfEmptyGrid()
        {
            return m_HideIfemptyGrid != null ;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeTag()
        {
            return false;
        }

        #endregion Methods
    }

    [Serializable]
    internal class GroupInfo
    {

         [NonSerialized]
        public string AciveGroupValue;

         public Dictionary<string, GroupDetails> List = new Dictionary<string, GroupDetails>();
    }
    [Serializable]
    internal class GroupDetails
    {
        public bool IsExpanded = true;
        public string GroupSqlFilter;
    }

    /// <summary>
    /// WebGrid Datagrid and database tool for ASP.NET by Olav Christian Botterli.
    /// WebGrid has an easy-to-use DataGrid and extended database tools for web applications and data source usage.
    /// <see cref="WebGrid.Grid">WebGrid.Grid</see> is a web control to display and edit data-tables from a data source , and the
    /// namespace <see cref="WebGrid.Util" /> contains developer tools to fast create advanced features for your applications.
    /// <list type="table">
    ///  <listheader>
    ///  <term>Getting Started</term>
    ///  <description>Description</description>
    ///	 </listheader>
    ///  <item><term><see cref="WebGrid.Grid">WebGrid.Grid</see></term><description>Before getting started using WebGrid learn what WebGrid can do for you with it's properties.</description></item>
    ///  <item><term><see cref="WebGrid.Column">WebGrid.Column</see></term><description>See all the column types (data types) WebGrid support and what properties they possess.</description></item>
    ///  <item><term><see cref="WebGrid.Events">WebGrid.Events</see></term><description>See what events you can use on <see cref="WebGrid.Grid">WebGrid.Grid</see> and see how you can have full control over your data source is being rendered to the web page.</description></item>
    ///  <item><term><see cref="InsertUpdate">InsertUpdate</see></term><description>This is a powerful and easy-to-use tool to update your data source in a secure and easy way. (Recommended!)</description></item>
    ///  <item><term><see cref="Query">Query</see></term><description>This is a powerful and easy-to-use tool to query your data source in a secure and easy way. (Recommended!)</description></item>
    /// </list>
    /// </summary>
    internal class NamespaceDoc
    {
    }
}