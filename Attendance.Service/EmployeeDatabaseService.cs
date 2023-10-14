using Attendance.Data.Entity;
using Attendance.Data.Model;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace Attendance.Service
{
    public class EmployeeDatabaseService : DatabaseServiceBase, IEmployeeDatabaseService
    {
        public EmployeeDatabaseService(IConfiguration configuration) : base(configuration)
        {

        }
        public async Task<Boolean> AddEmployees(List<Employee> employees)
        {

            var table = new DataTable();
            table.Columns.Add("Id");
            table.Columns.Add("Type");
            table.Columns.Add("AccountName");
            table.Columns.Add("Password");
            table.Columns.Add("FirstName");
            table.Columns.Add("LastName");
            table.Columns.Add("Sex");
            table.Columns.Add("Department");
            table.Columns.Add("PhoneNumber");
            table.Columns.Add("IsIntern");
            table.Columns.Add("Extension");

            foreach (var item in employees.Select(ConvertEmployeeToEntity))
            {
                table.Rows.Add(item.Id,
                    item.Type,
                    item.AccountName,
                    item.Password,
                    item.FirstName,
                    item.LastName,
                    item.Sex,
                    item.Department,
                    item.PhoneNumber,
                    item.IsIntern,
                    item.Extension
                    );
            }
            using (var conn = await GetSqlConnectionAsync())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "BatchAddEmployees";
                cmd.Parameters.Add(new SqlParameter("employees", table));
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<Employee> GetEmployeeByAccountName(string accountName)
        {
            using (var conn = await GetSqlConnectionAsync())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Employee WHERE AccountName=@AccountName";
                cmd.Parameters.Add(new SqlParameter("AccountName", accountName));
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        var entity = ReadEntity(reader);
                        return ConvertEntityToEmployee(entity);
                    }
                }
            }
            return null;
        }

        private EmployeeEntity ReadEntity(SqlDataReader reader)
        {
            var entity = new EmployeeEntity();
            entity.Id = (String)reader["Id"];
            entity.Type = (Int32)reader["Type"];
            entity.AccountName = (String)reader["AccountName"];
            entity.Password = (String)reader["Password"];
            entity.FirstName = (String)reader["FirstName"];
            entity.LastName = (String)reader["LastName"];
            entity.Sex = (Int32)reader["Sex"];
            entity.Department = (String)reader["Department"];
            entity.PhoneNumber = (String)reader["PhoneNumber"];
            entity.IsIntern = (Boolean)reader["IsIntern"];
            entity.Extension = (String)reader["Extension"];

            return entity;
        }

        private Employee ConvertEntityToEmployee(EmployeeEntity entity)
        {
            var result = new Employee();
            result.Id = entity.Id;
            result.Type = (EmployeeType)entity.Type;
            result.AccountName = entity.AccountName;
            result.Password = entity.Password;
            result.FirstName = entity.FirstName;
            result.LastName = entity.LastName;
            result.Sex = (Sex)entity.Sex;
            result.Department = entity.Department;
            result.PhoneNumber = entity.PhoneNumber;
            result.IsIntern = (Boolean)entity.IsIntern;

            if (!String.IsNullOrEmpty(entity.Extension))
            {
                Type extensionType = null;
                switch (result.Type)
                {
                    case EmployeeType.Developer:
                        extensionType = typeof(DeveloperExtension);
                        break;
                    case EmployeeType.QualityAssurance:
                        extensionType = typeof(QualityAssuaranceExtension);
                        break;
                    case EmployeeType.Manager:
                        extensionType = typeof(ManagerExtension);
                        break;

                }
                if (extensionType != null)
                {
                    result.Extension = JsonConvert.DeserializeObject(entity.Extension, extensionType) as EmployeeExtension;
                }

            }
            return result;
        }
        private EmployeeEntity ConvertEmployeeToEntity(Employee employee)
        {
            var result = new EmployeeEntity();

            if (String.IsNullOrEmpty(employee.Id))
            {
                employee.Id = Guid.NewGuid().ToString();
            }

            result.Id = employee.Id;
            result.Type = (Int32)employee.Type;
            result.AccountName = employee.AccountName;
            result.Password = employee.Password;
            result.FirstName = employee.FirstName;
            result.LastName = employee.LastName;
            result.Sex = (Int32)employee.Sex;
            result.Department = employee.Department;
            result.PhoneNumber = employee.PhoneNumber;
            result.IsIntern = (Boolean)employee.IsIntern;
            result.Extension = JsonConvert.SerializeObject(employee.Extension
                , new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });


            return result;
        }

        public async Task<Employee> GetEmployeeById(string id)
        {
            using (var conn = await GetSqlConnectionAsync())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Employee WHERE Id=@Id";
                cmd.Parameters.Add(new SqlParameter("Id", id));
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        var entity = ReadEntity(reader);
                        return ConvertEntityToEmployee(entity);
                    }
                }
            }
            return null;
        }

        public async Task<List<Employee>> GetAllEmployees()
        {
            using (var conn = await GetSqlConnectionAsync())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Employee";
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var result = new List<Employee>();
                    while (reader.Read())
                    {
                        var entity = ReadEntity(reader);
                        result.Add(ConvertEntityToEmployee(entity));
                    }

                    return result;
                }
            }
        }

        public async Task<List<Employee>> GetEmployeesByDepartment(string department)
        {
            using (var conn = await GetSqlConnectionAsync())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Employee WHERE Department=@Department";
                cmd.Parameters.Add(new SqlParameter("Department", department));
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var result = new List<Employee>();
                    while (reader.Read())
                    {
                        var entity = ReadEntity(reader);
                        result.Add(ConvertEntityToEmployee(entity));
                    }

                    return result;
                }
            }
        }
    }
}
