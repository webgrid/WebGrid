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

    /// <summary>
    /// Attributes holds one attribute, as is normally stored in an
    /// HTML or XML file. This includes a name, value and delimiter.
    /// This source code may be used freely under the
    /// Limited GNU Public License(LGPL).
    ///
    /// Written by Jeff Heaton (http://www.jeffheaton.com)
    /// </summary>
    internal class Attribute : ICloneable
    {
        #region Fields

        /// <summary>
        /// The delimiter for the value of this attribute(i.e. " or ').
        /// </summary>
        private char m_delim;

        /// <summary>
        /// The name of this attribute
        /// </summary>
        private string m_name;

        /// <summary>
        /// The value of this attribute
        /// </summary>
        private string m_value;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// The default constructor.  Construct a blank attribute.
        /// </summary>
        public Attribute()
            : this(string.Empty, string.Empty, (char) 0)
        {
        }

        /// <summary>
        /// Construct an attribute without a delimiter.
        /// </summary>
        /// <param name="name">The name of this attribute.</param>
        /// <param name="value">The value of this attribute.</param>
        public Attribute(String name, String value)
            : this(name, value,
                                                           (char) 0)
        {
        }

        /// <summary>
        /// Construct a new Attributes.  The name, delim, and value
        /// properties can be specified here.
        /// </summary>
        /// <param name="name">The name of this attribute.</param>
        /// <param name="value">The value of this attribute.</param>
        /// <param name="delim">The delimiter character for the value.
        /// </param>
        public Attribute(string name, string value, char delim)
        {
            m_name = name;
            m_value = value;
            m_delim = delim;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// The delimiter for this attribute.
        /// </summary>
        public char Delim
        {
            get { return m_delim; }

            set { m_delim = value; }
        }

        /// <summary>
        /// The name for this attribute.
        /// </summary>
        public string Name
        {
            get { return m_name; }

            set { m_name = value; }
        }

        /// <summary>
        /// The value for this attribute.
        /// </summary>
        public string Value
        {
            get { return m_value; }

            set { m_value = value; }
        }

        #endregion Properties

        #region Methods

        public virtual object Clone()
        {
            return new Attribute(m_name, m_value, m_delim);
        }

        #endregion Methods
    }
}