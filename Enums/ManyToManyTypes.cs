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
    /// Different ways to represent the editing of many-to-many relations.
    /// </summary>
    public enum ManyToManyType
    {
        /// <summary>
        /// Uses a dropdown interface where the user can choose from multiple choices.
        /// </summary>
        Multiselect = 0,

        /// <summary>
        /// Displays a list of check boxes.
        /// </summary>
        Checkboxes = 1,
    }

    #endregion Enumerations
}