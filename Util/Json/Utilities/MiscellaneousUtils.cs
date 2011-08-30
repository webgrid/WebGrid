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

namespace WebGrid.Util.Json.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Text;

    #region Delegates

    internal delegate void Action();

    internal delegate T Creator<T>();

    internal delegate T Func<A0, T>(A0 arg0);

    internal delegate T Func<A0, A1, T>(A0 arg0, A1 arg1);

    internal delegate T Func<A0, A1, A2, T>(A0 arg0, A1 arg1, A2 arg2);

    #endregion Delegates

    internal static class MiscellaneousUtils
    {
        #region Methods

        public static string GetDescription(object o)
        {
            ValidationUtils.ArgumentNotNull(o, "o");

              ICustomAttributeProvider attributeProvider = o as ICustomAttributeProvider;

              // object passed in isn't an attribute provider
              // if value is an enum value, get value field member, otherwise get values type
              if (attributeProvider == null)
              {
            Type valueType = o.GetType();

            if (valueType.IsEnum)
              attributeProvider = valueType.GetField(o.ToString());
            else
              attributeProvider = valueType;
              }

              DescriptionAttribute descriptionAttribute = ReflectionUtils.GetAttribute<DescriptionAttribute>(attributeProvider);

              if (descriptionAttribute != null)
            return descriptionAttribute.Description;
              else
            throw new Exception(string.Format("No DescriptionAttribute on '{0}'.", o.GetType()));
        }

        public static IList<string> GetDescriptions(IList values)
        {
            ValidationUtils.ArgumentNotNull(values, "values");

              string[] descriptions = new string[values.Count];

              for (int i = 0; i < values.Count; i++)
              {
            descriptions[i] = GetDescription(values[i]);
              }

              return descriptions;
        }

        public static string ToString(object value)
        {
            return (value != null) ? value.ToString() : "{null}";
        }

        public static bool TryAction<T>(Creator<T> creator, out T output)
        {
            ValidationUtils.ArgumentNotNull(creator, "creator");

              try
              {
            output = creator();
            return true;
              }
              catch
              {
            output = default(T);
            return false;
              }
        }

        public static bool TryGetDescription(object value, out string description)
        {
            return TryAction<string>(delegate { return GetDescription(value); }, out description);
        }

        #endregion Methods
    }
}