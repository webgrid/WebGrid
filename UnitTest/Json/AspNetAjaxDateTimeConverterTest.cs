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

namespace Newtonsoft.Json.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using NUnit.Framework;

    using Newtonsoft.Json;

    using WebGrid.Util.Json;
    using WebGrid.Util.Json.Converters;

    [TestFixture]
    public class AspNetAjaxDateTimeConverterTest
    {
        #region Methods

        [Test]
        public void DeSerialize()
        {
            DateTimeTestClass c =
              JavaScriptConvert.DeserializeObject<DateTimeTestClass>(@"{""PreField"":""Pre"",""DateTimeField"":""@1229083932012@"",""PostField"":""Post""}", new AspNetAjaxDateTimeConverter());

              Assert.AreEqual(new DateTime(2008, 12, 12, 12, 12, 12, 12), c.DateTimeField);
              Assert.AreEqual("Pre", c.PreField);
              Assert.AreEqual("Post", c.PostField);
        }

        [Test]
        public void Serialize()
        {
            DateTimeTestClass c = new DateTimeTestClass();
              c.DateTimeField = new DateTime(2008, 12, 12, 12, 12, 12, 12);
              c.PreField = "Pre";
              c.PostField = "Post";

              string json = JavaScriptConvert.SerializeObject(c, new AspNetAjaxDateTimeConverter());

              Assert.AreEqual(@"{""PreField"":""Pre"",""DateTimeField"":""@1229083932012@"",""PostField"":""Post""}", json);
        }

        #endregion Methods

        #region Nested Types

        public class DateTimeTestClass
        {
            #region Fields

            public DateTime DateTimeField;
            public string PostField;
            public string PreField;

            #endregion Fields
        }

        #endregion Nested Types
    }
}