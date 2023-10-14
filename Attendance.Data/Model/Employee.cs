namespace Attendance.Data.Model
{
    public class Employee
    {
        public string Id { get; set; }
        public EmployeeType Type { get; set; }
        public String AccountName { get; set; }
        public String Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Sex Sex { get; set; }
        public string Department { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsIntern { get; set; }

        public EmployeeExtension? Extension { get; set; }
    }
}
