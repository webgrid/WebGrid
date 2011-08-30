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

namespace WebGrid.Toolbar
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    /// <summary>
    /// </summary>
    /// <exclude/>
    /// <returns></returns>
    public abstract class ToolbarItem : IComponent
    {
        #region Fields

        internal PlaceHolder m_Controls = new PlaceHolder();

        string content;

        #endregion Fields

        #region Events

        /// <summary>
        /// Adds an event handler to listen to the Disposed event on the component.
        /// </summary>
        public event EventHandler Disposed;

        #endregion Events

        #region Properties

        /// <summary>
        /// Toolbar html with WebGrid Tags.
        /// </summary>
        /// <value>The html</value>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Category("Appearance")]
        [Description(@"Toolbar controls.")]
        [NotifyParentProperty(true)]
        public PlaceHolder Controls
        {
            get
            {
                 return m_Controls;

            }
            set
            {
                m_Controls = value;

            }
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
        /// Gets or sets whenever the toolbaritem should be cached. Default is true. If active it will store the toolbars for reuse in cache.
        /// </summary>
        /// <remarks>
        /// If you are using dynamic web controls you should disable cache.
        /// </remarks>
        /// <value></value>
        internal bool EnableCache
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whenever the toolbaritem should be cached. Default is true
        /// </summary>
        /// <value>The text to be displayed</value>
        internal Enums.ToolbarType ToolbarType
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
            //There is nothing to clean.
            if (Disposed != null)
                Disposed(this, EventArgs.Empty);
        }

        internal string GetControlsContent()
        {
            if (Controls == null)
                return content;

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter writer = new HtmlTextWriter(sw);

            Controls.RenderControl(writer);
            content = sb.ToString();
            return content;
        }

        /// <summary>
        /// Indicates if the property should serialize.
        /// </summary>
        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeEnableCache()
        {
            return false;
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

        /// <summary>
        /// Indicates if the property should serialize.
        /// </summary>
        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeToolbarType()
        {
            return true;
        }

        #endregion Methods
    }
}