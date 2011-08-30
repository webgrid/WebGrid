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

namespace WebGrid.Sentences
{
    using System;
    using System.Collections.Specialized;
    using System.Xml.XPath;

    using WebGrid.Design;
    using WebGrid.Enums;

    internal class SystemMessages
    {
        #region Fields

        internal NameValueCollection m_SystemMessageCollection = new NameValueCollection();

        #endregion Fields

        #region Constructors

        internal SystemMessages()
        {
        }

        internal SystemMessages(SystemLanguage language, string systemmessagefile, Grid grid)
        {
            if (string.IsNullOrEmpty(systemmessagefile))
                throw new ApplicationException("No XML file was found for WebGrid system messages.");
            switch (language)
            {
                case SystemLanguage.Undefined:
                    LoadXml("English", systemmessagefile, grid);
                    break;
                case SystemLanguage.English:
                    LoadXml("English", systemmessagefile, grid);
                    break;
                case SystemLanguage.Norwegian:
                    LoadXml("Norwegian", systemmessagefile, grid);
                    break;
                case SystemLanguage.Danish:
                    LoadXml("Danish", systemmessagefile, grid);
                    break;
                case SystemLanguage.Swedish:
                    LoadXml("Swedish", systemmessagefile, grid);
                    break;
                case SystemLanguage.Spanish:
                    LoadXml("Spanish", systemmessagefile, grid);
                    break;
                default:
                    LoadXml("English", systemmessagefile, grid);
                    break;
            }
        }

        #endregion Constructors

        #region Methods

        internal string GetSystemMessage(string systemMessageID)
        {
            return string.IsNullOrEmpty(systemMessageID)
                       ? null
                       : (m_SystemMessageCollection[systemMessageID] ?? systemMessageID);
        }

        internal string GetSystemMessageClass(string systemMessageID, string defaultClass)
        {
            if (string.IsNullOrEmpty(systemMessageID))
                return defaultClass;
            return m_SystemMessageCollection[systemMessageID + "_css"] ?? defaultClass;
        }

        internal void LoadXml(string language, string file, Grid grid)
        {
            //  int count = 0;
            try
            {
                XPathDocument doc = new XPathDocument(file);
                XPathNavigator nav = doc.CreateNavigator();
                XPathNodeIterator it = nav.Select(string.Format(@"//{0}/*", language));

                while (it.MoveNext())
                {
               //         count++;
                    if (string.IsNullOrEmpty(it.Current.Name) || string.IsNullOrEmpty(it.Current.Value))
                        continue;
                    SetSystemMessage(it.Current.Name, it.Current.Value, null);
                }
            }
            catch (Exception ee)
            {
                throw new GridException(
                    string.Format("Error loading WebGrid system message file '{0}'. Error reason: '{1}'", file, ee), ee);
            }
        }

        internal void SetSystemMessage(string systemMessageID, string displayText, string cssClass)
        {
            if (string.IsNullOrEmpty(systemMessageID))
                return;
            if (string.IsNullOrEmpty(displayText) == false)
                if (m_SystemMessageCollection[systemMessageID] != null)
                    m_SystemMessageCollection.Set(systemMessageID, displayText);
                else
                    m_SystemMessageCollection.Add(systemMessageID, displayText);

            if (string.IsNullOrEmpty(cssClass)) return;
            if (m_SystemMessageCollection[string.Format("{0}_css", systemMessageID)] != null)
                m_SystemMessageCollection.Set(string.Format("{0}_css", systemMessageID), cssClass);
            else
                m_SystemMessageCollection.Add(string.Format("{0}_css", systemMessageID), cssClass);
        }

        #endregion Methods
    }
}