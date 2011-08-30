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
    using System.Text;

    using WebGrid.Sentences;
    using WebGrid.Toolbar;

    /// <summary>
    /// A collection of the customized toolbars for this grid.
    /// </summary>
    [Serializable]
    public class ToolbarCollection : List<ToolbarItem>
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ToolbarCollection()
        {
            Visible = true;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Sets or gets if webgrid toolbars should be visible or not.
        /// </summary>
        public bool Visible
        {
            get; set;
        }

        #endregion Properties
    }
}