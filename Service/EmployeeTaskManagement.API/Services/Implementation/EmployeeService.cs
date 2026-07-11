using System.Threading;
using EmployeeTaskManagement.API.Common;
using EmployeeTaskManagement.API.Common.Attributes;
using EmployeeTaskManagement.API.Data;
using EmployeeTaskManagement.API.DTOs.Employee.Requests;
using EmployeeTaskManagement.API.DTOs.Employee.Responses;
using EmployeeTaskManagement.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeTaskManagement.API.Services.Implementation
{
    [ScopedService]
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;
        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
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

            if (request.IsActive)
            {
                query = query.Where(e => e.IsActive == request.IsActive);
            }

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
    }

}

