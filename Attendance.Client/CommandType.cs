using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance.Client
{
    public enum CommandType
    {
        Help,
        AddAttendanceRecord,
        GetMyAttendanceRecord,
        GetEmployeeAttendanceRecord,
        GetManagedEmployeeAttendanceRecords,
        Exit
    }
}
