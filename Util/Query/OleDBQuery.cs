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

    using WebGrid.Design;

    /// <summary>
    /// Util.Query a Microsoft SQL Server data source.
    /// </summary>
    /// <exclude/>
    internal class OleDBQuery : Query
    {
        #region Constructors

        /// <summary>
        /// Creates a new query against a database.
        /// </summary>
        /// <param name="sql">The sql query to execute.</param>
        /// <param name="connectionString">The connection string to use to connect to the database.</param>
        private OleDBQuery(string sql, string connectionString)
        {
            InitQuery(sql, connectionString, CommandBehavior.Default);
        }

        /// <summary>
        /// Creates a new query against a database.
        /// </summary>
        /// <param name="sql">The sql query to execute.</param>
        /// <param name="connectionString">The connection string to use to connect to the database.</param>
        /// <param name="sqlBehavior">The SQL behavior.</param>
        private OleDBQuery(string sql, string connectionString, CommandBehavior sqlBehavior)
        {
            InitQuery(sql, connectionString, sqlBehavior);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        public static new void ExecuteNonQuery(string sql)
        {
            ExecuteNonQuery(sql, null);
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="connectionString">The connection string.</param>
        public static new void ExecuteNonQuery(string sql, string connectionString)
        {
            connectionString = ConnectionString.FindConnectionString(connectionString);

            OleDbConnection connection = new OleDbConnection();
            OleDbCommand cmd = new OleDbCommand();

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
                throw new GridException(sql, "initialization error in OleDb ExecuteNonQuery", ee);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns></returns>
        public static new Query ExecuteReader(string sql)
        {
            return ExecuteReader(sql, null);
        }

        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        public static new Query ExecuteReader(string sql, string connectionString)
        {
            return new OleDBQuery(sql, connectionString);
        }

        public static Query ExecuteReader(string sql, string connectionString, CommandBehavior sqlBehavior)
        {
            return new OleDBQuery(sql, connectionString, sqlBehavior);
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns></returns>
        public static new object ExecuteScalar(string sql)
        {
            return ExecuteScalar(sql, null);
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        public static new object ExecuteScalar(string sql, string connectionString)
        {
            connectionString = ConnectionString.FindConnectionString(connectionString);

            OleDbConnection connection = new OleDbConnection();
            OleDbCommand cmd = new OleDbCommand();

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
                throw new GridException(sql, "initialization error in OleDb ExecuteScalar", ee);
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

            m_Connection = new OleDbConnection();
            OleDbCommand cmd = new OleDbCommand();

            m_Connection.ConnectionString = connectionString;

            cmd.Connection = (OleDbConnection)m_Connection;

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