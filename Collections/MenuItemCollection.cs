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
    using System;
    using System.Collections.Generic;
 

    /// <summary>
    /// A collection of the customized menu items for this grid.
    /// </summary>
    [Serializable]
    public class MenuItemCollection : List<ContextMenuItem>
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public MenuItemCollection()
        {
            Visible = true;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Sets or gets if webgrid context menu should be visible or not.
        /// </summary>
        public bool Visible
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        ///<summary>
        /// Gets index of a system menu item
        ///</summary>
        ///<param name="type"></param>
        ///<returns></returns>
        public bool Contains(Enums.ContextMenuType type)
        {
            return FindIndex(delegate(ContextMenuItem mItem)
                                 {
                                     return mItem.InternalMenuType == type;
                                 }) != -1;
        }

        #endregion Methods
    }
}