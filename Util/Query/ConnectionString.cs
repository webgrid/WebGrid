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
    using System.Configuration;
    using System.IO;
    using System.Web.UI.Design;

    using WebGrid.Config;

    /// <summary>
    /// This class contains methods and properties that are used to validate and
    /// use oleDB connection string. If <see cref="WebGrid.Grid.ConnectionString"/> is undefined
    ///connection string property will be set by Web.Config and key "WGConnectionstring".
    /// </summary>
    public static class ConnectionString
    {
        #region Methods

        /// <summary>
        /// Finds the valid connection string for your data source.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The connection string</returns>
        public static string FindConnectionString(string connectionString)
        {
            DataBaseConnectionType connection_type = ConnectionType.FindConnectionType(connectionString);

            if (connection_type != DataBaseConnectionType.Unknown) // Already valid connection string?
                return connectionString;

            // NO ConnectionString provided...
            connectionString = string.IsNullOrEmpty(connectionString) ? GridConfig.Get("WGConnectionString", null as string) : GridConfig.Get(connectionString, connectionString);

            return connectionString;
        }

        /// <summary>
        /// Finds the valid connection string for your data source.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="grid">The grid object</param>
        /// <returns>The connection string</returns>
        public static string FindConnectionString(string connectionString, Grid grid)
        {
            DataBaseConnectionType connection_type = ConnectionType.FindConnectionType(connectionString);
            if (string.IsNullOrEmpty(connectionString) == false &&
                connectionString.IndexOf("|DataDirectory|", StringComparison.OrdinalIgnoreCase) > -1)
            {
                if (!Grid.GotHttpContext)
                {
                    if (grid == null || grid.Site == null)
                        return connectionString;
                    IWebApplication webApp = (IWebApplication) grid.Site.GetService(typeof (IWebApplication));
                    if (webApp != null)
                    {
                        string directory = webApp.RootProjectItem.PhysicalPath;

                        if (directory == null)
                            throw new ApplicationException("No Physical path found for |DataDirectory|");
                        if (Directory.Exists(Path.Combine(directory, "App_data")))
                            directory = Path.Combine(directory, "App_data");
                        AppDomain.CurrentDomain.SetData("DataDirectory", directory);
                    }
                }
            } // We already got valid connection string
            if (connection_type != DataBaseConnectionType.Unknown)
            {
                return connectionString;
            }
            // Load default connection string from AppSettings
            if (string.IsNullOrEmpty(connectionString))
                connectionString = "WGConnectionString";
            if (GridConfig.Get(connectionString, null as string) == null)
            {
                //If not try to load it from ConnectionStrings
                if (ConfigurationManager.ConnectionStrings != null && ConfigurationManager.ConnectionStrings[connectionString] != null)
                    return ConfigurationManager.ConnectionStrings[connectionString].ConnectionString;
                return null;
            }
            connectionString = GridConfig.Get(connectionString, null as string);
               return connectionString;
        }

        internal static string GetPhysicalPath(Grid grid)
        {
            if (!Grid.GotHttpContext)
            {
                if (grid == null || grid.Site == null)
                    return null;
                IWebApplication webApp = (IWebApplication)grid.Site.GetService(typeof(IWebApplication));
                if (webApp != null)
                    return webApp.RootProjectItem.PhysicalPath;
            }
            return null;
        }

        #endregion Methods
    }
}