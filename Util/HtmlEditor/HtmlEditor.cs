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

namespace WebGrid.Util
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using AttributeCollection = System.ComponentModel.AttributeCollection;
    using System.Drawing;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using WebGrid.Config;

    #region Enumerations

    /// <summary>
    /// Determines which mode the editor is in.
    /// </summary>
    public enum EditorMode
    {
        /// <summary>
        /// What you see is what you get design mode.
        /// </summary>
        Design,
        /// <summary>
        /// HTML editing mode
        /// </summary>
        Source
    }

    /// <summary>
    /// Html formats that are supported by the editor.
    /// </summary>
    public enum SourceFormat
    {
        /// <summary>
        /// Html (The HyperText Markup Language)
        /// </summary>
        Html,
        /// <summary>
        /// Xhtml (The eXtensible HyperText Markup Language)
        /// </summary>
        Xhtml
    }

    #endregion Enumerations

    /// <summary>
    /// WYSIWYG Html editor component
    /// </summary>
    [ParseChildren(false)]
    [DefaultProperty("Text")]
    [Designer("WebGrid.Util.HtmlEditorDesigner")]
    [ToolboxData("<{0}:HtmlEditor runat=server></{0}:HtmlEditor>")]
    public class HtmlEditor : Control, IPostBackDataHandler
    {
        #region Fields

        private bool isRendered;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the background color of the editing document. 
        /// </summary>
        [Category("Appearance")]
        [Description("The document color used for editing document.")]
        public Color BackColor
        {
            get { return ControlStyle.BackColor; }
            set { ControlStyle.BackColor = value; }
        }

        /// <summary>
        /// Sets or gets the base href url of the editing document.
        /// </summary>
        [Category("Appearance")]
        [Description("The base href to be used for editing document.")]
        public string BaseHref
        {
            get
            {
                object o = ViewState["basehref"];
                return (null == o) ? String.Empty : (string) o;
            }
            set { ViewState["basehref"] = value; }
        }

        /// <summary>
        /// Gets or sets the application-relative virtual directory of the HtmlEditor scripts that contains this control. 
        /// </summary>
        [Category("Appearance")]
        [DefaultValue("~/htmleditor/")]
        [Description("The URL of the editor directory.")]
        public string BasePath
        {
            get
            {
                object o = ViewState["basepath"];
                if (null == o || " " == o.ToString())
                {
                    AttributeCollection attributes = TypeDescriptor.GetProperties(this)["BasePath"].Attributes;
                    DefaultValueAttribute myAttribute =
                        (DefaultValueAttribute) attributes[typeof (DefaultValueAttribute)];
                    ViewState["basepath"] = myAttribute.Value.ToString();
                }
                return ViewState["basepath"].ToString();
            }
            set { ViewState["basepath"] = value; }
        }

        /// <summary>
        /// Gets or sets external css file for the editing document
        /// </summary>
        [Category("Appearance")]
        [Description("The CSS file applied to the editing document.")]
        public string CssLink
        {
            get
            {
                object o = ViewState["csslink"];
                return (null == o) ? String.Empty : (string) o;
            }
            set { ViewState["csslink"] = value; }
        }

        /// <summary>
        /// Gets or sets document type of the editing document.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue("<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">")]
        [Description("The document type of the editing document")]
        public string DocumentType
        {
            get
            {
                object o = ViewState["documenttype"];
                if (null == o)
                {
                    AttributeCollection attributes = TypeDescriptor.GetProperties(this)["DocumentType"].Attributes;
                    DefaultValueAttribute myAttribute =
                        (DefaultValueAttribute) attributes[typeof (DefaultValueAttribute)];
                    ViewState["documenttype"] = myAttribute.Value.ToString();
                }
                return ViewState["documenttype"].ToString();
            }
            set { ViewState["documenttype"] = value; }
        }

        /// <summary>
        /// Convert double linefeeds as paragraph element during past text as plain text.
        /// </summary>
        [Category("Cut'n'Copy")]
        [DefaultValue(true)]
        [Description("Convert double linefeeds as paragraph element.")]
        public bool DoubleLFasP
        {
            get { return (null == ViewState["doublelfasp"]) ? true : (bool) ViewState["doublelfasp"]; }
            set { ViewState["doublelfasp"] = value; }
        }


        /// <summary>
        /// Gets ot sets external CSS of the editor
        /// </summary>
        [Category("Appearance")]
        [Description("Gets ot sets external CSS of the editor")]
        [DefaultValue("")]
        public string EditorCss
        {
            get
            {
                object o = ViewState["editorcss"];
                return null == o ? String.Empty : (string) o;
            }
            set { ViewState["editorcss"] = value; }
        }

        /// <summary>
        /// Gets ot sets external path of toolbar images
        /// </summary>
        [Category("Appearance")]
        [Description("Gets ot sets external path of toolbar images")]
        [DefaultValue("")]
        public string EditorImagePath
        {
            get
            {
                object o = ViewState["editorimagepath"];
                return null == o ? String.Empty : (string) o;
            }
            set { ViewState["editorimagepath"] = value; }
        }

        /// <summary>
        /// Returns a Font object that represents the default font of the editor.
        /// </summary>
        [Category("Appearance")]
        [Description("The font used as default for editing document.")]
        public FontInfo Font
        {
            get { return ControlStyle.Font; }
        }

        /// <summary>
        /// Gets or sets the color of the text of the control. 
        /// </summary>
        [Category("Appearance")]
        [Description("The text color used as default for editing document.")]
        public Color ForeColor
        {
            get { return ControlStyle.ForeColor; }
            set { ControlStyle.ForeColor = value; }
        }

        /// <summary>
        /// Gets or sets the heightEditableContent of the control.
        /// </summary>
        [Category("Layout")]
        [Description("The height of the control")]
        public Unit Height
        {
            get { return ControlStyle.Height; }
            set { ControlStyle.Height = value; }
        }

        /// <summary>
        /// Gets or sets the application-relative virtual directory of the user images.
        /// Web.Config key for this property is "WGEditorImagePath". 
        /// </summary>
        /// <value>The image path.</value>
        [Category("Appearance")]
        [Description("The URL of the directory where user images to be stored.")]
        public string ImagePath
        {
            get
            {
                object o = ViewState["imagepath"] ?? GridConfig.Get("WGEditorImagePath", null as string);
                return (null == o) ? "~/" : (string) o;
            }
            set { ViewState["imagepath"] = value; }
        }

        /// <summary>
        /// Gets a value indicating whether browser is compatible. 
        /// </summary>
        [Browsable(false)]
        public bool IsCompatible
        {
            get { return CheckBrowserCompatibility(); }
        }

        /// <summary>
        /// Gets ot sets editor mode
        /// </summary>
        [Category("Appearance")]
        [Description("The behavior mode of the editor")]
        [DefaultValue(typeof (EditorMode), "Design")]
        public EditorMode Mode
        {
            get
            {
                object o = ViewState["mode"];
                return null == o ? EditorMode.Design : (EditorMode) o;
            }
            set { ViewState["mode"] = value; }
        }

        /// <summary>
        /// Convert one linefeed as line break element during past text as plain text. 
        /// </summary>
        [Category("Cut'n'Copy")]
        [DefaultValue(true)]
        [Description("Convert one linefeed as line break element during past text as plain text")]
        public bool OneLFasBR
        {
            get { return (null == ViewState["onelfasbr"]) ? true : (bool) ViewState["onelfasbr"]; }
            set { ViewState["onelfasbr"] = value; }
        }

        /// <summary>
        /// Gets or sets the text content of the HtmlEditor control. 
        /// </summary>
        [DefaultValue("")]
        [Category("Appearance")]
        [Description("The text to be edited.")]
        public string Text
        {
            get
            {
                object o = ViewState["text"];
                return (null == o) ? String.Empty : (string) o;
            }
            set { ViewState["text"] = value; }
        }

        /// <summary>
        /// Gets or sets the returned text format.
        /// </summary>
        [Category("Appearance")]
        [Description("The text format to be returned.")]
        [DefaultValue(typeof (SourceFormat), "XHTML")]
        public SourceFormat TextFormat
        {
            get
            {
                object o = ViewState["sourceformat"];
                return (null == o) ? SourceFormat.Xhtml : (SourceFormat) o;
            }
            set { ViewState["sourceformat"] = value; }
        }

        /// <summary>
        /// Gets or sets editor toolbars and toolbarses' items
        /// </summary>
        [Category("Appearance")]
        [Description("The editor toolbars")]
        public string ToolBars
        {
            get
            {
                object o = ViewState["toolbars"];
                return null == o ? string.Empty : o.ToString();
            }
            set { ViewState["toolbars"] = value; }
        }

        /// <summary>
        /// Insert <br/> on Enter.
        /// </summary>
        [Category("behavior")]
        [DefaultValue(false)]
        [Description("Insert <BR/> on Enter")]
        public bool UserBRonCarriageReturn
        {
            get { return null != ViewState["usebroncr"] && (bool) ViewState["usebroncr"]; }
            set { ViewState["usebroncr"] = value; }
        }

        /// <summary>
        /// Gets or sets the widthEditableArea of the control. 
        /// </summary>
        [Category("Layout")]
        [Description("The width of the control")]
        public Unit Width
        {
            get { return ControlStyle.Width; }
            set { ControlStyle.Width = value; }
        }

        /// <summary>
        /// Gets a collection of text attributes that will be used as a default styles on the editing document. 
        /// </summary>
        private Style ControlStyle
        {
            get
            {
                object o = ViewState["controlstyle"];
                if (null == o)
                {
                    ViewState["controlstyle"] = new Style();
                }
                return (Style) ViewState["controlstyle"];
            }
            /*
            set { ViewState["controlstyle"] = value; }
            */
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Processes post back-data for a control. 
        /// </summary>
        /// <param name="postDataKey">The key identifier for the control.</param>
        /// <param name="postCollection">The collection of all incoming name values.</param>
        /// <returns>true if the server control's state changes as a result of the post back; otherwise, false.</returns>
        public bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            if (postCollection[postDataKey] != Text)
            {
                Text = postCollection[postDataKey];
                return true;
            }
            return false;
        }

        /// <summary>
        /// Signals the server control to notify the ASP.NET application that the state of the control has changed. 
        /// </summary>
        public void RaisePostDataChangedEvent()
        {
            // nothing
        }

        /// <summary>
        /// Render this control to the output parameter specified.
        /// </summary>
        /// <param name="writer">The Html writer to write out to</param>
        public void RenderEditor(HtmlTextWriter writer)
        {
            Render(writer);
            isRendered = true;
        }

        /// <summary> 
        /// Render this control to the output parameter specified.
        /// </summary>
        /// <param name="output"> The HTML writer to write out to </param>
        protected override void Render(HtmlTextWriter output)
        {
            // control is already rendered
            if (isRendered)
                return;
            output.Write("<div>");
            if (CheckBrowserCompatibility())
            {
                string basePath = "~/HtmlEditor.aspx";
                if (basePath.StartsWith("~"))
                    basePath = ResolveUrl(basePath);
                //if (!File.Exists(MapPathSecure(basePath)))
                //{
                //    // Directory not found throw BasePath Exception
                //    throw new BasePathException(basePath);
                //}
                basePath += string.Format("?res=editor&id={0}", ClientID);

                if (string.IsNullOrEmpty(EditorCss) == false)
                {
                    string editorCSS = EditorCss.StartsWith("~") ? ResolveUrl(EditorCss) : EditorCss;
                    basePath += string.Format("&cssFile={0}", editorCSS);
                }
                //basePath += "editor.htm?ClientId=" + ClientID;

                // make hidden field that store configuration settings
                output.Write("<input type=\"hidden\" id=\"{0}__CONFIG\" name=\"{1}__CONFIG\" value=\"{2}\" />",
                             ClientID, ClientID, GetConfigurationString());

                //Stupid work-around for CallBack events. It fixes the Anthem.Net have 'BADRESPONSE'
                if(!string.IsNullOrEmpty(Text))
                    Text = Text.Replace("\"","'");

                // make hidden filed that referenced to the edited document
                output.Write("<input type=\"hidden\" id=\"{0}\" name=\"{1}\"  value=\"{2}\"    />",
                             ClientID, ClientID, HttpUtility.HtmlEncode(Text));

                // make editor's IFRAME.
            //				output.Write("<iframe id=\"{0}__Frame\" src=\"{1}\" widthEditableArea=\"{2}\" heightEditableContent=\"{3}\" frameborder=\"no\" scrolling=\"no\"></iframe>",
            //				             ClientID, basePath, WidthEditableColumn, HeightEditableColumn);
                output.Write(
                    "<iframe id=\"{0}__Frame\" src=\"{1}\" width=\"{2}\" height=\"{3}\" frameborder=\"no\" scrolling=\"no\"></iframe>",
                    ClientID, basePath, Width, Height);
            }
            else
            {
                output.Write(
                    "<textarea name=\"{0}\" rows=\"4\" cols=\"40\" style=\"width: {1}; height: {2}\" >{3}</textarea>",
                    ClientID,
                    Width,
                    Height,
                    HttpUtility.HtmlEncode(Text));
            }
            output.Write("</div>");
        }

        /// <summary>
        /// Checks browser compatibility. 
        /// </summary>
        /// <returns>true if browser is compatible; otherwise, false</returns>
        private static bool CheckBrowserCompatibility()
        {
            if (!Grid.GotHttpContext || HttpContext.Current.Request.UserAgent == null)
                return true;
            HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
            if (browser.Browser.Equals("IE", StringComparison.OrdinalIgnoreCase) && (browser.MajorVersion >= 6 ||
                                                                                     (browser.MajorVersion == 5 &&
                                                                                      browser.MinorVersion >= 0.5)) &&
                browser.Win32
                )
                return true;

            if (browser.Browser.Equals("opera", StringComparison.OrdinalIgnoreCase) && 9 <= browser.MajorVersion)
                return true;

            Match oMatch = Regex.Match(HttpContext.Current.Request.UserAgent, @"(?<=Gecko/)\d{8}");

            return (oMatch.Success && int.Parse(oMatch.Value, CultureInfo.InvariantCulture) >= 20030210);
        }

        /// <summary>
        /// Convert a control settings into formatted string that is usable on the javascript. 
        /// </summary>
        /// <returns>The javascript configuration string.</returns>
        private string GetConfigurationString()
        {
            StringBuilder configString = new StringBuilder();
            string basePath = BasePath;
            if (basePath.StartsWith("~"))
                basePath = ResolveUrl(basePath);
            configString.AppendFormat("{0}={1}", "basepath", HttpUtility.UrlEncode(basePath));
            configString.Append("&");
            configString.AppendFormat("{0}={1}", "doctype", HttpUtility.UrlEncode(DocumentType));
            configString.Append("&");
            configString.AppendFormat("{0}={1}", "textformat", TextFormat);
            configString.Append("&");
            string imagePath = ImagePath.StartsWith("~") ? ResolveUrl(ImagePath) : ImagePath;
            configString.AppendFormat("{0}={1}", "imagepath", imagePath);
            configString.Append("&");

            if (string.IsNullOrEmpty(EditorImagePath) == false)
            {
                string toolbarImages = EditorImagePath.StartsWith("~") ? ResolveUrl(EditorImagePath) : EditorImagePath;
                configString.AppendFormat("{0}={1}", "editorimagepath", toolbarImages);
                configString.Append("&");
            }
            if (string.IsNullOrEmpty(EditorCss) == false)
            {
                string editorCSS = EditorCss.StartsWith("~") ? ResolveUrl(EditorCss) : EditorCss;
                configString.AppendFormat("{0}={1}", "editorcss", editorCSS);
                configString.Append("&");
            }
            configString.AppendFormat("{0}={1}", "mode", Mode.ToString("D"));
            configString.Append("&");
            configString.AppendFormat("{0}={1}", "usebroncr", UserBRonCarriageReturn);
            configString.Append("&");
            configString.Append("fontsize=");
            int i = 1;
            foreach (FontUnit u in TypeDescriptor.GetConverter(ControlStyle.Font.Size).GetStandardValues())
            {
                if (3 < i)
                    configString.AppendFormat("{0}*{1},", i - 3, u);
                i++;
            }
            configString.Remove(configString.Length - 1, 1);
            configString.Append("&fonts=");
            if (0 != ControlStyle.Font.Names.Length)
            {
                for (i = 0; i < ControlStyle.Font.Names.Length; i++)
                {
                    configString.AppendFormat("{0};", ControlStyle.Font.Names[i]);
                }
                configString.Remove(configString.Length - 1, 1);
            }
            else
            {
                // default font
                configString.Append("Arial;Times New Roman;Verdana;Tahoma");
            }

            return configString.ToString();
        }

        #endregion Methods
    }
}