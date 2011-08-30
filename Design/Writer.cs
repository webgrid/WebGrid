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
    using System.Web.UI;

    internal class WebGridHtmlWriter
    {
        #region Fields

        private readonly StringBuilder m_Allcontent = new StringBuilder();
        private readonly Grid m_Grid;

        private StringBuilder m_Content = new StringBuilder();

        #endregion Fields

        #region Constructors

        public WebGridHtmlWriter()
        {
        }

        public WebGridHtmlWriter(Grid control)
        {
            m_Grid = control;
        }

        #endregion Constructors

        #region Properties

        public Grid Grid
        {
            get { return m_Grid; }
        }

        #endregion Properties

        #region Methods

        public void Close()
        {
            if (m_Content.Length == 0) return;
            if (m_Grid == null)
                m_Allcontent.Append(m_Content.ToString());
            else
                m_Grid.Controls.Add(new LiteralControl(m_Content.ToString()));
            m_Content = new StringBuilder();
        }

        public override string ToString()
        {
            return m_Content.ToString();
        }

        public void Write(Control control)
        {
            if (control == null)
                return;
            if (m_Content.Length != 0)
            {
                if (m_Grid == null)
                    m_Allcontent.Append(m_Content);
                else
                    m_Grid.Controls.Add(new LiteralControl(m_Content.ToString()));
                m_Content = new StringBuilder();
            }
            if (m_Grid == null)
                m_Allcontent.Append(control.ToString());
            else
            {
                if (m_Grid.Controls.Contains(control))
                    m_Grid.Controls.Remove(control);
                m_Grid.Controls.Add(control);
            }
        }

        public void Write(string text)
        {
            m_Content.Append(text);
        }

        public void Write(string text, params object[] args)
        {
            if (text == null)
                return;
            m_Content.AppendFormat(text, args);
        }

        public void Write(StringBuilder text)
        {
            m_Content.Append(text);
        }

        #endregion Methods
    }
}