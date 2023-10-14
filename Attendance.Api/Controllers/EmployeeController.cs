using Attendance.Data.Dto;
using Attendance.Data.Model;
using Attendance.Service;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.Api.Controllers
{
    [ApiController]
    [Route("employees")]
    public class EmployeeController : Controller
    {
        private readonly ILogger<EmployeeController> logger;
        private readonly IEmployeeSerice employeeSerice;
        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeSerice employeeSerice)
        {
            this.logger = logger;
            this.employeeSerice = employeeSerice;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await employeeSerice.Login(loginDto.AccountName, loginDto.Password);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("accountnames/{accountName}")]
        public async Task<IActionResult> GetEmployeeByAccountName(String accountName)
        {
            var result = await employeeSerice.GetEmployeeByAccountName(accountName);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> ImportEmployees([FromBody] List<Employee> employees)
        {
            var result = await employeeSerice.AddEmployees(employees);
            return result ? Ok() : BadRequest(result);
        }
    }
}
