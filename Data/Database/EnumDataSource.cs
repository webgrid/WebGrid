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
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.OleDb;
    using System.Xml;

    using Design;
    using Enums;
    using Sort;

    internal static class EnumDataSource
    {
        #region Methods

        internal static DataTable ConvertDataViewToDataTable(DataView obDataView)
        {
            if (null == obDataView)
                throw new GridException("Invalid DataView object specified",null);

            DataTable obNewDt = obDataView.Table.Clone();
            int idx = 0;
            string[] strColNames = new string[obNewDt.Columns.Count];
            foreach (DataColumn col in obNewDt.Columns)
            {
                strColNames[idx++] = col.ColumnName;
            }

            IEnumerator viewEnumerator = obDataView.GetEnumerator();
            while (viewEnumerator.MoveNext())
            {
                DataRowView drv = (DataRowView)viewEnumerator.Current;
                DataRow dr = obNewDt.NewRow();
                try
                {
                    foreach (string strName in strColNames)
                    {
                        dr[strName] = drv[strName];
                    }
                }
                catch (Exception ex)
                {
                    throw new GridException("Error converting DataView to table.", ex);
                }
                obNewDt.Rows.Add(dr);
            }

            return obNewDt;
        }

        internal static void LoadDatasource(object datasource, DataTable columns, bool onlyColumns, Table table)
        {
            if (datasource is OleDbDataAdapter)
            {
                int startindex = 0;
                // initialize dataset object
                DataTable myData = new DataTable();
                DataTable tmpData = new DataTable();

                // fill with query results
                if (table == table.m_Grid.MasterTable && table.m_Grid.PageIndex != 1)
                {
                    startindex = (table.m_Grid.PageIndex - 1) * table.m_Grid.PageSize;
                    if (startindex < 0)
                        startindex = 0;
                }
                table.RecordCount = ((OleDbDataAdapter)datasource).Fill(tmpData);
                if (columns == null)
                {
                    columns = ((OleDbDataAdapter)datasource).FillSchema(tmpData, SchemaType.Source);
                    table.m_DataSourceColumns = columns;

                }
                tmpData.Dispose();
                if (onlyColumns == false)
                {
                    if (table != table.m_Grid.MasterTable || table.m_Grid.PageIndex == 0 || table.m_Grid.InternalId != null)
                        ((OleDbDataAdapter)datasource).Fill(myData);
                    else
                        ((OleDbDataAdapter)datasource).Fill(startindex, table.m_Grid.PageSize, myData);
                    datasource = myData;
                }
            }
            else if (datasource is XmlDataDocument)
            {
                if (table.m_XmlDataSet == null || ((XmlDataDocument)table.DataSource).DocumentElement == null)
                {
                    table.m_XmlDataSet = new DataSet();
                    table.m_XmlDataSet.ReadXml(table.m_XmlDataDocument, XmlReadMode.Auto);
                    table.DataSource = new XmlDataDocument(table.m_XmlDataSet);
                }
                XmlDataDocument xmlds = table.DataSource as XmlDataDocument;
                XmlNodeList nodeList = null;

                if (xmlds != null)
                    if (xmlds.DocumentElement != null)
                        nodeList = xmlds.DocumentElement.SelectNodes(table.m_Xmlxpath);

                int count = 0;
                if (nodeList != null)
                    foreach (XmlNode xmlNode in nodeList)
                    {
                        DataRow dataRow = xmlds.GetRowFromElement((XmlElement)xmlNode);
                        if (columns == null)
                        {
                            if (dataRow == null || dataRow.Table == null)
                            {
                                table.m_Grid.SystemMessage.Add(
                                    string.Format(
                                        "No DataRow was found from element {0} in xpath '{1}' in xml document '{2}'",
                                        xmlNode.Name, table.m_Xmlxpath, table.m_XmlDataDocument), true);
                                return;
                            }
                            columns = dataRow.Table;

                            table.m_DataSourceColumns = dataRow.Table;
                        }
                        if (onlyColumns)
                            break;
                        if (datasource is XmlDataDocument)
                            datasource = dataRow.Table.Clone();
                        if (dataRow == null)
                            continue;
                        count++;
                        ((DataTable)datasource).ImportRow(dataRow);

                    }
                if (count != 0)
                    table.RecordCount = count;
            }

            if (columns == null)
            {
                if (datasource is DataTable)
                {
                    Interface.LoadColumnSettings((DataTable)datasource, table);
                    table.m_DataSourceColumns = (DataTable)datasource;
                }
                else if (datasource is DataView)
                {
                    DataTable dataTable = ConvertDataViewToDataTable(datasource as DataView);
                    Interface.LoadColumnSettings(dataTable, table);
                    table.m_DataSourceColumns = dataTable;
                }
                else if (datasource is DataSet)
                {
                    if (string.IsNullOrEmpty(table.DataMember))
                        table.DataMember = ((DataSet)datasource).Tables[0].TableName;

                    foreach (DataRelation relation in ((DataSet)datasource).Relations)
                    {
                        if (relation.ParentColumns.Length != 1)
                            throw new GridException(
                                "WebGrid does not support DataSet with relations more then one column.");

                        string columnName = relation.ParentColumns[0].ColumnName;
                        if (table.Columns.Contains(columnName) && table.Columns[columnName].ColumnType != ColumnType.Foreignkey)
                            continue;

                        table.Columns[columnName].ColumnType = ColumnType.Foreignkey;
                        ((Foreignkey)table.Columns[columnName]).Table.DataSource = relation.ParentTable;
                        ((Foreignkey)table.Columns[columnName]).IdentityColumn = columnName;
                    }

                    Interface.LoadColumnSettings(((DataSet)datasource).Tables[table.DataMember], table);
                    table.m_DataSourceColumns = ((DataSet)datasource).Tables[table.DataMember];

                }
                else
                    throw new GridException(
                        string.Format(
                            "Columns definition must be type DataTable, DataView or DataSet. Type '{0}' was provided",
                            datasource.GetType()));

                foreach (Column column in table.Columns)
                    if (column.Identity)
                        table.m_DataSourceColumns.Columns[column.ColumnId].AutoIncrement = true;

            }
            else
                Interface.LoadColumnSettings(columns, table);

            if (onlyColumns)
                return;

            if (datasource is DataSet)
            {
                if (string.IsNullOrEmpty(table.DataMember))
                    table.DataMember = ((DataSet)datasource).Tables[0].TableName;
                ReadDataTable(((DataSet)datasource).Tables[table.DataMember], table);
            }
            else if (datasource is DataTable)
                ReadDataTable((DataTable)datasource, table);
            else if (datasource is DataView)
                ReadDataTable(ConvertDataViewToDataTable((DataView)datasource), table);
        }

        internal static void ReadDataTable(DataTable dataTable, Table table)
        {
            if (dataTable == null)
                return;
            SortColumn[] mylist = table.OrderBySortList.ColumnsSorted();

            string sort = table.GroupByExpression;
            foreach (SortColumn sortcolumn in mylist)
            {
                if (string.IsNullOrEmpty(sortcolumn.m_ColumnName))
                    continue;
                if (string.IsNullOrEmpty(sort) == false)
                    sort += ", ";
                sort += sortcolumn.m_ColumnName.Replace(".", string.Empty);
                sort += sortcolumn.m_Desc ? " DESC" : " ASC";
            }
            DataSourceControlType type = table.DataSourceType;
            table.DataSourceType = DataSourceControlType.EnumerableDataSource;
            string datasource = table.DataSourceId;
            table.DataSourceId = null;
            string where = Interface.BuildFilter(table, true);
            table.DataSourceType = type;
            table.DataSourceId = datasource;
            if (where.EndsWith("???", StringComparison.OrdinalIgnoreCase))
            {
                table.m_Grid.SystemMessage.Add("Your datasource is missing primary key(s) or identity key.", true);
                return;
            }
            if (where.StartsWith(" WHERE ", StringComparison.OrdinalIgnoreCase))
                where = where.Substring(7);
            DataRow[] datarows;
            try
            {
                if (string.IsNullOrEmpty(where) && string.IsNullOrEmpty(sort))
                {
                    List<DataRow> dtrows = new List<DataRow>();
                    foreach (DataRow row in dataTable.Rows)
                        dtrows.Add(row);
                    datarows = dtrows.ToArray();
                }
                else
                    datarows = dataTable.Select(where, sort);
            }
            catch (Exception ee)
            {

                throw new GridDataSourceException(string.Format("Failed to select from DataTable. (filterExpression: {0} sort:{1})", where, sort), ee);

            }
            if (!table.IsCustomPaging)
                table.RecordCount = datarows.Length;
            if (table == table.m_Grid.MasterTable && table.DataSource is OleDbDataAdapter == false)
                datarows = FilterDataRows(datarows, table.m_Grid);

            foreach (DataRow datarow in datarows)
            {
                if (datarow == null)
                    continue;
                Row newrow = new Row(table) { m_DataRow = datarow };
                foreach (DataColumn col in dataTable.Columns)
                {
                    if (table == table.m_Grid.MasterTable && table.DataSource is XmlDataDocument)
                        if (table.m_XmlDataSet.Tables[datarow.Table.TableName].Columns[col.ColumnName] == null)
                            newrow.Columns[col.ColumnName].AllowEdit = false;

                    newrow[col.ColumnName].Row = newrow;
                    newrow[col.ColumnName].Value = table.Columns[col.ColumnName].OnLoadFromDatabase(datarow[col]);
                    newrow[col.ColumnName].DataSourceValue = datarow[col];
                }

                foreach (Column column in table.Columns)
                {
                    if (column.ColumnType != ColumnType.ColumnTemplate)
                        continue;
                    newrow[column.ColumnId].Value = ((ColumnTemplate)column).CreateCellControls;
                }
                table.Rows.Add(newrow);
            }
        }

        private static DataRow[] FilterDataRows(DataRow[] datarows, Grid grid)
        {
            int startindex = 0;
            if (grid.PageIndex != 1 && grid.InternalId == null)
            {
                startindex = (grid.PageIndex - 1) * grid.PageSize;
                if (startindex < 0)
                    startindex = 0;
            }
            int pagelength = grid.PageSize;
            if (pagelength == 0 || datarows.Length < pagelength || grid.PageIndex == 0 || grid.MasterTable.IsCustomPaging)
                return datarows;
            if ((datarows.Length - 1) < (startindex + pagelength))
                pagelength = datarows.Length - startindex;
            DataRow[] tmpRows = new DataRow[pagelength];
            int counter = 0;
            while (counter < pagelength)
            {
                tmpRows[counter] = datarows[startindex + counter];
                counter++;
            }
            return tmpRows;
        }

        #endregion Methods
    }
}