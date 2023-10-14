using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance.Client
{
    public class ConsoleHelper
    {
        public static void PrintInfo(String format, params Object[] objects)
        {
            Console.WriteLine("INFO: " + String.Format(format, objects));
        }
        public static void PrintWarning(String format, params Object[] objects)
        {
            Console.WriteLine("WARNING: " + String.Format(format, objects));
        }
        public static void PrintError(String format, params Object[] objects)
        {
            Console.WriteLine("ERROR: " + String.Format(format, objects));
        }
        public static void PrintHelp(HelperDocument document)
        {
            var sb = new StringBuilder();
            sb.AppendLine(document.Description);
            sb.AppendLine("Commands:");
            foreach(var cmd in document.Commands)
            {
                sb.AppendLine($"       {cmd.Command}\t{cmd.Description}");
                if (!String.IsNullOrEmpty(cmd.Example))
                {
                    sb.AppendLine($"\te.g: {cmd.Example}");
                }
            }
            Console.WriteLine(sb);
        }
    }
}
