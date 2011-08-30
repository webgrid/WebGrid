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
namespace WebGrid.Design
{
    /// <summary>
    /// This class contains methods to generate a standard help text. 
    /// </summary>
    internal static class Help
    {
        #region Methods

        public static string GenerateHelp(string imagePath)
        {
            //						<span class=""wgHeader"">Grid of symbols or icons used in&nbsp;  WebGrid</span>
            //						<br/>
            const string help = @"
            <table>
                <tr>
                    <td>
                        <table class=""wgmaingrid"">
            <tr class=""wgrow"">
            <td class=""wgtitle"" width=""80"">
                Icon
            </td>
            <td class=""wgtitle"">
                Description
            </td>
            </tr>
            <tr class=""wgrow"">
            <td class=""wggridcell"">
                <span title=""delete row icon"" class=""ui-icon ui-icon-trash"" />
            </td>
            <td class=""wggridcell"">
                Delete a record from the datasource.
            </td>
            </tr>
            <tr class=""wgrow"">
            <td class=""wggridcell"">
                <div title=""First record icon"" class=""ui-icon ui-icon-seek-first""/>&nbsp;<div title=""Last record icon""
                    class=""ui-icon ui-icon-seek-end""/>
            </td>
            <td class=""wggridcell"">
                Navigate to first or last record in the recordset.
            </td>
            </tr>
            <tr class=""wgrow"">
            <td class=""wggridcell"">
                <span title=""previous record icon"" class=""ui-icon ui-icon-seek-prev""/>&nbsp;<span
                    title=""next record icon"" class=""ui-icon ui-icon-seek-next""/>
            </td>
            <td class=""wggridcell"">
                Navigate to previous or next page in the recordset.
            </td>
            </tr>
            <tr class=""wgrow"">
            <td class=""wggridcell"">
                <span title=""column that requires data"" class=""ui-icon ui-icon-triangle-1-s"" />
            </td>
            <td class=""wggridcell"">
                Sorts the data descending based on the column you currently click on.
            </td>
            </tr>
            <tr class=""wgrow"">
            <td class=""wggridcell"">
                <span title=""column that requires data"" class=""ui-icon ui-icon-triangle-1-n"" />
            </td>
            <td class=""wggridcell"">
                Sorts the data ascending based on the column you currently click on.
            </td>
            </tr>
            <tr class=""wgrow"">
            <td class=""wggridcell"">
                <span title=""column that requires data"" class=""ui-icon ui-icon-info"" />
            </td>
            <td class=""wggridcell"">
                Indicates the column requires data.
            </td>
            </tr>
            <tr>
            <td colspan=""2""><br/><br/>
                <h3>
                    Creating a new record</h3>
                If you create a new record using WebGrid, the input-fields for this record can be
                populated with default values (DefaultValue), and data is only saved when there
                are no ""system messages"" printed.<br />
                <br />
                <i><b>Note:</b> WebGrid does not automatically update data in callback and postbacks.</i>
            </td>
            </tr>
            <tr>
            <td colspan=""2""><br/>
                <h3>
                    Updating an existing record</h3>
                If you update an existing record using WebGrid, the input-fields retrieves data
                from the data source, and only fields that has been changed is updated against the
                data source<br />
                <br />
                <i><b>Note:</b> WebGrid does not automatically update data in callback and postbacks.</i>
            </td>
            </tr>
            <tr>
            <td colspan=""2""><br/>
                <h3>
                    Deleting an record</h3>
                Before deleting an data source record from WebGrid user interface you need to give
                a confirmation foreach record being deleted.
            </td>
            </tr>
            </table>";
            //						<br/>
            //						<span class=""wgrow""><small>Copyright &copy; 2004 <a href=""http://webgrid.com"" target=""_blank"">  WebGrid</a></small></span>

            return help.Replace("[PATH]", imagePath);
        }

        #endregion Methods
    }
}