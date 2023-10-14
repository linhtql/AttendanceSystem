using Microsoft.Extensions.DependencyInjection;

namespace Attendance.Service
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddAttendanceService(this IServiceCollection services)
        {
            services.AddSingleton<IEmployeeSerice, EmployeeService>();
            services.AddSingleton<IEmployeeDatabaseService, EmployeeDatabaseService>();

            services.AddSingleton<IAttendanceService, AttendanceService>();
            services.AddSingleton<IAttendanceDatabaseService, AttendanceDatabaseService>();


            return services;
        }
    }
}