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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;

    #region Enumerations

    internal enum ClientEventType
    {
        OnClientColumnClick = 1,
        OnClientColumnDblClick = 2,
        OnClientColumnMouseOver = 3,
        OnClientColumnMouseOut = 4,
        OnClientRowClick = 5,
        OnClientRowDblClick = 6,
        OnClientRowMouseOver = 7,
        OnClientRowMouseOut = 8
    }

    #endregion Enumerations

    public partial class Grid
    {
        #region Fields

        internal StringBuilder JsOnData = new StringBuilder();
        internal Dictionary<string, string> MaskedColumns = new Dictionary<string, string>();

        #endregion Fields

        #region Properties

        /// <summary>
        /// Client JavaScript method associated with this event fired when a column is clicked.
        /// </summary>
        [Bindable(true),
        Category("Client Events"),
        Description(@"Client JavaScript method associated with this event fired when a column is clicked.")]
        public string OnClientCellClick
        {
            get; set;
        }

        /// <summary>
        /// Client JavaScript method associated with this event fired when a column is double clicked.
        /// </summary>
        [Bindable(true),
        Category("Client Events"),
        Description(@"Client JavaScript method associated with this event fired when a column is double clicked.")]
        public string OnClientCellDblClick
        {
            get; set;
        }

        /// <summary>
        /// Client JavaScript method associated with this event fired mouse is leaving a column.
        /// </summary>
        [Bindable(true),
        Category("Client Events"),
        Description(@"Client JavaScript method associated with this event fired mouse is leaving a column.")]
        public string OnClientCellMouseOut
        {
            get; set;
        }

        /// <summary>
        /// Client JavaScript method associated with this event fired mouse is over a column.
        /// </summary>
        [Bindable(true),
        Category("Client Events"),
        Description(@"Client JavaScript method associated with this event fired mouse is over a column.")]
        public string OnClientCellMouseOver
        {
            get; set;
        }

        /// <summary>
        /// Client JavaScript method associated with this event fired when a row is clicked.
        /// </summary>
        [Bindable(true),
        Category("Client Events"),
        Description(@"Client JavaScript method associated with this event fired when a row is clicked.")]
        public string OnClientRowClick
        {
            get; set;
        }

        /// <summary>
        /// Client JavaScript method associated with this event fired when a row is double clicked.
        /// </summary>
        [Bindable(true),
        Category("Client Events"),
        Description(@"Client JavaScript method associated with this event fired when a row is double clicked.")]
        public string OnClientRowDblClick
        {
            get; set;
        }

        /// <summary>
        /// Client JavaScript method associated with this event fired mouse is leaving a column.
        /// </summary>
        [Bindable(true),
        Category("Client Events"),
        Description(@"Client JavaScript method associated with this event fired mouse is leaving a column.")]
        public string OnClientRowMouseOut
        {
            get; set;
        }

        /// <summary>
        /// Client JavaScript method associated with this event fired mouse is over a column.
        /// </summary>
        [Bindable(true),
        Category("Client Events"),
        Description(@"Client JavaScript method associated with this event fired mouse is over a column.")]
        public string OnClientRowMouseOver
        {
            get; set;
        }

        #endregion Properties
    }

    /// <summary>
    /// Event is raised after row insert/update.
    /// </summary>
    [Serializable]
    internal class ClientCellEventArgs : EventArgs
    {
        #region Properties

        public ClientEventType ClientEventType
        {
            get; internal set;
        }

        /// <summary>
        /// Name of the column
        /// </summary>
        public string ColumnId
        {
            get; internal set;
        }

        /// <summary>
        /// Column index
        /// </summary>
        public int RowIndex
        {
            get; internal set;
        }

        /// <summary>
        /// Custom data information
        /// </summary>
        public string Tag
        {
            get; internal set;
        }

        /// <summary>
        /// Column Value
        /// </summary>
        public object Value
        {
            get; internal set;
        }

        #endregion Properties
    }

    /// <summary>
    /// Event is raised after row insert/update.
    /// </summary>
    [Serializable]
    internal class ClientRowEventArgs : EventArgs
    {
        #region Properties

        public ClientEventType ClientEventType
        {
            get; internal set;
        }

        /// <summary>
        /// Column index
        /// </summary>
        public int RowIndex
        {
            get; internal set;
        }

        /// <summary>
        /// Custom data information
        /// </summary>
        public string Tag
        {
            get; internal set;
        }

        #endregion Properties
    }
}