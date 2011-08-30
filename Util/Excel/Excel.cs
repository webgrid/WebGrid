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
    using System.Text;
    using System.Web;
    using System.Web.UI;

    using Collections;
    using Data;
    using Enums;

    internal static class Excel
    {
        #region Methods

        internal static string ExcelFile(Table table, bool allRows)
        {
            table.GetData(true, allRows);
            StringBuilder returnValue = new StringBuilder();

            RowCollection rows = table.Rows;
            ColumnCollection settings = table.Columns;

            returnValue.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd/\">");
            returnValue.Append("<html>\n");
            returnValue.Append("<head>\n");
            returnValue.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">\n");
            returnValue.Append("</head>\n");
            returnValue.Append("<body>\n");
            returnValue.Append("<table>\r\n<tr>\r\n");

            int[] sortedID = new int[settings.Count];

            for (int i = 0; i < sortedID.Length; i++)
                sortedID[i] = i;

            for (int i = 0; i < sortedID.Length; i++)
                for (int j = 0; j < sortedID.Length; j++)
                {
                    if (settings[sortedID[i]].DisplayIndex > settings[sortedID[j]].DisplayIndex) continue;
                    int tmp = sortedID[i];
                    sortedID[i] = sortedID[j];
                    sortedID[j] = tmp;
                }

            for (int i = 0; i < sortedID.Length; i++)
            {
                Column column = settings[sortedID[i]];

                if (column.IsVisible == false)
                    continue;
                if (column.ColumnType == ColumnType.File || column.ColumnType == ColumnType.SystemColumn)
                    continue;

                returnValue.AppendFormat("<th {0} style=\" border:thin silver solid\" >{1}</th>\r\n", Alignment(column),
                                         HttpUtility.HtmlEncode(column.Title));
            }

            returnValue.Append("</tr>\r\n");

            for (int i = 0; i < rows.Count; i++)
            {

                returnValue.Append("<tr>\r\n");
                for (int ii = 0; ii < sortedID.Length; ii++)
                {
                    Column column = rows[i].Columns[settings[sortedID[ii]].ColumnId];

                    if (column.IsVisible == false || column.ColumnType == ColumnType.SystemColumn || column.ColumnType == ColumnType.File)
                        continue;
                    if (rows[i][column.ColumnId].Value == null && column.IsInDataSource == false && column.ColumnType == ColumnType.Decimal &&
                        ((Decimal) column).Sum != null)
                        returnValue.AppendFormat("<td {0} style=\" border:thin silver solid\" >{1}</td>\r\n",
                                                 Alignment(column),
                                                 HttpUtility.HtmlEncode(
                                                     ((Decimal)column).GetSum(rows[i][column.ColumnId]).ToString("N")));
                    else if (rows[i][column.ColumnId].Value == null)
                        returnValue.AppendFormat("<td {0} style=\" border:thin silver solid\" ></td>\r\n",
                                                 Alignment(column));
                    else if (column.ColumnType == ColumnType.Decimal && ((Decimal) column).Sum != null)
                        returnValue.AppendFormat("<td {0} style=\" border:thin silver solid\" >{1}</td>\r\n",
                                                 Alignment(column),
                                                 HttpUtility.HtmlEncode(
                                                     ((Decimal)column).GetSum(rows[i][column.ColumnId]).ToString("N")));
                    else switch (column.ColumnType)
                    {
                        case ColumnType.Number:
                            try
                            {
                                returnValue.AppendFormat("<td {0} style=\" border:thin silver solid\" >{1}</td>\r\n",
                                                         Alignment(column),
                                                         HttpUtility.HtmlEncode(int.Parse(rows[i][column.ColumnId].Value.ToString()).ToString("N")));
                                break;
                            }
                            catch
                            {
                                returnValue.AppendFormat("<td {0} style=\" border:thin silver solid\" >{1}</td>\r\n",
                                                         Alignment(column), HttpUtility.HtmlEncode(rows[i][column.ColumnId].Value.ToString()));
                                break;
                            }
                        case ColumnType.DateTime:
                            try
                            {
                                returnValue.AppendFormat("<td {0} style=\" border:thin silver solid\" >{1}</td>\r\n",
                                                         Alignment(column), HttpUtility.HtmlEncode(rows[i][column.ColumnId].Value.ToString()));
                                break;
                            }
                            catch
                            {
                                returnValue.AppendFormat("<td {0} style=\" border:thin silver solid\" >{1}</td>\r\n",
                                                         Alignment(column), HttpUtility.HtmlEncode(rows[i][column.ColumnId].Value.ToString()));
                                break;
                            }
                       default:
                            returnValue.AppendFormat("<td {0} style=\" border:thin silver solid\" >{1}</td>\r\n",
                                                     Alignment(column), HttpUtility.HtmlEncode(rows[i][column.ColumnId].Value.ToString()));
                            break;
                    }
                }

                returnValue.Append("</tr>\r\n");
            }
            returnValue.Append("</table>\r\n");
            returnValue.Append("</body>\n");
            returnValue.Append("</html>\n");

            return returnValue.ToString();
        }

        internal static void StreamToBrowser(Page page, Table table, string fileName, string content)
        {
            if (content == null || page == null) return;
            page.Response.Clear();
            page.Response.ContentType = "application/vnd.ms-excel";
            page.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName));
            page.Response.Write(content);

            page.Response.End();
        }

        private static string Alignment(Column column)
        {
            StringBuilder returnValue = new StringBuilder();

            if (column.GridAlign != HorizontalPosition.Undefined)
                returnValue.AppendFormat(" align={0}", column.GridAlign);

            if (column.GridVAlign != VerticalPosition.undefined)
                returnValue.AppendFormat(" valign={0}", column.GridVAlign);

            return returnValue.ToString();
        }

        #endregion Methods
    }
}