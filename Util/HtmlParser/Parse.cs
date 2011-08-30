/*
Copyright ©  Olav Christian Botterli.

Dual licensed under the MIT or GPL Version 2 licenses.

Date: 30.08.2011, Norway.

http://www.webgrid.com
*/


/*
Copyright ©  Olav Christian Botterli. 

Dual licensed under the MIT or GPL Version 2 licenses.

Date: 30.08.2011, Norway.

http://www.webgrid.com
*/
namespace WebGrid.Util.HtmlParser
{
    /// <summary>
    /// Base class for parsing tag based files, such as HTML,
    /// HTTP headers, or XML.
    ///
    /// This source code may be used freely under the
    /// Limited GNU Public License(LGPL).
    ///
    /// Written by Jeff Heaton (http://www.jeffheaton.com)
    /// </summary>
    internal class Parse : AttributeList
    {
        #region Fields

        /// <summary>
        /// The most recently parsed tag.
        /// </summary>
        public string M_tag;

        /// <summary>
        /// The current position inside of the text that
        /// is being parsed.
        /// </summary>
        private int m_idx;

        /// <summary>
        /// The most recently parsed attribute delimiter.
        /// </summary>
        private char m_parseDelim;

        /// <summary>
        /// This most recently parsed attribute name.
        /// </summary>
        private string m_parseName;

        /// <summary>
        /// The most recently parsed attribute value.
        /// </summary>
        private string m_parseValue;

        /// <summary>
        /// The source text that is being parsed.
        /// </summary>
        private string m_source;

        #endregion Fields

        #region Properties

        /// <summary>
        /// The last attribute delimiter that was encountered.
        /// </summary>
        /// <value>The parse delim.</value>
        internal char ParseDelim
        {
            get { return m_parseDelim; }

            set { m_parseDelim = value; }
        }

        /// <summary>
        /// The last attribute name that was encountered.
        /// </summary>
        /// <value>The name of the parse.</value>
        internal string ParseName
        {
            get { return m_parseName; }

            set { m_parseName = value; }
        }

        /// <summary>
        /// The last attribute value that was encountered.
        /// </summary>
        /// <value>The parse value.</value>
        internal string ParseValue
        {
            get { return m_parseValue; }

            set { m_parseValue = value; }
        }

        /// <summary>
        /// The text that is to be parsed.
        /// </summary>
        /// <value>The source.</value>
        internal string Source
        {
            get { return m_source; }

            set { m_source = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determine if the specified character is whitespace or not.
        /// </summary>
        /// <param name="ch">A character to check</param>
        /// <returns>true if the character is whitespace</returns>
        internal static bool IsWhiteSpace(char ch)
        {
            return ("\t\n\r ".IndexOf(ch) != -1);
        }

        /// <summary>
        /// Add a parsed attribute to the collection.
        /// </summary>
        internal void AddAttribute()
        {
            Attribute a = new Attribute(m_parseName,
                                        m_parseValue, m_parseDelim);
            Add(a);
        }

        /// <summary>
        /// Move the index forward by one.
        /// </summary>
        internal void Advance()
        {
            m_idx++;
        }

        /// <summary>
        /// Obtain the next character and advance the index by one.
        /// </summary>
        /// <returns>The next character</returns>
        internal char AdvanceCurrentChar()
        {
            return m_source[m_idx++];
        }

        /// <summary>
        /// Advance the index until past any whitespace.
        /// </summary>
        internal void EatWhiteSpace()
        {
            while (!Eof())
            {
                if (!IsWhiteSpace(GetCurrentChar()))
                    return;
                m_idx++;
            }
        }

        /// <summary>
        /// Determine if the end of the source text has been reached.
        /// </summary>
        /// <returns>True if the end of the source text has been
        /// reached.</returns>
        internal bool Eof()
        {
            return (m_idx >= m_source.Length);
        }

        /// <summary>
        /// Get the current character that is being parsed.
        /// </summary>
        internal char GetCurrentChar()
        {
            return GetCurrentChar(0);
        }

        /// <summary>
        /// Get a few characters ahead of the current character.
        /// </summary>
        /// <param name="peek">How many characters to peek ahead
        /// for.</param>
        /// <returns>The character that was retrieved.</returns>
        internal char GetCurrentChar(int peek)
        {
            return (m_idx + peek) < m_source.Length ? m_source[m_idx + peek] : (char) 0;
        }

        /// <summary>
        /// Parse the attribute name.
        /// </summary>
        internal void ParseAttributeName()
        {
            EatWhiteSpace();
            // get attribute name
            while (!Eof())
            {
                if (IsWhiteSpace(GetCurrentChar()) ||
                    (GetCurrentChar() == '=') ||
                    (GetCurrentChar() == '>'))
                    break;
                m_parseName += GetCurrentChar();
                m_idx++;
            }

            EatWhiteSpace();
        }

        /// <summary>
        /// Parse the attribute value
        /// </summary>
        internal void ParseAttributeValue()
        {
            if (m_parseDelim != 0)
                return;

            if (GetCurrentChar() != '=') return;
            m_idx++;
            EatWhiteSpace();
            if ((GetCurrentChar() == '\'') ||
                (GetCurrentChar() == '\"'))
            {
                m_parseDelim = GetCurrentChar();
                m_idx++;
                while (GetCurrentChar() != m_parseDelim)
                {
                    m_parseValue += GetCurrentChar();
                    m_idx++;
                }
                m_idx++;
            }
            else
            {
                while (!Eof() &&
                       !IsWhiteSpace(GetCurrentChar()) &&
                       (GetCurrentChar() != '>'))

                {
                    m_parseValue += GetCurrentChar();
                    m_idx++;
                }
            }
            EatWhiteSpace();
        }

        #endregion Methods
    }
}