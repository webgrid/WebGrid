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
    using System.Collections.Specialized;
    using System.Data;
    using System.Data.OleDb;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.IO;
    using System.Text;

    using WebGrid.Design;

    #region Enumerations

    /// <summary>
    /// Set or gets what type of action the class should do.
    /// </summary>
    public enum QueryType
    {
        /// <summary>
        /// Sets the action of this class to insert a new database record.
        /// </summary>
        Insert = 0,
        /// <summary>
        /// Sets the action of this class to update existing database record(s).
        /// </summary>
        /// <remarks>
        /// Use the property 'FilterExpression' to specifies the records to be updated.
        /// </remarks>
        Update = 1,
        /// <summary>
        /// Sets the action of this class to delete existing database records(s).
        /// </summary>
        /// <remarks>
        /// Use the property 'FilterExpression' to specifies the records to be updated.
        /// </remarks>
        Delete = 3,
    }

    #endregion Enumerations

    /// <summary>
    /// InsertUpdate class handles all database editing for <see cref="WebGrid.Grid">WebGrid.Grid</see> web control.
    /// This class contains methods and properties that helps and speeds up the process
    /// to update or insert a database record, it also contains basic SQL-injections security checks
    /// while updating or inserting a data record.
    /// </summary>
    public class InsertUpdate : IDisposable
    {
        #region Fields

        private readonly string m_DataTableName;

        private NameValueCollection m_Data;
        private bool m_Disposed;
        private bool m_IsMicrosoftSQLServer;
        private bool m_IsOleDbConnection;
        private OleDbCommand m_Oledbcmd;
        private OleDbConnection m_Oledbconn;
        private bool m_RetrieveId = true;
        private SqlCommand m_Sqlcmd;
        private SqlConnection m_Sqlconn;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// ColumnId of the table in the database you should insert a new row or update a existing row.
        /// </summary>
        /// <param name="tableName">ColumnId of the table.</param>
        public InsertUpdate(string tableName)
        {
            m_DataTableName = tableName;
            Setup();
        }

        /// <summary>
        /// ColumnId of the table in the database you should insert a new row or update a existing row.
        /// </summary>
        /// <param name="tableName">ColumnId of the table.</param>
        /// <param name="queryType">Indicates whether you should insert or update rows.</param>
        public InsertUpdate(string tableName, QueryType queryType)
        {
            QueryType = queryType;
            m_DataTableName = tableName;
            Setup();
        }

        /// <summary>
        /// ColumnId of the table in the database you should insert a new row or update a existing row.
        /// </summary>
        /// <param name="tableName">ColumnId of the table.</param>
        /// <param name="queryType">Indicates whether you should insert or update rows.</param>
        /// <param name="connectionString">A standard OleDB connection string.</param>
        public InsertUpdate(string tableName, QueryType queryType, string connectionString)
        {
            QueryType = queryType;
            ConnectionString = connectionString;
            m_DataTableName = tableName;
            Setup();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Set or gets a standard Oledb connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get; set;
        }

        /// <summary>
        /// Sets or gets a filter for the data source.
        /// </summary>
        /// <value>The SQL where statement</value>
        /// <remarks>
        /// "WHERE" should not be written. Only the statement.
        /// </remarks>
        public string FilterExpression
        {
            get; set;
        }

        /// <summary>
        /// Set type of action for this instance.
        /// </summary>
        /// <value>The type of the query.</value>
        public QueryType QueryType
        {
            get; set;
        }

        /// <summary>
        /// Sets or Gets whether a identifier for the record should be returned
        /// </summary>
        /// <value><c>true</c> if [retrieve ColumnId]; otherwise, <c>false</c>.</value>
        public bool RetrieveId
        {
            get { return m_RetrieveId; }
            set { m_RetrieveId = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Add a column you wish to update or insert into a table.
        /// </summary>
        /// <param name="column">ColumnId of the column</param>
        /// <param name="value">The value.</param>
        public void Add(string column, BinaryReader value)
        {
            if (value == null)
                throw new ApplicationException(string.Format("The value for this column ({0}) must have an object.", column));

            column = column.Replace("'", "''");
            if (m_Data[column] != null) throw new ApplicationException("A column with same name is already added.");

            m_Data.Add(column, string.Format("@file{0}", column.Replace(" ",string.Empty)));
            if (m_IsOleDbConnection == false)
            {
                m_Sqlcmd.Parameters.Add(string.Format("@file{0}", column.Replace(" ",string.Empty)), SqlDbType.Image);
                m_Sqlcmd.Parameters[string.Format("@file{0}", column.Replace(" ",string.Empty))].Value =
                    value.ReadBytes((int) value.BaseStream.Length);
            }
            else
            {
                m_Oledbcmd.Parameters.AddWithValue(string.Format("@file{0}", column.Replace(" ",string.Empty)), SqlDbType.Image);
                m_Oledbcmd.Parameters[string.Format("@file{0}", column.Replace(" ",string.Empty))].Value =
                    value.ReadBytes((int) value.BaseStream.Length);
            }
        }

        /// <summary>
        /// Add a column you wish to update or insert into a table.
        /// </summary>
        /// <param name="column">ColumnId of the column.</param>
        /// <param name="value">The value.</param>
        public void Add(string column, bool value)
        {
            column = column.Replace("'", "''").Replace(",", ".");
            if (m_Data[column] != null) throw new ApplicationException("A column with same name is already added.");
            m_Data.Add(column, value ? "1" : "0");
        }

        /// <summary>
        /// Add a column you wish to update or insert into a table.
        /// </summary>
        /// <param name="column">ColumnId of the column.</param>
        /// <param name="value">The value.</param>
        public void Add(string column, double value)
        {
            column = column.Replace("'", "''").Replace(",", ".");
            if (m_Data[column] != null) throw new ApplicationException("A column with same name is already added.");
            string myvalue = Grid.m_Whitespaceregexp.Replace(value.ToString(),string.Empty);
            m_Data.Add(column, myvalue.Replace(",", "."));
        }

        /// <summary>
        /// Add a column you wish to update or insert into a table.
        /// </summary>
        /// <param name="column">ColumnId of the column.</param>
        /// <param name="value">The value.</param>
        public void Add(string column, float value)
        {
            column = column.Replace("'", "''").Replace(",", ".");
            if (m_Data[column] != null)
                throw new ApplicationException("A column with same name is already added.");

            Add(column, Convert.ToDouble(value));
        }

        /// <summary>
        /// Add a column you wish to update or insert into a table.
        /// </summary>
        /// <param name="column">ColumnId of the column.</param>
        /// <param name="value">The value.</param>
        public void Add(string column, int value)
        {
            column = column.Replace("'", "''").Replace(",", ".");
            if (m_Data[column] != null) throw new ApplicationException("A column with same name is already added.");
            m_Data.Add(column, value.ToString());
        }

        /// <summary>
        /// Add a column you wish to update or insert into a table.
        /// </summary>
        /// <param name="column">ColumnId of the column.</param>
        /// <param name="value">The value.</param>
        public void Add(string column, long value)
        {
            column = column.Replace("'", "''").Replace(",", ".");
            if (m_Data[column] != null) throw new ApplicationException("A column with same name is already added.");
            m_Data.Add(column, value.ToString());
        }

        /// <summary>
        /// Add a column you wish to update or insert into a table.
        /// This function will automatically detect datatype from a object <see cref="WebGrid.Util.Query" /> element provided as argument
        /// and will do the necessary data-converting before inserting or updating a record into the database.
        /// </summary>
        /// <param name="column">ColumnId of the column.</param>
        /// <param name="value">The value as an object. The only valid objects are WebGrid.Util.Query[column]</param>
        public void Add(string column, object value)
        {
            string type = value == null ? "system.dbnull" : value.GetType().ToString().ToLowerInvariant();

            switch (type)
            {
                case "system.dbnull":
                case "system.empty":
                    Add(column, Grid.NULLCONSTANT);
                    break;
                case "system.boolean":
                    if (value != null) Add(column, Boolean.Parse(value.ToString()));
                    break;
                case "system.datetime":
                    if (value != null) Add(column, System.DateTime.Parse(value.ToString()));
                    break;
                case "system.single":
                case "system.int64":
                case "system.double":
                case "system.float":
                    if (value != null) Add(column, double.Parse(value.ToString()));
                    break;
                case "system.int32":
                case "system.int16":
                    if (value != null) Add(column, int.Parse(value.ToString()));
                    break;
                case "system.char":
                case "system.string":
                    if (value != null) Add(column, value.ToString());
                    break;
                case "system.byte[]":

                    Stream tmp = Stream.Null;
                    byte[] myvalue = (byte[]) value;
                    if (myvalue != null) tmp.Read(myvalue, 0, myvalue.Length);
                    BinaryReader image = new BinaryReader(tmp);

                    Add(column, image);
                    break;
                default:
                    if (value != null) Add(column, value.ToString());
                    break;
            }
        }

        /// <summary>
        /// Add a column you wish to update or insert into a table.
        /// </summary>
        /// <param name="column">ColumnId of the column.</param>
        /// <param name="value">The value.</param>
        public void Add(string column, string value)
        {
            if (string.IsNullOrEmpty(column))
                throw new GridException("Column ID is null or empty");
            // Inserting a float...
            if (string.IsNullOrEmpty(value) == false && value.IndexOf(",") != -1 && Validate.IsFloat(value))
            {
                Add(column, float.Parse(value));
                return;
            }

            column = column.Replace("'", "''").Replace(",", ".");

            if (m_Data[column] != null)
                throw new ApplicationException("A column with same name is already added.");

            value = value == null ? Grid.NULLCONSTANT : value.Replace("'", "''");

            // SQL function
            m_Data.Add(column, value.EndsWith("()") ? value : string.Format("'{0}'", value));
        }

        /// <summary>
        /// Add a column you wish to update or insert into a table.
        /// </summary>
        /// <param name="column">ColumnId of the column.</param>
        /// <param name="value">The value as a datetime. Cannot be null, use Add(string,string) to insert null values.</param>
        public void Add(string column, System.DateTime value)
        {
            if (column == null)
            {
                throw (new GridException(
                    "Add(string column, DateTime value) Cannot be null, use Add(string,string) to insert null values."));
            }
            column = column.Replace("'", "''").Replace(",", ".");
            if (m_Data[column] != null) throw new ApplicationException("A column with same name is already added.");
            m_Data.Add(column,
                       m_IsMicrosoftSQLServer
                           ? string.Format("'{0}'", value.ToString("yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture))
                           : string.Format("'{0}'", value.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)));
        }

        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // Unregister object for finalization.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This executes the sql statements and tries to update the row or insert a new row into the database.
        /// </summary>
        /// <returns>The row identifier.</returns>
        public string Execute()
        {
            return Execute(QueryType);
        }

        /// <summary>
        /// This executes the sql statements and tries to update the row or insert a new row into the database.
        /// </summary>
        /// <param name="queryTypes">The querytype.</param>
        /// <returns>The row identifier.</returns>
        /// <remarks>
        /// If you update a row then will the function return -1.
        /// </remarks>
        public string Execute(QueryType queryTypes)
        {
            return ExecuteSqlStatement(queryTypes);
        }

        /// <summary>
        /// Returns the the SQL generated by this instance.
        /// </summary>
        public string GenerateSql()
        {
            return GenerateSql(QueryType);
        }

        /// <summary>
        /// Generates SQL
        /// </summary>
        /// <param name="querytype">The action you want to perform.</param>
        /// <returns>The status</returns>
        public string GenerateSql(QueryType querytype)
        {
            if (string.IsNullOrEmpty(m_DataTableName))
                throw new ApplicationException("TableName was not provided.");
            switch (querytype)
            {
                case QueryType.Insert:
                    return BuildInsert();
                case QueryType.Update:
                    return BuildUpdate();
                case QueryType.Delete:
                    return BuildDelete();
            }
            return null;
        }

        /// <summary>
        /// Gets the sql statements used to insert or update a row to the database.
        /// </summary>
        /// <returns>
        /// The sql statements used to insert or update a row in the database.
        /// </returns>
        public string GetQueryString()
        {
            return GetQueryString(QueryType);
        }

        /// <summary>
        /// Gets the sql statements used to insert or update a row to the database.
        /// </summary>
        /// <param name="querytype">Indicates whether you should return sql statements for insert or update.</param>
        /// <returns>
        /// The sql statements used to insert or update a row in the database.
        /// </returns>
        /// <remarks>This is useful for debugging if the grid fails to insert/update.
        /// </remarks>
        public string GetQueryString(QueryType querytype)
        {
            switch (querytype)
            {
                case QueryType.Insert:
                    return BuildInsert();
                case QueryType.Update:
                    return BuildUpdate();
                case QueryType.Delete:
                    return BuildDelete();
            }
            return null;
        }

        /// <summary>
        /// Inserts this instance.
        /// </summary>
        public string Insert()
        {
            return ExecuteSqlStatement(QueryType.Insert);
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        public string Update()
        {
            return ExecuteSqlStatement(QueryType.Update);
        }

        /// <summary>
        /// This executes the sql statements and tries to insert a data.
        /// </summary>
        /// <returns>The row identifier from the database.</returns>
        internal string ExecuteSqlStatement(QueryType actionType)
        {
            string res = null;

            if (m_IsOleDbConnection == false)
            {
                m_Sqlcmd.CommandText = GetQueryString(actionType);

                m_Sqlcmd.Connection.Open();
                if (RetrieveId)
                    res = m_Sqlcmd.ExecuteScalar().ToString();
                else
                    m_Sqlcmd.ExecuteNonQuery();
                m_Sqlcmd.Connection.Close();
            }
            else
            {

                m_Oledbcmd.CommandText = GetQueryString(actionType);
                m_Oledbcmd.Connection.Open();
                m_Oledbcmd.ExecuteNonQuery();
                if (RetrieveId)
                {
                    m_Oledbcmd.CommandText = "Select @@IDENTITY";
                    res = m_Oledbcmd.ExecuteScalar().ToString();
                }
                m_Oledbcmd.Connection.Close();
            }
            return res;
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (m_Disposed) return;
            // If disposing equals true, dispose all managed
            // and unmanaged resources.
            if (disposing)
            {
                m_Sqlconn.Dispose();
                m_Oledbconn.Dispose();
                m_Sqlcmd.Dispose();
                m_Oledbcmd.Dispose();
            }

            m_Disposed = true;
        }

        private string BuildDelete()
        {
            RetrieveId = false;
            StringBuilder s = new StringBuilder();

            s.AppendFormat("DELETE FROM [{0}]", m_DataTableName);
            s.AppendFormat(" WHERE {0}", FilterExpression);
            return s.ToString();
        }

        private string BuildInsert()
        {
            bool first = true;
            StringBuilder s = new StringBuilder();
            if (RetrieveId && m_IsOleDbConnection == false)
                s.Append("SET NOCOUNT ON ");

            s.AppendFormat("INSERT INTO {0}(", m_DataTableName);

            foreach (string col in m_Data)
            {
                if (!first) s.Append(", ");
                s.Append(col);
                first = false;
            }

            s.Append(") VALUES(");
            first = true;
            foreach (string col in m_Data)
            {
                if (!first) s.Append(", ");
                s.Append(String.Compare(m_Data[col], "'NULL'", true) != 0 ? m_Data[col] : Grid.NULLCONSTANT);
                first = false;
            }
            s.Append(")");
            if (RetrieveId && m_IsOleDbConnection == false)
                s.Append(" SELECT @@IDENTITY SET NOCOUNT OFF");

            return s.ToString();
        }

        private string BuildUpdate()
        {
            bool first = true;
            RetrieveId = false;
            StringBuilder s = new StringBuilder();
            s.AppendFormat("UPDATE {0} SET ", m_DataTableName);
            foreach (string col in m_Data)
            {
                if (!first) s.Append(", ");
                if (String.Compare(m_Data[col], "'NULL'", true) != 0)
                    s.AppendFormat("[{0}] = {1}", col, m_Data[col]);
                else
                    s.AppendFormat("[{0}] = NULL", col);
                first = false;
            }
            s.AppendFormat(" WHERE {0}", FilterExpression);
            return s.ToString();
        }

        private void Setup()
        {
            m_Data = new NameValueCollection();

            if (Util.ConnectionString.FindConnectionString(ConnectionString) != null)
                ConnectionString = Util.ConnectionString.FindConnectionString(ConnectionString);

            if (ConnectionString == null)
                ConnectionString = Util.ConnectionString.FindConnectionString(null);

            DataBaseConnectionType contype = ConnectionType.FindConnectionType(ConnectionString);

            if (contype == DataBaseConnectionType.SqlConnection)
            {
                m_Sqlconn = new SqlConnection(ConnectionString);
                m_Sqlcmd = new SqlCommand {Connection = m_Sqlconn};
                m_IsMicrosoftSQLServer = true;
            }
            else
            {
                if (ConnectionString.IndexOf("Provider=sqloledb", StringComparison.OrdinalIgnoreCase) > -1 ||
                    ConnectionString.IndexOf("Provider=SQLNCLI", StringComparison.OrdinalIgnoreCase) > -1)
                    m_IsMicrosoftSQLServer = true;
                m_IsOleDbConnection = true;
                m_Oledbconn = new OleDbConnection(ConnectionString);
                m_Oledbcmd = new OleDbCommand {Connection = m_Oledbconn};
            }
        }

        #endregion Methods
    }

    /// <summary>
    /// This namespace contains public classes used by <see cref="WebGrid.Grid">WebGrid.Grid</see> web control.
    /// These classes are typical tool classes to edit,create, validate, retrieve data...
    /// from a data source.
    /// </summary>
    internal class NamespaceDoc
    {
    }
}