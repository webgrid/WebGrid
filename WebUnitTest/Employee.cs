/*
Copyright ©  Olav Christian Botterli. All Rights Reserved. 

Ask for permission if you want to use or change any part of the source code.

E-mail: olav@webgrid.com, olav@botterli.com (Olav Botterli)

http://www.webgrid.com
*/


/*
Copyright ©  Olav Christian Botterli. All Rights Reserved. 

Ask for permission if you want to use or change any part of the source code.

E-mail: olav@webgrid.com, olav@botterli.com (Olav Botterli)

http://www.webgrid.com
*/


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;

namespace WebGridUnitTest
{

    public class NorthwindEmployee
    {
        private string _address;
        private string _city;
        private int _employeeID;
        private string _firstName;
        private string _lastName;
        private string _postalCode;
        private string _region;

        public int EmployeeID
        {
            get { return _employeeID; }
            set { _employeeID = value; }
        }

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public string City
        {
            get { return _city; }
            set { _city = value; }
        }

        public string Region
        {
            get { return _region; }
            set { _region = value; }
        }

        public string PostalCode
        {
            get { return _postalCode; }
            set { _postalCode = value; }
        }
    }

   

    public class NorthwindEmployeeData
    {
        private string m_connectionString;


        public NorthwindEmployeeData()
        {
            Initialize();
        }


        public void Initialize()
        {
      
            m_connectionString =
                "Data Source=localhost\\MSSQLSERVER2008;Integrated Security=SSPI;Initial Catalog=Northwind;";
        }


        // Select all employees.

        public List<NorthwindEmployee> GetAllEmployeesList(string sortColumns, int startRecord, int maxRecords)
        {
            VerifySortColumns(sortColumns);

            string sqlCmd = "SELECT EmployeeID, LastName, FirstName, Address, City, Region, PostalCode FROM Employees ";

            if (sortColumns.Trim() == "")
                sqlCmd += "ORDER BY EmployeeID";
            else
                sqlCmd += "ORDER BY " + sortColumns;

            SqlConnection conn = new SqlConnection(m_connectionString);
            SqlCommand cmd = new SqlCommand(sqlCmd, conn);

            SqlDataReader reader = null;
            List<NorthwindEmployee> employees = new List<NorthwindEmployee>();
            int count = 0;

            
                conn.Open();

                reader = cmd.ExecuteReader();

                if (reader != null)
                    while (reader.Read())
                    {
                        if (count >= startRecord)
                        {
                            if (employees.Count < maxRecords)
                                employees.Add(GetNorthwindEmployeeFromReader(reader));
                            else
                                cmd.Cancel();
                        }

                        count++;
                    }
            
            
                if (reader != null)
                {
                    reader.Close();
                }
                conn.Close();
            

            return employees;
        }

        public Collection<NorthwindEmployee> GetAllEmployeesCollection(string sortColumns, int startRecord, int maxRecords)
        {
            VerifySortColumns(sortColumns);

            string sqlCmd = "SELECT EmployeeID, LastName, FirstName, Address, City, Region, PostalCode FROM Employees ";

            if (sortColumns.Trim() == "")
                sqlCmd += "ORDER BY EmployeeID";
            else
                sqlCmd += "ORDER BY " + sortColumns;

            SqlConnection conn = new SqlConnection(m_connectionString);
            SqlCommand cmd = new SqlCommand(sqlCmd, conn);

            SqlDataReader reader = null;
            Collection<NorthwindEmployee> employees = new Collection<NorthwindEmployee>();
            int count = 0;


            conn.Open();

            reader = cmd.ExecuteReader();

            if (reader != null)
                while (reader.Read())
                {
                    if (count >= startRecord)
                    {
                        if (employees.Count < maxRecords)
                            employees.Add(GetNorthwindEmployeeFromReader(reader));
                        else
                            cmd.Cancel();
                    }

                    count++;
                }


            if (reader != null)
            {
                reader.Close();
            }
            conn.Close();


            return employees;
        }
        public Dictionary<int,NorthwindEmployee> GetAllEmployeesDictionary(string sortColumns, int startRecord, int maxRecords)
        {
            VerifySortColumns(sortColumns);

            string sqlCmd = "SELECT EmployeeID, LastName, FirstName, Address, City, Region, PostalCode FROM Employees ";

            if (sortColumns.Trim() == "")
                sqlCmd += "ORDER BY EmployeeID";
            else
                sqlCmd += "ORDER BY " + sortColumns;

            SqlConnection conn = new SqlConnection(m_connectionString);
            SqlCommand cmd = new SqlCommand(sqlCmd, conn);

            SqlDataReader reader = null;
            Dictionary<int,NorthwindEmployee> employees = new Dictionary<int,NorthwindEmployee>();
            int count = 0;


            conn.Open();

            reader = cmd.ExecuteReader();

            if (reader != null)
                while (reader.Read())
                {
                    if (count >= startRecord)
                    {
                        if (employees.Count < maxRecords)
                            employees.Add(count,GetNorthwindEmployeeFromReader(reader));
                        else
                            cmd.Cancel();
                    }

                    count++;
                }


            if (reader != null)
            {
                reader.Close();
            }
            conn.Close();


            return employees;
        }
        //////////
        // Verify that only valid columns are specified in the sort expression to avoid a SQL Injection attack.

        private static void VerifySortColumns(string sortColumns)
        {
            if (sortColumns.ToLowerInvariant().EndsWith(" desc"))
                sortColumns = sortColumns.Substring(0, sortColumns.Length - 5);

            string[] columnNames = sortColumns.Split(',');

            foreach (string columnName in columnNames)
            {
                switch (columnName.Trim().ToLowerInvariant())
                {
                    case "employeeid":
                        break;
                    case "lastname":
                        break;
                    case "firstname":
                        break;
                    case "":
                        break;
                    default:
                        throw new ArgumentException("SortColumns contains an invalid column name. (" +
                                                    columnName.Trim().ToLowerInvariant() + ")");
                }
            }
        }


        private static NorthwindEmployee GetNorthwindEmployeeFromReader(IDataRecord reader)
        {
            NorthwindEmployee employee = new NorthwindEmployee();

            employee.EmployeeID = reader.GetInt32(0);
            employee.LastName = reader.GetString(1);
            employee.FirstName = reader.GetString(2);

            if (reader.GetValue(3) != DBNull.Value)
                employee.Address = reader.GetString(3);

            if (reader.GetValue(4) != DBNull.Value)
                employee.City = reader.GetString(4);

            if (reader.GetValue(5) != DBNull.Value)
                employee.Region = reader.GetString(5);

            if (reader.GetValue(6) != DBNull.Value)
                employee.PostalCode = reader.GetString(6);

            return employee;
        }


        // Select an employee.

        public List<NorthwindEmployee> GetEmployee(int EmployeeID)
        {
            try
            {
                SqlConnection conn = new SqlConnection(m_connectionString);
                SqlCommand cmd =
                    new SqlCommand("SELECT EmployeeID, LastName, FirstName, Address, City, Region, PostalCode " +
                                   "  FROM Employees WHERE EmployeeID = @EmployeeID", conn);
                cmd.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = EmployeeID;

                SqlDataReader reader = null;
                List<NorthwindEmployee> employees = new List<NorthwindEmployee>();

                try
                {
                    conn.Open();

                    reader = cmd.ExecuteReader(CommandBehavior.SingleRow);

                    if (reader != null)
                        while (reader.Read())
                            employees.Add(GetNorthwindEmployeeFromReader(reader));
                }
                catch (SqlException)
                {
                    // Handle exception.
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }

                    conn.Close();
                }

                return employees;
            }
            catch
            {
                return null;
            }
        }


        //
        // Update the Employee by ID.
        //   This method assumes that ConflictDetection is set to OverwriteValues.

        public int UpdateEmployee(NorthwindEmployee employee)
        {
            if (String.IsNullOrEmpty(employee.FirstName))
                throw new ArgumentException("FirstName cannot be null or an empty string.");
            if (String.IsNullOrEmpty(employee.LastName))
                throw new ArgumentException("LastName cannot be null or an empty string.");

            if (employee.Address == null)
            {
                employee.Address = String.Empty;
            }
            if (employee.City == null)
            {
                employee.City = String.Empty;
            }
            if (employee.Region == null)
            {
                employee.Region = String.Empty;
            }
            if (employee.PostalCode == null)
            {
                employee.PostalCode = String.Empty;
            }

            SqlConnection conn = new SqlConnection(m_connectionString);
            SqlCommand cmd = new SqlCommand("UPDATE Employees " +
                                            "  SET FirstName=@FirstName, LastName=@LastName, " +
                                            "  Address=@Address, City=@City, Region=@Region, " +
                                            "  PostalCode=@PostalCode " +
                                            "  WHERE EmployeeID=@EmployeeID", conn);

            cmd.Parameters.Add("@FirstName", SqlDbType.VarChar, 10).Value = employee.FirstName;
            cmd.Parameters.Add("@LastName", SqlDbType.VarChar, 20).Value = employee.LastName;
            cmd.Parameters.Add("@Address", SqlDbType.VarChar, 60).Value = employee.Address;
            cmd.Parameters.Add("@City", SqlDbType.VarChar, 15).Value = employee.City;
            cmd.Parameters.Add("@Region", SqlDbType.VarChar, 15).Value = employee.Region;
            cmd.Parameters.Add("@PostalCode", SqlDbType.VarChar, 10).Value = employee.PostalCode;
            cmd.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = employee.EmployeeID;

            int result = 0;

            try
            {
                conn.Open();

                result = cmd.ExecuteNonQuery();
            }
            catch (SqlException)
            {
                // Handle exception.
            }
            finally
            {
                conn.Close();
            }

            return result;
        }


        // Insert an Employee.

        public int InsertEmployee(NorthwindEmployee employee)
        {
            if (String.IsNullOrEmpty(employee.FirstName))
                throw new ArgumentException("FirstName cannot be null or an empty string.");
            if (String.IsNullOrEmpty(employee.LastName))
                throw new ArgumentException("LastName cannot be null or an empty string.");

            if (employee.Address == null)
            {
                employee.Address = String.Empty;
            }
            if (employee.City == null)
            {
                employee.City = String.Empty;
            }
            if (employee.Region == null)
            {
                employee.Region = String.Empty;
            }
            if (employee.PostalCode == null)
            {
                employee.PostalCode = String.Empty;
            }

            SqlConnection conn = new SqlConnection(m_connectionString);
            SqlCommand cmd = new SqlCommand("INSERT INTO Employees " +
                                            "  (FirstName, LastName, Address, City, Region, PostalCode) " +
                                            "  Values(@FirstName, @LastName, @Address, @City, @Region, @PostalCode); " +
                                            "SELECT @EmployeeID = SCOPE_IDENTITY()", conn);

            cmd.Parameters.Add("@FirstName", SqlDbType.VarChar, 10).Value = employee.FirstName;
            cmd.Parameters.Add("@LastName", SqlDbType.VarChar, 20).Value = employee.LastName;
            cmd.Parameters.Add("@Address", SqlDbType.VarChar, 60).Value = employee.Address;
            cmd.Parameters.Add("@City", SqlDbType.VarChar, 15).Value = employee.City;
            cmd.Parameters.Add("@Region", SqlDbType.VarChar, 15).Value = employee.Region;
            cmd.Parameters.Add("@PostalCode", SqlDbType.VarChar, 10).Value = employee.PostalCode;
            SqlParameter p = cmd.Parameters.Add("@EmployeeID", SqlDbType.Int);
            p.Direction = ParameterDirection.Output;

            int newEmployeeID = 0;

            try
            {
                conn.Open();

                cmd.ExecuteNonQuery();

                newEmployeeID = (int)p.Value;
            }
            catch (SqlException)
            {
                // Handle exception.
            }
            finally
            {
                conn.Close();
            }

            return newEmployeeID;
        }


        //
        // Delete the Employee by ID.
        //   This method assumes that ConflictDetection is set to OverwriteValues.

        public int DeleteEmployee(NorthwindEmployee employee)
        {
            const string sqlCmd = "DELETE FROM Employees WHERE EmployeeID = @EmployeeID";

            SqlConnection conn = new SqlConnection(m_connectionString);
            SqlCommand cmd = new SqlCommand(sqlCmd, conn);
            cmd.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = employee.EmployeeID;

            int result = 0;

            try
            {
                conn.Open();

                result = cmd.ExecuteNonQuery();
            }
            catch (SqlException)
            {
                // Handle exception.
            }
            finally
            {
                conn.Close();
            }

            return result;
        }
    }
}
