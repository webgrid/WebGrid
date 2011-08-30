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

namespace WebGrid.Data
{
    using System;
    using System.ComponentModel;
    using System.Text;

    using Util;

    /// <summary>
    /// WebGrid Row Cell
    /// </summary>
    public class RowCell
    {
        #region Fields

        internal object m_DataSourceValue; // Value in DB

        [NonSerialized]
        private bool _gotPostBackData;
        private string _postBackValue;
        private object m_ColumnValue;
        private object m_DisplayValue;
        private string m_StoredFormElementName;
        private Row m_row;

        #endregion Fields

        #region Constructors

        ///<summary>Creates a cell for a row
        ///</summary>
        ///<param name="cellId">Cell identifier</param>
        ///<param name="row">Row this cell belongs to</param>
        public RowCell(string cellId, Row row)
        {
            CellId = cellId;
            m_row = row;
            _gotPostBackData = false;
        }

        ///<summary>Creates a cell for a row
        ///</summary>
        ///<param name="cellId">Cell identifier</param>
        ///<param name="row">Row this cell belongs to</param>
        ///<param name="value">value for this cell</param>
        public RowCell(string cellId, Row row, object value)
        {
            CellId = cellId;
            m_row = row;
            _gotPostBackData = false;
            Value = value;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the HTML form element for the grid.
        /// </summary>
        /// <value>The name of the form element.</value>
        // Returns the name of the HTML form element.
        public string CellClientId
        {
            get
            {
                if (m_StoredFormElementName != null)
                    return m_StoredFormElementName;
                StringBuilder elementName = new StringBuilder(m_row.m_Table.m_Grid.ClientID);
                elementName.Append("_");
                if (Row != null)
                    elementName.Append(m_row.m_Table.m_Grid.m_IsCopyClick
                                           ? "__"
                                           : Row.PrimaryKeyValues.Replace(",", "__"));
                elementName.Append("_");
                elementName.Append(CellId);

                m_StoredFormElementName = elementName.Replace(Grid.EMPTYSTRINGCONSTANT, "__").ToString();
                return m_StoredFormElementName;
            }
        }


        private int _InternalcellSpan = 1;
        private int _cellSpan;



        /// <summary>
        /// Sets or gets colspan for the cell. Default is 0
        /// </summary>
        public int ExtraCellSpan
        {
            get { return _cellSpan; }
            set { _cellSpan = value; }
          
        }
        
        internal int InternalCellSpan
        {
            get { return _InternalcellSpan + _cellSpan; }
            set { _InternalcellSpan = value; }
        }

        /// <summary>
        /// Name of this Cell
        /// </summary>
        public string CellId
        {
            get; private set;
        }

        /// <summary>
        /// Gets the data source content reference for this column.
        /// DataSourceDisplayValue is a string or integer value that is a reference to a relation in your data source.
        /// </summary>
        /// <value>The data source reference ColumnId.</value>
        [Browsable(false),
        Description(@"Gets the data source content reference for this column.")]
        public object DataSourceDisplayValue
        {
            get; internal set;
        }

        /// <summary>
        /// Gets the data source content for this column.
        /// </summary>
        /// <value>The old value.</value>
        /// <remarks>
        /// Used to check if a value has been changed.
        /// </remarks>
        [Browsable(false),
        Category("Data"),
        Description(@"Gets the data source content for this column.")]
        public object DataSourceValue
        {
            get { return m_DataSourceValue; }
            internal set { m_DataSourceValue = value; }
        }

        /// <summary>
        /// Sets or gets the actual content reference for this column. You can also use this property for setting a default value for new records.
        /// DisplayText is a string or integer value that is reference to relation in your data source.
        /// </summary>
        /// <value>The value ColumnId.</value>
        [Browsable(true),
        Category("Foreign key options"),
        Description(
             @"Sets or gets the actual content reference for this column. You can also use this property for setting a default value for new records."
             )]
        public object DisplayValue
        {
            get { return m_DisplayValue; }
            set { m_DisplayValue = value; }
        }

        /// <summary>
        /// Set or gets if this cell got postback data.
        /// </summary>
        public bool GotPostBackData
        {
            get { return _gotPostBackData; }
            set { _gotPostBackData = value; }
        }

        /// <summary>
        /// Postback value for the column
        /// </summary>
        public string PostBackValue
        {
            get
            {
                if (!_gotPostBackData)
                {
                    GetCellPostBackData();
                }
                return _postBackValue;
            }
            internal set { _postBackValue = value; }
        }

        /// <summary>
        /// Gets or sets the row.
        /// </summary>
        /// <value>The row.</value>
        [Browsable(false),
        ]
        public Row Row
        {
            get { return m_row; }
            set { m_row = value; }
        }

        // Value from post back
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
        public object Value
        {
            get
            {
                return m_ColumnValue;
            }
            set
            {
                m_ColumnValue = value;

            }
        }

        internal object GetApostropheDisplayValue
        {
            get { return (DisplayValue == null || DisplayValue is int) ? DisplayValue : string.Format("'{0}'", DisplayValue); }
        }

        internal object GetApostropheValue
        {
            get
            {
                switch (Row.Columns[CellId].HasDataSourceQuote)
                {
                    case true:
                        return string.Format("'{0}'", Value);
                    default:
                        if ((Value == null) || Validate.IsNumeric(Value.ToString().Trim())) return Value;
                        return string.Format("'{0}'", Value);
                }
            }
        }

       

        #endregion Properties

        #region Methods

        internal void GetCellPostBackData()
        {
            Row.Columns[CellId].GetColumnPostBackData(this);
        }

        #endregion Methods
    }
}