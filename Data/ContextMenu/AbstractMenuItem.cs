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
    using System;
    using System.ComponentModel;
    using System.Web.UI;
    using System.Windows.Forms;

    using Enums;

    /// <summary>
    /// </summary>
    /// <exclude/>
    /// <returns></returns>
    public abstract class ContextMenuItem : IComponent
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        protected ContextMenuItem()
        {
            Visible = true;
            Enabled = true;
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Adds an event handler to listen to the Disposed event on the component.
        /// </summary>
        public event EventHandler Disposed;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets css class for this sentence.
        /// </summary>
        /// <value>The text to be displayed</value>
        [CssClassProperty]
        public string CssClass
        {
            get; set;
        }

        /// <summary>
        /// Sets or gets if this menu item is enabled or not.
        /// </summary>
        public bool Enabled
        {
            get; set;
        }

        /// <summary>
        /// Sets or gets the ID of this Menu item
        /// </summary>
        public string Id
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the site of the <see cref="System.ComponentModel.Component"/>
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.ComponentModel.ISite"></see> object associated with the component; or null, if the component does not have a site.</returns>
        [Browsable(false)]
        public virtual ISite Site
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the text that is to be displayed for this menu Item
        /// </summary>
        /// <remarks>
        /// There are default texts for System menu items
        /// </remarks>
        /// <value></value>
        public string Text
        {
            get; set;
        }

        /// <summary>
        /// Sets or gets if this menu item is visible or not.
        /// </summary>
        public bool Visible
        {
            get; set;
        }

        /// <summary>
        /// Sets or gets if this menu item is enabled or not.
        /// </summary>
        internal ContextMenuType InternalMenuType
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Releases all resources used by the SystemMessageItem.
        /// </summary>
        public virtual void Dispose()
        {
            ContextMenu t = new ContextMenu();

            //There is nothing to clean.
            if (Disposed != null)
                Disposed(this, EventArgs.Empty);
        }

        /// <summary>
        /// Indicates if the property should serialize.
        /// </summary>
        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeSite()
        {
            return false;
        }

        #endregion Methods
    }
}