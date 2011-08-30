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

namespace WebGrid.Design
{
    using System.Text;
    using System.Web.UI.WebControls;

    using WebGrid.Data;
    using WebGrid.Enums;
    using WebGrid.Util.Json;

    internal class ColumnHeader
    {
        #region Fields

        public const int m_RowSpan = 1;

        public HorizontalPosition m_Align = HorizontalPosition.Undefined;
        public string m_Class;
        public int m_ColumnSpan = 1;
        public Unit m_ColumnWidth = Unit.Empty;
        public string m_GridId;
        public int m_GridRowCount;
        public VerticalPosition m_VAlign = VerticalPosition.top;

        #endregion Fields

        #region Methods

        /* public static void Render(string content, string cssclass, HtmlTextWriter writer)
        {
            ColumnCell td = new ColumnCell {m_Class = cssclass};
            td.RenderBeginTag(writer);
            writer.Write(content);
            td.RenderEndTag(writer);
        }
        */
        public void RenderBeginTag(WebGridHtmlWriter writer,Column column,RowCell cell, string tagtype)
        {
            StringBuilder tdStart = new StringBuilder(tagtype);
            string name = "";
            if (column != null)
                name = column.ColumnId;
            if (m_Align != HorizontalPosition.Undefined)
                tdStart.AppendFormat(" align=\"{0}\"", m_Align.ToString().ToLowerInvariant());

            if (m_VAlign != VerticalPosition.undefined)
                tdStart.AppendFormat(" valign=\"{0}\"", m_VAlign.ToString().ToLowerInvariant());
            if( string.IsNullOrEmpty(m_Class) == false )
                tdStart.AppendFormat(" class=\"{0}\" ", m_Class);

            if (m_GridRowCount == -1)
            {
                if (m_ColumnWidth != Unit.Empty)
                    tdStart.AppendFormat(" style=\"width: {0}\"", m_ColumnWidth);
                if (string.IsNullOrEmpty(name) == false)
                    tdStart.AppendFormat(" id=\"{0}{1}r0\"", m_GridId, name);
            }
            else if (string.IsNullOrEmpty(name) == false && m_GridRowCount > 0)
                tdStart.AppendFormat(" id=\"{0}{1}r{2}\"", m_GridId, name, m_GridRowCount);
            else if (string.IsNullOrEmpty(name) == false)
                tdStart.AppendFormat(" id=\"{0}{1}\"", name, m_GridRowCount);

            if (m_ColumnSpan > 1) tdStart.AppendFormat(" colspan=\"{0}\"", m_ColumnSpan);
             //   if (m_RowSpan > 1) tdStart.AppendFormat(" rowspan=\"{0}\"", m_RowSpan);

            if(column != null && !string.IsNullOrEmpty( writer.Grid.OnClientCellClick ))
            {
                ClientCellEventArgs ea = new ClientCellEventArgs();

                ea.ColumnId = column.ColumnId;
                ea.RowIndex = m_GridRowCount;
                if (cell != null)
                    ea.Value = cell.Value;
                ea.ClientEventType = ClientEventType.OnClientColumnClick;
                string content = JavaScriptConvert.SerializeObject(ea);
                writer.Grid.JsOnData.AppendLine();
                string jsonId = string.Format("{0}r{2}{1}", m_GridId, name, m_GridRowCount).Replace("-","A");

                writer.Grid.JsOnData.AppendFormat("{0} = {1}", jsonId, content);

                tdStart.AppendFormat(@" onclick=""{0}(this,{1});return false"" ", writer.Grid.OnClientCellClick, jsonId);
            }
            if (column != null && !string.IsNullOrEmpty(writer.Grid.OnClientCellDblClick))
            {
                ClientCellEventArgs ea = new ClientCellEventArgs();

                ea.ColumnId = column.ColumnId;
                ea.RowIndex = m_GridRowCount;
                if (cell != null)
                    ea.Value = cell.Value;
                ea.ClientEventType = ClientEventType.OnClientColumnDblClick;
                string content = JavaScriptConvert.SerializeObject(ea);
                writer.Grid.JsOnData.AppendLine();
                string jsonId = string.Format("{0}r{2}{1}", m_GridId, name, m_GridRowCount).Replace("-", "A");

                writer.Grid.JsOnData.AppendFormat("{0} = {1}", jsonId, content);

                tdStart.AppendFormat(@" ondblclick=""{0}(this,{1});return false"" ", writer.Grid.OnClientCellDblClick, jsonId);
            }
            if (column != null && !string.IsNullOrEmpty(writer.Grid.OnClientCellMouseOver))
            {
                ClientCellEventArgs ea = new ClientCellEventArgs();

                ea.ColumnId = column.ColumnId;
                ea.RowIndex = m_GridRowCount;
                if (cell != null)
                    ea.Value = cell.Value;
                ea.ClientEventType = ClientEventType.OnClientColumnMouseOver;
                string content = JavaScriptConvert.SerializeObject(ea);
                writer.Grid.JsOnData.AppendLine();
                string jsonId = string.Format("{0}r{2}{1}mouseover", m_GridId, name, m_GridRowCount).Replace("-", "A");

                writer.Grid.JsOnData.AppendFormat("{0} = {1}", jsonId, content);

                tdStart.AppendFormat(@"onmouseover=""{0}(this,{1});return false""", writer.Grid.OnClientCellMouseOver, jsonId);
            }
            if (column != null && !string.IsNullOrEmpty(writer.Grid.OnClientCellMouseOut))
            {
                ClientCellEventArgs ea = new ClientCellEventArgs();

                ea.ColumnId = column.ColumnId;
                ea.RowIndex = m_GridRowCount;
                if (cell != null)
                    ea.Value = cell.Value;
                ea.ClientEventType = ClientEventType.OnClientColumnMouseOut;
                string content = JavaScriptConvert.SerializeObject(ea);
                writer.Grid.JsOnData.AppendLine();
                string jsonId = string.Format("{0}r{2}{1}mouseout", m_GridId, name, m_GridRowCount).Replace("-", "A");

                writer.Grid.JsOnData.AppendFormat("{0} = {1}", jsonId, content);

                tdStart.AppendFormat(@"onmouseout=""{0}(this,{1});return false""", writer.Grid.OnClientCellMouseOut, jsonId);
            }
            tdStart.Append(">");

            writer.Write(tdStart);
        }

        public void RenderEndTag(WebGridHtmlWriter writer, string tagtype)
        {
            writer.Write(tagtype);
        }

        #endregion Methods
    }
}