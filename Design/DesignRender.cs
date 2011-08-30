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
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Web;

    using Enums;

    internal class DesignRender : Rendering
    {
        #region Fields

        /// <summary>
        /// The grid instance to be rendered
        /// </summary>
        internal Grid m_Grid;

        private const string contextscript = 
            @"try {{$(document).ready( function() {{

            $(""#wgContent_{0}"").contextMenu({{
                    menu: '{0}_Menu'
                }});
            }})}}catch(err){{}}";
        private const string meiomask = @"
        $(document).ready( function() {{

        $.mask.masks = $.extend($.mask.masks,{{ {1} }});

        $('#wgContent_{0} input:text').setMask({2});

        }});";

        #endregion Fields

        #region Constructors

        public DesignRender(Grid grid)
        {
            m_Grid = grid;
        }

        #endregion Constructors

        #region Methods

        internal void Render()
        {
            WebGridHtmlWriter writer = new WebGridHtmlWriter(m_Grid);

            Render(writer);
        }

        internal void Render(WebGridHtmlWriter mywriter)
        {
            // MasterWebGrid is using SlaveGridMenu.
            if (((m_Grid.SystemMessage.CriticalCount == 0 && m_Grid.MasterGrid != null && m_Grid.MasterWebGrid != null &&
                  m_Grid.m_IsGridInsideGrid == false &&
                  m_Grid.MasterWebGrid.DisplaySlaveGridMenu &&
                  (m_Grid.MasterWebGrid.ActiveMenuSlaveGrid == null ||
                   m_Grid.MasterWebGrid.ActiveMenuSlaveGrid.ClientID != m_Grid.ClientID))
                ) ||
                (m_Grid.m_IsGridInsideGrid && m_Grid.MasterWebGrid != null &&
                 m_Grid.MasterWebGrid.m_HasRendered == false))
                return;
            if (m_Grid.HideIfEmptyGrid && m_Grid.MasterTable.Rows.Count == 0 && m_Grid.SystemMessage.CriticalCount == 0 &&
                !m_Grid.m_ActiveColumnFilter)
            {
                if (m_Grid.EmptyDataTemplate != null)
                    mywriter.Write(m_Grid.EmptyDataTemplate);
                return;
            }

            m_Grid.m_HasRendered = true;

            if (m_Grid.Tag["wgcsslink"] != null)
                mywriter.Write(m_Grid.Tag.Get("wgcsslink"));
            if (m_Grid.Tag["wgscriptstyles"] != null)
                mywriter.Write(m_Grid.Tag.Get("wgscriptstyles"));
            if (m_Grid.PreGridTemplate != null)
                mywriter.Write(m_Grid.PreGridTemplate);

            string width = string.Empty;
            if (m_Grid.Width.IsEmpty == false)
                width = string.Format(" width=\"{0}\"", m_Grid.Width);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            System.Web.UI.HtmlTextWriter renderBeginTag = new System.Web.UI.HtmlTextWriter(sw);

            m_Grid.RenderBeginTag(renderBeginTag);

            string webAttributes = sb.ToString();
            // Remove first tag which is always 'id='
            webAttributes = webAttributes.Remove(0, webAttributes.IndexOf("id="));
            // Clean up..
            webAttributes = webAttributes.Remove(0, webAttributes.IndexOf(" "));

            webAttributes = string.Format("<table{0}", webAttributes.Remove(webAttributes.Length - 1));

            #region slavegrid menu

            bool systemMessagetriggered = false;
            if (m_Grid.DisplayView == DisplayView.Detail && m_Grid.DisplaySlaveGridMenu && m_Grid.SlaveGrid.Count > 0 &&
                m_Grid.InternalId != null)
            {
                mywriter.Write("<!--WebGrid menu starts rendering -->\n");
                mywriter.Write("{0} class=\"wgmenu\" {1}  cellpadding=\"0\" cellspacing=\"0\" id=\"wgMenu_{2}\"><tr><td>",
                    webAttributes,
                    width, m_Grid.ClientID);
                if (m_Grid.SystemMessage.Count > 0)
                {

                    mywriter.Write(m_Grid.SystemMessage.RenderSystemMessage);
                    if (m_Grid.Scripts == null || !m_Grid.Scripts.DisableBookmark)
                        Grid.AddClientScript(mywriter, string.Format("bookmarkscroll.init();bookmarkscroll.scrollTo('wgSystemMessage_{0}')", m_Grid.ID));
                    mywriter.Write("</td></tr><tr><td>");
                    systemMessagetriggered = true;
                }

                /* cellpadding and cellspacing are fixed because CSS does not support similar behavior*/
                mywriter.Write("<table class=\"wgmenunavigation\" cellpadding=\"0\" cellspacing=\"0\"><tr align=\"left\">");
                bool createmenuspace = false;

                foreach (Grid slavegrid in m_Grid.SlaveGrid)
                {
                    if (createmenuspace)
                        mywriter.Write("<td class=\"wgmenuspacing\"></td>");
                    else
                        createmenuspace = true;

                    if (m_Grid.ActiveMenuSlaveGrid != slavegrid)
                    {
                        string colorstyle = null;
                        if (Grid.MenuInactiveColor != Color.Empty)
                            colorstyle = string.Format("style=\"background-color:{0}\"",
                                                       Grid.ColorToHtml(Grid.MenuInactiveColor));

                        if (m_Grid.IsUsingJQueryUICSSFramework)
                            mywriter.Write(
                                "<td id=\"wgmenu_{0}_{1}\">{2}</td>",
                                m_Grid.ClientID, slavegrid.ClientID, m_Grid.SlaveGrid.GetSlaveGridAnchor(slavegrid));
                        else
                            mywriter.Write(
                                "<td id=\"wgmenu_{0}_{1}\"><div class=\"wglefttab_off\"></div><div {2} class=\"wginactivemenu wgrighttab_off\">{3}</div></td>",
                                m_Grid.ClientID, slavegrid.ClientID, colorstyle,
                                m_Grid.SlaveGrid.GetSlaveGridAnchor(slavegrid));
                    }
                    else
                    {
                        string colorstyle = null;
                        if (Grid.MenuActiveColor != Color.Empty)
                            colorstyle =
                                string.Format("style=\"background-color:{0}\"", Grid.ColorToHtml(Grid.MenuActiveColor));

                        if (m_Grid.IsUsingJQueryUICSSFramework)
                            mywriter.Write(
                                "<td id=\"wgmenu_{0}_{1}\">{2}</td>",
                                m_Grid.ClientID, slavegrid.ClientID, m_Grid.SlaveGrid.GetSlaveGridAnchor(slavegrid));
                        else
                            mywriter.Write(
                                "<td id=\"wgmenu_{0}_{1}\"><div class=\"wglefttab_on\"></div><div {2} class=\"wgactivemenu wgrighttab_on\">{3}</div></td>",
                                m_Grid.ClientID, slavegrid.ClientID, colorstyle,
                                m_Grid.SlaveGrid.GetSlaveGridAnchor(slavegrid));
                    }
                }
                mywriter.Write("</tr></table></td></tr></table><br/>");
                mywriter.Write("<!--WebGrid menu finished rendering -->\n");
            }

            #endregion

            if (m_Grid.SystemMessage.Count > 0 && systemMessagetriggered == false)
            {
                mywriter.Write(m_Grid.SystemMessage.RenderSystemMessage);
                if (m_Grid.Scripts == null || !m_Grid.Scripts.DisableBookmark)
                    Grid.AddClientScript(mywriter,
                                         string.Format("bookmarkscroll.init();bookmarkscroll.scrollTo('wgSystemMessage_{0}')",
                                                       m_Grid.ID));
            }
            mywriter.Write("<!--WebGrid grid starts rendering -->\n");
            mywriter.Write("{0} {1} cellpadding=\"0\" class=\"wgmaingrid\" cellspacing=\"0\" id=\"wgContent_{2}\"><tr><td>",
                         webAttributes, width, m_Grid.ID);
            ApplyAjaxLoader(mywriter);
            if (m_Grid.SystemMessage.CriticalCount == 0)
            {
                switch (m_Grid.DisplayView)
                {
                    case DisplayView.Grid:
                        if (m_Grid.AllowEditInGrid && m_Grid.PageIndex == 0 && m_Grid.Page != null)
                        {
                            if (m_Grid.EnableCallBack)
                                mywriter.Write(
                                    "<input type=\"hidden\" id=\"{0}_where\" name=\"{0}_where\" value=\"{1}\" />",
                                    m_Grid.ClientID, HttpUtility.UrlEncode(m_Grid.FilterExpression, Encoding.Default));
                            else
                                m_Grid.Page.ClientScript.RegisterHiddenField(
                                    string.Format("{0}_where", m_Grid.ClientID),
                                    HttpUtility.UrlEncode(m_Grid.FilterExpression, Encoding.Default));
                        }
                        RenderGrid(ref m_Grid, ref mywriter);
                        break;
                    case DisplayView.Detail:

                        RenderDetail(ref m_Grid, ref mywriter);
                        break;
                }
            }

            mywriter.Write("</td></tr></table>");
             mywriter.Write("<!--WebGrid grid finished rendering -->\n");

            if (m_Grid.Page != null && (m_Grid.Scripts == null || !m_Grid.Scripts.DisableWebGridClientObject))
            {
                m_Grid.JsOnData.AppendLine();
                m_Grid.JsOnData.Append(WebGridClientObject.GetGridObject(m_Grid));
            }

            if (m_Grid.Scripts == null || !m_Grid.Scripts.DisableContextMenu)
            {
                m_Grid.GetContextMenuHtml(mywriter);
                Grid.AddClientScript(mywriter, string.Format(contextscript, m_Grid.ID));
            }
            if ( m_Grid.ClientNotifications != null && m_Grid.ClientNotifications.Count > 0 && (m_Grid.Scripts == null || !m_Grid.Scripts.DisableClientNotification) )
            {
                ClientNotification clientSettings = m_Grid.ClientNotification ?? Grid.DefaultClientNotification;

                if (clientSettings.HeaderText == null)
                    clientSettings.HeaderText = m_Grid.Title;
                if (clientSettings.HeaderText == null)
                    clientSettings.HeaderText = string.Empty;

                StringBuilder template = string.IsNullOrEmpty(clientSettings.Container)
                                             ?
                                                 new StringBuilder(
                                                     "$(document).ready( function() {{ $.jGrowl(\"{0}\", {{ header: '")
                                             :
                                                 new StringBuilder(
                                                     string.Format(
                                                         "$(document).ready( function() {{ $('#{0}').jGrowl(\"{0}\", {{ header: '",
                                                         clientSettings.Container));

                template.Append(clientSettings.HeaderText.Replace("\"", "\\"));
                template.AppendFormat("', sticky: {0}", clientSettings.Sticky.ToString().ToLower());
                template.AppendFormat(", life: {0}", clientSettings.LifeSpan);
                if (clientSettings.CloserTemplate != null)
                    template.AppendFormat(", closerTemplate: '{0}'",
                                          clientSettings.CloserTemplate.Replace("\"", "\\"));
                template.Append(", closer: " + clientSettings.Closer.ToString().ToLower());
                if (!string.IsNullOrEmpty(clientSettings.CssClass))
                    template.AppendFormat(", theme: '{0}'", clientSettings.CssClass);
                template.Append("}}); }});");

                if (m_Grid.ClientNotifications.Count > 0)

                    foreach (string arg in m_Grid.ClientNotifications)
                        Grid.AddClientScript(mywriter,
                                             string.Format(template.ToString(), arg.Replace("\"", "\\")));
            }

            if ( (m_Grid.Scripts == null || !m_Grid.Scripts.DisableInputMask) && m_Grid.MaskedColumns.Count > 0)
            {
                StringBuilder masks = new StringBuilder();

                foreach (KeyValuePair<string,string> dictionary in m_Grid.MaskedColumns)
                {
                    if (masks.Length > 3)
                        masks.Append(", ");
                    masks.AppendFormat("{0}:{{ mask: {1} }}", dictionary.Key, dictionary.Value);
                }
                string options;
                if (m_Grid.MaskedInput != null)
                    options = string.Format("{{selectCharsOnFocus: {0},textAlign: {1},autoTab:{2}, attr: '{3}'}}",
                                         MaskedInput.GetStringValue(m_Grid.MaskedInput.EnableSelectCharsOnFocus),
                                         MaskedInput.GetStringValue(m_Grid.MaskedInput.EnableTextAlign),
                                         MaskedInput.GetStringValue(m_Grid.MaskedInput.EnableAutoTab),
                                         m_Grid.MaskedInput.Attribute);
                else
                    options = "{selectCharsOnFocus: true,textAlign: false,autoTab:false, attr: 'alt'}";

                Grid.AddClientScript(mywriter, string.Format(meiomask, m_Grid.ID, masks, options));
            }
            if (Anthem.Manager.IsCallBack && (m_Grid.Scripts == null || !m_Grid.Scripts.DisableHoverButtons))
                Grid.AddClientScript(mywriter, Grid.HoverButtonScript);
            if (m_Grid.JsOnData.Length > 2)
                mywriter.Write(@"<script type=""text/javascript"">{0}</script>", m_Grid.JsOnData.ToString());

            mywriter.Close();
            if (m_Grid.Trace.IsTracing)
                m_Grid.Trace.Trace("Finish Render();");
        }

        private void ApplyAjaxLoader(WebGridHtmlWriter mywriter)
        {
            if (!m_Grid.EnableCallBack || (m_Grid.Scripts != null && m_Grid.Scripts.DisableAjaxLoader) ) 
                return;

            if (m_Grid.IsUsingJQueryUICSSFramework)
                mywriter.Write("<div class=\"ui-widget-overlay wgAjaxLoader\">");
            else
                mywriter.Write("<div class=\"wgAjaxLoader\">");
            if (m_Grid.AjaxLoaderTemplate == null)
            {
                string img = "Loading.";
                if (Grid.GotHttpContext)
                    img = string.Format("<img title=\"Loading data\" alt=\"Loading=\" src=\"{0}\" />",
                                        m_Grid.Page.ClientScript.GetWebResourceUrl(GetType(),
                                                                                   "WebGrid.Resources.images.loading.gif")
                                            .Replace("&", "&amp;"));
                mywriter.Write("<div class=\"wgAjaxLoaderContent\" id=\"{0}_AjaxLoader\">{1}</div>",
                               m_Grid.ID, img);
            }
            else
            {

                mywriter.Write("<div class=\"wgAjaxLoaderContent\" id=\"{0}_AjaxLoader\">", m_Grid.ID);
                mywriter.Write( m_Grid.AjaxLoaderTemplate);
                mywriter.Write("</div>");
            }
            mywriter.Write("</div>");
        }

        #endregion Methods
    }
}