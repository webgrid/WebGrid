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
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Various extended string function provided by 
    /// </summary>
    public static class StringFunctions
    {
        #region Methods

        /// <summary>
        /// Return a rating for how strong the string is as a password. Max rating is 100
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int PasswordStrength(string input)
        {
            double total = 0;
            bool hasUpperCase = false;
            bool hasLowerCase = false;

            total = input.Length * 3;

            char currentLetter;
            for (int i = 0; i < input.Length; i++)
            {
                currentLetter = input[i];
                if (Convert.ToInt32(currentLetter) >= 65 && Convert.ToInt32(currentLetter) <= 92)
                    hasUpperCase = true;

                if (Convert.ToInt32(currentLetter) >= 97 && Convert.ToInt32(currentLetter) <= 122)
                    hasLowerCase = true;
            }

            if (hasUpperCase && hasLowerCase) total *= 1.2;

            for (int i = 0; i < input.Length; i++)
            {
                currentLetter = input[i];
                if (Convert.ToInt32(currentLetter) >= 48 && Convert.ToInt32(currentLetter) <= 57) //Numbers
                    if (hasUpperCase && hasLowerCase) total *= 1.4;
            }

            for (int i = 0; i < input.Length; i++)
            {
                currentLetter = input[i];
                if ((Convert.ToInt32(currentLetter) <= 47 && Convert.ToInt32(currentLetter) >= 123) ||
                    (Convert.ToInt32(currentLetter) >= 58 && Convert.ToInt32(currentLetter) <= 64)) //symbols
                {
                    total *= 1.5;
                    break;
                }
            }

            if (total > 100.0) total = 100.0;

            return (int)total;
        }

        /// <summary>
        /// Split a string, dealing correctly with quoted items
        /// </summary>
        /// <param name="text">the string to be split</param>
        /// <param name="seperators">separator char. Default is comma</param>
        /// <param name="quotes">character used to quote strings. Default is "", quote starts and ends with "</param>
        /// <returns>string array</returns>
        public static string[] SplitQuoted(string text, string seperators, string quotes)
        {
            // Default seperators is a tab (e.g. "\t").
            // All seperators not inside quote pair are ignored.
            // Default quotes pair is two double quotes ( e.g. '""' ).
            if (text == null)
                throw new ArgumentNullException("text", "text is null.");
            if (string.IsNullOrEmpty(seperators))
                seperators = "\t";
            if (string.IsNullOrEmpty(quotes))
                quotes = "\"\"";
            ArrayList res = new ArrayList();

            // Get the open and close chars, escape them for use in regular expressions.
            string openChar = Regex.Escape(quotes[0].ToString());
            string closeChar = Regex.Escape(quotes[quotes.Length - 1].ToString());
            // Build the pattern that searches for both quoted and unquoted elements
            // notice that the quoted element is defined by group #2
            // and the unquoted element is defined by group #3.
            string pattern = string.Format(@"\s*({0}([^{1}]*){1})|([^{2}]+)\s*", openChar, closeChar, seperators);

            // Search the string.
            foreach (Match m in Regex.Matches(text, pattern))
            {
                string g3 = m.Groups[3].Value;
                if (!string.IsNullOrEmpty(g3))
                    res.Add(g3);
                else
                {
                    // get the quoted string, but without the quotes.
                    res.Add(m.Groups[2].Value);
                }
            }
            return (string[])res.ToArray(typeof(string));
        }

        #endregion Methods
    }
}