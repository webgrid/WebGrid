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
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

    internal struct MemberMapping
    {
        #region Fields

        private readonly bool _ignored;
        private readonly string _mappingName;
        private readonly MemberInfo _member;
        private readonly bool _readable;
        private readonly bool _writable;

        #endregion Fields

        #region Constructors

        public MemberMapping(string mappingName, MemberInfo member, bool ignored, bool readable, bool writable)
        {
            _mappingName = mappingName;
              _member = member;
              _ignored = ignored;
              _readable = readable;
              _writable = writable;
        }

        #endregion Constructors

        #region Properties

        public bool Ignored
        {
            get { return _ignored; }
        }

        public string MappingName
        {
            get { return _mappingName; }
        }

        public MemberInfo Member
        {
            get { return _member; }
        }

        public bool Readable
        {
            get { return _readable; }
        }

        public bool Writable
        {
            get { return _writable; }
        }

        #endregion Properties
    }
}