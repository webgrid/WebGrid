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

using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Web;
using System;
using System.Web.UI;
using WebGrid.Design;
using WebGrid.Enums;
using WebGrid.Util;
using System.Collections.Specialized;
using WebGrid.Events;
using System.Collections.Generic;
using System.Xml;
using System.Web.UI.WebControls;

namespace WebGrid.Data.Database
{
   
    /// <summary>
    /// Abstract class with the functions WebGrid needs to communicate with a database.
    /// </summary>
    public abstract class Interface
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether this <see cref="Interface"/> is cachable.
        /// </summary>
        /// <value><c>true</c> if cachable; otherwise, <c>false</c>.</value>
        public abstract bool Cachable
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether this interface requires a requires connection string
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [requires connection string]; otherwise, <c>false</c>.
        /// </value>
        public abstract bool RequiresConnectionString
        {
            get;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Loads an empty (new) row from the database into the specified Table.
        /// <param name="table">The Table object to populate with empty data.</param>
        /// </summary>
        public static void GetEmptyData(Table table)
        {
            Row r = new Row(table);

            for (int i = 0; i < table.Columns.Count; i++)
            {
                RowCell c = new RowCell(table.Columns[i].ColumnId, r) {Row = r};
                if (table.Columns[i].ColumnType == ColumnType.ColumnTemplate)
                    c.Value = ((ColumnTemplate)table.Columns[i]).CreateCellControls;
                else if (c.Value == null && table.Columns[i].DefaultValue != null)
                    c.Value = table.Columns[i].DefaultValue;
                r.Cells.Add(c);
            }

            if (table.m_Grid.MasterWebGrid == null)
            {
                table.Rows.Add(r);
                return;
            }
            List<Column> masterWebGridPrimarykeys = table.m_Grid.MasterWebGrid.MasterTable.Columns.Primarykeys;

            int index = 0;

            while (index < masterWebGridPrimarykeys.Count)
            {
                string columnname = masterWebGridPrimarykeys[index].ColumnId;
                if (r.Columns.GetIndex(columnname) > -1)
                    r[columnname].Value = table.m_Grid.MasterWebGrid.MasterTable.Rows[0][columnname].Value;
                index++;
            }
            table.Rows.Add(r);
        }

        /// <summary>
        /// Gets the database interface.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        public static object GetInterface(string connectionString)
        {
            DataBaseConnectionType conntype = ConnectionType.FindConnectionType(connectionString);
            switch (conntype)
            {
                case DataBaseConnectionType.OleDB:
                    return new OleDB();
                case DataBaseConnectionType.SqlConnection:
                    return new SqlConnection();
                /* case DataBaseConnectionTypes.MySql:
                     return new MySQL();*/
            }
            return null;
        }

        /// <summary>
        /// Gets the number of records in the table.
        /// </summary>
        /// <param name="table">The table in which to count the number of records.</param>
        /// <returns>The number of records in the table.</returns>
        //public abstract int RecordCount(Table table);
        public static int RecordCount(Table table)
        {
            string datatable = table.DataSourceId;
            string editIndex = null;
            string filter;

            if (table.m_Grid.DisplayView == DisplayView.Detail)
            {
                if (table == table.m_Grid.MasterTable)
                    return table.m_Grid.InternalId == null ? 0 : 1;
                return table.Rows.Count;
            }

            if (table.m_Grid.SystemMessage.CriticalCount != 0 || (String.IsNullOrEmpty(datatable) && String.IsNullOrEmpty(table.m_Grid.Sql)))
                return table.Rows.Count;

            if (table.Sql != null || (table == table.m_Grid.MasterTable && String.IsNullOrEmpty(table.m_Grid.Sql) == false))
            {
                if (table == table.m_Grid.MasterTable && String.IsNullOrEmpty(table.m_Grid.Sql) == false)
                    table.Sql = table.m_Grid.Sql;
                if (String.IsNullOrEmpty(table.ConnectionString))
                    throw new GridException(String.Format("'{0}' ConnectionString is invalid.", table.m_Grid.Title));

                if (table.m_Grid.InternalId != null)
                {
                    editIndex = table.m_Grid.InternalId;
                    table.m_Grid.InternalId = null;

                }

                filter = BuildFilter(table, true);

                if (editIndex != null)
                    table.m_Grid.InternalId = editIndex;

                int ant = 0;
                Query qant =
                    Query.ExecuteReader(table.Sql + filter, table.ConnectionString, table.m_Grid.DatabaseConnectionType);
                while (qant.Read())
                    ant++;
                qant.Close();
                return ant;
            }
            if (table.DataSourceType != DataSourceControlType.InternalDataSource)
                return table.Rows.Count;

            if (table.m_Grid.InternalId != null)
            {
                editIndex = table.m_Grid.InternalId;
                table.m_Grid.InternalId = null;
            }

            filter = BuildFilter(table, true);

            if (editIndex != null)
                table.m_Grid.InternalId = editIndex;

            string sql = String.Format("SELECT COUNT(1) FROM [{0}] {1}", datatable, filter);
             int r;
            try
            {
                r = Int32.Parse(Query.ExecuteScalar(sql, table.ConnectionString).ToString());
                if (table.m_Grid.Debug)
                    table.m_Grid.m_DebugString.AppendFormat("<b>{0}: SqlConnection/OleDB.RecordCount</b> - {1}<br/>",
                                                            table.m_Grid.ID, sql);
            }
            catch (Exception ex)
            {
                throw new GridException(sql, table.m_Grid.GetSystemMessage("SystemMessage_DataSource_retrieve"), ex);
            }

            return r;
        }

        /// <summary>
        /// Loads one or more rows from the database into the specified Table.
        /// </summary>
        /// <param name="table">The Table object to populate with rows.</param>
        public abstract void GetData(Table table);

        /// <summary>
        /// Gets the schema for the table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="retrieveForeignkeys">Whether foreignkeys also should be loaded.</param>
        public abstract void GetSchema(Table table, bool retrieveForeignkeys);

        /// <summary>
        /// Loads the columns.
        /// </summary>
        /// <param name="table">The table.</param>
        public abstract void LoadColumns(Table table);

        /// <summary>
        /// Indicates if the database-design structure for this table is valid. Only supported by MSSQL...
        /// </summary>
        public abstract bool ValidCacheStructure(Table table);

        internal static string BuildFilter(Table table, bool parentheses)
        {
            if (table.m_Grid.m_EventRanpreRender && String.IsNullOrEmpty(table.m_Loadfilter) == false)
                return table.m_Loadfilter;

            NameValueCollection filterElements = new NameValueCollection();
            if (table == table.m_Grid.MasterTable) // Gets some stuff from the grid
            {
                if (table.m_Grid.InternalId != null) // Just get 1 row
                {
                    filterElements.Add("GETCURRENT", BuildPKFilter(table, table.m_Grid.InternalId, parentheses));

                    // No need to add more filter stuff
                    goto finished;
                }
                if (String.IsNullOrEmpty(table.Search) == false)
                    filterElements.Add("SEARCHSTRING", BuildSearchString(table.Search, table, parentheses));
                if (String.IsNullOrEmpty(table.m_Grid.FilterExpression) == false)
                    filterElements.Add("EDITORWHERE", table.m_Grid.FilterExpression);
                if (!string.IsNullOrEmpty(table.GroupByExpression))
                {
                    string filter = null;
                    foreach (KeyValuePair<string, GroupInfo> info in table.m_Grid.GroupState)
                    {
                        if (info.Value == null || info.Value.List == null)
                            continue;

                        foreach (KeyValuePair<string, GroupDetails> subinfo in info.Value.List)
                        {
                            if (subinfo.Value == null || string.IsNullOrEmpty(subinfo.Value.GroupSqlFilter))
                                continue;
                            if (string.IsNullOrEmpty(filter))
                                filter = subinfo.Value.GroupSqlFilter;
                            else
                                filter += string.Format(" AND {0}", subinfo.Value.GroupSqlFilter);
                        }
                    }
                    filterElements.Add("GROUPFILTER", filter);
                }
                // Required filter
                if (table.m_Grid.MasterWebGrid != null)
                {
                    bool validMasterWebGrid = false;

                    List<Column> masterWebGridPrimarykeys =
                        table.m_Grid.MasterWebGrid.MasterTable.Columns.Primarykeys;

                    foreach (Column column in masterWebGridPrimarykeys)
                    {
                        if (table.m_Columns.GetIndex(column.ColumnId) != -1) continue;
                        Column columnClone = (Column) column.Clone();
                        columnClone.m_Table = table;
                        columnClone.Grid = table.m_Grid;
                        columnClone.Visibility = Visibility.None;
                        table.Columns.Add(columnClone);
                    }

                    StringBuilder masterWebGridfilter = new StringBuilder(String.Empty);
                    int index = 0;

                    while (index < masterWebGridPrimarykeys.Count)
                    {
                        string columnname = masterWebGridPrimarykeys[index].ColumnId;
                        if (table.Columns.GetIndex(columnname) > -1)
                        {
                            validMasterWebGrid = true;
                            Column column = table.Columns[columnname];
                            if (column.IsInDataSource == false)
                            {
                                index++;
                                continue;
                            }

                            object value = table.m_Grid.MasterWebGrid.MasterTable.Rows[0][columnname].Value ??
                                table.m_Grid.MasterWebGrid.MasterTable.Rows[0][columnname].DataSourceValue;
                            if ((value != null))
                            {

                                if (masterWebGridfilter.Length > 0)
                                    masterWebGridfilter.Append(" AND ");

                                masterWebGridfilter.Append(BuildFilterElement(parentheses, table.DataSourceId,
                                                                              column, value));

                            }
                        }
                        index++;
                    }

                    if (validMasterWebGrid == false)
                        throw new GridException(
                            String.Format("{0} ({1})", table.m_Grid.GetSystemMessage("SystemMessage_NoMasterGrid"),
                                          table.m_Grid.ID));
                    if (masterWebGridfilter.Length > 1)
                        filterElements.Add("MasterGrid", masterWebGridfilter.ToString());
                }
            }

            if (table.FilterExpression.Length != 0) // table filter
                filterElements.Add("TABLEWHERE", table.FilterExpression);

            // Look for Sessions that can be identified with "WG" + column name
            foreach (Column column in table.Columns)
            {
                if (column.IsInDataSource == false)
                    continue;

                if (table.m_Grid.Page == null || !Grid.GotHttpContext ||
                    HttpContext.Current.Session[String.Format("WG{0}", column.ColumnId)] == null ||
                    HttpContext.Current.Session[String.Format("WG{0}", column.ColumnId)].ToString() == String.Empty)
                    continue;

                filterElements.Add(String.Format("column_{0}", column.ColumnId),

                                  BuildFilterElement(parentheses, column.DataSourceId, column,
                                                    HttpContext.Current.Session[
                                                        String.Format("WG{0}", column.ColumnId)]));
            }

            finished:

            // BUILD filter SENTENCE

            StringBuilder buildFilter = new StringBuilder(String.Empty);
            if (filterElements.Count != 0)
            {
                buildFilter = new StringBuilder(" WHERE ");
                for (int i = 0; i < filterElements.Count; i++)
                {
                    if (String.IsNullOrEmpty(filterElements[i]))
                        continue;

                    if (i != 0 && filterElements[i].EndsWith(" OR", StringComparison.OrdinalIgnoreCase) == false)
                        buildFilter.Append(" AND ");
                    buildFilter.Append(filterElements[i]);
                }
            }

            if (table.GetUniqueDataColumn != null)
            {
                StringBuilder filter = new StringBuilder(null);

                if (buildFilter.Length > 0)
                {
                    filter = new StringBuilder(" WHERE ");
                    filter.Append(buildFilter);
                }
                foreach (Column column in table.m_Grid.MasterTable.Columns.Primarykeys)
                {
                    if (column.ColumnId == table.GetUniqueDataColumn)
                        continue;
                    if (table.m_Grid[column.ColumnId] == null || table.m_Grid.MasterTable.Rows[0][
                                                                     column.ColumnId].Value == null)
                        continue;
                    object sqlvalue = table.m_Grid.MasterTable.Columns[column.ColumnId].ColumnType ==
                                      ColumnType.Foreignkey
                                          ?
                                              table.m_Grid.MasterTable.Rows[0][column.ColumnId].GetApostropheDisplayValue
                                          : table.m_Grid.MasterTable.Rows[0][column.ColumnId].GetApostropheValue;

                    filter.AppendFormat(filter.Length > 0 ? " AND {0} = {1}" : " WHERE {0} = {1}", column.ColumnId,
                                        sqlvalue);
                }

                // Record is being updated.
                if (table.m_Grid.InternalId != null)
                {
                    Row row = table.m_Grid.MasterTable.Rows[0];
                    string updateException =
                        String.Format("{0} <> {1}", table.GetUniqueDataColumn, Foreignkey.GetApostropheValue(row[table.GetUniqueDataColumn]));
                    filter.AppendFormat(filter.Length > 0 ? " AND {0}" : " WHERE {0}", updateException);
                }
                string newBuildFilter;
                if (parentheses)

                    newBuildFilter = String.Format(" ({0} NOT IN (SELECT {1} FROM [{2}]{3}))",
                                                  BuildTableElement(true, table.DataSourceId,
                                                                         table.GetUniqueDataColumn),

                                                  BuildTableElement(true, table.m_Grid.DataSourceId,
                                                                         table.GetUniqueDataColumn),
                                                  table.m_Grid.DataSourceId, filter);
                else
                    newBuildFilter = String.Format(" ({0} NOT IN (SELECT {1} FROM  [{2}]{3}))",
                                                  BuildTableElement(false, table.DataSourceId,
                                                                         table.GetUniqueDataColumn),

                                                  BuildTableElement(false, table.m_Grid.DataSourceId,
                                                                         table.GetUniqueDataColumn),
                                                  table.m_Grid.DataSourceId, filter);

                buildFilter.AppendFormat(filter.Length > 0 ? "AND {0}" : " WHERE {0}", newBuildFilter);
            }

            string returnfilter = buildFilter.ToString();
            if (returnfilter.Equals(" where ", StringComparison.OrdinalIgnoreCase))
            {
                table.m_Loadfilter = String.Empty;
                return String.Empty;
            }
            if (table.m_Grid.m_EventRanpreRender)
                table.m_Loadfilter = returnfilter;
            return returnfilter;
        }

        internal static string BuildFilterElement(bool parentheses, string table, Column column, object value)
        {
            if (value != null)
                value = value.ToString().Replace("'", "''");
            if (!column.HasDataSourceQuote)
            {
                if (string.IsNullOrEmpty(table))
                    return parentheses
                               ? string.Format("[{0}] = {1}", column.ColumnId, value)
                               : string.Format("{0} = {1}", column.ColumnId, value);
                return parentheses ? string.Format("[{0}].[{1}] = {2}", table, column.ColumnId, value) : string.Format("{0}.{1} = {2}", table, column.ColumnId, value);
            }
            if (string.IsNullOrEmpty(table))
                return parentheses
                           ? string.Format("[{0}] = '{1}'", column.ColumnId, value)
                           : string.Format("{0} = '{1}'", column.ColumnId, value);
            return parentheses ? string.Format("[{0}].[{1}] = '{2}'", table, column.ColumnId, value) : string.Format("{0}.{1} = '{2}'", table, column.ColumnId, value);
        }

        internal static string BuildPKFilter(Table table, string keys, bool parentheses)
        {
            if (table.m_Grid.SystemMessage.CriticalCount > 0)
                return null;
            try
            {
                string datatable;
                if (table.Columns.Primarykeys == null || table.Columns.Primarykeys.Count == 0)
                {
                    // Use the primary key for MasterWebGrid as default if no primarykey is found.
                    if (table.m_Grid.MasterWebGrid != null)
                    {
                        StringBuilder filter = new StringBuilder(null);
                        foreach (RowCell rowcell in table.m_Grid.MasterWebGrid.MasterTable.Rows[0].GetPrimarykeys)
                        {
                            if (table.Columns.GetIndex(rowcell.CellId) <= -1) continue;
                            object value = rowcell.Value;

                            if (filter.Length > 0)
                                filter.Append(" AND ");

                            filter.AppendFormat("{0} = {1}",
                                                BuildTableElement(true,
                                                                  rowcell.Row.m_Table.DataSourceId, rowcell.CellId),
                                                value);
                        }
                        if (filter.Length > 0)
                            return filter.ToString();
                        datatable = "Your data source";
                        throw new GridException(
                            String.Format(
                                "{0} has no primarykeys, and no relation-columns was found in the master grid.",
                                datatable));
                    }
                    datatable = "Your data source";
                    throw new GridException(
                        String.Format(
                            "{0} has no primarykeys and no master grid, and WebGrid is unable to view the specific record.",
                            datatable));
                }

                datatable = table.DataSourceId;
                if (Grid.DATASOURCEID_NULL.Equals(datatable))
                    datatable = table.Columns.Primarykeys[0].DataSourceId ??
                                table.Columns.Primarykeys[0].DefaultDataSourceId;
                if (String.IsNullOrEmpty(datatable))
                    datatable = String.Empty;
                keys = keys.Replace(",,", "!");
                if (keys.IndexOf(",", StringComparison.OrdinalIgnoreCase) == -1) 
                    return BuildFilterElement(parentheses, datatable, table.Columns.Primarykeys[0],keys.Replace("!", ","));

                // more than one
                string[] ids = keys.Split(',');
                List<Column> columns = table.Columns.Primarykeys;
                StringBuilder res = new StringBuilder(String.Empty);
                for (int i = 0; i < columns.Count; i++)
                {
                    if (res.Length > 0)
                        res.Append(" AND ");

                    string value = ids[i].Replace("!", ",").Replace(Grid.EMPTYSTRINGCONSTANT, String.Empty);


                    res.Append(BuildFilterElement(parentheses, table.DataSourceId, columns[i], value));

                }
                return res.ToString();
            }
            catch (Exception ee)
            {
                throw new GridException(
                    String.Format("Error generating primary key(s) statement for DataSourceId: {0}", table.DataSourceId),
                    ee);
            }
        }

        internal static string BuildSearchString(string search, Table table, bool parentheses)
        {
            if (String.IsNullOrEmpty(search))
                return String.Empty;
            search = search.Replace("'", "''");
            bool firstC = true;
            StringBuilder filter = new StringBuilder(String.Empty);

            for (int i = 0; i < table.Columns.Count; i++)
            {
                Column c = table.Columns[i];
                if (c == null || c.IsInDataSource == false || c.Searchable == false ||
                    c.ColumnType == ColumnType.Text && ((Text)c).EncryptionKey != null)
                    continue;

                if( table.DataSourceType != DataSourceControlType.InternalDataSource  && c.ColumnType != ColumnType.Text)
                        continue;

                if (firstC)
                {
                    filter.Append("(");
                    firstC = false;
                }
                else if (search.StartsWith("-"))
                    filter.Append(" AND ");
                else
                    filter.Append(" OR ");

                if (c.ColumnType == ColumnType.Foreignkey)
                {
                    Foreignkey fk = (Foreignkey) c;
                    if (parentheses)
                    {

                        filter.Append(
                            SearchStringToWords(
                                String.Format("( {0} IN (SELECT {1} FROM [{2}] WHERE {3} LIKE '%",
                                              BuildTableElement(true, table.DataSourceId, fk.ColumnId),
                                              fk.Table.Columns.Primarykeys[0].ColumnId, fk.Table.DataSourceId,
                                              Foreignkey.BuildDisplayTextSql(
                                                  String.Format("{0}.", fk.Table.DataSourceId),
                                                  fk.ValueColumn, String.Empty,
                                                  true, fk.Table)), search, "%') )"));
                    }
                    else
                    {
                        filter.Append(
                            SearchStringToWords(
                                String.Format("( {0} IN (SELECT {1} FROM [{2}] WHERE {3} LIKE '%",
                                              BuildTableElement(false, table.DataSourceId, fk.ColumnId),
                                              fk.Table.Columns.Primarykeys[0].ColumnId, fk.Table.DataSourceId,
                                              fk.ValueColumn), search, "%') )"));
                    }
                }
                else
                {
                    if (parentheses)
                        filter.Append(
                            SearchStringToWords(String.Format(" {0} LIKE '%", BuildTableElement(true, table.DataSourceId, c.ColumnId)),
                                                search,
                                                "%' "));
                    else
                    {
                        if (String.IsNullOrEmpty(table.DataSourceId))
                            filter.Append(SearchStringToWords(String.Format(" [{0}] LIKE '%", c.ColumnId), search, "%' "));
                        else
                            filter.Append(
                                SearchStringToWords(
                                    String.Format(" {0} LIKE '%", BuildTableElement(false, table.DataSourceId, c.ColumnId)), search,
                                    "%' "));
                    }
                }
            }
            if (filter.Length > 0)
                filter.Append(" ) ");
            return filter.ToString();
        }

        internal static string BuildTableElement(bool parentheses, string table, string column)
        {
            if (string.IsNullOrEmpty(table))
                return parentheses ? string.Format("[{0}]", column) : string.Format("{0}}", column);
            return parentheses ? string.Format("[{0}].[{1}]", table, column) : string.Format("{0}.{1}", table, column);
        }

        /// <summary>
        /// Deletes a row from the database.
        /// </summary>
        /// <param name="table">The table in which to delete the row.</param>
        /// <param name="rowKeys">The row ID.</param>
        /// <returns></returns>
        internal static bool DeleteRow(Table table, string rowKeys)
        {
            if (Grid.GotHttpContext &&
                System.IO.File.Exists(String.Format("{0}\\noupdate.webgrid", HttpContext.Current.Server.MapPath("."))) &&
                table.m_Grid.Tag["allowupdate"] == null)
            {
                string content =
                    System.IO.File.ReadAllText(String.Format("{0}\\noupdate.webgrid", HttpContext.Current.Server.MapPath(".")));
                table.m_Grid.SystemMessage.Add(
                    String.IsNullOrEmpty(content)
                        ? "Inserting, updating, or deleting a database record functionality has been disabled."
                        : content);
                return false;
            }
            table.GetData(false);
            if (table.m_Grid.MasterTable.Rows.Count == 0)
                return true;
            if ( table.DataSource != null || table.m_Grid.MasterTable.Rows[0].DataRow != null)
            {
                string tmpId = table.m_Grid.InternalId;
                table.m_Grid.InternalId = rowKeys;
                table.m_Grid.MasterTable.GetData(true);

                switch (table.DataSourceType)
                {
                    case DataSourceControlType.SqlDataSource:
                    case DataSourceControlType.AccessDataSource:
                        DeleteDataSourceControl(table.m_Grid.MasterTable.Rows[0]);
                        break;
                    case DataSourceControlType.ObjectDataSource:
                        DeleteObjectDataSourceControl(table.m_Grid.MasterTable.Rows[0]);
                        break;
                }

                if (table.m_XmlDataDocument == null)
                {
                    table.m_Grid.MasterTable.Rows[0].DataRow.Delete();

                    if ( table.DataSource != null &&  table.DataSource is OleDbDataAdapter)
                    {
                        try
                        {
                            OleDbCommandBuilder updateCommand =
                                new OleDbCommandBuilder((OleDbDataAdapter) table.DataSource);

                            ((OleDbDataAdapter) table.DataSource).Update(
                                table.m_Grid.MasterTable.Rows[0].DataRow.Table);
                            updateCommand.Dispose();
                        }
                        catch (Exception ee)
                        {
                            throw new GridException("Error deleting record from data source.", ee);
                        }
                    }
                    table.m_Grid.MasterTable.Rows[0].DataRow.Table.AcceptChanges();
                }
                else if (table.m_XmlDataDocument != null)
                {

                    try
                    {
                        List<Column> datacolumns = table.Columns.Primarykeys;

                        if (datacolumns == null)
                        {
                            table.m_Grid.SystemMessage.Add("Primary key is required for the XML file to delete rows.",
                                                           true);
                            return false;
                        }

                        foreach (DataTable dt in table.m_XmlDataSet.Tables)
                        {
                            if (dt.TableName != rowKeys) continue;
                            int count = dt.Rows.Count;
                            for (int i = 0; i < count; i++)
                                table.m_XmlDataSet.Tables[dt.TableName].Rows.RemoveAt(0);
                            break;
                        }
                        table.m_XmlDataSet.AcceptChanges();
                        table.m_XmlDataSet.WriteXml(table.m_XmlDataDocument);
                    }
                    catch (Exception ee)
                    {
                        throw new GridDataSourceException("Error removing row in XML", ee);
                    }
                }

                table.m_Grid.InternalId = tmpId;

            }
            else
            {
                string datasourcetable = table.DataSourceId;
                if( datasourcetable == null)
                    return true;
                if (datasourcetable.Equals(Grid.DATASOURCEID_NULL))
                    datasourcetable = table.Columns.Primarykeys[0].DataSourceId ??
                                      table.Columns.Primarykeys[0].DefaultDataSourceId;
                InsertUpdate delete = new InsertUpdate(datasourcetable, QueryType.Delete, table.ConnectionString)
                                          {FilterExpression = BuildPKFilter(table, rowKeys, true)};

                if (table.m_Grid.Debug)
                    table.m_Grid.m_DebugString.AppendFormat("<b>{0}: SqlConnection/OleDB.DeleteRow({1}) </b>- {2}<br/>",
                                                            table.m_Grid.ID, table.DataSourceId, delete.GenerateSql());

                try
                {
                    // MUST FIX;
                    delete.Execute();
                }
                catch (Exception ee)
                {
                    throw new GridDataSourceException(String.Format("Error deleting datasource record (SQL generated:{0}) FILTER: " + BuildPKFilter(table, rowKeys, true), delete.GenerateSql()), ee);
                }
            }

            return true;
        }

        /// <summary>
        /// Updates or inserts a row into the table in the data source.
        /// </summary>
        /// <param name="row">The row to be inserted.</param>
        /// <param name="editIndex">The value of the primary key(s) (the column identifier).</param>
        /// <param name="isInsert">Indicates whether the operation is update or insert.</param>
        /// <returns>The (new) values for the primary key(s).</returns>
        internal static bool InsertUpdateRow(ref string editIndex, Row row, bool isInsert)
        {
            #region Error and permissions checks
            if (Grid.GotHttpContext &&
                System.IO.File.Exists(String.Format("{0}\\noupdate.webgrid", HttpContext.Current.Server.MapPath(".")))
                && row.m_Table.m_Grid.Tag["allowupdate"] == null)
            {
                string content =
                    System.IO.File.ReadAllText(String.Format("{0}\\noupdate.webgrid",
                                                             HttpContext.Current.Server.MapPath(".")));
                row.m_Table.m_Grid.SystemMessage.Add(
                    String.IsNullOrEmpty(content)
                        ? "The capability to insert, update, and delete has been disabled."
                        : content);
                return false;
            }
            if (row.Columns.Primarykeys.Count == 0)
            {
                row.m_Table.m_Grid.SystemMessage.Add(
                    "Your data source is missing primary key(s). This is needed to identify the row you want to update, insert or delete.");
                return false;
            }
            #endregion

            bool identity = false;

            if (row.m_Table == row.m_Table.m_Grid.MasterTable)
                row.m_Table.m_Grid.BeforeValidateEvent(ref row);
            ValidateColumns(row);

            BeforeUpdateInsertEventArgs columnArgs = new BeforeUpdateInsertEventArgs
                                                         {DataSourceId = row.m_Table.DataSourceId};
            if (isInsert)
            {
                columnArgs.m_Insert = true;
                columnArgs.m_EditIndex = null;
            }
            else
            {
                columnArgs.m_Update = true;
                columnArgs.m_EditIndex = editIndex;
            }

            for (int i = 0; i < row.Columns.Count; i++)
            {
              //  row.Columns[i].Row = row;
                CellUpdateInsertArgument ea = new CellUpdateInsertArgument();
                if (isInsert)
                {
                    ea.m_Insert = true;
                    ea.m_Update = false;
                }
                else
                {
                    ea.m_Update = true;
                    ea.m_Insert = false;
                }
                ea.Name = row.Columns[i].ColumnId;
                ea.Column = row.Columns[i];
                ea.PostBackValue = row[ea.Name].PostBackValue;
                ea.Value = row[ea.Name].Value;
                ea.DataSourceValue = row[ea.Name].DataSourceValue;

                if (row.m_Table.m_Grid.Trace.IsTracing)
                    row.m_Table.m_Grid.Trace.Trace(
                        isInsert
                            ? "{0} on insert has value '{1}' and old value was '{2}'"
                            : "{0} on update has value '{1}' and old value was '{2}'", ea.Name,
                        ea.Value,
                        ea.DataSourceValue);

                if (row.Columns[i].IsInDataSource == false || row.Columns[i].ColumnType == ColumnType.SystemColumn)
                    ea.IgnoreChanges = true;
                if (row.Columns[i].Identity)
                {
                    ea.IgnoreChanges = true;
                    identity = true;
                    if (editIndex == null && row[i].Value != null)
                        editIndex = row[i].Value.ToString();
                }
                if (ea.IgnoreChanges == false)
                    columnArgs.AcceptChanges = true;
                columnArgs.Row.Add(ea.Name, ea);
            }

            row.m_Table.m_Grid.BeforeUpdateInsertEvent(columnArgs);

            for (int i = 0; i < row.Columns.Count; i++)
                if (row.Columns[i].ColumnType == ColumnType.Foreignkey == false && row.Columns[i].IsInDataSource &&
                    row.Columns[i].ColumnType == ColumnType.SystemColumn == false)
                    row[row.Columns[i].ColumnId].Value = columnArgs.Row[row.Columns[i].ColumnId].Value;

            if (columnArgs.AcceptChanges == false || row.m_Table.m_Grid.SystemMessage.Count > 0)
                return false;

            if (String.IsNullOrEmpty(row.m_Table.DataSourceId) && row.m_Table.DataSource == null)
                return true; // No datasources.

            InsertUpdate updateColumn = null;
            bool updates = false;
            if ((String.IsNullOrEmpty(row.m_Table.m_Grid.DataSourceId) ||
                 row.m_Table.DataSourceType != DataSourceControlType.InternalDataSource) && isInsert)
            {
                if ( row.m_Table.DataSource is XmlDataDocument == false)
                    row.m_DataRow = row.m_Table.m_DataSourceColumns.NewRow();
                else
                {
                    // Detect if any node has child nodes.
                    XmlDataDocument xmldoc =  row.m_Table.DataSource as XmlDataDocument;

                    if (xmldoc != null && xmldoc.DocumentElement != null)
                    {
                        XmlNodeList nodeList = xmldoc.
                            DocumentElement.SelectNodes(
                            row.m_Table.m_Xmlxpath);
                        if (nodeList != null)
                            foreach (XmlNode xmlNode in nodeList)
                            {
                                DataRow myrow =
                                    ((XmlDataDocument)  row.m_Table.DataSource).GetRowFromElement(
                                        (XmlElement) xmlNode);
                                if (myrow == null)
                                    continue;

                                if (!xmlNode.HasChildNodes) continue;
                                row.m_Table.m_Grid.SystemMessage.Add(
                                    "This Xml node has child nodes and can only be updated or removed from the loaded XML document.");
                                return false;
                            }
                    }
                    else
                        throw new GridException("XmlDataDocument is null or have a invalid root element (DocumentElement is null)");
                    updates = true;
                    row.m_DataRow = row.m_Table.m_DataSourceColumns.NewRow();
                }
            }

            if (row.DataRow == null)
            {
                string datasourcetable = row.m_Table.DataSourceId;
                if (Grid.DATASOURCEID_NULL == datasourcetable)
                    datasourcetable = row.Columns.Primarykeys[0].DataSourceId ?? row.Columns.Primarykeys[0].DefaultDataSourceId;
                updateColumn =
                    new InsertUpdate(datasourcetable, QueryType.Update,
                                     row.m_Table.m_Grid.ActiveConnectionString);
                if (isInsert)
                    updateColumn.QueryType = QueryType.Insert;
                else
                    updateColumn.FilterExpression = BuildPKFilter(row.m_Table, editIndex, true);
            }
            else
            {
                if ( row.m_Table.DataSource is XmlDataDocument && isInsert == false)
                {
                    if (((XmlDataDocument) row.m_Table.DataSource).DocumentElement == null)
                        throw new GridException("Root of your XmlDocument is null" + row.m_Table.DataSource);
                    List<Column> datacolumns = row.m_Table.Columns.Primarykeys;

                    XmlNodeList nodeList = ((XmlDataDocument)row.m_Table.DataSource).DocumentElement.
                        SelectNodes(
                        row.m_Table.m_Xmlxpath);

                    if (nodeList != null)
                        foreach (XmlNode xmlNode in nodeList)
                        {
                            DataRow myrow =
                                ((XmlDataDocument)row.m_Table.DataSource).GetRowFromElement(
                                    (XmlElement) xmlNode);
                            if (myrow == null)
                                continue;

                            if (xmlNode.HasChildNodes)
                            {
                                if (DetectDataSourceRow(row.m_DataRow, myrow, datacolumns))
                                {
                                    row.m_DataRow = myrow;
                                    updates = true;
                                    break;
                                }
                            }
                            else if (DetectDataSourceRow(row.m_DataRow, myrow, datacolumns))
                            {
                                updates = true;
                                row.m_DataRow = myrow;
                                break;
                            }
                        }
                }
            }

            try
            {
                for (int i = 0; i < row.Columns.Count; i++)
                {
                    if (row.Columns[i].IsInDataSource == false || row.Columns[i].ColumnType == ColumnType.SystemColumn ||
                        !String.IsNullOrEmpty(row.Columns[i].DataSourceId) &&
                        row.Columns[i].ColumnType != ColumnType.Foreignkey &&
                        row.Columns[i].DataSourceId.Equals(row.m_Table.DataSourceId) == false)
                        continue;

                    CellUpdateInsertArgument ea = columnArgs.Row[row.Columns[i].ColumnId];

                   if ( ReferenceEquals(Grid.NULLCONSTANT, ea.Value) || ea.Value == null)
                        ea.Value = DBNull.Value;

                    if (row.Columns[i].ColumnType == ColumnType.Foreignkey == false)
                        row.Columns[i].OnUpdateInsert(ea, row[row.Columns[i].ColumnId]);

                    if (row.m_Table.m_Grid.SystemMessage.Count > 0)
                        return false;

                    if (ea.IgnoreChanges || (row.Columns[i].AllowEdit == false &&
                                             (isInsert == false ||
                                              (row.Columns[i].Primarykey == false &&
                                               (row.Columns[i].Required == false &&
                                                row.Columns[i].IsInDataSource == false)))) ||
                        (row.m_Table.m_Grid.DisplayView == DisplayView.Grid &&
                         row.Columns[i].AllowEditInGrid == false) ||
                        (ea.Value == ea.DataSourceValue && isInsert == false))
                    {
                        StringBuilder reason = new StringBuilder();

                        if (ea.IgnoreChanges)
                            reason.Append("ignore");
                        if (isInsert && row.Columns[i].Primarykey == false)
                            reason.Append(" was not primary key on insert");
                        if (row.m_Table.m_Grid.Trace.IsTracing)
                            row.m_Table.m_Grid.Trace.Trace(
                                isInsert
                                    ? "skipping {0} on insert has value '{1}' and old value was '{2}' (reason: {3})"
                                    : "skipping {0} on update has value '{1}' and old value was '{2}' (reason: {3})",
                                ea.Name, ea.Value, ea.DataSourceValue, reason.ToString());

                        continue;
                    }

                    switch (row.Columns[i].ColumnType)
                    {
                        case ColumnType.File:
                        case ColumnType.Image:
                            if (row.Columns[i].IsBlob == false &&
                                row.Columns[i].FileNameColumn != row.Columns[i].ColumnId)
                                continue;
                            if (ea.Parameter != null && row.Columns[i].FileNameColumn != row.Columns[i].ColumnId)
                            {
                                if (row.DataRow == null)
                                    updateColumn.Add(row.Columns[i].ColumnId, (BinaryReader) ea.Parameter);
                                else if (row.DataRow.Table.Columns.Contains(row.Columns[i].ColumnId))
                                    row.DataRow[row.Columns[i].ColumnId] = ea.Parameter;
                            }
                            else if (Grid.GotHttpContext &&row.Columns[i].FileNameColumn == row.Columns[i].ColumnId &&
                                     HttpContext.Current.Session[row[row.Columns[i].ColumnId].CellClientId + "_img"] != null)
                            {
                                object myvalue = HttpContext.Current.Session[row[row.Columns[i].ColumnId].CellClientId + "_img"];
                                if (row.DataRow == null)
                                    updateColumn.Add(row.Columns[i].ColumnId, myvalue);
                                else if (row.DataRow.Table.Columns.Contains(row.Columns[i].ColumnId))
                                    row.DataRow[row.Columns[i].ColumnId] = myvalue;
                            }
                            break;
                        default:
                            if (row.DataRow == null)
                            {
                                if (updateColumn != null)
                                    updateColumn.Add(row.Columns[i].ColumnId, ea.Value);
                            }
                            else if (row.DataRow.Table.Columns.Contains(row.Columns[i].ColumnId))
                                row.DataRow[row.Columns[i].ColumnId] = ea.Value;
                            break;
                    }
                    updates = true;
                }
            }
            catch (Exception ee)
            {
                throw new GridDataSourceException("Error generating update/insert sql", ee);
            }
            if (row.m_Table.m_Grid.Debug && updateColumn != null)
                row.m_Table.m_Grid.m_DebugString.AppendFormat("<b>Datainterface.InsertUpdateRow</b> - {0}<br/>",
                                                              updateColumn.GetQueryString());

            string res = null;
            if (updates)
            {
                try
                {
                    if (row.DataRow == null && row.m_Table.DataSource == null)
                        res = updateColumn.Execute();
                    else
                    {
                        if ( row.m_Table.DataSource != null &&
                             row.m_Table.DataSource is OleDbDataAdapter)
                        {
                            OleDbCommandBuilder updateCommand =
                                new OleDbCommandBuilder((OleDbDataAdapter)  row.m_Table.DataSource);
                            if (isInsert)
                                row.DataRow.Table.Rows.Add(row.DataRow);

                            ((OleDbDataAdapter)  row.m_Table.DataSource).Update(row.DataRow.Table);
                            updateCommand.Dispose();
                        }
                        else if (row.m_Table.m_XmlDataDocument != null)
                        {
                            try
                            {
                                if (isInsert)
                                    row.DataRow.Table.Rows.Add(row.DataRow);
                               row.m_Table.m_XmlDataSet.WriteXml(row.m_Table.m_XmlDataDocument);
                            }
                            catch (Exception ee)
                            {
                                throw new GridDataSourceException("Error processing xml for updating/inserting", ee);
                            }
                        }
                        else
                        {
                            if (isInsert && row.DataRow != null)
                                row.DataRow.Table.Rows.Add(row.DataRow);
                            switch (row.m_Table.DataSourceType)
                            {
                                case DataSourceControlType.SqlDataSource:
                                case DataSourceControlType.AccessDataSource:
                                    UpdateInsertDataSourceControl(row, isInsert);
                                    break;
                                case DataSourceControlType.ObjectDataSource:
                                    UpdateInsertObjectDataSourceControl(row, isInsert);
                                    break;
                                default:
                                    row.m_Table.m_Grid.BindDataSourceSession();
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ee)
                {
                    if (updateColumn != null)
                        throw new GridDataSourceException("Error executing insert/update sql", ee);
                    throw new GridDataSourceException("Error updating data source", ee);
                }
                if (res != null && isInsert)
                {
                    editIndex = res;

                    foreach (Column column in row.m_Table.Columns)
                    {
                        if (!column.Identity)
                            continue;
                        row[column.ColumnId].Value = editIndex;
                        break;
                    }
                }
            }

            for (int i = 0; i < row.Columns.Count; i++)
                if (!String.IsNullOrEmpty(row.Columns[i].DataSourceId) &&
                    row.m_Table.m_Grid[row.Columns[i].ColumnId].ColumnType == ColumnType.Foreignkey == false)
                    row.Columns[i].UpdateInsertColumnDataSourceId(row, columnArgs, editIndex);
            if (identity == false)
                editIndex = row.PrimaryKeyUpdateValues;
            return true;
        }

        internal static void LoadColumnSettings(DataTable columnSettings, Table table)
        {
            if (columnSettings == null)
                return;
            if (!"SchemaTable".Equals(columnSettings.TableName))
                columnSettings = columnSettings.CreateDataReader().GetSchemaTable();

            int displayindex = 10;

            foreach (DataRow row in columnSettings.Rows)
            {
                DataBaseColumnInformation dbci = new DataBaseColumnInformation
                                                     {
                                                         dataType = row["DataType"] as Type,
                                                         canNull = (bool) row["AllowDBNull"],
                                                         displayindex = displayindex
                                                     };

                displayindex += 10;

                dbci.columnName = row["ColumnName"] as string;
                if (dbci.dataType == typeof (string))
                    dbci.maxSize = (int?) row["ColumnSize"];
                dbci.title = dbci.columnName;
                dbci.defaultDataSourceId = row["BaseTableName"] as string;
                //   dbci.defaultValue = datacolumn.DefaultValue;
                if (columnSettings.Columns.Contains("IsKey") && (bool) row["IsKey"])
                {
                    dbci.uniquevalue = true;
                    dbci.primaryKey = true;
                }
                if ((columnSettings.Columns.Contains("IsIdentity") && (bool) row["IsIdentity"])
                    || (columnSettings.Columns.Contains("IsAutoIncrement") && (bool) row["IsAutoIncrement"]))
                {
                    dbci.readOnly = true;
                    dbci.identity = true;
                }
                else
                    dbci.readOnly = (bool) row["IsReadOnly"];

                LoadTableColumns(table, dbci);
            }
            if (displayindex > 10)
                table.m_LoadedTableSchema = true;
        }

        internal static void LoadTableColumns(Table table, DataBaseColumnInformation dbci)
        {
            ColumnType columnType = MapColumnType(Type.GetTypeCode(dbci.dataType));
            if (table.Columns.GetIndex(dbci.columnName) == -1)
            {
                Column column = Column.GetColumnFromType(dbci.columnName, table, columnType);

                column.m_Table = table;
                column.IsCreatedByWebGrid = true;
                table.Columns.Add(column);
            }
            else if (table.Columns[dbci.columnName].ColumnType == ColumnType.Unknown)
                table.Columns[dbci.columnName].ColumnType = columnType;

            if (columnType == ColumnType.File || columnType == ColumnType.Image)
                table.Columns[dbci.columnName].IsBlob = true;
            else if ((table.Columns[dbci.columnName].ColumnType == ColumnType.File ||
                      table.Columns[dbci.columnName].ColumnType == ColumnType.Image) &&
                     string.IsNullOrEmpty(table.Columns[dbci.columnName].FileNameColumn))
                table.Columns[dbci.columnName].FileNameColumn = table.Columns[dbci.columnName].ColumnId;
            table.Columns[dbci.columnName].LoadDataSourceInformation(dbci);
        }

        internal static ColumnType MapColumnType(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Empty:
                case TypeCode.DBNull:
                    throw new NotImplementedException("Empty and DBNull Data Types are not supported.");
                case TypeCode.Boolean:
                    return ColumnType.Checkbox;
                case TypeCode.Object:
                case TypeCode.SByte:
                case TypeCode.Byte:
                    return ColumnType.File;
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                    return ColumnType.Number;
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return ColumnType.Decimal;
                case TypeCode.DateTime:
                    return ColumnType.DateTime;
                case TypeCode.Char:
                case TypeCode.String:
                    return ColumnType.Text;
                default:
                    throw new GridException(string.Format("Could not map data-type: {0}", typeCode));
            }
        }

        /// <summary>
        /// SQL Builder for Access and SQL.
        /// </summary>
        /// <param name="table">The table to build SQL for.</param>
        /// <returns>SQL</returns>
        internal static string SqlBuilder(Table table)
        {
            StringBuilder sql;
            if (table == table.m_Grid.MasterTable && String.IsNullOrEmpty(table.m_Grid.Sql) ||
                table != table.m_Grid.MasterTable)
            {
                #region Generate SQL

                string top = String.Empty;
                if (table.m_Grid.PageSize != 0 && (string.IsNullOrEmpty(table.m_Grid.TreeParentId) || table != table.m_Grid.MasterTable))
                {
                    int recordsToFetch = (table.m_Grid.PageSize*table.m_Grid.PageIndex);
                    
                    if (table.m_Grid.InternalId == null && table == table.m_Grid.MasterTable &&
                        table.m_Grid.PageIndex != 0)
                        top = String.Format(" TOP {0} ", recordsToFetch);

                }

                StringBuilder foreignTextColumns = new StringBuilder(String.Empty);
                StringBuilder foreignJoinColumns = new StringBuilder(String.Empty);
                int numberOfForeignkeys = 0;
                if (table == table.m_Grid.MasterTable) // && table.Parent.Mode == Modes.Grid)
                {

                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        if (table.Columns[i].ColumnType != ColumnType.Foreignkey ||
                            table.Columns[i].DataSourceId == null)
                            continue;
                        Foreignkey fk = (Foreignkey) table.Columns[i];
                        if (fk.Table.DataSource != null)
                            continue;
                        string fkdatatable = fk.Grid.DataSourceId;
                        if (table.m_Grid.Trace.IsTracing)
                            table.m_Grid.Trace.Trace("ValueColumn for {0} Value:{1}", fk.ColumnId, fk.ValueColumn);
                        if (fk.Table.Columns.Primarykeys == null)
                        {
                            throw new GridException(
                                String.Format(
                                    "Foreignkey '{0}' in Grid '{1}' with DataSourceId: '{2}' does not have any primary key.",
                                    fk.ColumnId, fk.Grid.ID, fk.Table.DataSourceId));
                        }
                        if (table.m_Grid.Trace.IsTracing)
                            table.m_Grid.Trace.Trace("ValueColumn for {0} Value:{1}", fk.ColumnId, fk.ValueColumn);
                        try
                        {
                            if (fk.m_ConnectionStringSet == false && fk.IsInDataSource)
                            {
                                string pkalias = String.Format("_pk{0}_{1}", i, fk.DataSourceId);

                                foreignTextColumns.Append(
                                    Foreignkey.BuildDisplayTextSql(String.Format(", [{0}].", pkalias),
                                                                   fk.ValueColumn,
                                                                   String.Format(" as [_fk_{0}_text] ",
                                                                                 fk.ColumnId), false, fk.Table));
                                foreignJoinColumns.Append(" LEFT OUTER JOIN ");

                                if (fk.DataSourceId.IndexOf(".", StringComparison.OrdinalIgnoreCase) == -1)
                                {
                                    if (fkdatatable.IndexOf(".", StringComparison.OrdinalIgnoreCase) == -1)
                                        foreignJoinColumns.AppendFormat("[{0}] as [{1}] ", fk.DataSourceId, pkalias);
                                    else
                                    {
                                        // Grid table is [database].dbo.[table]
                                        string _table =
                                            String.Format("{0}.dbo.[{1}]",
                                                          fkdatatable.Substring(0, fkdatatable.IndexOf(".dbo.")),
                                                          fk.DataSourceId);
                                        foreignJoinColumns.AppendFormat("[{0}] as [{1}] ", _table, pkalias);
                                    }
                                    foreignJoinColumns.AppendFormat("ON [{0}].", table.DataSourceId);
                                }
                                else
                                    foreignJoinColumns.AppendFormat("{0} as [{1}] ON {2}.", fk.DataSourceId, pkalias,
                                                                    table.DataSourceId);

                                List<Column> primarykeys = fk.Table.Columns.Primarykeys;

                                if (primarykeys == null || primarykeys.Count == 0)
                                    throw new GridException(
                                        String.Format(
                                            "Foreignkey table '{0}' for column '{1}' in grid '{2}' does not contain any primary keys."
                                            , fk.Table.DataSourceId, fk.Title, fk.Grid.Title));

                                int columns = 0;
                                foreach (Column primarycolumn in primarykeys)
                                {
                                    if (columns > 0)
                                    {
                                        if (table.Columns[primarycolumn.ColumnId].IsInDataSource)
                                            foreignJoinColumns.AppendFormat(" and {0} = {1}",
                                                                            BuildTableElement(true,
                                                                                              table.DataSourceId,
                                                                                              primarycolumn.
                                                                                                  ColumnId),
                                                                            BuildTableElement(true, pkalias,
                                                                                              primarycolumn.
                                                                                                  ColumnId));
                                        else if (
                                            HttpContext.Current.Session[
                                                String.Format("WG{0}", primarycolumn.ColumnId)] != null)
                                            foreignJoinColumns.AppendFormat(" and {0} = '{1}'",
                                                                            BuildTableElement(true, pkalias,
                                                                                              primarycolumn.
                                                                                                  ColumnId),
                                                                            HttpContext.Current.Session[
                                                                                String.Format("WG{0}",
                                                                                              primarycolumn.ColumnId)]);
                                    }
                                    else
                                        foreignJoinColumns.AppendFormat("[{0}] = {1}", fk.ColumnId,
                                                                        BuildTableElement(true, pkalias,
                                                                                          primarycolumn.ColumnId));
                                    columns++;
                                }
                                if (columns > 0)
                                    foreignJoinColumns.Append(")");
                                numberOfForeignkeys++;
                            }
                            else
                            {
                                foreignTextColumns.AppendFormat(", NULL as [_fk_{0}_text] ", fk.ColumnId);
                            }
                        }
                        catch (Exception ee)
                        {
                            throw new GridException(
                                String.Format("Error generating SQL statement for DataSourceID: {0}", table.DataSourceId),
                                ee);
                        }
                    }
                    if (table.m_Grid.Trace.IsTracing)
                        table.m_Grid.Trace.Trace(String.Format("ForeignKeys={0}", numberOfForeignkeys));
                    //ForeignTextColumns
                }

                StringBuilder whatToSelect = new StringBuilder(String.Empty);
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    // FILES IN DATABASE SHOULD NOT BE LOADED HERE. -Olav
                    // IS ONLY ACTIVE IF FILENAMECOLUMN IS ACTIVE.
                    if (table.Columns[i].IsInDataSource == false || table.Columns[i].IsBlob)
                        continue;

                    StringBuilder colname;
                    if (String.IsNullOrEmpty(table.Columns[i].DataSourceId) == false &&
                        table.Columns[i].ColumnType == ColumnType.Foreignkey == false)
                    {
                        string datatable = table.DataSourceId; //table.Columns[i].DataSourceId;

                        string filter = table.Columns[i].GetDataTableFilterExpression(table.DataSourceId);
                        // THERE ARE ONLY IDENTITY.. ASSUMING USER WILL ONLY INSERT AND DO NOT UPDATE..
                        // INVESTIGATE WHEN TIME...
                        if (filter.Equals("IDENTITY", StringComparison.OrdinalIgnoreCase) == false)
                        {

                            colname = new StringBuilder(
                                String.Format("( SELECT TOP 1 {0} FROM [{1}]", BuildTableElement(true, datatable,
                                                                                                 table.Columns[i].
                                                                                                     ColumnId),
                                              table.Columns[i].DataSourceId));

                            if (filter.Length > 1)
                                colname.AppendFormat(" WHERE {0})", filter);
                            else
                                colname.Append(" )");
                        }
                        else
                            continue;
                    }
                    else
                    {
                        colname =
                            new StringBuilder(BuildTableElement(true, table.DataSourceId, table.Columns[i].ColumnId));
                    }
                    if (table.Columns[i].ColumnType == ColumnType.DateTime &&
                        (table.m_Grid.MasterTable.DataBaseInterface is SqlConnection ||
                         (table.m_Grid.ActiveConnectionString.IndexOf("Provider=sqloledb",
                                                                      StringComparison.OrdinalIgnoreCase) > -1 ||
                          table.m_Grid.ActiveConnectionString.IndexOf("Provider=SQLNCLI",
                                                                      StringComparison.OrdinalIgnoreCase) > -1)))
                    {
                        colname =
                            new StringBuilder(
                                String.Format("CONVERT(varchar,{0},120) as [{1}]", colname,
                                              table.Columns[i].ColumnId));
                    }
                    else if (table.Columns[i].DataSourceId != null)
                        colname.AppendFormat(" as [{0}]", table.Columns[i].ColumnId);
                    if (whatToSelect.Length > 0)
                        whatToSelect.Append(", ");
                    whatToSelect.Append(colname);
                }
                //}

                if (table.m_Grid.Distinct)
                    top = String.Format("DISTINCT {0}", top);
                if (whatToSelect.Length == 0)
                    whatToSelect = new StringBuilder("*");

                sql = new StringBuilder();
                sql.AppendFormat("SELECT {0} {1} {2} FROM {3} [{4}] {5} ", top, whatToSelect, foreignTextColumns,
                                 "".PadLeft(numberOfForeignkeys, '(')
                                 , table.DataSourceId, foreignJoinColumns);

                #endregion
            }
            else
                sql = new StringBuilder(table.m_Grid.Sql);

            sql.Append(BuildFilter(table, true));

            // Do we have a column name to sort by ?
            bool doSort = !String.IsNullOrEmpty(table.SortExpression);

            // If it is MasterTable, don't sort unless we're in grid view.
            doSort &= (table.m_Grid.MasterTable == table &&
                       (table.m_Grid.InternalId != null || table.m_Grid.DisplayView != DisplayView.Grid)) == false;

            // Finally....SORT! :D
            if (doSort)
                if (!string.IsNullOrEmpty(table.GroupByExpression))
                {
                  
                    sql.AppendFormat(" ORDER BY {0},{1}", BuildGroupExpression(table), table.SortExpression);
                }
                else
                    sql.AppendFormat(" ORDER BY {0}", table.SortExpression);
            else if (!string.IsNullOrEmpty(table.GroupByExpression))
                sql.AppendFormat(" ORDER BY {0}", BuildGroupExpression(table));

            return sql.ToString();
        }

        private static string BuildGroupExpression(Table table)
        {
            string[] expressions = table.GroupByExpression.Split(',');
            string sqlexp = string.Empty;
            foreach (string expression in expressions)
            {
                if (!table.Columns.Contains(expression))
                    throw new GridException(string.Format("Table expression '{0}' is does not exist.",
                                                          expression));
                if (sqlexp != string.Empty)
                    sqlexp += ", ";
                sqlexp += BuildTableElement(true, table.DataSourceId, expression);
            }
            return sqlexp;
        }

        private static void ApplyParameterCollection(Row row, ParameterCollection parameters)
        {
            ApplyParameterCollection(row, parameters, null);
        }

        private static void ApplyParameterCollection(Row row, ParameterCollection parameters, string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                prefix = "{0}";
            foreach (RowCell cell in row.Cells)
            {
                string value = null;
                if (cell.Value != null)
                    value = cell.Value.ToString();

                if (parameters[cell.CellId] == null)
                    parameters.Add(cell.CellId, row.Columns[cell.CellId].DataType, value);
                else
                    parameters[cell.CellId].DefaultValue = value;

                if (parameters[String.Format("@" + prefix, cell.CellId)] == null)
                    parameters.Add(String.Format("@" + prefix, cell.CellId), row.Columns[cell.CellId].DataType, value);
                else
                    parameters[String.Format("@" + prefix, cell.CellId)].DefaultValue = value;
            }
        }

        private static void DeleteDataSourceControl(Row row)
        {
            SqlDataSource datasource;

            if (row.m_Table.ControlDataSource is AccessDataSource)
                datasource = (AccessDataSource) row.m_Table.ControlDataSource;
            else
                datasource = (SqlDataSource) row.m_Table.ControlDataSource;
            if (String.IsNullOrEmpty(datasource.DeleteCommand))
                return;
            ApplyParameterCollection(row, datasource.DeleteParameters);

            int count = datasource.Delete();
            if (count > 0)
                row.m_Table.m_Grid.ReLoadData = true;
        }

        private static void DeleteObjectDataSourceControl(Row row)
        {
            ObjectDataSource datasource = (ObjectDataSource) row.m_Table.ControlDataSource;
            if (String.IsNullOrEmpty(datasource.DeleteMethod))
                throw new GridException(string.Format("No DeleteMethod for ObjectDataSource object '{0}' is provided",
                                                      datasource.ID));
            ApplyParameterCollection(row, datasource.DeleteParameters);
            if (datasource.DeleteParameters.Count == 0)
                throw new GridException("No delete parameters where found.");

            int count = datasource.Delete();
            if (count > 0)
                row.m_Table.m_Grid.ReLoadData = true;
        }

        private static bool DetectDataSourceRow(DataRow sourceRow, DataRow targetRow, IEnumerable<Column> keyColumns)
        {
            bool retVal = true;
            foreach (Column column in keyColumns)
            {
                if (targetRow[column.ColumnId] == null)
                    continue;
                retVal = sourceRow[column.ColumnId].Equals(targetRow[column.ColumnId]);

                if (!retVal) break;
            }

            return retVal;
        }

        private static string SearchStringToWords(string preFix, string search, string postFix)
        {
            string[] splitIntoWords = search.Split('"');
            for (int i = 0; i < splitIntoWords.Length; i++)
                splitIntoWords[i] = i % 2 == 1 ? splitIntoWords[i].Replace(" ", "\b") : splitIntoWords[i].Replace(",", " ").Replace(".", " ").Replace("?", " ").Replace("!", " ");

            search = String.Join(" ", splitIntoWords);

            string[] words = search.Replace("  ", " ").Trim().Split(' ');
            StringBuilder searchString = new StringBuilder();
            for (int i = 0; i < words.Length; i++)
            {
                string sQLOperator = String.Empty;
                if (words[i].StartsWith("-"))
                    sQLOperator = String.IsNullOrEmpty(searchString.ToString()) ? " NOT " : " AND NOT ";
                else if (!String.IsNullOrEmpty(searchString.ToString()))
                    sQLOperator = " OR ";
                searchString.AppendFormat("{0}{1}{2}{3}", sQLOperator, preFix,
                                          words[i].Replace("\b", " ").Replace("-", String.Empty).Replace("+", String.Empty).Trim(), postFix);
            }
            //				words[i] = words[i].Replace("\b"," ").Trim();
            return searchString.ToString();
        }

        private static void UpdateInsertDataSourceControl(Row row, bool isInsert)
        {
            SqlDataSource datasource;

            if (row.m_Table.ControlDataSource is AccessDataSource)
                datasource = (AccessDataSource)row.m_Table.ControlDataSource;
            else
                datasource = (SqlDataSource)row.m_Table.ControlDataSource;

            if (!isInsert && !String.IsNullOrEmpty(datasource.UpdateCommand))
            {
                ApplyParameterCollection (row, datasource.UpdateParameters);
                int count = datasource.Update();
                if (count > 0)
                    row.m_Table.m_Grid.ReLoadData = true;
            }
            else if (isInsert && !String.IsNullOrEmpty(datasource.InsertCommand))
            {
                ApplyParameterCollection(row, datasource.InsertParameters);

                if (datasource.InsertParameters.Count == 0)
                    throw new GridException("No insert parameters where found.");

                int count = datasource.Insert();

                if (count > 0)
                    row.m_Table.m_Grid.ReLoadData = true;
            }
        }

        private static void UpdateInsertObjectDataSourceControl(Row row, bool isInsert)
        {
            ObjectDataSource datasource = (ObjectDataSource) row.m_Table.ControlDataSource;

            if (!isInsert && !String.IsNullOrEmpty(datasource.UpdateMethod))
            {

                ApplyParameterCollection(row, datasource.UpdateParameters);
                int count = datasource.Update();
                if (count > 0)
                    row.m_Table.m_Grid.ReLoadData = true;
            }
            else if (isInsert && !String.IsNullOrEmpty(datasource.InsertMethod))
            {
                IDataSource ds = datasource;
                DataSourceView view = ds.GetView("DefaultView");

                Dictionary<string, object> values = new Dictionary<string, object>();

                foreach (RowCell cell in row.Cells)
                    if (cell.Value != null)
                        values.Add(cell.CellId,
                                   cell.Value == DBNull.Value
                                       ? Convert.ChangeType(null, row.Columns[cell.CellId].DataType)
                                       : Convert.ChangeType(cell.Value, row.Columns[cell.CellId].DataType));
                view.Insert(values, delegate { return false; });
            }
        }

        /// <summary>
        /// Validates the columns.
        /// </summary>
        /// <param name="row">The row.</param>
        private static void ValidateColumns(Row row)
        {
            if (row == null)
                return;

            for (int i = 0; i < row.Columns.Count; i++)
            {
                if (row.Columns[i].ColumnType == ColumnType.SystemColumn || row.Columns[i].Identity ||
                    (row.Columns[i].AllowEdit == false && row.Columns[i].Required == false &&
                     row.Columns[i].IsInDataSource == false && row.m_Table.m_Grid.InternalId != null)
                    || (row.m_Table.m_Grid.DisplayView == DisplayView.Grid && row.Columns[i].AllowEditInGrid == false) ||
                    (row.m_Table.m_Grid.DisplayView == DisplayView.Detail && row.Columns[i].Visibility == Visibility.Grid))
                    continue;
                row.Columns[i].Validate(row[row.Columns[i].ColumnId]);
            }
        }

        #endregion Methods
    }

    /// <summary>
    /// WebGrid database interface, used to communicate with database.
    /// </summary>
    internal class NamespaceDoc
    {
    }
}