/*
Copyright ©  Olav Christian Botterli.

Dual licensed under the MIT or GPL Version 2 licenses.

Date: 30.08.2011, Norway.

http://www.webgrid.com
*/


/*
Copyright ©  Olav Christian Botterli. 

Dual licensed under the MIT or GPL Version 2 licenses.

Date: 30.08.2011, Norway.

http://www.webgrid.com
*/
namespace WebGrid.Enums
{
    #region Enumerations

    /// <summary>
    /// Defines what type of column should be displayed.
    /// </summary>
    public enum ColumnType
    {
        /// <summary>
        /// The UnknownColumn class is displayed as a column with a free text input box in the web interface.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// The Checkbox class is displayed as a column with a checkbox in the web interface.
        /// This class inherits from the <see cref="WebGrid.Column">WebGrid.Column</see> class.
        /// </summary>
        Checkbox = 1,
        /// <summary>
        /// The Chart class is displayed as an image (graph) in the web interface.
        /// This class inherits from the <see cref="WebGrid.Column">WebGrid.Column</see> class.
        /// The Chart column is only presentable in grid view, and uses rowspan equal to
        /// the number of records visibility on the screen.
        /// </summary>
        Chart = 2,

        /// <summary>
        /// The DateTime class is displayed as a column with a free text input box in the web interface.
        /// Normally a calendar icon is shown next to the input box (if the column is editable).
        /// This class inherits from the <see cref="WebGrid.Column">WebGrid.Column</see> class.
        /// </summary>
        DateTime = 3,

        /// <summary>
        /// The Decimal class is displayed as a column with a free text inputbox in the web interface.
        /// This column automatically validates all user input and allows most float values.
        /// This class inherits from the <see cref="WebGrid.Column">WebGrid.Column</see> class.
        /// </summary>
        Decimal = 4,

        /// <summary>
        /// The Foreign key class is displayed as a column with a dropdown box by default (radio buttons is also available).
        /// With huge data-load for the Foreign key column this can be rendered as a search-box where the data is being cached.
        /// This class inherits from the <see cref="WebGrid.Column">WebGrid.Column</see> class.
        /// </summary>
        Foreignkey = 5,

        /// <summary>
        /// The Image class is displayed as a column with a file inputbox in the web interface.
        /// If the column contains a file it can be displayed as an image or a hyperlink.
        /// The file can either be stored on disk or in the data source (if available).
        /// This class inherits from the <see cref="WebGrid.Column">WebGrid.Column</see> class.
        /// </summary>
        File = 6,

        /// <summary>
        /// The Image class is displayed as a column with a file inputbox in the web interface.
        /// If the column contains a file it can be displayed as an image or a hyperlink.
        /// The file can either be stored on disk or in the data source (if available).
        /// This class inherits from the <see cref="WebGrid.Column">WebGrid.Column</see> class.
        /// </summary>
        Image  = 7,
        /// <summary>
        /// The Integer class is displayed as a column with a free text inputbox in the web interface.
        /// This column automatically validates all user input and allows most numerical values.
        /// This class inherits from the <see cref="WebGrid.Column">WebGrid.Column</see> class.
        /// </summary>
        Number = 8,

        /// <summary>
        /// The ManyToMany class is displayed as a column with two or more check boxes in the web interface.
        /// Many-to-many relationships occur where, for each instance of table A, there are many instances of table B,
        /// and for each instance of table B, there are many instances of the table A. For example, a poetry anthology
        /// can have many authors, and each author can appear in many poetry anthologies.
        /// This class inherits from the <see cref="WebGrid.Column">WebGrid.Column</see> class.
        /// </summary>
        ManyToMany = 9,

        /// <summary>
        /// The SystemColumn class is displayed as a column with various layout in the web interface.
        /// The SystemColumns are columns used by WebGrid. The most common columns are "edit record" and "delete record" columns.
        /// An Enumeration list is available at <see cref="WebGrid.Enums.SystemColumn">WebGrid.Enums.SystemColumn</see>.
        /// This class inherits from the <see cref="WebGrid.Column">WebGrid.Column</see> class.
        /// </summary>
        SystemColumn = 10,

        /// <summary>
        /// The Text class is displayed as a column with a free text inputbox in the web interface.
        /// This class inherits from the <see cref="WebGrid.Column">WebGrid.Column</see> class.
        /// </summary>
        Text = 11,
        /// <summary>
        /// The GridColumn class is displayed as Grid in the column space. The Grid becomes automatically
        /// a slavegrid for this grid, whoever it is not displayed in the slave grid menu. This column type is
        /// only supported in detail view (Detail mode for a record)
        /// This class inherits from the <see cref="WebGrid.Column">WebGrid.Column</see> class.
        /// </summary>
        GridColumn = 12,
        /// <summary>
        /// The ColumnTemplate class  is for  third-part ASP.NET controls and integrate them into WebGrid,
        /// this column type is at the moment only supported in detail view (Detail mode for a record)
        /// This class inherits from the <see cref="WebGrid.Column">WebGrid.Column</see> class.
        /// </summary>
        ColumnTemplate = 13,
        /// <summary>
        /// The Text class is displayed as link to WebGrid tooltip.
        /// This class inherits from the <see cref="WebGrid.Column">WebGrid.Column</see> class.
        /// </summary>
        ToolTip = 14
    }

    #endregion Enumerations
}