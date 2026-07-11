using System.ComponentModel.DataAnnotations;

namespace EmployeeTaskManagement.API.DTOs.Task.Requests
{
    public class UpdateTaskStatusRequest
    {
        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("^(Pending|In Progress|Completed)$", ErrorMessage = "Status must be Pending, In Progress, or Completed.")]
        public string Status { get; set; } = string.Empty;
    }
}
