using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attendance.Data;
using Attendance.Data.Dto;
using Attendance.Data.Model;
using Newtonsoft.Json;

namespace Attendance.Client
{
    public class EmployeeClient : IAsyncClient
    {
        private readonly HttpClient httpClient;
        private readonly string accountName;
        private readonly string password;
        private Employee currentEmployee;

        public EmployeeClient(HttpClient httpClient, String accountName, String password)
        {
            this.httpClient = httpClient;
            this.accountName = accountName;
            this.password = password;
        }
        public async Task RunAsync()
        {
            this.currentEmployee = await this.Login();
            if (this.currentEmployee != null)
            {
                ConsoleHelper.PrintInfo("login successfull. Employee role is {0}", this.currentEmployee.Type);
                this.PrintEmployeeInfo(this.currentEmployee);
                this.PrintHelp();
                await this.BeginHandleInputs();
            }
        }

        private async Task<Employee> Login()
        {
            try
            {
                using (var request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(this.httpClient.BaseAddress + "api/employees/login");

                    var loginDto = new LoginDto
                    {
                        AccountName = accountName,
                        Password = password
                    };

                    using (var content = new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json"))
                    {
                        request.Content = content;

                        using (var response = await this.httpClient.SendAsync(request))
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            if (response.IsSuccessStatusCode && !String.IsNullOrEmpty(responseContent))
                            {
                                var result = JsonConvert.DeserializeObject<Employee>(responseContent, JsonSetting.TypeNameHandlingAuto);
                                if (result != null)
                                {
                                    return result;
                                }
                            }
                            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                            {
                                ConsoleHelper.PrintWarning("Account name or password not correct.");
                            }
                            else
                            {
                                ConsoleHelper.PrintError("An error occurred.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                ConsoleHelper.PrintError("Failed to login. {0}, ex");
            }
            return null;
        }
        private async Task BeginHandleInputs()
        {
            var flag = true;
            while (flag)
            {
                try
                {
                    var input = Console.ReadLine();

                    var (commandType, arguments) = AnalyzeInput(input);

                    switch (commandType)
                    {
                        case CommandType.Help:
                            PrintHelp();
                            break;
                        case CommandType.AddAttendanceRecord:
                            await this.AddAttendanceRecord(arguments);
                            break;
                        case CommandType.GetMyAttendanceRecord:
                            await this.GetMyAttendanceRecord(commandType); break;
                        case CommandType.GetEmployeeAttendanceRecord:
                            await this.GetAttendanceRecordsByAccountName(arguments);
                            break;
                        case CommandType.GetManagedEmployeeAttendanceRecords:
                            await this.GetManagedEmployeeAttendanceRecords();
                            break;
                        case CommandType.Exit:
                            flag = false;
                            break;
                        default: break;
                    }

                }
                catch (Exception ex)
                {
                    ConsoleHelper.PrintError("An error occurred. {0}", ex);
                    PrintHelp();
                }
            }
        }

        private async Task GetMyAttendanceRecord(CommandType commandType)
        {
            var records = await this.GetAttendanceRecordsByEmployeeId(this.currentEmployee.Id);
            this.PrintRecords(records);
        }

        private async Task GetAttendanceRecordsByAccountName(List<String> arguments)
        {
            if (arguments != null && arguments.Any())
            {
                var accountName = arguments[0];
                var employee = await GetEmployeeByAccountName(accountName);
                if (employee != null)
                {
                    var records = await GetAttendanceRecordsByEmployeeId(employee.Id);
                    this.PrintEmployeeAndAttendanceRecords(employee, records);
                }

            }
        }

        private async Task<List<AttendanceRecord>> GetAttendanceRecordsByEmployeeId(String employeeId)
        {
            using (var response = await this.httpClient.GetAsync($"api/attendances/records/{employeeId}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var records = JsonConvert.DeserializeObject<List<AttendanceRecord>>(content);
                    if (records != null)
                    {
                        return records;
                    }
                }
            }
            return new List<AttendanceRecord>();
        }
        private void PrintEmployeeAndAttendanceRecords(Employee employee, List<AttendanceRecord> records)
        {
            this.PrintEmployeeInfo(employee);
            this.PrintRecords(records);
        }

        private async Task<Employee> GetEmployeeByAccountName(String accountName)
        {
            using (var response = await this.httpClient.GetAsync($"api/employees/accountnames/{accountName}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var str = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Employee>(str, JsonSetting.TypeNameHandlingAuto);
                }
            }
            return null;
        }

        private async Task AddAttendanceRecord(List<String> arguments)
        {
            if (arguments != null && arguments.Count == 1 && Int32.TryParse(arguments[0], out var typeNumber))
            {
                using (var response = await this.httpClient.PostAsync($"api/attendances/records/{this.currentEmployee.Id}/{typeNumber}", null))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        ConsoleHelper.PrintInfo("Add attendance record successful.");
                    }
                    else
                    {
                        ConsoleHelper.PrintError("Add attendance record failed.");
                    }
                }
            }
            else
            {
                ConsoleHelper.PrintError("Add attendance record failed");
            }
        }
        private async Task GetAttendanceRecords(CommandType type, List<String> arguments)
        {
            var employeeId = String.Empty;
            if (type == CommandType.GetMyAttendanceRecord)
            {
                employeeId = this.currentEmployee.Id;
            }
            else
            {
                if (this.IsManager() && arguments != null && arguments.Count == 1)
                {
                    employeeId = arguments.FirstOrDefault();
                }
            }
            if (!String.IsNullOrEmpty(employeeId))
            {
                using (var response = await this.httpClient.GetAsync($"api/attendances/records/{employeeId}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var records = JsonConvert.DeserializeObject<List<AttendanceRecord>>(content);
                        if (records != null)
                        {
                            this.PrintRecords(records);
                        }
                    }
                }
            }
        }
        private async Task GetManagedEmployeeAttendanceRecords()
        {
            if (this.IsManager())
            {
                using (var response = await this.httpClient.GetAsync($"api/Attendances/records/{this.currentEmployee.Id}/managed"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content?.ReadAsStringAsync();
                        if (!String.IsNullOrEmpty(content))
                        {
                            var records = JsonConvert.DeserializeObject<List<ManagedAttendanceRecordResult>>(content, JsonSetting.TypeNameHandlingAuto);
                            if (records != null)
                            {
                                this.PrintManagedEmployeeAttendanceRecords(records);
                            }
                        }
                    }
                }
            }
        }
        private void PrintManagedEmployeeAttendanceRecords(List<ManagedAttendanceRecordResult> records)
        {
            foreach (var item in records)
            {
                Console.WriteLine();
                this.PrintEmployeeAndAttendanceRecords(item.Employee, item.AttendanceRecords);
                Console.WriteLine();
            }
        }
        private void PrintRecords(List<AttendanceRecord> records)
        {
            var sb = new StringBuilder();
            foreach (var item in records.OrderByDescending(i => i.Date))
            {
                var date = new DateTime(item.Date, DateTimeKind.Utc).ToLocalTime();
                var arrivalTime = new DateTime(item.ArrivalTime, DateTimeKind.Utc).ToLocalTime();
                var leavingTime = new DateTime(item.LeaveTime, DateTimeKind.Utc).ToLocalTime();

                //
                sb.AppendLine($"Date: {date.ToShortDateString()}, ArrivalTime: {arrivalTime.ToShortTimeString()}," +
                    $" LeaveTime: {leavingTime.ToShortTimeString()}");
            }
            Console.WriteLine(sb);
        }
        private (CommandType, List<string>) AnalyzeInput(String input)
        {
            var type = CommandType.Help;
            var arguments = new List<string>();
            if (!String.IsNullOrEmpty(input))
            {
                var splitResult = input.Split(' ');
                if (splitResult.Length > 0 && !String.IsNullOrEmpty(splitResult[0]))
                {
                    var firstCommand = splitResult[0].ToLower();
                    type = firstCommand switch
                    {
                        "add-record" => CommandType.AddAttendanceRecord,
                        "get-record" => CommandType.GetMyAttendanceRecord,
                        "get-employee-record" => CommandType.GetEmployeeAttendanceRecord,
                        "get-employee-records" => CommandType.GetManagedEmployeeAttendanceRecords,
                        "exit" => CommandType.Exit,
                        _ => CommandType.Help
                    };

                    arguments = splitResult.Skip(1).ToList();
                }
            }


            return (type, arguments);
        }
        private void PrintEmployeeInfo(Employee employee)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{employee.FirstName} {employee.LastName} ({employee.AccountName})");

            var extensionStr = String.Empty;
            switch (employee.Type)
            {
                case EmployeeType.Developer:
                    extensionStr = (employee.Extension as DeveloperExtension)?.Band.ToString();
                    break;
                case EmployeeType.QualityAssurance:
                    var qaExt = employee.Extension as QualityAssuaranceExtension;
                    if (qaExt != null)
                    {
                        extensionStr = qaExt.Band.ToString();
                        if (qaExt.CanWriteCode)
                        {
                            extensionStr += ", can write code";
                        }
                    }
                    break;
                case EmployeeType.Manager:
                    extensionStr = (employee.Extension as ManagerExtension)?.ManagerType.ToString();
                    break;
            }
            sb.AppendLine($"{employee.Department} ");
            if (employee.Type != EmployeeType.Manager)
            {
                sb.Append(employee.Type.ToString()).Append(",");
            }
            sb.Append(extensionStr);
            Console.WriteLine(sb.ToString());
        }

        private void PrintHelp()
        {
            var doc = new HelperDocument();
            doc.Description = "Please refer commands below:";
            doc.Commands.Add(new HelpCommand
            {
                Command = "add-record [record-type",
                Description = "Add attendance record when arriving or leaving office. 1 - arrive time, 2 - leave office"
            });

            doc.Commands.Add(new HelpCommand
            {
                Command = "get-record",
                Description = "View my attendance record"
            });

            if (IsManager())
            {
                doc.Commands.Add(new HelpCommand
                {
                    Command = "get-employee-record [account name]",
                    Description = "Get employee attendance records by account name"
                });

                doc.Commands.Add(new HelpCommand
                {
                    Command = "get-employee-records",
                    Description = "View your managed employees's attendance record"
                });
            }
            doc.Commands.Add(new HelpCommand
            {
                Command = "exit",
                Description = "Exit system"
            });

            ConsoleHelper.PrintHelp(doc);
        }
        private Boolean IsManager()
        {
            return this.currentEmployee.Type == EmployeeType.Manager;
        }


    }
}
