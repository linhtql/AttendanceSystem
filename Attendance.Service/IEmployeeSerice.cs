using Attendance.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance.Service
{
    public interface IEmployeeSerice
    {
        Task<Employee> Login (String accountName, String password);
        Task<Boolean> AddEmployees(List<Employee> employees);
        Task<Employee> GetEmployeeById(String id);
        Task<Employee> GetEmployeeByAccountName(String accountName);
        Task<List<Employee>> GetManagedEmployees(String department);
        Task<List<Employee>> GetAllEmployees();
        Task<List<Employee>> GetEmployeesByDepartment(String department);

    }
}
