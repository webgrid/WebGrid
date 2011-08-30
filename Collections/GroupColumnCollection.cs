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
    public class GroupColumnCollection : List<GroupingColumn>
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public GroupColumnCollection()
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
            get;
            set;
        }

        #endregion Properties

        #region Methods

        

        #endregion Methods
    }
}