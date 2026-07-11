using System.ComponentModel.DataAnnotations;

namespace EmployeeTaskManagement.API.DTOs.Task.Requests
{
    public class UpsertTaskRequest
    {
        [Required(ErrorMessage = "EmployeeId is required.")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Priority is required.")]
        [RegularExpression("^(Low|Medium|High)$", ErrorMessage = "Priority must be Low, Medium, or High.")]
        public string Priority { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("^(Pending|In Progress|Completed)$", ErrorMessage = "Status must be Pending, In Progress, or Completed.")]
        public string Status { get; set; } = string.Empty;

        [Required(ErrorMessage = "StartDate is required.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "DueDate is required.")]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage = "Estimated Hours is required.")]
        [Range(0, 1000, ErrorMessage = "Estimated Hours must be between 0 and 1000.")]
        public decimal EstimatedHours { get; set; }
    }
}
