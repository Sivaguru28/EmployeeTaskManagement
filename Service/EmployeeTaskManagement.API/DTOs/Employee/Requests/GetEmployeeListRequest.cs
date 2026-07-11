namespace EmployeeTaskManagement.API.DTOs.Employee.Requests
{

    public class GetEmployeeListRequest
    {
        public string? SearchText { get; set; }
        public string? Department { get; set; }
        public bool IsActive { get; set; } = true;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
