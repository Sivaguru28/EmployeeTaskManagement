namespace EmployeeTaskManagement.API.DTOs.Employee.Responses
{

    public class GetEmployeeListResponse
    {
        public List<EmployeeListItemDto> Employees { get; set; } = new();
        public int TotalRecords { get; set; }
    }

    public class EmployeeListItemDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public DateTime DateOfJoining { get; set; }
        public bool IsActive { get; set; }
        public int TotalAssignedTasks { get; set; } // This directly addresses SQL Query 1 requirements!
    }
}

