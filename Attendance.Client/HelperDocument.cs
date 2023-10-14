using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance.Client
{
    public class HelperDocument
    {
        public String Description { get; set; }
        public List<HelpCommand> Commands { get; set; } = new List<HelpCommand>();  
    }
}
