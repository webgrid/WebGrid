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

    #region Enumerations

    /// <summary>
    /// Connection types supported by <see cref="WebGrid.Grid">WebGrid.Grid</see> web control.
    /// </summary>
    public enum DataBaseConnectionType
    {
        /// <summary>
        /// Automatically detect your connection type by looking at your connection string.
        /// </summary>
        Auto = 0,
        /// <summary>
        /// Sets ConnectionType manually to be SqlConnection
        /// </summary>
        SqlConnection = 1,
           /* /// <summary>
        /// Sets ConnectionType manually to be MySql
        /// </summary>
        MySql = 2,*/
        /// <summary>
        /// Sets ConnectionType manually to be Ole DB Connection String
        /// </summary>
        OleDB = 3,
        /// <summary>
        /// Unknown connection type.
        /// </summary>
        Unknown = 4
    }

    #endregion Enumerations

    /// <summary>
    /// Class containing the supported SQL Server connection types.
    /// </summary>
    public static class ConnectionType
    {
        #region Methods

        /// <summary>
        /// Finds the type of the connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The connection string</returns>
        public static DataBaseConnectionType FindConnectionType(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString) ||
                connectionString.IndexOf(";", StringComparison.OrdinalIgnoreCase) == -1)
                return DataBaseConnectionType.Unknown;

            if (connectionString.IndexOf("Provider=sqloledb", StringComparison.OrdinalIgnoreCase) > -1 ||
                connectionString.IndexOf("Provider=SQLNCLI", StringComparison.OrdinalIgnoreCase) > -1 ||
                connectionString.IndexOf("microsoft.jet.oledb", StringComparison.OrdinalIgnoreCase) > -1 ||
                connectionString.IndexOf("microsoft access driver", StringComparison.OrdinalIgnoreCase) > -1 ||
                connectionString.IndexOf("microsoft.ace.oledb", StringComparison.OrdinalIgnoreCase) > -1)
                return DataBaseConnectionType.OleDB;

            if (connectionString.IndexOf("server=", StringComparison.OrdinalIgnoreCase) > -1 ||
                connectionString.IndexOf("data source=", StringComparison.OrdinalIgnoreCase) > -1)
                return DataBaseConnectionType.SqlConnection;

            return DataBaseConnectionType.Unknown;
        }

        #endregion Methods
    }
}