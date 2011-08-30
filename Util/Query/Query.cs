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
    using System.Web.UI.WebControls;

    using WebGrid.Design;

    /// <summary>
    /// This class contains methods and properties to execute or retrieve 
    /// database records from a data source. This class is should primary be used to retrieve
    /// database information. For security and application stability use <see cref="WebGrid.Util.InsertUpdate"/>
    /// to update or insert a database record.
    /// </summary>
    public abstract class Query
    {
        #region Fields

        internal IDbConnection m_Connection;

        /*		internal IDataParameterCollection _parameters;

                /// <summary>
                /// CREATE SOME DAY...
                /// </summary>
                /// <value>The data reader object</value>
                private IDataParameterCollection Parameters
                {
                    get { return _parameters; }

                    set { _parameters = value; }
                }
        */
        internal IDataReader m_DataReader;

        private static readonly object m_CommandTimeoutLock = new object();

        private static int m_CommandTimeout = 180;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Sets the timeout in seconds before a sql command fails.
        /// Default is 180 seconds.
        /// </summary>
        public static int CommandTimeout
        {
            get
            {
                lock (m_CommandTimeoutLock)
                {
                    return m_CommandTimeout;
                }
            }
            set
            {
                lock (m_CommandTimeoutLock)
                {
                    m_CommandTimeout = value;
                }
            }
        }

        /// <summary>
        /// Gets the data reader object for this instance.
        /// </summary>
        /// <value>The data reader object</value>
        public IDataReader DataReader
        {
            get { return m_DataReader; }

            set { m_DataReader = value; }
        }

        internal IDbConnection Connection
        {
            get { return m_Connection; }
        }

        #endregion Properties

        #region Indexers

        /// <summary>
        /// Gets data from a data source by the index value.
        /// </summary>
        /// <value>(field) data reader object.</value>
        public object this[int columnIndex]
        {
            get { return m_DataReader[columnIndex]; }
        }

        /// <summary>
        /// Gets data from a data source by the column name.
        /// </summary>
        /// <value>(field) data reader object.</value>
        public object this[string columnName]
        {
            get { return m_DataReader[columnName]; }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Executes a query and binds it to a ListControl.
        /// </summary>
        /// <param name="listControl">The list control you wish to use. For example a dropdown box.</param>
        /// <param name="sql">Your sql query string</param>
        public static void ExecuteAndBind(ListControl listControl, string sql)
        {
            ExecuteAndBind(listControl, null, null, null, sql, null, DataBaseConnectionType.Auto);
        }

        /// <summary>
        /// Executes a query and binds it to a ListControl.
        /// </summary>
        /// <param name="listControl">The list control you wish to use. For example a dropdown box.</param>
        /// <param name="sql">Your sql query string</param>
        /// <param name="connectionString">Your database connection string.</param>
        public static void ExecuteAndBind(ListControl listControl, string sql, string connectionString)
        {
            ExecuteAndBind(listControl, null, null, null, sql, connectionString, DataBaseConnectionType.Auto);
        }

        /// <summary>
        /// Executes a query and binds it to a ListControl.
        /// </summary>
        /// <param name="listControl">The list control you wish to use. For example a dropdown box.</param>
        /// <param name="sql">Your sql query string</param>
        /// <param name="connectionString">Your database connection string.</param>
        /// <param name="connectionType">The type of database you are using.</param>
        public static void ExecuteAndBind(ListControl listControl, string sql, string connectionString,
            DataBaseConnectionType connectionType)
        {
            ExecuteAndBind(listControl, null, null, null, sql, connectionString, connectionType);
        }

        /// <summary>
        /// Executes a query and binds it to a ListControl.
        /// </summary>
        /// <param name="listControl">The list control you wish to use. For example a dropdown box.</param>
        /// <param name="valueField">The name of the field representing the value.</param>
        /// <param name="textField">The name of the field representing the text to show.</param>
        /// <param name="sql">Your sql query string</param>
        public static void ExecuteAndBind(ListControl listControl, string valueField, string textField, string sql)
        {
            ExecuteAndBind(listControl, valueField, textField, null, sql, null, DataBaseConnectionType.Auto);
        }

        /// <summary>
        /// Executes a query and binds it to a ListControl.
        /// </summary>
        /// <param name="listControl">The list control you wish to use. For example a dropdown box.</param>
        /// <param name="valueField">The name of the field representing the value.</param>
        /// <param name="textField">The name of the field representing the text to show.</param>
        /// <param name="selectedValue">The selectedValue for this web control.</param>
        /// <param name="sql">Your sql query string</param>
        public static void ExecuteAndBind(ListControl listControl, string valueField, string textField,
            string selectedValue, string sql)
        {
            ExecuteAndBind(listControl, valueField, textField, selectedValue, sql, null, DataBaseConnectionType.Auto);
        }

        /// <summary>
        /// Executes a query and binds it to a ListControl.
        /// </summary>
        /// <param name="listControl">The list control you wish to use. For example a dropdown box.</param>
        /// <param name="valueField">The name of the field representing the value.</param>
        /// <param name="textField">The name of the field representing the text to show.</param>
        /// <param name="selectedValue">The selectedValue for this web control.</param>
        /// <param name="sql">Your sql query string</param>
        /// <param name="connectionString">Your database connection string.</param>
        /// <param name="connectionType">The type of database you are using.</param>
        public static void ExecuteAndBind(ListControl listControl, string valueField, string textField,
            string selectedValue, string sql, string connectionString,
            DataBaseConnectionType connectionType)
        {
            Query q = ExecuteReader(sql, connectionString, connectionType);
            listControl.DataSource = q.DataReader;
            listControl.DataTextField = textField ?? q.DataReader.GetName(1);
            listControl.DataValueField = valueField ?? q.DataReader.GetName(0);
            listControl.DataBind();
            if (selectedValue != null)
            {
                foreach (ListItem item in listControl.Items)
                {
                    if (item != null && item.Value != null)
                        item.Selected = Equals(item.Value.ToLowerInvariant() , selectedValue.ToLowerInvariant() );
                }
            }
            q.Close();
        }

        /// <summary>
        /// Executes a Transact-SQL statement.
        /// </summary>
        /// <param name="sql">The SQL statement</param>
        public static void ExecuteNonQuery(string sql)
        {
            ExecuteNonQuery(sql, null, DataBaseConnectionType.Auto);
        }

        /// <summary>
        /// Executes a Transact-SQL statement.
        /// </summary>
        /// <param name="sql">The SQL statement</param>
        /// <param name="connectionString">The connection string.</param>
        public static void ExecuteNonQuery(string sql, string connectionString)
        {
            ExecuteNonQuery(sql, connectionString, DataBaseConnectionType.Auto);
        }

        /// <summary>
        /// Executes a Transact-SQL statement and returns a WebGrid.Util.Query object.
        /// </summary>
        /// <param name="sql">The SQL statement</param>
        /// <returns>
        /// WebGrid.Util.Query object containing the results from the executed SQL statement
        /// </returns>
        public static Query ExecuteReader(string sql)
        {
            return ExecuteReader(sql, null, DataBaseConnectionType.Auto);
        }

        /// <summary>
        /// Executes a Transact-SQL statement and returns a WebGrid.Util.Query object.
        /// </summary>
        /// <param name="sql">The SQL statement</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>
        /// WebGrid.Util.Query object containing the results from the executed SQL statement
        /// </returns>
        public static Query ExecuteReader(string sql, string connectionString)
        {
            return ExecuteReader(sql, connectionString, DataBaseConnectionType.Auto);
        }

        /// <summary>
        /// Executes a Transact-SQL statement and returns the first column of the first row as an object value.
        /// </summary>
        /// <param name="sql">The SQL statement</param>
        /// <returns>the first row from the executed SQL statement</returns>
        public static object ExecuteScalar(string sql)
        {
            return ExecuteScalar(sql, null, DataBaseConnectionType.Auto);
        }

        /// <summary>
        /// Executes a Transact-SQL statement and returns the first column of the first row as an object value.
        /// </summary>
        /// <param name="sql">The SQL statement</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>the first row from the executed SQL statement</returns>
        public static object ExecuteScalar(string sql, string connectionString)
        {
            return ExecuteScalar(sql, connectionString, DataBaseConnectionType.Auto);
        }

        /// <summary>
        /// Closes the data reader and the connection for this instance.
        /// </summary>
        public void Close()
        {
            if (m_DataReader != null) m_DataReader.Close();
            if (m_Connection != null) m_Connection.Close();
            if (m_Connection != null) m_Connection.Dispose();
        }

        /// <summary>
        /// Gets safe value from database if column is null or DBNull.
        /// </summary>
        /// <param name="columnName">Name of the column</param>
        /// <param name="safeValue">Safevalue to return if null or DBNull</param>
        /// <returns></returns>
        public object GetSafeValue(string columnName, object safeValue)
        {
            if (IsDBNull(columnName) || m_DataReader[columnName] == DBNull.Value)
                return safeValue;
            return m_DataReader[columnName];
        }

        /// <summary>
        /// Checks if the specified column has database null value.
        /// </summary>
        /// <param name="columnIndex">The index of the column to check.</param>
        /// <returns>
        /// True if the column is null. False if the column has some value.
        /// </returns>
        public bool IsDBNull(int columnIndex)
        {
            return m_DataReader.IsDBNull(columnIndex);
        }

        /// <summary>
        /// Checks if the specified column has database null value.
        /// </summary>
        /// <param name="columnName">The name of the column to check.</param>
        /// <returns>
        /// True if the column is null. False if the column has some value.
        /// </returns>
        public bool IsDBNull(string columnName)
        {
            return IsDBNull(m_DataReader.GetOrdinal(columnName));
        }

        /// <summary>
        /// Attempts to read a new record from the database.
        /// </summary>
        /// <returns>
        /// True if a new record is available. False when we are finish reading the data.
        /// </returns>
        public bool Read()
        {
            return m_DataReader.Read();
        }

        /// <summary>
        /// Executes a Transact-SQL statement and returns a WebGrid.Util.Query object.
        /// </summary>
        /// <param name="sql">The SQL statement</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="connectionType">Type of the connection.</param>
        /// <returns>
        /// WebGrid.Util.Query object containing the results from the executed SQL statement
        /// </returns>
        internal static Query ExecuteReader(string sql, string connectionString, DataBaseConnectionType connectionType)
        {
            connectionString = ConnectionString.FindConnectionString(connectionString);
            if (connectionType == DataBaseConnectionType.Auto ||
                connectionType == DataBaseConnectionType.Unknown)
                connectionType = ConnectionType.FindConnectionType(connectionString);

            if (connectionType == DataBaseConnectionType.Unknown)
                throw new ApplicationException(
                    string.Format("Unable to automatically determine type of database from connection string. (ExecuteReader) ConnectionString: {0}", connectionString));

            switch (connectionType)
            {
                case DataBaseConnectionType.SqlConnection:
                    return SqlConnectionQuery.ExecuteReader(sql, connectionString);
                case DataBaseConnectionType.OleDB:
                    return OleDBQuery.ExecuteReader(sql, connectionString);
            //    case DataBaseConnectionTypes.MySql:
            //        return MySqlQuery.ExecuteReader(sql, connectionString);
                default:
                    throw new ApplicationException("Unknown connection type.");
            }
        }

        private static void ExecuteNonQuery(string sql, string connectionString, DataBaseConnectionType connectionType)
        {
            connectionString = ConnectionString.FindConnectionString(connectionString);
            if (connectionType == DataBaseConnectionType.Auto ||
                connectionType == DataBaseConnectionType.Unknown)
                connectionType = ConnectionType.FindConnectionType(connectionString);

            if (connectionType == DataBaseConnectionType.Unknown)
                throw new ApplicationException(
                    "Unable to automatically determine type of database from connection string. (ExecuteNonQuery)");

            switch (connectionType)
            {
                case DataBaseConnectionType.SqlConnection:
                    SqlConnectionQuery.ExecuteNonQuery(sql, connectionString);
                    break;
                case DataBaseConnectionType.OleDB:
                    OleDBQuery.ExecuteNonQuery(sql, connectionString);
                    break;
                default:
                    throw new ApplicationException("Unknown connection type.");
            }
        }

        private static object ExecuteScalar(string sql, string connectionString, DataBaseConnectionType connectionType)
        {
            connectionString = ConnectionString.FindConnectionString(connectionString);
            if (connectionType == DataBaseConnectionType.Auto || connectionType == DataBaseConnectionType.Unknown)
                connectionType = ConnectionType.FindConnectionType(connectionString);

            if (connectionType == DataBaseConnectionType.Unknown)
                throw new GridException(
                    "Unable to automatically determine type of database from connection string (ExecuteScalar)"+connectionString);

            switch (connectionType)
            {
                case DataBaseConnectionType.SqlConnection:
                    return SqlConnectionQuery.ExecuteScalar(sql, connectionString);
                case DataBaseConnectionType.OleDB:
                    return OleDBQuery.ExecuteScalar(sql, connectionString);
                default:
                    throw new GridException("Unknown connection type.");
            }
        }

        #endregion Methods
    }
}