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
namespace WebGrid
{
    /// <summary>
    /// WebGrid Client object
    /// </summary>
    public static class WebGridClientObject
    {
        #region Fields

        const string gridobject = @"
        function {0}() {{
           this.title = '{2}';
           this.rows = {3};
           this.editIndex = '{4}'
         }}
        {0}.prototype.refresh = function() {{
        try{{$.fn.colorbox.close();}}catch (err){{}}

        {1};
        }};
        {0} = new {0}();

        ";

        #endregion Fields

        #region Methods

        internal static string GetGridObject(Grid grid)
        {
            string refresh = grid.EnableCallBack
                                 ? string.Format(
                                       @"setActiveGrid('{1}');Anthem_InvokeControlMethod(""{0}"", ""CallBackMethod"", ""refresh"", null, null);",
                                       grid.ClientID,grid.ID)
                                 : grid.Page.ClientScript.GetPostBackEventReference(grid, "refresh");
            return string.Format(gridobject, grid.ClientObjectId ?? grid.ID, refresh, Util.Json.Utilities.JavaScriptUtils.ToEscapedJavaScriptString(grid.Title, '\'', false), grid.RecordCount, Util.Json.Utilities.JavaScriptUtils.ToEscapedJavaScriptString(grid.EditIndex, '\'', false));
        }

        #endregion Methods
    }
}