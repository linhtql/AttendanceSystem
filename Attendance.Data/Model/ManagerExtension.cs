using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance.Data.Model
{
    public class ManagerExtension : EmployeeExtension
    {
        public ManagerType ManagerType { get; set; }
    }
}
