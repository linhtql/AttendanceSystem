using Attendance.Data.Model;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Attendance.Service
{
    public class EmployeeService : IEmployeeSerice
    {
        private readonly ILogger<EmployeeService> logger;
        private readonly IEmployeeDatabaseService employeeDatabaseService;
        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeDatabaseService employeeDatabaseService)
        {
            this.logger = logger;
            this.employeeDatabaseService = employeeDatabaseService;
        }
        public async Task<Boolean> AddEmployees(List<Employee> employees)
        {
            var invalidEmployees = employees.Where(i => !this.ValidateEmployee(i));
            if (invalidEmployees.Any())
            {
                this.logger.LogError("Invalid Employee Data:\n{0}", String.Join("\n", invalidEmployees.Select(i => i.AccountName)));
            }
            this.logger.LogInformation("Begin add employees, count: {0}", employees.Count());

            foreach (var item in employees)
            {
                item.Password = this.EncryptPassword(item.Password);
            }
            var result = await employeeDatabaseService.AddEmployees(employees);
            if (result)
            {
                this.logger.LogInformation("Create employees successful");
            }

            return result;
        }

        public async Task<Employee> Login(string accountName, string password)
        {
            this.logger.LogInformation("Begin handle login for account: {0}", accountName);

            if (String.IsNullOrEmpty(accountName) || String.IsNullOrEmpty(password))
            {
                this.logger.LogError("Accountname or password is empty.");
                return null;
            }
            var employee = await this.employeeDatabaseService.GetEmployeeByAccountName(accountName);

            if (employee == null)
            {
                this.logger.LogError("Cannot get account by name: {0}", accountName);
                return null;
            }

            var hashedPassword = this.EncryptPassword(password);
            if (!String.Equals(hashedPassword, employee.Password))
            {
                this.logger.LogError("Incorrect password.");
                return null;
            }

            this.logger.LogInformation("Validate login successful for account {0}.", accountName);
            RemoveEmployeePassword(employee);
            return employee;
        }

        private Boolean ValidateEmployee(Employee employee)
        {

            var basicValidateResult = !String.IsNullOrEmpty(employee.AccountName)
                && !String.IsNullOrEmpty(employee.Password)
                && !String.IsNullOrEmpty(employee.FirstName)
                && !String.IsNullOrEmpty(employee.LastName)
                && !String.IsNullOrEmpty(employee.Department)
                && !String.IsNullOrEmpty(employee.PhoneNumber);

            if (basicValidateResult && employee.Extension != null)
            {
                switch (employee.Type)
                {
                    case EmployeeType.Developer:
                        return employee.Extension is DeveloperExtension;
                    case EmployeeType.QualityAssurance:
                        return employee.Extension is QualityAssuaranceExtension;
                    case EmployeeType.Manager:
                        return employee.Extension is ManagerExtension;
                }
            }
            return false;
        }
        private String EncryptPassword(String plainText)
        {
            var bytes = Encoding.UTF8.GetBytes(plainText);
            SHA512.HashData(bytes);
            return Convert.ToBase64String(bytes);
        }
        private void RemoveEmployeePassword(Employee employee)
        {
            employee.Password = null;
        }

        public async Task<Employee> GetEmployeeById(string id)
        {
            var employee = await this.employeeDatabaseService.GetEmployeeById(id);
            RemoveEmployeePassword(employee);
            return employee;
        }

        public Task<List<Employee>> GetManagedEmployees(string department)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Employee>> GetAllEmployees()
        {
            var employees = await employeeDatabaseService.GetAllEmployees();

            employees.ForEach(RemoveEmployeePassword);
            return employees;
        }

        public async Task<List<Employee>> GetEmployeesByDepartment(string department)
        {
            var employees = await employeeDatabaseService.GetEmployeesByDepartment(department);

            employees.ForEach(RemoveEmployeePassword);
            return employees;
        }

        public async Task<Employee> GetEmployeeByAccountName(string accountName)
        {
            var employee = await this.employeeDatabaseService.GetEmployeeByAccountName(accountName);
            RemoveEmployeePassword(employee);
            return employee;
        }
    }
}
