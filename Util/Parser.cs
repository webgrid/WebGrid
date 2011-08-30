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
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using Convert = System.Convert;
    using System.Data;
    using System.Drawing;
    using System.Globalization;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;

    ///<summary>
    ///</summary>
    ///<typeparam name="T"></typeparam>
    internal interface IDataTableConverter<T>
    {
        #region Methods

        DataTable GetDataTable(List<T> items);

        #endregion Methods
    }

    /// <summary>
    /// WebGrid.Util.Parser is a standalone class that has a static method that allow the programmer to 
    /// send a string represent a mathematical or logical expression and get the answer.
    /// This feature is used by <see cref="WebGrid.Decimal"/> column.
    /// </summary>
    public static class Parser
    {
        #region Methods

        /// <summary>
        /// Extract only the hex digits from a string.
        /// </summary>
        public static string ExtractHexDigits(string input)
        {
            // remove any characters that are not digits (like #)
            Regex isHexDigit
                = new Regex("[abcdefABCDEF\\d]+", RegexOptions.Compiled);
            string newnum = string.Empty;
            foreach (char c in input)
            {
                if (isHexDigit.IsMatch(c.ToString()))
                    newnum += c.ToString();
            }
            return newnum;
        }

        /// <summary>
        /// Convert a hex string to a .NET Color object.
        /// </summary>
        /// <param name="hexColor">a hex string: "FFFFFF", "#000000"</param>
        public static Color HexStringToColor(string hexColor)
        {
            string hc = ExtractHexDigits(hexColor);
            if (hc.Length != 6)
            {
                // you can choose whether to throw an exception
                //throw new ArgumentException("hexColor is not exactly 6 digits.");
                return Color.Empty;
            }
            string r = hc.Substring(0, 2);
            string g = hc.Substring(2, 2);
            string b = hc.Substring(4, 2);
            Color color;
            try
            {
                int ri
                    = Int32.Parse(r, NumberStyles.HexNumber);
                int gi
                    = Int32.Parse(g, NumberStyles.HexNumber);
                int bi
                    = Int32.Parse(b, NumberStyles.HexNumber);
                color = Color.FromArgb(ri, gi, bi);
            }
            catch
            {
                // you can choose whether to throw an exception
                //throw new ArgumentException("Conversion failed.");
                return Color.Empty;
            }
            return color;
        }

        internal static DataTable ObtainDataTableFromIEnumerable(IEnumerable argument)
        {
            if (argument == null)
                return null;
            if (argument is IDataReader)
            {
                DataTable datatable = new DataTable();
                datatable.Load((IDataReader) argument);
                return datatable;
            }

            DataTable dt = new DataTable();
            foreach (object obj in argument)
            {
                if (obj == null)
                    continue;
                Type t = obj.GetType();
                PropertyInfo[] pis = t.GetProperties();

                if (dt.Columns.Count == 0)
                {
                    foreach (PropertyInfo pi in pis)
                    {
                        if (dt.Columns.Contains(pi.Name))
                            continue;
                        if (pi.PropertyType.IsGenericType &&
                            pi.PropertyType.GetGenericTypeDefinition() == typeof (Nullable<>))
                            dt.Columns.Add(pi.Name, pi.PropertyType.GetGenericArguments()[0]);
                        else if (pi.PropertyType == typeof (char))
                            dt.Columns.Add(pi.Name, typeof (string));
                        else
                            dt.Columns.Add(pi.Name, pi.PropertyType);
                    }
                }

                DataRow dr = dt.NewRow();
                foreach (PropertyInfo pi in pis)
                {
                    if (!pi.CanRead)
                        continue;
                    if (pi.PropertyType == typeof (char)) dr[pi.Name] = obj;
                    else
                    {
                        object data = pi.GetValue(obj, null);
                        dr[pi.Name] = data ?? DBNull.Value;
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        internal static void SetIOrderedDictionaryFromDataRow(DataRow row, IOrderedDictionary argument)
        {
            foreach (DictionaryEntry var in argument)
            {
                object obj = var.Value;
                Type t = obj.GetType();
                PropertyInfo[] pis = t.GetProperties();

                foreach (PropertyInfo pi in pis)
                {
                    if (!pi.CanWrite)
                        continue;
                   /* if (pi.PropertyType.IsGenericType &&
                        pi.PropertyType.GetGenericTypeDefinition() == typeof (Nullable<>))*/
                        if (row[pi.Name] == DBNull.Value)
                            pi.SetValue(obj, null, null);
                        else
                            pi.SetValue(obj, row[pi.Name], null);
            /*                else if (row[pi.Name] != null)
                        pi.SetValue(obj, row[pi.Name], null);*/
                }
            }
        }

        internal static DataTable ToDataTable(Collection<object> collection)
        {
            DataTable table = new DataTable();

            if (collection.Count > 0)
            {
                PropertyInfo[] properties = collection[0].GetType().GetProperties();

                List<string> columns = new List<string>();

                foreach (PropertyInfo pi in properties)
                {
                    table.Columns.Add(pi.Name);

                    columns.Add(pi.Name);
                }

                foreach (object item in collection)
                {
                    object[] cells = getValues(columns, item);

                    table.Rows.Add(cells);
                }
            }

            return table;
        }

        private static object[] getValues(IList<string> columns, object instance)
        {
            object[] ret = new object[columns.Count];

            for (int n = 0; n < ret.Length; n++)
            {
                PropertyInfo pi = instance.GetType().GetProperty(columns[n]);
                if (!pi.CanRead)
                    continue;
                object value = pi.GetValue(instance, null);
                ret[n] = value;
            }

            return ret;
        }

        #endregion Methods
    }

    [AttributeUsage(AttributeTargets.Property)]
    internal abstract class ConversionAttribute : Attribute
    {
        #region Properties

        internal bool AllowDBNull
        {
            get; set;
        }

        internal bool DataTableConversion
        {
            get; set;
        }

        internal bool KeyField
        {
            get; set;
        }

        #endregion Properties
    }

    internal class DataTableConverter<T> : IDataTableConverter<T>
    {
        #region Fields

        private readonly bool m_enforceKeys;

        #endregion Fields

        #region Constructors

        public DataTableConverter()
        {
        }

        public DataTableConverter(bool enforceKeys)
        {
            m_enforceKeys = enforceKeys;
        }

        #endregion Constructors

        #region Methods

        public DataTable GetDataTable(List<T> items)
        {
            DataTable dt;
            try
            {
                // Build a table schema from the first element in the collection

                dt = ConstructDataTableSchema(items[0]);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw (new ApplicationException(
                    "Cannot convert List of zero length to a DataTable", ex));
            }
            // If the container is not convertable than throw an

            // ApplicationException.

            if (dt == null)
                throw new ApplicationException("List items are not convertable.");
            // Create a new row for every item in the collection and fill it.
            for (int i = 0; i < items.Count; i++)
            {
                DataRow dr = dt.NewRow();
                Type type = items[i].GetType();
                MemberInfo[] members = type.GetProperties();

                foreach (MemberInfo member in members)
                {
                    object[] attributes = member.GetCustomAttributes(true);
                    if (attributes.Length == 0) continue;
                    foreach (object attribute in attributes)
                    {
                        ConversionAttribute ca = attribute as
                                                 ConversionAttribute;
                        if (ca == null) continue;
                        if (!ca.DataTableConversion) continue;
                        string[] nameArray
                            = member.Name.Split(
                                Convert.ToChar(" "));
                        PropertyInfo prop = type.GetProperty(
                            nameArray[0]);
            //                       Type valueType = prop.GetValue(items[i],
            //                                                      null).GetType() // COMMENTED BY CODEIT.RIGHT;
                        dr[nameArray[0]] = prop.GetValue(items[i], null);
                    }
                }

                dt.Rows.Add(dr);
            }
            return dt;
        }

        // This method reads the attributes of your container class via
        // reflection  in order to build a schema for the DataTable that you
        // will explicitly convert to.
        private DataTable ConstructDataTableSchema(T item)
        {
            string tableName = string.Empty;
            List<DTConverterContainer> schemaContainers = new List<DTConverterContainer>();
            Type type = item.GetType();
            MemberInfo[] members = type.GetProperties();
            foreach (MemberInfo member in members)
            {
                object[] attributes = member.GetCustomAttributes(true);
                if (attributes.Length == 0) continue;
                foreach (object attribute in attributes)
                {
                    ConversionAttribute ca = attribute as ConversionAttribute;
                    if (ca == null) continue;
                    if (!ca.DataTableConversion) continue;
                    // The name of the container class is used to name
                    // your DataTable

                    string[] classNameArray =
                        member.ReflectedType.ToString().Split(
                            Convert.ToChar("."));
                    tableName = classNameArray[classNameArray.Length - 1];
                    string name = member.Name;
                    PropertyInfo prop = type.GetProperty(name);
                    Type valueType = prop.GetValue(item, null).GetType();
                    // Each property that is  will be a column in our
                    // DataTable.

                    schemaContainers.Add(new DTConverterContainer(name,
                                                                  valueType, ca.AllowDBNull, ca.KeyField));
                }
            }
            if (schemaContainers.Count > 0)
            {
                DataTable dataTable = new DataTable(tableName);
                DataColumn[] dataColumn = new DataColumn[schemaContainers.Count];
                // Counts the number of keys that will need to be created

                int totalNumberofKeys = 0;
                foreach (DTConverterContainer container in schemaContainers)
                {
                    if (container.IsKey && m_enforceKeys)
                    {
                        totalNumberofKeys = totalNumberofKeys + 1;
                    }
                }
                // Builds the DataColumns for our DataTable

                DataColumn[] keyColumnArray = new DataColumn[totalNumberofKeys];
                int keyColumnIndex = 0;
                for (int i = 0; i < schemaContainers.Count; i++)
                {
                    dataColumn[i] = new DataColumn
                                        {
                                            DataType = schemaContainers[i].PropertyType,
                                            ColumnName = schemaContainers[i].PropertyName,
                                            AllowDBNull = schemaContainers[i].AllowDBNull
                                        };
                    dataTable.Columns.Add(dataColumn[i]);
                    if (schemaContainers[i].IsKey != true || m_enforceKeys != true) continue;
                    keyColumnArray[keyColumnIndex] = dataColumn[i];
                    keyColumnIndex = keyColumnIndex + 1;
                }
                if (m_enforceKeys)
                {
                    dataTable.PrimaryKey = keyColumnArray;
                }
                return dataTable;
            }
            return null;
        }

        #endregion Methods

        #region Nested Types

        private class DTConverterContainer
        {
            #region Constructors

            internal DTConverterContainer(string propertyName, Type propertyType,
                bool allowDBNull, bool isKey)
            {
                PropertyName = propertyName;
                PropertyType = propertyType;
                AllowDBNull = allowDBNull;
                IsKey = isKey;
            }

            #endregion Constructors

            #region Properties

            public bool AllowDBNull
            {
                get; private set;
            }

            public bool IsKey
            {
                get; private set;
            }

            public string PropertyName
            {
                get; private set;
            }

            public Type PropertyType
            {
                get; private set;
            }

            #endregion Properties
        }

        #endregion Nested Types
    }

    internal class Parserstring
    {
        #region Fields

        public readonly int Length;

        /*
             * this class contain an array of characters treated as a string
             * and some methods that do some functions over it like removing
             * and replacing and inserting
             * the constructor puts an string into the array
             *
             */
        public readonly char[] Wordarray;

        #endregion Fields

        #region Constructors

        public Parserstring(string word)
        {
            Length = word.Length;
            Wordarray = new char[Length];
            Wordarray = word.ToCharArray(0, Length);
        }

        #endregion Constructors

        #region Methods

        public string Copy( /*from starting with zero*/ int start, /*to*/int end)
        {
            /*
                 * this static method copy a piece of the parserstring array to an string
                 */
            StringBuilder copyedBuilder = new StringBuilder();
            for (int loop = start; loop < end; loop++)
            {
                copyedBuilder.Append(Wordarray[loop]);
            }
            return copyedBuilder.ToString();
        }

        public Parserstring Insert(string whattoinsert, int at)
        {
            /*
                 * this method insert a string at an specific index(before it)
                 */
            string resultstring = Copy(0, at);
            resultstring += whattoinsert + Copy(at, Length);
            return new Parserstring(resultstring);
        }

        public string ParseToString()
        {
            StringBuilder backBuilder = new StringBuilder();
            foreach (char i in Wordarray)
            {
                backBuilder.Append(i.ToString());
            }
            return backBuilder.ToString();
        }

        public Parserstring Replace(string whattoinsert, int from, int to)
        {
            /*
                 * this method replace a No. of characters in the array with a string
                 * from an specified index to another
                 *
                 */
            StringBuilder resultstringBuilder = new StringBuilder(Copy(0, from));
            resultstringBuilder.Append(whattoinsert + Copy(to, Length));
            return new Parserstring(resultstringBuilder.ToString());
        }

        #endregion Methods
    }
}