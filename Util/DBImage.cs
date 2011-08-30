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

namespace WebGrid.Util
{
    using System;
    using System.IO;
    using System.Text;
    using System.Web;

    using WebGrid.Design;
    using WebGrid.Enums;

    internal static class DBImage
    {
        #region Methods

        internal static string GetExtension(string fileName)
        {
            // SHOULD USSE THIS INSTEAD, must be tested though: return Path.GetExtension( fileName );

            fileName = fileName.Trim();
            if (fileName == string.Empty) return string.Empty;
            fileName = Path.GetExtension(fileName);
            return fileName.Length < 2 ? string.Empty : fileName.Substring(1, fileName.Length - 1);
        }

        internal static byte[] LoadNoImage(string imagePath)
        {
            byte[] img = null;
            if (System.IO.File.Exists(imagePath + "//noimage.gif"))
                img = GetFile(imagePath + "//noimage.gif");
            return img;
        }

        internal static void Render(Grid grid)
        {
            if (!Grid.GotHttpContext)
                return;

            string columnName = HttpContext.Current.Request["wgdbimgcolumnname"];
            string imageID = HttpContext.Current.Request["wgdbimgimageid"];
            bool doDownload = (HttpContext.Current.Request["wgdbimgdl"] != null &&
                               HttpContext.Current.Request["wgdbimgdl"].Equals("true",
                                                                               StringComparison.OrdinalIgnoreCase));
            string fileName = HttpContext.Current.Request["wgfilename"];
            bool isBlob = (HttpContext.Current.Request["wgblob"] != null &&
                           HttpContext.Current.Request["wgblob"].Equals("true",
                                                                        StringComparison.OrdinalIgnoreCase));

            FileNameOption fileNameOption = FileNameOption.Undefined;
            if (Grid.GotHttpContext && HttpContext.Current.Request["wgfilenameoption"] != null)
                fileNameOption = (FileNameOption) int.Parse(HttpContext.Current.Request["wgfilenameoption"]);

            byte[] img = null;
            string dbFileName = null;
            if (grid.MasterTable.Columns[columnName].IsInDataSource && isBlob)
            {
                //                DataBaseConnectionType di = ConnectionType.FindConnectionType(grid.ConnectionString) // COMMENTED BY CODEIT.RIGHT;
                //Util.Query.GetDataInterface(grid.ConnectionString);

                const string top = "TOP 1";

                // const string limit = "LIMIT 1";

                StringBuilder sql = new StringBuilder(string.Empty);
                string datasourceId = grid[columnName].DataSourceId;
                if (string.IsNullOrEmpty(datasourceId))
                    datasourceId = grid.DataSourceId;
                sql.AppendFormat("SELECT {0} [{1}] FROM [{2}] WHERE ",top,columnName,datasourceId);

                int columns = 0;
                try
                {
                    foreach (Column column in grid.MasterTable.Columns.Primarykeys)
                    {
                        if (columns > 0)
                            sql.Append(" AND ");
                        sql.AppendFormat("[{0}] = {1}", column.ColumnId, imageID.Split(';')[columns]);
                        columns++;
                    }

                }
                catch(Exception ex)
                {
                    throw new ApplicationException(ex.ToString());
                }
                Query q = Query.ExecuteReader(sql.ToString(), grid.ActiveConnectionString, grid.DatabaseConnectionType);
                try
                {
                    if (q.Read())
                    {
                        img = (byte[]) q[0];
                        if (q[0].ToString().IndexOf("Byte[]") < 0)
                        {
                            img = null;
                            dbFileName = q[0].ToString();
                        }
                    }
                    q.Close();
                }
                catch (Exception ee)
                {
                    throw new GridException("Error retrieving file from data source.", ee);
                }
            }
            else
            {
                if (fileName != null && fileNameOption == FileNameOption.UsePrimaryKey)
                    dbFileName = string.Format("{0}.{1}", imageID, GetExtension(fileName));
                else
                    dbFileName = fileName;
            }

            if (dbFileName == null && img == null)
            {
                if (Grid.GotHttpContext) img = LoadNoImage(HttpContext.Current.Server.MapPath(grid.ImagePath));
            }
            else
            {
                if (isBlob == false)
                {
                    string file;
                    if (fileNameOption == FileNameOption.UsePrimaryKey)
                    {
                        string folder = ((Image) grid[columnName]).AbsoluteDirectory;
                        file = string.Format("{0}{1}.{2}", folder, imageID, GetExtension(fileName));
                    }
                    else
                        file = dbFileName;

                    if (System.IO.File.Exists(file))
                        img = GetFile(file);
                    else
                        grid.SystemMessage.Add("File not found.");
                }
            }

            if (img != null)
            {
                if (Grid.GotHttpContext)
                {
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.ClearContent();
                    HttpContext.Current.Response.ClearHeaders();

                    if (doDownload == false)
                    {
                        if (fileName != null)
                        {
                            if (fileName.IndexOf(".jp", StringComparison.OrdinalIgnoreCase) > -1)
                                HttpContext.Current.Response.AddHeader("content-type", "image/jpeg");
                            else if (fileName.IndexOf(".png", StringComparison.OrdinalIgnoreCase) > -1)
                                HttpContext.Current.Response.AddHeader("content-type", "image/png");
                            else if (fileName.IndexOf(".gif", StringComparison.OrdinalIgnoreCase) > -1)
                                HttpContext.Current.Response.AddHeader("content-type", "image/gif");
                            else if (fileName.IndexOf(".bmp", StringComparison.OrdinalIgnoreCase) > -1)
                                HttpContext.Current.Response.AddHeader("content-type", "image/bmp");
                        }
                        HttpContext.Current.Response.BinaryWrite(img);
                        HttpContext.Current.Response.End();
                    }
                    else
                    {
                        if (fileName != null)
                            HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment; filename=\"{0}\"", fileName));
                        if (fileName != null)
                            if (fileName.IndexOf(".pdf", StringComparison.OrdinalIgnoreCase) > -1)
                            {
                                HttpContext.Current.Response.AddHeader("content-type", "application/pdf");
                                HttpContext.Current.Response.ContentType = "application/pdf";
                            }
                            else
                                HttpContext.Current.Response.ContentType = "application/octet-stream";

                        if (img.Length > 0)
                            HttpContext.Current.Response.BinaryWrite(img);
                        else
                             grid.SystemMessage.Add("File not found in database (file object was null).");
                        HttpContext.Current.Response.End();
                    }
                }
            }
            else
            {
                if (Grid.GotHttpContext)
                {
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.ClearContent();
                    HttpContext.Current.Response.ClearHeaders();
                    HttpContext.Current.Response.End();
                }
                //				page.Response.Clear();
                // FAEN, nothing to do, nothing to see... If we get this, we're
                // screw'ed anyhow... :-P

                // dvs. Ingen mte  skrive ut feilbeskjed, og ikke noe mer vi kan f gjordt.
                // Lage et bilde med feilbeskjed i kode kanskje?
            }
            // clean up...
            //	page.Response.End();
        }

        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        private static byte[] GetFile(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            BinaryReader br = new BinaryReader(fs);
            byte[] fileRD = br.ReadBytes((int) fs.Length);
            br.Close();
            fs.Close();
            return fileRD;
        }

        #endregion Methods
    }
}