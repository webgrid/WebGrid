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

namespace WebGrid.Collections
{
    using System.Collections.Generic;

    /// <summary>
    /// Collection class used for Many-To-Many columns
    /// </summary>
    public class ManyToManyCollection : List<ManyToManyItem>
    {
        #region Indexers

        /// <summary>
        /// Sets or gets a Many-to-Many item.
        /// </summary>
        public ManyToManyItem this[string iD]
        {
            get
            {
                int index = FindByID(iD);
                return index == -1 ? null : this[index];
            }
            set
            {
                int index = FindByID(value.Value);
                if (index == -1)
                {
                    Add(value);
                }
                else
                {
                    this[index] = value;
                }
            }
        }

        #endregion Indexers

        #region Methods

        internal int FindByID(string iD)
        {
            if (Count == 0) return -1;
            for (int i = 0; i < Count; i++)
            {
                if (base[i].Value == iD)
                    return i;
            }
            return -1;
        }

        #endregion Methods
    }

    /// <summary>
    /// Collection class used for LoadItems within a Many-To-Many column
    /// </summary>
    public class ManyToManyItem
    {
        #region Fields

        private bool m_Checked;
        private string m_DisplayText;
        private string m_ParentId;
        private string m_Value;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ManyToManyItem"/> is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        public bool Checked
        {
            set { m_Checked = value; }
            get { return m_Checked; }
        }

        /// <summary>
        /// Gets or sets the displayText for this item in the many-to-many column
        /// </summary>
        /// <value>The value.</value>
        public string DisplayText
        {
            set { m_DisplayText = value; }
            get { return m_DisplayText; }
        }

        /// <summary>
        /// Gets or sets the parent id. Parent id is used for tree structure in Many-To-Many columns
        /// </summary>
        /// <value>The parent id.</value>
        public string ParentId
        {
            set { m_ParentId = value; }
            get { return m_ParentId; }
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public string Value
        {
            set { m_Value = value; }
            get { return m_Value; }
        }

        #endregion Properties
    }
}