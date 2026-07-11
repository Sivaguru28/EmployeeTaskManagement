using System.Net;
using EmployeeTaskManagement.API.Common;
using EmployeeTaskManagement.API.Common.Attributes;
using EmployeeTaskManagement.API.Data;
using EmployeeTaskManagement.API.DTOs.Task.Requests;
using EmployeeTaskManagement.API.DTOs.Task.Responses;
using EmployeeTaskManagement.API.Models;
using EmployeeTaskManagement.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeTaskManagement.API.Services.Implementation
{
    [ScopedService]
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TaskService> _logger;

        public TaskService(ApplicationDbContext context, ILogger<TaskService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region GetAllTasksAsync
        public async Task<Result<List<TaskDetailsDto>>> GetAllTasksAsync(string? status, int? employeeId, CancellationToken token)
        {
            var query = _context.EmployeeTasks.AsNoTracking().Include(t => t.Employee).Where(t => t.IsActive).AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(t => t.Status == status);
            }

            if (employeeId.HasValue && employeeId.Value > 0)
            {
                query = query.Where(t => t.EmployeeId == employeeId.Value);
            }

            var tasks = await query
                .OrderByDescending(t => t.CreatedDate)
                .Select(t => MapToDetailsDto(t))
                .ToListAsync(token);

            return Result<List<TaskDetailsDto>>.SuccessResult(tasks, "Tasks retrieved successfully.");
        }
        #endregion

        #region GetTaskByIdAsync

        public async Task<Result<TaskDetailsDto>> GetTaskByIdAsync(int taskId, CancellationToken token)
        {
            var task = await _context.EmployeeTasks.AsNoTracking()
                .Include(t => t.Employee)
                .FirstOrDefaultAsync(t => t.EmployeeTaskId == taskId, token);

            if (task == null)
            {
                return Result<TaskDetailsDto>.FailureResult(HttpStatusCode.NotFound, "Task not found.");
            }

            return Result<TaskDetailsDto>.SuccessResult(MapToDetailsDto(task), "Task retrieved successfully.");
        }
        #endregion

        #region GetTasksByEmployeeAsync

        public async Task<Result<List<TaskDetailsDto>>> GetTasksByEmployeeAsync(int employeeId, CancellationToken token)
        {
            var employeeExists = await _context.Employees.AnyAsync(e => e.EmployeeId == employeeId, token);
            if (!employeeExists)
            {
                return Result<List<TaskDetailsDto>>.FailureResult(HttpStatusCode.NotFound, "Employee not found.");
            }

            var tasks = await _context.EmployeeTasks.AsNoTracking()
                .Include(t => t.Employee)
                .Where(t => t.EmployeeId == employeeId)
                .OrderByDescending(t => t.CreatedDate)
                .Select(t => MapToDetailsDto(t))
                .ToListAsync(token);

            return Result<List<TaskDetailsDto>>.SuccessResult(tasks, "Tasks for employee retrieved successfully.");
        }
        #endregion

        #region CreateTaskAsync
        public async Task<Result<TaskDetailsDto>> CreateTaskAsync(UpsertTaskRequest request, CancellationToken token)
        {
            // Verify Employee exists
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == request.EmployeeId, token);
            if (employee == null)
            {
                _logger.LogWarning("Failed to create task: Employee {EmployeeId} does not exist.", request.EmployeeId);
                return Result<TaskDetailsDto>.FailureResult(HttpStatusCode.BadRequest, "Assigned Employee does not exist.");
            }

            // Verify dates
            if (request.DueDate.Date < request.StartDate.Date)
            {
                _logger.LogWarning("Failed to create task: DueDate {DueDate} is earlier than StartDate {StartDate}.", request.DueDate, request.StartDate);
                return Result<TaskDetailsDto>.FailureResult(HttpStatusCode.BadRequest, "Due Date cannot be earlier than Start Date.");
            }

            var task = new EmployeeTask
            {
                EmployeeId = request.EmployeeId,
                Title = request.Title,
                Description = request.Description,
                Priority = request.Priority,
                Status = request.Status,
                StartDate = request.StartDate,
                DueDate = request.DueDate,
                EstimatedHours = request.EstimatedHours,
                CreatedDate = DateTime.UtcNow,
                IsActive= true
            };

            _context.EmployeeTasks.Add(task);
            await _context.SaveChangesAsync(token);

            // Populate Employee navigation property for return DTO
            task.Employee = employee;

            _logger.LogInformation("Task {TaskId} created successfully and assigned to Employee {EmployeeId}.", task.EmployeeTaskId, task.EmployeeId);

            return Result<TaskDetailsDto>.SuccessResult(MapToDetailsDto(task), "Task created successfully.");
        }
        #endregion

        #region UpdateTaskAsync

        public async Task<Result<TaskDetailsDto>> UpdateTaskAsync(int taskId, UpsertTaskRequest request, CancellationToken token)
        {
            var task = await _context.EmployeeTasks.Include(t => t.Employee).FirstOrDefaultAsync(t => t.EmployeeTaskId == taskId, token);
            if (task == null)
            {
                return Result<TaskDetailsDto>.FailureResult(HttpStatusCode.NotFound, "Task not found.");
            }

            // Verify Employee exists
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == request.EmployeeId, token);
            if (employee == null)
            {
                _logger.LogWarning("Failed to update task {TaskId}: Employee {EmployeeId} does not exist.", taskId, request.EmployeeId);
                return Result<TaskDetailsDto>.FailureResult(HttpStatusCode.BadRequest, "Assigned Employee does not exist.");
            }

            // Verify dates
            if (request.DueDate.Date < request.StartDate.Date)
            {
                _logger.LogWarning("Failed to update task {TaskId}: DueDate {DueDate} is earlier than StartDate {StartDate}.", taskId, request.DueDate, request.StartDate);
                return Result<TaskDetailsDto>.FailureResult(HttpStatusCode.BadRequest, "Due Date cannot be earlier than Start Date.");
            }

            // Verify status constraints: A Completed task cannot be changed back to Pending
            if (task.Status == "Completed" && request.Status == "Pending")
            {
                _logger.LogWarning("Failed to update task {TaskId}: Cannot change a Completed task back to Pending.", taskId);
                return Result<TaskDetailsDto>.FailureResult(HttpStatusCode.BadRequest, "A Completed task cannot be changed back to Pending.");
            }

            task.EmployeeId = request.EmployeeId;
            task.Title = request.Title;
            task.Description = request.Description;
            task.Priority = request.Priority;
            task.Status = request.Status;
            task.StartDate = request.StartDate;
            task.DueDate = request.DueDate;
            task.EstimatedHours = request.EstimatedHours;

            // Re-assign employee navigation for returning mapping
            task.Employee = employee;

            await _context.SaveChangesAsync(token);

            _logger.LogInformation("Task {TaskId} updated successfully.", taskId);

            return Result<TaskDetailsDto>.SuccessResult(MapToDetailsDto(task), "Task updated successfully.");
        }

        #endregion

        #region UpdateTaskStatusAsync
        public async Task<Result<TaskDetailsDto>> UpdateTaskStatusAsync(int taskId, UpdateTaskStatusRequest request, CancellationToken token)
        {
            var task = await _context.EmployeeTasks.Include(t => t.Employee).FirstOrDefaultAsync(t => t.EmployeeTaskId == taskId, token);
            if (task == null)
            {
                return Result<TaskDetailsDto>.FailureResult(HttpStatusCode.NotFound, "Task not found.");
            }

            // Verify status constraints: A Completed task cannot be changed back to Pending
            if (task.Status == "Completed" && request.Status == "Pending")
            {
                _logger.LogWarning("Failed to patch status for task {TaskId}: Cannot change a Completed task back to Pending.", taskId);
                return Result<TaskDetailsDto>.FailureResult(HttpStatusCode.BadRequest, "A Completed task cannot be changed back to Pending.");
            }

            task.Status = request.Status;
            await _context.SaveChangesAsync(token);

            _logger.LogInformation("Task {TaskId} status updated to {Status} successfully.", taskId, request.Status);

            return Result<TaskDetailsDto>.SuccessResult(MapToDetailsDto(task), "Task status updated successfully.");
        }
        #endregion

        #region DeleteTaskAsync
        public async Task<Result<bool>> DeleteTaskAsync(int taskId, CancellationToken token)
        {
            var task = await _context.EmployeeTasks.FirstOrDefaultAsync(t => t.EmployeeTaskId == taskId, token);
            if (task == null)
            {
                return Result<bool>.FailureResult(HttpStatusCode.NotFound, "Task not found.");
            }

            _context.EmployeeTasks.Remove(task);
            await _context.SaveChangesAsync(token);

            _logger.LogInformation("Task {TaskId} deleted successfully.", taskId);

            return Result<bool>.SuccessResult(true, "Task deleted successfully.");
        }
        #endregion

        #region Private - Mapping Methods
        private static TaskDetailsDto MapToDetailsDto(EmployeeTask task)
        {
            return new TaskDetailsDto
            {
                EmployeeTaskId = task.EmployeeTaskId,
                EmployeeId = task.EmployeeId,
                Title = task.Title,
                Description = task.Description,
                Priority = task.Priority,
                Status = task.Status,
                StartDate = task.StartDate,
                DueDate = task.DueDate,
                EstimatedHours = task.EstimatedHours,
                CreatedDate = task.CreatedDate,
                EmployeeName = task.Employee != null ? $"{task.Employee.FirstName} {task.Employee.LastName}" : string.Empty
            };
        }
        #endregion
    }
}
