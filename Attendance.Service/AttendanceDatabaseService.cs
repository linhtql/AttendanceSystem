using Attendance.Data.Entity;
using Attendance.Data.Model;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance.Service
{
    public class AttendanceDatabaseService : DatabaseServiceBase, IAttendanceDatabaseService
    {
        public AttendanceDatabaseService(IConfiguration configuration) : base(configuration)
        {

        }
        public async Task<Boolean> AddRecord(AttendanceRecord record)
        {
            using (var conn = await GetSqlConnectionAsync())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO [dbo].[AttendanceRecord]
                                           ([Id]
                                           ,[EmployeeId]
                                           ,[Date]
                                           ,[ArrivalTime]
                                           ,[LeaveTime])
                                     VALUES
                                           (@Id,
                                           @EmployeeId,
                                           @Date,
                                           @ArrivalTime,
                                           @LeaveTime)";
                cmd.Parameters.AddRange(this.GetSqlParameters(ConvertToEntity(record)));
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<AttendanceRecord> GetRecordByEmplyeeIdAndDate(string employeeId, long date)
        {
            using (var conn = await this.GetSqlConnectionAsync())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM [AttendanceRecord] WHERE [EmployeeId]=@EmployeeId AND [Date]=@Date";
                cmd.Parameters.Add(new SqlParameter("EmployeeId", employeeId));
                cmd.Parameters.Add(new SqlParameter("Date", date));

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var entity = this.ReadEntity(reader);
                        return this.ConvertToRecord(entity);
                    }
                }
            }
            return null;
        }

        public async Task<bool> UpdateRecordTime(AttendanceRecord record)
        {
            var entity = ConvertToEntity(record);
            using (var conn = await GetSqlConnectionAsync())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "UPDATE [dbo].[AttendanceRecord] SET [ArrivalTime]=@ArrivalTime, [LeaveTime]=@LeaveTime WHERE [Id]=@Id";

                cmd.Parameters.Add(new SqlParameter("Id", entity.Id));
                cmd.Parameters.Add(new SqlParameter("ArrivalTime", entity.ArrivalTime));
                cmd.Parameters.Add(new SqlParameter("LeaveTime", entity.LeaveTime));

                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        private AttendanceRecordEntity ReadEntity(SqlDataReader reader)
        {
            var result = new AttendanceRecordEntity();

            result.Id = (String)reader["Id"];
            result.EmployeeId = (String)reader["EmployeeId"];
            result.Date = (Int64)reader["Date"];
            result.ArrivalTime = (Int64)reader["ArrivalTime"];
            result.LeaveTime = (Int64)reader["LeaveTime"];

            return result;
        }

        private AttendanceRecord ConvertToRecord(AttendanceRecordEntity entity)
        {
            return new AttendanceRecord
            {
                Id = entity.Id,
                EmployeeId = entity.EmployeeId,
                Date = entity.Date,
                ArrivalTime = entity.ArrivalTime,
                LeaveTime = entity.LeaveTime
            };
        }

        private AttendanceRecordEntity ConvertToEntity(AttendanceRecord record)
        {
            if (String.IsNullOrEmpty(record.Id))
            {
                record.Id = Guid.NewGuid().ToString();
            }

            return new AttendanceRecordEntity
            {
                Id = record.Id,
                EmployeeId = record.EmployeeId,
                Date = record.Date,
                ArrivalTime = record.ArrivalTime,
                LeaveTime = record.LeaveTime
            };
        }

        public async Task<List<AttendanceRecord>> GetRecordByEmplyeeId(string employeeId)
        {
            using (var conn = await this.GetSqlConnectionAsync())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM [AttendanceRecord] WHERE [EmployeeId]=@EmployeeId";
                cmd.Parameters.Add(new SqlParameter("EmployeeId", employeeId));
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var result = new List<AttendanceRecord>();
                    while (await reader.ReadAsync())
                    {
                        var entity = this.ReadEntity(reader);
                        result.Add(ConvertToRecord(entity));
                    }
                    return result;
                }
            }
        }

        public async Task<Boolean> AddRecords(List<AttendanceRecord> records)
        {
            var table = new DataTable();
            table.Columns.Add("Id");
            table.Columns.Add("EmployeeId");
            table.Columns.Add("Date");
            table.Columns.Add("ArrivalTime");
            table.Columns.Add("LeaveTime");

            foreach (var item in records.Select(ConvertToEntity))
            {
                table.Rows.Add(
                    item.Id,
                    item.EmployeeId,
                    item.Date,
                    item.ArrivalTime,
                    item.LeaveTime
                    );
            }

            using (var conn = await this.GetSqlConnectionAsync())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "BatchAddAttendanceRecord";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("records", table));
                return await cmd.ExecuteNonQueryAsync() > 0;

            }
        }

        public async Task<List<AttendanceRecord>> GetAllRecord()
        {
            using (var conn = await this.GetSqlConnectionAsync())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM [AttendanceRecord]";
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var result = new List<AttendanceRecord>();
                    while (await reader.ReadAsync())
                    {
                        var entity = this.ReadEntity(reader);
                        result.Add(ConvertToRecord(entity));
                    }
                    return result;
                }
            }
        }

        public async Task<List<AttendanceRecord>> GetRecordByDepartment(string department)
        {
            using (var conn = await this.GetSqlConnectionAsync())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM [AttendanceRecord] a JOIN [Employee] e ON a.EmployeeId=e.Id WHERE e.Department=@Department";
                cmd.Parameters.Add(new SqlParameter("Department", department));
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var result = new List<AttendanceRecord>();
                    while (await reader.ReadAsync())
                    {
                        var entity = this.ReadEntity(reader);
                        result.Add(ConvertToRecord(entity));
                    }
                    return result;
                }
            }
        }
    }
}
