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
    using System.Text;
    using System.Web;

    using Design;
    using Enums;
    using Util;

    /// <summary>
    /// Provides communication between an MS SQL Server database and WebGrid.
    /// </summary>
    internal class SqlConnection : Interface
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
        /// Loads one or more rows from the SQL Server database into the specified Table.
        /// </summary>
        /// <param name="table">The Table object to populate with rows.</param>
        public override void GetData(Table table)
        {
            if (table.m_Grid.SystemMessage.CriticalCount > 0 || (table.DataSourceId == null && table.Sql == null))
                return;

            string sql = table.Sql;
            if (sql == null)
            {
                if (table.m_Grid.Trace.IsTracing) table.m_Grid.Trace.Trace("Start SqlConnection-GetData({0});", table.DataSourceId);
                sql = SqlBuilder(table);
                if (table.m_Grid.Trace.IsTracing) table.m_Grid.Trace.Trace("Finish SqlConnection-GetData({0});", table.DataSourceId);
            }

            // FAEN FAEN NY HACK..
             //   if (sql.StartsWith("SELECT FROM", StringComparison.OrdinalIgnoreCase))
              //      sql = sql.Replace("SELECT FROM", "SELECT * FROM");

            if (table.m_Grid.Debug)
                table.m_Grid.m_DebugString.AppendFormat("<b>{0}: SqlConnection.GetData({1})</b> - {2}<br/>", table.m_Grid.ID,
                                                        table.DataSourceId, sql);

            Query q = null;

            try
            {
                q = table.Sql == null ? SqlConnectionQuery.ExecuteReader(sql, table.ConnectionString) : SqlConnectionQuery.ExecuteReader(sql, table.ConnectionString, CommandBehavior.KeyInfo);
            }
            catch (Exception ee)
            {
                if (q != null)
                    q.Close();
                throw new GridException("Error retrieving data from SqlConnection data interface", ee);
            }
            if (table.Sql != null)
                LoadColumnSettings(q.DataReader.GetSchemaTable(), table);

            // Read past first TOP records for paging
            // Do not do this if we have a tree. Then manual paging in design render
            if (string.IsNullOrEmpty(table.m_Grid.TreeParentId) && table == table.m_Grid.MasterTable &&
                table.m_Grid.PageSize > 0 &&
                table.m_Grid.PageIndex > 1 && table.m_Grid.DisplayView == DisplayView.Grid)
                //if(table.Parent.PageLength != 0 && (table.Parent.TreeParentId == null || table == table.Parent.MasterTable ) && table.Parent.PageIndex > 1 && table.Parent.Mode == Modes.Grid)

                for (int i = 0; i < (table.m_Grid.PageIndex - 1)*table.m_Grid.PageSize; i++)
                    q.Read();

            if (table.m_Grid.Trace.IsTracing) table.m_Grid.Trace.Trace("SQL: {0} : {1}", table.DataSourceId, sql);

            // Predefining rows and columns
            int tablecolumns = table.Columns.Count;
               List<int> skipcolumns = new List<int>();
               try
               {
               while (q.Read())
               {
                   Row r = new Row(table);
                   for (int i = 0; i < tablecolumns; i++)
                   {
                       Column tablecolumn = table.Columns[i];
                       int columnIndex = i;

                       RowCell c = new RowCell(tablecolumn.ColumnId, r);
                       if (tablecolumn.ColumnType == ColumnType.ColumnTemplate)
                           c.Value = ((ColumnTemplate)tablecolumn).CreateCellControls;

                       c.Row = r;
                       if (tablecolumn.IsInDataSource)
                           c.Value = null;
                       r.Cells.Add(c);

                       if (skipcolumns.Contains(columnIndex.GetHashCode()))
                           continue;
                       if ((tablecolumn.IsInDataSource == false || tablecolumn.ColumnType == ColumnType.SystemColumn) ||
                           (tablecolumn.IsBlob/* && tablecolumn.FileNameColumn != tablecolumn.ColumnId*/))
                       {
                           skipcolumns.Add(columnIndex.GetHashCode());
                           continue;
                       }

                       int dsindex = q.m_DataReader.GetOrdinal(tablecolumn.ColumnId);

                       if (q[dsindex] == null)
                       {
                           skipcolumns.Add(columnIndex.GetHashCode());
                           continue;
                       }
                        c.DataSourceValue = q.IsDBNull(dsindex) ? tablecolumn.OnLoadFromDatabase(null) : tablecolumn.OnLoadFromDatabase(q[dsindex]);

                       c.Value = c.DataSourceValue;

                       // FILES IN DATABASE SHOULD NOT BE LOADED HERE. -Olav
                       // IS ONLY ACTIVE IF FILENAMECOLUMN IS ACTIVE.

                       if (tablecolumn.ColumnType == ColumnType.Text && ((Text)tablecolumn).EncryptionKey != null &&
                           c.DataSourceValue != null)
                       {
                           // Decrypt
                           c.DataSourceValue =
                               Security.Decrypt(c.DataSourceValue.ToString(), ((Text)tablecolumn).EncryptionKey);
                           c.Value = c.DataSourceValue;
                       }

                       // Foreign key stuff
                       if (tablecolumn.ColumnType != ColumnType.Foreignkey || string.IsNullOrEmpty(tablecolumn.DataSourceId))
                           continue;
                       if (!string.IsNullOrEmpty(table.m_Grid.Sql) || ((Foreignkey)tablecolumn).Table.DataSource != null)
                           continue;
                       c.DataSourceDisplayValue = q[string.Format("_fk_{0}_text", c.CellId)];
                       c.DisplayValue = c.DataSourceDisplayValue;
                   }
                   table.Rows.Add(r);
               }
               }
               catch (Exception ee)
               {
               q.Close();
               throw new GridException("Error populating grid with data from SqlConnection data interface"+sql, ee);
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
            q.Close();
        }

        /// <summary>
        /// Gets the schema for the table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="retrieveForeignkeys">Whether foreignkeys also should be loaded.</param>
        public override void GetSchema(Table table, bool retrieveForeignkeys)
        {
            if (string.IsNullOrEmpty(table.Sql) == false)
                GetData(table);
            else
            {
                LoadColumns(table);
                if (table.m_Grid.SystemMessage.CriticalCount == 0 && retrieveForeignkeys)
                    LoadForeignKeys(table);
                if (table.m_Grid.Trace.IsTracing && table.m_Grid.Trace.IsTracing)
                    table.m_Grid.Trace.Trace(string.Format("SqlConnection - GetSchema from Grid: {0} DataSource: {1}",
                                                           table.m_Grid.ID, table.DataSourceId));
            }
        }

        public override void LoadColumns(Table table)
        {
            // Halt Loadcolumns if there are critical errors.

            if (table.m_LoadedTableSchema || string.IsNullOrEmpty(table.DataSourceId) ||
                table.m_Grid.SystemMessage.CriticalCount > 0)
                return;
            if (table.m_Grid.Trace.IsTracing)
                table.m_Grid.Trace.Trace("{0}: LoadColumns( {1})", table.m_Grid.ID, table.DataSourceId);

            string sql = String.Format("SELECT * FROM [{0}]", table.DataSourceId);

            if (table == table.m_Grid.MasterTable && string.IsNullOrEmpty(table.m_Grid.Sql) == false)
                sql = table.m_Grid.Sql;

            Query q = SqlConnectionQuery.ExecuteReader(sql, table.ConnectionString, CommandBehavior.KeyInfo);
            LoadColumnSettings(q.DataReader.GetSchemaTable(), table);
            q.Close();
        }

        /// <summary>
        /// Indicates if the database-design structure for this table is valid.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public override bool ValidCacheStructure(Table table)
        {
            string cacheId =
                string.Format("{0}_dtm", table.CacheKey);

            if (HttpRuntime.Cache.Get(cacheId) == null)
                return true;

            if (table.m_Grid.ValidPageHashCode == false || !table.m_Grid.CacheGridStructure)
            {
                HttpRuntime.Cache.Remove(cacheId);
                HttpRuntime.Cache.Remove(table.CacheKey);
                if (table.m_Grid.Debug)
                    table.m_Grid.m_DebugString.AppendFormat(
                        "<b>Cache</b> - (Invalid Page hash code) Removed datasource information from cache: {0}_{1}_{2}<br/>",
                        table.m_Grid.Trace.ClientID, table.m_Grid.ID, table.DataSourceId);
                if (table.m_Grid.Trace.IsTracing)
                {

                    int registeredHashcode = (int)HttpRuntime.Cache.Get(table.m_Grid.Trace.ClientID + table.m_Grid.ClientID);
                    int pageHash = table.m_Grid.Page.GetType().GetHashCode();
                    if (table.m_Grid.Trace.IsTracing)
                        table.m_Grid.Trace.Trace(
                        "{0} : (Invalid Page hash code) Removed datasource information from cache: {1}_{2}_{3} ('{4}' vs '{5}')",
                        table.m_Grid.ClientID, table.m_Grid.Trace.ClientID, table.m_Grid.ID, table.DataSourceId, registeredHashcode, pageHash);
                }
                return false;
            }
            object cache = HttpRuntime.Cache.Get(cacheId);

            if (table.DataSourceType == DataSourceControlType.EnumerableDataSource || table.DataSourceType == DataSourceControlType.Undefined)
                return true;
             object date =Query.ExecuteScalar("SELECT TOP 1 refdate FROM sysobjects ORDER BY refdate DESC", table.m_Grid.ActiveConnectionString);

             if (date is DateTime == false || cache is long == false)
                 return false;
             if (((DateTime)date).Ticks == (long)cache)
                 return true;
            HttpRuntime.Cache.Remove(cacheId);
            HttpRuntime.Cache.Remove(cacheId.Replace("_dtm", string.Empty));
            if (table.m_Grid.Debug)
                table.m_Grid.m_DebugString.AppendFormat(
                    "<b>Cache</b> - (Invalid Database settings) Removed datasource information from cache: {0}<br/>",
                    table.CacheKey);
            if (table.m_Grid.Trace.IsTracing)
                table.m_Grid.Trace.Trace(
                    "{0} : (Invalid Database settings) Removed datasource information from cache: {1}}",
                    table.CacheKey);
            return false;
        }

        private static void LoadForeignKeys(Table table)
        {
            string foreignkeydatatable = table.DataSourceId;

            if (string.IsNullOrEmpty(foreignkeydatatable) ||
                table.m_Grid.SystemMessage.CriticalCount != 0)
            {
                if (table.m_Grid.Debug)
                    table.m_Grid.m_DebugString.AppendFormat("<b>SqlConnection.SkippingForeignkeyLoad({0})", table);
                return;
            }
            if (table.m_Grid.Trace.IsTracing) table.m_Grid.Trace.Trace("LoadForeignKeys()");
            StringBuilder sql = new StringBuilder(string.Empty);
            sql.Append("SELECT SO1.name AS Tab,  ");
            sql.Append("       SC1.name AS Col,  ");
            sql.Append("       SO2.name AS RefTab,  ");
            sql.Append("       SC2.name AS RefCol,  ");
            sql.Append("       FO.name AS FKName ");
            sql.Append("FROM dbo.sysforeignkeys FK   ");
            sql.Append("INNER JOIN dbo.syscolumns SC1 ON FK.fkeyid = SC1.id  ");
            sql.Append("                              AND FK.fkey = SC1.colid  ");
            sql.Append("INNER JOIN dbo.syscolumns SC2 ON FK.rkeyid = SC2.id  ");
            sql.Append("                              AND FK.rkey = SC2.colid  ");
            sql.Append("INNER JOIN dbo.sysobjects SO1 ON FK.fkeyid = SO1.id  ");
            sql.Append("INNER JOIN dbo.sysobjects SO2 ON FK.rkeyid = SO2.id  ");
            sql.Append("INNER JOIN dbo.sysobjects FO ON FK.constid = FO.id ");
            sql.AppendFormat("WHERE SO1.Name = '{0}'", foreignkeydatatable);

            if (foreignkeydatatable.IndexOf("[", StringComparison.OrdinalIgnoreCase) != -1)
            {
                string database = foreignkeydatatable.Substring(0, foreignkeydatatable.IndexOf("."));
                string datatable = foreignkeydatatable.Replace(string.Format("{0}.dbo.", database), string.Empty).Replace("[", string.Empty).Replace("]", string.Empty);
                sql = new StringBuilder(
                    @"SELECT SO1.name AS Tab, SC1.name AS Col, SO2.name AS RefTab, SC2.name AS RefCol, FO.name AS FKName
                    FROM         [XXX].sysforeignkeys FK INNER JOIN
                      [XXX].syscolumns SC1 ON FK.fkeyid = SC1.id AND FK.fkey = SC1.colid INNER JOIN
                      [XXX].syscolumns SC2 ON FK.rkeyid = SC2.id AND FK.rkey = SC2.colid INNER JOIN
                      [XXX].sysobjects SO1 ON FK.fkeyid = SO1.id INNER JOIN
                      [XXX].sysobjects SO2 ON FK.rkeyid = SO2.id INNER JOIN
                      [XXX].sysobjects FO ON FK.constid = FO.id ");
                sql = sql.Replace("[XXX]", string.Format("{0}.dbo", database));

                sql.AppendFormat("WHERE SO1.Name = '{0}'", datatable);
            }

            if (table.m_Grid.Debug)
                table.m_Grid.m_DebugString.AppendFormat("<b>{0}: SqlConnection.LoadForeignKeys({1})</b> - {2}<br/>",
                                                        table.m_Grid.ID, foreignkeydatatable, sql);
            if (table.m_Grid.Trace.IsTracing)
                table.m_Grid.Trace.Trace(
                            "{0}: SqlConnection.LoadForeignKeys({1})</b> - {2}<br/>",
                                                        table.m_Grid.ID, foreignkeydatatable, sql);
            Query q = SqlConnectionQuery.ExecuteReader(sql.ToString(), table.ConnectionString);
            //Util.Query q = new Util.Query(sql,table.ConnectionString);
            while (q.Read())
            {
                string col = q["Col"].ToString();
                // is there any column with this name?
                Foreignkey column = new Foreignkey(col, table);
                if (table.Columns.GetIndex(col) > -1)
                {
                    if (table.Columns[col].IsCreatedByWebGrid == false
                        && table.Columns[col].ColumnType != ColumnType.Foreignkey)
                        continue;

                    Column oldcolumn = table.Columns[col];
                    table.Columns[col] = oldcolumn.CopyTo(column);
                    table.m_Columns[col].m_ColumnType = ColumnType.Foreignkey;
                    table.Columns[col].GridAlign = HorizontalPosition.Left;

                    if (table.Columns[col].m_Searchable == null )
                        table.Columns[col].m_Searchable = true ;

                    if (table.Columns[col].m_Sortable == null )
                        table.Columns[col].m_Sortable = true ;
                }
                else
                {
                    // else new column
                    table.Columns.Add(column);
                    column.ColumnId = col;
                }

                if (column.ConnectionString == null)
                    column.ConnectionString = table.ConnectionString;
                if (column.m_DataSourceId == null)
                    column.m_DataSourceId = q["reftab"] as string;
                column.m_SaveValueToViewState = true;
                column.GridAlign = HorizontalPosition.Left;
            }
            q.Close();
        }

        #endregion Methods
    }
}