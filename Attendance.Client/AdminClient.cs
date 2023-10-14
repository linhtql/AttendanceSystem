using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance.Client
{
    public class AdminClient : IAsyncClient
    {
        private readonly HttpClient httpClient;
        public AdminClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        public async Task RunAsync()
        {
            this.PrintHelp();

            var flag = true;
            while (flag)
            {
                try
                {
                    var line = Console.ReadLine();
                    if (!String.IsNullOrEmpty(line))
                    {
                        var split = line.Split(' ');
                        if (split.Length == 2)
                        {
                            if (split[0].ToLower() == "import-employees")
                            {
                                await ImportEmployees(split[1]);
                            }

                            if (split[0].ToLower() == "import-attendance-record")
                            {
                                
                                await ImportAttendanceRecords(split[1]);
                            }
                        }
                        else if (split.Length == 1 && split[0].ToLower() == "exit")
                        {
                            flag = false;
                        }
                        else
                        {
                            PrintHelp();
                        }
                    }
                }
                catch (FileNotFoundException ex)
                {

                    ConsoleHelper.PrintError("Json file not found. {0}", ex);
                }
            }
        }

        private async Task<String> ReadText(String path)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }
            return await File.ReadAllTextAsync(filePath);
        }

        private async Task ImportEmployees(String path)
        {
            var text = await ReadText(path);

            using (var request = new HttpRequestMessage(HttpMethod.Post, "api/employees"))
            {
                request.Content = new StringContent(text, Encoding.UTF8, "application/json");
                using (var response = await httpClient.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        ConsoleHelper.PrintInfo("Employee created");
                    }
                }
            }
        }
        private async Task ImportAttendanceRecords(String path)
        {
            var text = await ReadText(path);

            using (var request = new HttpRequestMessage(HttpMethod.Post, "api/attendances/records"))
            {
                request.Content = new StringContent(text, Encoding.UTF8, "application/json");
                using (var response = await httpClient.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        ConsoleHelper.PrintInfo("Attendance created");
                    }
                }
            }
        }

        private void PrintHelp()
        {
            var doc = new HelperDocument();
            doc.Description = "Admin mode, please refer below commands";
            doc.Commands.Add(new HelpCommand
            {
                Command = "import-employees [path]",
                Description = "Import employees from json file"
            });
            doc.Commands.Add(new HelpCommand
            {
                Command = "import-attendance-record [path]",
                Description = "Import attendance records from json file"
            });
            doc.Commands.Add(new HelpCommand
            {
                Command = "exit",
                Description = "Exit client"
            });

            ConsoleHelper.PrintHelp(doc);
        }
    }
}
