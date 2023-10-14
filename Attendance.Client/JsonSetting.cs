using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Attendance.Client
{
    public class JsonSetting
    {
        public static JsonSerializerSettings TypeNameHandlingAuto = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto};
    }
}
