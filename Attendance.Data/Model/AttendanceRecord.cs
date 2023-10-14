using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance.Data.Model
{
    public class AttendanceRecord
    {
        public String Id { get; set; }
        public String EmployeeId { get; set; }
        public Int64 Date { get; set; }
        public Int64 ArrivalTime { get; set; }
        public Int64 LeaveTime { get; set; }
    }
}
