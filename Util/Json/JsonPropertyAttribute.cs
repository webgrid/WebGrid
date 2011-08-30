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

namespace WebGrid.Util.Json
{
    using System;

    ///<summary>
    /// Attributes for JSON
    ///</summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class JsonPropertyAttribute : Attribute
    {
        #region Constructors

        ///<summary>
        /// Default constructor
        ///</summary>
        ///<param name="propertyName">Name of the property</param>
        public JsonPropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        #endregion Constructors

        #region Properties

        ///<summary>
        /// Name of the property
        ///</summary>
        public string PropertyName
        {
            get; set;
        }

        #endregion Properties
    }
}