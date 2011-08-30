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
    using System.ComponentModel;

    using WebGrid.Data;
    using WebGrid.Design;
    using HtmlTextWriter = WebGrid.Design.WebGridHtmlWriter;
    using WebGrid.Enums;

    #region Delegates

    ///<summary>
    ///</summary>
    ///<param name="sender"></param>
    ///<param name="args"></param>
    public delegate void BeforeToolTipRenderEventHandler(object sender, BeforeToolTipRenderEventHandlerArgs args);

    #endregion Delegates

    /// <summary>
    /// Event arguments for 'BeforeToolTipRender'
    /// </summary>
    public class BeforeToolTipRenderEventHandlerArgs
    {
        #region Properties

        /// <summary>
        /// Unique identifier in the data source.
        /// </summary>
        public string ColumnName
        {
            get; internal set;
        }

        /// <summary>
        /// Unique identifier in the data source.
        /// </summary>
        public string EditIndex
        {
            get; internal set;
        }

        /// <summary>
        /// The data you would like to return
        /// </summary>
        public string ReturnData
        {
            get; set;
        }

        /// <summary>
        /// The current row the tooltip belongs to.
        /// </summary>
        public Row Row
        {
            get; internal set;
        }

        #endregion Properties
    }

    /// <summary>
    /// The Text class is displayed as link to WebGrid tooltip.
    /// This class inherits from the <see cref="WebGrid.Column">WebGrid.Column</see> class.
    /// </summary>
    public class ToolTipColumn : Column
    {
        #region Fields

        private const string template = @"
        $(document).ready(function() {{
            $('#{0}').qtip({{
                content: {{
                text: '{9}',
                title: {{
                        text: '{10}'
                        {11}
                    }}
                 }},
                show: {{ when: {{ event: '{2}' }}, delay: 100, solo: {8} }},
                hide: {{ when: '{3}', delay: 100 }},

                position: {{
                    corner: {{
                    tooltip: 'topMiddle',
                    target: 'bottomMiddle'
                }}
                }},
                style: {{ name: '{7}', width: {4}, height: {5}, overflow: 'auto'}},
                api: {{
                    onShow: function() {{
                    $(""#{0}"").addClass(""wgtooltipselected"");
                    }},
                    onHide: function() {{
                    $(""#{0}"").removeClass(""wgtooltipselected"");
                    }},
                    onRender: function() {{
                       var self = this;

                    Anthem_CallBack(null, ""Control"", ""{1}"", ""GetToolTipData"", [""{6}""],
                    function (result) {{
                        self.updateContent(result.value);
                    }}, null, false, false);

                   }}
            }}

            }});
           }});";

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Empty Constructor.
        /// </summary>
        public ToolTipColumn()
        {
            ContentText = "Loading..";
            Text = "my tooltip";
            HideMethod = HideMethod.click;
            ShowMethod = ShowMethod.click;
            m_ColumnType = ColumnType.ToolTip;
            ToolTipWidth = 200;
            ToolTipHeight = 50;
        }

        /// <summary>
        /// UnknownColumn Constructor.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="table">The table object.</param>
        public ToolTipColumn(string columnName, Table table)
            : base(columnName, table)
        {
            ContentText = "Loading..";
            Text = "my tooltip";
            Grid = table.m_Grid;
            m_Table = table;
            ColumnId = columnName;
            m_ColumnType = ColumnType.ToolTip;
            HideMethod = HideMethod.click;
            ShowMethod = ShowMethod.click;
            ToolTipWidth = 200;
            ToolTipHeight = 50;
            CloseButtonText = "Close";
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Event is raised after row delete.
        /// </summary>
        [Category("Data"),
        Description(@"Event is raised after row delete.")]
        public event BeforeToolTipRenderEventHandler BeforeToolTipRender;

        #endregion Events

        #region Properties

        /// <summary>
        /// Tooltip height in pixels
        /// </summary>
        [Category("ToolTip"),
        Description(@"Close text")]
        public string CloseButtonText
        {
            get; set;
        }

        /// <summary>
        /// Tooltip height in pixels
        /// </summary>
        [Category("ToolTip"),
        Description(@"Tooltip height in pixels")]
        public string ContentText
        {
            get; set;
        }

        /// <summary>
        /// Behavior of a visible ToolTip.
        /// </summary>
        [Category("ToolTip"),
        Description(@"Behavior of a visible tool tip.")]
        public HideMethod HideMethod
        {
            get; set;
        }

        /// <summary>
        /// Ways to trigger the tooltip.
        /// </summary>
        [Category("ToolTip"),
        Description(@"Ways to trigger the tool tip.")]
        public ShowMethod ShowMethod
        {
            get; set;
        }

        /// <summary>
        /// Text for the Tooltip
        /// </summary>
        [Category("ToolTip"),
        Description(@"Text for the Tooltip")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public string Text
        {
            get; set;
        }

        /// <summary>
        /// Tooltip height in pixels
        /// </summary>
        [Category("ToolTip"),
        Description(@"Tooltip height in pixels")]
        public string ToolTipClientId
        {
            get; set;
        }

        /// <summary>
        /// Tooltip height in pixels
        /// </summary>
        [Category("ToolTip"),
        Description(@"Tooltip height in pixels")]
        public int ToolTipHeight
        {
            get; set;
        }

        /// <summary>
        /// Sets or gets the style
        /// </summary>
        [Category("ToolTip"),
        Description(@"Ways to trigger the tool tip.")]
        public StyleName ToolTipStyle
        {
            get; set;
        }

        /// <summary>
        /// Tooltip width in pixels
        /// </summary>
        [Category("ToolTip"),
        Description(@"Tooltip width in pixels")]
        public int ToolTipWidth
        {
            get; set;
        }

        /// <summary>
        /// Sets or gets if we allow multiple ToolTips
        /// </summary>
        [Category("ToolTip"),
        Description(@"Sets or gets if we allow multiple ToolTips")]
        internal bool SingleToolTip
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        internal override void CopyFrom(Column column)
        {
            base.CopyFrom(column);
            if (column.ColumnType != ColumnType.Number) return;
            ToolTipColumn c = (ToolTipColumn)column;
            BeforeToolTipRender = c.BeforeToolTipRender;
            HideMethod = c.HideMethod;
            ShowMethod = c.ShowMethod;
            ToolTipHeight = c.ToolTipHeight;
            ToolTipWidth = c.ToolTipWidth;
            ToolTipClientId = c.ToolTipClientId;
            ContentText = c.ContentText;
            Text = c.Text;
            CloseButtonText = c.CloseButtonText;
        }

        internal override Column CopyTo(Column column)
        {
            if (column.ColumnType == ColumnType.Number)
            {
                ToolTipColumn c = (ToolTipColumn)base.CopyTo(column);
                c.BeforeToolTipRender = BeforeToolTipRender;
                c.HideMethod = HideMethod;
                c.ShowMethod = ShowMethod;
                c.ToolTipHeight = ToolTipHeight;
                c.ToolTipWidth = ToolTipWidth;
                c.Text = Text;
                c.ContentText = ContentText;
                c.ToolTipClientId = ToolTipClientId;
                c.CloseButtonText = CloseButtonText;
                return c;
            }
            return base.CopyTo(column);
        }

        internal override Column CreateColumn()
        {
            return new ToolTipColumn(ColumnId, m_Table);
        }

        internal override Column Duplicate()
        {
            ToolTipColumn c = new ToolTipColumn(ColumnId, m_Table);
            CopyTo(c);
            return c;
        }

        internal string GetEventData(string columnName, string RowId, Row row)
        {
            if (BeforeToolTipRender != null)
            {

                BeforeToolTipRenderEventHandlerArgs ea = new BeforeToolTipRenderEventHandlerArgs { EditIndex = RowId, ColumnName = columnName, Row = row };
                try
                {
                    BeforeToolTipRender(this, ea);
                    return ea.ReturnData;
                }
                catch (Exception ee)
                {
                    return Grid.Debug
                               ? string.Format("Error in 'BeforeToolTipRender'  Column: {1}, RowId: {2}<br/><b>Error description</b><br/><br/>:{0}", ee,
                                               columnName, RowId)
                               : "Server-error loading. Use debug for more information.";
                }
            }
            return "null";
        }

        internal override void RenderEditView(HtmlTextWriter writer, RowCell cell)
        {
            RenderLabelView(writer, cell);
        }

        internal override void RenderLabelView(HtmlTextWriter writer, RowCell cell)
        {
            if (!Grid.EnableCallBack)
                throw new GridException("WebGrid must have callback enabled to use tooltip columns, you can enable this by setting 'Grid.EnableCallBack=true'");
            string clientId = ToolTipClientId ?? string.Format("{0}{1}TP{2}", Grid.ID, ColumnId, cell.Row.RowIndex);
            string closebutton = null;
            if (CloseButtonText != null)
                closebutton = string.Format(",button: '{0}'", CloseButtonText);

            Grid.AddClientScript(writer, string.Format(template, clientId,
                                                       Grid.ClientID, ShowMethod, HideMethod, ToolTipWidth,
                                                       ToolTipHeight,
                                                       string.Format("{0}!{1}", ColumnId, cell.Row.PrimaryKeyValues)
                                                       , ToolTipStyle, SingleToolTip.ToString().ToLower(), ContentText,
                                                       Title, closebutton));

            LabelHtml(string.Format("<div class=\"wgtoolelement\" id=\"{0}{1}TP{2}\">{3}</div>", Grid.ID, ColumnId,
                                    cell.Row.RowIndex, Text), writer, cell);
        }

        #endregion Methods
    }
}