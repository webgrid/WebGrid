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

namespace WebGrid.Design
{
    using System;
    using System.ComponentModel;

    internal class GridModeSettings : IComponent
    {
        #region Fields

        private readonly Grid m_GridSettingsGrid;

        #endregion Fields

        #region Constructors

        public GridModeSettings()
        {
        }

        public GridModeSettings(Grid grid)
        {
            m_GridSettingsGrid = grid;
        }

        #endregion Constructors

        #region Events

        public event EventHandler Disposed;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets the site of the <see cref='System.ComponentModel.Component'/>
        /// </summary>
        [Browsable(false)]
        public virtual ISite Site
        {
            get { return m_GridSettingsGrid.Site; }
            set { m_GridSettingsGrid.Site = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Releases all resources used by the column.
        /// </summary>
        public virtual void Dispose()
        {
            //There is nothing to clean.
            if (Disposed != null)
                Disposed(this, EventArgs.Empty);
        }

        #endregion Methods
    }
}