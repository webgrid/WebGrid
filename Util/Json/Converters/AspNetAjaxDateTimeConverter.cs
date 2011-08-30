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

namespace WebGrid.Util.Json.Converters
{
    using System;
    using System.Globalization;

    ///<summary>
    ///</summary>
    public class AspNetAjaxDateTimeConverter : JsonConverter
    {
        #region Methods

        ///<summary>
        ///</summary>
        ///<param name="valueType"></param>
        ///<returns></returns>
        public override bool CanConvert(Type valueType)
        {
            return typeof(System.DateTime).IsAssignableFrom(valueType);
        }

        ///<summary>
        ///</summary>
        ///<param name="reader"></param>
        ///<param name="objectType"></param>
        ///<returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType)
        {
            string dateTimeText = (string)reader.Value;
              dateTimeText = dateTimeText.Substring(1, dateTimeText.Length - 2);

              long javaScriptTicks = Convert.ToInt64(dateTimeText);

              return JavaScriptConvert.ConvertJavaScriptTicksToDateTime(javaScriptTicks);
        }

        ///<summary>
        ///</summary>
        ///<param name="writer"></param>
        ///<param name="value"></param>
        public override void WriteJson(JsonWriter writer, object value)
        {
            System.DateTime dateTime = (System.DateTime)value;
              long javaScriptTicks = JavaScriptConvert.ConvertDateTimeToJavaScriptTicks(dateTime);

              writer.WriteValue("@" + javaScriptTicks.ToString(null, CultureInfo.InvariantCulture) + "@");
        }

        #endregion Methods
    }
}