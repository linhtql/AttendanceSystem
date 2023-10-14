using Attendance.Data.Dto;
using Attendance.Data.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance.Service
{
    public class AttendanceService : IAttendanceService
    {
        private readonly ILogger<AttendanceService> logger;
        private readonly IEmployeeSerice employeeSerice;
        private readonly IAttendanceDatabaseService attendanceDatabaseService;

        public AttendanceService(ILogger<AttendanceService> logger, IEmployeeSerice employeeSerice, IAttendanceDatabaseService attendanceDatabaseService)
        {
            this.logger = logger;
            this.employeeSerice = employeeSerice;
            this.attendanceDatabaseService = attendanceDatabaseService;
        }

        public IAttendanceDatabaseService AttendanceDatabaseService { get; }

        public async Task<bool> AddAttendance(string employeeId, AttendanceRecordType type)
        {
            // AttendanceRecord
            var currentTime = DateTime.UtcNow;
            var currentDate = currentTime.Date.Ticks;

            var currentRecords = await attendanceDatabaseService.GetRecordByEmplyeeIdAndDate(employeeId, currentDate);

            if (currentRecords == null)
            {
                this.logger.LogInformation("Attendance record not exist, new item will create");
                currentRecords = new AttendanceRecord
                {
                    EmployeeId = employeeId,
                    Date = currentDate,
                };
                this.SetRecordTime(currentRecords, type, currentTime.Ticks);
                var result = await this.attendanceDatabaseService.AddRecord(currentRecords);
                return result;
            }
            else
            {
                this.SetRecordTime(currentRecords, type, currentTime.Ticks);
                var result = await this.attendanceDatabaseService.UpdateRecordTime(currentRecords);
                return result;
            }
        }

        public async Task<bool> AddAttendanceRecords(List<AttendanceRecord> records)
        {
            return await this.attendanceDatabaseService.AddRecords(records);
        }

        public async Task<List<AttendanceRecord>> GetAttendanceRecords(string employeeId)
        {
            var result = await attendanceDatabaseService.GetRecordByEmplyeeId(employeeId);
            if (result == null)
            {
                return result.OrderByDescending(x => x.Date).ToList();
            }
            return result;
        }

        public async Task<List<AttendanceRecord>> GetAttendanceRecordsByEmployee(string employeeId)
        {
            var result = await attendanceDatabaseService.GetRecordByEmplyeeId(employeeId);
            if (result.Any())
            {
                return result.OrderByDescending(x => x.Date).ToList();
            }
            return result;
        }

        public async Task<List<ManagedAttendanceRecordResult>> GetManagedAttendanceRecords(string managerId)
        {
            var manager = await this.employeeSerice.GetEmployeeById(managerId);
            if (manager != null && manager.Type == EmployeeType.Manager)
            {
                if (manager.Extension is ManagerExtension managerExtension)
                {
                    Task<List<Employee>> employeeTask;
                    Task<List<AttendanceRecord>> recordTask;

                    if (managerExtension.ManagerType == ManagerType.DepartmentManager)
                    {
                        employeeTask = employeeSerice.GetEmployeesByDepartment(manager.Department);
                        recordTask = attendanceDatabaseService.GetRecordByDepartment(manager.Department);
                    }
                    else
                    {
                        employeeTask = employeeSerice.GetAllEmployees();
                        recordTask = attendanceDatabaseService.GetAllRecord();
                    }

                    await Task.WhenAll(employeeTask, recordTask);

                    var records = recordTask.Result;
                    var employees = employeeTask.Result;

                    var result = new List<ManagedAttendanceRecordResult>();

                    if (records.Any())
                    {
                        var recordGroup = records.GroupBy(i => i.EmployeeId);

                        foreach (var group in recordGroup)
                        {
                            var employee = employees.FirstOrDefault(i => String.Equals(i.Id, group.Key));

                            if (employee != null)
                            {
                                var item = new ManagedAttendanceRecordResult();
                                item.Employee = employee;
                                item.AttendanceRecords = group.OrderByDescending(i => i.Date).ToList();
                                result.Add(item);
                            }
                            else
                            {
                                this.logger.LogWarning("Cannot find employee with id: {0}", group.Key);
                            }
                        }
                    }
                    return result;
                }
            }
            return null;
        }

        private void SetRecordTime(AttendanceRecord record, AttendanceRecordType type, Int64 time)
        {
            if (type == AttendanceRecordType.Arrival)
            {
                record.ArrivalTime = time;
            }
            else
            {
                record.LeaveTime = time;
            }
        }

    }
}
