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
    using System.Text;
    using System.Text.RegularExpressions;

    using Data;
    using Design;
    using Enums;
    using Events;
    using Util;

    /// <summary>
    /// The Number class is displayed as a column with a free text input box in the web interface.
    /// This column automatically validates all user input and allows most numerical values.
    /// This class inherits from the <see cref="WebGrid.Column">WebGrid.Column</see> class.
    /// </summary>
    public class Number : Column
    {
        #region Fields

        internal string m_Format = "N0";
        internal bool m_NumberSwap;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WebGrid.Number">Number</see> class.
        /// </summary>
        public Number()
        {
            GridAlign = HorizontalPosition.Left;
            m_ColumnType = ColumnType.Number;
            Searchable = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebGrid.Number">Number</see> class.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="table">The table object.</param>
        public Number(string columnName, Table table)
            : base(columnName, table)
        {
            Grid = table.m_Grid;
            m_Table = table;
            ColumnId = columnName;
            Searchable = false;
            GridAlign = HorizontalPosition.Left;
            EditAlign = HorizontalPosition.Right;
            m_ColumnType = ColumnType.Number;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Reformats the data in a column from one format to another. This applies to non-editable columns.
        /// </summary>
        /// <value>The format of integer value.</value>
        [Browsable(true),
        Category("Behavior"),
        DefaultValue("N0"),
        Description(
             @"Reformats the data in a column from one format to another. This applies to non-editable columns.")]
        public string Format
        {
            get { return m_Format; }
            set { m_Format = value; }
        }

        /// <summary>
        /// Reformats the data in a column from one format to another. This applies to non-editable columns.
        /// </summary>
        /// <value>The format of integer value.</value>
        [Browsable(true),
        Category("Behavior"),
        DefaultValue(false),
        Description(
             @"Number swap.")]
        public bool NumberSwap
        {
            get { return m_NumberSwap; }
            set
            {
                m_NumberSwap = value;
                if (value)
                    AllowEditInGrid = true;
            }
        }

        internal override string GenerateColumnMask
        {
            get
            {

                if (string.IsNullOrEmpty(MaskedInput))
                {
                    //Workarund so the space is reconized by the meiomask
                    string text = 999999999999.ToString(Format, Grid.Culture).Replace("0", "9");
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

        /// <summary>
        /// Display Value for number Columns
        /// </summary>
        public override string DisplayText(RowCell cell)
        {
            if (cell.PostBackValue == null || !Equals(cell.Value, cell.DataSourceValue))
                if (Value(cell) != null)
                    return ((long)cell.Value).ToString(Format, Grid.Culture);
                else if (cell.Value != null)
                    return cell.Value.ToString();
                else
                    return DefaultValue;
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

            if (!Equals(cell.Value, cell.DataSourceValue) && cell.Value is long)
                return true;
            if (!cell.GotPostBackData && (cell.Value != null && cell.Value is long == false))
                cell.PostBackValue = cell.Value.ToString();

            bool res = base.Validate(cell);

            if (res)
            {
                if (!string.IsNullOrEmpty(cell.PostBackValue))
                {

                    long result;
                    if (!long.TryParse(Regex.Replace(cell.PostBackValue, @"\s", string.Empty), NumberStyles.Any | NumberStyles.AllowTrailingWhite |
                                            NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite |
                                            NumberStyles.AllowTrailingWhite, Grid.Culture, out result))
                    {
                        if (string.IsNullOrEmpty(SystemMessage) == false)
                            Grid.SystemMessage.Add(SystemMessage, SystemMessageStyle,
                                                   cell.Row.GetColumnInitKeys(ColumnId));
                        else if (Grid.DisplayView == DisplayView.Grid && SystemMessageStyle != SystemMessageStyle.WebGrid)
                            Grid.SystemMessage.Add(String.Format(Grid.GetSystemMessage("SystemMessage_Grid_Int")),
                                                   SystemMessageStyle, cell.Row.GetColumnInitKeys(ColumnId));
                        else
                            Grid.SystemMessage.Add(
                                String.Format(
                                    string.Format("{0}({1})", Grid.GetSystemMessage("SystemMessage_Int"), cell.Value),
                                    Title),
                                SystemMessageStyle, cell.Row.GetColumnInitKeys(ColumnId));
                        res = false;
                    }
                    else
                        cell.Value = result;
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

            long result;
            if (
                !long.TryParse(Regex.Replace(cell.PostBackValue, @"\s", string.Empty),
                               NumberStyles.Any | NumberStyles.AllowTrailingWhite |
                               NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite |
                               NumberStyles.AllowTrailingWhite, Grid.Culture, out result))
                return null;
            return result;
        }

        internal override void CopyFrom(Column column)
        {
            base.CopyFrom(column);
            if (column.ColumnType != ColumnType.Number) return;
            Number c = (Number)column;
            m_Format = c.m_Format;
            m_NumberSwap = c.m_NumberSwap;
        }

        internal override Column CopyTo(Column column)
        {
            if (column.ColumnType == ColumnType.Number)
            {
                Number c = (Number)base.CopyTo(column);
                c.m_Format = m_Format;
                c.m_NumberSwap = m_NumberSwap;
                return c;
            }
            return base.CopyTo(column);
        }

        internal override Column CreateColumn()
        {
            return new Number(ColumnId, m_Table);
        }

        internal override Column Duplicate()
        {
            Number c = new Number(ColumnId, m_Table);
            CopyTo(c);
            return c;
        }

        internal override void OnUpdateInsert(CellUpdateInsertArgument ea, RowCell cell)
        {
            base.OnUpdateInsert(ea, cell);
            ea.AddApostrophe = false;
        }

        internal override void RenderEditView(WebGridHtmlWriter writer, RowCell cell)
        {
            if (Identity || AllowEdit == false || (Grid.DisplayView == DisplayView.Grid && AllowEditInGrid == false))
            {
                RenderLabelView(writer, cell);
                return;
            }
            #region Number Swap
            if (NumberSwap)
            {
                try
                {
                    if (Grid.DisplayView == DisplayView.Detail)
                        return;

                    StringBuilder tmp = new StringBuilder();
                    string buttonup = null;
                    string buttondown = null;

                    int myinx = m_Table.Rows.IndexOf(cell.Row);
                    if (myinx == -1)
                        throw new GridException("Could not locate row");

                    if (myinx <= m_Table.Rows.Count - 1 && myinx != 0)
                    {

                        string value = m_Table.Rows[myinx - 1][ColumnId].Value != null ? m_Table.Rows[myinx - 1][ColumnId].Value.ToString() : cell.Row.RowIndex.ToString();
                        string myvalue = cell.Value != null ? cell.Value.ToString() : "0";
                        buttonup = Buttons.TextButtonControl(Grid,
                                                             string.Format(@"<span title=""push row one step up"" class=""ui-icon ui-icon-triangle-1-n""/>"),
                                                             "columnpushup",
                                                             new[]
                                                             {
                                                                 cell.Row.PrimaryKeyValues,
                                                                 m_Table.Rows[myinx - 1].PrimaryKeyValues,
                                                                 ColumnId, myvalue,
                                                                 value
                                                             },
                                                             Grid.GetSystemMessageClass("UpdateRow", "wgUpdateRow"));
                    }
                   if (myinx != m_Table.Rows.Count - 1)
                    {
                        string value = m_Table.Rows[myinx + 1][ColumnId].Value != null ? m_Table.Rows[myinx + 1][ColumnId].Value.ToString() : cell.Row.RowIndex.ToString();
                        string myvalue = cell.Value != null ? cell.Value.ToString() : "0";
                        buttondown = Buttons.TextButtonControl(Grid,
                                                               string.Format(@"<span title=""push row one step down"" class=""ui-icon ui-icon-triangle-1-s""/>"),
                                                               "columnpushdown",
                                                               new[]
                                                               {
                                                                   cell.Row.PrimaryKeyValues,
                                                                   m_Table.Rows[myinx + 1].PrimaryKeyValues,
                                                                   ColumnId, myvalue,
                                                                   value
                                                               },
                                                               Grid.GetSystemMessageClass("UpdateRow", "wgUpdateRow"));
                    }
                   if (buttonup != null && buttondown != null)
                       tmp.AppendFormat("<table cellspacing=\"0\" cellpadding=\"0\" class=\"wgrowpush\"><tr><td>{0}</td><td>{1}</td></tr></table>", buttonup, buttondown);
                   else if (buttonup != null)
                       tmp.Append(buttonup);
                   else if (buttondown != null)
                       tmp.Append(buttondown);
                   RenderGrid(tmp.ToString(), writer, cell);
                }
                catch (Exception ex)
                {
                    throw new GridException(ex.ToString());
                }
                return;
            }
            #endregion

            base.RenderEditView(writer, cell);
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
        internal new virtual long? Value(RowCell cell)
        {
            LoadValueFromColumnDataSource(cell);

            if (cell.Value is long)
                return (long)cell.Value;
            if (cell.Value == null)
                return null;
            long result;
            if (long.TryParse(Regex.Replace(cell.Value.ToString(), @"\s", string.Empty), NumberStyles.Any | NumberStyles.AllowTrailingWhite |
                                        NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite |
                                        NumberStyles.AllowTrailingWhite, Grid.Culture, out result))
                return (long?)(cell.Value = result);
            return null;
        }

        #endregion Methods
    }
}