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
    using Enums;

    ///<summary>
    /// User defined Menu items for Context Menu
    ///</summary>
    public class SystemMenuItem : ContextMenuItem
    {
        #region Fields

        private ContextMenuType m_menuType;

        #endregion Fields

        #region Constructors

        ///<summary>
        /// Default Constructor
        ///</summary>
        public SystemMenuItem()
        {
            InternalMenuType = ContextMenuType.Undefined;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets whenever the toolbaritem should be cached. Default is true
        /// </summary>
        /// <value>The text to be displayed</value>
        public virtual ContextMenuType MenuType
        {
            get { return m_menuType; }
            set
            {

                m_menuType = value;
                InternalMenuType = value;
                if (Text == null)
                    Text = value.ToString();
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Indicates if the property should serialize.
        /// </summary>
        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeMenuType()
        {
            return true;
        }

        #endregion Methods
    }
}