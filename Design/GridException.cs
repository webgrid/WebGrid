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

namespace WebGrid.Design
{
    using System;

    /// <summary>
    /// Grid data source exception handler.
    /// </summary>
    [Serializable]
    public class GridDataSourceException : Exception
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataSourceException"/> class.
        /// </summary>
        /// <param name="systemmessage">Exception system message</param>
        /// <param name="ex">Inner exception for this exception</param>
        public GridDataSourceException(string systemmessage, Exception ex)
            : base(systemmessage, ex)
        {
        }

        #endregion Constructors
    }

    /// <summary>
    /// Grid general exception handler.
    /// </summary>
    [Serializable]
    public class GridException : Exception
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GridException"/> class.
        /// </summary>
        /// <param name="systemmessage">Exception system message</param>
        public GridException(string systemmessage)
            : base(systemmessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridException"/> class.
        /// </summary>
        /// <param name="systemmessage">Exception system message</param>
        /// <param name="ex">Inner exception for this exception</param>
        public GridException(string systemmessage, Exception ex)
            : base(systemmessage, ex)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridException"/> class.
        /// </summary>
        /// <param name="sql">SQL for this exception (this creates an <see cref="GridDataSourceException"/> exception) </param>
        /// <param name="systemmessage">Exception system message</param>
        /// <param name="ex">Inner exception for this exception</param>
        public GridException(string sql, string systemmessage, Exception ex)
            : base(systemmessage, ex)
        {
            if (!string.IsNullOrEmpty(sql))
                throw new GridDataSourceException(sql, ex);
        }

        #endregion Constructors
    }
}