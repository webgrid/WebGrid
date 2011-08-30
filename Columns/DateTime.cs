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
    using System.Web;

    using Data;
    using Design;
    using Enums;

    /// <summary>
    /// The DateTime class is displayed as a column with a free text input box in the web interface.
    /// Normally a calendar icon is shown next to the input box (if the column is editable).
    /// This class inherits from the <see cref="WebGrid.Column">WebGrid.Column</see> class.
    /// </summary>
    public class DateTime : Column
    {
        #region Fields

        //     ,onSelect: function(dateText, inst) {{  this.focus();  }}
        private const string calendarsetup = 
            @"$(function() {{
                $(""#{0}"").datepicker({{ dateFormat: '{1}',showOn: 'button', buttonImage: '{3}', buttonImageOnly: true,
                onClose: function(dateText, inst) {{ this.focus();  }}
               {2}
                }});
            }});";

        private string m_AddCalendarClientAttributes = string.Empty;
        private string m_CalendarFormat;
        private bool m_DisplayCalendar = true;

        // 2005.01.09 - jorn, string optimize
        private string m_Format;
        private string m_Searchfilter;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WebGrid.DateTime">DateTime</see> class.
        /// </summary>
        public DateTime()
        {
            m_ColumnType = ColumnType.DateTime;
            Searchable = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebGrid.DateTime">DateTime</see> class.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="table">The table object.</param>
        public DateTime(string columnName, Table table)
            : base(columnName, table)
        {
            Grid = table.m_Grid;
            m_Table = table;
            ColumnId = columnName;
            m_ColumnType = ColumnType.DateTime;
            Searchable = false;
            GridAlign = HorizontalPosition.Left;
            EditAlign = HorizontalPosition.Right;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Additional client script for the calendar.
        /// </summary>
        public string AddCalendarClientAttributes
        {
            get { return m_AddCalendarClientAttributes; }
            set { m_AddCalendarClientAttributes = value; }
        }

        /// <summary>
        /// Calendar format. See here for supported calendar formats: http://docs.jquery.com/UI/Datepicker/formatDate (default is dd.mm.yy)
        /// </summary>
        public string CalendarFormat
        {
            get
            {
                if (m_CalendarFormat == null && Format != null)
                {
                    //Compability issues for the calendar formats and .net formatting.
                    m_CalendarFormat = Format.Replace("MM", "mm").Replace("yyyy", "yy").Replace("M","m");
                }
                return m_CalendarFormat;
            }
            set
            {
                m_CalendarFormat = value;
            }
        }

        /// <summary>
        /// Sets or gets whether the calendar icon should be visibility or not. Default is true.
        /// The calendar icon is only visibility if the column is editable.
        /// </summary>
        /// <value><c>true</c> if [calendar]; otherwise, <c>false</c>.</value>
        [Browsable(true),
        DefaultValue(true),
        Description(@"Sets or gets whether the calendar icon should be visible or not. Default is true.")]
        public bool DisplayCalendar
        {
            get { return m_DisplayCalendar; }
            set { m_DisplayCalendar = value; }
        }

        /// <summary>
        /// Sets or gets the date/datetime format of the column. Default date format is retrieved from
        /// <see cref="WebGrid.Grid.DefaultDateTimeFormat">Grid.DefaultDateTimeFormat</see>.
        /// </summary>
        /// <value>The desired format.</value>
        [Browsable(true),
        Category("Behavior"),
        Description(
             @" Sets or gets the date/datetime format of the column. Default date format is retrieved from grid by the property ""DefaultDateTimeFormat"""
             )]
        public string Format
        {
            get { return string.IsNullOrEmpty(m_Format) ? Grid.DefaultDateTimeFormat : m_Format; }
            set { m_Format = value; }
        }

        internal override string GenerateColumnMask
        {
            get
            {
                //Only dd, MM, yyyy formats with a seperator is supported like "dd.MM.yyyy" or "MM.dd.yyyy"
                if (Format == null || Format.Length != 10)
                    return null;
                 return string.IsNullOrEmpty(MaskedInput) ? string.Format("'39{0}39{0}9999'",Grid.Culture.DateTimeFormat.DateSeparator) : MaskedInput;
            }
        }

        /// <summary>
        /// Gets if this column requires single quote for
        /// data source operations.
        /// </summary>
        internal override bool HasDataSourceQuote
        {
            get
            {
                return true;
            }
        }

        internal string SearchFilter
        {
            get
            {
                if (m_Searchfilter != null || !Grid.GotHttpContext)
                    return m_Searchfilter;

                string searchfrom = HttpContext.Current.Request[ColumnId + "_headerfrom"];
                string searchto = HttpContext.Current.Request[ColumnId + "_headerto"];

                System.DateTime result;
                if (string.IsNullOrEmpty(searchfrom) == false)
                    if (System.DateTime.TryParseExact(searchfrom, "yyyy/MM/dd", DateTimeFormatInfo.InvariantInfo,
                                                      DateTimeStyles.AllowWhiteSpaces, out result))
                        searchfrom = result.ToString("yyyyMMdd", Grid.Culture);
                if (string.IsNullOrEmpty(searchto) == false)
                    if (System.DateTime.TryParseExact(searchto, "yyyy/MM/dd", DateTimeFormatInfo.InvariantInfo,
                                                      DateTimeStyles.AllowWhiteSpaces, out result))
                        searchto = result.ToString("yyyyMMdd", Grid.Culture);

                string filter = string.Empty;

                if (string.IsNullOrEmpty(searchfrom) == false && string.IsNullOrEmpty(searchto) == false)
                    filter = string.Format("BETWEEN '{0}' AND '{1}'", searchfrom, searchto);
                else if (string.IsNullOrEmpty(searchfrom) == false && string.IsNullOrEmpty(searchto))
                    filter = string.Format(" >= '{0}'", searchfrom);
                else if (string.IsNullOrEmpty(searchfrom) && string.IsNullOrEmpty(searchto) == false)
                    filter = string.Format(" <= '{0}'", searchto);
                m_Searchfilter = filter;
                return filter;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Display Value for datetime column
        /// </summary>
        public override string DisplayText(RowCell cell)
        {
            if (!Equals(cell.Value, cell.DataSourceValue) || cell.PostBackValue == null)
                if (Value(cell) != null)
                    return ((System.DateTime)cell.Value).ToString(Format, Grid.Culture);
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

            if (!Equals(cell.Value, cell.DataSourceValue) && cell.Value is System.DateTime)
                return true;
            if (!cell.GotPostBackData && (cell.Value != null && cell.Value is System.DateTime == false))
                cell.PostBackValue = cell.Value.ToString();
            bool res = base.Validate(cell);

            if (!res)
                return true;
            if (string.IsNullOrEmpty(cell.PostBackValue))
            {
                cell.Value = null;
                return true;
            }

            System.DateTime result;
            if (
                !System.DateTime.TryParseExact(cell.PostBackValue, Format,CultureInfo.InvariantCulture,
                                               DateTimeStyles.None, out result))
            {
                if (string.IsNullOrEmpty(SystemMessage) == false)
                    Grid.SystemMessage.Add(SystemMessage, SystemMessageStyle,
                                           cell.Row.GetColumnInitKeys(ColumnId));
                else if (Grid.DisplayView == DisplayView.Grid && SystemMessageStyle != SystemMessageStyle.WebGrid)
                    Grid.SystemMessage.Add(
                        String.Format(Grid.GetSystemMessage("SystemMessage_Grid_date"), Format),
                        SystemMessageStyle, cell.Row.GetColumnInitKeys(ColumnId));
                else
                    Grid.SystemMessage.Add(
                        String.Format(Grid.GetSystemMessage("SystemMessage_Date"), Title, Format),
                        SystemMessageStyle, cell.Row.GetColumnInitKeys(ColumnId));
                return false;
            }
            cell.Value = result;
            return true;
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
            System.DateTime result;

            if (!System.DateTime.TryParseExact(cell.PostBackValue, Format, Grid.Culture,
                                               DateTimeStyles.None, out result))
                return null;
            return result;
        }

        internal override void CopyFrom(Column column)
        {
            base.CopyFrom(column);
            if (column.ColumnType != ColumnType.DateTime) return;
            DateTime c = (DateTime)column;
            m_Format = c.m_Format;
            m_DisplayCalendar = c.m_DisplayCalendar;
            m_CalendarFormat = c.m_CalendarFormat;
            m_AddCalendarClientAttributes = c.m_AddCalendarClientAttributes;
            m_Searchfilter = c.m_Searchfilter;
        }

        internal override Column CopyTo(Column column)
        {
            if (column.ColumnType == ColumnType.DateTime)
            {
                DateTime c = (DateTime)base.CopyTo(column);
                c.m_Format = m_Format;
                c.m_DisplayCalendar = m_DisplayCalendar;
                c.m_CalendarFormat = m_CalendarFormat;
                c.m_AddCalendarClientAttributes = m_AddCalendarClientAttributes;
                c.m_Searchfilter = m_Searchfilter;
                return c;
            }
            return base.CopyTo(column);
        }

        internal override Column CreateColumn()
        {
            return new DateTime(ColumnId, m_Table);
        }

        internal override Column Duplicate()
        {
            DateTime c = new DateTime(ColumnId, m_Table);
            CopyTo(c);
            return c;
        }

        /// <summary>
        /// This method is fired when loading data and is handled for each column type. 
        /// </summary>
        /// <param name="databaseValue"></param>
        /// <returns></returns>
        internal override object OnLoadFromDatabase(object databaseValue)
        {
            if (databaseValue == null || databaseValue == DBNull.Value)
                return null;
            if (databaseValue is System.DateTime)
                return (System.DateTime)databaseValue;
            System.DateTime result;

            if (System.DateTime.TryParse(databaseValue.ToString(), out result))
                return result;
            return (System.DateTime.TryParseExact(databaseValue.ToString(),
                                                  Grid.Culture.DateTimeFormat.GetAllDateTimePatterns(),
                                                  DateTimeFormatInfo.InvariantInfo,
                                                  DateTimeStyles.AllowWhiteSpaces, out result)
                        ? result
                        : (System.DateTime.TryParse(databaseValue.ToString(), new CultureInfo("en-us"),
                                                    DateTimeStyles.AdjustToUniversal, out result)
                               ? result
                               : databaseValue)) as System.DateTime?;
        }

        // 2005.01.09 - jorn, string optimize, String.Compare
        internal override void RenderEditView(WebGridHtmlWriter writer, RowCell cell)
        {
            if (AllowEdit == false || (Grid.DisplayView == DisplayView.Grid && AllowEditInGrid == false))
            {
                RenderLabelView(writer, cell);
                return;
            }

            base.RenderEditView(writer, cell);

            if (DisplayCalendar && Grid.GotHttpContext)
                Grid.AddClientScript(writer, string.Format(calendarsetup, cell.CellClientId,
                                                           CalendarFormat, AddCalendarClientAttributes,
                                                           Grid.Page.ClientScript.GetWebResourceUrl(Grid.GetType(),
                                                                                                    "WebGrid.Resources.images.calendar.gif")));
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
        internal new virtual System.DateTime? Value(RowCell cell)
        {
            LoadValueFromColumnDataSource(cell);
            if (cell.Value is System.DateTime) return (System.DateTime)cell.Value;
            if (cell.Value == null)
                return null;
            System.DateTime result;
            if (System.DateTime.TryParseExact(cell.Value.ToString(), Format, Grid.Culture,
                                           DateTimeStyles.None, out result))
                return (System.DateTime?)(cell.Value = result);
            return null;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeFormat()
        {
            return m_Format != null;
        }

        #endregion Methods
    }
}