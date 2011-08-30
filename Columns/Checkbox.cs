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
    using System.Drawing;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;

    using WebGrid.Data;
    using WebGrid.Design;
    using WebGrid.Enums;
    using WebGrid.Events;
    using WebGrid.Util;

    /// <summary>
    /// The Checkbox class is displayed as a column with a checkbox in the web interface.
    /// This class inherits from the <see cref="WebGrid.Column">WebGrid.Column</see> class.
    /// </summary>
    public class Checkbox : Column
    {
        #region Fields

        private const string FalseValue = "0";
        private const string TrueValue = "1";

        private string m_CheckedAlias;
        private string m_Displaylabel;
        private string m_UncheckedAlias;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WebGrid.Checkbox">Checkbox</see> class.
        /// </summary>
        public Checkbox()
        {
            Required = false;
            m_ColumnType = ColumnType.Checkbox;
            Searchable = false;
    
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebGrid.Checkbox">Checkbox</see> class.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="table">The grid.</param>
        public Checkbox(string columnName, Table table)
            : base(columnName, table)
        {
            Grid = table.m_Grid;
            m_Table = table;
            ColumnId = columnName;
            Searchable = false;
       
            Required = false;
            m_ColumnType = ColumnType.Checkbox;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Sets or gets an column grid Alias for a checked checkbox. The default is null (nothing).
        /// A checked checkbox normally being rendered with this column type can be replaced by
        /// the string provided. (This string can also be HTML.)
        /// </summary>
        /// <value>The text/HTML to display when checked.</value>
        [Browsable(true),
        Category("Behavior"),
        Description(@"Sets or gets an alias for a checked checkbox. Default is null (nothing).")]
        public string CheckedAlias
        {
            get { return m_CheckedAlias; }
            set { m_CheckedAlias = value; }
        }

        /// <summary>
        /// Sets or gets display label (text after checkbox) Default is null (nothing).
        /// </summary>
        /// <value>The text to be displayed.</value>
        [Browsable(true),
        Category("Behavior"),
        Description(@"Sets or gets display label (text after checkbox) Default is null (nothing).")]
        public string Displaylabel
        {
            get { return m_Displaylabel; }
            set { m_Displaylabel = value; }
        }

        /// <summary>
        /// Sets or gets the column grid Alias for an unchecked checkbox. Default is null (nothing).
        /// An unchecked checkbox normally being rendered with this column type can be replaced by
        /// the string provided. (This string can also be html.)
        /// </summary>
        /// <value>The text/HTML to display when unchecked.</value>
        [Browsable(true),
        Category("Behavior"),
        Description(@"Sets or gets the alias for an unchecked checkbox. Default is null (nothing).")]
        public string UncheckedAlias
        {
            get { return m_UncheckedAlias; }
            set { m_UncheckedAlias = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Display Value for number Columns
        /// </summary>
        public override string DisplayText(RowCell cell)
        {
            if (!Equals(cell.Value, cell.DataSourceValue) || cell.PostBackValue == null)
                if (!Value(cell))
                    return FalseValue;
                else if (Value(cell))
                    return TrueValue;
            return cell.PostBackValue;
        }

        /// <summary>
        /// Validates this instance. (Returns always true)
        /// </summary>
        /// <returns></returns>
        public override bool Validate(RowCell cell)
        {
            if (!Equals(cell.Value, cell.DataSourceValue) && cell.Value is Boolean)
                return true;
            if (!cell.GotPostBackData && (cell.Value != null && cell.Value is Boolean == false))
                cell.PostBackValue = cell.Value.ToString();
            if (string.IsNullOrEmpty(cell.PostBackValue))
                return true;
            bool res;
            if (cell.PostBackValue.Equals(TrueValue))
            {
                cell.Value = true;
                return true;
            }
            if (cell.PostBackValue.Equals(FalseValue))
            {
                cell.Value = false;
                return true;
            }
            if (bool.TryParse(cell.PostBackValue, out res))
            {
                cell.Value = res;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Validates a postback value for this column.
        /// </summary>
        /// <param name="cell">The cell with postback value</param>
        /// <returns>returns null if failed to validate else the column object type</returns>
        public override object ValidateColumnPostBackValue(RowCell cell)
        {
            if (string.IsNullOrEmpty(cell.PostBackValue))
                return false;
            if (cell.PostBackValue.Equals(TrueValue))
                return true;
            if (cell.PostBackValue.Equals(FalseValue))
                return false;
            bool res;
            if (bool.TryParse(cell.PostBackValue, out res))
                return res;
            return cell.Value;
        }

        // 2005.01.05 - Jorn - Gromfunksjon
        internal override void CopyFrom(Column column)
        {
            base.CopyFrom(column);
            if (column.ColumnType != ColumnType.Checkbox) return;
            Checkbox bt = (Checkbox)column;
            CheckedAlias = bt.CheckedAlias;
            UncheckedAlias = bt.UncheckedAlias;

            m_Displaylabel = bt.m_Displaylabel;
        }

        // 2005.01.05 - Jorn - Denne er bra
        internal override Column CopyTo(Column column)
        {
            if (column.ColumnType == ColumnType.Checkbox)
            {
                Checkbox bt = (Checkbox)base.CopyTo(column);
                bt.m_CheckedAlias = m_CheckedAlias;
                bt.m_UncheckedAlias = m_UncheckedAlias;

                bt.m_Displaylabel = m_Displaylabel;
                return bt;
            }
            return base.CopyTo(column);
        }

        /*
        internal override Column	CreateDesignColumn(DesignRow row)
        {
            DesignColumn designcolumn = new DesignColumn(row);
            Columns.Checkbox column = new Columns.Checkbox(this.ColumnId,this.Grid);
            column.CopyFrom(this);
            designcolumn.Elements.Add(column);
            return designcolumn;
        }
        */
        /// <summary>
        /// Creates the column.
        /// </summary>
        internal override Column CreateColumn()
        {
            return new Checkbox(ColumnId, m_Table);
        }

        // 2005.01.05 - Jorn - Denne skal ikke brukes....
        /*internal override Column	Duplicate()
        {
            Checkbox c = new Checkbox(this.ColumnId,this.Grid);
            c.CheckedAlias = this.CheckedAlias;
            c.UncheckedAlias = this.UncheckedAlias;
            this.CopyTo( c );
            return c;
        }*/
        // 2005.01.05 - Jorn - Denne er mye bedre! :D
        internal override Column Duplicate()
        {
            Checkbox c = new Checkbox(ColumnId, m_Table);
            CopyTo(c);
            return c;
        }

        internal override void GetColumnPostBackData(RowCell cell)
        {
            if (cell.GotPostBackData || !Grid.GotHttpContext)
                return;
            cell.GotPostBackData = true;
            string uniqueID = cell.CellClientId;

            // Check if we have a post back, need this since checkbox "returns null" both for unchecked and non-existing
            if (HttpContext.Current.Request[uniqueID + "_cb"] == null)
                return;

            cell.PostBackValue = HttpContext.Current.Request[uniqueID] ?? FalseValue;
        }

        // 2004.10.27, jorn - Added this, because bits must always be updated. Stupid HTML!
        internal override void OnUpdateInsert(CellUpdateInsertArgument ea, RowCell cell)
        {
            ea.AddApostrophe = false;
            ea.IgnoreChanges = false;
        }

        // 2005.01.09 - jorn -  String.Compare, string.Length
        // 2005.01.05 - Jorn -  Added check to see if there actually is a checkbox on "previous page".
        //                        Need to add a hidden field since checkbox "returns null" both for unchecked and non-existing
        internal void RenderDetail(bool enabled, WebGridHtmlWriter writer, RowCell cell)
        {
            if (AllowEdit == false || (Grid.DisplayView == DisplayView.Grid && AllowEditInGrid == false))
                enabled = false;

            string uniqueId = cell.CellClientId;
            string strValue = DisplayText(cell);
            const string checkedValue = TrueValue;
            const string uncheckedValue = FalseValue;

            StringBuilder sb;

            if (String.Compare(strValue, checkedValue, true) == 0 && CheckedAlias != null && enabled == false)
                sb = new StringBuilder(CheckedAlias);
            else if (String.Compare(strValue, uncheckedValue, true) == 0 && UncheckedAlias != null &&
                     enabled == false)
                sb = new StringBuilder(UncheckedAlias);
            else
            {
                string bitchecked = string.Empty;
                string bitEnabled = string.Empty;
                if (String.Compare(strValue, checkedValue, true) == 0)
                    bitchecked = " checked=\"checked\"";
                if (enabled == false)
                    bitEnabled = " disabled=\"disabled\"";

                StringBuilder javascript = new StringBuilder(string.Empty);
                StringBuilder onblur = new StringBuilder(" onblur=\"");
                if (Grid.InputHighLight != Color.Empty)
                {
                    javascript.AppendFormat(
                        " onfocus=\"this.accessKey = this.style.backgroundColor;this.style.backgroundColor='{0}';\"",
                        Grid.ColorToHtml(Grid.InputHighLight));
                    onblur.Append("this.style.backgroundColor=this.accessKey;");
                }
                if (Grid.ColumnChangedColour != Color.Empty)
                {
                    onblur.AppendFormat("isChanged(this,'{0}');", Grid.ColorToHtml(Grid.ColumnChangedColour));
                }
                onblur.Append("\"");

                if (AutoPostback || string.IsNullOrEmpty(ConfirmMessage) == false)
                {
                    StringBuilder eventScript = new StringBuilder(" onclick=\"");
                    if (string.IsNullOrEmpty(ConfirmMessage) == false)
                        eventScript.AppendFormat(" if(wgconfirm('{0}',this,'{1}')) ", ConfirmMessage.Replace("'", "\\'"), Grid.DialogTitle.Replace("'", "\\'"));
                    string link = Grid.EnableCallBack && !ForcePostBack ? Asynchronous.GetCallbackEventReference(Grid,
                                                                                               string.Format("ElementPostBack!{0}!{1}",
                                                                                                             ColumnId, cell.Row.PrimaryKeyValues), false,
                                                                                               string.Empty, string.Empty) : Grid.Page.ClientScript.GetPostBackEventReference(Grid,
                                                                                                                                                                              string.Format(
                                                                                                                                                                                  "ElementPostBack!{0}!{1}", ColumnId, cell.Row.PrimaryKeyValues));
                    eventScript.AppendFormat("{0}\"", link);

                    javascript.Append(eventScript);
                }
                javascript.Append(onblur);
                sb =
                     new StringBuilder(
                         string.Format(
                             "<input {0} type=\"checkbox\" {1} {2} {4}  id=\"cb_{3}\" name=\"{3}\" value=\"1\"/>",
                             javascript, bitchecked, bitEnabled, uniqueId, Attributes));
                if (string.IsNullOrEmpty(m_Displaylabel) == false)
                    sb.AppendFormat(
                        "<label class=\"wglabel\" id=\"label_{0}\" for=\"cb_{0}\">{1}</label>", uniqueId, m_Displaylabel);

                if (enabled)
                    writer.Write("<input type=\"hidden\" id=\"{0}_cb\" name=\"{0}_cb\" value=\"{1}\" />",
                                 uniqueId, checkedValue);
            }

            if (string.IsNullOrEmpty(ToolTipInput) == false)
                sb = new StringBuilder(Tooltip.Add(sb.ToString(), ToolTipInput));

            EditHtml(sb.ToString(), writer, cell);
        }

        internal override void RenderEditView(WebGridHtmlWriter writer, RowCell cell)
        {
            if (AllowEdit == false)
            {
                RenderLabelView(writer, cell);
                return;
            }
            m_IsFormElement = false;
            RenderDetail(true, writer, cell);
        }

        internal override void RenderLabelView(WebGridHtmlWriter writer, RowCell cell)
        {
            m_IsFormElement = false;
            RenderDetail(false, writer, cell);
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
        internal new virtual bool Value(RowCell cell)
        {
            LoadValueFromColumnDataSource(cell);
            if (cell.Value == null)
                return false;
            if (cell.Value is bool)
                return (bool)cell.Value;
            bool result;
            if (bool.TryParse(Regex.Replace(cell.Value.ToString(), @"\s", string.Empty), out result))
                return (bool)(cell.Value = result);
            return false;
        }

        #endregion Methods
    }
}