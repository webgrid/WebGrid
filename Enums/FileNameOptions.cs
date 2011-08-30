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
namespace WebGrid.Enums
{
    #region Enumerations

    /// <summary>
    /// When uploading files and images this can set to rename the file to use the
    /// primary key as the filename.
    /// </summary>
    public enum FileNameOption
    {
        /// <summary>
        /// Undefined.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Uses the column name and primary key(s) of the row as filename.
        /// You can store the filename by using the 'filenamecolumn' property.
        /// </summary>
        UsePrimaryKey = 1,

        /// <summary>
        /// Does not rename the file or image.
        /// </summary>
        Original = 2
    }

    #endregion Enumerations
}