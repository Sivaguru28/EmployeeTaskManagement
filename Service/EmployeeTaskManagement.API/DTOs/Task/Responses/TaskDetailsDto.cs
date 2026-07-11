namespace EmployeeTaskManagement.API.DTOs.Task.Responses
{
    public class TaskDetailsDto
    {
        public int EmployeeTaskId { get; set; }
        public int EmployeeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal EstimatedHours { get; set; }
        public DateTime CreatedDate { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
    }
}
