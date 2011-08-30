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
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Web.UI;
    using System.Web.UI.WebControls.WebParts;

    /// <summary>
    /// GridPart web control can render parts (<see cref="WebGrid.GridPart" /> ) of <see cref="WebGrid.Grid">WebGrid.Grid</see> web control.
    /// Example of usage: Use this control if you application need to render example the "search box" that comes with WebGrid
    /// at a special place on the web page. GridPart identifies it's WebGrid.Grid object by the MasterGrid property.
    /// </summary>
    [ToolboxBitmap(typeof (WebPartZone))]
    [Description(
            @"GridPart web control can render parts of WebGrid.Grid web control outside the anywhere on the http context."
            )]
    [ToolboxData("<{0}:GridPart runat=\"server\"></{0}:GridPart>")]
    [Guid("2BE383E7-1197-446f-A353-50A9938FD9DB")]
    public class GridPart : Control
    {
        #region Fields

        internal string m_InternalHtml;

        private Enums.GridPart m_GridPartType = Enums.GridPart.Undefined;
        private string m_MasterGrid;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the HTML that this Control should render after gridpart.
        /// </summary>
        /// <value>The HTML.</value>
        [Description("Fixed html after grid Part")]
        [Category("Grid part")]
        public string AfterHtml
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the HTML that this Control should render before gridpart.
        /// </summary>
        /// <value>The HTML.</value>
        [Description("Fixed html before grid Part")]
        [Category("Grid part")]
        public string BeforeHtml
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the GridPartType, this part of WebGrid will be rendered inside this control instead.
        /// </summary>
        /// <value>The type of the tag control.</value>
        [Description("Grid part type")]
        [Category("Grid part")]
        public Enums.GridPart GridPartType
        {
            get { return m_GridPartType; }
            set { m_GridPartType = value; }
        }

        /// <summary>
        /// Gets or sets the HTML that this Control should render instead
        /// </summary>
        /// <value>The HTML.</value>
        [Description("Fixed html grid Part")]
        [Category("Grid part")]
        public string Html
        {
            get; set;
        }

        /// <summary>
        /// Sets or gets the name of the grid required to use this web control.
        /// </summary>
        /// <value>The master grid.</value>
        [Bindable(true),
        DefaultValue("")]
        [Description("Grid (ID) related to this Grid part")]
        [Category("Grid part")]
        public string MasterGrid
        {
            get
            {
                return m_MasterGrid;
                /*if(ViewState["GridIdentifier"] == null)		// ViewState er for verdier som tar lang tid  "kalkulere"/hente fra db,
                    return null;							// eller bruker-input som man ikke vil lagre i session/etc...
                return ViewState["GridIdentifier"].ToString();*/
                // MasterGrid blir satt hver gang av aspx-page-parser uansett s......
            }
            set
            {
                m_MasterGrid = value;
                //ViewState["GridIdentifier"] = value;
            }
        }

        #endregion Properties

        #region Methods

        /*
        private static void FindControls(WebGrid.Grid grid, System.Web.UI.ControlCollection controls,ref WebGrid.GridPart  tagControl, WebGrid.GridPart controlType )
        {
            if( tagControl != null )
                return;
            for(int i = 0; i < controls.Count; i++)
                if( controls[i] is WebGrid.GridPart && ((WebGrid.GridPart)controls[i]).ForeignkeyType == controlType && ((WebGrid.GridPart)controls[i]).MasterGrid == grid.ColumnId )
                {
                    tagControl =  (WebGrid.GridPart)controls[i];
                }
                else if ( controls[i].Controls.Count > 0 )
                    FindControls(grid, controls[i].Controls,ref tagControl, controlType );
        }
        */
        /// <summary>
        /// Gets true if same control type else false.
        /// </summary>
        /// <returns>This object if found, else null</returns>
        /// <param name="controlTypeSearchFor">Control type</param>
        /// <param name="grid">Grid</param>
        internal static GridPart GetControl(Grid grid, Enums.GridPart controlTypeSearchFor)
        {
            return grid.Page == null ? null : FindControls(grid.ID, grid.Page.Controls, controlTypeSearchFor);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            if (m_GridPartType == Enums.GridPart.Undefined)
                throw new ApplicationException("GridPartType is undefined");

            if (m_MasterGrid == null)
                throw new ApplicationException("MasterGrid property is uninitialized.");
            Grid masterWebGrid = Page.Parent != null ? FindGrid(Page.Parent, m_MasterGrid) : FindGrid(Page, m_MasterGrid);

            if (masterWebGrid == null)
                throw new ApplicationException(string.Format("MasterGrid for GridPart is not found. ({0})", MasterGrid));

            /*
            if( ViewState["GridIdentifier"] == null ||												// bra
                (WebGrid.Grid)Page.FindControl(ViewState["GridIdentifier"].ToString()) == null)	// ekke noe poeng  caste om til WebGrid.Grid s lenge du bare skal sjekke om den er null
                throw new Exception("Required WebGrid grid for the tagcontrol not found.");
            */

            // base.OnPreRender (e);  --- nothing to do...
        }

        /// <summary>
        /// Sends server control content to a provided <see cref="System.Web.UI.HtmlTextWriter"/> object, which writes the content to
        /// be rendered on
        /// the client.
        /// </summary>
        /// <param name="writer">The <see langword="HtmlTextWriter"/> object that receives the server control content.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (m_InternalHtml == null)
                return;
            if (BeforeHtml != null)
                writer.Write(BeforeHtml);
            writer.Write(m_InternalHtml);
            if (AfterHtml != null)
                writer.Write(AfterHtml);
        }

        private static GridPart FindControls(string masterEditorID, ControlCollection controls,
            Enums.GridPart controlTypeSearchFor)
        {
            // Search the children of this control
            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i] is GridPart == false) continue;

                GridPart found = (GridPart) controls[i];
                if (found.GridPartType != controlTypeSearchFor ||
                    String.Compare(found.MasterGrid, masterEditorID, true) != 0)
                    continue;
                return found;
            }

            // Start searching the children of the children of this control
            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i].Controls.Count == 0) continue;

                GridPart found = FindControls(masterEditorID, controls[i].Controls, controlTypeSearchFor);
                if (found != null)
                    return found;
            }

            // Oops!
            return null;
        }

        /// <summary>
        /// Finds a WebGrid.Grid object within a Control.
        /// </summary>
        /// <param name="parent">The control to look into</param>
        /// <param name="gridID">The grid to search for</param>
        /// <returns></returns>
        private static Grid FindGrid(Control parent, string gridID)
        {
            foreach (Control child in parent.Controls)
            {
                Grid grid = child as Grid;
                if (grid != null && grid.ID.Equals(gridID, StringComparison.OrdinalIgnoreCase))
                {
                    return grid;
                }
                if (!child.HasControls()) continue;
                Grid gridScope = FindGrid(child, gridID);
                if (gridScope != null)
                {
                    return gridScope;
                }
            }

            return null;
        }

        #endregion Methods
    }
}