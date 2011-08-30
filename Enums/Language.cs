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
    /// The languages that are supported by WebGrid. You can also write your own
    /// language using WebGrid.SystemMessages or using XML.
    /// 
    /// Additional languages may be added to WebGrid. Please contact us if you have
    /// a translation that you wish to be included in future version. support@webgrid.com
    /// </summary>
    public enum SystemLanguage
    {
        /// <summary>
        /// Undefined.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// The system messages for WebGrid is rendered in english. (Default.)
        /// </summary>
        English = 1,

        /// <summary>
        /// The system messages for WebGrid is rendered in norwegian.
        /// </summary>
        /// <remarks>
        /// For norwegian table and column titles are updated to use special
        /// characters like æ, ø, and å in replacement for AE,OE and AA.
        /// </remarks>
        Norwegian = 2,

        /// <summary>
        /// The system messages for WebGrid is rendered in danish.
        /// </summary>
        /// <remarks>
        /// For danish table and column titles are updated to use special
        /// characters likeæ, ø, and å in replacement for AE,OE and AA.
        /// </remarks>
        Danish = 3,

        /// <summary>
        /// The system messages for WebGrid is rendered in swedish.
        /// </summary>
        /// <remarks>
        /// For swedish table and column titles are updated to use special
        /// characters like æ, ø, and å in replacement for AE,OE and AA.
        /// </remarks>
        Swedish = 4,
        /// <summary>
        /// The system messages for WebGrid is rendered in spanish.
        /// </summary>
        Spanish = 5,
        /// <summary>
        /// The system messages for WebGrid is rendered in german.
        /// </summary>
        German = 6,
        /// <summary>
        /// The system messages for WebGrid is rendered in dutch.
        /// </summary>
        Dutch = 7,
        /// <summary>
        /// The system messages for WebGrid is rendered in turkish.
        /// </summary>
        /// <remark>
        /// Thanks for translate to Erdal Kilinc, erkilinc@googlemail.com
        /// </remark>
        Turkish = 8,
        /// <summary>
        /// The system messages for WebGrid is rendered in arabic.
        /// </summary>
        /// <remark>
        /// Thanks for translate to aboganas at http://forums.webgrid.com
        /// </remark>
        Arabic = 8,
    }

    #endregion Enumerations
}