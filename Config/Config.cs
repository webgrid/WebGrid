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

namespace WebGrid.Config
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using ConfigurationManager = System.Configuration.ConfigurationManager;
    using System.IO;
    using System.Web;
    using System.Web.UI.Design;
    using System.Windows.Forms;
    using System.Xml;

    using EnvDTE;

    using WebGrid.Design;

    internal class GridConfig
    {
        #region Fields

        internal readonly Dictionary<int, string> m_AppSettings;

        internal static ISite m_Site;

        private const string HTTPCONST = "HTTP://";

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GridConfig"/> class.
        /// </summary>
        /// <param name="site">The site.</param>
        public GridConfig(ISite site)
        {
            m_Site = site;

            m_AppSettings = new Dictionary<int, string>();
            foreach (string item in ConfigurationManager.AppSettings)
                m_AppSettings.Add(item.GetHashCode(), ConfigurationManager.AppSettings[item]);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// This method reads Web.Config and looks inside "appSettings" tags for
        /// a key with the same value as configName. If found the value from this key will be
        /// returned, else null will be returned.
        /// </summary>
        /// <param name="configName">The name of the key you want to find in Web.Config.</param>
        /// <returns>The value of the Key you found in Web.Config or null if no match was found.</returns>
        internal static string Get(string configName)
        {
            return Get(configName, null as string);
        }

        internal static bool Get(string configName, bool defaultValue)
        {
            string value = Get(configName);

            if (string.IsNullOrEmpty(value))
                return defaultValue;
            bool result;
            return bool.TryParse(value, out result) ? result : defaultValue;
        }

        internal static int Get(string configName, int defaultValue)
        {
            string value = Get(configName);

            if (string.IsNullOrEmpty(value))
                return defaultValue;
            int result;
            return int.TryParse(value, out result) ? result : defaultValue;
        }

        /// <summary>
        /// This method reads Web.Config and looks inside "appSettings" tags for
        /// a key with the same value as configName. If found the value from this key will be
        /// returned, else defaultValue will be returned.
        /// </summary>
        /// <param name="configName">The name of the key you want to find in Web.Config.</param>
        /// <param name="defaultValue">The default value you wish to use if Key is not found.</param>
        /// <returns>The value from the Key you found in Web.Config or defaultValue if no match was found.</returns>
        internal static string Get(string configName, string defaultValue)
        {
            if (m_Site == null || !m_Site.DesignMode)
            {

                string value = ConfigurationManager.AppSettings[configName];

                if (!string.IsNullOrEmpty(value))
                    return value;

                if (Grid.GotHttpContext && HttpContext.Current.Session != null &&
                    HttpContext.Current.Session[configName] != null)
                {
                    string runval = HttpContext.Current.Session[configName].ToString();
                    if (runval != null)
                        return runval;
                }
                return defaultValue;
            }
            string designval = GetConfigFromFile(configName);

            if (designval != null)
                return designval.StartsWith("Error Message", StringComparison.OrdinalIgnoreCase)
                           ? defaultValue
                           : designval;
            return defaultValue;
        }

        internal static string LoadSystemLanguages(string systemmessagefile)
        {
            if (systemmessagefile != null &&
                systemmessagefile.StartsWith(HTTPCONST, StringComparison.OrdinalIgnoreCase) == false)
            {
                if (Grid.GotHttpContext)
                {
                    if (systemmessagefile.StartsWith("~/"))
                        systemmessagefile = systemmessagefile.Remove(0, 2);

                    systemmessagefile = Path.Combine(HttpContext.Current.Server.MapPath("."), systemmessagefile);
                }
            }
            return systemmessagefile;
        }

        internal static string LoadSystemLanguages(string systemmessagefile, ISite site)
        {
            if (systemmessagefile != null &&
                systemmessagefile.StartsWith(HTTPCONST, StringComparison.OrdinalIgnoreCase) == false)
            {
                IWebApplication webApp = (IWebApplication) site.GetService(typeof (IWebApplication));
                if (webApp != null)
                {
                    if (webApp.RootProjectItem.PhysicalPath == null)
                        throw new ApplicationException(
                            string.Format("No Phystical path found for web project '{0}'", webApp.RootProjectItem.Name));
                    if (systemmessagefile.StartsWith("~/"))
                        systemmessagefile = systemmessagefile.Remove(0, 2);
                    systemmessagefile = Path.Combine(webApp.RootProjectItem.PhysicalPath, systemmessagefile);
                }
            }
            return systemmessagefile;
        }

        internal bool? Get(string configName, bool?  defaultValue)
        {
            string value = Get(configName);

            if (string.IsNullOrEmpty(value))
                return defaultValue;
            bool result;
            return bool.TryParse(value, out result) ? (result ? true  : false ) : defaultValue;
        }

        /// <summary>
        /// Gets the config from file.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private static string GetConfigFromFile(string key)
        {
            Grid grid =  m_Site.Component as Grid;

            if (grid == null)
                throw new GridException("Could not get Grid");
            if (grid.WebConfigAppSettings != null)
            {
                if (grid.NextConfigCheck < System.DateTime.Now)
                    return GetConfigSetting(grid,key);
                grid.NextConfigCheck = System.DateTime.Now.AddSeconds(5);
                return GetConfigSetting(grid, grid.WebConfigFile, key);
            }
            DTE devenv = (DTE) m_Site.GetService(typeof (DTE));

            if (devenv == null)
            {
                MessageBox.Show("Error Message: No design time environment was detected.");
                return null;
            }

            Array projects = (Array) devenv.ActiveSolutionProjects;

            if (projects.Length == 0 )
            {
                MessageBox.Show("Error Message: No Projects found in your designer environment. (Your IDE contains 0 active projects)");
                return null;
            }
            // go through the items of the project to find the configuration
            Project project = (Project) (projects.GetValue(0));

            foreach (ProjectItem item in project.ProjectItems)
            {
                // if it is the configuration, then open it up
                if (string.Compare(item.Name, "web.config", true) != 0)
                    continue;
                FileInfo info = new FileInfo("/");

                IWebApplication webApp = (IWebApplication) m_Site.GetService(typeof (IWebApplication));
                if (webApp != null)
                {
                    IProjectItem appitem = webApp.GetProjectItemFromUrl("~/web.config");
                    if (appitem.PhysicalPath == null)
                    {
                        MessageBox.Show(string.Format("Error Message: '{0}' has no physical path.", appitem.Name));
                        return null;
                    }
                    info = new FileInfo(appitem.PhysicalPath);
                }
                else
                    MessageBox.Show("Web Application Design Host is unavailable.");
                if (info.Directory != null)
                    if (System.IO.File.Exists(String.Format("{0}\\{1}", info.Directory.FullName, item.Name)) ==
                        false)
                    {
                        MessageBox.Show(
                            string.Format("Error Message: '{0}\\{1}' does not exists.", info.Directory.FullName,
                                          item.Name));
                        return null;
                    }

                if (info.Directory == null)
                    continue;
                string val = GetConfigSetting(grid,String.Format("{0}\\{1}", info.Directory.FullName, item.Name), key);
                if (val != null)
                    return val;

            }

            return null;
        }

        private static string GetConfigSetting(Grid grid, string key)
        {
            XmlDocument doc = grid.WebConfigAppSettings;
            if (doc == null || doc.DocumentElement == null)
                return null;

            XmlNode nodelist = doc.GetElementsByTagName("appSettings").Item(0);

            XmlNode hit = nodelist.SelectSingleNode("add[@key='" + key + "']");

            if (hit == null) return null;

            return hit.Attributes["value"] != null ? hit.Attributes["value"].Value : hit.Value;
        }

        private static string GetConfigSetting(Grid grid, string fileName, string key)
        {
            if (grid.Site != null)
            {
                XmlDocument doc = new XmlDocument();
                StreamReader sr = System.IO.File.OpenText(fileName);
                sr.BaseStream.Position = 0;
                string xml = sr.ReadToEnd();
                sr.Close();
                doc.LoadXml(xml);
                grid.WebConfigAppSettings = doc;
                grid.WebConfigFile = fileName;

            }
            return GetConfigSetting(grid,key);
        }

        #endregion Methods
    }

    /// <summary>
    /// WebGrid.Config
    /// </summary>
    internal class NamespaceDoc
    {
    }
}