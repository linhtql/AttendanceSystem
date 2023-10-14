// See https://aka.ms/new-console-template for more information
using Attendance.Client;
using Microsoft.Extensions.Configuration;


var helpDocument = new HelperDocument
{
    Description = "Attendance.Client [command] [parameter[1] [parameter2]",
    Commands = new List<HelpCommand>
    {
        new HelpCommand
        {
            Command = "-a",
            Description = "Admin mode, allow to import data"
        },
        new HelpCommand
        {
            Command ="-u",
            Description = "Usermode, input account name and password to login",
            Example = "Attendance.Client - u demo@avepoint.com myPassword"
        }
    }
};

if (args.Length == 1 || args.Length == 3)
{
    var config = GetConfiguration();
    var httpClient = GetHttpClient(config);
    IAsyncClient? client = null;
    // assemble client
    if (String.Equals(args[0], "-a"))
    {
        ConsoleHelper.PrintInfo("Admin mode");
        client = new AdminClient(httpClient);
    }

    if (String.Equals(args[0], "-u") && args.Length == 3)
    {
        ConsoleHelper.PrintInfo("User mode, account name is: {0}", args[1]);
        client = new EmployeeClient(httpClient, args[1], args[2]);
    }

    if (client != null)
    {
        await client.RunAsync();
        return;
    }
}

ConsoleHelper.PrintHelp(helpDocument);

static IConfiguration GetConfiguration()
{
    var configuration = new ConfigurationBuilder();
    configuration.AddJsonFile("appsettings.json");
    return configuration.Build();
}

static HttpClient GetHttpClient(IConfiguration configuration)
{
    var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri(configuration["ApiAddress"]);
    return httpClient;
}