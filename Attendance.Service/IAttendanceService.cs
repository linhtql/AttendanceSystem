using Attendance.Data.Dto;
using Attendance.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance.Service
{
    public interface IAttendanceService
    {
        Task<Boolean> AddAttendance(String employeeId, AttendanceRecordType type);
        Task<List<AttendanceRecord>> GetAttendanceRecords(String employeeId);
        Task<Boolean> AddAttendanceRecords(List<AttendanceRecord> records);
        Task<List<ManagedAttendanceRecordResult>> GetManagedAttendanceRecords(String managerId);
    }
}
