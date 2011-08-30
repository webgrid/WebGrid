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
    /// Tri stateBool can hold 3 states of information.
    /// True, False and Undefined.
    /// </summary>
    public enum TristateBool2
    {
        /// <summary>
        /// Undefined.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// True.
        /// </summary>
        True = 1,

        /// <summary>
        /// False.
        /// </summary>
        False = 2
    }

    #endregion Enumerations

    /// <summary>
    /// Class for parsing to and to and from Tri state bool to boolean.
    /// </summary>
    public sealed class TristateBoolType
    {
        #region Methods

        /// <summary>
        /// Converts a Tri state bool to boolean.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">If undefined. Default is true.</param>
        /// <returns>The converted result.</returns>
        public static bool ToBool(bool? value, bool defaultValue)
        {
            return value == null ? defaultValue : (bool) value;
        }

        #endregion Methods
    }
}