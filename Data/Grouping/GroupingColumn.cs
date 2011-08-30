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

    /// <summary>
    /// </summary>
    /// <exclude/>
    /// <returns></returns>
    public class GroupingColumn : IComponent
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public GroupingColumn()
        {
            IsExpanded = true;
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
        /// Sets or gets if the group IsExpanded, default is true.
        /// </summary>
        public bool IsExpanded
        {
            get; set;
        }

        /// <summary>
        /// Sets or gets the ColumnId
        /// </summary>
        public string ColumnId
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the site of the <see cref="System.ComponentModel.Component"/>
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.ComponentModel.ISite"></see> object associated with the component; or null, if the component does not have a site.</returns>
        [Browsable(false)]
        public  ISite Site
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
        public string HeaderValueSeparator
        {
            get; set;
        }

      

       

        #endregion Properties

        #region Methods

        /// <summary>
        /// Releases all resources used by the SystemMessageItem.
        /// </summary>
        public  void Dispose()
        {
           
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