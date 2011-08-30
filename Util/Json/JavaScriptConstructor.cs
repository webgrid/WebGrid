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

namespace WebGrid.Util.Json
{
    using System;
    using System.Text;

    /// <summary>
    /// Represents a JavaScript constructor.
    /// </summary>
    public class JavaScriptConstructor
    {
        #region Fields

        private readonly string _name;
        private readonly JavaScriptParameters _parameters;

        #endregion Fields

        #region Constructors

        ///<summary>
        ///</summary>
        ///<param name="name"></param>
        ///<param name="parameters"></param>
        ///<exception cref="ArgumentNullException"></exception>
        ///<exception cref="ArgumentException"></exception>
        public JavaScriptConstructor(string name, JavaScriptParameters parameters)
        {
            if (name == null)
            throw new ArgumentNullException("name");

              if (name.Length == 0)
            throw new ArgumentException("Constructor name cannot be empty.", "name");

              _name = name;
              _parameters = parameters ?? JavaScriptParameters.Empty;
        }

        #endregion Constructors

        #region Properties

        ///<summary>
        ///</summary>
        public string Name
        {
            get { return _name; }
        }

        ///<summary>
        ///</summary>
        public JavaScriptParameters Parameters
        {
            get { return _parameters; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

              sb.Append("new ");
              sb.Append(_name);
              sb.Append("(");
              if (_parameters != null)
              {
            for (int i = 0; i < _parameters.Count; i++)
            {
              sb.Append(_parameters[i]);
            }
              }
              sb.Append(")");

              return sb.ToString();
        }

        #endregion Methods
    }
}