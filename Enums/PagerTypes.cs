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
    /// The different types of navigation types available. This feature applies to grid view.
    /// The pager will by default be visibility if there are enough records from the datasorce to span more than one page.
    /// </summary>
    public enum PagerType
    {
        /// <summary>
        /// Undefined. Standard pager is used if this pager type is set.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// This navigation type renders a horsiontal pager list, with no navigation bars or status.
        /// </summary>
        Standard = 1,

        /// <summary>
        /// Use a dropdown list to navigate through the records.
        /// </summary>
        DropDown = 2,

        /// <summary>
        ///  slider
        /// </summary>
        Slider = 3,

        /// <summary>
        /// Ranged slider
        /// </summary>
        RangedSlider = 4,
        /// <summary>
        /// Does not display a pager, and displays all data.
        /// </summary>
        None = 5
    }

    #endregion Enumerations
}