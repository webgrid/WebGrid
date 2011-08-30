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
    using System.Text;
    using System.Web;

    using Data;
    using Enums;
    using Events;

    public partial class Grid
    {
        #region Methods

        /// <summary>
        /// Used to process callback from the client to the server.
        /// </summary>
        /// <param name="eventArgument"></param>
        [Anthem.Method]
        public void CallBackMethod(string eventArgument)
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
                GridPostBackEventArgs ea = new GridPostBackEventArgs { EventName = postBackEvent };
                ea.SetEventArguments(eventArgs);
                GridPostBack(this, ea);
            }
            if (PagerSettings.PagerType == PagerType.Slider || PagerSettings.PagerType == PagerType.RangedSlider)
                PagerSettings.updateSliderValues();

            if (Trace.IsTracing)
                Trace.Trace("{0} : Start CallBackMethod() Event: {1} Args length:{2}", ID, postBackEvent,
                            eventArgs.Length);
            if (Debug)
                m_DebugString.AppendFormat("<b>{0}: CallBackMethod event '{1}' has value array '{2}'</b><br/>", ID,
                                           postBackEvent, eventArgument);
        }

        ///<summary>
        /// Gets tooltip information from database.
        ///</summary>
        ///<param name="eventArgument"></param>
        ///<returns></returns>
        [Anthem.Method]
        public string GetToolTipData(string eventArgument)
        {
            // 16.10.2004, jorn - Added UpdateRowsClick, added support for autosave.
              if (string.IsNullOrEmpty(eventArgument))
                  return null;

              string[] eventArgs = eventArgument.Split('!');
              if (eventArgs.Length != 2)
                  return null;
              string columnName = eventArgs[0];
              string EditKey = eventArgs[1];

              if (!MasterTable.Columns.Contains(columnName) || string.IsNullOrEmpty(EditKey) ||
                  MasterTable.Columns[columnName].ColumnType != ColumnType.ToolTip)
                  return null;

              EditIndex = EditKey;
              ToolTipColumn column = ((ToolTipColumn) MasterTable.Columns[columnName]);

              Row row = Rows[0];

              if (row == null)
                  return "No DataRow found";
              return column.GetEventData(columnName, EditKey, row);
        }

        #endregion Methods
    }
}