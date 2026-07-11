using EmployeeTaskManagement.API.Common;
using EmployeeTaskManagement.API.DTOs.Task.Requests;
using EmployeeTaskManagement.API.DTOs.Task.Responses;

namespace EmployeeTaskManagement.API.Services.Interfaces
{
    public interface ITaskService
    {
        Task<Result<List<TaskDetailsDto>>> GetAllTasksAsync(string? status, int? employeeId, CancellationToken token);
        Task<Result<TaskDetailsDto>> GetTaskByIdAsync(int taskId, CancellationToken token);
        Task<Result<List<TaskDetailsDto>>> GetTasksByEmployeeAsync(int employeeId, CancellationToken token);
        Task<Result<TaskDetailsDto>> CreateTaskAsync(UpsertTaskRequest request, CancellationToken token);
        Task<Result<TaskDetailsDto>> UpdateTaskAsync(int taskId, UpsertTaskRequest request, CancellationToken token);
        Task<Result<TaskDetailsDto>> UpdateTaskStatusAsync(int taskId, UpdateTaskStatusRequest request, CancellationToken token);
        Task<Result<bool>> DeleteTaskAsync(int taskId, CancellationToken token);
    }
}
