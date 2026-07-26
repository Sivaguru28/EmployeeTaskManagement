using System.Threading;
using System.Net;
using EmployeeTaskManagement.API.Common;
using EmployeeTaskManagement.API.Common.Attributes;
using EmployeeTaskManagement.API.Data;
using EmployeeTaskManagement.API.DTOs.Employee.Requests;
using EmployeeTaskManagement.API.DTOs.Employee.Responses;
using EmployeeTaskManagement.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using EmployeeTaskManagement.API.Models;

namespace EmployeeTaskManagement.API.Services.Implementation
{
    [ScopedService]
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmployeeService> _logger;
        private readonly ITaskService _taskService;

        public EmployeeService(ApplicationDbContext context, ILogger<EmployeeService> logger,ITaskService taskService)
        {
            _context = context;
            _logger = logger;
            _taskService = taskService;
        }


        public async Task<Result<GetEmployeeListResponse>> GetEmployeeListAsync(GetEmployeeListRequest request, CancellationToken token)
        {
            var query = _context.Employees.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                string search = request.SearchText.Trim().ToLower();
                query = query.Where(e => e.FirstName.ToLower().Contains(search)
                                      || e.LastName.ToLower().Contains(search)
                                      || e.EmployeeCode.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.Department))
            {
                query = query.Where(e => e.Department == request.Department);
            }

           
                query = query.Where(e => e.IsActive == request.IsActive);
            

            int totalRecords = await query.CountAsync(token);

            int pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            int pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            var employeesList = await query
                .OrderByDescending(e => e.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EmployeeListItemDto
                {
                    EmployeeId = e.EmployeeId,
                    EmployeeCode = e.EmployeeCode,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    MobileNumber = e.MobileNumber,
                    Department = e.Department,
                    Designation = e.Designation,
                    DateOfJoining = e.DateOfJoining,
                    IsActive = e.IsActive,
                    TotalAssignedTasks = e.Tasks.Count,
                })
                .ToListAsync(token);

            var responseData = new GetEmployeeListResponse
            {
                Employees = employeesList,
                TotalRecords = totalRecords
            };

            return Result<GetEmployeeListResponse>.SuccessResult(responseData, "Employee list retrieved successfully.");
        }

        public async Task<Result<EmployeeDetailsDto>> GetEmployeeByIdAsync(int employeeId, CancellationToken token)
        {
            var employee = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.EmployeeId == employeeId, token);
            if (employee == null)
            {
                return Result<EmployeeDetailsDto>.FailureResult(HttpStatusCode.NotFound, "Employee not found.");
            }

            var dto = MapToDetailsDto(employee);
            return Result<EmployeeDetailsDto>.SuccessResult(dto, "Employee retrieved successfully.");
        }

        public async Task<Result<EmployeeDetailsDto>> CreateEmployeeAsync(UpsertEmployeeRequest request, CancellationToken token)
        {
            var emailExists = await _context.Employees.AnyAsync(e => e.Email.ToLower() == request.Email.ToLower(), token);
            if (emailExists)
            {
                _logger.LogWarning("Failed to create employee: Email {Email} already exists.", request.Email);
                return Result<EmployeeDetailsDto>.FailureResult(HttpStatusCode.BadRequest, "Email must be unique.");
            }

            var codeExists = await _context.Employees.AnyAsync(e => e.EmployeeCode.ToLower() == request.EmployeeCode.ToLower() && e.IsActive, token);
            if (codeExists)
            {
                _logger.LogWarning("Failed to create employee: Employee Code {Code} already exists for an active employee.", request.EmployeeCode);
                return Result<EmployeeDetailsDto>.FailureResult(HttpStatusCode.BadRequest, "Employee Code is already in use by an active employee.");
            }

            if (request.DateOfJoining.Date >= DateTime.UtcNow.Date)
            {
                _logger.LogWarning("Failed to create employee: Joining Date {DateOfJoining} is in the future.", request.DateOfJoining);
                return Result<EmployeeDetailsDto>.FailureResult(HttpStatusCode.BadRequest, "Joining Date cannot be a future date.");
            }

            var employee = new Employee
            {
                EmployeeCode = request.EmployeeCode,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                MobileNumber = request.MobileNumber,
                Department = request.Department,
                Designation = request.Designation,
                DateOfJoining = request.DateOfJoining,
                IsActive = request.IsActive,
                CreatedDate = DateTime.UtcNow
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync(token);

            _logger.LogInformation("Employee {EmployeeId} ({Email}) created successfully.", employee.EmployeeId, employee.Email);

            return Result<EmployeeDetailsDto>.SuccessResult(MapToDetailsDto(employee), "Employee created successfully.");
        }

        public async Task<Result<EmployeeDetailsDto>> UpdateEmployeeAsync(int employeeId, UpsertEmployeeRequest request, CancellationToken token)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == employeeId, token);
            if (employee == null)
            {
                return Result<EmployeeDetailsDto>.FailureResult(HttpStatusCode.NotFound, "Employee not found.");
            }

            var emailExists = await _context.Employees.AnyAsync(e => e.Email.ToLower() == request.Email.ToLower() && e.EmployeeId != employeeId, token);
            if (emailExists)
            {
                _logger.LogWarning("Failed to update employee {EmployeeId}: Email {Email} already exists.", employeeId, request.Email);
                return Result<EmployeeDetailsDto>.FailureResult(HttpStatusCode.BadRequest, "Email must be unique.");
            }

            var codeExists = await _context.Employees.AnyAsync(e => e.EmployeeCode.ToLower() == request.EmployeeCode.ToLower() && e.EmployeeId != employeeId && e.IsActive, token);
            if (codeExists)
            {
                _logger.LogWarning("Failed to update employee {EmployeeId}: Employee Code {Code} already exists for an active employee.", employeeId, request.EmployeeCode);
                return Result<EmployeeDetailsDto>.FailureResult(HttpStatusCode.BadRequest, "Employee Code is already in use by an active employee.");
            }

            if (request.DateOfJoining.Date > DateTime.UtcNow.Date)
            {
                _logger.LogWarning("Failed to update employee {EmployeeId}: Joining Date {DateOfJoining} is in the future.", employeeId, request.DateOfJoining);
                return Result<EmployeeDetailsDto>.FailureResult(HttpStatusCode.BadRequest, "Joining Date cannot be a future date.");
            }

            employee.EmployeeCode = request.EmployeeCode;
            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.Email = request.Email;
            employee.MobileNumber = request.MobileNumber;
            employee.Department = request.Department;
            employee.Designation = request.Designation;
            employee.DateOfJoining = request.DateOfJoining;
            employee.IsActive = request.IsActive;

            await _context.SaveChangesAsync(token);

            _logger.LogInformation("Employee {EmployeeId} ({Email}) updated successfully.", employee.EmployeeId, employee.Email);

            return Result<EmployeeDetailsDto>.SuccessResult(MapToDetailsDto(employee), "Employee updated successfully.");
        }

        public async Task<Result<bool>> DeleteEmployeeAsync(int employeeId, CancellationToken token)
        {
            var EmployeeDetail = _context.Employees.Include(e => e.Tasks).FirstOrDefault(e => e.EmployeeId == employeeId);
            
            if (EmployeeDetail == null)
            {
                return Result<bool>.FailureResult(HttpStatusCode.NotFound, "Employee not found.");
            }
            if (EmployeeDetail.Tasks.Count > 0) {
                List<EmployeeTask> TaskCollection =  EmployeeDetail.Tasks.ToList();
                foreach (var task in TaskCollection) { 
                    await _taskService.DeleteTaskAsync(task.EmployeeTaskId, token);
                }               
            }

            EmployeeDetail.IsActive = false;
            await _context.SaveChangesAsync(token);

            _logger.LogInformation("Employee {EmployeeId} soft deleted successfully.", employeeId);

            return Result<bool>.SuccessResult(true, "Employee deleted successfully.");
        }

        private static EmployeeDetailsDto MapToDetailsDto(Employee employee)
        {
            return new EmployeeDetailsDto
            {
                EmployeeId = employee.EmployeeId,
                EmployeeCode = employee.EmployeeCode,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                MobileNumber = employee.MobileNumber,
                Department = employee.Department,
                Designation = employee.Designation,
                DateOfJoining = employee.DateOfJoining,
                IsActive = employee.IsActive,
                CreatedDate = employee.CreatedDate
            };
        }

        public async Task<Result<string>> GetNextEmployeeCodeAsync(CancellationToken token)
        {
            var codes = await _context.Employees
                .AsNoTracking()
                .Select(e => e.EmployeeCode)
                .ToListAsync(token);

            int maxNum = 0;
            foreach (var code in codes)
            {
                if (code.StartsWith("EMP", System.StringComparison.OrdinalIgnoreCase))
                {
                    var numStr = code.Substring(3);
                    if (int.TryParse(numStr, out int num))
                    {
                        if (num > maxNum)
                        {
                            maxNum = num;
                        }
                    }
                }
            }

            string nextCode = $"EMP{(maxNum + 1):D3}";
            return Result<string>.SuccessResult(nextCode, "Next employee code generated successfully.");
        }
    }
}

