using Attendance.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance.Service
{
    public interface IEmployeeDatabaseService
    {
        Task<Employee> GetEmployeeByAccountName(String accountName);
        Task<Boolean> AddEmployees(List<Employee> employees);
        Task<Employee> GetEmployeeById(String id);
        Task<List<Employee>> GetAllEmployees();
        Task<List<Employee>> GetEmployeesByDepartment(String department);
    }
}
