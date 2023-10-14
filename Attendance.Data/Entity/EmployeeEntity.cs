using Attendance.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance.Data.Entity
{
    public class EmployeeEntity
    {
        public string Id { get; set; }
        public Int32 Type { get; set; }
        public String AccountName { get; set; }
        public String Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Int32 Sex { get; set; }
        public string Department { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsIntern { get; set; }

        public String Extension { get; set; }
    }
}
