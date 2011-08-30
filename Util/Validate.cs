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
    using System.Text.RegularExpressions;

    /// <summary>
    /// The validate class contains a set of predefined server-validation-methods to validate data or 
    /// methods where the programmer can self write regular expression.
    /// </summary>
    public sealed class Validate
    {
        #region Methods

        /// <summary>
        /// Validates only alphanumeric input with spaces.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <returns>
        /// 	<c>true</c> if [is alfa numeric] [the specified argument]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAlfaNumeric(string argument)
        {
            return !string.IsNullOrEmpty(argument) && Regex.IsMatch(argument, @"^[a-zA-Z0-9\s]+$");
        }

        /// <summary>
        /// Validates only valid domain names
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <returns>
        /// 	<c>true</c> if the specified argument is domain; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDomain(string argument)
        {
            return !string.IsNullOrEmpty(argument) && Regex.IsMatch(argument,
                                                                    @"^([a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?\.)*[a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?$");
        }

        /// <summary>
        /// Validates only correct Email Addresses
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <returns>
        /// 	<c>true</c> if the specified argument is email; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEmail(string argument)
        {
            return !string.IsNullOrEmpty(argument) && Regex.IsMatch(argument,
                                                                    @"^([\w\d\-\.]+)@{1}(([\w\d\-]{1,67})|([\w\d\-]+\.[\w\d\-]{1,67}))\.(([a-zA-Z\d]{2,4})(\.[a-zA-Z\d]{2})?)$");
        }

        /// <summary>
        /// Gets if a regular expression is true or false.
        /// </summary>
        /// <param name="expression">The regular expression</param>
        /// <param name="argument">The string to search for a match</param>
        /// <returns>
        /// 	<c>true</c> if the specified expression is expression; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsExpression(string expression, string argument)
        {
            return argument == null || expression == null || Regex.IsMatch(argument,expression);
        }

        /// <summary>
        /// File ColumnId Validator. Validates both UNC (\\server\share\file) and regular MS path (c:\file)
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <returns>
        /// 	<c>true</c> if [is file name] [the specified argument]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsFileName(string argument)
        {
            return !string.IsNullOrEmpty(argument) && Regex.IsMatch(argument,
                                                                             @"^(([a-zA-Z]:|\\)\\)?(((\.)|(\.\.)|([^\\/:\*\?""\|<>\. ](([^\\/:\*\?""\|<>\. ])|([^\\/:\*\?""\|<>]*[^\\/:\*\?""\|<>\. ]))?))\\)*[^\\/:\*\?""\|<>\. ](([^\\/:\*\?""\|<>\. ])|([^\\/:\*\?""\|<>]*[^\\/:\*\?""\|<>\. ]))?$");
        }

        /// <summary>
        /// Validates float/decimal values only.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <returns>
        /// 	<c>true</c> if the specified argument is float; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsFloat(string argument)
        {
            return !string.IsNullOrEmpty(argument) && Regex.IsMatch(argument,
                                                                    "^[\\-]{0,1}[0-9]{1,}(([\\.\\,]{0,1}[0-9]{1,})|([0-9]{0,}))$");
        }

        /// <summary>
        /// Validate an hour entry to be between 00:00 and 23:59
        /// Matches:  [00:00], [13:59], [23:59]
        /// Non-Matches:  [24:00], [23:60]
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <returns>
        /// 	<c>true</c> if the specified argument is hour; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsHour(string argument)
        {
            return !string.IsNullOrEmpty(argument) &&
                   Regex.IsMatch(argument, @"([0-1][0-9]|2[0-3]):[0-5][0-9]");
        }

        /// <summary>
        /// Validates only valid IP addresses.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <returns>
        /// 	<c>true</c> if [is ip address] [the specified argument]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIPAddress(string argument)
        {
            return !string.IsNullOrEmpty(argument) && Regex.IsMatch(argument,
                                                                    @"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$");
        }

        /// <summary>
        /// Validates MAC address. With colons separating octets. It will ignore strings too short or long, or with invalid characters. It will accept mixed case hexadecimal. Use extended grep.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <returns>
        /// 	<c>true</c> if [is mac address] [the specified argument]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMacAddress(string argument)
        {
            return !string.IsNullOrEmpty(argument) &&
                   Regex.IsMatch(argument, @"^([0-9a-fA-F][0-9a-fA-F]:){5}([0-9a-fA-F][0-9a-fA-F])$");
        }

        /// <summary>
        /// Validates numeric input of 99,999,999 to 0 with or without commas. but no decimal places.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <returns>
        /// 	<c>true</c> if the specified argument is numeric; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNumeric(string argument)
        {
            return !string.IsNullOrEmpty(argument) && Regex.IsMatch(argument, @"^\d{1,8}$|^\d{1,3},\d{3}$|^\d{1,2},\d{3},\d{3}$");
        }

        /// <summary>
        /// Validates OleDB connection string.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <returns>
        /// 	<c>true</c> if [is OLE DB connection string] [the specified argument]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsOleDBConnectionString(string argument)
        {
            return !string.IsNullOrEmpty(argument) && Regex.IsMatch(argument,
                                                                    @"(?:Provider=""??(?<Provider>[^;\n]+)""??[;\n""]??|Data\sSource=(?<DataSource>[^;\n]+)[;\n""]??|Initial\sCatalog=(?<InitialCatalog>[^;\n]+)[;\n""]??|User\sID=(?<UserID>[^;\n]+)[;\n""]??|Password=""??(?<Password>[^;\n]+)""??[;\n""]??|Integrated\sSecurity=(?<IntegratedSecurity>[^;\n]+)[;\n]??|Connection\sTimeOut=(?<ConnectionTimeOut>[^;\n]+)[;\n""]??)+$");
        }

        /// <summary>
        /// Finds any HTML tag and sub-matches properties weather it has an apposterphee, quote, or no quote/apposterphee
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <returns>
        /// 	<c>true</c> if the specified argument is tag; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsTag(string argument)
        {
            return !string.IsNullOrEmpty(argument) &&
                   Regex.IsMatch(argument, @"</?(\w+)(\s+\w+=(\w+|""[^""]*""|'[^']*'))*>");
        }

        /// <summary>
        /// Validates http input (validates both www and http:// )
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <returns>
        /// 	<c>true</c> if the specified argument is URL; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsUrl(string argument)
        {
            return !string.IsNullOrEmpty(argument) && Regex.IsMatch(argument, @"(?<http>(http:[/][/]|www.)([a-z]|[A-Z]|[0-9]|[/.]|[~])*)");
        }

        #endregion Methods
    }
}