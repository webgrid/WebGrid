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

namespace WebGrid.Sort
{
    using System;

    internal class SortColumn
    {
        #region Fields

        internal string m_ColumnName;
        internal bool m_Desc;
        internal int m_DisplayIndex;

        #endregion Fields

        #region Constructors

        internal SortColumn()
        {
            m_ColumnName = string.Empty;
            m_Desc = false;
            m_DisplayIndex = -1;
        }

        internal SortColumn(string colname)
        {
            m_ColumnName = colname;
            m_Desc = false;
            m_DisplayIndex = 0;
        }

        #endregion Constructors
    }

    internal class SortList
    {
        #region Fields

        private readonly SortColumn[] m_Columns = new SortColumn[5];

        #endregion Fields

        #region Constructors

        internal SortList(string sortExpression)
        {
            FromString(sortExpression);
        }

        #endregion Constructors

        #region Indexers

        internal SortColumn this[string x]
        {
            set
            {
                x = x.Trim();
                bool pluss = (x.IndexOf("+") > 0);
                if (pluss == false)
                    x = x.ToUpperInvariant().Replace("[",string.Empty).Replace("]",string.Empty);
                for (int i = 0; i < m_Columns.Length; i++)
                {
                    if (pluss)
                        if (String.Compare(m_Columns[i].m_ColumnName, x, false) != 0)
                            continue;
                    if (pluss == false)
                        if (m_Columns[i].m_ColumnName.ToUpperInvariant().Replace("[",string.Empty).Replace("]",string.Empty) != x)
                            continue;

                    m_Columns[i] = value;
                }
            }
            get
            {
                x = x.Trim();
                bool pluss = (x.IndexOf("+") > 0);
                if (pluss == false)
                    x = x.ToUpperInvariant().Replace("[",string.Empty).Replace("]",string.Empty);
                for (int i = 0; i < m_Columns.Length; i++)
                {
                    if (pluss)
                        if (String.Compare(m_Columns[i].m_ColumnName, x, false) != 0)
                            continue;
                    if (pluss == false)
                        if (m_Columns[i].m_ColumnName.ToUpperInvariant().Replace("[", string.Empty).Replace("]", string.Empty) != x)
                            continue;

                    return m_Columns[i];
                }
                return null;
            }
        }

        #endregion Indexers

        #region Methods

        internal void Add(string colName)
        {
            colName = colName.Trim();

            if (string.IsNullOrEmpty(colName))
                return;

            bool pluss = (colName.IndexOf("+") > 0);
            if (pluss == false)
                colName = colName.Replace("[",string.Empty).Replace("]",string.Empty);

            // first check if it exists
            if (this[colName] == null)
            {
                // first move all columns
                int theOneWithTheHighestPriority = 0;
                for (int i = 0; i < m_Columns.Length; i++)
                {
                    if (m_Columns[i].m_DisplayIndex < 0)
                    {
                        theOneWithTheHighestPriority = i;
                        continue;
                    }
                    m_Columns[i].m_DisplayIndex++;
                    if (m_Columns[theOneWithTheHighestPriority].m_DisplayIndex >= 0 &&
                        m_Columns[i].m_DisplayIndex >= m_Columns[theOneWithTheHighestPriority].m_DisplayIndex)
                        theOneWithTheHighestPriority = i;
                }
                // then insert the new one
                m_Columns[theOneWithTheHighestPriority] = new SortColumn(colName);
            }
            else // it exists
            {
                if (this[colName].m_DisplayIndex == 0) // if it's first just revert DESC
                    this[colName].m_Desc = ! this[colName].m_Desc;
                else // if not move it to the top and set desc = false
                {
                    for (int i = 0; i < m_Columns.Length; i++) // only add the ones that are over
                    {
                        if (m_Columns[i].m_DisplayIndex < 0) continue;
                        if (this[colName].m_DisplayIndex < m_Columns[i].m_DisplayIndex) continue;
                        m_Columns[i].m_DisplayIndex++;
                    }
                    this[colName].m_DisplayIndex = 0;
                    this[colName].m_Desc = false;
                }
            }
        }

   
        internal SortColumn[] ColumnsSorted()
        {
            // First sort them
            for (int i = 0; i < m_Columns.Length; i++)
                for (int ii = 0; ii < m_Columns.Length; ii++)
                {
                    if (m_Columns[i].m_DisplayIndex > m_Columns[ii].m_DisplayIndex) continue;
                    SortColumn m = m_Columns[i];
                    m_Columns[i] = m_Columns[ii];
                    m_Columns[ii] = m;
                }
            return m_Columns;
        }

        internal void FromString(string orderby)
        {
            ClearList();
            if (string.IsNullOrEmpty(orderby))
                return;
            orderby = orderby.Trim();

            // TODO: huh?!??! :-O
            char[] chOrderby = orderby.ToCharArray();
            bool isString = false;
            for (int i = 0; i < chOrderby.Length; i++)
            {
                switch (chOrderby[i])
                {
                    case '\'':
                        isString = !isString;
                        break;
                    default:
                        if (isString && chOrderby[i] == ',')
                            chOrderby[i] = '\b';
                        break;
                }
            }
            orderby = string.Empty;
            for (int i = 0; i < chOrderby.Length; i++)
                orderby += chOrderby[i];

            string[] s = orderby.Split(","[0]);
            for (int i = 0; i < s.Length; i++)
            {
                s[i] = s[i].Trim().Replace("\b", ",");
                bool pluss = (s[i].IndexOf("+") > 0);
                if (pluss == false)
                    s[i] = s[i].Replace("[",string.Empty).Replace("]",string.Empty);
                if (i >= m_Columns.Length) continue;
                m_Columns[i].m_DisplayIndex = i;
                if (s[i].ToUpperInvariant().EndsWith(" DESC"))
                {
                    m_Columns[i].m_Desc = true;
                    s[i] = s[i].Substring(0, s[i].Length - 5);
                }
                if (s[i].ToUpperInvariant().EndsWith(" ASC"))
                {
                    m_Columns[i].m_Desc = false;
                    s[i] = s[i].Substring(0, s[i].Length - 4);
                }
                m_Columns[i].m_ColumnName = s[i].Trim();
            }
        }

        internal new string ToString()
        {
            // First sort them
            for (int i = 0; i < m_Columns.Length; i++)
                for (int ii = 0; ii < m_Columns.Length; ii++)
                {
                    if (m_Columns[i].m_DisplayIndex > m_Columns[ii].m_DisplayIndex) continue;
                    SortColumn m = m_Columns[i];
                    m_Columns[i] = m_Columns[ii];
                    m_Columns[ii] = m;
                }

            // Then add them up
            string s = string.Empty;
            for (int i = 0; i < m_Columns.Length; i++)
            {
                if (m_Columns[i].m_DisplayIndex < 0) continue;

                string colname = m_Columns[i].m_ColumnName.Trim();

                if (colname.IndexOf("+") < 1)
                    colname = string.Format("[{0}]", colname.Replace(".", "].["));
                s += colname;
                if (m_Columns[i].m_Desc)
                    s += " DESC";
                s += ", ";
            }
            if (s.Length != 0) // remove last comma
                s = s.Substring(0, s.Length - 2);
            return s;
        }

        private void ClearList()
        {
            for (int i = 0; i < m_Columns.Length; i++)
                m_Columns[i] = new SortColumn();
        }

        #endregion Methods
    }
}