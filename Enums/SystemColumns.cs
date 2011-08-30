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
    public enum SystemColumn
    {
        /// <summary>
        /// Undefined.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// The column will be shown as copy link (copy selected record and set detail view for the user).
        /// </summary>
        CopyColumn = 1,
        /// <summary>
        /// The column will be shown as edit link (edit current record).
        /// </summary>
        EditColumn = 2,

        /// <summary>
        /// The column will be shown as delete link (delete current record).
        /// </summary>
        DeleteColumn = 3,

        /// <summary>
        /// The column will be shown as new link ( create new record ).
        /// </summary>
        NewRecordColumn = 4,

        /// <summary>
        /// The column will be displayed as select column (checkbox for the row).
        /// </summary>
        SelectColumn = 5,
        /// <summary>
        /// The column will be shown as updateRecords link.
        /// </summary>
        RowColumn = 6,
        /// <summary>
        /// The column will be shown as updateRow link.
        /// </summary>
        UpdateGridRecordsColumn = 7,
        /// <summary>
        /// The column will be shown as update current record link.
        /// </summary>
        UpdateGridRecordColumn = 8,
        /// <summary>
        /// Change visibility and layout of update records link (outside WebGrid).
        /// </summary>
        UpdateRecordsLink = 9,
        /// <summary>
        /// Change visibility and layout of new record link (outside WebGrid).
        /// </summary>
        NewRecordLink = 10,
        /// <summary>
        /// Change visibility and layout of update record button (inside WebGrid).
        /// </summary>
        UpdateRecordButton = 11,
        /// <summary>
        /// Change visibility and layout of back button (inside WebGrid).
        /// </summary>
        BackButton = 12,
        /// <summary>
        /// Spacing tag is used when grid width is not percentage. (Used to enable 'WidthColumnHeaderTitle' in grid view)
        /// </summary>
        SpacingColumn = 13,
        /// <summary>
        /// Used for vertical slider columns.
        /// </summary>
        PagerColumn = 14,
    }

    #endregion Enumerations
}