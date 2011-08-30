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

    using WebGrid.Enums;
    using WebGrid.Toolbar;

    /// <summary>
    /// Toolbar for detail bottom
    /// </summary>
    [Serializable]
    public class ToolbarDetailBottom : ToolbarItem
    {
        #region Constructors

        ///<summary>
        /// Default constructor
        ///</summary>
        public ToolbarDetailBottom()
        {
            // Work-around to serialize default toolbar settings in design mode
            Controls.SkinID = "wgToolbarDetailBottomreplace";
            EnableCache = true;
            ToolbarType = ToolbarType.ToolbarDetailBottom;
        }

        #endregion Constructors
    }

    /// <summary>
    /// Toolbar for detail bottom
    /// </summary>
    [Serializable]
    public class ToolbarDetailTop : ToolbarItem
    {
        #region Constructors

        ///<summary>
        /// Default constructor
        ///</summary>
        public ToolbarDetailTop()
        {
            EnableCache = true;
            ToolbarType = ToolbarType.ToolbarDetailTop;
        }

        #endregion Constructors
    }

    /// <summary>
    /// Toolbar for detail bottom
    /// </summary>
    [Serializable]
    public class ToolbarGridBottom : ToolbarItem
    {
        #region Constructors

        ///<summary>
        /// Default constructor
        ///</summary>
        public ToolbarGridBottom()
        {
            // Work-around to serialize default toolbar settings in design mode
            Controls.SkinID = "wgToolbarGridBottomreplace";
            EnableCache = true;
            ToolbarType = ToolbarType.ToolbarGridBottom;
        }

        #endregion Constructors
    }

    /// <summary>
    /// Toolbar for detail bottom
    /// </summary>
    [Serializable]
    public class ToolbarGridTop : ToolbarItem
    {
        #region Constructors

        ///<summary>
        ///</summary>
        public ToolbarGridTop()
        {
            EnableCache = true;
            ToolbarType = ToolbarType.ToolbarGridTop;
        }

        #endregion Constructors
    }
}