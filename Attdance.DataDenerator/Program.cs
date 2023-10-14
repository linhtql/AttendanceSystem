using Attendance.Data.Model;
using Newtonsoft.Json;

var employee = PrepairEmployees();
var records = PrepairAttendanceRecords(employee.Select(i => i.Id));

var jsonSetting = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

var employeeJson = JsonConvert.SerializeObject(employee, jsonSetting);
var recordJson = JsonConvert.SerializeObject(records, jsonSetting);


var id = Guid.NewGuid();

await File.WriteAllTextAsync($"employees-{id}.json", employeeJson);
await File.WriteAllTextAsync($"records-{id}.json", recordJson);
static List<Employee> PrepairEmployees()
{
    var result = new List<Employee>(); ;

    #region Developer
    result.Add(new Employee
    {
        Id = Guid.NewGuid().ToString(),
        AccountName = "sophia.tran@avepoint.com",
        Password = "test_sophia",
        Department = "DN_OFFICE",
        FirstName = "Sophia",
        LastName = "Tran",
        PhoneNumber = "0961389719",
        Sex = Sex.Male,
        Type = EmployeeType.Developer,
        Extension = new DeveloperExtension
        {
            Band = Band.A6,
            TechDirection = "Server"
        }

    });

    result.Add(new Employee
    {
        Id = Guid.NewGuid().ToString(),
        AccountName = "jay.ha@avepoint.com",
        Password = "test_jay",
        Department = "DN_OFFICE",
        FirstName = "Jay",
        LastName = "Ha",
        PhoneNumber = "11223344",
        Sex = Sex.Female,
        Type = EmployeeType.Developer,
        Extension = new DeveloperExtension
        {
            Band = Band.A3,
            TechDirection = "GUI"
        }

    });

    result.Add(new Employee
    {
        Id = Guid.NewGuid().ToString(),
        AccountName = "test.test@avepoint.com",
        Password = "test_test",
        Department = "HN_OFFICE",
        FirstName = "Test",
        LastName = "test",
        PhoneNumber = "0961389719",
        Sex = Sex.Female,
        Type = EmployeeType.Developer,
        IsIntern = true,
        Extension = new DeveloperExtension
        {
            Band = Band.A1,
            TechDirection = "GUI"
        }

    }); ;

    #endregion

    #region QA
    result.Add(new Employee
    {
        Id = Guid.NewGuid().ToString(),
        AccountName = "mia.trashin@avepoint.com",
        Password = "test_mia",
        Department = "DN_OFFICE_",
        FirstName = "Mia",
        LastName = "Trashin",
        PhoneNumber = "0961389719",
        Sex = Sex.Male,
        Type = EmployeeType.QualityAssurance,
        Extension = new QualityAssuaranceExtension
        {
            Band = Band.A6,
            CanWriteCode = true
        }

    });

    result.Add(new Employee
    {
        Id = Guid.NewGuid().ToString(),
        AccountName = "rory.tre@avepoint.com",
        Password = "test_rory",
        Department = "DN_OFFICE_",
        FirstName = "Rory",
        LastName = "Tre",
        PhoneNumber = "221133444",
        Sex = Sex.Male,
        Type = EmployeeType.QualityAssurance,
        Extension = new QualityAssuaranceExtension
        {
            Band = Band.A1,
            CanWriteCode = false
        }

    });

    result.Add(new Employee
    {
        Id = Guid.NewGuid().ToString(),
        AccountName = "abc.xyz@avepoint.com",
        Password = "test_abc",
        Department = "DN_OFFICE_",
        FirstName = "Abc",
        LastName = "Xyz",
        PhoneNumber = "0961389719",
        Sex = Sex.Female,
        IsIntern = true,
        Type = EmployeeType.QualityAssurance,
        Extension = new QualityAssuaranceExtension
        {
            Band = Band.A1,
            CanWriteCode = true
        }

    });
    #endregion
    #region Manager
    result.Add(new Employee
    {
        Id = Guid.NewGuid().ToString(),
        AccountName = "fake.tran@avepoint.com",
        Password = "test_fake",
        Department = "DN_DAO_M1",
        FirstName = "Fake",
        LastName = "Tran",
        PhoneNumber = "554433221",
        Sex = Sex.Male,
        Type = EmployeeType.Manager,
        Extension = new ManagerExtension
        {
            ManagerType = ManagerType.DepartmentManager,
        }

    });
    result.Add(new Employee
    {
        Id = Guid.NewGuid().ToString(),
        AccountName = "lll.nguyen@avepoint.com",
        Password = "test_lll",
        Department = "DN_OFFICE_M1",
        FirstName = "Lll",
        LastName = "Nguyen",
        PhoneNumber = "11224433",
        Sex = Sex.Female,
        Type = EmployeeType.Manager,
        Extension = new ManagerExtension
        {
            ManagerType = ManagerType.DepartmentManager
        }

    });
    result.Add(new Employee
    {
        Id = Guid.NewGuid().ToString(),
        AccountName = "gg.hh@avepoint.com",
        Password = "test_gg",
        Department = "HN_OFFICE_GG",
        FirstName = "Gg",
        LastName = "hh",
        PhoneNumber = "76767562",
        Sex = Sex.Male,
        Type = EmployeeType.Manager,
        Extension = new ManagerExtension
        {
            ManagerType = ManagerType.GeneralManager
        }

    });

    #endregion
    return result;
}

static List<AttendanceRecord> PrepairAttendanceRecords(IEnumerable<String> employeeIds)
{
    var result = new List<AttendanceRecord>();
    var random = new Random();

    foreach (var employee in employeeIds)
    {
        for (var i = 1; i < 30; i++)
        {
            var date = DateTime.Now.Date;
            var targetDate = date.AddDays(-i);

            var arrvial = new DateTime(targetDate.Year, targetDate.Month, targetDate.Day, 7 + random.Next(0, 2), random.Next(0, 59), 0);
            var leave = new DateTime(targetDate.Year, targetDate.Month, targetDate.Day, 16 + random.Next(0, 2), random.Next(0, 59), 0);

            Console.WriteLine($"arrival: {arrvial}, leave: {leave}");

            result.Add(new AttendanceRecord
            {
                Id = Guid.NewGuid().ToString(),
                EmployeeId = employee,
                Date = targetDate.ToUniversalTime().Ticks,
                ArrivalTime = arrvial.ToUniversalTime().Ticks,
                LeaveTime = leave.ToUniversalTime().Ticks,
            });

        }
    }
    return result;

}