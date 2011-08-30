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
// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

#endregion Header

namespace WebGrid.Util.Json.Utilities
{
    using System.IO;
    using System.Web.UI;

    ///<summary>
    ///</summary>
    public static class JavaScriptUtils
    {
        #region Methods

        ///<summary>
        ///</summary>
        ///<param name="page"></param>
        ///<param name="control"></param>
        ///<returns></returns>
        public static string GetCallbackEventFunction(Page page, Control control)
        {
            string script = page.ClientScript.GetCallbackEventReference(control, "eventArgument", "eventCallback", "context", "errorCallback", true);

            script = string.Format("function(eventArgument,eventCallback,context,errorCallback){{{0}}}", script);

            return script;
        }

        ///<summary>
        ///</summary>
        ///<param name="page"></param>
        ///<param name="control"></param>
        ///<param name="argument"></param>
        ///<returns></returns>
        public static string GetCallbackEventFunction(Page page, Control control, string argument)
        {
            string script = page.ClientScript.GetCallbackEventReference(control, string.Format("'{0}'", argument), "eventCallback", "context", "errorCallback", true);

            script = "function(eventCallback,context,errorCallback){" + script + "}";

            return script;
        }

        ///<summary>
        ///</summary>
        ///<param name="value"></param>
        ///<returns></returns>
        public static string ToEscapedJavaScriptString(string value)
        {
            return ToEscapedJavaScriptString(value, '"', true);
        }

        ///<summary>
        ///</summary>
        ///<param name="value"></param>
        ///<param name="delimiter"></param>
        ///<param name="appendDelimiters"></param>
        ///<returns></returns>
        public static string ToEscapedJavaScriptString(string value, char delimiter, bool appendDelimiters)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            using (StringWriter w = StringUtils.CreateStringWriter(StringUtils.GetLength(value) ?? 16))
            {
                WriteEscapedJavaScriptString(w, value, delimiter, appendDelimiters);
                return w.ToString();
            }
        }

        ///<summary>
        /// Write escaped javascript char.
        ///</summary>
        ///<param name="writer">Textwriter </param>
        ///<param name="c">character</param>
        ///<param name="delimiter">delimiter</param>
        public static void WriteEscapedJavaScriptChar(TextWriter writer, char c, char delimiter)
        {
            switch (c)
            {
                case '\t':
                    writer.Write(@"\t");
                    break;
                case '\n':
                    writer.Write(@"\n");
                    break;
                case '\r':
                    writer.Write(@"\r");
                    break;
                case '\f':
                    writer.Write(@"\f");
                    break;
                case '\b':
                    writer.Write(@"\b");
                    break;
                case '\\':
                    writer.Write(@"\\");
                    break;
                case '\'':
                    // only escape if this charater is being used as the delimiter
                    writer.Write((delimiter == '\'') ? @"\'" : @"'");
                    break;
                case '"':
                    // only escape if this charater is being used as the delimiter
                    writer.Write((delimiter == '"') ? "\\\"" : @"""");
                    break;
                default:
                    if (c > '\u001f')
                        writer.Write(c);
                    else
                        StringUtils.WriteCharAsUnicode(writer, c);
                    break;
            }
        }

        ///<summary>
        ///</summary>
        ///<param name="writer"></param>
        ///<param name="value"></param>
        ///<param name="delimiter"></param>
        ///<param name="appendDelimiters"></param>
        public static void WriteEscapedJavaScriptString(TextWriter writer, string value, char delimiter, bool appendDelimiters)
        {
            // leading delimiter
            if (appendDelimiters)
                writer.Write(delimiter);

            if (value != null)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    WriteEscapedJavaScriptChar(writer, value[i], delimiter);
                }
            }

            // trailing delimiter
            if (appendDelimiters)
                writer.Write(delimiter);
        }

        #endregion Methods
    }
}