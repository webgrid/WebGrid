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
    using System.Drawing.Design;
    using System.IO;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using Anthem;

    using WebGrid.Collections;
    using WebGrid.Data;
    using WebGrid.Design;
    using WebGrid.Enums;
    using WebGrid.Events;
    using WebGrid.Util;

    /// <summary>
    /// The Image class is displayed as a column with a file input box in the web interface.
    /// If the column contains a file it can be displayed as an image.
    /// The file can either be stored on disk or in the data source (if available).
    /// Default upload directory for files stored on disk is "Upload/"
    /// This class inherits from the <see cref="WebGrid.Column">WebGrid.Column</see> class.
    /// </summary>
    public class Image : Column
    {
        #region Fields

        private string m_AbsoluteDirectoryFileName;

        // 2005.01.09 - jorn, string optimize
        private string m_AllowExtensions = "*";

        // 2005.01.09 - jorn, string optimize
        private string m_DenyExtensions = "exe,com,asp,aspx,ascx,dll,bat,htm,html,php,config,ini";
        private string m_Directory = "upload/";
        private bool m_DisplayFileName;
        private string m_FileName;
        private FileNameOption m_FileNameOption = FileNameOption.Undefined;
        private FileTemporaryMode m_FileTemporaryMode = FileTemporaryMode.Undefined;
        private bool m_HideImage;
        private string m_PopupImageCaption;
        private string m_PopupImageColumn;
        private ImageDisplayType m_PopupImageDisplayType = ImageDisplayType.Single;
        private ImageSizeCollection m_Sizes;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="File">Image</see> class.
        /// </summary>
        public Image()
        {
            Sortable = false;
            m_ColumnType = ColumnType.Image;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="File">Image</see> class.
        /// </summary>
        /// <param name="columnName">The name of the data-source column.</param>
        /// <param name="table">The table object.</param>
        public Image(string columnName, Data.Table table)
            : base(columnName, table)
        {
            Grid = table.m_Grid;
            m_Table = table;
            ColumnId = columnName;
            m_ColumnType = ColumnType.Image;
            Sortable = false;
            GridVAlign = VerticalPosition.middle;
            EditVAlign = VerticalPosition.middle;
            EditAlign = HorizontalPosition.Center;
            GridAlign = HorizontalPosition.Center;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the absolute path for uploading files.
        /// </summary>
        /// <value>The absolute directory.</value>
        [Browsable(false),
        Description(@"Gets the absolute path for uploading files.")]
        public string AbsoluteDirectory
        {
            get
            {
                return Grid.GotHttpContext ? HttpContext.Current.Server.MapPath(Directory) : null;
            }
        }

        /// <summary>
        /// Gets or sets which filename extensions are allowed. If set to anything other than '*',
        /// then DenyExtensions is set to *.
        /// </summary>
        /// <value>The allowed extensions.</value>
        /// <remarks>
        /// Default value is '*'.
        /// </remarks>
        [Browsable(true),
        Category("Image options"),
        DefaultValue("*"),
        Description(
             @"Gets or sets which filename extensions are allowed. If set to anything other than '*', then DenyExtensions is set to '*'."
             )]
        public string AllowExtensions
        {
            get { return m_AllowExtensions; }
            set
            {
                m_AllowExtensions = value ?? string.Empty;
                m_AllowExtensions = m_AllowExtensions.Trim();
                if (m_AllowExtensions == string.Empty)
                    m_AllowExtensions = "*";
                if (String.Compare(m_AllowExtensions, "*", false) != 0)
                    DenyExtensions = "*";
            }
        }

        /// <summary>
        /// Gets or sets what filename extensions are denied. If set to anything other than *, then AllowExtensions is set to *.
        /// </summary>
        /// <value>The denied extensions.</value>
        /// <remarks>
        /// Default value = "exe, com, asp, aspx, ascx, dll, bat, htm, html, php, config"
        /// </remarks>
        [Browsable(true),
        Category("Image options"),
        DefaultValue("exe,com,asp,aspx,ascx,dll,bat,htm,html,php,config,ini"),
        Description(
             @"Gets or sets what filename extensions are denied. If set to anything other than *, then AllowExtensions is set to *."
             )]
        public string DenyExtensions
        {
            get { return m_DenyExtensions; }
            set
            {
                m_DenyExtensions = value ?? String.Empty;
                m_DenyExtensions = m_DenyExtensions.Trim();
                if (m_DenyExtensions == string.Empty)
                    m_DenyExtensions = "*";
                if (String.Compare(m_DenyExtensions, "*", false) != 0)
                    AllowExtensions = "*";
            }
        }

        /// <summary>
        /// Sets or gets the path of the upload directory. Default is "upload/".
        /// </summary>
        /// <value>The directory.</value>
        [Browsable(true),
        Category("Image options"),
        DefaultValue("upload/"),
        Description(@"Sets or gets the path of the upload directory. Default is ""upload/"".")]
        public string Directory
        {
            set
            {
                m_Directory = value;
                m_Directory = m_Directory.Replace("\\", "/");
                m_Directory += (m_Directory.EndsWith("/")) ? string.Empty : "/";
            }
            get { return m_Directory; }
        }

        /// <summary>
        /// Gets or sets whether the filename should be visibility. This applies to grid and detail view.
        /// </summary>
        /// <value><c>true</c> if [display Image name]; otherwise, <c>false</c>.</value>
        [Browsable(true),
        Category("Image options"),
        DefaultValue(false),
        Description(@"Gets or sets whether the filename should be visible. This applies to grid and detail view.")]
        public bool DisplayFileNameOnly
        {
            get { return m_DisplayFileName; }
            set { m_DisplayFileName = value; }
        }

        /// <summary>
        /// Sets or gets which grid column that holds the filename. 
        /// If this property is empty file upload will force "UsePk" as <see cref="FileNameOption"/>
        /// </summary>
        /// <value>The file name column.</value>
        [Browsable(true),
        Category("Image options"),
        Description(@"Sets or gets which editor column that holds the filename.")]
        public new string FileNameColumn
        {
            get { return base.FileNameColumn; }
            set { base.FileNameColumn = value; }
        }

        /// <summary>
        /// Sets or gets which method should be used to name the file being uploaded.
        /// The default method is Undefined.
        /// </summary>
        /// <value>The file name option.</value>
        [Browsable(true),
        Category("Image options"),
        Description(@"Sets or gets which method should be used to name the file being uploaded.")]
        public FileNameOption FileNameOption
        {
            get
            {
                if (m_FileNameOption == FileNameOption.Undefined)
                {
                    return IsInDataSource == false ? FileNameOption.Original : FileNameOption.UsePrimaryKey;
                }
                return m_FileNameOption;
            }
            set { m_FileNameOption = value; }
        }

        /// <summary>
        /// Sets or gets how temporary files should be stored. Default is Undefined. If set to File, then temporary files are saved to the windows %temp% folder.
        /// </summary>
        /// <value>The sort mode.</value>
        [Browsable(true),
        Category("Image options"),
        Description(
             @"Sets or gets how temporary files should be stored. Default is Undefined. If set to File, then temporary files are saved to the windows %temp% folder."
             )]
        public FileTemporaryMode FileTemporaryMode
        {
            get
            {
                return m_FileTemporaryMode == FileTemporaryMode.Undefined ? FileTemporaryMode.Memory : m_FileTemporaryMode;
            }
            set { m_FileTemporaryMode = value; }
        }

        /// <summary>
        /// A collection of customized system messages that either override system messages, or 
        /// add new system messages for this grid.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [MergableProperty(true)]
        [Description("Add or edit image sizes (applies to image upload).")]
        [Browsable(true)]
        [TypeConverter(typeof(WebGridFileImageSizeCollectionConverter))]
        [Editor(typeof(WebGridFileImageSizeCollectionEditor), typeof(UITypeEditor))]
        public ImageSizeCollection ImageSizes
        {
            get { return m_Sizes ?? (m_Sizes = new ImageSizeCollection()); }
        }

        /// <summary>
        /// Returns true if the column is a binary data source object, else false. (Applies to File/Image columns)
        /// </summary>
        /// <value><c>true</c> if this instance is blob (in database); otherwise, <c>false</c>.</value>
        [Browsable(false),
        Description(
            @"Returns true if the file is stored in the database, returns false if the file is stored on disk.")]
        public new bool IsBlob
        {
            get { return base.IsBlob; }
            internal set { base.IsBlob = value; }
        }

        /// <summary>
        /// Sets or gets caption text used for 'PopupImageColumn' property. 
        /// </summary>
        [Browsable(true),
        Category("Image options"),
        Description(@"Sets or gets  caption text used for 'PopupImageColumn' property.")]
        public string PopupImageCaption
        {
            get { return m_PopupImageCaption; }
            set
            {
                  m_PopupImageCaption = value;
            }
        }

        /// <summary>
        /// Sets or gets the column holding a reference for a popup image for this column. This image must be located in disk drive.
        /// </summary>
        [Browsable(true),
        Category("Image options"),
        TypeConverter(typeof(WebGridTreeParentSelector)),
        Description(
             @"Sets or gets the column holding a reference for a popup image for this column. This image must be located in disk drive."
             )]
        public string PopupImageColumn
        {
            get { return m_PopupImageColumn; }
            set
            {
                if (Grid != null && Grid.MasterTable.Columns[value] == null)
                    throw new GridException(
                        string.Format(
                            "The column provided for property 'DetailedImageColumn' in column '{0}' does not exist",
                            Title));
                m_PopupImageColumn = value;
            }
        }

        ///<summary>
        /// Sets or gets display for This Image column
        ///</summary>
        public ImageDisplayType PopupImageDisplayType
        {
            get
             {
                 return m_PopupImageDisplayType;
             }
             set
             {
                 m_PopupImageDisplayType = value;
             }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the name of the absolute directory file.
        /// </summary>
        /// <value>The name of the absolute directory file.</value>
        public string AbsoluteDirectoryFileName(RowCell cell)
        {
            if (m_AbsoluteDirectoryFileName != null)
                    return m_AbsoluteDirectoryFileName;
                if (FileNameOption == FileNameOption.UsePrimaryKey)
                    m_AbsoluteDirectoryFileName =
                        string.Format("{0}{1}_{2}.{3}", AbsoluteDirectory, cell.Row.PrimaryKeyUpdateValues, ColumnId,
                                      GetExtension(Value(cell)));
                else
                    m_AbsoluteDirectoryFileName = AbsoluteDirectory + FileName(cell);
                return m_AbsoluteDirectoryFileName;
        }

        /// <summary>
        /// Gets the filename for the Image.
        /// </summary>
        /// <value>The name of the Image.</value>
        [Browsable(false),
        Description(@"Gets the filename for the Image.")]
        public string FileName(RowCell cell)
        {
            if (IsBlob && string.IsNullOrEmpty(FileNameColumn))
                    return null;
                if (!string.IsNullOrEmpty(FileNameColumn))
                {
                    if (!IsBlob)
                    {
                        if (ColumnId == FileNameColumn)
                            return m_FileName = Value(cell);
                        if (FileNameOption == FileNameOption.UsePrimaryKey && cell.Row[FileNameColumn].Value != null)
                            return m_FileName =
                                   string.Format("{0}_{1}.{2}", cell.Row.PrimaryKeyUpdateValues, ColumnId,
                                                 GetExtension(cell.Row[FileNameColumn].Value.ToString()));
                    }
                    if (cell.Row[FileNameColumn].Value != null)
                        return m_FileName = cell.Row[FileNameColumn].Value.ToString();
                }

                if (m_FileName == null)
                {
                    if (IsBlob)
                        return null;
                    return FileNameOption == FileNameOption.UsePrimaryKey
                               ? (m_FileName =
                                  string.Format("{0}_{1}.{2}", cell.Row.PrimaryKeyUpdateValues, ColumnId, Value(cell)))
                               : (m_FileName = Value(cell));
                }
                return m_FileName;
        }

        /// <summary>
        /// Gets the name of the get storedfile.
        /// </summary>
        /// <value>The name of the get storedfile.</value>
        public string GetStoredfileName(RowCell cell)
        {
            string fileName;
            if (FileNameOption == FileNameOption.UsePrimaryKey && cell.Row[FileNameColumn].Value != null)
                fileName =
                    string.Format("{0}_{1}.{2}", cell.Row.PrimaryKeyUpdateValues, ColumnId,
                                  GetExtension(cell.Row[FileNameColumn].Value.ToString()));
            else if (FileNameColumn != null && cell.Row[FileNameColumn].Value != null)
                fileName = cell.Row[FileNameColumn].Value.ToString();
            else
            {
                if (IsBlob)
                    return null;
                fileName = FileNameOption == FileNameOption.UsePrimaryKey
                               ? string.Format("{0}_{1}.{2}", cell.Row.PrimaryKeyUpdateValues, ColumnId, Value(cell))
                               : Value(cell);
            }
            return fileName;
        }

        /// <summary>
        /// Gets the absolute url for this file.
        /// </summary>
        /// <value>The URL.</value>
        [Browsable(false),
        Description(@"Gets the absolute url for this file.")]
        public string GetUrl(RowCell cell)
        {
            return FileSource(true,cell);
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns></returns>
        public override bool Validate(RowCell cell)
        {
            bool res = base.Validate(cell);

            return res;
        }

        internal override void AfterCancel()
        {
        }

        internal override void AfterUpdate(string editIndex,RowCell cell)
        {
            string uniqueID = cell.CellClientId;
            // Renames file
            if (!Grid.GotHttpContext ||
                HttpContext.Current.Session[string.Format("{0}_imgdata", uniqueID)] == null)
                return;

            string filename = (FileNameOption == FileNameOption.Original)
                                 ? HttpContext.Current.Session[uniqueID + "_img"] as string
                                 :
                                     string.Format("{0}_{1}.{2}", editIndex, ColumnId, GetExtension((string)HttpContext.Current.Session[uniqueID + "_img"]));
            System.Drawing.Image img = CreateImageSizes(filename,cell);
            if (IsBlob || (IsBlob && FileNameOption != FileNameOption.UsePrimaryKey && ColumnId != FileNameColumn))
            {
                DeleteSession(cell);
                return;
            }

            try
            {
                System.IO.File.Delete(AbsoluteDirectory + filename);
            }
            catch (Exception ee)
            {
                Grid.SystemMessage.Add(string.Format("Warning: Failed to delete existing file '{0}'. Turn on debug for more information.", HttpContext.Current.Session[uniqueID + "_img"]));
                if (Grid.Debug)
                    Grid.m_DebugString.Append(ee);
            }

            try
            {
                img.Save(AbsoluteDirectory + filename, img.RawFormat);
            }
            catch (Exception ee)
            {
                Grid.SystemMessage.Add(
                    "Warning: Failed saving file to disk. Turn on debug for more information.");
                if (Grid.Debug)
                    Grid.m_DebugString.Append(ee);
            }
            //}
            img.Dispose();

            if (Grid.GotHttpContext &&
                HttpContext.Current.Session[uniqueID + "_imgdata"] is Image)
                ((Image)HttpContext.Current.Session[uniqueID + "_imgdata"]).Dispose();
            if (Grid.GotHttpContext &&
                HttpContext.Current.Session[uniqueID + "_imgdata"] is Stream)
                ((Stream)HttpContext.Current.Session[uniqueID + "_imgdata"]).Close();

            DeleteSession(cell);
        }

        internal override void CopyFrom(Column column)
        {
            base.CopyFrom(column);
            Sortable = false;
            if (column.ColumnType != ColumnType.Image)
                return;
            Image c = (Image)column;
            m_PopupImageDisplayType = c.m_PopupImageDisplayType;
            m_Directory = c.m_Directory;
            m_AllowExtensions = c.m_AllowExtensions;
            m_DenyExtensions = c.m_DenyExtensions;
            m_DisplayFileName = c.m_DisplayFileName;
            m_HideImage = c.m_HideImage;
            m_FileNameOption = c.m_FileNameOption;
            m_FileTemporaryMode = c.m_FileTemporaryMode;
            m_FileName = c.m_FileName;
            m_PopupImageColumn = c.m_PopupImageColumn;
            m_Sizes = c.m_Sizes;
            m_PopupImageCaption = c.m_PopupImageCaption;
            base.FileNameColumn = c.FileNameColumn;
            base.IsBlob = c.IsBlob;
        }

        internal override Column CopyTo(Column column)
        {
            if (column.ColumnType == ColumnType.Image)
            {
                Image c = (Image)base.CopyTo(column);
                c.m_PopupImageDisplayType = m_PopupImageDisplayType;
                c.m_Directory = m_Directory;
                c.m_HideImage = m_HideImage;
                c.m_AllowExtensions = m_AllowExtensions;
                c.m_DenyExtensions = m_DenyExtensions;
                c.m_DisplayFileName = m_DisplayFileName;
                c.m_FileNameOption = m_FileNameOption;
                c.m_FileTemporaryMode = m_FileTemporaryMode;
                c.m_FileName = m_FileName;
                c.m_PopupImageColumn = m_PopupImageColumn;
                c.m_Sizes = m_Sizes;
                c.m_FileName = m_FileName;
                c.FileNameColumn = base.FileNameColumn;
                c.IsBlob = base.IsBlob;
                c.m_PopupImageCaption = m_PopupImageCaption;
                return c;
            }
            base.CopyTo(column);
            column.Sortable = false;
            return column;
        }

        internal override Column CreateColumn()
        {
            return new Image(ColumnId, m_Table);
        }

        internal override Column Duplicate()
        {
            Image c = new Image(ColumnId, m_Table);
            CopyTo(c);
            return c;
        }

        // 2005.01.09 - jorn, string optimize
        internal string FileSource(bool onlyParameters,RowCell cell)
        {
            bool isNewRecord = String.Compare(cell.Row.PrimaryKeyValues, Grid.EMPTYSTRINGCONSTANT, false) == 0;
            StringBuilder s = new StringBuilder(string.Empty);
            if (IsBlob)
            {
                if (isNewRecord)
                    return String.Empty;
                //   if (Value == null && string.IsNullOrEmpty(FileName))
                //       return String.Empty;
                s.AppendFormat(
                    !DisplayFileNameOnly
                        ? "<img alt=\"{0}\" border=\"0\" class=\"wgimage\" src=\""
                        : "<a alt=\"{0}\" target=\"gridfile\" href=\"", Title);

                if (onlyParameters)
                    s = new StringBuilder(string.Empty);

                if (Grid.GotHttpContext && HttpContext.Current.Request["QUERY_STRING"] != null)
                {
                    s.AppendFormat("?{0}", HttpContext.Current.Request["QUERY_STRING"]);
                    s.AppendFormat("&amp;wgdbimgdl=false");
                }
                else
                    s.AppendFormat("?wgdbimgdl=false");

                s.AppendFormat(
                   "&amp;wgblob=true&amp;wgfilenameoption={0}&amp;wgfilename={1}&amp;wgdbimgeditorid={2}&amp;wgdbimgimageid={3}&amp;wgdbimgcolumnname={4}",
                   (int)FileNameOption, HttpUtility.UrlEncode(FileName(cell), Encoding.Default),
                   HttpUtility.UrlEncode(Grid.ClientID, Encoding.Default),
                   HttpUtility.UrlEncode(cell.Row.PrimaryKeyValues, Encoding.Default),
                   HttpUtility.UrlEncode(ColumnId, Encoding.Default));
                if (onlyParameters)
                    return s.ToString();

                s.Append("\"");

                if (!ValidateFile(cell))
                {
                    if (WidthEditableColumn != Unit.Empty)
                        s.AppendFormat(" width=\"{0}\"", WidthEditableColumn);
                    if (HeightEditableColumn != Unit.Empty)
                        s.AppendFormat(" height=\"{0}\"", HeightEditableColumn);
                }
                s.Append(" />");

                if (DisplayFileNameOnly)
                {
                    string systemMessage = Grid.GetSystemMessage("Attachment");
                    string fil;
                    if (FileNameColumn != null && cell.Row[FileNameColumn].Value != null)
                        fil = cell.Row[FileNameColumn].Value.ToString();
                    else
                        fil = FileName(cell);
                    if (!string.IsNullOrEmpty(systemMessage))
                        fil = String.Format("{1} ({0})", fil, systemMessage);
                    s.AppendFormat("{0}</a>", fil);
                }
            }
            else
            {
                if (isNewRecord)
                    return String.Empty;

                if (IsInDataSource)
                    if (Value(cell) == null)
                        return String.Empty;
                    else if (string.IsNullOrEmpty(FileName(cell)))
                        return String.Empty;

                s.AppendFormat(
                    !DisplayFileNameOnly
                        ? "<img alt=\"{0}\" title=\"{0}\" border=\"0\" class=\"wgimage\" src=\""
                        : "<a alt=\"{0}\" title=\"{0}\" target=\"gridfile\" href=\"", Title);

                if (onlyParameters)
                    s = new StringBuilder(string.Empty);

                s.AppendFormat("{0}{1}", Directory, GetStoredfileName(cell));

                if (onlyParameters)
                    return s.ToString();
                s.Append("\"");
                if (!ValidateFile(cell))
                {
                    if (WidthEditableColumn != Unit.Empty)
                        s.AppendFormat(" width=\"{0}\"", WidthEditableColumn);
                    if (HeightEditableColumn != Unit.Empty)
                        s.AppendFormat(" height=\"{0}\"", HeightEditableColumn);
                }
                s.Append(" />");

                if (DisplayFileNameOnly)
                {
                    string systemMessage = Grid.GetSystemMessage("Attachment");
                    string fil;
                    if (FileNameColumn != null && cell.Row[FileNameColumn].Value != null)
                        fil = cell.Row[FileNameColumn].Value.ToString();
                    else
                        fil = FileName(cell);
                    if (!string.IsNullOrEmpty(systemMessage))
                        fil = String.Format("{1} ({0})", fil, systemMessage);
                    s.AppendFormat("{0}</a>", fil);
                }

            }
            return s.ToString();
        }

        internal override void GetColumnPostBackData(RowCell cell)
        {
            if (cell.GotPostBackData || !Grid.GotHttpContext)
                return;
            // Gets the posted file and saves it to a temporary image.
            // Updates views tate saying that the file needs to be renamed
            using (HtmlInputFile f = new HtmlInputFile())
            {
                f.ID = cell.CellClientId;

                if (f.PostedFile == null || f.PostedFile.FileName.Length == 0) return;
                if (AllowExtension(f.PostedFile.FileName) == false)
                {
                    Grid.SystemMessage.Add("You are not allowed to upload files of this type.",
                                           SystemMessageStyle, cell.Row.GetColumnInitKeys(ColumnId));
                    return;
                }

                string fileName = Path.GetFileName(f.PostedFile.FileName);

                // We must set the filename column before we do any stuff
                if (FileNameColumn != null)
                    cell.Row[FileNameColumn].Value = fileName;
                if (FileTemporaryMode == FileTemporaryMode.File)
                {
                    string tempFile = Path.GetTempFileName();
                    f.PostedFile.SaveAs(tempFile);
                    if (SaveSession(fileName, tempFile,cell) == false)
                        return;
                }
                else if (SaveSession(fileName, f.PostedFile.InputStream,cell) == false)
                    return;

                cell.Value = fileName;
                cell.GotPostBackData = true;
            }
        }

        internal override void OnUpdateInsert(CellUpdateInsertArgument ea, RowCell cell)
        {
            if (!Grid.GotHttpContext || HttpContext.Current.Session == null)
                return;
            if (IsBlob == false && System.IO.Directory.Exists(AbsoluteDirectory) == false)
                throw new GridException(
                    string.Format("Upload directory for '{0}' does not exist ({1})", Title,
                                  AbsoluteDirectory));

            // Should we remove the file in DB?
            string strDelete = HttpContext.Current.Request[cell.CellClientId + "_delete"];

            if (strDelete != null && String.Compare(strDelete, "TRUE") == 0)
            {
                if (IsBlob == false)
                {
                    string filename;
                    if (FileNameOption == FileNameOption.UsePrimaryKey)
                        filename =
                            string.Format("{0}{1}_{2}.{3}", AbsoluteDirectory, cell.Row.PrimaryKeyUpdateValues,
                                          ColumnId, GetExtension(Value(cell)));
                    else
                        filename = AbsoluteDirectory + Value(cell);
                    if (System.IO.File.Exists(filename))
                    {
                        try
                        {
                            System.IO.File.Delete(filename);
                        }
                        catch (Exception ee)
                        {
                            Grid.SystemMessage.Add(
                                string.Format("Warning: failed to remove file associated with column '{0}'", Title));
                            if (Grid.Debug)
                                Grid.m_DebugString.Append(ee.Message);
                        }
                    }
                    if (FileNameColumn == ColumnId)
                    {
                        cell.Value = null;
                        cell.PostBackValue = null;
                    }
                }
                ea.IgnoreChanges = false;
               // ea.Value = Grid.NULLCONSTANT;
                ea.AddApostrophe = false;

                if (FileNameColumn != null && FileNameColumn != ColumnId)
                    cell.Row[FileNameColumn].Value = null;

                // TODO: Should delete old image if not blob.
                return;
            }

            ea.IgnoreChanges = true;
            if (!Grid.GotHttpContext || HttpContext.Current.Session[cell.CellClientId + "_img"] == null)
                // No image!
                return;
            if (IsBlob)
            {
                OnUpdateInsertDataSource(ea,cell);
                return;
            }
            ea.Parameter = null;
            ea.IgnoreChanges = false;

            ea.Value = HttpContext.Current.Session[cell.CellClientId + "_img"].ToString();
        }

        internal override void RenderEditView(WebGridHtmlWriter writer,RowCell cell)
        {
            if (AllowEdit == false || (Grid.DisplayView == DisplayView.Grid && AllowEditInGrid == false) ||
                !Grid.GotHttpContext)
            {
                RenderLabelView(writer,cell);
                return;
            }

            StringBuilder s = new StringBuilder(string.Empty);
            StringBuilder javascript = new StringBuilder(string.Empty);
            s.Append(FileSource(false,cell));

            StringBuilder onblur = new StringBuilder(" onblur=\"");
            if (Grid.InputHighLight != Color.Empty)
            {
                javascript.AppendFormat(
                    " onfocus=\"accessKey = style.backgroundColor;style.backgroundColor='{0}';\"",
                    Grid.ColorToHtml(Grid.InputHighLight));
                onblur.Append("style.backgroundColor=accessKey;");
            }
            if (Grid.ColumnChangedColour != Color.Empty)
            {
                onblur.AppendFormat("isChanged(this,'{0}');", Grid.ColorToHtml(Grid.ColumnChangedColour));
            }

            onblur.Append("\"");
            javascript.Append(onblur);

            s.AppendFormat("<br/><input {0} {2} type=\"file\" class=\"wgeditfield\"  name=\"{1}\" id=\"{1}\"/>",
                           javascript, cell.CellClientId, Attributes);

            if (AllowEmpty && Required == false && Grid.InternalId != null && ((cell.Value != null)
                                                                              ||
                                                                              (FileNameColumn != null &&
                                                                               cell.Value != null)))
                s.AppendFormat(
                    "<br/><input class=\"wglinkfield\" name=\"{0}_delete\" id=\"{0}_delete\"  value=\"TRUE\" type=\"checkbox\" /><label class=\"wgnowrap wglabel\" id=\"label_{0}\" for=\"{0}_delete\">{1}</label>",
                    cell.CellClientId,
                    Grid.GetSystemMessage("RemoveImage"));

            if (Grid.FindForm != null)
            {
                if (Grid.EnableCallBack)
                {
                    if (Grid.FindForm.Enctype.Equals("multipart/form-data", StringComparison.OrdinalIgnoreCase) ==
                        false)
                        Manager.AddScriptForClientSideEval(string.Format("document.getElementById(\"{0}\").encoding = \"multipart/form-data\";", Grid.FindForm.ClientID));
                }
                else
                    Grid.FindForm.Enctype = "multipart/form-data";
            }
            else
                throw new GridException(
                    string.Format("HtmlForm is not found for grid '{0}' and column '{1}'", Grid.ID, ColumnId));
            if (string.IsNullOrEmpty(ToolTipInput) == false)
                s = new StringBuilder(Tooltip.Add(s.ToString(), ToolTipInput));

            EditHtml(s.ToString(), writer,cell);
        }

        internal override void RenderLabelView(WebGridHtmlWriter writer,RowCell cell)
        {
            string image = FileSource(true,cell);
            if (!string.IsNullOrEmpty(PopupImageColumn))
            {

                if (!cell.Row.Columns.Contains(PopupImageColumn))
                    throw new ApplicationException(
                        string.Format(@"""PopupImageColumn"" property value for column '{0}' is not valid.", Title));
                string rel = "";
                if (m_PopupImageDisplayType == ImageDisplayType.Navigation || m_PopupImageDisplayType == ImageDisplayType.NavigationSlideShow)
                    rel = string.Format("rel=\"{0}_{1}\"", Grid.ClientID, ColumnId);
               image =
                    string.Format(@"<a href=""{1}"" title=""{3}"" {4} class=""wgimage_{5}""><img class=""wgimage"" src=""{0}"" alt=""{2}""/></a>",
                        image, ((Image)cell.Row.Columns[PopupImageColumn]).GetUrl(cell.Row[PopupImageColumn]), Title, PopupImageCaption, rel, ColumnId);
                switch (PopupImageDisplayType)
                {
                    case ImageDisplayType.Navigation:
                        Grid.AddClientScript(writer, string.Format("$(\"a[rel='{0}_{1}']\").colorbox({{slideshow:false}});", Grid.ClientID, ColumnId));
                        break;
                    case ImageDisplayType.NavigationSlideShow:
                        Grid.AddClientScript(writer, string.Format("$(\"a[rel='{0}_{1}']\").colorbox({{slideshow:true}});", Grid.ClientID, ColumnId));
                        break;
                    case ImageDisplayType.Single:
                        Grid.AddClientScript(writer, string.Format("$(document).ready(function() {{$(\".wgimage_{0}\").colorbox();}});", ColumnId));
                        break;
                }
               }
            else
                image = string.Format(@"<img class=""wgimage"" src=""{0}"" alt=""{1}""/>", image, Title);

            if (HyperLinkColumn && Grid.DisplayView == DisplayView.Grid)
            {
                string a = Grid.EnableCallBack ? Asynchronous.GetCallbackEventReference(Grid,
                                                                                        string.Format("RecordClick!{0}!{1}", ColumnId,
                                                                                                      cell.Row.PrimaryKeyValues),
                                                                                        false, string.Empty, string.Empty) : Grid.Page.ClientScript.GetPostBackEventReference(Grid,
                                                                                                                                                                              string.Format("RecordClick!{0}!{1}",
                                                                                                                                                                                            ColumnId,
                                                                                                                                                                                            cell.Row.
                                                                                                                                                                                                PrimaryKeyValues));

                string b = (String.IsNullOrEmpty(ConfirmMessage))
                               ? string.Empty
                               : String.Format("if(wgconfirm('{0}',this,'{1}')) ", ConfirmMessage.Replace("'", "\\'"), Grid.DialogTitle.Replace("'", "\\'"));

                image =
                    string.Format(
                        "<a class=\"wglinkfield\" href=\"#\" onclick=\"{0}{1}\">{2}</a>", b, a, image);
            }

            LabelHtml(image, writer,cell);
        }

        internal bool ValidateFile(RowCell cell)
        {
            return !string.IsNullOrEmpty(FileName(cell));
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
        internal new virtual string Value(RowCell cell)
        {
            if (cell.Value != null) return cell.Value.ToString();
            return null;
        }

        private static void DeleteSession(RowCell cell)
        {
            if (!Grid.GotHttpContext)
                return;

            HttpContext.Current.Session.Remove(cell.CellClientId + "_img");
            HttpContext.Current.Session.Remove(cell.CellClientId + "_imgdata");
        }

        private static string GetExtension(string fileName)
        {
            // SHOULD USSE THIS INSTEAD, must be tested though: return Path.GetExtension( _fileName );
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;
            fileName = fileName.Trim();
            fileName = Path.GetExtension(fileName);

            return fileName.Length < 2 ? string.Empty : fileName.Substring(1, fileName.Length - 1);
        }

        // 2005.01.09 - jorn, string optimize
        private bool AllowExtension(string fileName)
        {
            if (String.Compare(AllowExtensions, "*", false) == 0 && String.Compare(DenyExtensions, "*", false) == 0)
                return true;
            string[] extensionList = AllowExtensions.Split(',');
            if (String.Compare(DenyExtensions, "*", false) != 0)
                extensionList = DenyExtensions.Split(',');
            string extension = GetExtension(fileName);
            for (int i = 0; i < extensionList.Length; i++)
            {
                if (String.Compare(extensionList[i], extension, false) == 0)
                    return false;
            }
            return true;
        }

        private System.Drawing.Image CreateImageSizes(string filename, RowCell cell)
        {
            System.Drawing.Image img = null;
            if (FileTemporaryMode == FileTemporaryMode.File)
            {
                string tmpFile = HttpContext.Current.Session[string.Format("{0}_imgdata", cell.CellClientId)] as string;
                if (tmpFile != null) img = System.Drawing.Image.FromFile(tmpFile);
            }
            else // File in memory
                img =
                    (System.Drawing.Image) HttpContext.Current.Session[string.Format("{0}_imgdata", cell.CellClientId)];
            if (ImageSizes != null && img != null)
                for (int i = 0; i < ImageSizes.Count; i++)
                {
                    string debugstr =
                        ImageSizes[i].Resize(img, AbsoluteDirectory, filename,cell);
                    if (Grid.Debug)
                        Grid.m_DebugString.AppendFormat("{0}<br/>", debugstr);
                }
            return img;
        }

        private void OnUpdateInsertDataSource(CellUpdateInsertArgument ea,RowCell cell)
        {
            if (!Grid.GotHttpContext)
                return;
            if (HttpContext.Current.Session[cell.CellClientId + "_imgdata"] == null)
            {
                ea.IgnoreChanges = true;
                return;
            }
            BinaryReader br;
            switch (FileTemporaryMode)
            {
                case FileTemporaryMode.Memory:

                    System.Drawing.Image img = (System.Drawing.Image)HttpContext.Current.Session[cell.CellClientId + "_imgdata"];

                    //if (Sizes.Count > 0)
                    //    img = Sizes[0].Resize(img);

                    MemoryStream ms = new MemoryStream();
                    img.Save(ms, img.RawFormat);
                    br = new BinaryReader(ms);

                    br.BaseStream.Position = 0;
                    ea.Parameter = br;
                    ea.IgnoreChanges = false;
                    break;
                case FileTemporaryMode.File:
                    {
                        FileStream fs =
                            System.IO.File.Open((string)HttpContext.Current.Session[cell.CellClientId + "_imgdata"],
                                                FileMode.Open, FileAccess.Read, FileShare.Read);
                        br = new BinaryReader(fs);
                        fs.Close();
                        br.BaseStream.Position = 0;
                        ea.Parameter = br;
                        ea.IgnoreChanges = false;
                    }
                    break;
            }
        }

        private bool SaveSession(string fileName, Stream fileStream,RowCell cell)
        {
            if (!Grid.GotHttpContext)
                return false;
            HttpContext.Current.Session[cell.CellClientId + "_img"] = fileName;
            try
            {
                HttpContext.Current.Session[cell.CellClientId + "_imgdata"] =
                    System.Drawing.Image.FromStream(fileStream);
            }
            catch
            {
                HttpContext.Current.Session.Remove(cell.CellClientId + "_img");
                Grid.SystemMessage.Add(
                    string.Format("{0} is not a valid image file, '{1}' requires a valid image file.", fileName,
                                  Title),
                    SystemMessageStyle, cell.Row.GetColumnInitKeys(ColumnId));
                return false;
            }
            return true;
        }

        private bool SaveSession(string fileName, string tempFile,RowCell cell)
        {
            if (!Grid.GotHttpContext)
                return false;
            HttpContext.Current.Session[cell.CellClientId + "_img"] = fileName;

            try
            {
                System.Drawing.Image.FromFile(tempFile);
            }
            catch
            {
                Grid.SystemMessage.Add(
                    string.Format("{0} is not a valid image file, '{1}' requires a valid image file.", fileName,
                                  Title),
                    SystemMessageStyle, cell.Row.GetColumnInitKeys(ColumnId));
                return false;
            }
            HttpContext.Current.Session[cell.CellClientId + "_imgdata"] = tempFile;
            return true;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeFileNameOption()
        {
            return m_FileNameOption != FileNameOption.Undefined;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeFileTemporaryMode()
        {
            return m_FileTemporaryMode != FileTemporaryMode.Undefined;
        }

        #endregion Methods
    }
}