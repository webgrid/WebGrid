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
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Web;

    /// <summary>
    /// Summary description for ControlManager.
    /// </summary>
    public sealed class ControlManager
    {
        #region Methods

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="imageId">The image id.</param>
        /// <returns></returns>
        public static byte[] GetImage(string imageId)
        {
            return LoadImageResource(string.Format("images.toolbar.{0}", imageId));
        }

        /// <summary>
        /// Returns HtmlEditor resource
        /// </summary>
        /// <param name="resourceType">Type of the resource.</param>
        /// <param name="resourceId">The resource id.</param>
        /// <param name="browser">The browser.</param>
        /// <param name="cssFile">The CSS file.</param>
        /// <returns></returns>
        public static string GetPage(string resourceType, string resourceId, string browser, string cssFile)
        {
            switch (resourceType)
            {
                case "editor":
                    return MakeEditor(resourceId, browser, cssFile);
                case "dialog":
                    return MakeDialog(resourceId);
                case "blank":
                    return "<html><head></head><body></body><html>";
                case "imagebrowser":
                    return MakeImageBrowser(resourceId, browser);
                default:
                    return String.Empty;
            }
        }

        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="imagePath">The image path.</param>
        /// <returns></returns>
        public static bool UploadFile(HttpFileCollection files, string imagePath)
        {
            for (int i = 0; i < files.Count; i++)
            {
                if (0 >= files[i].ContentLength) continue;
                int lastIndex = files[i].FileName.LastIndexOf("\\");
                string fileName = files[i].FileName;
                if (-1 != lastIndex)
                    fileName = files[i].FileName.Substring(files[i].FileName.LastIndexOf("\\"));
                files[i].SaveAs(imagePath + fileName);
            }
            return true;
        }

        private static byte[] LoadImageResource(string resourceName)
        {
            Type type = Type.GetType("WebGrid.Util.HtmlEditor");
            Assembly myAssembly = Assembly.GetAssembly(type);
            Stream resStream = myAssembly.GetManifestResourceStream(string.Format("WebGrid.Util.HtmlEditor.{0}", resourceName));
            int length = (int) resStream.Length;
            BinaryReader br = new BinaryReader(resStream);
            byte[] output = br.ReadBytes(length);
            br.Close();
            resStream.Close();
            return output;
        }

        private static string LoadStringResource(string resourceName)
        {
            Type type = Type.GetType("WebGrid.Util.HtmlEditor");
            Assembly myAssembly = Assembly.GetAssembly(type);
            Stream resStream = myAssembly.GetManifestResourceStream(string.Format("WebGrid.Util.HtmlEditor.{0}", resourceName));
            StreamReader sr = new StreamReader(resStream);
            string resource = sr.ReadToEnd();
            sr.Close();
            if (resStream != null) resStream.Close();
            return resource;
        }

        private static string MakeDialog(string dialogId)
        {
            return LoadStringResource("dialogs." + dialogId);
        }

        private static string MakeEditor(string controlId, string browser, string cssFile)
        {
            StringBuilder editor = new StringBuilder();
            editor.Append("<html><head><script>");
            // make engine

            editor.AppendFormat(" var clientId = '{0}';", controlId);

            string browserScript = LoadStringResource("scripts.browser.js");
            string configScript = LoadStringResource("scripts.config.js");
            string engineScript = LoadStringResource("scripts.engine.js");
            string loaderScript = LoadStringResource("scripts.loader.js");
            // moved from loader.js

            string eventsScript = LoadStringResource("scripts.events.js");
            string constScript = LoadStringResource("scripts.constant.js");
            string cleanupScript = LoadStringResource("scripts.cleanup.js");

            string domutilsScript = LoadStringResource("scripts.domutils.js");
            string xhtmlScript = LoadStringResource("scripts.XHTML.js");
            string entitiesScript = LoadStringResource("scripts.entities.js");

            string engineBrowserScript = LoadStringResource(string.Format("scripts.engine{0}.js", browser));
            string domutilsBrowserScript = LoadStringResource(string.Format("scripts.domutils{0}.js", browser));
            string xhtmlBrowserScript = LoadStringResource(string.Format("scripts.XHTML{0}.js", browser));

            // load ui & commands
            string commandsScript = LoadStringResource("scripts.commands.Command.js");
            string cppCommandsScript = LoadStringResource("scripts.commands.ccpcommands.js");
            string dialogCommandScript = LoadStringResource("scripts.commands.dialogcommand.js");
            string modeCommandScript = LoadStringResource("scripts.commands.modecommand.js");
            string commandCollectionScript = LoadStringResource("scripts.commands.commandcollection.js");

            string buttonScript = LoadStringResource("scripts.ui.button.js");
            string comboboxScript = LoadStringResource("scripts.ui.combobox.js");
            string dialogScript = LoadStringResource("scripts.ui.dialog.js");
            string fontnameScript = LoadStringResource("scripts.ui.fontname.js");
            string fontsizeScript = LoadStringResource("scripts.ui.fontsize.js");
            string separatorScript = LoadStringResource("scripts.ui.separator.js");
            string switchbuttonScript = LoadStringResource("scripts.ui.switchbutton.js");
            string toolbarScript = LoadStringResource("scripts.ui.toolbar.js");
            string toolbarCollectionScript = LoadStringResource("scripts.ui.toolbaritemcollection.js");
            string toolbarsScript = LoadStringResource("scripts.ui.toolbars.js");

            editor.Append(browserScript);
            editor.Append(configScript);
            editor.Append(engineScript);
            // moved from loader.js
            editor.Append(eventsScript);
            editor.Append(constScript);
            editor.Append(cleanupScript);

            editor.Append(engineBrowserScript);
            editor.Append(domutilsScript);
            editor.Append(domutilsBrowserScript);

            editor.Append(xhtmlScript);
            editor.Append(entitiesScript);
            //  browser specific files

            editor.Append(xhtmlBrowserScript);

            // ui & command
            editor.Append(commandsScript);
            editor.Append(cppCommandsScript);
            editor.Append(dialogCommandScript);
            editor.Append(modeCommandScript);
            editor.Append(commandCollectionScript);

            editor.Append(buttonScript);
            editor.Append(comboboxScript);
            editor.Append(dialogScript);
            editor.Append(fontnameScript);
            editor.Append(fontsizeScript);
            editor.Append(separatorScript);
            editor.Append(switchbuttonScript);
            editor.Append(toolbarScript);
            editor.Append(toolbarCollectionScript);
            editor.Append(toolbarsScript);

            editor.Append(loaderScript);
            editor.Append("</script>");

            if (string.IsNullOrEmpty(cssFile) == false)
                editor.Append("<link type=\"text/css\" href=\"" + cssFile + "\" rel=\"stylesheet\" />");
            else
            {
                editor.Append("<style>");
                string editorStyle = LoadStringResource("styles.editor.css");
                editor.Append(editorStyle);
                editor.Append("</style>");
            }
            editor.Append("</head><body>");
            string editorBody = LoadStringResource("editor.htm");
            editor.Append(editorBody);
            editor.Append("</body></html>");

            return editor.ToString();
        }

        private static string MakeImageBrowser(string imagePath, string url)
        {
            StringBuilder page = new StringBuilder();
            page.Append("<html><head><title>Images</title><script> function SelectFile(fileName)");
            page.Append("{ opener.SetElementValue(\"url\",");
            page.AppendFormat("'{0}/'", url);
            page.Append("+ fileName); window.close(); }</script></head><body>");
            page.Append("<table border=0 width=100% ");

            DirectoryInfo di = new DirectoryInfo(imagePath);
            FileInfo[] files = di.GetFiles();
            foreach (FileInfo file in files)
            {

                if (".jpg" == file.Extension.ToLowerInvariant() || ".gif" == file.Extension.ToLowerInvariant()  ||
                    ".gif" == file.Extension.ToLowerInvariant()  || ".bmp" == file.Extension.ToLowerInvariant()  ||
                    ".jpeg" == file.Extension.ToLowerInvariant() )
                    page.AppendFormat(
                        "<tr><td onmouseover='this.style.backgroundColor=\"#c1d2ee\"' onmouseout='this.style.backgroundColor=\"#ffffff\"' onclick=\"SelectFile('{0}')\">{1}</td></tr>",
                        file.Name, file.Name);
            }

            page.AppendFormat("</table><form name='frmUpload' method='post' enctype='multipart/form-data' action='htmleditor.aspx?res=upload&id={0}'> ", url);
            page.Append("<table width=100%><tr><td><input type=\"file\" name=fUpload></td></tr>");
            page.Append("<tr><td><input type='submit' value='Upload'></td></tr></table></form>");
            page.Append("</body></html>");
            return page.ToString();
        }

        #endregion Methods
    }
}