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
    using System.Globalization;
    using System.IO;

    using Collections;
    using Data;
    using Design;
    using Enums;
    using Events;

    /// <summary>
    /// The Decimal class is displayed as a column with a free text input box in the web interface.
    /// This column automatically validates all user input and allows most float values.
    /// This class inherits from the <see cref="WebGrid.Column">WebGrid.Column</see> class.
    /// </summary>
    [DesignTimeVisible(true),
    Browsable(true),
    EditorBrowsable(EditorBrowsableState.Always)]
    public class Decimal : Column
    {
        #region Fields

        internal bool? m_DisplayTotalSummary;

        private string m_Format = "N2";
        private decimal m_GetColumnSum;
        private string m_Sum;
        private decimal m_Totalsum;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WebGrid.Decimal">Decimal</see> class.
        /// </summary>
        public Decimal()
        {
            m_ColumnType = ColumnType.Decimal;
            Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebGrid.Decimal">Decimal</see> class.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="table">The column grid.</param>
        public Decimal(string columnName, Table table)
            : base(columnName, table)
        {
            Grid = table.m_Grid;
            m_Table = table;
            ColumnId = columnName;
            m_ColumnType = ColumnType.Decimal;

            Init();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Sets or Gets whether total summary should be shown at bottom of the Grid. Default is false;
        /// This property is by default activated if any grid column is using the "Sum" Property (Applies only to Decimal columns.)
        /// </summary>
        /// <value><c>true</c> if [display total summary]; otherwise, <c>false</c>.</value>
        [Browsable(true),
        Category("Behavior"),
        Description(
             @"Sets or gets whether total sum should be shown at the bottom of the grid. This applies to grid view."
             )]
        public bool DisplayTotalSummary
        {
            get { return TristateBoolType.ToBool(m_DisplayTotalSummary, false); }
            set { m_DisplayTotalSummary = value; }
        }

        /// <summary>
        /// Reformats the data in the column from one type to another.
        /// </summary>
        /// <value>The desired format.</value>
        [Browsable(true),
        Category("Behavior"),
        DefaultValue("N2"),
        Description(@"Reformats the data in the column from one type to another.")]
        public string Format
        {
            get { return m_Format; }
            set { m_Format = value; }
        }

        /// <summary>
        /// Use a mathematical expression to display the value for this column. Other columns in the expression
        /// should be represented by the name property surrounded by [ and ].
        /// Example: "3*(3+4)/(5*2) or "([mnyCost]*[intQuantity])"
        /// This property applies to grid view.
        /// </summary>
        /// <value>A mathematical expression.</value>
        /// The property can be disabled by setting Sum to null (nothing).
        [Browsable(true),
        Category("Behavior"),
        Description(
             @"Use a mathematical expression to display the value for this column. Other columns in the expression should be represented by the name property surrounded by [ and ]. Example: 3*(3+4)/(5*2) or ([mnyCost]*[intQuantity]). This property applies to grid view."
             )]
        public string Sum
        {
            get { return m_Sum; }
            set { m_Sum = value; }
        }

        /// <summary>
        /// Sets or gets the total summary for this column over all visibility rows. This applies to grid view.
        /// </summary>
        /// <value>The total sum.</value>
        [Browsable(false),
        Description(
             @"Sets or gets the total summary for this column over all visible rows. This applies to grid view.")]
        public decimal TotalSum
        {
            get
            {
                return m_Totalsum;
            }
            set
            {
                m_Totalsum = value;
            }
        }

        internal override string GenerateColumnMask
        {
            get
            {

                if (string.IsNullOrEmpty(MaskedInput))
                {
                    //Workarund so the space is reconized by the meiomask
                    string text = 999999999999.0m.ToString(Format, Grid.Culture).Replace("0", "9");
                    text = Grid.m_Whitespaceregexp.Replace(text, " ");
                    char[] str = text.ToCharArray();
                    Array.Reverse(str);

                    return string.Format("'{0}', type : 'reverse', defaultValue : '+0'", new string(str));
                }
                return MaskedInput;
            }
        }

        #endregion Properties

        #region Methods

        ///<summary>
        ///</summary>
        ///<param name="expression"></param>
        ///<returns></returns>
        public static double Evaluate(string expression)
        {
            return (double)new System.Xml.XPath.XPathDocument
            (new StringReader("<r/>")).CreateNavigator().Evaluate
            (string.Format("number({0})", new
            System.Text.RegularExpressions.Regex(@"([\+\-\*])")
            .Replace(expression, " ${1} ")
            .Replace("/", " div ")
            .Replace("%", " mod ")));
        }

        /// <summary>
        /// Display Value for decimal column
        /// </summary>
        public override string DisplayText(RowCell cell)
        {
            if (!Equals(cell.Value, cell.DataSourceValue) || cell.PostBackValue == null)
                if (Value(cell) != null)
                    return ((System.Decimal)cell.Value).ToString(Format, Grid.Culture);
                else if (cell.Value != null) return cell.Value.ToString();
                else return DefaultValue;
            return cell.PostBackValue;
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns></returns>
        public override bool Validate(RowCell cell)
        {
            if (DenyValidate())
                return true;

            if (!Equals(cell.Value, cell.DataSourceValue) && cell.Value is System.Decimal)
                return true;
            if (!cell.GotPostBackData && (cell.Value != null && cell.Value is System.Decimal == false))
                cell.PostBackValue = cell.Value.ToString();

            bool res = base.Validate(cell);

            if (res)
            {
                if (!string.IsNullOrEmpty(cell.PostBackValue))
                {
                    string myValue = Grid.m_Whitespaceregexp.Replace(cell.PostBackValue, string.Empty);
                    decimal f;
                    if (!decimal.TryParse(myValue,
                                         NumberStyles.Any | NumberStyles.AllowTrailingWhite |
                                         NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite |
                                         NumberStyles.AllowTrailingWhite, Grid.Culture,
                                         out f))
                    {
                        if (!string.IsNullOrEmpty(SystemMessage))
                            Grid.SystemMessage.Add(SystemMessage, SystemMessageStyle,
                                                   cell.Row.GetColumnInitKeys(ColumnId));
                        else if (Grid.DisplayView == DisplayView.Grid && SystemMessageStyle != SystemMessageStyle.WebGrid)
                            Grid.SystemMessage.Add(String.Format(Grid.GetSystemMessage("SystemMessage_Grid_dec")),
                                                   SystemMessageStyle,
                                                   cell.Row.GetColumnInitKeys(ColumnId));
                        else
                            Grid.SystemMessage.Add(String.Format(Grid.GetSystemMessage("SystemMessage_Dec"), Title),
                                                   SystemMessageStyle,
                                                   cell.Row.GetColumnInitKeys(ColumnId));
                        res = false;
                    }
                    else
                        cell.Value = f;
                }
                else
                    cell.Value = null;
            }
            else
                return false;

            if (Value(cell) != null)
            {
                if (MaxSize != 0 && Value(cell) > MaxSize)
                {

                    if (!String.IsNullOrEmpty(SystemMessage))
                        Grid.SystemMessage.Add(SystemMessage, SystemMessageStyle,
                                               cell.Row.GetColumnInitKeys(ColumnId));
                    else if (Grid.DisplayView == DisplayView.Grid && SystemMessageStyle != SystemMessageStyle.WebGrid)
                        Grid.SystemMessage.Add(
                            string.Format("Maximum allowed value is {0}.", MaxSize.ToString(Format, Grid.Culture)),
                            SystemMessageStyle,
                            cell.Row.GetColumnInitKeys(ColumnId));
                    else
                        Grid.SystemMessage.Add(
                            string.Format("Maximum allowed value for '{0}' is {1}.", Title,
                                          MaxSize.ToString(Format, Grid.Culture)),
                            SystemMessageStyle, cell.Row.GetColumnInitKeys(ColumnId));
                    return false;
                }

                if (MinSize != 0 && Value(cell) < MinSize)
                {
                    if (!String.IsNullOrEmpty(SystemMessage))
                        Grid.SystemMessage.Add(SystemMessage, SystemMessageStyle,
                                               cell.Row.GetColumnInitKeys(ColumnId));
                    else if (Grid.DisplayView == DisplayView.Grid && SystemMessageStyle != SystemMessageStyle.WebGrid)
                        Grid.SystemMessage.Add(
                            string.Format("Minimum allowed value is {0}.", MinSize.ToString(Format, Grid.Culture)),
                            SystemMessageStyle,
                            cell.Row.GetColumnInitKeys(ColumnId));
                    else
                        Grid.SystemMessage.Add(
                            string.Format("Minimum allowed value for '{0}' is {1}.", Title,
                                          MinSize.ToString(Format, Grid.Culture)),
                            SystemMessageStyle, cell.Row.GetColumnInitKeys(ColumnId));
                    return false;
                }
            }

            return res;
        }

        /// <summary>
        /// Validates a postback value for this column.
        /// </summary>
        /// <param name="cell">The cell with postback value</param>
        /// <returns>returns null if failed to validate else the column object type</returns>
        public override object ValidateColumnPostBackValue(RowCell cell)
        {
            if (string.IsNullOrEmpty(cell.PostBackValue))
                return null;

            string myValue = Grid.m_Whitespaceregexp.Replace(cell.PostBackValue, string.Empty);
            decimal f;
            if (!decimal.TryParse(myValue,
                                  NumberStyles.Any | NumberStyles.AllowTrailingWhite |
                                  NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite |
                                  NumberStyles.AllowTrailingWhite, Grid.Culture,
                                  out f) == false)
                return null;
            return f;
        }

        internal override void CopyFrom(Column column)
        {
            base.CopyFrom(column);
            switch (column.ColumnType)
            {
                case ColumnType.Decimal:
                    {
                        Decimal c = (Decimal)column;
                        m_Format = c.m_Format;
                        m_Sum = c.m_Sum;
                        m_Totalsum = c.m_Totalsum;
                        m_DisplayTotalSummary = c.m_DisplayTotalSummary;
                    }
                    break;
                case ColumnType.Number:
                    m_Format = ((Number)column).m_Format;
                    break;
            }
        }

        internal override Column CopyTo(Column column)
        {
            if (column.ColumnType == ColumnType.Decimal)
            {
                Decimal c = (Decimal)base.CopyTo(column);
                c.m_Format = m_Format;
                c.m_Sum = m_Sum;
                c.m_Totalsum = m_Totalsum;
                c.m_DisplayTotalSummary = m_DisplayTotalSummary;
                return c;
            }
            return base.CopyTo(column);
        }

        internal override Column CreateColumn()
        {
            return new Decimal(ColumnId, m_Table);
        }

        internal override Column Duplicate()
        {
            Decimal c = new Decimal(ColumnId, m_Table);
            CopyTo(c);
            return c;
        }

        internal decimal GetSum(RowCell cell)
        {
            if (Sum == null)
                return m_GetColumnSum;
            string mathExpression = Sum.ToLowerInvariant();
            if (Grid.Trace.IsTracing)
                Grid.Trace.Trace("Starting getting data for math. ({0})", ColumnId);
            ColumnCollection columns = m_Table.Columns;
            foreach (Column column in columns)
            {
                if (mathExpression.IndexOf(string.Format("[{0}]", column.ColumnId), StringComparison.OrdinalIgnoreCase) == -1)
                    continue;
                if (cell.Row[column.ColumnId].Value == null || cell.Row[column.ColumnId].Value == DBNull.Value)
                    mathExpression = mathExpression.Replace(string.Format("[{0}]", column.ColumnId.ToLowerInvariant()),
                                                            "0");
                else
                {
                    object myValue = cell.Row[column.ColumnId].Value;
                    if (myValue != null)
                        mathExpression =
                            mathExpression.Replace(string.Format("[{0}]", column.ColumnId.ToLowerInvariant()),
                                                   myValue.ToString().Replace(",", "."));
                    else
                        mathExpression = "";
                }
            }
            try
            {
                if (Grid.Trace.IsTracing)
                    Grid.Trace.Trace("Ended getting data for math. ({0})", ColumnId);

                m_GetColumnSum = Convert.ToDecimal(Evaluate(mathExpression));
                TotalSum = TotalSum + m_GetColumnSum;
                if (Grid.Trace.IsTracing)
                    Grid.Trace.Trace("Successfully calculating expression: {0} = {1} (Total: {2})", mathExpression,
                                     m_GetColumnSum.ToString(Format, Grid.Culture), TotalSum);
            }
            catch (Exception ee)
            {
                throw new GridException(
                    string.Format("Error calculating expression: {0} for columnId: {1}", mathExpression, ColumnId), ee);
            }
            return m_GetColumnSum;
        }

        internal void Init()
        {
            GridAlign = HorizontalPosition.Left;
            EditAlign = HorizontalPosition.Right;
            Searchable = false;
        }

        internal override void OnUpdateInsert(CellUpdateInsertArgument ea, RowCell cell)
        {
            base.OnUpdateInsert(ea, cell);
            if (ea.IgnoreChanges) return;
            ea.AddApostrophe = false;
        }

        internal override void RenderEditView(WebGridHtmlWriter writer, RowCell cell)
        {
            if (Identity || AllowEdit == false || (Grid.DisplayView == DisplayView.Grid && AllowEditInGrid == false))
            {
                RenderLabelView(writer, cell);
                return;
            }

            if (Sum != null && cell.Row.RowType == RowType.DataRow)
            {
                decimal? unusedVar = GetSum(cell);
            }
            base.RenderEditView(writer, cell);
        }

        internal override void RenderLabelView(WebGridHtmlWriter writer, RowCell cell)
        {
            if (!string.IsNullOrEmpty(Sum) && cell.Row.RowType == RowType.DataRow)
                cell.Value = GetSum(cell);

            base.RenderLabelView(writer, cell);
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
        internal new virtual decimal? Value(RowCell cell)
        {
            LoadValueFromColumnDataSource(cell);
            if (cell.Value is decimal) return (decimal)cell.Value;
            if (cell.Value == null)
                return null;
            decimal result;
            if (
                decimal.TryParse(cell.Value.ToString(),
                                 NumberStyles.Any | NumberStyles.AllowTrailingWhite |
                                 NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite |
                                 NumberStyles.AllowTrailingWhite, Grid.Culture,
                                 out result))
                return (decimal?)(cell.Value = result);
            return null;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeDisplayTotalSummary()
        {
            return m_DisplayTotalSummary != null;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeTotalSum()
        {
            return false;
        }

        #endregion Methods
    }
}