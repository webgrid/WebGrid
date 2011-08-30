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
    using System.Data;

    using Collections;
    using Data;

    namespace Events
    {
        /// <summary>
        /// This <c>namespace</c> contains the predefined events available for <see cref="WebGrid.Grid">WebGrid.Grid</see> web control.
        /// <see cref="WebGrid"/> contains two groups of events. You have click-events that are triggered from the web page
        /// and data events that are triggered by <see cref="WebGrid"/> data-handling.
        /// </summary>
        internal class NamespaceDoc
        {
        }

        #region Click events

        /// <summary>
        /// Event is raised when copy row is clicked.
        /// </summary>
        public delegate void CopyRowClickEventHandler(object sender, CopyRowEventArgs e);

        /// <summary>
        /// Event is raised when a row in the grid is clicked.
        /// </summary>
        /// <remarks>
        /// This event is only available if the row highlight property is true.
        /// </remarks>
        public delegate void RowClickEventHandler(object sender, RowClickEventArgs e);

        /// <summary>
        /// Event is raised when a header in the grid is clicked.
        /// </summary>
        public delegate void HeaderClickEventHandler(object sender, HeaderClickEventArgs e);

        /// <summary>
        /// Event is raised when the grid's pager is clicked.
        /// </summary>
        public delegate void PagerClickEventHandler(object sender, PagerClickEventArgs e);

        /// <summary>
        /// Event is raised when a search string is submitted.
        /// </summary>
        public delegate void SearchChangedEventHandler(object sender, SearchChangedEventArgs e);

        /// <summary>
        /// Event is raised when delete button is clicked.
        /// </summary>
        public delegate void DeleteClickEventHandler(object sender, RowClickEventArgs e);

        /// <summary>
        /// Event is raised when cancel button is clicked.
        /// </summary>
        public delegate void CancelClickEventHandler(object sender, CancelClickEventArgs e);

        /// <summary>
        /// Event is raised when update button is clicked.
        /// </summary>
        public delegate void UpdateClickEventHandler(object sender, UpdateClickEventArgs e);

        /// <summary>
        /// Event is raised when new record button is clicked.
        /// </summary>
        public delegate void NewRecordClickEventHandler(object sender, NewRecordClickEventArgs e);

        /// <summary>
        /// Event is raised when update rows button is clicked.
        /// </summary>
        public delegate void UpdateRowsClickEventHandler(object sender, UpdateRowsClickEventArgs e);

        /// <summary>
        /// Event is raised when update row button is clicked.
        /// </summary>
        public delegate void UpdateRowClickEventHandler(object sender, UpdateRowClickEventArgs e);

        /// <summary>
        /// Event is raised when post back column is processed.
        /// </summary>
        public delegate void ColumnPostBackEventHandler(object sender, ColumnPostBackEventArgs e);

        /// <summary>
        /// Event is raised when post back grid is processed.
        /// </summary>
        public delegate void GridPostBackEventHandler(object sender, GridPostBackEventArgs e);

        /// <summary>
        /// Event is raised when excel export is processed.
        /// </summary>
        public delegate void ExportClickEventHandler(object sender, ExcelExportClickEventArgs e);

        #endregion Click events

        #region Click EventHandlers

        /// <summary>
        /// Event is raised when clicked.
        /// </summary>
        public class RowClickEventArgs : EventArgs
        {
            /// <summary>
            /// Indicates whether the event is successful.
            /// </summary>
            public bool AcceptChanges
            {
                get { return m_AcceptChanges; }
                set { m_AcceptChanges = value; }
            }

            /// <summary>
            /// ColumnId of the column being clicked.
            /// </summary>
            public string ColumnName
            {
                get; internal set;
            }

            /// <summary>
            /// Unique identifier in the data source.
            /// </summary>
            public string EditIndex
            {
                get; internal set;
            }

            /// <summary>
            /// Returns the row you clicked on.
            /// </summary>
            public Row Row
            {
                get; internal set;
            }

            private bool m_AcceptChanges = true;
        }

        /// <summary>
        /// Event is raised when users updates WebGrid in grid view.
        /// </summary>
        public class UpdateRowsClickEventArgs : EventArgs
        {
            /// <summary>
            /// Indicates whether the event is successful.
            /// </summary>
            public bool AcceptChanges
            {
                get { return m_AcceptChanges; }
                set { m_AcceptChanges = value; }
            }

            private bool m_AcceptChanges = true;
        }

        /// <summary>
        /// Event is raised when users update a row in grid view or detail view.
        /// </summary>
        public class UpdateRowClickEventArgs : EventArgs
        {
            // 16.10.2004, Jorn

            /// <summary>
            /// Indicates whether the event is successful.
            /// </summary>
            public bool AcceptChanges
            {
                get { return m_AcceptChanges; }
                set { m_AcceptChanges = value; }
            }

            /// <summary>
            /// Unique identifier in the data source.
            /// </summary>
            public string EditIndex
            {
                get; internal set;
            }

            private bool m_AcceptChanges = true;
        }

        /// <summary>
        /// Event is raised when users before the user exports excel from WebGrid.
        /// </summary>
        public class ExcelExportClickEventArgs : EventArgs
        {
            /// <summary>
            /// Indicates whether the event is successful.
            /// </summary>
            public bool AcceptChanges
            {
                get { return m_AcceptChanges; }
                set { m_AcceptChanges = value; }
            }

            /// <summary>
            /// Default filename for the Excel document being exported.
            /// </summary>
            public string FileName
            {
                get; set;
            }

            private bool m_AcceptChanges = true;
        }

        /// <summary>
        /// Event raised when Update button is clicked in detail view.
        /// </summary>
        public class UpdateClickEventArgs : EventArgs
        {
            /// <summary>
            /// Indicates whether the event is successful.
            /// </summary>
            public bool AcceptChanges
            {
                get { return m_AcceptChanges; }
                set { m_AcceptChanges = value; }
            }

            /// <summary>
            /// Indicates whether the row should be copied or edited.
            /// </summary>
            public bool Copy
            {
                get; set;
            }

            /// <summary>
            /// Unique identifier in the data source.
            /// </summary>
            public string EditIndex
            {
                get; internal set;
            }

            private bool m_AcceptChanges = true;
        }

        /// <summary>
        /// Event raised when Cancel button in detail view is clicked.
        /// </summary>
        public class CancelClickEventArgs : EventArgs
        {
            /// <summary>
            /// Indicates whether the event is successful.
            /// </summary>
            public bool AcceptChanges
            {
                get { return m_AcceptChanges; }
                set { m_AcceptChanges = value; }
            }

            /// <summary>
            /// Unique identifier in the data source.
            /// </summary>
            public string EditIndex
            {
                get; internal set;
            }

            private bool m_AcceptChanges = true;
        }

        /// <summary>
        /// Event raised when New Record is clicked in grid view.
        /// </summary>
        public class NewRecordClickEventArgs : EventArgs
        {
            /// <summary>
            /// Indicates whether the event is successful.
            /// </summary>
            public bool AcceptChanges
            {
                get { return m_AcceptChanges; }
                set { m_AcceptChanges = value; }
            }

            private bool m_AcceptChanges = true;
        }

        /// <summary>
        /// Event is raised when Header in grid view is clicked.
        /// </summary>
        public class HeaderClickEventArgs : EventArgs
        {
            /// <summary>
            /// Indicates whether the event is successful.
            /// </summary>
            public bool AcceptChanges
            {
                get { return m_AcceptChanges; }
                set { m_AcceptChanges = value; }
            }

            /// <summary>
            /// ColumnId of the column being clicked.
            /// </summary>
            public string ColumnName
            {
                get; internal set;
            }

            private bool m_AcceptChanges = true;
        }

        /// <summary>
        /// Event is raised when column do post back.
        /// </summary>
        public class GridPostBackEventArgs : EventArgs
        {
            /// <summary>
            /// Get event arguments.
            /// </summary>
            public string[] GetEventArguments()
            {
                return m_EventArguments;
            }

            /// <summary>
            /// Set events arguments
            /// </summary>
            /// <param name="value"></param>
            public void SetEventArguments(string[] value)
            {
                m_EventArguments = value;
            }

            /// <summary>
            /// Unique identifier in the data source.
            /// </summary>
            public string EventName
            {
                get; internal set;
            }

            private string[] m_EventArguments;
        }

        /// <summary>
        /// Event is raised when column do post back.
        /// </summary>
        public class ColumnPostBackEventArgs : EventArgs
        {
            /// <summary>
            /// ColumnId of the column being clicked.
            /// </summary>
            public string ColumnName
            {
                get; internal set;
            }

            /// <summary>
            /// Unique identifier in the data source.
            /// </summary>
            public string EditIndex
            {
                get; internal set;
            }

            /// <summary>
            /// Current Row containing post-back data.
            /// </summary>
            public Row Row
            {
                get; internal set;
            }
        }

        /// <summary>
        /// Event is raised when search text has changed.
        /// </summary>
        public class SearchChangedEventArgs : EventArgs
        {
            /// <summary>
            /// The previous search string.
            /// </summary>
            public string OldSearchString
            {
                get; internal set;
            }

            /// <summary>
            /// The current (active) search string.
            /// </summary>
            public string SearchString
            {
                get; set;
            }
        }

        /// <summary>
        /// Event is raised when pager is clicked.
        /// </summary>
        public class PagerClickEventArgs : EventArgs
        {
            /// <summary>
            /// Indicates whether the event is successful.
            /// </summary>
            public bool AcceptChanges
            {
                get { return m_AcceptChanges; }
                set { m_AcceptChanges = value; }
            }

            /// <summary>
            /// The current (active) page number.
            /// </summary>
            public int NewCurrentPage
            {
                get; set;
            }

            /// <summary>
            /// The previous page number.
            /// </summary>
            public int OldCurrentPage
            {
                get; internal set;
            }

            private bool m_AcceptChanges = true;
        }

        #endregion Click EventHandlers

        #region Data Events

        /// <summary>
        /// Event is raised before a record is updated or inserted in the data source.
        /// </summary>
        public delegate void BeforeUpdateInsertEventHandler(object sender, BeforeUpdateInsertEventArgs e);

        /// <summary>
        /// Event is raised before the column is validated.
        /// </summary>
        public delegate void BeforeValidateEventHandler(object sender, BeforeValidateEventArgs e);

        /// <summary>
        /// Event is executed once the grid or a cell is asking for data.
        /// </summary>
        public delegate void NeedDataHandler(object sender, BeforeGetDataEventArgs e);

        /// <summary>
        /// Event is raised for each row in grid view.
        /// </summary>
        public delegate void GridRowBoundEventHandler(object sender, GridRowBoundEventArgs e);


        /// <summary>
        /// Event is raised for each row in grid view.
        /// </summary>
        public delegate void RowGroupingEventHandler(object sender, RowGroupingEventArgs e);

        

        /// <summary>
        /// Event is raised after a record is updated or inserted in the data source.
        /// </summary>
        public delegate void AfterUpdateInsertEventHandler(object sender, AfterUpdateInsertEventArgs e);

        /// <summary>
        /// Event is raised after a record is deleted from the data source.
        /// </summary>
        public delegate void AfterDeleteEventHandler(object sender, AfterDeleteEventArgs e);

        /// <summary>
        /// Event is raised before a record is deleted from the data source.
        /// </summary>
        public delegate void BeforeDeleteEventHandler(object sender, BeforeDeleteEventArgs e);

        /// <summary>
        /// Event is raised when grid switches from grid to detail view.
        /// </summary>
        public delegate void EditRowEventHandler(object sender, EditRowEventArgs e);

        /// <summary>
        /// Event is raised before a row is being rendered.
        /// </summary>
        public delegate void BeforeRowRenderHandler(object sender, BeforeRenderRowArgs e);

        /// <summary>
        /// Event is raised when grid switches from grid to detail view.
        /// </summary>
        public delegate void CopyRowEventHandler(object sender, CopyRowEventArgs e);

        /// <summary>
        /// Event is raised when the grid is raised when complete.
        /// </summary>
        public delegate void DoneEventHandler(object sender);

        /// <summary>
        /// Event is raised before the rows are rendered.
        /// </summary>
        public delegate string BeforeRowsEventHandler(object sender);

        /// <summary>
        /// Event is raised after the rows are rendered.
        /// </summary>
        public delegate string AfterRowsEventHandler(object sender);

        #endregion Data Events

        #region Data Event Handlers

        /// <summary>
        /// Event is raised after row delete.
        /// </summary>
        public class AfterDeleteEventArgs : EventArgs
        {
            /// <summary>
            /// Unique identifier in the data source.
            /// </summary>
            public string EditIndex
            {
                get; internal set;
            }
        }

        /// <summary>
        /// Event is raised before the row is validated.
        /// </summary>
        public class BeforeValidateEventArgs : EventArgs
        {
            /// <summary>
            /// Unique identifier in the data source.
            /// </summary>
            public string EditIndex
            {
                get; internal set;
            }

            /// <summary>
            /// The row that is about to be validated.
            /// </summary>
            public Row Row
            {
                get; internal set;
            }
        }

        /// <summary>
        /// Event is raised when the grid or grid cells need data.
        /// </summary>
        public class BeforeGetDataEventArgs : EventArgs
        {
            ///<summary>
            /// Default constructor
            ///</summary>
            public BeforeGetDataEventArgs()
            {
                ForceGetData = false;
                GetAllRows = false;
            }

            /// <summary>
            /// Gets or sets if we should accept the changes, default is false
            /// </summary>
            public bool ForceGetData
            {
                get; set;
            }

            /// <summary>
            /// Gets or sets if we should load all rows from the data source, default is false
            /// </summary>
            public bool GetAllRows
            {
                get; set;
            }

            /// <summary>
            /// The data source requesting data.
            /// </summary>
            public Table Table
            {
                get; internal set;
            }
        }

        
         /// <summary>
        /// Event is raised when a column is grouping
        /// </summary>
        public class RowGroupingEventArgs : EventArgs
        {
            private bool m_AcceptChanges = true;

            /// <summary>
            /// Indiciates if the row should be added to the grid's Rows collection. Default is true
            /// </summary>
            public bool AcceptChanges
            {
                get { return m_AcceptChanges; }
                set { m_AcceptChanges = value; }
            }

            /// <summary>
            /// Gets the column that is being grouped.
            /// </summary>
            public string ColumnId
            {
                get;
                internal set;
            }

            /// <summary>
            /// Unique identifier in the data source.
            /// </summary>
            public string Description
            {
                get;
                set;
            }
            /// <summary>
            /// The current row.
            /// </summary>
            public Row GroupingRow
            {
                get;
                internal set;
            }
            /*
            RowCollection m_RowsInGroup = null;
            ///<summary>
            /// Gets the rows in the group
            ///</summary>
            public RowCollection RowsInGroup
            {
                get
                {
                    if (m_RowsInGroup != null)
                        return m_RowsInGroup;
                    if (GroupIdentifier != null && string.IsNullOrEmpty(GroupIdentifier.ToString()))
                    {
                        

                        return m_RowsInGroup;
                    }
                    return null;
                }

            }
            */


            /// <summary>
            /// Sets or gets if the group is expanded
            /// </summary>
            public bool IsExpanded
            {
                get;
                set;
            }

            /// <summary>
            /// Gets the identifier used to create this group.
            /// </summary>
            public object GroupIdentifier
            {
                get;
                internal set;
            }
        }


        /// <summary>
        /// Event is raised for each row in grid view.
        /// </summary>
        public class GridRowBoundEventArgs : EventArgs
        {
            private bool m_AcceptChanges = true;

            /// <summary>
            /// Indiciates if the row should be added to the grid's Rows collection. Default is true
            /// </summary>
            public bool AcceptChanges
            {
                get { return m_AcceptChanges; }
                set { m_AcceptChanges = value; }
            }

            /// <summary>
            /// Unique identifier in the data source.
            /// </summary>
            public string EditIndex
            {
                get; internal set;
            }

            /// <summary>
            /// The current row.
            /// </summary>
            public Row Row
            {
                get; internal set;
            }
        }

        /// <summary>
        /// Event is raised after row insert/update.
        /// </summary>
        public class AfterUpdateInsertEventArgs : EventArgs
        {
            /// <summary>
            /// All the columns.
            /// </summary>
            public Row Row
            {
                get; internal set;
            }

            /// <summary>
            /// Unique identifier in the data source.
            /// </summary>
            public string EditIndex
            {
                get;  internal set;
            }

            /// <summary>
            /// Gets the data row.
            /// </summary>
            /// <value>The data row.</value>
            public DataRow DataRow
            {
                get { return m_Datarow; }
            }

            /// <summary>
            /// Indicates whether the data source is inserting a new record.
            /// </summary>
            public bool Insert
            {
                get; internal set;
            }

            /// <summary>
            /// Indicates whether the data source is updating a record.
            /// </summary>
            public bool Update
            {
                get; internal set;
            }

            internal DataRow m_Datarow;
        }

        /// <summary>
        /// Event is raised before row delete.
        /// </summary>
        public class BeforeDeleteEventArgs : EventArgs
        {
            /// <summary>
            /// Indicates whether the event is successful.
            /// </summary>
            public bool AcceptChanges
            {
                get { return m_AcceptChanges; }
                set { m_AcceptChanges = value; }
            }

            /// <summary>
            /// The row being deleted.
            /// </summary>
            public Row Row
            {
                get; internal set;
            }

            /// <summary>
            /// Unique identifier in the data source.
            /// </summary>
            public string EditIndex
            {
                get; internal set;
            }

            private bool m_AcceptChanges = true;
        }

        /// <summary>
        /// Event is raised when edit row button is clicked.
        /// </summary>
        public class EditRowEventArgs : EventArgs
        {
            /// <summary>
            /// Indicates whether the event is successful.
            /// </summary>
            public bool AcceptChanges
            {
                get { return m_AcceptChanges; }
                set { m_AcceptChanges = value; }
            }

            /// <summary>
            /// Unique identifier in the data source.
            /// </summary>
            public string EditIndex
            {
                get; internal set;
            }

            /// <summary>
            /// The row being edited.
            /// </summary>
            public Row Row
            {
                get; internal set;
            }

            private bool m_AcceptChanges = true;
        }

        /// <summary>
        /// Raised before a row is being rendered.
        /// </summary>
        public class BeforeRenderRowArgs : EventArgs
        {
            private bool m_AcceptChanges = true;

            /// <summary>
            /// Indicates if we should apply changes for this row.
            /// </summary>
            public bool AcceptChanges
            {
                get { return m_AcceptChanges; }
                set { m_AcceptChanges = value; }
            }

            private bool m_RenderThisRow = true;

            /// <summary>
            /// Indiciates if the row should be rendered. Default is true
            /// </summary>
            public bool RenderThisRow
            {
                get { return m_RenderThisRow; }
                set { m_RenderThisRow = value; }
            }

            /// <summary>
            /// Unique identifier in the data source.
            /// </summary>
            public string EditIndex
            {
                get; internal set;
            }

            /// <summary>
            /// The row being edited.
            /// </summary>
            public Row Row
            {
                get; internal set;
            }

            /// <summary>
            /// Columns settings for this row. (It's a copy of the grid's columns)
            /// </summary>
            public ColumnCollection Columns
            {
                get; internal set;
            }
        }

        /// <summary>
        /// Event is raised when copy row button is clicked.
        /// </summary>
        public class CopyRowEventArgs : EventArgs
        {
            /// <summary>
            /// Indicates whether the event is successful.
            /// </summary>
            public bool AcceptChanges
            {
                get { return m_AcceptChanges; }
                set { m_AcceptChanges = value; }
            }

            /// <summary>
            /// Unique identifier in the data source.
            /// </summary>
            public string CopiedId
            {
                get; internal set;
            }

            /// <summary>
            /// The row being edited.
            /// </summary>
            public Row Row
            {
                get; internal set;
            }

            private bool m_AcceptChanges = true;
        }

        #endregion Data Event Handlers

        #region Special ColumnUpdateInsert EventArgs

        /// <summary>
        /// Event is raised when row is updated or inserted.
        /// </summary>
        public class CellUpdateInsertArgument
        {
            /// <summary>
            /// Indicates if this update else insert.
            /// </summary>
            public bool Update
            {
                get { return m_Update; }
            }

            /// <summary>
            /// Indicates whether there should be automatically added apostrophe's for the column while inserting or updating data.
            /// </summary>
            public bool AddApostrophe
            {
                get; internal set;
            }

            /// <summary>
            /// The column being updated/inserted.
            /// </summary>
            public Column Column
            {
                get; internal set;
            }

            /// <summary>
            /// The value from the data source.
            /// </summary>
            public object DataSourceValue
            {
                get; internal set;
            }

            /// <summary>
            /// Indicates whether the event is successful.
            /// </summary>
            public bool IgnoreChanges
            {
                get; internal set;
            }

            /// <summary>
            /// ColumnId of the column being updated/inserted in the data source.
            /// </summary>
            public string Name
            {
                get; internal set;
            }

            /// <summary>
            /// Parameters used by the events to identify the data.
            /// </summary>
            public Object Parameter
            {
                get; set;
            }

            /// <summary>
            /// The actual value in the cell
            /// </summary>
            public object Value
            {
                get; set;
            }

            /// <summary>
            /// Postback value for the cell (not validated yet)
            /// </summary>
            public string PostBackValue
            {
                get; set;
            }

            internal bool m_Insert;
            internal bool m_Update;

            /// <summary>
            /// Event is raised when row is updated or inserted.
            /// </summary>
            public CellUpdateInsertArgument()
            {
                AddApostrophe = true;
            }
        }

        /// <summary>
        /// Collections of columns used as argument in update/insert event.
        /// </summary>
        public class BeforeUpdateInsertEventArgs
        {
            private readonly Dictionary<string, CellUpdateInsertArgument> m_Row = new Dictionary<string, CellUpdateInsertArgument>();

            /// <summary>
            /// Indicates whether the event is successful.
            /// </summary>
            public bool AcceptChanges
            {
                get { return m_AcceptChanges; }
                set { m_AcceptChanges = value; }
            }

            /// <summary>
            /// Unique identifier in the data source.
            /// </summary>
            public string EditIndex
            {
                get { return m_EditIndex; }
            }

            /// <summary>
            /// Gets if column is inserted.
            /// </summary>
            public bool Insert
            {
                get { return m_Insert; }
            }

            /// <summary>
            /// Gets if column is inserted.
            /// </summary>
            public string DataSourceId
            {
                get { return m_DataSourceId; }
                internal set { m_DataSourceId = value; }
            }

            /// <summary>
            /// The row being updated / inserted
            /// </summary>
            public Dictionary<string, CellUpdateInsertArgument> Row
            {
                get { return m_Row; }
            }

            /// <summary>
            /// Gets if column is updated.
            /// </summary>
            public bool Update
            {
                get { return m_Update; }
            }

            internal string m_EditIndex;
            internal bool m_Insert;
            internal string m_DataSourceId;
            internal bool m_Update;
            private bool m_AcceptChanges = true;
        }

        #endregion Special ColumnUpdateInsert EventArgs
    }
}