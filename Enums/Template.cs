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

namespace WebGrid.Enums
{
    using System.ComponentModel;

    #region Enumerations

    /// <summary>
    /// Column templates
    /// </summary>
    public enum ColumnTemplates
    {
        /// <summary>
        /// Empty and default template.
        /// </summary>
        None = 0,
        /// <summary>
        /// Select row column
        /// </summary>
        SelectRowColumn = 1,
        /// <summary>
        /// Edit row column
        /// </summary>
        EditRowColumn = 2,
        /// <summary>
        /// Delete row column
        /// </summary>
        DeleteRowColumn = 3,
        /// <summary>
        /// Copy row column
        /// </summary>
        CopyRowColumnn = 4,
        /// <summary>
        /// Update current record in grid view.
        /// </summary>
        UpdateGridRecordColumn = 5,
        /// <summary>
        /// Update all records in grid view.
        /// </summary>
        UpdateGridRecordsColumn = 6,
        /// <summary>
        /// Column with a WYSIWYG html-editor capable properties.
        /// </summary>
        HtmlEditorColumn = 7,
        /// <summary>
        /// Column that requires an valid e-mail address.
        /// </summary>
        EmailValidColumn = 8,
    }

    /// <summary>
    /// Grid templates available.
    /// </summary>
    public enum GridTemplate
    {
        /// <summary>
        /// Empty and default template.
        /// </summary>
        [Description("None")] None = 0,
        /// <summary>
        /// Grid with columns for sub total and summary.
        /// </summary>
        [Description("Row summary")] RowSummary = 1,

        /// <summary>
        /// Grid with columns arranged as product catalog style.
        /// </summary>
        [Description("Product catalog")] ProductCatalog = 2,
        /// <summary>
        /// Grid with columns arranged with user name and password
        /// </summary>
        [Description("User registration form")] UserRegistration = 3,
        /// <summary>
        /// Grid with columns arranged for documents, and title.
        /// </summary>
        [Description("Document database")] DocumentDatabase = 4,
    }

    #endregion Enumerations
}