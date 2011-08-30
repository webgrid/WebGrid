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
    using System.Collections.Specialized;
    using System.IO;
    using System.Web.UI;
    using System.Xml;

    /// <summary>
    /// This class contains methods and properties to translate your applications into new
    /// languages.
    /// </summary>
    public class TranslatePage : Page
    {
        #region Fields

        private readonly NameValueCollection systemMessageCollection = new NameValueCollection();
        private readonly NameValueCollection systemMessageLanguageCollection = new NameValueCollection();

        private string _Language;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public TranslatePage()
        {
            Debug = false;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public bool Debug
        {
            get; set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Language
        {
            get { return _Language; }
            set { _Language = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        public void LoadXml(string file)
        {
            XmlTextReader reader = new XmlTextReader(file);
            while (reader.Read())
                if (string.IsNullOrEmpty(reader.GetAttribute("value")) == false)
                    SetSystemMessage(reader.Name, reader.GetAttribute("value"));
            reader.Close();
        }

        /// <summary>
        /// Set or Add a system message that will be translated.
        /// </summary>
        /// <param name="systemMessageID">Alias for the system message.</param>
        /// <param name="displayText">The display text.</param>
        public void SetSystemMessage(string systemMessageID, string displayText)
        {
            systemMessageID = systemMessageID.ToUpperInvariant();
            if (systemMessageCollection[systemMessageID] != null)
                systemMessageCollection.Set(systemMessageID, displayText);
            else
                systemMessageCollection.Add(systemMessageID, displayText);
        }

        /*
                /// <summary>
                ///
                /// </summary>
                /// <param name="output"></param>
                override protected void Render(HtmlTextWriter output)
                {
                    StringBuilder StrBuilder = new StringBuilder();
                    StringWriter StrWriter = new StringWriter(StrBuilder);
                    HtmlTextWriter Baseoutput = new HtmlTextWriter(StrWriter);

                    base.Render(Baseoutput);
                    Baseoutput.Flush();
                    string content = Translate( StrBuilder.ToString() );
                    output.Write(content);
                    }
        */
        /// <summary>
        /// Set or Add a system message that will be translated.
        /// </summary>
        /// <param name="systemMessageID">The system message ID.</param>
        /// <param name="displayText">The display text.</param>
        /// <param name="language">language</param>
        public void SetSystemMessage(string systemMessageID, string displayText, string language)
        {
            systemMessageID = systemMessageID.ToUpperInvariant();
            if (systemMessageLanguageCollection[string.Format("{0};;{1}", systemMessageID, language)] != null)
                systemMessageLanguageCollection.Set(string.Format("{0};;{1}", systemMessageID, language), displayText);
            else
                systemMessageLanguageCollection.Add(string.Format("{0};;{1}", systemMessageID, language), displayText);
        }

        /// <summary>
        /// Get a translation system message.
        /// </summary>
        /// <param name="systemMessageID">Alias for the system message.</param>
        /// <returns>Title for the system message.</returns>
        public string SystemMessage(string systemMessageID)
        {
            string alias2 = systemMessageID.ToUpperInvariant();
            return systemMessageCollection[alias2] ?? systemMessageID;
        }

        /// <summary>
        /// Get a translation system message.
        /// </summary>
        /// <param name="systemMessageID">The system message ID.</param>
        /// <param name="language">The language.</param>
        /// <returns>Title for the system message.</returns>
        public string SystemMessage(string systemMessageID, string language)
        {
            string alias2 = systemMessageID.ToUpperInvariant();
            return systemMessageLanguageCollection[string.Format("{0};;{1}", alias2, language)] ?? systemMessageID;
        }

        /// <summary>
        /// Translates the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void Translate(HtmlTextWriter writer)
        {
            TextWriter tempWriter = new StringWriter();
            base.Render(new HtmlTextWriter(tempWriter));
            writer.Write(Translate(tempWriter.ToString()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        public string Translate(string content)
        {
            string[] tTranslated = new string[700];
            string[] tText = new string[700];
            int count = 0;
            int pos = content.IndexOf("[");

            while (pos > -1 && count < 699)
            {
                int posEnd = content.IndexOf("]", pos);
                if (posEnd != -1)
                {
                    string translatetext = content.Substring(pos + 1, posEnd - pos - 1);
                    if (string.IsNullOrEmpty(translatetext) == false)
                    {
                        tTranslated[count] = _Language != null ? SystemMessage(translatetext, _Language) : SystemMessage(translatetext);

                        if (tTranslated[count] != string.Empty)
                        {
                            tText[count] = translatetext;
                            count++;
                        }
                    }
                }
                else posEnd = pos;
                pos = content.IndexOf("[", posEnd);
            }

            if (count != 0)
            {
                for (int i = 0; i < count; i++)
                {
                    if (tText[i] != tTranslated[i])
                        content = content.Replace(string.Format("[{0}]", tText[i]), tTranslated[i]);
                }
            }
            return content;
        }

        /// <summary>
        /// Sends server control content to a provided <see cref="System.Web.UI.HtmlTextWriter"/> object, which writes the content to
        /// be rendered on
        /// the client.
        /// </summary>
        /// <param name="writer">The <see langword="HtmlTextWriter"/> object that receives the server control content.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            TextWriter tempWriter = new StringWriter();
            base.Render(new HtmlTextWriter(tempWriter));
            writer.Write(Translate(tempWriter.ToString()));
        }

        #endregion Methods
    }
}