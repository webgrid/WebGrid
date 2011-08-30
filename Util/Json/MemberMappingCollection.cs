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
    using System.Collections.ObjectModel;
    using System.Text;

    internal class MemberMappingCollection : KeyedCollection<string, MemberMapping>
    {
        #region Methods

        public void AddMapping(MemberMapping memberMapping)
        {
            if (Contains(memberMapping.MappingName))
              {
            // don't overwrite existing mapping with ignored mapping
            if (memberMapping.Ignored)
              return;

            MemberMapping existingMemberMapping = this[memberMapping.MappingName];

            if (!existingMemberMapping.Ignored)
            {
              throw new JsonSerializationException(
            string.Format(
              "A member with the name '{0}' already exists on {1}. Use the JsonPropertyAttribute to specify another name.",
              memberMapping.MappingName, memberMapping.Member.DeclaringType));
            }
            else
            {
              // remove ignored mapping so it can be replaced in collection
              Remove(existingMemberMapping);
            }
              }

              Add(memberMapping);
        }

        protected override string GetKeyForItem(MemberMapping item)
        {
            return item.MappingName;
        }

        #endregion Methods
    }
}