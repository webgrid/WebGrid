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

using System;
using System.Configuration;
using System.Xml;

internal class AppConfig : AppSettingsReader
{
    #region Fields

    internal string m_DocName = String.Empty;

    private XmlNode m_Node;

    #endregion Fields

    #region Methods

    public bool RemoveElement(string elementKey)
    {
        try
        {
            XmlDocument cfgDoc = new XmlDocument();

            LoadConfigDoc(cfgDoc);

            // retrieve the appSettings node

            m_Node = cfgDoc.SelectSingleNode("//appSettings");

            if (m_Node == null)
            {
                throw new InvalidOperationException("appSettings section not found");
            }

            // XPath select setting "add" element that contains this key to remove

            m_Node.RemoveChild(m_Node.SelectSingleNode(string.Format("//add[@key='{0}']", elementKey)));

            SaveConfigDoc(cfgDoc, m_DocName);

            return true;
        }

        catch
        {
            return false;
        }
    }

    public bool SetValue(string key, string value, string section, string prefix)
    {
        if (string.IsNullOrEmpty(section) || section.StartsWith("//"))

            throw new ApplicationException("Section must start with //");

        XmlDocument cfgDoc = new XmlDocument();

        LoadConfigDoc(cfgDoc);

        // retrieve the appSettings node

        m_Node = cfgDoc.SelectSingleNode(section);

        if (m_Node == null)
        {
            throw new InvalidOperationException("appSettings section not found");
        }

        try
        {
            // XPath select setting "add" element that contains this key

            XmlElement addElem = (XmlElement) m_Node.SelectSingleNode(string.Format("//{0}[@key='{1}']", prefix, key));

            if (addElem != null)
            {
                addElem.SetAttribute("value", value);
            }

                // not found, so we need to add the element, key and value

            else
            {
                addElem = cfgDoc.CreateElement("add");

                addElem.SetAttribute("key", key);

                addElem.SetAttribute("value", value);

                m_Node.AppendChild(addElem);
            }
            //save it

            SaveConfigDoc(cfgDoc, m_DocName);

            return true;
        }

        catch
        {
            return false;
        }
    }

    private static void SaveConfigDoc(XmlNode cfgDoc, string cfgDocPath)
    {
        {
            XmlTextWriter writer = new XmlTextWriter(cfgDocPath, null) {Formatting = Formatting.Indented};

            cfgDoc.WriteTo(writer);

            writer.Flush();

            writer.Close();

            return;
        }
    }

    private void LoadConfigDoc(XmlDocument cfgDoc)
    {
        cfgDoc.Load(m_DocName);

        return;
    }

    #endregion Methods
}