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
    using System.Web.UI;

    using Enums;

    ///<summary>
    /// User defined Menu items for Context Menu
    ///</summary>
    public class CustomMenuItem : ContextMenuItem
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public CustomMenuItem()
        {
            InternalMenuType = ContextMenuType.Undefined;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets attribute for this Menu item. The attribute text is added to the html tag used for this menu item
        /// </summary>
        /// <remarks>
        /// There are default texts for System menu items
        /// </remarks>
        /// <value></value>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Description("Gets or sets attribute for this Menu item. The attribute text is added to the html tag used for this menu item")]
        [NotifyParentProperty(true)]
        public string Attributes
        {
            get; set;
        }

        #endregion Properties
    }
}