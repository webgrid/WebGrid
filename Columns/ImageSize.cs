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
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Web.UI;

    using Data;
    using Design;

    /// <summary>
    /// WebGrid Image Size
    /// </summary>
    public class ImageSize : IComponent
    {
        #region Fields

        private ImageFormat _resizeMode = ImageFormat.Jpeg;
        private bool keepAspectRatio = true;

        /// <summary>
        /// Prefix for the image filename.
        /// </summary>
        private string m_Prefix = string.Empty;

        /// <summary>
        /// Suffix for the image filename.
        /// </summary>
        private string m_Suffix = string.Empty;

    
        private bool useFileName = true;

        #endregion Fields

        #region Events

        /// <summary>
        /// Represents the method that handles the <see cref="E:System.ComponentModel.IComponent.Disposed"/> event of a component.
        /// </summary>
        public event EventHandler Disposed;

        #endregion Events

        #region Properties

        ///<summary>
        /// Custom graphics to override WebGrid defaults
        ///</summary>
        /// <remarks>
        /// Defaults are:
        /// CompositingQuality = CompositingQuality.HighQuality;
        /// SmoothingMode = SmoothingMode.HighQuality;
        /// InterpolationMode = InterpolationMode.HighQualityBicubic;
        /// </remarks>
        [PersistenceMode(PersistenceMode.Attribute)]
        [Description("Custom graphics to override WebGrid defaults")]
        [NotifyParentProperty(true)]
        public Graphics CustomGraphics { get; set; }

        ///<summary>
        /// Sets or gets the absolute directory where the file is to be stored. Default is Directory for the Column
        ///</summary>
        public string Directory { get; set; }

        /// <summary>
        /// Sets or gets if to keep the aspect ratio. Default is false;
        /// </summary>
        public bool EnlargeSmallerImages { get; set; }

        ///<summary>
        /// Sets or gets filename, default is the orginal filename.
        /// Create dynamic content by referring to columnId surrounded by [ and ] an example of dynamic content is:
        /// "previewImage_[ImageId]"
        ///</summary>
        /// <remarks>
        /// Create dynamic content by referring to columnId surrounded by [ and ] an example of dynamic content is:
        /// "previewImage_[ImageId]"
        /// </remarks>
        public string FileName { get; set; }

        ///<summary>
        /// Image Height
        ///</summary>
        public int Height { get; set; }

        /// <summary>
        /// Sets or gets the method for resizing images. Default is Jpeg.
        /// </summary>
        public ImageFormat ImageFormat
        {
            get
                {
                    return _resizeMode;
                }
                set { _resizeMode = value; }
        }

        /// <summary>
        /// Sets or gets if to keep the aspect ratio. Default is true;
        /// </summary>
        public bool KeepAspectRatio
        {
            get { return keepAspectRatio; }
                set { keepAspectRatio = value; }
        }

        /// <summary>
        /// Prefix for the image
        /// </summary>
        public string Prefix
        {
            get { return m_Prefix; }
                set { m_Prefix = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:System.ComponentModel.ISite"/> associated with the <see cref="T:System.ComponentModel.IComponent"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.ComponentModel.ISite"/> object associated with the component; or null, if the component does not have a site.
        /// </returns>
        [Browsable(false)]
        public virtual ISite Site
        {
            get; set;
        }

        /// <summary>
        /// Suffix for the image
        /// </summary>
        public string Suffix
        {
            get { return m_Suffix; }
                set { m_Suffix = value; }
        }

        /// <summary>
        /// Sets or gets if we should use filename provided for resizing.
        /// </summary>
        public bool UseFileName
        {
            get { return useFileName; }
                set { useFileName = value; }
        }

        /// <summary>
        /// Height for the image
        /// </summary>
        public int Width { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            //There is nothing to clean.
                if (Disposed != null)
                    Disposed(this, EventArgs.Empty);
        }

        ///<summary>
        /// Image Resize
        ///</summary>
        ///<param name="image">The image you want to resize</param>
        ///<param name="path">Path to store the image</param>
        ///<param name="fileName">Filename for the image</param>
        ///<param name="cell">WebGrid RowCell</param>
        ///<returns></returns>
        ///<exception cref="GridException"></exception>
        public string Resize(System.Drawing.Image image, string path, string fileName, RowCell cell)
        {
            if (!string.IsNullOrEmpty(Directory))
                    path = Directory;
                if (!string.IsNullOrEmpty(FileName))
                    fileName = FileName;

                if (Width < 1)
                    throw new ArgumentException("Width is 0 or less");
                if (Height < 1)
                    throw new ArgumentException("Height is 0 or less");
                if (string.IsNullOrEmpty(fileName))
                    throw new ArgumentException("FileName is empty");
                if (string.IsNullOrEmpty(path))
                    throw new ArgumentException("Directory is empty");
                if (image == null)
                    throw new ArgumentException("Image is null");
                if (cell != null && cell.Row != null && fileName.Contains("["))
                {
                    string tmp_file = fileName;
                    cell.Row.Columns.ForEach(delegate(Column column)
                                                 {
                                                     if (
                                                         tmp_file.IndexOf(string.Format("[{0}]", column.ColumnId),
                                                                          StringComparison.OrdinalIgnoreCase) == -1)
                                                         return;
                                                     if (cell.Row[column.ColumnId].Value != null)
                                                         tmp_file =
                                                             tmp_file.Replace(string.Format("[{0}]", column.ColumnId),
                                                                              cell.Row[column.ColumnId].Value.ToString());
                                                     else
                                                         tmp_file =
                                                             tmp_file.Replace(string.Format("[{0}]", column.ColumnId),
                                                                              "");

                                                 });
                    fileName = tmp_file;
                }
                if (!string.IsNullOrEmpty(Path.GetExtension(fileName)))
                    fileName = fileName.Replace(Path.GetExtension(fileName), "");
                

                string ret;
                System.Drawing.Image bitMapImage = null;



                try
                {
                    if (CustomGraphics != null)
                    {
                         bitMapImage = new Bitmap(image.Width, image.Height, CustomGraphics);
                        CustomGraphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height));
                    }
                    else
                        bitMapImage = ResizeImage(image, Width, Height, KeepAspectRatio, EnlargeSmallerImages);

                    string file = UseFileName
                                      ? string.Format("{0}{1}{2}{3}.{4}", path, Prefix, fileName, Suffix, ImageFormat)
                                      :
                                          string.Format("{0}{1}{2}.{3}", path, Prefix, Suffix, ImageFormat);
                    if (System.IO.File.Exists(file))
                        System.IO.File.Delete(file);
                    bitMapImage.Save(file, ImageFormat);
                    ret = path + Prefix + fileName + Suffix;
                }
                catch (Exception ee)
                {
                    throw new GridException(
                        string.Format("Error resizing image file: {0}",
                                      string.Format("{0}{1}{2}{3}.{4}", path, Prefix, fileName, Suffix, ImageFormat)),
                        ee);
                }
                finally
                {
                    if (bitMapImage != null)
                        bitMapImage.Dispose();
                  }
                return ret;
        }

        private static System.Drawing.Image DefaultBitMapResize(System.Drawing.Image image, int width, int height)
        {
          

            System.Drawing.Image bitMapImage = new Bitmap(width, height);
            Graphics oGraphic = Graphics.FromImage(bitMapImage);
            oGraphic.CompositingQuality = CompositingQuality.HighQuality;
            oGraphic.SmoothingMode = SmoothingMode.HighQuality;
            oGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            oGraphic.DrawImage(image, new Rectangle(0, 0, width, height));
            oGraphic.Dispose();
            return bitMapImage;
        }

        internal static System.Drawing.Image ResizeImage(System.Drawing.Image image, int width, int height, bool keepAspectRatio, bool enlargeSmallerImages)
        {
            if (!enlargeSmallerImages && image.Width <= width && image.Height <= height)
                return new Bitmap(image);

            if (!keepAspectRatio)
                return new Bitmap(image, width, height);
            double aspectRatio = image.Width/(double) image.Height;
            double newAspectRatio = width/(double) height;

            if (aspectRatio >= newAspectRatio) //fit horizontally
            {
                double scale = image.Width/(double) width;
                int newHeight = (int) (image.Height/scale);
                return DefaultBitMapResize(image, width, newHeight);
            
            }
            else //fit vertically
            {
                double scale = image.Height/(double) height;
                int newWidth = (int) (image.Width/scale);
                return DefaultBitMapResize(image, newWidth, height);
           
            }
        }

        /// <summary>
        /// Indicates if the property should serialize.
        /// </summary>
        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeSite()
        {
            return false;
        }

        #endregion Methods
    }
}