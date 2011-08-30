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
    using System.Data.SqlClient;

    using WebGrid.Design;

    /// <summary>
    /// Util.Query a Microsoft SQL Server data source.
    /// </summary>
    /// <exclude/>
    internal class SqlConnectionQuery : Query
    {
        #region Constructors

        /// <summary>
        /// Creates a new query against a database.
        /// </summary>
        /// <param name="sql">The sql query to execute.</param>
        /// <param name="connectionString">The connection string to use to connect to the database.</param>
        private SqlConnectionQuery(string sql, string connectionString)
        {
            InitQuery(sql, connectionString, CommandBehavior.Default);
        }

        /// <summary>
        /// Creates a new query against a database.
        /// </summary>
        /// <param name="sql">The sql query to execute.</param>
        /// <param name="connectionString">The connection string to use to connect to the database.</param>
        /// <param name="sqlBehavior">The SQL behavior.</param>
        private SqlConnectionQuery(string sql, string connectionString, CommandBehavior sqlBehavior)
        {
            InitQuery(sql, connectionString, sqlBehavior);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Executes a Transact-SQL statement.
        /// </summary>
        /// <param name="sql">The SQL statement</param>
        public static new void ExecuteNonQuery(string sql)
        {
            ExecuteNonQuery(sql, null);
        }

        /// <summary>
        /// Executes a Transact-SQL statement.
        /// </summary>
        /// <param name="sql">The SQL statement</param>
        /// <param name="connectionString">The connection string.</param>
        public static new void ExecuteNonQuery(string sql, string connectionString)
        {
            connectionString = ConnectionString.FindConnectionString(connectionString);

            SqlConnection connection = new SqlConnection();
            SqlCommand cmd = new SqlCommand();

            connection.ConnectionString = connectionString;

            cmd.Connection = connection;
            cmd.CommandText = sql;
            cmd.CommandTimeout = CommandTimeout;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ee)
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                throw new GridException(sql, "initialization error in SqlConnection ExecuteNonQuery", ee);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        /// <summary>
        /// Executes a Transact-SQL statement and returns a WebGrid.Util.Query object.
        /// </summary>
        /// <param name="sql">The SQL statement</param>
        /// <returns>
        /// WebGrid.Util.Query object containing the results from the executed SQL statement
        /// </returns>
        public static new Query ExecuteReader(string sql)
        {
            return ExecuteReader(sql, null);
        }

        /// <summary>
        /// Executes a Transact-SQL statement and returns a WebGrid.Util.Query object.
        /// </summary>
        /// <param name="sql">The SQL statement</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>
        /// WebGrid.Util.Query object containing the results from the executed SQL statement
        /// </returns>
        public static new Query ExecuteReader(string sql, string connectionString)
        {
            return new SqlConnectionQuery(sql, connectionString);
        }

        /// <summary>
        /// Executes a Transact-SQL statement and returns a WebGrid.Util.Query object.
        /// </summary>
        /// <param name="sql">The SQL statement</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="sqlBehavior">The SQL behavior.</param>
        /// <returns>
        /// WebGrid.Util.Query object containing the results from the executed SQL statement
        /// </returns>
        public static Query ExecuteReader(string sql, string connectionString, CommandBehavior sqlBehavior)
        {
            return new SqlConnectionQuery(sql, connectionString, sqlBehavior);
        }

        /// <summary>
        /// Executes a Transact-SQL statement and returns the first column of the first row as an object value.
        /// </summary>
        /// <param name="sql">The SQL statement</param>
        /// <returns>the first row from the executed SQL statement</returns>
        public static new object ExecuteScalar(string sql)
        {
            return ExecuteScalar(sql, null);
        }

        /// <summary>
        /// Executes a Transact-SQL statement and returns the first column of the first row as an object value.
        /// </summary>
        /// <param name="sql">The SQL statement</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>the first row from the executed SQL statement</returns>
        public static new object ExecuteScalar(string sql, string connectionString)
        {
            connectionString = ConnectionString.FindConnectionString(connectionString);

            SqlConnection connection = new SqlConnection();
            SqlCommand cmd = new SqlCommand();

            connection.ConnectionString = connectionString;

            cmd.Connection = connection;
            cmd.CommandText = sql;
            cmd.CommandTimeout = CommandTimeout;
            object ret;
            try
            {
                connection.Open();
                ret = cmd.ExecuteScalar();
            }
            catch (Exception ee)
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                throw new GridException(sql, "initialization error in SqlConnection ExecuteScalar", ee);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return ret;
        }

        private void InitQuery(string sql, string connectionString, CommandBehavior sqlBehavior)
        {
            connectionString = ConnectionString.FindConnectionString(connectionString);

            m_Connection = new SqlConnection();
            SqlCommand cmd = new SqlCommand();

            m_Connection.ConnectionString = connectionString;

            cmd.Connection = (SqlConnection) m_Connection;

            cmd.CommandText = sql;
            cmd.CommandTimeout = CommandTimeout;
            try
            {
                Connection.Open();
                m_DataReader = cmd.ExecuteReader(sqlBehavior);
            }
            catch (Exception ee)
            {
                if (Connection != null && Connection.State == ConnectionState.Open)
                    Connection.Close();
                throw new GridException(sql, "initialization error in SqlConnection InitQuery", ee);
            }
        }

        #endregion Methods
    }
}