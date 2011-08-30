
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
    /// Data Source controls supported by WebGrid
    /// </summary>
    public enum DataSourceControlType
    {
        /// <summary>
        /// SqlDataSource
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// SqlDataSource
        /// </summary>
        SqlDataSource = 1,

        /// <summary>
        /// AccessDataSource
        /// </summary>
        AccessDataSource = 2,

        /// <summary>
        /// XmlDataSource
        /// </summary>
        XmlDataSource = 3,

        /// <summary>
        /// Object DataSource.
        /// </summary>
        ObjectDataSource = 4,
        /// <summary>
        /// Internal data source is from a connection data source and can be sql statement, table, view, or
        /// stored procedure.
        /// </summary>
        InternalDataSource = 5,
        /// <summary>
        /// IList or IEnumerable data source.
        /// </summary>
        EnumerableDataSource = 6,
        /// <summary>
        /// LinqDataSource
        /// </summary>
        LinqDataSource,
        /// <summary>
        /// EntityDataSource
        /// </summary>
        EntityDataSource
    }

    #endregion Enumerations
}