using EmployeeTaskManagement.API.Common;
using EmployeeTaskManagement.API.DTOs.Task.Requests;
using EmployeeTaskManagement.API.DTOs.Task.Responses;
using EmployeeTaskManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeTaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost("List")]
        public async Task<Result<List<TaskDetailsDto>>> GetAllTasks([FromBody] GetTasksRequest request, CancellationToken cancellationToken)
        {
            return await _taskService.GetAllTasksAsync(request.Status, request.EmployeeId, cancellationToken);
        }

        [HttpPost("Get")]
        public async Task<Result<TaskDetailsDto>> GetTaskById([FromBody] TaskIdRequest request, CancellationToken cancellationToken)
        {
            return await _taskService.GetTaskByIdAsync(request.Id, cancellationToken);
        }

        [HttpPost("GetByEmployee")]
        public async Task<Result<List<TaskDetailsDto>>> GetTasksByEmployee([FromBody] GetTasksByEmployeeRequest request, CancellationToken cancellationToken)
        {
            return await _taskService.GetTasksByEmployeeAsync(request.EmployeeId, cancellationToken);
        }

        [HttpPost("Create")]
        public async Task<Result<TaskDetailsDto>> CreateTask([FromBody] UpsertTaskRequest request, CancellationToken cancellationToken)
        {
            return await _taskService.CreateTaskAsync(request, cancellationToken);
        }

        [HttpPost("Update")]
        public async Task<Result<TaskDetailsDto>> UpdateTask([FromBody] UpdateTaskBodyRequest request, CancellationToken cancellationToken)
        {
            return await _taskService.UpdateTaskAsync(request.Id, request, cancellationToken);
        }

        [HttpPost("UpdateStatus")]
        public async Task<Result<TaskDetailsDto>> UpdateTaskStatus([FromBody] UpdateTaskStatusBodyRequest request, CancellationToken cancellationToken)
        {
            var statusDto = new UpdateTaskStatusRequest { Status = request.Status };
            return await _taskService.UpdateTaskStatusAsync(request.Id, statusDto, cancellationToken);
        }

        [HttpPost("Delete")]
        public async Task<Result<bool>> DeleteTask([FromBody] TaskIdRequest request, CancellationToken cancellationToken)
        {
            return await _taskService.DeleteTaskAsync(request.Id, cancellationToken);
        }
    }

    public class GetTasksRequest
    {
        public string? Status { get; set; }
        public int? EmployeeId { get; set; }
    }

    public class TaskIdRequest
    {
        public int Id { get; set; }
    }

    public class GetTasksByEmployeeRequest
    {
        public int EmployeeId { get; set; }
    }

    public class UpdateTaskBodyRequest : UpsertTaskRequest
    {
        public int Id { get; set; }
    }

    public class UpdateTaskStatusBodyRequest
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
