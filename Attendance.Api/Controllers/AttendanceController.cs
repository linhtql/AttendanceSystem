using Attendance.Data.Dto;
using Attendance.Data.Model;
using Attendance.Service;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.Api.Controllers
{
    [Route("api/attendances")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            this.attendanceService = attendanceService;
        }

        [HttpPost("records/{employeeId}/{type}")]
        public async Task<IActionResult> AddAttendanceRecord([FromRoute] String employeeId, [FromRoute] AttendanceRecordType type)
        {
            var result = await this.attendanceService.AddAttendance(employeeId, type);
            return result ? Ok() : BadRequest();
        }

        [HttpGet("records/{employeeId}")]
        public async Task<IActionResult> GetEmplyeeAttendances(String employeeId)
        {
            var attendanceRecord = await attendanceService.GetAttendanceRecords(employeeId);
            if (attendanceRecord.Any())
            {
                return Ok(attendanceRecord);
            }
            return NotFound();
        }

        [HttpPost("records")]
        public async Task<IActionResult> ImportRecords([FromBody] List<AttendanceRecord> records)
        {
            var result = await attendanceService.AddAttendanceRecords(records);

            return result ? Ok() : BadRequest(new
            {
                Message = "Fail to import recods"
            });
        }

        [HttpGet("records/{managerId}/managed")]
        public async Task<IActionResult> GetManagedAttendanceRecord([FromRoute] String managerId)
        {
            var result = await this.attendanceService.GetManagedAttendanceRecords(managerId);
            if (result.Any())
            {
                return Ok(result);
            }
            return NotFound();
        }
    }
}

