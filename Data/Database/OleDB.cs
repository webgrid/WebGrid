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

namespace WebGrid.Data.Database
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.OleDb;
    using System.Web;

    using Design;
    using Enums;
    using Util;

    /// <summary>
    /// Provides communication between an Access database and WebGrid.
    /// </summary>
    internal class OleDB : Interface
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether this <see cref="Interface"/> is cachable.
        /// </summary>
        /// <value><c>true</c> if cachable; otherwise, <c>false</c>.</value>
        public override bool Cachable
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether this interface requires a requires connection string
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [requires connection string]; otherwise, <c>false</c>.
        /// </value>
        public override bool RequiresConnectionString
        {
            get { return true; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Loads one or more rows from the Access database into the specified Table.
        /// </summary>
        /// <param name="table">The Table object to populate with rows.</param>
        public override void GetData(Table table)
        {
            if (table.m_Grid.SystemMessage.CriticalCount > 0 || (table.DataSourceId == null && table.Sql == null))
                return;

            string sql = SqlBuilder(table);

            if (table.m_Grid.Debug)
                table.m_Grid.m_DebugString.AppendFormat("<b>OleDB.GetData</b> - {0}<br/>", sql);

            DataTable q = new DataTable();

            try // FAEN
            {
                OleDbConnection myConn = new OleDbConnection(table.ConnectionString);
                OleDbDataAdapter wgoledbadapter = new OleDbDataAdapter(sql, myConn);

                int startindex = 0;
                if (table.m_Grid.MasterTable == table && !string.IsNullOrEmpty(table.m_Grid.Sql))
                {
                    table.SetDataSource(wgoledbadapter, null);
                }
                if (table.m_Grid.PageIndex != 1)
                {
                    startindex = (table.m_Grid.PageIndex - 1) * table.m_Grid.PageSize;
                    if (startindex < 0)
                        startindex = 0;
                }
                if (table != table.m_Grid.MasterTable || table.m_Grid.PageIndex == 0 || table.m_Grid.InternalId != null)
                    wgoledbadapter.Fill(q);
                else
                    wgoledbadapter.Fill(startindex, table.m_Grid.PageSize, q);
            }
            catch (Exception ee)
            {
                throw table.m_Grid.MasterTable == table && string.IsNullOrEmpty(table.m_Grid.Sql) == false
                          ? new GridException(table.m_Grid.Sql, "Error retrieving data from OleDb data interface", ee)
                          : new GridException(sql, "Error retrieving data from OleDb data interface", ee);
            }

            const int treelevel = 0;

            int tablecolumns = table.Columns.Count;
            List<int> skipcolumns = new List<int>();

            try
            {
                for (int row = treelevel; row < q.Rows.Count; row++)
                {
                    DataRow datarow = q.Rows[row];

                    if (datarow == null)
                        continue;

                    Row r = new Row(table);

                    if (!string.IsNullOrEmpty(table.m_Grid.Sql))
                        r.m_DataRow = datarow;

                    for (int i = 0; i < tablecolumns; i++)
                    {
                        Column tablecolumn = table.Columns[i];
                        int columnIndex = i;

                        RowCell c = new RowCell(tablecolumn.ColumnId, r);
                        if (tablecolumn.ColumnType == ColumnType.ColumnTemplate)
                            c.Value = ((ColumnTemplate)tablecolumn).CreateCellControls;

                        c.Row = r;
                        r.Cells.Add(c);
                        if (skipcolumns.Contains(columnIndex.GetHashCode()))
                            continue;
                        if ((tablecolumn.IsInDataSource == false || tablecolumn.ColumnType == ColumnType.SystemColumn) ||
                            (tablecolumn.IsBlob))
                        {
                            skipcolumns.Add(columnIndex.GetHashCode());
                            continue;
                        }

                        if (datarow[c.CellId] is DBNull)
                            c.DataSourceValue = tablecolumn.OnLoadFromDatabase(null);
                        else
                            c.DataSourceValue = tablecolumn.OnLoadFromDatabase(datarow[c.CellId]);
                        c.Value = c.DataSourceValue;

                        if (tablecolumn.ColumnType == ColumnType.Text &&
                            ((Text)tablecolumn).EncryptionKey != null)
                        {
                            // Decrypt
                            c.DataSourceValue =
                                Security.Decrypt(c.DataSourceValue.ToString(),
                                                 ((Text)tablecolumn).EncryptionKey);
                            c.Value = c.DataSourceValue;
                        }
                        // Foreign key stuff
                        if (tablecolumn.ColumnType != ColumnType.Foreignkey || string.IsNullOrEmpty(tablecolumn.DataSourceId))
                            continue;

                        if (!string.IsNullOrEmpty(table.m_Grid.Sql) || ((Foreignkey)tablecolumn).Table.DataSource != null)
                            continue;
                        c.DataSourceDisplayValue = datarow[string.Format("_fk_{0}_text", c.CellId)];
                        c.DisplayValue = datarow[string.Format("_fk_{0}_text", c.CellId)];
                    }
                    table.Rows.Add(r);
                }
            }
            catch (Exception ee)
            {
                q.Dispose();
                throw new GridException("Error populating grid with data from OleDb data interface", ee);
            }
            if (table.Rows.Count == 0 && table == table.m_Grid.MasterTable && (table.m_Grid.DisplayView == DisplayView.Detail))
            {
                if (table.m_Grid.m_IsOneToOneRelationGrid == false)
                    table.m_Grid.SystemMessage.Add(string.Format("{0} ({1})", table.m_Grid.GetSystemMessage("SystemMessage_NoRecord"), table.m_Grid.InternalId), true);
                else
                {
                    table.m_Grid.m_IsOneToOneRelationNewRecord = true;
                }
            }
            q.Dispose();
        }

        /// <summary>
        /// Gets the schema for the table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="retrieveForeignkeys">Whether foreignkeys also should be loaded.</param>
        public override void GetSchema(Table table, bool retrieveForeignkeys)
        {
            if (table.m_Grid.Trace.IsTracing)
                table.m_Grid.Trace.Trace(
                    string.Format("OleDB - GetSchema from DataSource: {0} DataSource: {1} ", table.m_Grid.ID,
                                  table.DataSourceId));
            LoadColumns(table);
            if (table.m_Grid.SystemMessage.CriticalCount == 0 && retrieveForeignkeys)
                LoadForeignKeys(table);
        }

        //    private bool tablecolumnsloaded = false;
        public override void LoadColumns(Table table)
        {
            // Halt Load columns if there are critical errors.
            if (table.m_LoadedTableSchema || string.IsNullOrEmpty(table.DataSourceId) ||
                table.m_Grid.SystemMessage.CriticalCount > 0)
                return;

            //		tablecolumnsloaded = true;
            /*if (table.DataSourceId == null )
            {
                table.grid.SystemMessage.Add(table.grid.GetSentence("SystemMessage_DataSource_NoTableName"));
                return;
            }*/
            if (table.m_Grid.Trace.IsTracing) table.m_Grid.Trace.Trace("LoadColumns()");

            if (table.m_Grid.Debug)
                table.m_Grid.m_DebugString.AppendFormat("<b>OleDB.LoadColumns</b> - {0}<br/>", table.DataSourceId);

            string sql = String.Format("SELECT * FROM [{0}]", table.DataSourceId);

            if (table == table.m_Grid.MasterTable && string.IsNullOrEmpty(table.m_Grid.Sql) == false)
                sql = table.m_Grid.Sql;

            Query q = OleDBQuery.ExecuteReader(sql, table.ConnectionString, CommandBehavior.KeyInfo);
            LoadColumnSettings(q.DataReader.GetSchemaTable(), table);
            q.Close();
        }

        /// <summary>
        /// Indicates if the database-design structure for this table is valid. If not it removes cache.
        /// </summary>
        public override bool ValidCacheStructure(Table table)
        {
            string cacheId = string.Format("{0}_dtm", table.CacheKey);
            if (HttpRuntime.Cache.Get(cacheId) == null)
                return true;
            if (table.m_Grid.ValidPageHashCode == false)
            {
                HttpRuntime.Cache.Remove(cacheId);
                HttpRuntime.Cache.Remove(cacheId.Replace("_dtm", string.Empty));
                if (table.m_Grid.Debug)
                    table.m_Grid.m_DebugString.AppendFormat(
                        "<b>Cache</b> - (Invalid Page hash code) Removed datasource information from cache: {0}_{1}_{2}<br/>",
                        table.m_Grid.Trace.ClientID, table.m_Grid.ClientID, table.DataSourceId);
                if (table.m_Grid.Trace.IsTracing)
                    table.m_Grid.Trace.Trace(
                        "{0} : (Invalid Page hash code) Removed datasource information from cache: {1}", table.m_Grid.ID,
                        table.CacheKey);
                return false;
            }

            if (table.DataSourceType == DataSourceControlType.EnumerableDataSource || table.DataSourceType == DataSourceControlType.Undefined)
                return true;

            OleDbConnection conn = new OleDbConnection(table.m_Grid.ActiveConnectionString);
            conn.Open();
            DataTable schema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            conn.Close();

            object realtime = null;
            if (schema.Rows.Count > 0)
                realtime = schema.Select(null, "DATE_MODIFIED DESC")[0]["DATE_MODIFIED"];
            conn.Close();

            object cache = HttpRuntime.Cache.Get(cacheId);

            if (realtime is DateTime == false || cache is long == false)
                return false;
            if (((DateTime) realtime).Ticks == (long)cache)
                return true;

            HttpRuntime.Cache.Remove(cacheId);
            HttpRuntime.Cache.Remove(cacheId.Replace("_dtm", string.Empty));
            if (table.m_Grid.Debug)
                table.m_Grid.m_DebugString.AppendFormat(
                    "<b>Cache</b> - (Invalid Database settings) Removed datasource information from cache: {0}<br/>",
                    table.CacheKey);
            if (table.m_Grid.Trace.IsTracing)
                table.m_Grid.Trace.Trace(
                    "{0} : (Invalid Database settings) Removed datasource information from cache: {1}",
                    table.CacheKey);
            return false;
        }

        private static void LoadForeignKeys(Table table)
        {
            if (table.DataSourceId == null)
                return;
            //return;
            if (table.m_Grid.Trace.IsTracing) table.m_Grid.Trace.Trace("LoadForeignKeys()");

            OleDbConnection conn = new OleDbConnection(table.ConnectionString);
            conn.Open();

            DataTable fks = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys,
                                                     new Object[] { null, null, null, null });

            conn.Close();

            if (table.m_Grid.Debug)
                table.m_Grid.m_DebugString.Append("<b>OleDB.LoadForeignKeys</b><br/>");
            for (int i = 0; i < fks.Rows.Count; i++)
            {
                string fkDataSource = fks.Rows[i]["FK_TABLE_NAME"] as string;
                string fkColumnId = fks.Rows[i]["FK_COLUMN_NAME"] as string;

                if (String.Compare(fkDataSource, table.DataSourceId, true) != 0)
                    continue;

                // is there any column with this name?

                Foreignkey column = new Foreignkey(fkColumnId, table);

                if (table.Columns.GetIndex(fkColumnId) > -1)
                {
                    if (table.Columns[fkColumnId].IsCreatedByWebGrid == false &&
                        table.Columns[fkColumnId].ColumnType != ColumnType.Foreignkey)
                        continue;

                    Column oldcolumn = table.Columns[fkColumnId];
                    table.Columns[fkColumnId] = oldcolumn.CopyTo(column);
                    table.Columns[fkColumnId].m_ColumnType = ColumnType.Foreignkey;

                    table.Columns[fkColumnId].GridAlign = HorizontalPosition.Left;

                    if (table.Columns[fkColumnId].m_Searchable == null)
                        table.Columns[fkColumnId].m_Searchable = true;

                    if (table.Columns[fkColumnId].m_Sortable == null)
                        table.Columns[fkColumnId].m_Sortable = true;
                }
                else
                {
                    table.Columns.Add(column);
                    column.ColumnId = fkColumnId;
                }
                if (column.ConnectionString == null)
                    column.ConnectionString = table.ConnectionString;
                if (column.m_DataSourceId == null)
                    column.m_DataSourceId = fks.Rows[i]["PK_TABLE_NAME"] as string;
                column.m_SaveValueToViewState = true;
            }
        }

        #endregion Methods
    }
}