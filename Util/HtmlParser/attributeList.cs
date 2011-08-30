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
    using System.Collections;

    /// <summary>
    /// The AttributeList class is used to store list of
    /// Attributes classes.
    /// This source code may be used freely under the
    /// Limited GNU Public License(LGPL).
    ///
    /// Written by Jeff Heaton (http://www.jeffheaton.com)
    /// </summary>
    ///
    internal class AttributeList : Attribute
    {
        #region Fields

        /// <summary>
        /// An internally used Vector.  This vector contains
        /// the entire list of attributes.
        /// </summary>
        protected ArrayList m_list;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Create a new, empty, attribute list.
        /// </summary>
        public AttributeList()
            : base(string.Empty,string.Empty)
        {
            m_list = new ArrayList();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// How many attributes are in this AttributeList?
        /// </summary>
        public int Count
        {
            get { return m_list.Count; }
        }

        /// <summary>
        /// A list of the attributes in this AttributeList
        /// </summary>
        public ArrayList List
        {
            get { return m_list; }
        }

        #endregion Properties

        #region Indexers

        /// <summary>
        /// Access the individual attributes
        /// </summary>
        public Attribute this[int index]
        {
            get { return index < m_list.Count ? (Attribute) m_list[index] : null; }
        }

        /// <summary>
        /// Access the individual attributes by name.
        /// </summary>
        public Attribute this[string index]
        {
            get
            {
                int i = 0;

                while (this[i] != null)
                {
                    if (this[i].Name.ToLowerInvariant() .Equals((index.ToLowerInvariant() )))
                        return this[i];
                    i++;
                }

                return null;
            }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Add the specified attribute to the list of attributes.
        /// </summary>
        /// <param name="a">An attribute to add to this
        /// AttributeList.</param>
        public void Add(Attribute a)
        {
            m_list.Add(a);
        }

        /// <summary>
        /// Clear all attributes from this AttributeList and return
        /// it to a empty state.
        /// </summary>
        public void Clear()
        {
            m_list.Clear();
        }

        /// <summary>
        /// Make an exact copy of this object using the cloneable
        /// interface.
        /// </summary>
        /// <returns>A new object that is a clone of the specified
        /// object.</returns>
        public override Object Clone()
        {
            AttributeList rtn = new AttributeList();

            for (int i = 0; i < m_list.Count; i++)
                rtn.Add((Attribute) this[i].Clone());

            return rtn;
        }

        /// <summary>
        /// Returns true of this AttributeList is empty, with no
        /// attributes.
        /// </summary>
        /// <returns>True if this AttributeList is empty, false
        /// otherwise.</returns>
        public bool IsEmpty()
        {
            return (m_list.Count <= 0);
        }

        /// <summary>
        /// If there is already an attribute with the specified name,
        /// it will have its value changed to match the specified
        /// value. If there is no Attributes with the specified name,
        /// one will be created. This method is case-insensitive.
        /// </summary>
        /// <param name="name">The name of the Attributes to edit or
        /// create.  Case-insensitive.</param>
        /// <param name="value">The value to be held in this
        /// attribute.</param>
        public void Set(string name, string value)
        {
            if (name == null)
                return;
            if (value == null)
                value = string.Empty;

            Attribute a = this[name];

            if (a == null)
            {
                a = new Attribute(name, value);
                Add(a);
            }

            else
                a.Value = value;
        }

        #endregion Methods
    }
}