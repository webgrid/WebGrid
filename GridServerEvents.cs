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
    using System.Text;
    using System.Web;

    using Data;
    using Design;
    using Enums;
    using Events;
    using Util;
    using Data.Database;

    public partial class Grid
    {
        #region Fields

        /// <summary>
        /// indicates creates a new record and using an already existing record in the data source as template.
        /// </summary>
        internal bool m_IsCopyClick;

        private bool m_DoExcelExport;
        private bool? m_GetColumnsPostBackData;

        /// <summary>
        /// indicates if one record has been deleted from data source successfully. (Only true when no error message generated)
        /// </summary>
        private bool m_IsRecordDelete;

        /// <summary>
        /// indicates if one record has been inserted into data source successfully. (Only true when no error message generated)
        /// </summary>
        private bool m_IsRecordNew;

        /// <summary>
        /// indicates if one record has been updated successfully. (Only true when no error message generated)
        /// </summary>
        private bool m_IsRecordUpdate;

        /// <summary>
        /// indicates if records has been updated in grid view without generating any error messages.
        /// </summary>
        private bool m_IsRecordUpdateRows;

        /// <summary>
        /// indicates editIndex should reset, required when updating one record in a row.
        /// </summary>
        private bool m_ResetEditIndex;

        /// <summary>
        /// indicates if one record has been updated in grid view.
        /// </summary>
        private string[] m_UpdateRow;

        //Rows skipped when using ranged slider.
        private int RangedSliderRowLoad;
        //Rows skipped when grouping rows.
        internal int GroupRowsLoad;
        #endregion Fields

        #region Events

        /// <summary>
        /// Event is raised after row delete.
        /// </summary>
        [Category("Data"),
        Description(@"Event is raised after row delete.")]
        public event AfterDeleteEventHandler AfterDelete;

        /// <summary>
        /// Event is raised after WebGrid has finished rendering rows, applies to grid view.
        /// </summary>
        [Category("Data"),
        Description(@"Event is raised after WebGrid has finished rendering rows, applies to grid view.")]
        public event AfterRowsEventHandler AfterRows;

        /// <summary>
        /// Event is raised after row update, applies to grid and detail view.
        /// </summary>
        [Category("Data"),
        Description(@"Event is raised after row update, applies to grid and detail view.")]
        public event AfterUpdateInsertEventHandler AfterUpdateInsert;

        /// <summary>
        /// Event is raised before row delete.
        /// </summary>
        [Category("Data"),
        Description(@"Event is raised before row delete.")]
        public event BeforeDeleteEventHandler BeforeDelete;

        /// <summary>
        /// Event is raised when WebGrid needs data.
        /// </summary>
        [Category("Data"),
        Description(@"Event is raised when WebGrid needs data.")]
        public event NeedDataHandler BeforeGetData;

        /// <summary>
        /// Event is raised before a row is being rendered.
        /// </summary>
        [Category("Data"),
        Description(@"Event is raised before a row is being rendered.")]
        public event BeforeRowRenderHandler BeforeRowRender;

        /// <summary>
        /// Event is raised before WebGrid starts rendering rows, applies to grid view.
        /// </summary>
        [Category("Data"),
        Description(@"Event is raised before WebGrid starts rendering rows, applies to grid view.")]
        public event BeforeRowsEventHandler BeforeRows;

        /// <summary>
        /// Event is raised before record insert/update, applies to grid and detail view.
        /// </summary>
        [Category("Data"),
        Description(@"Event is raised before record insert/update, applies to grid and detail view.")]
        public event BeforeUpdateInsertEventHandler BeforeUpdateInsert;

        /// <summary>
        /// Event is raised before row is validated for insert and update, applies to grid and detail view.
        /// </summary>
        [Category("Data"),
        Description(@"Event is raised before row is validated, applies to grid and detail view.")]
        public event BeforeValidateEventHandler BeforeValidate;

        /// <summary>
        /// Event is raised when column is post back in grid view.
        /// </summary>
        [Category("Data"),
        Description(@"Event is raised when column is post back in grid view.")]
        public event ColumnPostBackEventHandler ColumnPostBack;

        /// <summary>
        /// Event is raised when Grid has completed it's task. Applies to grid and detail view.
        /// </summary>
        [Category("Data"),
        Description(@"Event is raised when WebGrid has completed it's task. Applies to grid and detail view.")]
        public event DoneEventHandler Complete;

        /// <summary>
        /// Event is raised when copy row button is clicked in grid view.
        /// </summary>
        [Category("Data"),
        Description(@"Event is raised when copy row button is clicked in grid view.")]
        public event CopyRowEventHandler CopyRow;

        /// <summary>
        /// Event is raised when edit row button is clicked in grid view.
        /// </summary>
        [Category("Data"),
        Description(@"Event is raised when edit row button is clicked in grid view.")]
        public event EditRowEventHandler EditRow;

        /// <summary>
        /// Event is raised when export to excel button is clicked in grid view.
        /// </summary>
        [Category("Action"),
        Description(@"Event is raised when export to excel button is clicked in grid view.")]
        public event ExportClickEventHandler ExportClick;

        /// <summary>
        /// Event is raised when column is post back in grid view.
        /// </summary>
        [Category("Action"),
        Description(@"Event is raised if any grid post back happens.")]
        public event GridPostBackEventHandler GridPostBack;

        /// <summary>
        /// Event is raised for each row in grid view.
        /// </summary>
        [Category("Data"),
        Description(@"Event is raised when a WebGrid row is bound against the data source.")]
        public event GridRowBoundEventHandler GridRowBound;


        /// <summary>
        /// Event is raised for each row in grid view.
        /// </summary>
        [Category("Data"),
        Description(@"Event is raised when a WebGrid row is bound against the data source.")]
        public event RowGroupingEventHandler RowGrouping;


        /// <summary>
        /// Event is raised when header title (used by sorting) in grid view is clicked.
        /// </summary>
        [Category("Action"),
        Description(@"Event is raised when header title (used by sorting) in grid view is clicked.")]
        public event HeaderClickEventHandler HeaderClick;

        /// <summary>
        /// Event is raised when New Record is clicked in grid view.
        /// </summary>
        [Category("Action"),
        Description(@"Event is raised when New Record is clicked in grid view.")]
        public event NewRecordClickEventHandler NewRecordClick;

        /// <summary>
        /// Event is raised when pager is clicked in grid view.
        /// </summary>
        [Category("Action"),
        Description(@"Event is raised when navigation pager is clicked in grid view")]
        public event PagerClickEventHandler PagerClick;

        // CLICK Edit
        /// <summary>
        /// Event is raised when Cancel button is clicked in detail view.
        /// </summary>
        [Category("Action"),
        Description(@"Event is raised when Cancel button is clicked in detail view.")]
        public event CancelClickEventHandler RecordCancelClick;

        /// <summary>
        /// Event is raised when Delete button is clicked in grid view.
        /// </summary>
        [Category("Action"),
        Description(@"Event is raised when Delete button is clicked in grid view.")]
        public event DeleteClickEventHandler RecordDeleteClick;

        /// <summary>
        /// Event is raised when Update button is clicked in detail view.
        /// </summary>
        [Category("Action"),
        Description(@"Event is raised when Update button is clicked in detail view.")]
        public event UpdateClickEventHandler RecordUpdateClick;

        /// <summary>
        /// Event is raised when row in grid view is clicked. ('UpdateRow' SystemColumn)
        /// </summary>
        [Category("Action"),
        Description(@"Event is raised when row in grid view is clicked.")]
        public event RowClickEventHandler RowClick;

        /// <summary>
        /// Event is raised when Search text has changed in grid view.
        /// </summary>
        [Category("Data"),
        Description(@"Event is raised when Search text has changed in grid view.")]
        public event SearchChangedEventHandler SearchChanged;

        /// <summary>
        /// Event is raised when 'Update row' is clicked in grid view.
        /// </summary>
        [Category("Action"),
        Description(@"Event is raised when 'Update row' is clicked in grid view.")]
        public event UpdateRowClickEventHandler UpdateRowClick;

        // 16.10.2004, Jorn
        /// <summary>
        /// Event is raised when 'Update rows' is clicked in grid view (in toolbars).
        /// </summary>
        [Category("Action"),
        Description(@"Event is raised when 'Update rows' is clicked in grid view.")]
        public event UpdateRowsClickEventHandler UpdateRowsClick;

        #endregion Events

        #region Methods

        /// <summary>
        /// Adds a Row object to the grid row collection.
        /// </summary>
        /// <param name="row">The row to be added.</param>
        /// <returns>
        /// The Row that has been added to the grid row collection.
        /// </returns>
        public void AddRow(Row row)
        {
            MasterTable.Rows.Add(row);
        }

        /// <summary>
        /// Creates the text button.
        /// </summary>
        /// <param name="displayText">The display text.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="cssClass">The CSS class.</param>
        /// <param name="confirmText">The confirm text.</param>
        /// <returns></returns>
        public string CreateTextButton(string displayText, string eventName, string[] arguments, string cssClass,
            string confirmText)
        {
            return Buttons.TextButton(this, displayText, eventName, arguments, cssClass, confirmText);
        }

        /// <summary>
        /// When implemented by a class, enables a server control to process an event raised when a form is posted to the server.
        /// </summary>
        /// <param name="eventArgument">A <see cref="T:System.String"></see> that represents an optional event argument to be passed to the event handler.</param>
        /// <remarks>
        /// Supported eventarguments are:
        /// 
        /// UpdateRowsClick,  UpdateRowClick, RecordClick, RecordCopyClick, ExcelExportClick, CollapseGridClick, CollapseGridClick, ColumnHeaderClick
        /// SlaveGridClick, RecordUpdateClick, RecordCancelClick, NewRecordClick, PagerClick, RecordDeleteClick, ElementPostBack. For further examples how to use these please
        /// see WebGrid starter kit.
        /// </remarks>
        public void RaisePostBackEvent(string eventArgument)
        {
            // 16.10.2004, jorn - Added UpdateRowsClick, added support for autosave.
            if (string.IsNullOrEmpty(eventArgument))
                return;
            if (EnableCallBack)
            {
                eventArgument = HttpUtility.UrlDecode(eventArgument, Encoding.Default);
                eventArgument = eventArgument.Replace("%27", "'");
            }
            string[] eventArgs = eventArgument.Split('!');
            // ColumnId of the event
            string postBackEvent = eventArgs[0];

            // If RaisePostBackEvent is raised programatically.
            m_EventRanInit = true;

            if (GridPostBack != null)
            {
                GridPostBackEventArgs ea = new GridPostBackEventArgs {EventName = postBackEvent};
                ea.SetEventArguments(eventArgs);
                GridPostBack(this, ea);
            }
            if (Trace.IsTracing)
                Trace.Trace("{0} : Start CreatePostBackvent() Event: {1} Args length:{2}", ID, postBackEvent,
                            eventArgs.Length);
            if (Debug)
                m_DebugString.AppendFormat("<b>{0}: Post back-event '{1}' has value array '{2}'</b><br/>", ID,
                                           postBackEvent, eventArgument);
            postBackEvent = postBackEvent.ToLowerInvariant();

            switch (postBackEvent)
            {
                case "refresh":
                    {
                        if (PagerSettings.PagerType == PagerType.Slider || PagerSettings.PagerType == PagerType.RangedSlider)
                            PagerSettings.updateSliderValues();
                        break;
                    }
                case "columnpushup":
                    {
                        PushRowUp(eventArgs);
                        break;
                    }
                case "columnpushdown":
                    {
                        PushRowDown(eventArgs);
                        break;
                    }
                case "updaterowsclick":
                    {
                        m_IsRecordUpdateRows = true;
                        m_GetColumnsPostBackData = true;
                        break;
                    }

                case "updaterowclick":
                    {
                        if (InternalId == null)
                        {
                            InternalId = eventArgs[1];
                            m_ResetEditIndex = true;
                        }
                        m_UpdateRow = eventArgs;
                        m_GetColumnsPostBackData = true;
                        break;
                    }
                case "recordclick":
                    {
                        RecordClick(eventArgs);
                        break;
                    }
                case "groupclick":
                    {
                        GroupClick(eventArgs);
                        break;
                    }
                case "recordcopyclick":
                    {
                        m_IsCopyClick = true;
                        CopyRecordClick(eventArgs);
                        break;
                    }
                case "excelexportclick":
                    {

                        ExcelExportClick();
                        break;
                    }
                case "collapsegridclick":
                    {
                        CollapseGridClick();
                        break;
                    }
                case "columnheaderclick":
                    {
                        ColumnHeaderClick(eventArgs);
                        break;
                    }
                case "slavegridclick":
                    {
                        SlaveGridClick(eventArgs);
                        break;
                    }
                case "recordupdateclick":
                    {
                        m_GetColumnsPostBackData = true;
                        RecordUpdateClickEvent(eventArgs);
                        break;
                    }
                case "recordcancelclick":
                    {
                        RecordCancelClickEvent(eventArgs);
                        break;
                    }
                case "newrecordclick":
                    {
                        NewRecordClickEvent();
                        break;
                    }
                case "pagerclick":
                    {
                        PagerClickEvent(eventArgs);
                        break;
                    }
                case "recorddeleteclick":
                    {
                        RecordDeleteClickEvent(eventArgs);
                        ReLoadData = true;
                        break;
                    }
                case "elementpostback":
                    {
                        m_GetColumnsPostBackData = true;
                        ColumnPostBackEvent(eventArgs);
                        break;
                    }
                case "searchclick":
                    {
                        UpdateGridSearch();
                        break;
                    }
            }

            if (Trace.IsTracing)
                Trace.Trace("{0} : Finish CreatePostBackvent()", ClientID);
        }

        internal bool AddRow(ref Row row)
        {
            if (PagerSettings.PagerType == PagerType.RangedSlider && PagerSettings != null &&
                (PagerSettings.GetSliderValues != null &&
                 (PagerSettings.GetSliderValues.Length == 2 &&
                  (PagerSettings.GetSliderValues[0] > row.RowIndex + RangedSliderRowLoad ||
                   PagerSettings.GetSliderValues[1] < row.RowIndex))))
            {
                RangedSliderRowLoad++;
                return false;
            }


            if (DisplayView == DisplayView.Grid)
            {
                #region Grouping

                if (!string.IsNullOrEmpty(GroupByExpression))
                {
                    string[] columnGroups = GroupByExpression.Split(',');
                    int displayIndex = -10;




                    //First group column must have lowest displayindex.
                    displayIndex -= (10*columnGroups.Length) + 10;
                    //for (int i = columnGroups.Length; i > 0; i--)
                    foreach (string columnGroup in columnGroups)
                    {
                        if (row[columnGroup].Value == null)
                            continue;
                        string columnGroupId = row[columnGroup].Value.ToString();

                        if (!GroupState.ContainsKey(columnGroup))
                            GroupState.Add(columnGroup, new GroupInfo());
                        if (columnGroupId == GroupState[columnGroup].AciveGroupValue)
                            continue;

                        GroupState[columnGroup].AciveGroupValue = columnGroupId;
                        if (!GroupState[columnGroup].List.ContainsKey(columnGroupId))
                            GroupState[columnGroup].List.Add(columnGroupId, new GroupDetails());

                        bool IsExpanded = GroupState[columnGroup].List[columnGroupId].IsExpanded;
                        Row grouprow = new Row(row.m_Table)
                                           {
                                               RowType = RowType.Group,
                                               GroupColumnID = columnGroup,
                                               GroupIsExpanded = IsExpanded
                                           };




                        grouprow[columnGroup].DataSourceValue = row[columnGroup].Value;
                        grouprow.PrimaryKeyValues = row.PrimaryKeyValues;
                        grouprow[columnGroup].Value = row.Columns[columnGroup].ColumnType == ColumnType.Foreignkey
                                                          ? row[columnGroup].DisplayValue
                                                          : row[columnGroup].Value;
                        //;+" <hr style=\"vertical-align:middle;float:right;width:99%; margin: 0;\"/>";
                        int cellsToSpan = 0;
                        int index = displayIndex;
                        grouprow.m_Table.Columns.ForEach(delegate(Column column)
                                                             {
                                                                 if (column.DisplayIndex > index &&
                                                                     (column.Visibility == Visibility.Both ||
                                                                      column.Visibility == Visibility.Grid))
                                                                     cellsToSpan++;
                                                             });

                        grouprow[columnGroup].ExtraCellSpan = cellsToSpan;


                        if (RowGrouping != null)
                        {
                            RowGroupingEventArgs eagroup = new RowGroupingEventArgs
                                                               {
                                                                   ColumnId = columnGroup,
                                                                   IsExpanded = IsExpanded,
                                                                   GroupingRow = grouprow,
                                                                   GroupIdentifier = row[columnGroup].Value
                                                               };
                            if (grouprow[columnGroup].Value != null)
                                eagroup.Description = grouprow[columnGroup].Value.ToString();
                           
                            RowGrouping(this, eagroup);
                            if (eagroup.AcceptChanges)
                            {
                                IsExpanded = eagroup.IsExpanded;
                             
                                grouprow = eagroup.GroupingRow;
                                grouprow[columnGroup].Value = eagroup.Description;
                            }
                        }

                        displayIndex += 10;
                        row.m_Table.Columns[columnGroup + "_Group"].ColumnType = ColumnType.Text;
                        row.m_Table.Columns[columnGroup + "_Group"].Visibility = Visibility.Grid;
                        row.m_Table.Columns[columnGroup + "_Group"].Title = string.Empty;
                        row.m_Table.Columns[columnGroup + "_Group"].DisplayIndex = displayIndex - 10;
                        row.m_Table.Columns[columnGroup + "_Group"].HyperLinkColumn = true;
                        grouprow[columnGroup + "_Group"].Value = IsExpanded ? string.Format("<span class=\"ui-icon ui-icon-triangle-1-s\"/>") : string.Format("<span class=\"ui-icon ui-icon-triangle-1-e\"/>");


                        GroupState[columnGroup].List[columnGroupId].IsExpanded = IsExpanded;
                        row.m_Table.Rows.Add(grouprow);

                        if (!string.IsNullOrEmpty(columnGroupId) && !IsExpanded)
                        {
                            GroupRowsLoad++;
                            return false;
                        }
                    }
                }

                #endregion

                if (GridRowBound == null)
                    return true;
                GridRowBoundEventArgs ea = new GridRowBoundEventArgs {EditIndex = row.PrimaryKeyValues, Row = row};
                try
                {
                    GridRowBound(this, ea);
                    if (ea.AcceptChanges)
                        row = ea.Row;
                }
                catch (Exception ee)
                {
                    throw new GridException(
                        string.Format("Error in 'GridRowBound' event. (Row identifier:{0})", ea.EditIndex), ee);
                }

                return ea.AcceptChanges;
            }
            else
            {
                if (EditRow == null)
                    return true;
                EditRowEventArgs ea = new EditRowEventArgs { Row = row, EditIndex = InternalId };
                EditRow(this, ea);
                row = ea.Row;
                if (ea.AcceptChanges == false)
                {
                    InternalId = null;
                    DisplayView = DisplayView.Grid;
                    MasterTable.GetData(true);
                    return true;
                }
                return ea.AcceptChanges;
            }
        }

        internal void AfterDeleteEvent(string columnID)
        {
            // Delete has happened! :-O
            if (AfterDelete != null)
            {
                AfterDeleteEventArgs ea = new AfterDeleteEventArgs { EditIndex = columnID };
                AfterDelete(this, ea);
            }
            if (SystemMessage.Count == 0)
                m_IsRecordDelete = true;
        }

        internal string AfterRowsEvent()
        {
            return AfterRows != null ? AfterRows(this) : null;
        }

        internal bool BeforeDeleteEvent(string rowId)
        {
            // Delete is about to happen!  Return false = cancel delete
            if (BeforeDelete != null)
            {
                BeforeDeleteEventArgs ea = new BeforeDeleteEventArgs { EditIndex = rowId, Row = MasterTable.Rows[rowId] };
                BeforeDelete(this, ea);
                if (ea.AcceptChanges == false) // Programmer doesn't want to delete...! :-O
                    return false;
            }
            return true;
        }

        internal BeforeGetDataEventArgs BeforeGetDataEvent(Table table)
        {
            if (BeforeGetData != null)
            {
                BeforeGetDataEventArgs ea = new BeforeGetDataEventArgs {Table = table};
                BeforeGetData(this, ea);
                return ea;
            }
            return null;
        }

        internal string BeforeRowsEvent()
        {
            return BeforeRows != null ? BeforeRows(this) : null;
        }

        internal void BeforeUpdateInsertEvent(BeforeUpdateInsertEventArgs ea)
        {
            if (BeforeUpdateInsert != null)
                BeforeUpdateInsert(this, ea);
        }

        internal void BeforeValidateEvent(ref Row row)
        {
            if (BeforeValidate == null)
                return;
            BeforeValidateEventArgs ea = new BeforeValidateEventArgs { Row = row, EditIndex = InternalId };
            if (Trace.IsTracing)
                Trace.Trace("{0} : Launch BeforeValidate();", ID);
            BeforeValidate(this, ea);
            row = ea.Row;
        }

        private void ColumnPostBackEvent(string[] eventArguments)
        {
            if (ColumnPostBack == null) return;
            ColumnPostBackEventArgs ea = new ColumnPostBackEventArgs
                                             {
                                                 ColumnName = eventArguments[1],
                                                 EditIndex =
                                                     (eventArguments.Length > 2 ? eventArguments[2] : InternalId)
                                             };
            if (ea.EditIndex != null)
                ea.Row = MasterTable.Rows[ea.EditIndex];
            if (ea.Row == null)
                ea.Row = MasterTable.Rows[0];
            ColumnPostBack(this, ea);
        }

        private void CollapseGridClick()
        {
            if (ViewState["CollapseGrid"] == null || (bool)ViewState["CollapseGrid"] == false)
                ViewState["CollapseGrid"] = true;
            else
                ViewState["CollapseGrid"] = false;
        }

        private void ColumnHeaderClick(string[] eventArguments)
        {
            // Happens when the user clicks a column title in grid-mode.
            if (HeaderClick != null)
            {
                HeaderClickEventArgs ea = new HeaderClickEventArgs { ColumnName = eventArguments[1] };

                HeaderClick(this, ea);
                if (!ea.AcceptChanges) // The programmer can abort further actions.
                    return;
            }
            MasterTable.OrderByAdd = eventArguments[1];
        }

        private void CopyRecordClick(string[] eventArguments)
        {
            // Happens when the user clicks "Copy record" inside grid view.
            InternalId = eventArguments[2];
            Row CopiedRow = MasterTable.Rows[InternalId];

            if (CopyRow != null)
            {
                CopyRowEventArgs ea = new CopyRowEventArgs { CopiedId = eventArguments[2], Row = CopiedRow };
                CopyRow(this, ea);
                if (!ea.AcceptChanges)
                    return;
                CopiedRow = ea.Row;
            }
            //Set detail view
            InternalId = null;
            DisplayView = DisplayView.Detail;
            MasterTable.GetData(true);
            //Copy the row
            MasterTable.Rows[0] = CopiedRow;
            //Make sure we don't receive any postback data or load any data.
            MasterTable.m_GotPostBackData = true;
            MasterTable.m_GotData = true;

            return;
        }

        private void ExcelExportClick()
        {
            if (Equals(m_DoExcelExport, true))
                return;
            ExcelExportClickEventArgs ea = new ExcelExportClickEventArgs { FileName = ExportFileName };

            // Happens when the user clicks a link in grid-mode.
            if (ExportClick != null)
            {
                ExportClick(this, ea);
                if (ea.AcceptChanges == false) // The programmer can abort further actions.
                {
                    return;
                }
            }

            ExportFileName = ea.FileName;

            m_DoExcelExport = true;
            if (Trace.IsTracing)
                Trace.Trace(string.Format("{0} : ExcelExportClick!", ID));
        }

        private void NewRecordClickEvent()
        {
            m_GetColumnsPostBackData = false;
            // Happens when the user clicks "New record" inside grid view.
            DisplayView oldMode = DisplayView;
            string oldEditIndex = InternalId;

            DisplayView = DisplayView.Detail;
            InternalId = null;
            MasterTable.m_GotData = false;

            if (NewRecordClick == null) return;
            NewRecordClickEventArgs ea = new NewRecordClickEventArgs();
            NewRecordClick(this, ea);
            if (ea.AcceptChanges) return;
            DisplayView = oldMode;
            InternalId = oldEditIndex;

            return;
        }

        private void PagerClickEvent(string[] eventArguments)
        {
            int newPage;
            if (eventArguments[1].Equals("all", StringComparison.OrdinalIgnoreCase))
                newPage = 0;
            else
            {
                if (!int.TryParse(eventArguments[1], out newPage))
                    throw new GridException(
                        string.Format(
                            "Failed on 'PagerClick' event. Excepted number value as second eventArgument, got: {0}",
                            eventArguments[1]));
                newPage = int.Parse(eventArguments[1]);
                if (newPage < 1) newPage = 1;
            }
            // Happens when the user clicks on the pager thingy
            if (PagerClick != null)
            {
                PagerClickEventArgs ea = new PagerClickEventArgs
                {
                    OldCurrentPage = PageIndex,
                    NewCurrentPage = newPage
                };
                PagerClick(this, ea);

                if (ea.AcceptChanges == false) // The programmer can abort further actions.
                    return;
                newPage = ea.NewCurrentPage;
            }
            if (PagerSettings.PagerType == PagerType.Slider)
                PagerSettings.SliderValue = newPage.ToString();
            PageIndex = newPage;
        }

        private void PushRowDown(string[] eventArgs)
        {
            if (eventArgs.Length != 6)
                throw new GridException(
                    string.Format("Invalid number of parameters for PushRowDown method, excepted 7 arguments was {0}.",
                                  eventArgs.Length));
            string columnId = eventArgs[3];
            string orginalrowId = eventArgs[1];
            string swaprowId = eventArgs[2];
            int orgValue, swapValue;
            bool res = int.TryParse(eventArgs[4], out orgValue);
            if (!res)
            {
                SystemMessage.Add(string.Format("Unable to parse orginal values from '{0}' column (Value:'{1}').",
                                                Columns[columnId].Title, eventArgs[4]));
                return;
            }
            res = int.TryParse(eventArgs[5], out swapValue);
            if (!res)
            {
                SystemMessage.Add(string.Format("Unable to parse orginal values from '{0}' column (Value:'{1}').",
                                                Columns[columnId].Title, eventArgs[5]));
                return;
            }

            if (orgValue == swapValue)
                orgValue -= 1;

            MasterTable.Rows[orginalrowId][columnId].Value = swapValue;
            MasterTable.Rows[swaprowId][columnId].Value = orgValue;

            res = MasterTable.InsertUpdate(ref orginalrowId, MasterTable.Rows[orginalrowId]);
            if (!res)
                SystemMessage.Add("Failed to push rows down 1.");
            MasterTable.InsertUpdate(ref swaprowId, MasterTable.Rows[swaprowId]);
            if (!res)
                SystemMessage.Add("Failed to push rows down 2.");
            ReLoadData = true;
        }

        private void PushRowUp(string[] eventArgs)
        {
            if (eventArgs.Length != 6)
                throw new GridException(
                    string.Format("Invalid number of parameters for PushRowDown method, excepted 7 arguments was {0}.",
                                  eventArgs.Length));
            string columnId = eventArgs[3];
            string orginalrowId = eventArgs[1];
            string swaprowId = eventArgs[2];
            int orgValue, swapValue;
            bool res = int.TryParse(eventArgs[4], out orgValue);
            if (!res)
            {
                SystemMessage.Add(string.Format("Unable to parse orginal values from '{0}' column (Value:'{1}').",
                                                Columns[columnId].Title, eventArgs[4]));
                return;
            }
            res = int.TryParse(eventArgs[5], out swapValue);
            if (!res)
            {
                SystemMessage.Add(string.Format("Unable to parse orginal values from '{0}' column (Value:'{1}').",
                                                Columns[columnId].Title, eventArgs[5]));
                return;
            }

            if (orgValue == swapValue)
                orgValue += 1;

            MasterTable[orginalrowId][columnId].Value = swapValue;
            MasterTable[swaprowId][columnId].Value = orgValue;

            res = MasterTable.InsertUpdate(ref orginalrowId, MasterTable.Rows[orginalrowId]);
            if (!res)
                SystemMessage.Add("Failed to push row up");
            MasterTable.InsertUpdate(ref swaprowId, MasterTable.Rows[swaprowId]);
            if (!res)
                SystemMessage.Add("Failed to push row down");
            ReLoadData = true;
        }

        private void RecordCancelClickEvent(string[] eventArguments)
        {
            // Happens when the user clicks a "CANCELWINDOW" inside detail view.
            if (RecordCancelClick != null)
            {
                CancelClickEventArgs ea = new CancelClickEventArgs {EditIndex = eventArguments[1]};
                RecordCancelClick(this, ea);
                if (ea.AcceptChanges == false) // The programmer can abort further actions.
                    return;
            }
            MasterTable.Cancel();

            if (((
                     m_MasterWebGrid != null && m_MasterWebGrid.ActiveMenuSlaveGrid == this &&
                     string.IsNullOrEmpty(eventArguments[1]) && DisplayView == DisplayView.Grid)
                 || AllowCancel == false ||
                 (eventArguments.Length > 2 &&
                  Equals(eventArguments[2].Equals("true", StringComparison.OrdinalIgnoreCase), true))) &&
                m_MasterWebGrid != null)

                m_MasterWebGrid.ActiveMenuSlaveGrid = null;
               // else
               // {
                EditIndex = null;
                MasterTable.m_GotData = false;
                DisplayView = DisplayView.Grid;
             //   }
        }
        private void GroupClick(string[] eventArguments)
        {
            /*
             1:GroupColumnID
             2:DataSourceValue
             3:PrimaryKeyValues
             4:IsExpanded
            */
            string columnGroup = eventArguments[1];
            string columnGroupId = eventArguments[2];
            string columnKeys = eventArguments[3];
            bool IsExpanded = !(eventArguments[4].ToLower() == "true" || eventArguments[4].ToLower() == "1");

            if (!GroupState.ContainsKey(columnGroup))
                GroupState.Add(columnGroup, new GroupInfo());

            if (!GroupState[columnGroup].List.ContainsKey(columnGroupId))
                GroupState[columnGroup].List.Add(columnGroupId, new GroupDetails());

            GroupState[columnGroup].List[columnGroupId].IsExpanded = IsExpanded;


            if (!GroupState[columnGroup].List[columnGroupId].IsExpanded)
            {
                string t = Interface.BuildFilterElement(true, DataSourceId, MasterTable.Columns[columnGroup], columnGroupId
                                                        );
                t = t.Replace(" = ", " <> ");
                GroupState[columnGroup].List[columnGroupId].GroupSqlFilter = string.Format("({0} OR {1})", t,
                                                                             Interface.BuildPKFilter(MasterTable,columnKeys
                                                                                                     ,
                                                                                                     true));
                
            }
            else
                GroupState[columnGroup].List[columnGroupId].GroupSqlFilter = null;

        }


        private void RecordClick(string[] eventArguments)
        {
            DisplayView oldMode = DisplayView;
            string oldEditIndex = InternalId;
            DisplayView = DisplayView.Detail;
            MasterTable.m_GotPostBackData = true;
            MasterTable.m_GotData = false;

            InternalId = eventArguments[2];
            // Happens when the user clicks a link in grid-mode.
            if (RowClick != null)
            {
                RowClickEventArgs ea = new RowClickEventArgs
                                           {
                                               ColumnName = eventArguments[1],
                                               EditIndex = eventArguments[2],
                                               Row = MasterTable.Rows[InternalId]
                                           };
                if (ea.Row == null)
                    return;// throw new GridException("Row was not found.");
                RowClick(this, ea);
                if (ea.AcceptChanges == false) // The programmer can abort further actions.
                {
                    DisplayView = oldMode;
                    InternalId = oldEditIndex;
                    if (DisplayView == DisplayView.Grid)
                        MasterTable.m_GotData = false;
                    return;
                }
            }
            if (Trace.IsTracing)
                Trace.Trace(string.Format("{0} : RecordClick!", ClientID));
        }

        private void RecordDeleteClickEvent(string[] eventArguments)
        {
            //m_Deleting = true;
            // Happens when the user clicks on the delete thingy on a record
            if (RecordDeleteClick != null)
            {
                RowClickEventArgs ea = new RowClickEventArgs
                                           {
                                               EditIndex = eventArguments[1],
                                               Row = MasterTable[InternalId]
                                           };
                RecordDeleteClick(this, ea);
                if (ea.AcceptChanges == false)
                    return;
            }
            // Delete record! :-O
            MasterTable.DeleteRow(eventArguments[1]);
            // m_Deleting = false;
        }

        // Happens when the user clicks a "UPDATE" inside detail view.
        private void RecordUpdateClickEvent(string[] eventArguments)
        {
            DisplayView = DisplayView.Detail;

            if (RecordUpdateClick != null)
            {
                UpdateClickEventArgs ea = new UpdateClickEventArgs();
                if (eventArguments.Length > 2 && eventArguments[2].Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    ea.Copy = true;
                    ea.EditIndex = null;
                }
                else
                    ea.EditIndex = eventArguments[1];

                RecordUpdateClick(this, ea);
                if (ea.AcceptChanges == false) // The programmer can abort further actions.
                    return;
            }
            // Row should be copied.
            if (eventArguments.Length > 2 && eventArguments[2].Equals("true", StringComparison.OrdinalIgnoreCase))
                InternalId = null;

            else if (eventArguments.Length > 3)
            {
                MasterTable.m_GotPostBackData = false;

                MasterTable.m_GotData = false;
                m_IsOneToOneRelationGrid = true;

                MasterTable.m_Grid.InternalId = eventArguments[1];
                if (eventArguments[3].Equals("new", StringComparison.OrdinalIgnoreCase))
                    m_IsOneToOneRelationNewRecord = true;
            }
            if (m_MasterTable.m_GotData == false)
                MasterTable.GetData(false);

            bool update = (InternalId != null);
            bool insert = (InternalId == null);
            string res = null;
            if (MasterTable.InsertUpdate(ref res) == false)
                return;

            if (SystemMessage.Count != 0) return;
            ReLoadData = true;

            if (MasterTable.m_Grid.StayInDetail == false)
            {
                InternalId = null;
                DisplayView = DisplayView.Grid;
                if (MasterTable.m_Grid.Trace.IsTracing)
                    MasterTable.m_Grid.Trace.Trace(string.Format("{0} grid view after insert", ID));
            }
            if (update)
                m_IsRecordUpdate = true;
            else
                m_IsRecordNew = true;

            if (AfterUpdateInsert != null)
            {
                AfterUpdateInsertEventArgs ea = new AfterUpdateInsertEventArgs
                {
                    EditIndex = res,
                    Row = MasterTable.Rows[0],
                    m_Datarow = MasterTable.Rows[0].m_DataRow,
                    Update = update,
                    Insert = insert
                };

                AfterUpdateInsert(this, ea);
            }
            string message = update
                                 ? GetSystemMessage("ClientNotification_Update")
                                 : GetSystemMessage("ClientNotification_Insert");
            AddClientNotificationMessage(message);

            if (!MasterTable.m_Grid.StayInDetail || !insert) return;
            MasterTable.m_Grid.InternalId = res;
            if( MasterTable.Rows.Count == 1)
                MasterTable.Rows[0].PrimaryKeyValues = res;
            DisplayView = DisplayView.Detail;
            if (MasterTable.m_Grid.Trace.IsTracing)
                MasterTable.m_Grid.Trace.Trace("{0} sets EditIndex at StayInDetail && Insert", ID);
        }

        private void SlaveGridClick(string[] eventArguments)
        {
            if (eventArguments[1] == null) return;
            if (GetState("ActiveMenuSlaveGrid") != null &&
                (string) GetState("ActiveMenuSlaveGrid") == eventArguments[1])
                State("ActiveMenuSlaveGrid", null);
            else
                State("ActiveMenuSlaveGrid", eventArguments[1]);
        }

        private void UpdateGridSearch()
        {
            if (!GotHttpContext)
                return;

            string search = HttpContext.Current.Request[ClientID + "_gridsearch"];
            string oldsearch = Search;

            if (SearchChanged != null)
            {
                SearchChangedEventArgs ea = new SearchChangedEventArgs
                {
                    OldSearchString = oldsearch,
                    SearchString = search
                };

                SearchChanged(this, ea);

                Search = ea.SearchString;
            }
            else
                Search = search;
        }

        private void UpdateRowClickEvent(string[] eventArguments)
        {
            if (Trace.IsTracing)
                Trace.Trace("UpdateRowClick({0}) Start", MasterTable.DataSourceId);

            int i = 0;
            bool status = false;

            while (i < MasterTable.Rows.Count)
            {
                if (eventArguments[1] == MasterTable.Rows[i].PrimaryKeyUpdateValues)
                {
                    status = true;
                    break;
                }
                i++;
            }
            if (Equals(status, true))
            {
                if (UpdateRowClick != null)
                {
                    UpdateRowClickEventArgs ea = new UpdateRowClickEventArgs {EditIndex = eventArguments[1]};
                    UpdateRowClick(this, ea);

                    if (!ea.AcceptChanges) // The programmer can abort further actions.
                        return;
                }

                string res = DisplayView == DisplayView.Detail ? InternalId : MasterTable.Rows[i].PrimaryKeyUpdateValues;

                // Retrieve postback data before we proceeds
                for (int ii = 0; ii < MasterTable.Rows[i].Columns.Count; ii++)
                    MasterTable.Rows[i][ii].GetCellPostBackData();

                MasterTable.InsertUpdate(ref res, MasterTable.Rows[i]);

                if (SystemMessage.Count == 0)
                {
                    ReLoadData = true;
                    if (AfterUpdateInsert != null)
                    {
                        AfterUpdateInsertEventArgs ea = new AfterUpdateInsertEventArgs
                                                            {
                                                                EditIndex = res,
                                                                Row = MasterTable.Rows[i],
                                                                Update = true,
                                                                Insert = false
                                                            };
                        AfterUpdateInsert(this, ea);
                    }
                }
                else // error for record update should not generate on systemMessages.
                    m_IsRecordUpdate = false;
            }
            AddClientNotificationMessage(GetSystemMessage("ClientNotification_Update"));

            if (Trace.IsTracing)
                Trace.Trace("UpdateRowClick({0}) Finished;", MasterTable.DataSourceId);
        }

        private void UpdateRowsClickEvent()
        {
            string oldWhere = null;
            if (PageIndex == 0 && GotHttpContext)
            {
                oldWhere = FilterExpression;
                FilterExpression = HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request[ClientID + "_where"]);
            }

            // 16.10.2004, Jorn
            // Happens when the user clicks "Update Rows" inside grid view.
            if (Trace.IsTracing)
                Trace.Trace("UpdateRowsClick({0}) Start;", MasterTable.DataSourceId);

            if (Equals(m_MasterTable.m_GotData, false))
                MasterTable.GetData(false);
            if (UpdateRowsClick != null)
            {
                UpdateRowsClickEventArgs ea = new UpdateRowsClickEventArgs();
                UpdateRowsClick(this, ea);
                if (ea.AcceptChanges == false) // The programmer can abort further actions.
                    return;
            }
            // Kj0r update!
            for (int i = 0; i < MasterTable.Rows.Count; i++)
            {
                string res = DisplayView == DisplayView.Detail ? InternalId : MasterTable.Rows[i].PrimaryKeyUpdateValues;

                if (MasterTable.InsertUpdate(ref res, MasterTable.Rows[i]) == false)
                    continue;

                if (SystemMessage.Count != 0) continue;
                ReLoadData = true;
                if (AfterUpdateInsert == null) continue;
                AfterUpdateInsertEventArgs ea = new AfterUpdateInsertEventArgs
                {
                    EditIndex = res,
                    Row = MasterTable.Rows[i],
                    Update = true,
                    Insert = false
                };

                AfterUpdateInsert(this, ea);
            }
            AddClientNotificationMessage(GetSystemMessage("ClientNotification_Update"));
             // Done cause of the property SystemMessageUpdateRows message should not be rendered if there are systemMessages.
            if (SystemMessage.Count > 0)
                m_IsRecordUpdateRows = false;

            if (Trace.IsTracing)
                Trace.Trace("UpdateRowsClick({0}) Finished;", MasterTable.DataSourceId);
            if (PageIndex == 0)
                FilterExpression = oldWhere;
        }

        #endregion Methods
    }
}