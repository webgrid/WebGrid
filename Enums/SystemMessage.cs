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
    /// Defines which mode WebGrid is currently using. Either Grid or Edit.
    ///</summary>
    public enum SystemMessageAlias
    {
        /// <summary>
        /// Default (english) system message for this alias is 'Total numbers of records found.'
        ///</summary>
        AllTitle = 1,
        /// <summary>
        /// Default (english) system message for this alias is 'Update'
        ///</summary>
        Update = 2,

        /// <summary>
        /// Default (english) system message for this alias is 'Updated'
        ///</summary>
        Updated = 3,
        /// <summary>
        /// Default (english) system message for this alias is 'CloseWindow'
        ///</summary>
        CloseWindow = 4,
        /// <summary>
        /// Default (english) system message for this alias is 'Insert new record'
        ///</summary>
        Insert = 5,
        /// <summary>
        /// Default (english) system message for this alias is 'Are you sure you want to delete this?'
        ///</summary>
        ConfirmDelete = 6,
        /// <summary>
        /// Default (english) system message for this alias is 'Are you sure you want to copy this?'
        ///</summary>
        ConfirmCopy = 7,
        /// <summary>
        /// Default (english) system message for this alias is 'New Record'
        ///</summary>
        NewRecord = 8,
        /// <summary>
        /// Default (english) system message for this alias is 'Displaying rows {0} through {1} of total {2}'
        ///</summary>
        ShowRows = 9,
        /// <summary>
        /// Default (english) system message for this alias is 'Search'
        ///</summary>
        Search = 10,
        /// <summary>
        /// Default (english) system message for this alias is 'Edit row'
        ///</summary>
        EditRow = 11,
        /// <summary>
        /// Default (english) system message for this alias is 'Delete row'
        ///</summary>
        DeleteRow = 12,
        /// <summary>
        /// Default (english) system message for this alias is 'Copy row'
        ///</summary>
        CopyRow = 13,
        /// <summary>
        /// Default (english) system message for this alias is 'Field is required.'
        ///</summary>
        Required = 14,
        /// <summary>
        /// Default (english) system message for this alias is 'First'
        ///</summary>
        First = 15,
        /// <summary>
        /// Default (english) system message for this alias is 'Last page'
        ///</summary>
        Last = 16,
        /// <summary>
        /// Default (english) system message for this alias is 'Next page'
        ///</summary>
        Next = 17,
        /// <summary>
        /// Default (english) system message for this alias is 'Previous page'
        ///</summary>
        Prev = 18,
        /// <summary>
        /// Default (english) system message for this alias is 'Click the image to open a date picker.'
        ///</summary>
        DatePicker = 19,
        /// <summary>
        /// Default (english) system message for this alias is 'Sorted increasing/decreasing.'
        ///</summary>
        SortIcon = 20,
        /// <summary>
        /// Default (english) system message for this alias is 'Update changes'
        ///</summary>
        UpdateRows = 21,
        /// <summary>
        /// Default (english) system message for this alias is 'Download attachment'
        ///</summary>
        Attachment = 22,
        /// <summary>
        /// Default (english) system message for this alias is 'Remove file from database.'
        ///</summary>
        RemoveImage = 23,
        /// <summary>
        /// Default (english) system message for this alias is 'Total/TotalSummary'
        ///</summary>
        TotalSummary = 24,
        /// <summary>
        /// Default (english) system message for this alias is 'Password:'
        ///</summary>
        Password = 25,
        /// <summary>
        /// Default (english) system message for this alias is 'generate password'
        ///</summary>
        GenPassword = 26,
        /// <summary>
        /// Default (english) system message for this alias is 'Click to activate.'
        ///</summary>
        ClickToActivate = 27,
        /// <summary>
        /// Default (english) system message for this alias is 'Click to activate menu.'
        ///</summary>
        ClickToActivateMenu = 28,
        /// <summary>
        /// Default (english) system message for this alias is 'Click to close menu.'
        ///</summary>
        ClickToCloseMenu = 29,
        /// <summary>
        /// Default (english) system message for this alias is 'System message'
        ///</summary>
        SystemMessage = 30,
        /// <summary>
        /// Default (english) system message for this alias is 'Error updating!'
        ///</summary>
        SystemMessage_UpdateInsert = 31,
        /// <summary>
        /// Default (english) system message for this alias is 'MasterGrid not found!'
        ///</summary>
        SystemMessage_NoMasterGrid = 32,
        /// <summary>
        /// Default (english) system message for this alias is 'Table name not found.'
        ///</summary>
        SystemMessage_Table = 33,
        /// <summary>
        /// Default (english) system message for this alias is 'Foreignkeys contains no records.'
        ///</summary>
        SystemMessage_NoForeignkeys = 34,
        /// <summary>
        /// Default (english) system message for this alias is 'Some fields have to be unique.'
        ///</summary>
        SystemMessage_Unique = 35,
        /// <summary>
        /// Default (english) system message for this alias is 'Error saving multiselect information!'
        ///</summary>
        SystemMessage_MultiSelect = 36,
        /// <summary>
        /// Default (english) system message for this alias is 'Record not found.'
        ///</summary>
        SystemMessage_NoRecord = 37,
        /// <summary>
        /// Default (english) system message for this alias is 'The foreign key table for column '{0}' is not specified.'
        ///</summary>
        SystemMessage_Foreignkey_tablename = 38,
        /// <summary>
        /// Default (english) system message for this alias is 'Datasource error for the ManyToMany column '{0}'.'
        ///</summary>
        SystemMessage_ManyToMany_datasource = 39,
        /// <summary>
        /// Default (english) system message for this alias is 'Failed to retrieve number of records from the data source.'
        ///</summary>
        SystemMessage_DataSource_retrieve = 40,
        /// <summary>
        /// Default (english) system message for this alias is 'There is no table, view, or sql statement (query list) specified for the grid.'
        ///</summary>
        SystemMessage_DataSource_NoTableName = 41,
        /// <summary>
        /// Default (english) system message for this alias is 'There are incorrect or missing parameters for a foreign key.'
        ///</summary>
        SystemMessage_Foreignkey_Incorrect = 42,
        /// <summary>
        /// Default (english) system message for this alias is 'The foreign key table '{0}' does not contain any primary keys.'
        ///</summary>
        SystemMessage_Foreignkey_noKey = 43,
        /// <summary>
        /// Default (english) system message for this alias is 'The table schema could not be loaded from the data source.'
        ///</summary>
        SystemMessage_DataSource_failedSchema = 44,
        /// <summary>
        /// Default (english) system message for this alias is 'Failed to retrieve data from the data source.'
        ///</summary>
        SystemMessage_DataSource_failedData = 45,
        /// <summary>
        /// Default (english) system message for this alias is 'The property name '{0}' does not exist in the column '{1}'.'
        ///</summary>
        SystemMessage_property_notfound = 46,
        /// <summary>
        /// Default (english) system message for this alias is 'A standard OLEDB, OleDbConnection(.Net) connection string property has not been set.'
        ///</summary>
        SystemMessage_DataSource_NoConnectionString = 47,
        /// <summary>
        /// Default (english) system message for this alias is 'There is an error retrieving data from the server.'
        ///</summary>
        SystemMessage_DataSource_retrievedata = 48,
        /// <summary>
        /// Default (english) system message for this alias is 'All predefined columns must have a name.'
        ///</summary>
        SystemMessage_DataSource_NoColumnName = 49,
        /// <summary>
        /// Default (english) system message for this alias is 'Error parsing the property'
        ///</summary>
        SystemMessage_DataSource_parseProperty = 50,
        /// <summary>
        /// Default (english) system message for this alias is 'Error parsing the field'
        ///</summary>
        SystemMessage_DataSource_parseField = 51,
        /// <summary>
        /// Default (english) system message for this alias is 'There was an error loading the XML document.'
        ///</summary>
        SystemMessage_Data_failedXML = 52,
        /// <summary>
        /// Default (english) system message for this alias is 'The ID (identifier) property can not start with '_' or contain blank spaces (' ').'
        ///</summary>
        SystemMessage_Editor_noValidID = 53,
        /// <summary>
        /// Default (english) system message for this alias is 'Record could not be saved to data source.'
        ///</summary>
        SystemMessage_Update_data = 54,
        /// <summary>
        /// Default (english) system message for this alias is 'ValueColumn (WebGrid.Foreignkey) for '{0}' has no valid foreign key.'
        ///</summary>
        SystemMessage_ForeignkeyNoValueColumn = 55,
        /// <summary>
        /// Default (english) system message for this alias is 'WebGrid image file '{0}' is missing.'
        ///</summary>
        SystemMessage_Editor_filemissing = 56,
        /// <summary>
        /// Default (english) system message for this alias is 'The mathematical expression for '{0}' is not valid.'
        ///</summary>
        SystemMessage_Double_mathExpression = 58,
        /// <summary>
        /// Default (english) system message for this alias is 'Please fill in.'
        ///</summary>
        SystemMessage_Grid_required = 59,
        /// <summary>
        /// Default (english) system message for this alias is 'Numeric value is not valid.'
        ///</summary>
        SystemMessage_Grid_Int = 60,
        /// <summary>
        /// Default (english) system message for this alias is 'Decimal value is not valid.'
        ///</summary>
        SystemMessage_Grid_dec = 61,
        /// <summary>
        /// Default (english) system message for this alias is 'No valid date. ({0}).'
        ///</summary>
        SystemMessage_Grid_date = 62,
        /// <summary>
        /// Default (english) system message for this alias is 'Value must not be more than {0} characters.'
        ///</summary>
        SystemMessage_Grid_minLength = 63,
        /// <summary>
        /// Default (english) system message for this alias is 'Value must be at least {0} characters.'
        ///</summary>
        SystemMessage_Grid_maxLength = 64,
        /// <summary>
        /// Default (english) system message for this alias is 'Email is not valid.'
        ///</summary>
        SystemMessage_Grid_email = 65,
        /// <summary>
        /// Default (english) system message for this alias is 'Value is not valid.'
        ///</summary>
        SystemMessage_Grid_validate = 66,
        /// <summary>
        /// Default (english) system message for this alias is 'Similar value in field '{0}' already exists.'
        ///</summary>
        SystemMessage_UniqueValueRequired = 67,
        /// <summary>
        /// Default (english) system message for this alias is 'Please fill in '{0}'.'
        ///</summary>
        SystemMessage_Required = 68,
        /// <summary>
        /// Default (english) system message for this alias is 'Numeric value in '{0}' is not valid.'
        ///</summary>
        SystemMessage_Int = 69,
        /// <summary>
        /// Default (english) system message for this alias is 'Decimal value in '{0}' is not valid.'
        ///</summary>
        SystemMessage_Dec = 70,
        /// <summary>
        /// Default (english) system message for this alias is 'Value in '{0}' is not a valid date ({1} is valid date).'
        ///</summary>
        SystemMessage_Date = 71,
        /// <summary>
        /// Default (english) system message for this alias is ''{0}' must be at least {1} characters long.'
        ///</summary>
        SystemMessage_MinLength = 72,
        /// <summary>
        /// Default (english) system message for this alias is ''{0}' can not contain more than {1} characters.'
        ///</summary>
        SystemMessage_MaxLength = 73,
        /// <summary>
        /// Default (english) system message for this alias is ''{0}' is not a valid email address.'
        ///</summary>
        SystemMessage_Email = 74,
        /// <summary>
        /// Default (english) system message for this alias is 'Value in '{0}' has not the valid expression required for this column.'
        ///</summary>
        SystemMessage_Validate = 75,
        /// <summary>
        /// Default (english) system message for this alias is 'Page: '
        ///</summary>
        PagerPrefix = 76,
        /// <summary>
        /// Default (english) system message for this alias is 'Displaying page {0} of {1}, records {2} to {3} of {4} records.'
        ///</summary>
        PagerStatus = 77,
        /// <summary>
        /// Default (english) system message for this alias is 'Update and close window'
        ///</summary>
        UpdateClose = 78,

        /// <summary>
        /// Default (english) system message for this alias is 'Are you sure you want to display {0} records ?'
        /// </summary>
        DisplayRecords,
        /// <summary>
        /// alias for 'Search option' when no filter active.
        /// </summary>
        EmptySearchFilter
    }

    #endregion Enumerations
}