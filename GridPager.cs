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

namespace WebGrid
{
    using System.ComponentModel;

    using Config;
    using Enums;

    public partial class Grid
    {
        #region Fields

        private int m_CurrentPage = 1;
        private bool m_CurrentPageset;
        private int m_PageSize = 10;
        private string m_PagerPrefix;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Sets or gets the current page for the grid. Default PageIndex is 1.
        /// </summary>
        [Bindable(true),
        Category("Navigation"),
        DefaultValue(1),
        Description(@"Sets or gets the current page for the grid. Default PageIndex is 1."),
        ]
        public int PageIndex
        {
            get
            {
                if (Equals(m_CurrentPageset, true))
                    return m_CurrentPage;

                if (ViewState["PageIndex"] != null)
                    m_CurrentPage = (int)ViewState["PageIndex"];

                if (DisplayView == DisplayView.Grid && MasterTable.RecordCount > 0 && PageSize > 0 &&
                    m_CurrentPage > ((MasterTable.RecordCount / PageSize) + 1))
                    m_CurrentPage = 1;

                m_CurrentPageset = true;
                return m_CurrentPage;
            }
            set
            {
                if (DisplayView == DisplayView.Grid)
                    ViewState["PageIndex"] = value;
                m_CurrentPage = value;
                m_CurrentPageset = true;
            }
        }

        /// <summary>
        /// Sets or gets the page size (number of records per page). Default is 10.
        /// Paging abilities for WebGrid is disabled if value "0" is passed to this property.
        /// Web.Config key for this property is "WG" + property name.
        /// </summary>
        [Bindable(true),
        Category("Navigation"),
        DefaultValue(10),
        Description(
                @"Sets or gets the page size (number of records per page). Default is 10. Paging abilities for WebGrid is disabled if value ""0"" is passed to this  property. Web.Config key for this property is ""WG"" + property name"
                ),
        ]
        public int PageSize
        {
            get
            {
                /*if (PagerSettings.PagerType == PagerType.None)
                    m_PageSize = 0;
                else */if (m_PageSize == -1)
                    m_PageSize = GridConfig.Get("WGPageSize", 10);
                return m_PageSize;
            }
            set { m_PageSize = value; }
        }

        /// <summary>
        /// The text before grid navigation. Only visible with the pager.
        /// </summary>
        [Category("Navigation"),
        DefaultValue(null),
        Description(@"The text before grid navigation. Only visible with the pager."),
        ]
        public string PagerPrefix
        {
            get
            {
                return string.IsNullOrEmpty(m_PagerPrefix) ? GetSystemMessage("PagerPreFix") : m_PagerPrefix;
            }
            set { m_PagerPrefix = value; }
        }

        #endregion Properties

        #region Methods

        internal int GetAbsoluteCurrentPage()
        {
            if (ViewState["PageIndex"] != null)
                return (int)ViewState["PageIndex"];
            return 1;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializePageSize()
        {
            return 10 != m_PageSize;
        }

        /// <summary>
        /// </summary>
        /// <exclude/>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializePagerPrefix()
        {
            return !string.IsNullOrEmpty(m_PagerPrefix);
        }

        #endregion Methods
    }
}