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
    using System.Collections.Generic;

    using WebGrid.Util;

    /// <summary>
    /// A collection of WebGrid Grids
    /// </summary>
    public class GridCollection : List<Grid>
    {
        #region Methods

        /// <summary>
        /// Gets Hyperlink for this slave-grid
        /// </summary>
        /// <param name="slaveGrid">SlaveGrid</param>
        /// <returns>returns null if grid is not found, else the hyperlink.</returns>
        public string GetSlaveGridAnchor(Grid slaveGrid)
        {
            if (!Contains(slaveGrid) || slaveGrid.MasterWebGrid == null)
                return null;
            if (slaveGrid.MasterWebGrid.ActiveMenuSlaveGrid == slaveGrid)
                return Buttons.Anchor(slaveGrid.MasterWebGrid, slaveGrid.MenuText, "SlaveGridClick",
                                      new[] { slaveGrid.ID }, null,
                                      slaveGrid.MasterWebGrid.GetSystemMessage("ClickToCloseMenu"), "wgmenulinkactive",
                                      null,true);
            return Buttons.Anchor(slaveGrid.MasterWebGrid, slaveGrid.MenuText, "SlaveGridClick", new[] { slaveGrid.ID },
                                  null,
                                  slaveGrid.MasterWebGrid.GetSystemMessage("ClickToActivateMenu"), "wgmenulinkinactive",
                                  null, true);
        }

        #endregion Methods
    }
}