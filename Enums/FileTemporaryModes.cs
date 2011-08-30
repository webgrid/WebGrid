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
    /// When uploading files and images this defines where temporary files should be stored.
    /// </summary>
    public enum FileTemporaryMode
    {
        /// <summary>
        /// Undefined.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Memory (session) is used to store temporary files.
        /// </summary>
        Memory = 1,

        /// <summary>
        /// Files are saved to disk and are deleted after use.
        /// Note that some files might not be deleted, for example when the user leaves the page.
        /// </summary>
        File = 2
    }

    #endregion Enumerations
}