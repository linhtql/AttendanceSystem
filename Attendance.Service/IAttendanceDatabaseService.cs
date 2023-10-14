using Attendance.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance.Service
{
    public interface IAttendanceDatabaseService
    {
        Task<AttendanceRecord> GetRecordByEmplyeeIdAndDate(String employeeId, Int64 date);
        Task<List<AttendanceRecord>> GetRecordByEmplyeeId(String employeeId);
        Task<List<AttendanceRecord>> GetAllRecord();
        Task<List<AttendanceRecord>> GetRecordByDepartment(String department);
        Task<Boolean> AddRecord(AttendanceRecord record);
        Task<Boolean> UpdateRecordTime(AttendanceRecord record);
        Task<Boolean> AddRecords(List<AttendanceRecord> records);
    }
}
