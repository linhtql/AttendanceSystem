using Attendance.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance.Data.Dto
{
    public class ManagedAttendanceRecordResult
    {
        public Employee Employee { get; set; }
        public List<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    }
}
