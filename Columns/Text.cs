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
    using System.IO;
    using System.Text;
    using System.Web;
    using System.Web.UI.WebControls;

    using Config;
    using Data;
    using Design;
    using Enums;
    using Events;
    using Util;

    /// <summary>
    /// The Text class is displayed as a column with a free text input box in the web interface.
    /// This class inherits from the <see cref="WebGrid.Column">WebGrid.Column</see> class.
    /// </summary>
    public class Text : Column
    {
        #region Fields

        private const string HTTPCONST = "http://";

        private string m_ColumngridAlias;
        private string m_EncryptionKey;
        private string m_Format = string.Empty;
        private bool m_IsEmail;
        private bool m_IsHtml;
        private bool m_IsPassword;
        private bool m_IsUrl;
        private string m_PasswordString = "********";

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WebGrid.Text">Text</see> class.
        /// </summary>
        /// 
        public Text()
        {
            Searchable = true;
            m_ColumnType = ColumnType.Text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebGrid.Text">Text</see> class.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="table">The table object.</param>
        public Text(string columnName, Data.Table table)
            : base(columnName, table)
        {
            Grid = table.m_Grid;
            m_Table = table;
            ColumnId = columnName;
            Searchable = true;
            GridAlign = HorizontalPosition.Left;
            m_ColumnType = ColumnType.Text;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Sets or gets the grid alias for the text value. Default is null (not in use).
        /// This property affects only non-editable columns.
        /// The grid alias is used to hide long strings in grid view, like long URL and email addresses.
        /// </summary>
        /// <value>The alias string</value>
        [Browsable(true),
        Category("Text options"),
        Description(
             @"Sets or gets the alias for the text value. Default is null (not in use). This property affects only non-editable columns."
             )]
        public string ColumnGridAlias
        {
            get { return m_ColumngridAlias; }
            set { m_ColumngridAlias = value; }
        }

        /// <summary>
        /// Sets or gets the key used to encrypt the content of the column.
        /// This applies to non-editable column.
        /// </summary>
        /// <value>The encryption key.</value>
        [Browsable(true),
        Category("Text options"),
        Description(
             @"Sets or gets the key used to encrypt the content of the column. This applies to non-editable column."
             )]
        public string EncryptionKey
        {
            get { return m_EncryptionKey; }
            set
            {
                if (value == null)
                {
                    Searchable = false;
                    Sortable = false;
                }
                m_EncryptionKey = value;
            }
        }

        /// <summary>
        /// Sets or gets whether the column should validate the input to check if it's a valid email address.
        /// </summary>
        /// <value><c>true</c> if this instance is e-mail address; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// If the column is non-editable this will display the value (ie: the email address) as a clickable
        /// mailto link.
        /// </remarks>
        [Browsable(true),
        Category("Text options"),
        DefaultValue(false),
        Description(
             @"Sets or gets whether the column should validate the input to check if it's a valid email address."
             )]
        public bool IsEmail
        {
            get { return m_IsEmail; }
            set { m_IsEmail = value; }
        }

        /// <summary>
        /// Sets or gets whether the column is using a WYSIWYG (default size is 400x500 pixels) html-grid for input. Default is false.
        /// This property requires an MSHTML object for your browser (Will return false if internet explorer 5.5 or later is not used.)
        /// </summary>
        /// <value><c>true</c> if this instance is HTML; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// Create a http handler for this text editor.
        /// an example how this can be achived is putting this in your web.config
        /// <httpHandlers>
        ///     <add path="HtmlEditor.aspx" verb="*" type="WebGrid.Util.ControlHandler,WebGrid" />
        /// </httpHandlers>
        /// </remarks>
        [Browsable(true),
        Category("Text options"),
        DefaultValue(false),
        Description(
             @"Sets or gets whether the column is using a WYSIWYG (default size is 400x500 pixels) html-editor for input. Default is false. This property requires an MSHTML object for your browser (Will return false if internet explorer 5.5 or later is not used.)"
             )]
        public bool IsHtml
        {
            get { return m_IsHtml; }
            set { m_IsHtml = value; }
        }

        /// <summary>
        /// Sets or gets whether the column is a password column. If true the content will be hidden.
        /// This applies to editable columns.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is password; otherwise, <c>false</c>.
        /// </value>
        [Browsable(true),
        Category("Text options"),
        DefaultValue(false),
        Description(
             @"Sets or gets whether the column is a password column. If true the content will be hidden. This applies to editable columns."
             )]
        public bool IsPassword
        {
            get { return m_IsPassword; }
            set
            {
                if (value)
                {
                    Sortable = false;
                    Searchable = false;
                }
                m_IsPassword = value;
            }
        }

        /// <summary>
        /// This adds http:\\ (if needed) to the value (if any) and non-editable columns become clickable.
        /// </summary>
        /// <value><c>true</c> if this instance is URL; otherwise, <c>false</c>.</value>
        [Browsable(true),
        Category("Text options"),
        DefaultValue(false),
        Description(
             @"This adds http:\\ (if needed) to the value (if any) and non-editable columns become clickable.")]
        public bool IsUrl
        {
            get { return m_IsUrl; }
            set { m_IsUrl = value; }
        }

        /// <summary>
        /// Sets or gets what should be displayed instead of the password stored in the data source.
        /// This applies to non-editable columns.
        /// </summary>
        /// <value>The password- column grid Alias string.</value>
        /// <remarks>
        /// If PasswordString is null the password will be shown as plain text.
        /// </remarks>
        [Browsable(true),
        Category("Text options"),
        DefaultValue("********"),
        Description(
             @"Sets or gets what should be displayed instead of the password stored in the datasource. This applies to non-editable columns."
             )]
        public string PasswordString
        {
            get { return m_PasswordString; }
            set { m_PasswordString = value; }
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

        #endregion Properties

        #region Methods

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns></returns>
        public override bool Validate(RowCell cell)
        {
            if (DenyValidate())
                return true;
            bool res = base.Validate(cell);

            if (res)
            {
                if (AllowEmpty && string.IsNullOrEmpty(cell.PostBackValue))
                {
                    if (cell.GotPostBackData)
                        cell.Value = cell.PostBackValue;
                    return true;
                }

                if (IsEmail && Mail.ValidEmails(cell.PostBackValue) == false)
                {
                    if (string.IsNullOrEmpty(SystemMessage) == false)
                        Grid.SystemMessage.Add(SystemMessage, SystemMessageStyle,
                                               cell.Row.GetColumnInitKeys(ColumnId));
                    else if (Grid.DisplayView == DisplayView.Grid && SystemMessageStyle != SystemMessageStyle.WebGrid)
                        Grid.SystemMessage.Add(String.Format(Grid.GetSystemMessage("SystemMessage_Grid_email")),
                                               SystemMessageStyle, cell.Row.GetColumnInitKeys(ColumnId));
                    else
                        Grid.SystemMessage.Add(String.Format(Grid.GetSystemMessage("SystemMessage_Email"), Title),
                                               SystemMessageStyle, cell.Row.GetColumnInitKeys(ColumnId));
                    return false;
                }
            }
            else
                return false;
            if (cell.PostBackValue != null && MaxSize > 0 && cell.PostBackValue.Length > MaxSize)
            {
                if (!String.IsNullOrEmpty(SystemMessage))
                    Grid.SystemMessage.Add(SystemMessage, SystemMessageStyle,
                                           cell.Row.GetColumnInitKeys(ColumnId));
                else if (Grid.DisplayView == DisplayView.Grid && SystemMessageStyle != SystemMessageStyle.WebGrid)
                    Grid.SystemMessage.Add(
                        String.Format(Grid.GetSystemMessage("SystemMessage_Grid_maxLength"), MaxSize),
                        SystemMessageStyle, cell.Row.GetColumnInitKeys(ColumnId));
                else
                    Grid.SystemMessage.Add(
                        String.Format(Grid.GetSystemMessage("SystemMessage_MaxLength"), Title, MaxSize),
                        SystemMessageStyle, cell.Row.GetColumnInitKeys(ColumnId));
                return false;
            }

            if (cell.PostBackValue != null && MinSize > 0 && cell.PostBackValue.Length < MinSize)
            {
                if (!String.IsNullOrEmpty(SystemMessage))
                    Grid.SystemMessage.Add(SystemMessage, SystemMessageStyle,
                                           cell.Row.GetColumnInitKeys(ColumnId));
                else if (Grid.DisplayView == DisplayView.Grid && SystemMessageStyle != SystemMessageStyle.WebGrid)
                    Grid.SystemMessage.Add(
                        String.Format(Grid.GetSystemMessage("SystemMessage_Grid_minLength"), MinSize),
                        SystemMessageStyle, cell.Row.GetColumnInitKeys(ColumnId));
                else
                    Grid.SystemMessage.Add(
                        String.Format(Grid.GetSystemMessage("SystemMessage_MinLength"), Title, MinSize),
                        SystemMessageStyle, cell.Row.GetColumnInitKeys(ColumnId));
                return false;
            }
            cell.Value = cell.PostBackValue;
            return true;
        }

        internal override void CopyFrom(Column column)
        {
            base.CopyFrom(column);
            if (column.ColumnType != ColumnType.Text)
                return;
            Text c = (Text) column;
            c.m_IsHtml = m_IsHtml;
            c.m_IsEmail = m_IsEmail;
            c.m_IsUrl = m_IsUrl;
            c.m_IsPassword = m_IsPassword;
            c.m_Format = m_Format;
            c.m_ColumngridAlias = m_ColumngridAlias;
            c.m_EncryptionKey = m_EncryptionKey;
        }

        internal override Column CopyTo(Column column)
        {
            if (column.ColumnType == ColumnType.Text)
            {
                Text c = (Text) base.CopyTo(column);
                c.m_IsHtml = m_IsHtml;
                c.m_IsEmail = m_IsEmail;
                c.m_IsUrl = m_IsUrl;
                c.m_IsPassword = m_IsPassword;
                c.m_Format = m_Format;
                c.m_EncryptionKey = m_EncryptionKey;
                c.m_ColumngridAlias = m_ColumngridAlias;

                return c;
            }
            return base.CopyTo(column);
            //return null;
        }

        internal override Column CreateColumn()
        {
            return new Text(ColumnId, m_Table);
        }

        internal string DrawHtml(string uniqueID, WebGridHtmlWriter writer, Grid grid,RowCell cell)
        {
            string theValueToShow = Value(cell);

            HtmlEditor e = new HtmlEditor
                               {
                                   ID = uniqueID,
                                   ImagePath = Grid.ImagePath,
                                   UserBRonCarriageReturn = true,
                                   Width = (WidthEditableColumn != Unit.Empty ? WidthEditableColumn : 500),
                                   Height = (HeightEditableColumn != Unit.Empty ? HeightEditableColumn : 400),
                                   Text = theValueToShow
                               };

            e.ImagePath = GridConfig.Get("WGEditorImagePath", grid.ImagePath);
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            System.Web.UI.HtmlTextWriter mywriter = new System.Web.UI.HtmlTextWriter(sw);
            e.RenderControl(mywriter);
            mywriter.Flush();
            mywriter.Close();

            return sb.ToString();
        }

        internal override Column Duplicate()
        {
            Text c = new Text(ColumnId, m_Table);
            CopyTo(c);
            return c;
        }

        // 2005.01.09 - jorn, optimize
        internal override void GetColumnPostBackData(RowCell cell)
        {
            base.GetColumnPostBackData(cell);
            if (!IsUrl || string.IsNullOrEmpty(Value(cell)))
                return;

            // Fix input errors ( usual of users...)
            if (String.Compare(Value(cell), 0, "http://http://", 0, 14, true) == 0)
                cell.Value = HTTPCONST + Value(cell).Remove(0, 14);
            if (String.Compare(Value(cell), 0, HTTPCONST, 0, 7, true) != 0)
                cell.Value = HTTPCONST + Value(cell);
            if (String.Compare(Value(cell), HTTPCONST, true) == 0)
                cell.Value = null;
        }

        internal override void OnUpdateInsert(CellUpdateInsertArgument ea, RowCell cell)
        {
            base.OnUpdateInsert(ea, cell);
            if (ea.IgnoreChanges)
                return;

            if (EncryptionKey != null && ea.Value != null) // Decrypt ?
                ea.Value = Security.Encrypt(ea.Value.ToString(), EncryptionKey);
        }

        internal override void RenderEditView(WebGridHtmlWriter writer,RowCell cell)
        {
            if (Identity || AllowEdit == false || (Grid.DisplayView == DisplayView.Grid && AllowEditInGrid == false))
            {
                RenderLabelView(writer,cell);
                return;
            }

            string uniqueID = cell.CellClientId;

            GetColumnPostBackData(cell);

            if (IsHtml)
            {
                EditHtml(
                    !Grid.IsDesignTime
                        ? DrawHtml(uniqueID, writer, Grid, cell)
                        : HttpUtility.HtmlEncode("<HTML EDITOR DESIGN TIME>"), writer, cell);
                return;
            }
            string s;
            string theValueToShow = DisplayText(cell);
            /*if(TheValueToShow == null)
                TheValueToShow = Value;*/

            if (IsUrl && string.IsNullOrEmpty(theValueToShow))
                theValueToShow = HTTPCONST;

            StringBuilder cssstyle = new StringBuilder("style=\"");

            theValueToShow = HttpUtility.HtmlEncode(theValueToShow);

            string size = string.Empty;
            string type = "text";
            StringBuilder javascript = new StringBuilder(string.Empty);
            if (WidthEditableColumn != Unit.Empty)
                cssstyle.AppendFormat("width:{0}; ", WidthEditableColumn);
            if (HeightEditableColumn != Unit.Empty)

                cssstyle.AppendFormat("height:{0}; ", HeightEditableColumn);
            if (MaxSize > 0)
                size = string.Format("maxlength=\"{0}\" ", MaxSize);
            if (IsPassword)
                type = "password";
            StringBuilder onblur = new StringBuilder(" onblur=\"");
            if (Grid.InputHighLight != Color.Empty)
            {
                javascript.AppendFormat(
                    " onfocus=\"this.accessKey = style.backgroundColor;style.backgroundColor='{0}';\"",
                    Grid.ColorToHtml(Grid.InputHighLight));
                onblur.Append("style.backgroundColor=this.accessKey;");
            }
            if (Grid.ColumnChangedColour != Color.Empty)
                onblur.AppendFormat("isChanged(this,'{0}');", Grid.ColorToHtml(Grid.ColumnChangedColour));

            if (AutoPostback)
            {
                onblur.Append("if(hasChanged(this))");
                onblur.Append(Grid.EnableCallBack && !ForcePostBack ? Asynchronous.GetCallbackEventReference(Grid, string.Format("ElementPostBack!{0}!{1}",
                                                                                                                ColumnId, cell.Row.PrimaryKeyValues), false,
                                                                                                  string.Empty, string.Empty) : Grid.Page.ClientScript.GetPostBackEventReference(Grid,
                                                                                                                                                                                 string.Format(
                                                                                                                                                                                    "ElementPostBack!{0}!{1}", ColumnId, cell.Row.PrimaryKeyValues)));

            }
            onblur.Append("\"");

            javascript.Append(onblur);

            string mask = null;

            if (!DisableMaskedInput && GenerateColumnMask != null)
            {
                string maskId = string.Format("{0}_{1}", Grid.ID, ColumnId);
                if (!Grid.MaskedColumns.ContainsKey(maskId))
                    Grid.MaskedColumns.Add(maskId, GenerateColumnMask);

                mask = string.Format(" alt=\"{0}\"", maskId);
            }

            cssstyle.Append("\"");
            if (HeightEditableColumn == Unit.Empty)
            {
                s =
                    string.Format(
                        "<input {0} {1} {6}{7} type=\"{2}\" class=\"wgeditfield\" {3} value=\"{4}\" id=\"{5}\" name=\"{5}\"/>",
                        javascript, cssstyle, type, size, theValueToShow, uniqueID, Attributes,mask);
            }
            else
            {
                s =
                    string.Format(
                        "<textarea  rows=\"1\" cols=\"1\" {0} {1} class=\"wgeditfield\" id=\"{2}\" name=\"{3}\" {4}>",
                        javascript, cssstyle, uniqueID, uniqueID, Attributes);
                if (theValueToShow != null) s += theValueToShow;
                s += "</textarea>";
            }

            if (string.IsNullOrEmpty(ToolTipInput) == false)
                s = Tooltip.Add(s, ToolTipInput);

            EditHtml(s, writer,cell);
        }

        // string optimize
        internal override void RenderLabelView(WebGridHtmlWriter writer,RowCell cell)
        {
            string s = Value(cell);
            if (IsUrl && string.IsNullOrEmpty(s) == false && s.Length > 0)
            {
                //if( s.ToLowerInvariant() .StartsWith("http") == false )
                if (String.Compare(s, 0, "http", 0, 4, true) != 0)
                    s = HTTPCONST + s;
                s = ColumnGridAlias != null ? string.Format(
                                                  "<a class=\"wglinkfield\" href=\"{0}\" target=\"_blank\">{1}</a>",
                                                  s,  ColumnGridAlias) : string.Format("<a class=\"wglinkfield\" href=\"{0}\" target=\"_blank\">{1}</a>",
                                                                                                                      s,  s);
            }
            if (IsEmail && !string.IsNullOrEmpty(s))
                s = ColumnGridAlias != null ? string.Format(
                                                  "<a class=\"wglinkfield\"  href=\"mailto:{0}\">{1}</a>",
                                                  s, ColumnGridAlias) : string.Format("<a class=\"wglinkfield\" href=\"mailto:{0}\"  >{1}</a>",s, s);
            if (IsPassword && PasswordString != null)
                s = PasswordString;
            cell.Value = s;
            base.RenderLabelView(writer,cell);
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
        internal new string Value(RowCell cell)
        {
            LoadValueFromColumnDataSource(cell);

            return cell.Value == null ? null : cell.Value.ToString();
        }

        #endregion Methods
    }
}