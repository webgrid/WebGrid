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

namespace WebGrid.Util.HtmlParser
{
    using System;
    using System.Text;

    /// <summary>
    /// This class contains methods and properties for <see cref="WebGrid.Grid">WebGrid.Grid</see> web control
    /// to read ASPX file.
    /// </summary>
    internal class ParseHtml : Parse
    {
        #region Methods

        public String BuildTag()
        {
            StringBuilder bufferBuilder = new StringBuilder("<");
            bufferBuilder.Append(M_tag);
            int i = 0;
            while (this[i] != null)

            {
            // has attributes
                bufferBuilder.Append(" ");
                if (this[i].Value == null)
                {
                    if (this[i].Delim != 0)
                        bufferBuilder.Append(this[i].Delim);
                    bufferBuilder.Append(this[i].Name);
                    if (this[i].Delim != 0)
                        bufferBuilder.Append(this[i].Delim);
                }
                else
                {
                    bufferBuilder.Append(this[i].Name);
                    if (this[i].Value != null)
                    {
                        bufferBuilder.Append("=");
                        if (this[i].Delim != 0)
                            bufferBuilder.Append(this[i].Delim);
                        bufferBuilder.Append(this[i].Value);
                        if (this[i].Delim != 0)
                            bufferBuilder.Append(this[i].Delim);
                    }
                }
                i++;
            }
            bufferBuilder.Append(">");
            return bufferBuilder.ToString();
        }

        public AttributeList GetTag()
        {
            AttributeList tag = new AttributeList();
            tag.Name = M_tag;

            foreach (Attribute x in List)
            {
                tag.Add((Attribute) x.Clone());
            }

            return tag;
        }

        public char Parse()
        {
            switch (GetCurrentChar())
            {
                case '<':
                    {
                        Advance();

                        char ch = char.ToUpper(GetCurrentChar());
                        if ((ch >= 'A') && (ch <= 'Z') || (ch == '!') || (ch == '/'))
                        {
                            ParseTag();
                            return (char) 0;
                        }

                        return (AdvanceCurrentChar());
                    }
                default:
                    return (AdvanceCurrentChar());
            }
        }

        protected void ParseTag()
        {
            M_tag = string.Empty;
            Clear();

            // Is it a comment?
            if ((GetCurrentChar() == '!') &&
                (GetCurrentChar(1) == '-') &&
                (GetCurrentChar(2) == '-'))
            {
                while (!Eof())
                {
                    if ((GetCurrentChar() == '-') &&
                        (GetCurrentChar(1) == '-') &&
                        (GetCurrentChar(2) == '>'))
                        break;
                    if (GetCurrentChar() != '\r')
                        M_tag += GetCurrentChar();
                    Advance();
                }
                M_tag += "--";
                Advance();
                Advance();
                Advance();
                ParseDelim = (char) 0;
                return;
            }

            // Find the tag name
            while (!Eof())
            {
                if (IsWhiteSpace(GetCurrentChar()) ||
                    (GetCurrentChar() == '>'))
                    break;
                M_tag += GetCurrentChar();
                Advance();
            }

            EatWhiteSpace();

            // Get the attributes
            while (GetCurrentChar() != '>')
            {
                ParseName = string.Empty;
                ParseValue = string.Empty;
                ParseDelim = (char) 0;

                ParseAttributeName();

                if (GetCurrentChar() == '>')

                {
                    AddAttribute();
                    break;
                }

                // Get the value(if any)
                ParseAttributeValue();
                AddAttribute();
            }
            Advance();
        }

        #endregion Methods
    }
}