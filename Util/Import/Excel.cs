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
    using System.Data;
    using System.Data.OleDb;
    using System.Text;

    /// <summary>
    /// This class contains methods and properties for <see cref="WebGrid.Grid">WebGrid.Grid</see> web control to generate
    /// a excel sheet from the data source.
    /// </summary>
    /// <exclude/>
    public sealed class ImportExcel
    {
        #region Methods

        /// <summary>
        /// This method retrieves the data from an column in an excel workbook sheet.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="sheetName">The SheetName inside the file</param>
        /// <param name="columnName">The ColumnName inside the excel sheet</param>
        /// <returns>The data in the column</returns>
        public static String[] GetExcelSheetColumnData(string excelFile, string sheetName, string columnName)
        {
            return GetExcelSheetColumnData(excelFile, sheetName, columnName, null, null, false);
        }

        /// <summary>
        ///
        /// This method retrieves the data from an column in an excel workbook sheet.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="sheetName">The SheetName inside the file</param>
        /// <param name="columnName">The ColumnName inside the excel sheet</param>
        /// <param name="where">Select a where sentence if you wish to filter the output.</param>
        /// <param name="top">Set how many records should be return. Can be disabled with null</param>
        /// <param name="isUnique">Set true if the output should only contain unique column names.</param>
        /// <returns>String[]</returns>
        public static String[] GetExcelSheetColumnData(string excelFile, string sheetName, string columnName,
            string where, string top, bool isUnique)
        {
            OleDbConnection objConn = null;

            try
            {
                // Connection String. Change the excel file to the file you
                // will search.
                String connString = string.Format("Provider=Microsoft.Jet.OLEDatabase.4.0;Data Source={0};Extended Properties=Excel 8.0;", excelFile);
                // Create connection object by using the preceding connection string.
                objConn = new OleDbConnection(connString);
                // Open connection with the database.
                objConn.Open();
                // Get the data table containing the schema guid.

                StringBuilder sql = new StringBuilder("SELECT");

                if (isUnique)
                    sql.Append(" DISTINCT");
                if (top != null)
                    sql.AppendFormat(" TOP {0}", top);
                sql.AppendFormat(" [{0}] FROM [{1}]", columnName, sheetName);

                if (where != null)
                {
                    if (where.ToUpperInvariant().StartsWith("WHERE") )
                        where = where.Trim().Replace(where.Substring(0, 5),string.Empty);
                    sql.AppendFormat(" {0}", where);
                }

                OleDbCommand objCmdSelect = new OleDbCommand(sql.ToString(), objConn);

                OleDbDataAdapter objAdapter1 = new OleDbDataAdapter {SelectCommand = objCmdSelect};

                DataSet objDataset1 = new DataSet();

                objAdapter1.Fill(objDataset1);

                objConn.Close();

                String[] excelSheetsColumnsData = new String[objDataset1.Tables[0].Rows.Count];
                int i = 0;

                // Add the sheet name to the string array.
                foreach (DataRow row in objDataset1.Tables[0].Rows)
                {
                    excelSheetsColumnsData[i] = row[columnName].ToString();
                    i++;
                }
                return excelSheetsColumnsData;
            }
            catch
            {
                return null;
            }
            finally
            {
                // Clean up.
                if (objConn != null)
                {
                    objConn.Close();
                    objConn.Dispose();
                }
            }
        }

        /// <summary>
        /// This method retrieves the excel column names from an from an excel workbook sheet.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="sheetName">The SheetName inside the file</param>
        /// <returns>The column names</returns>
        public static String[] GetExcelSheetColumns(string excelFile, string sheetName)
        {
            OleDbConnection objConn = null;
            DataTable dt = null;

            try
            {
                // Connection String. Change the excel file to the file you
                // will search.
                String connString = string.Format("Provider=Microsoft.Jet.OLEDatabase.4.0;Data Source={0};Extended Properties=Excel 8.0;", excelFile);
                // Create connection object by using the preceding connection string.
                objConn = new OleDbConnection(connString);
                // Open connection with the database.
                objConn.Open();
                // Get the data table contain the schema guid.
                dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] {null, null, sheetName, null});

                if (dt == null)
                {
                    return null;
                }

                String[] excelSheetsColumns = new String[dt.Rows.Count];
                int i = 0;

                // Add the sheet name to the string array.
                foreach (DataRow row in dt.Rows)
                {
                    excelSheetsColumns[i] = row["COLUMN_NAME"].ToString();
                    i++;
                }
                return excelSheetsColumns;
            }
            catch
            {
                return null;
            }
            finally
            {
                // Clean up.
                if (objConn != null)
                {
                    objConn.Close();
                    objConn.Dispose();
                }
                if (dt != null)
                {
                    dt.Dispose();
                }
            }
        }

        /// <summary>
        /// This method retrieves a dataset with column names and data from an excel workbook sheet.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="sheetName">The SheetName inside the file</param>
        /// <returns>DataSet</returns>
        public static DataSet GetExcelSheetColumnsData(string excelFile, string sheetName)
        {
            return GetExcelSheetColumnsData(excelFile, sheetName, null, null, false);
        }

        /// <summary>
        /// This method retrieves a dataset with column names and data from an excel workbook sheet.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="sheetName">The SheetName inside the file</param>
        /// <param name="where">Select a where sentence if you wish to filter the output.</param>
        /// <param name="top">Set how many records should be return. Can be disabled with null</param>
        /// <param name="isUnique">Set true if the output should only contain unique column names.</param>
        /// <returns>DataSet</returns>
        public static DataSet GetExcelSheetColumnsData(string excelFile, string sheetName, string where, string top,
            bool isUnique)
        {
            OleDbConnection objConn = null;

            try
            {
                // Connection String. Change the excel file to the file you
                // will search.
                String connString = string.Format("Provider=Microsoft.Jet.OLEDatabase.4.0;Data Source={0};Extended Properties=Excel 8.0;", excelFile);
                // Create connection object by using the preceding connection string.
                objConn = new OleDbConnection(connString);
                // Open connection with the database.
                objConn.Open();
                // Get the data table containing the schema guid.

                StringBuilder sql = new StringBuilder("SELECT");

                if (isUnique)
                    sql.Append(" DISTINCT");
                if (top != null)
                    sql.AppendFormat(" TOP {0}", top);
                sql.AppendFormat(" * FROM [{0}]", sheetName);

                if (where != null)
                {
                    if (where.StartsWith("WHERE", StringComparison.OrdinalIgnoreCase))
                        where = where.Trim().Replace(where.Substring(0, 5),string.Empty);
                    sql.AppendFormat(" {0}", where);
                }

                OleDbCommand objCmdSelect = new OleDbCommand(sql.ToString(), objConn);

                OleDbDataAdapter objAdapter1 = new OleDbDataAdapter {SelectCommand = objCmdSelect};

                DataSet objDataset1 = new DataSet();

                objAdapter1.Fill(objDataset1);

                return objDataset1;
            }
            catch
            {
                return null;
            }
            finally
            {
                // Clean up.
                if (objConn != null)
                {
                    objConn.Close();
                    objConn.Dispose();
                }
            }
        }

        /// <summary>
        /// This method retrieves the excel sheet names from an excel workbook.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <returns>The Excel sheet names</returns>
        public static String[] GetExcelSheetNames(string excelFile)
        {
            OleDbConnection objConn = null;
            DataTable dt = null;

            try
            {
                // Connection String. Change the excel file to the file you
                // will search.
                String connString = string.Format("Provider=Microsoft.Jet.OLEDatabase.4.0;Data Source={0};Extended Properties=Excel 8.0;", excelFile);
                // Create connection object by using the preceding connection string.
                objConn = new OleDbConnection(connString);
                // Open connection with the database.
                objConn.Open();
                // Get the data table containing the schema guid.
                dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                if (dt == null)
                {
                    return null;
                }

                String[] excelSheets = new String[dt.Rows.Count];
                int i = 0;

                // Add the sheet name to the string array.
                foreach (DataRow row in dt.Rows)
                {
                    excelSheets[i] = row["TABLE_NAME"].ToString();
                    i++;
                }
                return excelSheets;
            }
            catch
            {
                return null;
            }
            finally
            {
                // Clean up.
                if (objConn != null)
                {
                    objConn.Close();
                    objConn.Dispose();
                }
                if (dt != null)
                {
                    dt.Dispose();
                }
            }
        }

        #endregion Methods
    }
}