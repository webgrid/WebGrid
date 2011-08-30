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
    using WebGrid.Enums;
    using WebGrid.Sentences;

    /// <summary>
    /// Class used to add customized system messages for the grid.
    /// </summary>
    public class NewSystemMessage : SystemMessageItem
    {
        #region Properties

        /// <summary>
        /// Gets or sets the SystemMessageID to identify the system message
        /// </summary>
        /// <value>The name.</value>
        public string SystemMessageID
        {
            get; set;
        }

        #endregion Properties
    }

    /// <summary>
    /// Class used to change customized system messages for the grid.
    /// </summary>
    public class OverrideSystemMessage : SystemMessageItem
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public SystemMessageAlias SystemMessageID
        {
            get; set;
        }

        #endregion Properties
    }
}