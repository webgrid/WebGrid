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

namespace WebGrid
{
    using System;

    internal class DataBaseColumnInformation
    {
        #region Fields

        internal bool canNull;
        internal string columnName;
        internal Type dataType;

        //    internal string datasource;
        internal string defaultDataSourceId;
        internal int displayindex;

        //internal object defaultValue;
        internal bool identity;
        internal bool isInDB = true;
        internal int? maxSize;
        internal bool primaryKey;
        internal bool readOnly;
        internal string title;
        internal bool uniquevalue;

        #endregion Fields
    }
}