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

    /// <summary>
    /// The exception that is thrown when the HtmlEditor base page is invalid. 
    /// </summary>
    [Serializable]
    public class BasePathException : Exception
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the BasePathException class.
        /// </summary>
        /// <param name="basePath"></param>
        public BasePathException(string basePath)
            : base(string.Format("Directory {0} not found.", basePath))
        {
        }

        #endregion Constructors
    }
}