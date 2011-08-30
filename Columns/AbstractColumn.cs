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

namespace WebGrid
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Drawing;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using Data;
    using Data.Database;
    using Table = Data.Table;
    using Design;
    using HtmlTextWriter = Design.WebGridHtmlWriter;
    using Enums;
    using Events;
    using Util;
    using Util.Json;

    /// <summary>
    /// When <see cref="WebGrid.Grid">WebGrid.Grid</see> web control is displaying the selected fields from
    /// a data source it uses <see cref="WebGrid.Column">Column</see> to render the content of these
    /// fields. Properties, methods, and <see cref="WebGrid.Column.ColumnType" >ColumnType</see>
    /// set how the column should behave and be displayed. By default the <see cref="WebGrid.Column.ColumnType" >ColumnType</see>
    /// is automatically detected and optimal properties are set for this <see cref="WebGrid.Column.ColumnType" >ColumnType</see>
    /// to create a userfriendly web-interface.
    /// </summary>
    public abstract class Column : IComponent, ICloneable
    {
        #region Fields

        internal bool IsCreatedByWebGrid;
        internal bool? m_AllowEdit;
        internal bool? m_AllowUpdate;
        internal int m_ColumnIndex = -2;
        internal ColumnType m_ColumnType;
        internal string m_DataSourceId;
        internal Table m_DataSourceTableId;
        internal object m_DbDefaultValue;
        internal bool m_IsFormElement = true;

        //Used to keep column value (applies only to editable columns)
        internal bool m_SaveValueToViewState;
        internal bool? m_Searchable;
        internal bool? m_Sortable;
        internal bool? m_Groupable;
        //Data.Table for this Column
        internal Table m_Table;
        internal int m_TreeLevel;

        private bool? m_AllowEditInGrid;
        private bool m_AllowEditset;
        private bool? m_AllowEmpty;
        private bool m_AllowEmptyset;
        private bool m_AllowUpdateSet;
        private string m_Attributes;
        private bool m_AutoPostback;
       
        private string m_Columnname;
        private ISite m_Columnsite;
        private string m_ConfirmMessage;
        private string m_Cssclass;
        private string m_CssclassTitle;
        string m_DataSourcePrimaryKeys;
        private string m_DefaultDataSourceId;
        private int m_DisplayIndex = -1;
        private HorizontalPosition m_EditAlign = HorizontalPosition.Undefined;
        private HorizontalPosition m_EditAlignTitle = HorizontalPosition.Undefined;
        private VerticalPosition m_EditVAlign = VerticalPosition.middle;
        private bool m_EnableFilterByColumn;
        private bool m_ForcePostBack;
        private Grid m_Grid;
        private HorizontalPosition m_GridAlign = HorizontalPosition.Undefined;
        private bool? m_GridModeEditTransparent;
        private VerticalPosition m_GridVAlign = VerticalPosition.undefined;
        private string m_HeaderText;
        private Unit m_HeightEditableColumn;
        private bool m_HideDetailTitle;
        private bool m_HideIfEmpty;
        private bool m_HtmlEncode;
        private bool? m_HyperLinkColumn;
        private bool m_Identity;
        private bool? m_IsInDataSource;
        private bool m_LoadColumnDataSourceData;
        private bool m_DisableMaskedInput;
        private string m_MaskedInput;
        private int m_MaxSize;
        private int m_MinSize;
        private bool m_NewRowInDetail = true;
        private bool m_NewRowInGrid;
        private bool? m_NonBreaking;
        private string m_NullText;
        private string m_PostDetailText;
        private string m_PostGridText;

        //  private bool? m_AllowClientHide ;
        //     private bool? m_AllowClientResize  ;
        private string m_PreDetailText;
        private string m_PreGridText;
        private bool? m_Primarykey;
        private bool m_Primarykeybywebgrid;
        private bool? m_Required;
        private int m_Rowspan = 1;
        private NameValueCollection m_FilterByColumn = new NameValueCollection();
        private bool m_Setvisibility;
        private string m_SystemMessage;
        private SystemMessageStyle m_SystemMessageStyle = SystemMessageStyle.Undefined;
        private string m_Tag;
        private int m_Texttruncate;
        private string m_Title;
        private string m_ToolTipInput;
        private string m_TooltipEditTitle;
        private string m_TooltipGridHeaderTitle;
        private string m_TreeIndentText = "&nbsp;&nbsp;&nbsp;";
        private TypeCode m_TypeCode;
        private bool? m_UniqueValueRequired;
        private bool? m_UseAllRows;
        private string m_ValidExpression;
        private Visibility m_Visibility = Visibility.Undefined;
        private Unit m_WidthColumnHeaderTitle;
        private Unit m_WidthColumnTitle;
        private Unit m_WidthEditableColumn;
        private string m_defaultValue;
        bool? shouldShow;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Column">Column</see> class.
        /// </summary>
        internal Column()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Column">Column</see> class.
        /// </summary>
        /// <param name="columnName">ColumnId of the column.</param>
        /// <param name="table">Table object.</param>
        protected Column(string columnName, Table table)
        {
            Grid = table.m_Grid;
            m_Table = table;
            ColumnId = columnName;
            DisplayIndex = Grid.MasterTable.Columns.Count*10 + 10;
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Adds an event handler to listen to the Disposed event on the component.
        /// </summary>
        public event EventHandler Disposed;

        #endregion Events

        #region Properties

        /// <summary>
        /// Sets or gets whether column edit is allowed. True by default.
        /// </summary>
        /// <value><c>true</c> if [allow edit]; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// Identity and primary keys are not editable by default.
        /// </remarks>
        [Browsable(true),
        Category("Behavior"),
        Description(
             @"Sets or gets whether column edit is allowed. True by default if not Identity or primarykey.")]
        public bool AllowEdit
        {
            get
            {
                if (m_Grid.InternalId != null && AllowUpdate == false && !m_Grid.m_IsCopyClick)
                    m_AllowEdit = false;
                if (m_AllowEdit != null)
                    return (bool) m_AllowEdit;

                // Identity and Primarykeys are not allowed edited by default.
                if (Primarykey && m_AllowEdit == null || Identity)
                    m_AllowEdit = Primarykey && Identity == false && m_Grid.DisplayView == DisplayView.Detail &&
                                  m_Grid.InternalId == null;

                if (m_Grid.InternalId != null && AllowUpdate == false && !m_Grid.m_IsCopyClick)
                    m_AllowEdit = false;
                else if (AllowEditInGrid)
                    m_AllowEdit = true;
                else if (Grid != null)
                    m_AllowEdit = Grid.AllowEdit;
                return TristateBoolType.ToBool(m_AllowEdit, true);
            }
            set
            {
                m_AllowEdit = value;
                m_AllowEditset = true;
            }
        }

        /// <summary>
        /// Indicates whether this column is allowing grid-mode editing. This property is useful if you want to edit several data-records at once.
        /// </summary>
        /// <value><c>true</c> if [grid view edit]; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// Default errorStyle is ColumnType (systemMessage message is shown in the column) if this property is set true.
        /// </remarks>
        [Browsable(true),
        Category("Behavior"),
        Description(
             @"Indicates whether this column is allowing grid-mode editing. This property is useful if you want to edit several data-records at once."
             )]
        public bool AllowEditInGrid
        {
            get { return TristateBoolType.ToBool(m_AllowEditInGrid, false); }
            set
            {
                 m_AllowEditInGrid = value;
            }
        }

        /// <summary>
        /// Sets or Gets whether the column accepts 'null' (nothing) as a value.
        /// </summary>
        /// <value><c>true</c> if [allow null]; otherwise, <c>false</c>.</value>
        [Browsable(true),
        Category("Behavior"),
        Description(@"Sets or Gets whether the column accepts 'null' (nothing) as a value.")]
        public bool AllowEmpty
        {
            get { return TristateBoolType.ToBool(m_AllowEmpty, true); }
            set
            {
                m_AllowEmpty = value;
                m_AllowEmptyset = true;
            }
        }

        /// <summary>
        /// Sets or gets whether column updating is allowed. True by default.
        /// </summary>
        /// <value><c>true</c> if [allow edit]; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// Identity and primary keys are not updatable by default.
        /// </remarks>
        [Browsable(true),
        Category("Behavior"),
        Description(
             @"Sets or gets whether column updating is allowed. True by default if not Identity or primarykey column."
             )]
        public bool AllowUpdate
        {
            get
            {
                if (m_AllowUpdate != null)
                    return TristateBoolType.ToBool(m_AllowUpdate, true);
               /* if (Row != null)
                {
                    m_AllowUpdate = TristateBoolType.Parse(cell.Row.AllowUpdate);
                }*/
                if (Grid != null)
                    m_AllowUpdate = Grid.AllowUpdate;
                return TristateBoolType.ToBool(m_AllowUpdate, true);
            }
            set
            {
                m_AllowUpdate = value;
                m_AllowUpdateSet = false;
            }
        }

        /// <summary>
        /// Sets or gets extra arguments for html elements like input and textArea
        /// </summary>
        /// <value>The input box arguments.</value>
        [Browsable(true),
        Description(@"Sets or gets extra arguments for html elements like input and textArea"),
        Category("Appearance"),
        ]
        public string Attributes
        {
            get { return m_Attributes; }
            set { m_Attributes = value; }
        }

        /// <summary>
        /// Sets or gets whether changing a selection should result in a post back.
        /// </summary>
        /// <value><c>true</c> if [auto post back]; otherwise, <c>false</c>.</value>
        [Browsable(true),
        DefaultValue(false),
        Description(@"Sets or gets whether changing a selection should result in a post back.")]
        public bool AutoPostback
        {
            get { return m_AutoPostback; }
            set { m_AutoPostback = value; }
        }

        /// <summary>
        /// Gets the name of the HTML form element for the grid.
        /// </summary>
        /// <value>The name of the form element.</value>
        public string ClientHeaderId
        {
            get { return string.Format("{0}_{1}", Grid.ClientID, ColumnId); }
        }

        /// <summary>
        /// Sets or gets the column identifier in the data source.
        /// If the column identifier is found in the data source it will automatically retrieve data from it.
        /// </summary>
        /// <remarks>
        /// ColumnID reflected at html page is 'FormElementName'[+Row Index or "_header"] property.
        /// </remarks>
        /// <value>The name.</value>
        [Browsable(true),
        Category("Data"),
        Description(
             @"Sets or gets the column identifier in the data source. If the column name is found in the data source it will automatically retrieve data from it."
             )]
        public string ColumnId
        {
            get { return m_Columnname; }
            set { m_Columnname = value; }
        }

        

        /// <summary>
        /// Sets or gets the column type for this instance. When setting column type you might change the behavior and/or
        /// how the user experiences the column in the application. An overview and description of the different columns available
        /// for <see cref="WebGrid.Grid">WebGrid.Grid</see> can be found at <see cref="WebGrid.Column">WebGrid.Column</see>.
        /// Some properties unique to a certain column type might be lost when you switch from one column type to another.
        /// </summary>
        /// <value>The type of the column.</value>
        [Browsable(false),
        Description(
             @"Sets or gets the column type for this instance. When setting column type you might change the behavior and/or how the user experiences the column in the application."
             )]
        public ColumnType ColumnType
        {
            get { return m_ColumnType; }
            set
            {
                if (ColumnId != null && value != ColumnType && value != ColumnType.Unknown)
                {

                    Column newCol = GetColumnFromType(ColumnId, m_Table, value);
                    newCol.CopyFrom(this);
                    newCol.m_ColumnType = value;

                    switch (newCol.ColumnType)
                    {
                        case ColumnType.DateTime:
                        case ColumnType.Number:
                        case ColumnType.Decimal:
                            newCol.EditAlign = HorizontalPosition.Right;
                            newCol.GridAlign = HorizontalPosition.Left;
                            break;
                        default:
                            newCol.GridAlign = HorizontalPosition.Left;
                            newCol.EditAlign = HorizontalPosition.Left;
                            break;
                    }

                    m_Table.Columns[ColumnId] = newCol;

                    if (Grid.Trace.IsTracing)
                        Grid.Trace.Trace("Column ({0}) changed column type: {1} --> {2}", ColumnId, ColumnType,
                                         value);
                }
                else
                    m_ColumnType = value;
            }
        }

        /// <summary>
        /// The user has to confirm the action with a dialog box. This property can be disabled by setting ConfirmMessage to null (nothing).
        /// </summary>
        /// <value>The text to be displayed in the dialog box.</value>
        [Browsable(true),
        Description(
             @"The user has to confirm the action with a dialog box. This property can be disabled by setting ConfirmMessage to null (nothing)."
             )]
        public string ConfirmMessage
        {
            get { return m_ConfirmMessage; }
            set { m_ConfirmMessage = HttpUtility.HtmlEncode(value); }
        }

        /// <summary>
        /// Select a custom style sheet for this column.
        /// </summary>
        /// <value>The style sheet to be used.</value>
        [Browsable(true),
        Category("Appearance"),
        Description(@"Select a custom style sheet for this column.")]
        [CssClassProperty]
        public string CssClass
        {
            get { return m_Cssclass; }
            set { m_Cssclass = value; }
        }

        /// <summary>
        /// Select a custom style sheet for this column's title.
        /// </summary>
        /// <value>The style sheet to be used.</value>
        [Browsable(true),
        Category("Appearance"),
        Description(@"Select a custom style sheet for this column.")]
        [CssClassProperty]
        public string CssClassTitle
        {
            get { return m_CssclassTitle; }
            set { m_CssclassTitle = value; }
        }

        /// <summary>
        /// Sets or gets data source for this column, as default columns are using same data source as the grid."
        /// With WebGrid you can, if you have the proper permissions, access other databases from your data-source.
        /// One such example is microsoft's NorthWind database that can be accessed like this to read the
        /// table labeled Customers:
        /// [NorthWind].dbo.[Customers]
        /// </summary>
        /// <remarks>
        /// See the columns 'DataSourcePrimaryKeys'
        /// </remarks>
        /// <value>The name of the table.</value>
        [Browsable(true),
        Category("Data"),
        Description(
             @"Sets or gets data source for this column, as default columns are using same datasource as the grid."
             )]
        public string DataSourceId
        {
            get { return m_DataSourceId; }
            set
            {
              m_DataSourceId = value;
             }
        }

        /// <summary>
        /// Sets of gets data source primary keys for this column, this is used to identify rows to be updated inserted. Seperate primary keys with ';'.
        /// </summary>
        [Browsable(true),
        Category("Data"),
        Description(
            @"The primary keys needed to update the rows for this column's 'DataSourceID'"
            )]
        public string DataSourcePrimaryKeys
        {
            get
            {
                return m_DataSourcePrimaryKeys;
            }
            set
            {
                m_DataSourcePrimaryKeys = value;
            }
        }

        /// <summary>
        /// Gets Table for 'DataSourceId' for this column.
        /// </summary>
        public Table DataSourceTable
        {
            get
            {

                if (m_Table.Columns[ColumnId].m_DataSourceTableId == null)
                {
                    m_Table.Columns[ColumnId].m_DataSourceTableId = new Table(Grid) { DataSourceId = DataSourceId };

                    m_Table.Columns[ColumnId].m_DataSourceTableId.GetSchema();
                    m_Table.Columns[ColumnId].m_DataSourceTableId.RetrieveForeignkeys = false;
                }
                return m_Table.Columns[ColumnId].m_DataSourceTableId;
            }
        }

        /// <summary>
        /// Datatype of this Column 
        /// </summary>
        public TypeCode DataType
        {
            get { return m_TypeCode; }
        }

        /// <summary>
        /// Gets the default data source set by WebGrid
        /// </summary>
        public string DefaultDataSourceId
        {
            get
            {
                 //It is an enumerable data source.
                if (m_Table.DataSourceType != DataSourceControlType.InternalDataSource || ( m_Grid.MasterTable.DataSource != null && string.IsNullOrEmpty(m_Grid.Sql)))
                    return null;

                if (DataSourceId != null && !DataSourceId.Equals(Grid.DataSourceId) && ColumnType != ColumnType.Foreignkey)
                    return DataSourceId;
                if (ColumnType == ColumnType.Foreignkey)
                    return m_Table.DataSourceId;
                //Check if there is a data source for the column.
                return string.IsNullOrEmpty(m_DefaultDataSourceId) ? m_Table.DataSourceId : m_DefaultDataSourceId;
            }
            internal set
            {
                m_DefaultDataSourceId = value;
            }
        }

        /// <summary>
        /// Sets or gets the actual content in the column. 
        /// You can also use this property for setting a default value for new records.
        /// (Relationship columns should use DisplayText for setting default value.)
        /// </summary>
        /// <value>The value.</value>
        [Browsable(true),
        Category("Data"),
        Description(
             @"Sets or gets the actual content in the column. You can also use this property for setting a default value for new records."
             )]
        public string DefaultValue
        {
            get
            {
                if (m_defaultValue == null && m_DbDefaultValue != null)
                    m_defaultValue = m_DbDefaultValue.ToString();
                return m_defaultValue;
            }
            set
            {
                m_defaultValue = value;
            }
        }

        /// <summary>
        /// Sets or gets the ordered priority of the columns. This property is used to rank which column is displayed first. Lower number equals higher priority.
        /// </summary>
        /// <value>The desired priority.</value>
        /// <remarks>Lower number equals higher priority.</remarks>
        [Browsable(true),
        Category("Behavior"),
        DefaultValue(10),
        Description(
             @"Sets or gets the priority of the column. This property is used to rank which column is displayed first."
             )]
        public int DisplayIndex
        {
            get { return m_DisplayIndex; }
            set
            {
                m_DisplayIndex = value;

                if (Grid != null && Grid.m_EventRanInit)
                    Grid.m_VisibleColumnsByIndex = null;
            }
        }

        /// <summary>
        /// Sets or gets Horizontal Position for the column in detail view.
        /// </summary>
        /// <value>The desired alignment.</value>
        [Browsable(true),
        Category("Behavior"),
        Description(@"Sets or gets Horizontal Position for the column in detail view.")]
        public HorizontalPosition EditAlign
        {
            get { return m_EditAlign; }
            set { m_EditAlign = value; }
        }

        /// <summary>
        /// Sets or gets horizontal position for the column title in detail view.
        /// </summary>
        /// <value>The desired alignment.</value>
        [Browsable(true),
        Category("Behavior"),
        Description(@"Sets or gets Horizontal Position for the column title in detail view.")]
        public HorizontalPosition EditAlignTitle
        {
            get
            {
                return m_EditAlignTitle == HorizontalPosition.Undefined ? HorizontalPosition.Left : m_EditAlignTitle;
            }
            set { m_EditAlignTitle = value; }
        }

        /// <summary>
        /// Sets or gets Vertical Position for the column in detail view.
        /// </summary>
        /// <value>The desired alignment.</value>
        [Browsable(true),
        Category("Behavior"),
        DefaultValue(typeof(VerticalPosition), "Middle"),
        Description(@"Sets or gets Vertical Position for the column in detail view.")]
        public VerticalPosition EditVAlign
        {
            get { return m_EditVAlign; }
            set { m_EditVAlign = value; }
        }

        /// <summary>
        /// Sets or gets whether this column should use postback not Callback.
        /// </summary>
        /// <value><c>true</c> if [auto post back]; otherwise, <c>false</c>.</value>
        [Browsable(true),
        DefaultValue(false),
        Description(@"Sets or gets whether this column should use postback not Callback.")]
        public bool ForcePostBack
        {
            get { return m_ForcePostBack; }
            set { m_ForcePostBack = value; }
        }

        /// <summary>
        /// Gets or sets the grid.
        /// </summary>
        /// <value>The grid.</value>
        [Browsable(false)]
        public Grid Grid
        {
            get
            {
                return m_Grid;
            }
            set { m_Grid = value; }
        }

        /// <summary>
        /// Sets or gets Horizontal Position for the column in grid view.
        /// </summary>
        /// <value>The desired alignment.</value>
        [Browsable(true),
        Category("Behavior"),
        Description(@"Sets or gets Horizontal Position for the column in grid view.")]
        public HorizontalPosition GridAlign
        {
            get { return m_GridAlign; }
            set { m_GridAlign = value; }
        }

        /// <summary>
        /// Sets or gets Vertical Position for the column in grid view.
        /// </summary>
        /// <value>The desired alignment.</value>
        [Browsable(true),
        Category("Behavior"),
        DefaultValue(typeof(VerticalPosition), "Middle"),
        Description(@"Sets or gets Vertical Position for the column in grid view.")]
        public VerticalPosition GridVAlign
        {
            get { return m_GridVAlign; }
            set { m_GridVAlign = value; }
        }

        // if null, return ColumnName
        /// <summary>
        /// Sets or gets header column text. The header text property is used as the header in grid-mode and column title in edit-mode.
        /// If header text is null (nothing) the <see cref="Column.Title">Title</see> property is used as header text.
        /// </summary>
        /// <value>The header text</value>
        [Browsable(true),
        Category("Behavior"),
        Description(
             @"Sets or gets the header column text. The Title property is used as the header in grid-mode and column title in edit-mode."
             )]
        public string HeaderText
        {
            get
            {
                return string.IsNullOrEmpty(m_HeaderText) ? Title : m_HeaderText;
            }
            set { m_HeaderText = value; }
        }

        /// <summary>
        /// This property is obsolete property, please use the property 'HeightEditableColumn' instead.
        /// </summary>
        [Bindable(true),
        Category("Data"),
        Browsable(false),
        Obsolete("This property is obsolete property, please use the property 'HeightEditableColumn' instead."),
        Description(@"This property is obsolete property, please use the property 'HeightEditableColumn' instead.")]
        public Unit Height
        {
            get { return HeightEditableColumn; }
            set { HeightEditableColumn = value; }
        }

        /// <summary>
        /// Sets or Gets the column heightEditableColumn. This property applies to the editable area within the column.
        /// </summary>
        /// <value>The heightEditableColumn of the editable area within the column.</value>
        [Browsable(true),
        Description(
             @"Sets or Gets the column height. This property applies to the editable area within the column.")]
        public Unit HeightEditableColumn
        {
            get { return m_HeightEditableColumn; }
            set { m_HeightEditableColumn = value; }
        }

        /// <summary>
        /// Sets or Gets whether the Title should be rendered in detail view.
        /// This property is set to false if column has <see cref="Column.AllowEdit">AllowEdit</see> property set to true and <see cref="Column.Title">Title</see> is empty.
        /// </summary>
        /// <value><c>true</c> if [hide edit title]; otherwise, <c>false</c>.</value>
        [Browsable(true),
        Category("Appearance"),
        DefaultValue(false),
        Description(@"Sets or Gets whether the Title should be rendered in detail view.")]
        public bool HideDetailTitle
        {
            get { return m_HideDetailTitle; }
            set { m_HideDetailTitle = value; }
        }

        /// <summary>
        /// Hides the column in grid view if no rows have a value.
        /// This only works with columns that are in a data source.
        /// </summary>
        /// <value><c>true</c> if [hide if empty]; otherwise, <c>false</c>.</value>
        [Browsable(true),
        Category("Behavior"),
        Description(
             @"Hides the column in grid view if no rows have a value. This only works with columns that are in a datasource."
             )]
        public bool HideIfEmpty
        {
            get
            {
                return ColumnType == ColumnType.SystemColumn || m_HideIfEmpty;
            }
            set
            {
                m_HideIfEmpty = value;
                if (Grid != null && Grid.m_EventRanInit)
                    Grid.m_VisibleColumnsByIndex = null;
            }
        }

        /// <summary>
        /// Sets or Gets whether the value for this column should be HTML encoded before rendered.
        /// This property applies to columns that have the <see cref="Column.AllowEdit">AllowEdit</see> property set to false.
        /// </summary>
        /// <value><c>true</c> if [HTML encode]; otherwise, <c>false</c>.</value>
        [Browsable(true),
        Category("Data"),
        DefaultValue(false),
        Description(@"Sets or Gets whether the value for this column should be HTML encoded before rendered.")]
        public bool HtmlEncode
        {
            get { return m_HtmlEncode; }
            set { m_HtmlEncode = value; }
        }

        /// <summary>
        /// Sets or Gets whether the content of the value property is a hyperlink in grid view with target to detail view.
        /// </summary>
        /// <value><c>true</c> if [link field]; otherwise, <c>false</c>.</value>
        [Browsable(true),
        Category("Behavior"),
        Description(
             @"Sets or Gets whether the content of the value property is a hyperlink in grid view with target to detail view."
             )]
        public bool HyperLinkColumn
        {
            get { return TristateBoolType.ToBool(m_HyperLinkColumn, false); }
            set { m_HyperLinkColumn = value; }
        }

        /// <summary>
        /// Sets or gets whether this is an identify column.
        /// </summary>
        /// <value><c>true</c> if identity; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// The <see cref="WebGrid.Grid">WebGrid.Grid</see> automatically detects primary keys(s) and identity columns from the data source.
        /// Normally there is no need to set this property.
        /// </remarks>
        [Browsable(true),
        Category("Data"),
        DefaultValue(false),
        Description(@"Sets or gets whether this is an identify column.")]
        public bool Identity
        {
            get { return m_Identity; }
            set
            {
                if (value && Visibility == Visibility.Undefined) Visibility = Visibility.None;
                m_Identity = value;
            }
        }

        /// <summary>
        /// Sets or Gets whether the column exists in the data source.
        /// </summary>
        /// <value><c>true</c> if this instance is in DB; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// The Grid detects automatically if the value from the ColumnId property is in the data source.
        /// Normally there is no need to set this property.
        /// </remarks>
        [Browsable(true),
        Category("Data"),
        Description(
             @"Sets or Gets whether the column exists in the data source.(WebGrid automatically detects this as default)"
             )]
        public bool IsInDataSource
        {
            get { return TristateBoolType.ToBool(m_IsInDataSource, false); }
            set { m_IsInDataSource = value; }
        }

        ///<summary>
        /// Indicates if this column are visible or not.
        ///</summary>
        public bool IsVisible
        {
            get
            {
                return IsVisibleColumn(false);
            }
        }

        ///<summary>
        ///</summary>
        public bool LoadColumnDataSourceData
        {
            get
            {
                return m_LoadColumnDataSourceData;
            }
            set
            {
                m_LoadColumnDataSourceData = value;
            }
        }

        /// <summary>
        /// Apply masks at text input fields.
        /// </summary>
        /// <value>The input box arguments.</value>
        [Browsable(true),
        Description(@"Apply masks at text input fields."),
        Category("Appearance"),
        ]
        public string MaskedInput
        {
            get { return m_MaskedInput; }
            set { m_MaskedInput = value; }
        }
        /// <summary>
        /// Apply masks at text input fields.
        /// </summary>
        /// <value>The input box arguments.</value>
        [Browsable(true),
        Description(@"Apply masks at text input fields."),
        Category("Appearance"),
        ]
        public bool DisableMaskedInput
        {
            get { return m_DisableMaskedInput; }
            set { m_DisableMaskedInput = value; }
        }

        /// <summary>
        /// Sets or gets the maximum amount of characters allowed in the column. Disabled if value is 0.
        /// </summary>
        /// <value>The integer that represents the maximum amount of characters allowed in the column.</value>
        [Browsable(true),
        Category("Behavior"),
        Description(
             @"Sets or gets the maximum amount of characters allowed in the column. Disabled if value is 0.")]
        public int MaxSize
        {
            get { return m_MaxSize; }
            set { m_MaxSize = value; }
        }

        /// <summary>
        /// Sets or gets the minimum characters allowed in the column. This property is disabled if 0 is set.
        /// This property applies only to editable columns.
        /// </summary>
        /// <value>The minimum amount of characters allowed.</value>
        /// <remarks>
        /// The Required property is set to true when this property is active.
        /// </remarks>
        [Browsable(true),
        Category("Behavior"),
        DefaultValue(0),
        Description(
             @"Sets or gets the minimum characters allowed in the column. This property is disabled if 0 is set. This property applies only to editable columns."
             )]
        public int MinSize
        {
            get { return m_MinSize; }
            set
            {
                if (value != 0 && Required == false)
                    Required = true;
                m_MinSize = value;
            }
        }

        /// <summary>
        /// Sets or Gets whether this column automatically starts on a new row in detail view.
        /// </summary>
        /// <value><c>true</c> if [edit new row]; otherwise, <c>false</c>.</value>
        [Browsable(true),
        Category("Appearance"),
        DefaultValue(true),
        Description(@"Sets or Gets whether this column automatically starts on a new row in detail view.")]
        public bool NewRowInDetail
        {
            get { return m_NewRowInDetail; }
            set { m_NewRowInDetail = value; }
        }

        /// <summary>
        /// Sets or Gets whether the column automatically starts on a new row in grid view.
        /// </summary>
        /// <value><c>true</c> if [list new row]; otherwise, <c>false</c>.</value>
        [Browsable(true),
        Category("Appearance"),
        DefaultValue(false),
        Description(@"Sets or Gets whether the column automatically starts on a new row in grid view.")]
        public bool NewRowInGrid
        {
            get { return m_NewRowInGrid; }
            set { m_NewRowInGrid = value; }
        }

        /// <summary>
        /// Set to TRUE to disable word-wrapping in this column.
        /// </summary>
        /// <value><c>true</c> if [non breaking]; otherwise, <c>false</c>.</value>
        [Browsable(true),
        Category("Behavior"),
        Description(@"Set to true to disable word-wrapping in this column.")]
        public bool NonBreaking
        {
            get
            {
                return m_NonBreaking == null
                           ? Grid != null && Grid.NonBreakingColumns
                           : TristateBoolType.ToBool(m_NonBreaking, false);
            }
            set { m_NonBreaking = value; }
        }

        /// <summary>
        /// If nullText is not null (nothing) it is returned. Otherwise it is set to value. Applies to both grid and detail views and
        /// both editable and non-editable columns.
        /// </summary>
        /// <value>The text to set.</value>
        [Browsable(true),
        Category("Behavior"),
        Description(
             @"If nullText is not null (nothing) it is returned. Otherwise it is set to value. Applies to both grid and detail views and both editable and non-editable columns."
             )]
        public string NullText
        {
            get { return m_NullText; }
            set { m_NullText = value; }
        }

        /// <summary>
        /// Insert an HTML object placed after of the column in detail view.
        /// </summary>
        /// <value>The HTML to be inserted.</value>
        [Browsable(true),
        Category("Appearance"),
        Description(@"Insert an HTML object placed after of the column in detail view.")]
        public string PostDetailText
        {
            get { return m_PostDetailText; }
            set { m_PostDetailText = value; }
        }

        /// <summary>
        /// Insert an HTML object placed after of the column in grid view.
        /// </summary>
        /// <value>The HTML to be inserted.</value>
        [Browsable(true),
        Category("Appearance"),
        Description(@"Insert an HTML object placed after of the column in grid view.")]
        public string PostGridText
        {
            get { return m_PostGridText; }
            set { m_PostGridText = value; }
        }

        /// <summary>
        /// Insert an HTML object placed in front of the column in detail view.
        /// </summary>
        /// <value>The HTML to be inserted.</value>
        /// <remarks>
        /// The HTML object is placed in front of the column, but after the title text for the column.
        /// </remarks>
        [Browsable(true),
        Category("Appearance"),
        Description(@"Insert an HTML object placed in front of the column in detail view.")]
        public string PreDetailText
        {
            get { return m_PreDetailText; }
            set { m_PreDetailText = value; }
        }

        /// <summary>
        /// Insert an HTML object in front of the column in grid view.
        /// </summary>
        /// <value>The HTML to be inserted.</value>
        /// <remarks>
        /// The HTML object is placed in front of the column, but behind the title text for the column.
        /// </remarks>
        [Browsable(true),
        Category("Appearance"),
        Description(@"Insert an HTML object in front of the column in grid view.")]
        public string PreGridText
        {
            get { return m_PreGridText; }
            set { m_PreGridText = value; }
        }

        /// <summary>
        /// Sets or gets whether the column is the primary key.
        /// </summary>
        /// <value><c>true</c> if primary key; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// The <see cref="WebGrid.Grid">WebGrid.Grid</see> automatically detects primary key(s) and identity columns from the data source.
        /// Normally there is no need to set this property.
        /// </remarks>
        [Browsable(true),
        Category("Data"),
        DefaultValue(false),
        Description(@"Sets or gets whether the column is the primary key.")]
        public bool Primarykey
        {
            get { return TristateBoolType.ToBool(m_Primarykey, false); }

            set
            {
                if (value)
                {
                    if (m_UniqueValueRequired == null)
                        UniqueValueRequired = true;
                    m_Primarykey = true;
                }
                else
                    m_Primarykey = false;
                if (m_Table != null)
                    m_Table.Columns.Primarykeys = null;
            }
        }

        /// <summary>
        /// Sets or gets whether this column is allowing empty content.
        /// </summary>
        /// <value><c>true</c> if required; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// The Required icon is not visibility in grid view. This is for columns that
        /// have the <see cref="Column.AllowEditInGrid">AllowEditInGrid</see> property set to true.
        /// </remarks>
        [Browsable(true),
        Category("Behavior"),
        Description(@"Sets or gets whether this column is allowing empty content.")]
        public bool Required
        {
            get { return TristateBoolType.ToBool(m_Required, false); }
            set { m_Required = value; }
        }

        /// <summary>
        /// How many cells this column spans across downwards. Default is 1.
        /// This property applies only to grid view.
        /// </summary>
        /// <value>The row span.</value>
        [Browsable(true),
        Category("Appearance"),
        DefaultValue(1),
        Description(@"How many cells this column spans across downwards. Default is 1.")]
        public int Rowspan
        {
            get { return m_Rowspan; }
            set { m_Rowspan = value; }
        }

        /// <summary>
        /// Sets or gets if this column should render search-option dropdown box in the header.
        /// </summary>
        /// <value><c>true</c> if [search option]; otherwise, <c>false</c>.</value>
        [Browsable(true),
        Description(@"Sets or gets if this column should render search-option dropdown box in the header."),
        Category("Appearance"),
        DefaultValue(false),
        ]
        public bool FilterByColumn
        {
            get { return m_EnableFilterByColumn; }
            set {
                m_EnableFilterByColumn = value;

            }
        }

        /// <summary>
        /// Sets or Gets whether the column is searchable. Text columns like foreignkey and Text are
        /// searchable by default. This property applies to grid view.
        /// </summary>
        /// <value><c>true</c> if searchable; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// This affects only the search results for the searchbox provided by the grid.
        /// </remarks>
        [Browsable(true),
        Category("Behavior"),
        DefaultValue(true),
        Description(
             @"Sets or Gets whether the column is searchable. Text and relationship columns are searchable by default"
             )]
        public bool Searchable
        {
            get { return TristateBoolType.ToBool(m_Searchable, true); }
            set { m_Searchable = value; }
        }

        /// <summary>
        /// Gets or sets the site of the <see cref='System.ComponentModel.Component'/>
        /// </summary>
        [Browsable(false)]
        public virtual ISite Site
        {
            get
            {
                if (m_Columnname == null && m_Grid != null)
                    return m_Grid.Site;

                return m_Columnsite;
            }
            set { m_Columnsite = value; }
        }

        /// <summary>
        /// Sets or gets whether column title should be sortable in grid-mode.
        /// </summary>
        /// <value><c>true</c> if sortable; otherwise, <c>false</c>.</value>
        [Browsable(true),
        Description(@"Sets or gets whether column title should be sortable in grid-mode."),
        Category("Behavior"),
        DefaultValue(true),
        ]
        public bool Sortable
        {
            get { return TristateBoolType.ToBool(m_Sortable, true); }
            set { m_Sortable = value; }
        }

         /// <summary>
        /// Sets or gets whether column is groupable, default is true.
        /// </summary>
        /// <value><c>true</c> if groupable; otherwise, <c>false</c>.</value>
        [Browsable(true),
        Description(@"Sets or gets whether column title should be sortable in grid-mode."),
        Category("Behavior"),
        DefaultValue(true),
        ]
        public bool Groupable
        {
            get { return TristateBoolType.ToBool(m_Groupable, true); }
            set { m_Groupable = value; }
        }
        

        /// <summary>
        /// Sets a custom error for the column.
        /// If set this message will override the default message used by <see cref="WebGrid">WebGrid</see>.
        /// Using 'null' as a value for the systemMessage resets the property and <see cref="WebGrid">WebGrid</see>
        /// will revert to the default errormessage.
        /// </summary>
        /// <value>The error message.</value>
        [Browsable(true),
        Category("Behavior"),
        DefaultValue(null),
        Description(
             @"Sets a custom error for the column. If set this message will override the default error generated by WebGrid."
             )]
        public string SystemMessage
        {
            get { return m_SystemMessage; }
            set { m_SystemMessage = value; }
        }

        /// <summary>
        /// Sets or gets how the errormessage should be displayed. Default systemMessageStyle is 'Undefined'.
        /// The systemMessageStyle 'ColumnTop' is returned if systemMessageStyle is undefined and AllowEditInGrid is true.
        /// </summary>
        /// <value>The errorstyle.</value>
        [Browsable(true),
        Category("Behavior"),
        Description(
             @"Sets or gets how the errormessage should be displayed. Default systemMessageStyle is 'Undefined'."
             )]
        public SystemMessageStyle SystemMessageStyle
        {
            get
            {
                if (AllowEditInGrid && Grid.DisplayView == DisplayView.Grid)
                    return SystemMessageStyle.ColumnTop;
                return m_SystemMessageStyle;
            }
            set { m_SystemMessageStyle = value; }
        }

        /// <summary>
        /// Sets or gets custom information about the column.
        /// Use this property if you need to pass custom information with the column.
        /// </summary>
        /// <value>The tag.</value>
        [Browsable(true),
        Category("Data"),
        Description(@"Sets or gets custom information about the column.")]
        public string Tag
        {
            get { return m_Tag; }
            set { m_Tag = value; }
        }

        /// <summary>
        /// TextTruncate the text when it exceeds a certain number of characters. It truncates at whole
        /// words and ends with ' ..'.
        /// A truncated text will be clickable by default.
        /// The truncated text can be made non-clickable by setting 'linkfield = false' for this column.
        /// </summary>
        /// <remarks>
        /// This property is disabled if the value is less than 1.
        /// This property applies only to grid view.
        /// </remarks>
        [Browsable(true),
        Category("Behavior"),
        DefaultValue(0),
        Description(
             @"Truncate the text when it exceeds a certain number of characters. It truncates at wholewords and ends with ' ..'."
             )]
        public int TextTruncate
        {
            get { return m_Texttruncate; }
            set { m_Texttruncate = value; }
        }

        // if null, return ColumnName
        /// <summary>
        /// Sets or gets the column title. The Title property is used as the header in grid-mode and column title in edit-mode.
        /// If Title is null (nothing) the ColumnId property is used as title.
        /// </summary>
        /// <value>The title.</value>
        /// <remarks>
        /// "<P>" and "</P>" tags are removed from the title.
        /// </remarks>
        [Browsable(true),
        Category("Behavior"),
        Description(
             @"Sets or gets the column title. The Title property is used as the header in grid-mode and column title in edit-mode."
             )]
        public string Title
        {
            get
            {
                return m_Title ?? ColumnId;
            }
            set { m_Title = value; }
        }

        /// <summary>
        /// Gets or sets the text displayed when the mouse pointer hovers over the column's title in detail view.
        /// </summary>
        /// <value>The tool tip.</value>
        [Browsable(true),
        Category("Behavior"),
        Description(
             @"GGets or sets the text displayed when the mouse pointer hovers over the column's title in detail view."
             )]
        public string ToolTipEditTitle
        {
            get { return m_TooltipEditTitle; }
            set { m_TooltipEditTitle = value; }
        }

        /// <summary>
        /// Gets or sets the text displayed when the mouse pointer hovers over the column's header title in grid view.
        /// </summary>
        /// <value>The tool tip.</value>
        [Browsable(true),
        Category("Behavior"),
        Description(
             @"Gets or sets the text displayed when the mouse pointer hovers over the column's header title in grid view."
             )]
        public string ToolTipGridHeaderText
        {
            get { return m_TooltipGridHeaderTitle; }
            set { m_TooltipGridHeaderTitle = value; }
        }

        /// <summary>
        /// Gets or sets the text displayed when the mouse pointer hovers over the column and it is editable. (has inputbox, dropdown etc.)
        /// </summary>
        /// <value>The tool tip.</value>
        [Browsable(true),
        Category("Behavior"),
        Description(
             @"Gets or sets the text displayed when the mouse pointer hovers over the column and it is editable. (has inputbox, dropdown etc.)"
             )]
        public string ToolTipInput
        {
            get { return m_ToolTipInput; }
            set { m_ToolTipInput = value; }
        }

        /// <summary>
        /// Sets or gets the tree indent text for tree structures. This property applies to grid-mode.
        /// </summary>
        /// <value>The tree indent text.</value>
        [Browsable(true),
        DefaultValue("&nbsp;&nbsp;&nbsp;"),
        Category("Behavior"),
        Description(
             @"Sets or gets the tree indent text for tree structures. This property applies to grid-mode. Default is ""  """
             )]
        public string TreeIndentText
        {
            get { return m_TreeIndentText; }
            set { m_TreeIndentText = value; }
        }

        /// <summary>
        /// Gets the tree level in a tree structure. This property applies to grid-mode.
        /// </summary>
        /// <value>The tree level.</value>
        [Browsable(false),
        Category("Data"),
        Description(@"Gets the tree level in a tree structure. This property applies to grid-mode.")]
        public int TreeLevel
        {
            get { return m_TreeLevel; }
        }

        /// <summary>
        /// Sets or Gets whether the column requires a unique value in the data source.
        /// This property uses <see cref="WebGrid.Grid.FilterExpression">WebGrid.Grid.Where</see> to check if the Value for this column already exist.
        /// </summary>
        /// <value><c>true</c> if [unique value required]; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// The <see cref="Column.Required">Required</see> property is also enabled if this property is enabled.
        /// </remarks>
        [Browsable(true),
        Category("Data"),
        DefaultValue(false),
        Description(@"Sets or Gets whether the column requires a unique value in the data source.")]
        public bool UniqueValueRequired
        {
            get
            {
                return m_UniqueValueRequired ?? false;
            }
            set
            {
                if (value)
                    Required = true;
                m_UniqueValueRequired = value;
            }
        }

        /// <summary>
        /// Sets or gets if this column should span over all rows in grid view.
        /// </summary>
        /// <value><c>true</c> if [use all rows]; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// This property is by default false, except for Image columns that have
        /// a <see cref="WebGrid.Grid.RecordsPerRow">RecordsPerRow</see> value that is larger than one.
        /// </remarks>
        [Browsable(true),
        Description(@"Sets or gets if this column should span over all rows in grid view."),
        Category("Appearance"),
        ]
        public bool UseAllRows
        {
            get
            {
                return m_UseAllRows == null  && ColumnType == ColumnType.Image && Grid.DisplayView == DisplayView.Grid && Grid.RecordsPerRow > 1

                       || TristateBoolType.ToBool(m_UseAllRows, false);
            }
            set { m_UseAllRows = value; }
        }

        /// <summary>
        /// Sets or gets a regular or logical expression for the column. This applies for columns that allow updating.
        /// It will automatically detect type of expression (logical or regular).
        /// </summary>
        /// <value>The valid expression.</value>
        /// <remarks>
        /// Remember that logical expression use '==' for 'equals', not '='.
        /// (A single '=' is used to assign new values.)
        /// Logical expression can also get column values by writing column names surrounded by brackets.
        /// Example: "([fromcolumn] &gt; [todatecolumn])"
        /// </remarks>
        [Browsable(true),
        Description(
             @"Sets or gets a regular or logical expression for the column. This applies for columns that allow updating. It will automatically detect type of expression (logical or regular)."
             ),
        Category("Data")]
        public string ValidExpression
        {
            get { return m_ValidExpression; }
            set { m_ValidExpression = value; }
        }

        /// <summary>
        /// Sets or gets column visibility. If Undefined the visibility is set by the
        /// <see cref="WebGrid.Grid.DefaultVisibility">WebGrid.Grid.DefaultVisibility</see> property.
        /// </summary>
        /// <value>The desired visibility.</value>
        [Browsable(true),
        Category("Behavior"),
        Description(
             @"Sets or gets column visibility. If Undefined the visibility is set by the grid's property ""defaultvisibility"" property."
             )]
        public Visibility Visibility
        {
            get
            {
                if (m_Visibility != Visibility.Undefined)
                    return m_Visibility;

                if (Grid != null)
                {
                    m_Visibility = Grid.DefaultVisibility;
                    m_Setvisibility = true;
                }
                return m_Visibility;
            }
            set { m_Visibility = value; }
        }

        /// <summary>
        /// This property is obsolete property, please use the property 'WidthEditableColumn' instead.
        /// </summary>
        [Bindable(true),
        Category("Data"),
        Browsable(false),
        Obsolete("This property is obsolete property, please use the property 'WidthEditableColumn' instead."),
        Description(@"This property is obsolete property, please use the property 'WidthEditableColumn' instead.")]
        public Unit Width
        {
            get { return WidthEditableColumn; }
            set { WidthEditableColumn = value; }
        }

        /// <summary>
        /// Sets or Gets the column header title width. This applies to grid view.
        /// </summary>
        /// <value>The width of column header title.</value>
        /// <remarks>
        /// This property is automatically set as default, depending width of title, where maximum of 140 pixels.
        /// </remarks>
        [Browsable(true),
        Category("Behavior"),
        Description(
             @"Sets or Gets the column header title width. This applies to grid view.")]
        public Unit WidthColumnHeaderTitle
        {
            get { return m_WidthColumnHeaderTitle; }
            set { m_WidthColumnHeaderTitle = value; }
        }

        /// <summary>
        /// Sets or Gets the column title width. This applies to detail view.
        /// </summary>
        /// <value>The width of column title.</value>
        /// <remarks>
        /// This property is automatically set as default, depending width of title, where maximum of 140 pixels.
        /// The BasicDetailLayout property is not affected by this property.
        /// </remarks>
        [Browsable(true),
        Category("Behavior"),
        Description(
             @"Sets or Gets the column title width. This applies to detail view.")]
        public Unit WidthColumnTitle
        {
            get { return m_WidthColumnTitle; }
            set { m_WidthColumnTitle = value; }
        }

        /// <summary>
        /// Sets or Gets the column width. This property applies to editable area within a column.
        /// </summary>
        /// <value>The width of the editable area within a column.</value>
        [Browsable(true),
        Category("Behavior"),
        Description(@"Sets or Gets the column width. This property applies to editable area within a column.")]
        public Unit WidthEditableColumn
        {
            get { return m_WidthEditableColumn; }
            set { m_WidthEditableColumn = value; }
        }

        internal int ColumnIndex
        {
            get
            {
                if (m_ColumnIndex != -2)
                    return m_ColumnIndex;
                return m_ColumnIndex = m_Table.Columns.GetIndex(ColumnId);
            }
        }

        internal string FileNameColumn
        {
            get; set;
        }

        internal virtual string GenerateColumnMask
        {
            get { return m_MaskedInput; }
        }

        /// <summary>
        /// Gets if this column requires single quote for
        /// data source operations.
        /// </summary>
        internal virtual bool HasDataSourceQuote
        {
            get
            {
                return false;
            }
        }

        //Used in Image & File columns
        /// <summary>
        /// Gets or sets the cell.Row.
        /// </summary>
        /// <value>The cell.Row.</value>
        [Browsable(false),
        ]
        internal bool IsBlob
        {
            get; set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is a form element or not.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is form element; otherwise, <c>false</c>.
        /// </value>
        internal bool IsFormElement
        {
            get { return m_IsFormElement; }
        }

        /// <summary>
        /// When the FilterByColumn property is used it will create a dropdown box for the column
        /// in grid view with auto post-back enabled. This property can be used to create an 
        /// easy-to-use search options interface for the user.
        /// </summary>
        /// <remarks>
        /// It uses a ColumnId-Value collection where ColumnId is the title for the search-option and Value is the SQL identifier.
        /// </remarks>
        [Browsable(true),
        Category("Data"),
        Description(
             @"When the FilterByColumn property is used it will create a dropdown box for the column n grid view with auto post-back enabled. This property can be used to create an easy-to-use search options interface for the user."
             )]
        internal NameValueCollection FilterByColumnCollection
        {
            get { return m_FilterByColumn; }

            set { m_FilterByColumn = value; }
        }

        #endregion Properties

        #region Methods

        // ICloneable implementation
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            Column mc = (Column)MemberwiseClone();
            return mc;
        }

        /// <summary>
        /// Display Value for this column
        /// </summary>
        public virtual string DisplayText(RowCell cell)
        {
            if (!Equals(cell.Value, cell.DataSourceValue) || cell.PostBackValue == null)
                if (cell.Value != null) return cell.PostBackValue = cell.Value.ToString();
                    else return DefaultValue;
            return cell.PostBackValue;
        }

        /// <summary>
        /// Releases all resources used by the column.
        /// </summary>
        public virtual void Dispose()
        {
            //There is nothing to clean.
            if (Disposed != null)
                Disposed(this, EventArgs.Empty);
        }

        /// <summary>
        /// When the FilterByColumn property is used it will create a dropdown box for the column
        /// in grid-mode with autopostback enabled. This property can be used to create an 
        /// easy-to-use search options interface for the user.
        /// </summary>
        /// <param name="searchtitle">A user friendly name for the search option (existing searchtitle for search option will be replaced).</param>
        /// <param name="identifier">The SQL identifier; the sql statement to retrieve data.</param>
        public void SetFilterByColumn(string searchtitle, string identifier)
        {
            m_FilterByColumn.Set(searchtitle, identifier);
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns></returns>
        public virtual bool Validate(RowCell cell)
        {
            if (DenyValidate())
                return true;

            if (string.IsNullOrEmpty(cell.PostBackValue) && (!Equals(cell.Value, cell.DataSourceValue) || !cell.GotPostBackData))
                cell.PostBackValue = DisplayText(cell);

            if (ValidExpression != null && ValidateExpression(cell) == false)
            {
                if (string.IsNullOrEmpty(SystemMessage) == false)
                    Grid.SystemMessage.Add(SystemMessage, SystemMessageStyle,
                                           cell.Row.GetColumnInitKeys(ColumnId));
                else if (Grid.DisplayView == DisplayView.Grid && SystemMessageStyle != SystemMessageStyle.WebGrid)
                    Grid.SystemMessage.Add(String.Format(Grid.GetSystemMessage("SystemMessage_Grid_validate")),
                                           SystemMessageStyle, cell.Row.GetColumnInitKeys(ColumnId));
                else
                    Grid.SystemMessage.Add(String.Format(Grid.GetSystemMessage("SystemMessage_Validate"), Title),
                                           SystemMessageStyle, cell.Row.GetColumnInitKeys(ColumnId));
                return false;
            }

            if (string.IsNullOrEmpty(cell.PostBackValue) && Required)
            {
                if (string.IsNullOrEmpty(SystemMessage) == false)
                    Grid.SystemMessage.Add(SystemMessage, SystemMessageStyle,
                                           cell.Row.GetColumnInitKeys(ColumnId));
                else if (Grid.DisplayView == DisplayView.Grid && SystemMessageStyle != SystemMessageStyle.WebGrid)
                    Grid.SystemMessage.Add(String.Format(Grid.GetSystemMessage("SystemMessage_Grid_required")),
                                           SystemMessageStyle, cell.Row.GetColumnInitKeys(ColumnId));
                else
                {
                    string searchtitle = Title;
                    if (string.IsNullOrEmpty(searchtitle))
                        searchtitle = ColumnId;
                    Grid.SystemMessage.Add(
                        String.Format(Grid.GetSystemMessage("SystemMessage_Required"), searchtitle),
                        SystemMessageStyle, cell.Row.GetColumnInitKeys(ColumnId));
                }
                return false;
            }

            if (UniqueValueRequired && IsInDataSource && (ValidateColumnPostBackValue(cell) != null) &&
                !string.IsNullOrEmpty(DataSourceId ?? DefaultDataSourceId))
            {

                string _where = Interface.BuildFilter(Grid.MasterTable, true);
                object myvalue = ValidateColumnPostBackValue(cell);
                string addonWhere = Interface.BuildFilterElement(true, null, this, myvalue);
                if (ColumnType == ColumnType.DateTime)
                {
                    System.DateTime? datecolumn = ((DateTime)this).Value(cell);
                    if (datecolumn != null)
                        addonWhere = String.Format("{0} = '{1}'", ColumnId,
                                                   datecolumn.Value.ToString("yyyyMMdd HH:mm:ss"));
                }
                bool isUnique = true;
                addonWhere = string.IsNullOrEmpty(_where) == false
                                 ? string.Format("{0} AND {1}", _where, addonWhere)
                                 : string.Format("WHERE {0}", addonWhere);
                Query getDuplicate =
                    Query.ExecuteReader(
                        string.Format("SELECT {0} FROM [{1}] {2}", ColumnId, DataSourceId ?? DefaultDataSourceId,
                                      addonWhere),
                        Grid.ActiveConnectionString, Grid.DatabaseConnectionType);

                if (getDuplicate.Read())
                    isUnique = false;

                getDuplicate.Close();

                if (isUnique == false)
                {
                    Grid.SystemMessage.Add(
                        string.IsNullOrEmpty(SystemMessage) == false
                            ? SystemMessage
                            : String.Format(Grid.GetSystemMessage("SystemMessage_UniqueValueRequired"), Title),
                        SystemMessageStyle,
                        cell.Row.GetColumnInitKeys(ColumnId));
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Validates a postback value for this column.
        /// </summary>
        /// <param name="cell">The cell with postback value</param>
        /// <returns>returns null if failed to validate else the column object type</returns>
        public virtual object ValidateColumnPostBackValue(RowCell cell)
        {
            return cell.PostBackValue;
        }

        /// <summary>
        /// Gets the result (true or false) from ValidExpression. Read remarks for further documentation.
        /// Undefined is returned when the system was unable to <see cref="WebGrid.Util.Validate">WebGrid.Util.Validate</see>.
        /// </summary>
        public bool? ValidateExpression(RowCell cell)
        {
            return (cell.Value == null || !(cell.Value is string)) ? true : Util.Validate.IsExpression(ValidExpression, cell.Value.ToString());
        }

        internal static string BuildColumnDataSourceIdFilter(Row row, IEnumerable<string> primaryKeys)
        {
            StringBuilder where = new StringBuilder(string.Empty);

            foreach (string primaryKey in primaryKeys)
            {

                if (row.Columns.GetIndex(primaryKey) == -1)
                    throw new GridException(
                        string.Format(
                            "The column '{0}' was not found in the grid. (Caused by 'DataSourcePrimaryKeys')",
                            primaryKey));
                if (string.IsNullOrEmpty(where.ToString()) == false)
                    where.Append(" AND ");
                if (row[primaryKey].Value != null)
                    where.AppendFormat(!row.Columns[primaryKey].HasDataSourceQuote ? " {0}={1}" : " {0}='{1}'",
                                       primaryKey, row[primaryKey].Value);

                else if (row.Columns[primaryKey].DefaultValue != null)
                    where.AppendFormat(!row.Columns[primaryKey].HasDataSourceQuote ? " {0}={1}" : " {0}='{1}'",
                                       primaryKey, row.Columns[primaryKey].DefaultValue);
                else throw new GridException(string.Format("Missing value for key '{0}'", primaryKey));
            }
            return where.ToString();
        }

        internal static Column GetColumnFromType(string columnName, Table table, ColumnType columnType)
        {
            Column column;
            switch (columnType)
            {
                case ColumnType.Text:
                    column = new Text(columnName, table);
                    break;
                case ColumnType.Number:
                    column = new Number(columnName, table);
                    break;
                case ColumnType.Decimal:
                    column = new Decimal(columnName, table);
                    break;
                case ColumnType.Checkbox:
                    column = new Checkbox(columnName, table);
                    break;
                case ColumnType.DateTime:
                    column = new DateTime(columnName, table);
                    break;
                case ColumnType.Foreignkey:
                    column = new Foreignkey(columnName, table);
                    break;
                case ColumnType.ManyToMany:
                    column = new ManyToMany(columnName, table);
                    break;
                case ColumnType.GridColumn:
                    column = new GridColumn(columnName, table);
                    break;
                case ColumnType.Chart:
                    column = new Chart(columnName, table);
                    break;
                case ColumnType.File:
                    column = new File(columnName, table) { IsInDataSource = false };
                    break;
                case ColumnType.Image:
                    column = new Image(columnName, table) { IsInDataSource = false };
                    break;
                case ColumnType.ToolTip:
                    column = new ToolTipColumn(columnName, table) { IsInDataSource = false };
                    break;
                case ColumnType.SystemColumn:
                    column = new SystemColumn(columnName, Enums.SystemColumn.Undefined, table) { IsInDataSource = false };
                    break;
                default:
                    column = new UnknownColumn(columnName, table);
                    break;
            }
            return column;
        }

        internal virtual void AfterCancel()
        {
        }

        internal virtual void AfterUpdate(string editIndex,RowCell cell)
        {
        }

        // 24.03.2005 - jorn : removed ColumnType to avoid resetting of all data. This  might have affects elsewhere :|
        internal virtual void CopyFrom(Column column)
        {
            if (column == this)
                return;
            m_Grid = column.m_Grid;
            m_TreeIndentText = column.m_TreeIndentText;
            m_TreeLevel = column.m_TreeLevel;
            m_Columnname = column.m_Columnname;
            m_ColumnType = column.m_ColumnType;
            m_Title = column.m_Title;
            m_HeaderText = column.m_HeaderText;
            m_IsInDataSource = column.m_IsInDataSource;
            DisplayIndex = column.DisplayIndex;
            m_MaxSize = column.m_MaxSize;
            m_MinSize = column.m_MinSize;
            m_AllowEmpty = column.m_AllowEmpty;
            m_NullText = column.m_NullText;
             m_DbDefaultValue = column.m_DbDefaultValue;
            m_HyperLinkColumn = column.m_HyperLinkColumn;
            m_Visibility = column.m_Visibility;
            m_Primarykey = column.m_Primarykey;
            m_Identity = column.m_Identity;
            m_AllowUpdate = column.m_AllowUpdate;
            m_Table = column.m_Table;
            m_Required = column.m_Required;
            m_WidthEditableColumn = column.m_WidthEditableColumn;
            m_WidthColumnTitle = column.m_WidthColumnTitle;
            m_WidthColumnHeaderTitle = column.m_WidthColumnHeaderTitle;
            m_HeightEditableColumn = column.m_HeightEditableColumn;
            m_EditAlign = column.m_EditAlign;
            m_EditVAlign = column.m_EditVAlign;
            m_EditAlignTitle = column.m_EditAlignTitle;
            m_GridAlign = column.m_GridAlign;
            m_GridVAlign = column.m_GridVAlign;
            m_SaveValueToViewState = column.m_SaveValueToViewState;
            m_Searchable = column.m_Searchable;
            m_Sortable = column.m_Sortable;
            m_Groupable = column.m_Groupable;
            m_UseAllRows = column.m_UseAllRows;
            m_AllowEdit = column.m_AllowEdit;
            m_HtmlEncode = column.m_HtmlEncode;
            m_DataSourceId = column.m_DataSourceId;
            m_DataSourcePrimaryKeys = column.m_DataSourcePrimaryKeys;
            m_LoadColumnDataSourceData = column.m_LoadColumnDataSourceData;
            m_FilterByColumn = column.m_FilterByColumn;
            m_EnableFilterByColumn = column.m_EnableFilterByColumn;
            m_Cssclass = column.m_Cssclass;
            m_CssclassTitle = column.m_CssclassTitle;
           
            m_Rowspan = column.m_Rowspan;
            m_NonBreaking = column.m_NonBreaking;
            m_AutoPostback = column.m_AutoPostback;
            m_ForcePostBack = column.m_ForcePostBack;

            m_NewRowInDetail = column.m_NewRowInDetail;
            m_NewRowInGrid = column.m_NewRowInGrid;

            m_PreDetailText = column.m_PreDetailText;
            m_PreGridText = column.m_PreGridText;
            m_PostDetailText = column.m_PostDetailText;
            m_PostGridText = column.m_PostGridText;
            m_HideDetailTitle = column.m_HideDetailTitle;
            TextTruncate = column.TextTruncate;
            UniqueValueRequired = column.UniqueValueRequired;
            m_Attributes = column.m_Attributes;
            m_MaskedInput = column.m_MaskedInput;
            m_DisableMaskedInput = column.m_DisableMaskedInput;

            m_TypeCode = column.m_TypeCode;
            Tag = column.Tag;

            SystemMessageStyle = column.SystemMessageStyle;
            SystemMessage = column.SystemMessage;
            ConfirmMessage = column.ConfirmMessage;
            ValidExpression = column.ValidExpression;
            m_HideIfEmpty = column.m_HideIfEmpty;
            IsCreatedByWebGrid = column.IsCreatedByWebGrid;
            m_AllowEditInGrid = column.m_AllowEditInGrid;
            m_GridModeEditTransparent = column.m_GridModeEditTransparent;
            m_TooltipEditTitle = column.m_TooltipEditTitle;
            m_TooltipGridHeaderTitle = column.m_TooltipGridHeaderTitle;
            m_ToolTipInput = column.m_ToolTipInput;
            m_DefaultDataSourceId = column.m_DefaultDataSourceId;
            m_defaultValue = column.m_defaultValue;
              /*  m_GridViewTemplate = column.m_GridViewTemplate;
            m_GridViewTemplateCache = column.m_GridViewTemplateCache;
            m_DetailViewTemplate = column.m_DetailViewTemplate;
            m_DetailViewTemplateCache = column.m_DetailViewTemplateCache;*/
        }

        // 24.03.2005 - jorn : removed ColumnType to avoid resetting of all data. This  might have affects elsewhere :|
        internal virtual Column CopyTo(Column column)
        {
            if (column == this)
                return column;
            if (Grid.Trace.IsTracing)
                Grid.Trace.Trace("Column ({0}) started CopyTo.", ColumnId);
            //   column = (Column)base.MemberwiseClone();
            //   return column;
            column.m_Grid = m_Grid;
            column.m_TypeCode = m_TypeCode;
            column.m_Columnname = m_Columnname;
            column.m_ColumnType = m_ColumnType;
            column.m_Title = m_Title;
            column.m_IsInDataSource = m_IsInDataSource;
            column.m_HeaderText = m_HeaderText;
            column.m_DisplayIndex = m_DisplayIndex;
            column.m_MaxSize = m_MaxSize;
            column.m_MinSize = m_MinSize;
            column.m_AllowEmpty = m_AllowEmpty;
            column.m_NullText = m_NullText;
            column.m_DbDefaultValue = m_DbDefaultValue;
            column.m_HyperLinkColumn = m_HyperLinkColumn;
            column.m_Visibility = m_Visibility;
            column.m_Primarykey = m_Primarykey;
            column.m_Identity = m_Identity;
            column.m_Table = m_Table;
            column.m_Required = m_Required;
            column.m_WidthEditableColumn = m_WidthEditableColumn;
            column.m_WidthColumnTitle = m_WidthColumnTitle;
            column.m_WidthColumnHeaderTitle = m_WidthColumnHeaderTitle;
            column.m_HeightEditableColumn = m_HeightEditableColumn;
            column.m_EditAlign = m_EditAlign;
            column.m_EditAlignTitle = m_EditAlignTitle;
            column.m_EditVAlign = m_EditVAlign;
            column.m_GridAlign = m_GridAlign;
            column.m_GridVAlign = m_GridVAlign;
            column.m_TreeIndentText = m_TreeIndentText;
            column.m_TreeLevel = m_TreeLevel;
            column.m_SaveValueToViewState = m_SaveValueToViewState;
            column.m_Searchable = m_Searchable;
            column.m_Sortable = m_Sortable;
            column.m_Groupable = m_Groupable;
            column.m_UseAllRows = m_UseAllRows;
            column.m_AllowEdit = m_AllowEdit;
            column.m_AllowUpdate = m_AllowUpdate;
            column.m_HtmlEncode = m_HtmlEncode;
            column.m_Cssclass = m_Cssclass;
            column.m_CssclassTitle = m_CssclassTitle;
           
            column.m_Rowspan = m_Rowspan;
            column.m_NonBreaking = m_NonBreaking;
            column.m_Texttruncate = m_Texttruncate;
            column.m_NewRowInGrid = m_NewRowInGrid;
            column.m_HideDetailTitle = m_HideDetailTitle;
            column.m_NewRowInDetail = m_NewRowInDetail;
            column.m_DataSourcePrimaryKeys = m_DataSourcePrimaryKeys;
            column.m_LoadColumnDataSourceData = m_LoadColumnDataSourceData;
            column.m_DataSourceId = m_DataSourceId;
            column.m_FilterByColumn = m_FilterByColumn;
            column.m_EnableFilterByColumn = m_EnableFilterByColumn;
            column.m_PreDetailText = m_PreDetailText;
            column.m_PreGridText = m_PreGridText;
            column.m_PostDetailText = m_PostDetailText;
            column.m_PostGridText = m_PostGridText;
            column.m_AutoPostback = m_AutoPostback;
            column.m_ForcePostBack = m_ForcePostBack;
            column.m_SystemMessageStyle = m_SystemMessageStyle;
            column.m_SystemMessage = m_SystemMessage;
            column.m_ConfirmMessage = m_ConfirmMessage;
            column.m_ValidExpression = m_ValidExpression;
            column.m_Tag = m_Tag;
            column.m_UniqueValueRequired = m_UniqueValueRequired;
            column.m_Attributes = m_Attributes;
            column.m_DisableMaskedInput = m_DisableMaskedInput;

            column.m_MaskedInput = m_MaskedInput;
            column.m_HideIfEmpty = m_HideIfEmpty;
            column.m_AllowEditInGrid = m_AllowEditInGrid;
            column.m_GridModeEditTransparent = m_GridModeEditTransparent;
            column.IsCreatedByWebGrid = IsCreatedByWebGrid;

            column.m_TooltipEditTitle = m_TooltipEditTitle;
            column.m_TooltipGridHeaderTitle = m_TooltipGridHeaderTitle;
            column.m_ToolTipInput = m_ToolTipInput;
            column.m_DefaultDataSourceId = m_DefaultDataSourceId;
            column.m_defaultValue = m_defaultValue;
             /*   column.m_GridViewTemplate = m_GridViewTemplate;
            column.m_GridViewTemplateCache = m_GridViewTemplateCache;
            column.m_DetailViewTemplate = m_DetailViewTemplate;
            column.m_DetailViewTemplateCache = m_DetailViewTemplateCache;*/
             if (Grid.Trace.IsTracing)
                Grid.Trace.Trace("Column ({0}) ended CopyTo.", ColumnId);
            return column;
        }

        internal virtual Column CreateColumn()
        {
            return new UnknownColumn(ColumnId, m_Table);
        }

        internal bool DenyValidate()
        {
            return (AllowEdit == false || ColumnType == ColumnType.SystemColumn ||
                   (Grid.DisplayView == DisplayView.Grid && AllowEditInGrid == false) ||
                   (Grid.DisplayView == DisplayView.Detail && (Visibility == Visibility.Grid || Visibility == Visibility.None))) &&
                  HttpContext.Current.Request.Form[string.Format("{0}_{1}_validate", m_Grid.ClientID, ColumnId)] ==
                  null;
        }

        internal abstract Column Duplicate();

        internal void EditHtml(string text, HtmlTextWriter writer,RowCell cell)
        {
            string systemMessage = null;
            if (cell.Row != null && Grid.SystemMessage[cell.Row.GetColumnInitKeys(ColumnId)] != null)
                systemMessage = Grid.SystemMessage[cell.Row.GetColumnInitKeys(ColumnId)].Message;

            if (TreeLevel > 0 && Grid.DisplayView == DisplayView.Grid)
            {
                StringBuilder indent = new StringBuilder(string.Empty);
                for (int i = 0; i < TreeLevel; i++)
                    indent.Append(TreeIndentText);
                writer.Write("<span class=\"wgnowrap\">");
                writer.Write(indent);
            }

            if (string.IsNullOrEmpty(PreDetailText) == false && m_Grid.DisplayView == DisplayView.Detail)
                writer.Write(PreDetailText);
            if (string.IsNullOrEmpty(PreGridText) == false && m_Grid.DisplayView == DisplayView.Grid)
                writer.Write(PreGridText);

            if (systemMessage != null && (SystemMessageStyle == SystemMessageStyle.WebGrid) == false)
            {
                string jqueryuicss = "";
                if (m_Grid.IsUsingJQueryUICSSFramework)
                    jqueryuicss = "ui-state-error-text ";
                switch (SystemMessageStyle)
                {
                    case SystemMessageStyle.ColumnLeft:
                        writer.Write("<div class=\"{1}wgsystemmessagetitle\">{0}</div>", systemMessage, jqueryuicss);
                        break;
                    case SystemMessageStyle.ColumnTop:
                        writer.Write("<div class=\"{1}wgsystemmessagetitle\">{0}</div><br/>", systemMessage, jqueryuicss);
                        break;
                }
            }
             /*  if (m_GridViewTemplate != null && m_Grid.DisplayView == DisplayView.Grid)
                writer.Write(RenderGridViewTemplate(cell));
            else if (m_DetailViewTemplate != null && m_Grid.DisplayView == DisplayView.Detail)
                writer.Write(RenderDetailViewTemplate(cell));
            else*/
                writer.Write(text);

            if (systemMessage != null && (SystemMessageStyle == SystemMessageStyle.WebGrid) == false)
            {
                string jqueryuicss = "";
                if (m_Grid.IsUsingJQueryUICSSFramework)
                    jqueryuicss = "ui-state-error-text ";
                switch (SystemMessageStyle)
                {
                    case SystemMessageStyle.ColumnRight:
                        writer.Write("<div class=\"wgsystemmessagetitle\">{0}</div>", systemMessage, jqueryuicss);
                        break;
                    case SystemMessageStyle.ColumnBottom:
                        writer.Write("<br/><div class=\"wgsystemmessagetitle\">{0}</div>", systemMessage, jqueryuicss);
                        break;
                }
            }
            if (TreeLevel > 0 && Grid.DisplayView == DisplayView.Grid)
                writer.Write("</span>");
            if (string.IsNullOrEmpty(PostDetailText) == false && m_Grid.DisplayView == DisplayView.Detail)
                writer.Write(PostDetailText);
            else if (string.IsNullOrEmpty(PostGridText) == false && m_Grid.DisplayView == DisplayView.Grid)
                writer.Write(PostGridText);
        }

        /*
        internal string RenderGridViewTemplate(RowCell cell)
        {

            if (m_GridViewTemplateCache == null)
            {

                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);
                System.Web.UI.HtmlTextWriter controlwriter = new System.Web.UI.HtmlTextWriter(sw);
                m_GridViewTemplate.RenderControl(controlwriter);
                m_GridViewTemplateCache = sb.ToString();
                sw.Close();
                sw.Dispose();
                if (
                       m_GridViewTemplateCache.IndexOf(string.Format("[{0}]", ColumnId),
                                                           StringComparison.OrdinalIgnoreCase) != -1)
                    throw new GridException(string.Format("The template '{0}' can't contain reference to itself.",
                                                          ColumnId));
            }

            if (!m_GridViewTemplateCache.Contains("["))
                return m_GridViewTemplateCache;
            string content = m_GridViewTemplateCache;
            cell.Row.Columns.ForEach(delegate(Column column)
                                    {
                                        if (
                                            content.IndexOf(string.Format("[{0}]", column.ColumnId),
                                                             StringComparison.OrdinalIgnoreCase) == -1)
                                            return;

                                        HtmlTextWriter writer = new HtmlTextWriter(Grid);
                                        Visibility columnvisibility = column.Visibility;
                                        column.Visibility = Visibility;
                                        column.SystemMessageStyle = SystemMessageStyle.WebGrid;
                                        if (Grid.Trace.IsTracing)
                                            Grid.Trace.Trace("Rendering ColumnName: {0} in column: {1}",
                                                              column.ColumnId, ColumnId);
                                         column.RenderEditView(writer, cell.Row.Cells[column.ColumnId]);
                                        column.Visibility = columnvisibility;
                                        if (column.AllowEdit)
                                            writer.Write(
                                                "<input type=\"hidden\" value=\"validate\" name=\"{0}_{1}_validate\" />",
                                                m_Grid.ClientID, column.ColumnId);
                                        content = content.Replace(string.Format("[{0}]", column.ColumnId),
                                                                  writer.ToString());
                                    });
            return content;
        }
        internal string RenderDetailViewTemplate(RowCell cell)
        {
            if (m_DetailViewTemplate == null )
                return null;

            if (m_DetailViewTemplateCache == null)
            {

                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);
                System.Web.UI.HtmlTextWriter controlwriter = new System.Web.UI.HtmlTextWriter(sw);
                m_DetailViewTemplate.RenderControl(controlwriter);
                m_DetailViewTemplateCache = sb.ToString();
                sw.Close();
                sw.Dispose();
                if (
                    m_DetailViewTemplateCache.IndexOf(string.Format("[{0}]", ColumnId),
                                                      StringComparison.OrdinalIgnoreCase) != -1)
                    throw new GridException(string.Format("The template '{0}' can't contain reference to itself.",
                                                          ColumnId));
            }

            if (!m_DetailViewTemplateCache.Contains("["))
                return m_DetailViewTemplateCache;
            string content = m_DetailViewTemplateCache;
            cell.Row.Columns.ForEach(delegate(Column column)
                                    {
                                        if (
                                            content.IndexOf(string.Format("[{0}]", column.ColumnId),
                                                            StringComparison.OrdinalIgnoreCase) == -1)
                                            return;
                                        HtmlTextWriter writer = new HtmlTextWriter(Grid);
                                        Visibility columnvisibility = column.Visibility;
                                        column.Visibility = Visibility;
                                        column.SystemMessageStyle = SystemMessageStyle.WebGrid;
                                        if (Grid.Trace.IsTracing)
                                            Grid.Trace.Trace("Rendering ColumnName: {0} in column: {1}",
                                                             column.ColumnId, ColumnId);
                                        column.RenderEditView(writer, cell.Row.Cells[column.ColumnId]);
                                        column.Visibility = columnvisibility;
                                        if (column.AllowEdit)
                                            writer.Write(
                                                "<input type=\"hidden\" value=\"validate\" name=\"{0}_{1}_validate\" />",
                                                m_Grid.ClientID, column.ColumnId);
                                        content = content.Replace(string.Format("[{0}]", column.ColumnId),
                                                                  writer.ToString());
                                    });
            return content;
        }
        */
        internal virtual void GetColumnPostBackData(RowCell cell)
        {
            if (cell.GotPostBackData || !m_Grid.GetColumnsPostBackData)
                 return;

            string uniqueID = cell.CellClientId;
            if (Grid.GotHttpContext && HttpContext.Current.Request.Form[uniqueID] != null)
            {
                cell.PostBackValue = HttpContext.Current.Request.Form[uniqueID];

                cell.GotPostBackData = true;

                if (Grid.Trace.IsTracing)
                    Grid.Trace.Trace("{0} ({2}) contains post back-data '{1}'", uniqueID, cell.PostBackValue, ColumnId);
            }
            else
            {
                if (Grid.Trace.IsTracing)
                    Grid.Trace.Trace("{0} ({2}) was not found for post back-data ({1})", uniqueID, uniqueID, ColumnId);
            }
        }

        internal string GetDataTableFilterExpression(string tablename)
        {
            StringBuilder where = new StringBuilder(string.Empty);

            bool isidentity = false;

            if (DataSourceTable == null || DataSourceTable.Columns == null || DataSourceTable.Columns.Count == 0)
                throw new GridException(
                    string.Format("{0} datasource ({1}) is a invalid object.", Title, DataSourceId));

            foreach (Column column in DataSourceTable.Columns)
            {
                bool isInDb = false;
                bool isInSession = false;
                bool isInWebGrid = false;

                if (column.Identity)
                    isidentity = true;
                if (Grid.GotHttpContext && HttpContext.Current.Session[column.ColumnId] != null)
                    isInSession = true;
                if (Grid.MasterTable.Columns.GetIndex(column.ColumnId) > -1)
                {
                    isInWebGrid = true;
                    if (Grid.MasterTable.Columns[column.ColumnId].IsInDataSource)
                        isInDb = true;
                }
                if (Grid.Trace.IsTracing)
                    Grid.Trace.Trace("DataTable Column: {0} IsInDb : {1} IsInWebGrid : {2} IsInSession : {3}",
                                      column.ColumnId, isInDb, isInWebGrid, isInSession);
                if (column.Primarykey && Grid.MasterTable.Columns.GetIndex(column.ColumnId) > -1 &&
                    Grid.MasterTable.Columns[column.ColumnId].IsInDataSource)
                {
                    if (where.Length > 0)
                        where.Append(" AND ");

                    where.AppendFormat(" {0} = {1}", Interface.BuildTableElement(true, DataSourceId, column.ColumnId),
                        Interface.BuildTableElement(true,tablename,
                                       column.ColumnId));
                }
               /* else if (column.Primarykey &&
                         Grid.MasterTable.Columns.GetIndex(column.ColumnId) != -1 &&
                         Grid.MasterTable.Columns[column.ColumnId].Value != null)
                {
                    if (where.Length > 0)
                        where.Append(" AND ");
                    where.AppendFormat(" {0} = '{1}'", Interface.BuildTableFilterElement(true, DataSourceId, column.ColumnId),
                                       Grid.MasterTable.Columns[column.ColumnId].Value);
                }*/
                else if (Grid.GotHttpContext && HttpContext.Current.Session[column.ColumnId] != null)
                {
                    if (where.Length > 0)
                        where.Append(" AND ");
                    where.AppendFormat(" {0} = '{1}'", Interface.BuildTableElement(true, DataSourceId, column.ColumnId),
                                       HttpContext.Current.Session[column.ColumnId].ToString().Replace("'", "''"));
                }
            }

            if (where.Length == 0)
            {
                if (isidentity == false)
                    throw new GridException(
                        string.Format(
                            "{0} is using an invalid data source ({1}). No primarykey columns can be found.",
                            ColumnId, DataSourceId), null);
                where.Append("IDENTITY");
            }

            return where.ToString();
        }

        internal bool IsVisibleColumn(bool ignoreMode)
        {
            if (shouldShow != null)
                return (bool) shouldShow;

            Visibility visible = Visibility;

            // Special exception for Identity-columns
            if (ignoreMode == false && Identity && visible == Visibility.Undefined)
                return false;

            if (visible == Visibility.Undefined)
                visible = Grid.DefaultVisibility;
            switch (visible)
            {
                case Visibility.None:
                    shouldShow = false;
                    return false;
                case Visibility.Both:
                    shouldShow = true;
                    return true;
                case Visibility.Grid:
                    if (Grid.DisplayView == DisplayView.Detail)
                    {
                        shouldShow = false;
                        return false;
                    }
                    shouldShow = true;
                    return true;
                case Visibility.Detail:
                    if (Grid.DisplayView == DisplayView.Detail)
                    {
                        shouldShow = true;
                        return true;
                    }
                    shouldShow = false;
                    return false;
            }
            return false;
        }

        internal void LabelHtml(string text, HtmlTextWriter writer, RowCell cell)
        {
            if (string.IsNullOrEmpty(PreGridText) == false && m_Grid.DisplayView == DisplayView.Grid)
                writer.Write(PreGridText);
            else if (string.IsNullOrEmpty(PreDetailText) == false && m_Grid.DisplayView == DisplayView.Detail)
                writer.Write(PreDetailText);


            writer.Write(text);

            if (string.IsNullOrEmpty(PostGridText) == false && m_Grid.DisplayView == DisplayView.Grid)
                writer.Write(PostGridText);
            else if (string.IsNullOrEmpty(PostDetailText) == false && m_Grid.DisplayView == DisplayView.Detail)
                writer.Write(PostDetailText);
        }

        // 2005.01.09 - jorn, MaxSize.Length
        internal void LoadDataSourceInformation(DataBaseColumnInformation dbci)
        {
            if (m_IsInDataSource == false) // Overridden by WebGrid
                return;

            IsInDataSource = dbci.isInDB;
            if (ColumnType == ColumnType.Unknown)
            {
                m_TypeCode = Type.GetTypeCode(dbci.dataType);
                ColumnType columntype = Interface.MapColumnType(m_TypeCode);

                if (m_ColumnType != columntype)
                    ColumnType = columntype;
            }
            m_TypeCode = Type.GetTypeCode(dbci.dataType);
            if (m_Primarykey == null)
            {
                Primarykey = dbci.primaryKey;
                m_Primarykeybywebgrid = true;
            }
            if (m_Identity == false)
                Identity = dbci.identity;

            if (DisplayIndex == -1) // is it changed in aspx file?
                DisplayIndex = dbci.displayindex;

            if (ColumnType == ColumnType.File)
                Searchable = false;
            if (string.IsNullOrEmpty(dbci.defaultDataSourceId) == false)
                m_DefaultDataSourceId = dbci.defaultDataSourceId;
            if (MaxSize == 0 && dbci.maxSize != null)
            {
                MaxSize = (int) dbci.maxSize;
                if (dbci.maxSize > 5000)
                    Sortable = false;
            }

            if (m_Title == null) // is it changed in the aspx file?
                Title = dbci.title;

            if (m_AllowEmpty == null)
                AllowEmpty = dbci.canNull;

            if (Required == false && Identity != true)
                Required = !dbci.canNull;

               // if (m_DbDefaultValue == null) // is it changed in the aspx file?
               //     m_DbDefaultValue = dbci.defaultValue;
            if (dbci.readOnly)
                AllowEdit = false;
        }

        internal void LoadValueFromColumnDataSource(RowCell cell)
        {
            if (!m_LoadColumnDataSourceData)
                return;
            if (string.IsNullOrEmpty(DataSourcePrimaryKeys))
                throw new GridException(
                    string.Format(
                        "Set the 'DataSourcePrimaryKeys' for the property 'LoadColumnDataSourceData' in the column '{0}'",
                        ColumnId));

            string where = BuildColumnDataSourceIdFilter(cell.Row, DataSourcePrimaryKeys.Split(';'));
            cell.Value =
                Query.ExecuteScalar(string.Format("SELECT {2} FROM [{0}] WHERE {1}", DataSourceId, where, ColumnId),
                                    m_Grid.ActiveConnectionString);
            cell.DataSourceValue = cell.Value;
            if (Grid.Trace.IsTracing)
                Grid.Trace.Trace("LoadValueFromColumnDataSource - {3}.{2}:  SELECT {2} FROM [{0}] WHERE {1}", DataSourceId, where, ColumnId,Grid.ID);
        }

        /// <summary>
        /// This method is fired when loading data and is handled for each column type. 
        /// </summary>
        /// <param name="databaseValue"></param>
        /// <returns></returns>
        internal virtual object OnLoadFromDatabase(object databaseValue)
        {
            return databaseValue;
        }

        /// <summary>
        /// Used to validate data source against on update insert.
        /// </summary>
        /// <param name="ea">insertupdate argument</param>
        /// <param name="cell">Rowcell</param>
        internal virtual void OnUpdateInsert(CellUpdateInsertArgument ea, RowCell cell)
        {
            // Fix value
            ea.AddApostrophe = true;

            if (IsInDataSource == false || (ea.Value != null && ea.Value.Equals(cell.DataSourceValue)) || (( ea.Value == DBNull.Value) && (ea.DataSourceValue == null)) )
                ea.IgnoreChanges = true;
            if (ea.Value == null)
                ea.AddApostrophe = false;
        }

        internal void Render(HtmlTextWriter writer, RowCell cell)
        {
            if (cell.Row.CellsToSpan > 0)
                cell.Row.CellsToSpan--;
            else
            {
                if (cell.ExtraCellSpan > 0)
                    cell.Row.CellsToSpan += cell.ExtraCellSpan;
                if (Grid.DisplayView == DisplayView.Grid)
                    DesignRenderGrid(writer, cell);
                else
                    DesignRenderDetail(writer, cell);
            }
        }

        internal virtual void RenderEditView(HtmlTextWriter writer, RowCell cell)
        {
            if (Identity | AllowEdit == false || (Grid.DisplayView == DisplayView.Grid && AllowEditInGrid == false))
            {
                RenderLabelView(writer, cell);
                return;
            }
            GetColumnPostBackData(cell);

            string uniqueID = cell.CellClientId;

            string theValueToShow = DisplayText(cell);
            theValueToShow = HttpUtility.HtmlEncode(theValueToShow);

            StringBuilder cssstyle = new StringBuilder();
            if (WidthEditableColumn != Unit.Empty)
                cssstyle.AppendFormat("width: {0};", WidthEditableColumn);
            if (HeightEditableColumn != Unit.Empty)
                cssstyle.AppendFormat("height: {0};", HeightEditableColumn);

            if (EditAlign != HorizontalPosition.Undefined)
                cssstyle.AppendFormat("text-align: {0};", EditAlign);

            StringBuilder onblur = new StringBuilder(" onblur=\"");
            string script = string.Empty;
            if (Grid.InputHighLight != Color.Empty)
            {
                script =
                    string.Format(
                        " onfocus=\"this.accessKey = style.backgroundColor;style.backgroundColor='{0}';\"",
                        Grid.ColorToHtml(Grid.InputHighLight));
                onblur.Append("style.backgroundColor=this.accessKey;");
            }
            if (Grid.ColumnChangedColour != Color.Empty)
                onblur.AppendFormat("isChanged(this,'{0}');", Grid.ColorToHtml(Grid.ColumnChangedColour));

            if (AutoPostback)
            {
                onblur.Append("if(hasChanged(this))");
                onblur.Append(Grid.EnableCallBack && !ForcePostBack
                                  ? Asynchronous.GetCallbackEventReference(Grid,
                                                                           string.Format("ElementPostBack!{0}!{1}",
                                                                                         ColumnId,
                                                                                         cell.Row.PrimaryKeyValues),
                                                                           false,
                                                                           string.Empty, string.Empty).Replace(
                                        "javascript:", "")
                                  : Grid.Page.ClientScript.GetPostBackEventReference(Grid,
                                                                                     string.Format(
                                                                                         "ElementPostBack!{0}!{1}",
                                                                                         ColumnId,
                                                                                         cell.Row.PrimaryKeyValues)));

            }
            onblur.AppendFormat("\"{0}", script);

            string mask = null;

            if (!DisableMaskedInput && GenerateColumnMask != null)
            {
                string maskId = string.Format("{0}_{1}", Grid.ID, ColumnId);
                if (!Grid.MaskedColumns.ContainsKey(maskId))
                    Grid.MaskedColumns.Add(maskId, GenerateColumnMask);

                mask = string.Format(" alt=\"{0}\"", maskId);
            }
            string s = string.Format(
                "<input type=\"text\" {0} {4}{5} class=\"wgeditfield\" style=\"{1}\" value=\"{2}\" id=\"{3}\" name=\"{3}\"/>",
                onblur, cssstyle, theValueToShow, uniqueID, Attributes, mask);

            if (string.IsNullOrEmpty(m_ToolTipInput) == false)
                s = Tooltip.Add(s, m_ToolTipInput);

            EditHtml(s, writer, cell);
        }

        internal void RenderGrid(object strValue, HtmlTextWriter writer,RowCell cell)
        {
            if (strValue == null)
            {
                LabelHtml(null, writer, cell);
                return;
            }

            object theValueToShow = strValue;
            if (HtmlEncode)
                theValueToShow = HttpUtility.HtmlEncode(theValueToShow.ToString());

            string s = theValueToShow.ToString();

            if (Grid.DisplayView == DisplayView.Grid)
            {
                if (TextTruncate > 0 && s.Length > TextTruncate)
                {
                    string[] words = s.Split(' ');
                    if (words.Length == 1)
                    {
                        s = string.Format("{0} <b>..</b>", s.Substring(0, TextTruncate));
                        if (m_HyperLinkColumn == null)
                            HyperLinkColumn = true;
                    }
                    else
                    {
                        StringBuilder truncatedmsgBuilder = new StringBuilder();

                        for (int i = 0; i < words.Length; i++)
                        {
                            if (i == 0 && words[i].Length > TextTruncate)
                            {
                                s = string.Format("{0} <b>..</b>", words[i].Substring(0, TextTruncate));
                                if (m_HyperLinkColumn == null)
                                    HyperLinkColumn = true;
                                break;
                            }
                            if ((truncatedmsgBuilder.Length + words[i].Length) > TextTruncate)
                            {
                                s = string.Format("{0} <b>..</b>", truncatedmsgBuilder);
                                if (m_HyperLinkColumn == null)
                                    HyperLinkColumn = true;
                                break;
                            }
                            truncatedmsgBuilder.AppendFormat(" {0}", words[i]);
                        }
                    }
                }
                if (HyperLinkColumn)
                {
                    if (Grid.Page != null)
                    {
                        string argument;
                        if (cell.Row.RowType == RowType.Group)
                            argument = string.Format("GroupClick!{0}!{1}!{2}!{3}",
                                                     cell.Row.GroupColumnID, cell.Row[cell.Row.GroupColumnID].DataSourceValue, cell.Row.PrimaryKeyValues,cell.Row.GroupIsExpanded);
                        else argument = string.Format("RecordClick!{0}!{1}", ColumnId, cell.Row.PrimaryKeyValues);
                        string link = Grid.EnableCallBack
                                          ? Asynchronous.GetCallbackEventReference(Grid, argument
                                                                                   ,
                                                                                   false, string.Empty, string.Empty)
                                          : Grid.Page.ClientScript.GetPostBackEventReference(Grid, argument);

                        string b = (string.IsNullOrEmpty(ConfirmMessage))
                                       ? string.Empty
                                       : string.Format("if(wgconfirm('{0}',this,'{1}')) ",
                                                       ConfirmMessage.Replace("'", "\\'"),
                                                       Grid.DialogTitle.Replace("'", "\\'"));

                        if (Grid.PopupExtender == null)
                            s = string.Format("<a class=\"wglinkfield\" href=\"#\" onclick=\" {0}{1}\">{2}</a>", b, link,
                                              s);
                        else if (Grid.Scripts == null ||
                                 (!Grid.Scripts.DisableJQuery && !Grid.Scripts.DisablePopupExtender))
                        {
                            string title = string.Empty;
                            if (Grid.PopupExtender.Title != null)
                                title = string.Format(" title=\"{0}\" ",
                                                      HttpUtility.HtmlEncode(Grid.PopupExtender.Title));
                            s =
                                string.Format(
                                    "<a class=\"wgpopupiframe wglinkfield\"{5}href=\"{2}?DetailPopupKeys={1}&amp;TB_Grid=true&amp;TB_iframe=true&amp;height={3}&amp;width={4}\">{0}</a>",
                                    s, cell.Row.PrimaryKeyValues,
                                    Grid.PopupExtender.Url, Grid.PopupExtender.Height, Grid.PopupExtender.Width,
                                    title);

                            Grid.AddClientScript(writer,
                                                 string.Format(
                                                     @"$(document).ready(function() {{$("".wgpopupiframe"").colorbox({{ width: ""{1}px"", height: ""{0}px"", iframe: true }});}});",
                                                     Grid.PopupExtender.Height, Grid.PopupExtender.Width));
                        }

                    }
                }
                StringBuilder indentBuilder = new StringBuilder();
                if (TreeLevel > 0)
                {
                    for (int i = 0; i < TreeLevel; i++)
                        indentBuilder.Append(TreeIndentText);
                    s = indentBuilder + s;
                }
            }
            LabelHtml(s, writer,cell);
        }

        internal virtual void RenderLabelView(HtmlTextWriter writer,RowCell cell)
        {
            m_IsFormElement = false;
            RenderGrid(DisplayText(cell), writer, cell);
        }

        /// <summary>
        /// This method is fired by 'Data Interface/InsertUpdateRow' method.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnArgs"></param>
        /// <param name="rowIdentity"></param>
        internal void UpdateInsertColumnDataSourceId(Row row, BeforeUpdateInsertEventArgs columnArgs,
            string rowIdentity)
        {
            bool isInsert = true;

            if (DataSourceId == null || row == null)
                return;
            if (string.IsNullOrEmpty(m_DataSourcePrimaryKeys))
                throw new GridException(string.Format("Set the 'DataSourcePrimaryKeys' for the property 'DataSourceId' in the column '{0}'", ColumnId));

            columnArgs.DataSourceId = DataSourceId;

            for (int i = 0; i < row.Columns.Count; i++)
                columnArgs.Row[row.Columns[i].ColumnId].Value = row[row.Columns[i].ColumnId].Value;

            m_Table.m_Grid.BeforeUpdateInsertEvent(columnArgs);

                for (int i = 0; i < row.Columns.Count; i++)
                if (row.Columns[i].ColumnType == ColumnType.SystemColumn == false)
                    row[row.Columns[i].ColumnId].Value = columnArgs.Row[row.Columns[i].ColumnId].Value;
            if (!columnArgs.AcceptChanges)
                return;

            string where = BuildColumnDataSourceIdFilter(row, m_DataSourcePrimaryKeys.Split(';'));
            if (where == null || where.Length < 2)
                throw new GridException(
                    string.Format(
                        "Unable to build a valid filter expression for the property 'DataSourceId' in the column '{0}'",
                        ColumnId));

            Query shouldUpdate =
                Query.ExecuteReader(string.Format("SELECT (1) FROM [{0}] WHERE {1}", DataSourceId, where),
                                    m_Grid.ActiveConnectionString, m_Grid.DatabaseConnectionType);

            if (shouldUpdate.Read())
                isInsert = false;
            shouldUpdate.Close();

            InsertUpdate insertUpdateDataTable = isInsert
                                                     ?
                                                         new InsertUpdate(DataSourceId, QueryType.Insert,
                                                                          Grid.ActiveConnectionString)
                                                     :
                                                         new InsertUpdate(DataSourceId, QueryType.Update,
                                                                          Grid.ActiveConnectionString);
            columnArgs.DataSourceId = DataSourceId;
             foreach (Column column in DataSourceTable.Columns)
            {
                string columnID = column.ColumnId;

                if (column.ColumnType == ColumnType.SystemColumn)
                    continue;
                object value = null;
                if (row.Columns.Contains(columnID)) // FIRST TRY TO RETRIEVE DATA FROM row.Row...
                {
                    CellUpdateInsertArgument ea = columnArgs.Row[row.Columns[columnID].ColumnId];

                    if (ea != null)
                    {
                        row.Columns[columnID].OnUpdateInsert(ea, row[column.ColumnId]);

                        if (ea.Value != null)
                        {
                            if (ea.m_Insert && row.Columns[columnID].Identity)
                                value = rowIdentity;
                            else
                                switch (row.Columns[columnID].ColumnType)
                                {
                                    case ColumnType.Foreignkey:
                                        value = ea.m_Insert ? ea.Value : row[ea.Column.ColumnId].Value;
                                        break;
                                    default:
                                        value = ea.Value;
                                        break;
                                }
                        }
                        else if (Grid.GotHttpContext && HttpContext.Current.Session[columnID] != null)
                            // THEN SESSION OBJECT..
                            value = HttpContext.Current.Session[columnID];
                    }
                }
                else if (Grid.GotHttpContext && HttpContext.Current.Session[columnID] != null) //SKIP
                    value = HttpContext.Current.Session[columnID];
                else
                    continue;

                if (columnID != null)
                    insertUpdateDataTable.Add(columnID, value);
            }
            try
            {
                if (isInsert == false)
                    insertUpdateDataTable.FilterExpression = where;
                insertUpdateDataTable.Execute();
                if (Grid.Debug)
                {
                    Grid.m_DebugString.AppendFormat("<b>Matrix-InsertUpdate({0})</b>:{1}<br/>", ColumnId,
                                                    insertUpdateDataTable.GetQueryString());
                    Grid.m_DebugString.AppendFormat("<b>Matrix-InsertUpdate where</b>:{0}<br/>", where);
                }
            }
            catch (Exception ee)
            {

                throw new GridException(insertUpdateDataTable.GetQueryString(),
                                        string.Format("Error updating/inserting datatable for column: {0}", ColumnId),
                                        ee);
            }
        }

        /*

        /// <summary>
        /// HTML Template for WebGrid Columns, if set it will render the content of this instead of the Value of the content.
        /// Create dynamic content by referring to columnId surrounded by [ and ] an example of dynamic content is:
        /// "Hi my first name is [FirstName]"
        /// </summary>
        /// <value>The html content</value>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateInstance(TemplateInstance.Single)]
        [DefaultValue(null)]
        [Browsable(false)]
        [MergableProperty(false)]
        [NotifyParentProperty(true)]
        public PlaceHolder GridViewTemplate
        {
            get
            {
                if (m_GridViewTemplate == null)
                    m_GridViewTemplate = new PlaceHolder();
                return m_GridViewTemplate;
            }
            set
            {
                m_GridViewTemplate = value;

            }
        }

        private PlaceHolder m_GridViewTemplate;
        private string  m_GridViewTemplateCache;

        /// <summary>
        /// HTML Template for WebGrid Columns, if set it will render the content of this instead of the Value of the content.
        /// Create dynamic content by referring to columnId surrounded by [ and ] an example of dynamic content is:
        /// "Hi my first name is [FirstName]"
        /// </summary>
        /// <value>The html content</value>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateInstance(TemplateInstance.Single)]
        [DefaultValue(null)]
        [Browsable(false)]
        [MergableProperty(false)]
        [NotifyParentProperty(true)]
        public PlaceHolder DetailViewTemplate
        {
            get
            {
                if (m_DetailViewTemplate == null)
                    m_DetailViewTemplate = new PlaceHolder();
                return m_DetailViewTemplate;
            }
            set
            {
                m_DetailViewTemplate = value;

            }
        }

        private PlaceHolder m_DetailViewTemplate;
        private string m_DetailViewTemplateCache;
        */
        internal object Value(RowCell cell)
        {
            LoadValueFromColumnDataSource(cell);
            return cell.Value;
        }
        
        private void DesignRenderDetail(HtmlTextWriter writer,RowCell cell)
        {
            // 16.10.2004, jorn - Kommenterte ut EditAlign....

            if (Grid.BasicDetailLayout)
                RenderEditView(writer,cell);
            else
            {
                StringBuilder tdStart = new StringBuilder("<td ");

                if (cell != null && cell.InternalCellSpan > 0)
                    tdStart.AppendFormat(" colspan=\"{0}\"", cell.InternalCellSpan);
                if (WidthColumnTitle != Unit.Empty)
                    tdStart.AppendFormat(" width=\"{0}\"", WidthColumnTitle);

                // 16.10.2004, jorn - Det er dataene vi skal aligne, ikke selve input boksen.
                // ja?
                if (ColumnType == ColumnType.GridColumn && EditAlign != HorizontalPosition.Undefined)
                    tdStart.AppendFormat(" align=\"{0}\"", EditAlign);

                if (EditVAlign != VerticalPosition.undefined)
                    tdStart.AppendFormat(" valign=\"{0}\"", EditVAlign);

                if (CssClass != null)
                    tdStart.AppendFormat(" class=\"wgdetailcell {0}\">", CssClass);
                else
                    tdStart.Append(" class=\"wgdetailcell\">");
                writer.Write(tdStart);
                RenderEditView(writer,cell);

                writer.Write("</td>");
            }
        }

        private void DesignRenderGrid(HtmlTextWriter writer,RowCell cell)
        {
            if (Grid == null)
                return;
            StringBuilder tdStart = new StringBuilder();
            StringBuilder style = new StringBuilder();

            if (CssClass != null)
                tdStart.AppendFormat("<td class=\"wggridcell {0}\" ", CssClass);
            else
                tdStart.Append("<td class=\"wggridcell\"");

            if (cell != null && cell.InternalCellSpan > 0)
                tdStart.AppendFormat(" colspan=\"{0}\"", cell.InternalCellSpan);
            if (Rowspan > 1) tdStart.AppendFormat(" rowspan=\"{0}\"", Rowspan);

            if( cell != null )
                tdStart.AppendFormat(" id=\"{0}{1}r{2}\"", Grid.ID, ColumnId, cell.Row.RowIndex);
            else
                tdStart.AppendFormat(" id=\"{0}{1}\"", Grid.ID, ColumnId);

            if (GridAlign != HorizontalPosition.Undefined)
                style.AppendFormat("text-align :{0};", GridAlign);
            else if (Grid.RecordsPerRow > 1) // GUESSING IT IS A PRODUCTLIST OR SOMETHING SIMILAR..
                style.Append("width: 100%;text-align : center;vertical-align:top");

            if (style.Length > 1)
                tdStart.AppendFormat(" style=\"{0}\"", style);

            if (GridVAlign != VerticalPosition.undefined)
                tdStart.AppendFormat(" valign=\"{0}\"", GridVAlign);

            if (cell != null)
            {
                if (!string.IsNullOrEmpty(writer.Grid.OnClientCellClick))
                {
                    ClientCellEventArgs ea = new ClientCellEventArgs
                                                 {
                                                     ColumnId = ColumnId,
                                                     RowIndex = cell.Row.RowIndex,
                                                     Value = cell.Value,
                                                     ClientEventType = ClientEventType.OnClientColumnClick
                                                 };

                    string content = JavaScriptConvert.SerializeObject(ea);
                    writer.Grid.JsOnData.AppendLine();
                    string jsonId = string.Format("{0}r{2}{1}click", Grid.ID, ColumnId, cell.Row.RowIndex).Replace("-",
                                                                                                                   "A");

                    writer.Grid.JsOnData.AppendFormat("{0} = {1}", jsonId, content);

                    tdStart.AppendFormat(@"onclick=""{0}(this,{1});return false""", writer.Grid.OnClientCellClick,
                                         jsonId);
                }
                if (!string.IsNullOrEmpty(writer.Grid.OnClientCellDblClick))
                {
                    ClientCellEventArgs ea = new ClientCellEventArgs
                                                 {
                                                     ColumnId = ColumnId,
                                                     RowIndex = cell.Row.RowIndex,
                                                     Value = cell.Value,
                                                     ClientEventType = ClientEventType.OnClientColumnDblClick
                                                 };

                    string content = JavaScriptConvert.SerializeObject(ea);
                    writer.Grid.JsOnData.AppendLine();
                    string jsonId =
                        string.Format("{0}r{2}{1}dblclick", Grid.ID, ColumnId, cell.Row.RowIndex).Replace("-", "A");

                    writer.Grid.JsOnData.AppendFormat("{0} = {1}", jsonId, content);

                    tdStart.AppendFormat(@"ondblclick=""{0}(this,{1});return false""", writer.Grid.OnClientCellDblClick,
                                         jsonId);
                }
                if (!string.IsNullOrEmpty(writer.Grid.OnClientCellMouseOver))
                {
                    ClientCellEventArgs ea = new ClientCellEventArgs
                                                 {
                                                     ColumnId = ColumnId,
                                                     RowIndex = cell.Row.RowIndex,
                                                     Value = cell.Value,
                                                     ClientEventType = ClientEventType.OnClientColumnMouseOver
                                                 };

                    string content = JavaScriptConvert.SerializeObject(ea);
                    writer.Grid.JsOnData.AppendLine();
                    string jsonId =
                        string.Format("{0}r{2}{1}mouseover", Grid.ID, ColumnId, cell.Row.RowIndex).Replace("-", "A");

                    writer.Grid.JsOnData.AppendFormat("{0} = {1}", jsonId, content);

                    tdStart.AppendFormat(@"onmouseover=""{0}(this,{1});return false""",
                                         writer.Grid.OnClientCellMouseOver, jsonId);
                }
                if (!string.IsNullOrEmpty(writer.Grid.OnClientCellMouseOut))
                {
                    ClientCellEventArgs ea = new ClientCellEventArgs
                                                 {
                                                     ColumnId = ColumnId,
                                                     RowIndex = cell.Row.RowIndex,
                                                     Value = cell.Value,
                                                     ClientEventType = ClientEventType.OnClientColumnMouseOut
                                                 };

                    string content = JavaScriptConvert.SerializeObject(ea);
                    writer.Grid.JsOnData.AppendLine();
                    string jsonId =
                        string.Format("{0}r{2}{1}mouseout", Grid.ID, ColumnId, cell.Row.RowIndex).Replace("-", "A");

                    writer.Grid.JsOnData.AppendFormat("{0} = {1}", jsonId, content);

                    tdStart.AppendFormat(@"onmouseout=""{0}(this,{1});return false""", writer.Grid.OnClientCellMouseOut,
                                         jsonId);
                }
            }
            tdStart.Append(">");

            writer.Write(tdStart);

            if (NonBreaking)
                writer.Write("<span class=\"wgnowrap\">");

            if (AllowEditInGrid)
            {
                if (Grid != null)
                    if (Grid.MasterTable.Columns.Primarykeys == null ||
                        Grid.MasterTable.Columns.Primarykeys.Count == 0)
                        throw new ApplicationException(
                            string.Format(
                                "Using 'AllowEditInGrid' property requires one or more columns in '{0}' to be a primary key.",
                                Grid.ID));
                RenderEditView(writer, cell);
                if (Identity == false && AllowEdit)
                    if (Grid != null) Grid.m_StoredAllowEditInGrid = true;
            }
            else
                RenderLabelView(writer, cell);
            if (NonBreaking)
                writer.Write("</span>");

            writer.Write("</td>");
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeAllowEdit()
        {
            return m_AllowEdit != null && m_AllowEditset;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeAllowEditInGrid()
        {
            return m_AllowEditInGrid != null;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeAllowEmpty()
        {
            return m_AllowEmpty != null && m_AllowEmptyset;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeAllowUpdate()
        {
            return m_AllowUpdate != null && m_AllowUpdateSet;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeColumnId()
        {
            return true;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeColumnType()
        {
            return false;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeDisplayIndex()
        {
            return m_DisplayIndex != -1;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeEditAlign()
        {
            return m_EditAlign != HorizontalPosition.Undefined;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeEditAlignTitle()
        {
            return m_EditAlignTitle != HorizontalPosition.Undefined;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeGrid()
        {
            return false;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeGridAlign()
        {
            return m_GridAlign != HorizontalPosition.Undefined;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeGridVAlign()
        {
            return m_GridVAlign != VerticalPosition.undefined;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeHeaderText()
        {
            return m_HeaderText != null;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeHeight()
        {
            return false;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeHeightEditableColumn()
        {
            return m_HeightEditableColumn != Unit.Empty;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeHideIfEmpty()
        {
            return m_HideDetailTitle && ColumnType != ColumnType.SystemColumn;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeHyperLinkColumn()
        {
            return m_HyperLinkColumn != null;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeIdentity()
        {
            return false;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeDisableMaskedInput()
        {
            return m_DisableMaskedInput != true;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeIsInDataSource()
        {
            return m_IsInDataSource != null && (m_IsInDataSource != false ||
                                                                  (ColumnType == ColumnType.Chart || ColumnType == ColumnType.SystemColumn ||
                                                                   ColumnType == ColumnType.ManyToMany
                                                                       ? false
                                                                       : true));
        }

        private bool ShouldSerializeLoadColumnDataSourceData()
        {
            return m_LoadColumnDataSourceData;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeMaxSize()
        {
            return ((((ColumnType != ColumnType.Foreignkey && ColumnType != ColumnType.Chart) && (ColumnType != ColumnType.File)) && ColumnType != ColumnType.GridColumn) && m_MaxSize >= 1) && m_MaxSize <= 1000000;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeNonBreaking()
        {
            return m_NonBreaking != null;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializePrimarykey()
        {
            return !m_Primarykeybywebgrid && m_Primarykey != null;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeRequired()
        {
            return m_Required != null;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeFilterByColumn()
        {
            //   if (FilterByColumn.Count > 1)
            //       return true;
            return false;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeSite()
        {
            return false;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeSystemMessageStyle()
        {
            return m_SystemMessageStyle != SystemMessageStyle.Undefined;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeTextTruncate()
        {
            return m_Texttruncate != 0;
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
        private bool ShouldSerializeUseAllRows()
        {
            return m_UseAllRows != null;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeVisibility()
        {
            return m_Visibility != Visibility.Undefined && !m_Setvisibility;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeWidth()
        {
            return false;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeWidthColumnHeaderTitle()
        {
            return m_WidthColumnHeaderTitle != Unit.Empty;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeWidthColumnTitle()
        {
            return m_WidthColumnTitle != Unit.Empty;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeWidthEditableColumn()
        {
            return m_WidthEditableColumn != Unit.Empty;
        }

        #endregion Methods

        #region Other

        /*
        /// <summary>
        /// Postback value for the column
        /// </summary>
        public string PostBackValue
        {
            get
            {
                return RowCell == null ? null : cell.PostBackValue;
            }
            internal set
            {
                if (RowCell == null)
                    return;
                cell.PostBackValue = value;
            }
        }*/
        /*
         2004.01.09 - jorn, recoded all this, and moved all the systemcolumn shit into the systemcolumn class!
         internal virtual bool IsVisibleColumn(bool ignoreMode)
         {
         Visibility visibility = _visible;
         if(visibility == Visibility.Undefined)
         visibility = Grid.DefaultVisibility;

         if( ColumnType == ColumnType.SystemColumn )
         {
         if( SystemColumn.EditColumn == ((WebGrid.SystemColumn)this).ColumnType && ( Grid.AllowEdit == true && Grid.DisplayEditColumn == true ) )
         goto wgcontinue;
         if( SystemColumn.DeleteColumn == ((WebGrid.SystemColumn)this).ColumnType && Grid.AllowDelete == true )
         goto wgcontinue;
         if( SystemColumn.NewRecordColumn == ((WebGrid.SystemColumn)this).ColumnType && Grid.AllowNew == true )
         goto wgcontinue;
         if( SystemColumn.UpdateRecordsColumn  == ((WebGrid.SystemColumn)this).ColumnType && Grid.AllowUpdate == true )
         goto wgcontinue;
         if( SystemColumn.SelectColumn == ((WebGrid.SystemColumn)this).ColumnType )
         goto wgcontinue;
         return false;
         }
         wgcontinue:
         if(visibility == Visibility.None)
         return false;

         return true;
         }

         internal virtual bool	IsVisibleColumn()
         {
         Visibility visibility = _visible;

         if(Identity == true && visibility == Visibility.Undefined)
         return false;

         if( ColumnType == ColumnType.SystemColumn )
         {
         if( SystemColumn.EditColumn == ((WebGrid.SystemColumn)this).ColumnType && ( Grid.AllowEdit == true && Grid.DisplayEditColumn == true ) )
         goto wgcontinue;
         if( SystemColumn.DeleteColumn == ((WebGrid.SystemColumn)this).ColumnType && Grid.AllowDelete == true )
         goto wgcontinue;
         if( SystemColumn.NewRecordColumn == ((WebGrid.SystemColumn)this).ColumnType && Grid.AllowNew == true )
         goto wgcontinue;
         if( SystemColumn.UpdateRecordsColumn  == ((WebGrid.SystemColumn)this).ColumnType && Grid.AllowUpdate == true )
         goto wgcontinue;
         if( SystemColumn.SelectColumn == ((WebGrid.SystemColumn)this).ColumnType )
         goto wgcontinue;
         return false;
         }
         wgcontinue:
         if(visibility == Visibility.Undefined)
         visibility = Grid.DefaultVisibility;

         if(visibility == Visibility.None)
         return false;
         if(visibility == Visibility.Both)
         return true;
         if(Grid.DisplayView == Modes.Edit && visibility == Visibility.Edit)
         return true;
         if(Grid.DisplayView == Modes.Grid && visibility == Visibility.Grid)
         return true;
         return false;
         }
         */

        #endregion Other
    }
}