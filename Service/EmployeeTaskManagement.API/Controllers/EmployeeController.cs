using System.Net;
using EmployeeTaskManagement.API.Common;
using EmployeeTaskManagement.API.Common.Attributes;
using EmployeeTaskManagement.API.DTOs.Employee.Requests;
using EmployeeTaskManagement.API.DTOs.Employee.Responses;
using EmployeeTaskManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeTaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// Retrieves a paginated and filterable list of employees.
        /// Pattern: Enterprise POST-only for query operations.
        /// </summary>
        [HttpPost("GetEmployeeList")]
        public async Task<Result<GetEmployeeListResponse>> GetEmployeeList([FromBody] GetEmployeeListRequest request, CancellationToken cancellationToken)
        {
            return await _employeeService.GetEmployeeListAsync(request, cancellationToken);
        }

        [HttpPost("Get")]
        public async Task<Result<EmployeeDetailsDto>> GetEmployeeById([FromBody] IdRequest request, CancellationToken cancellationToken)
        {
            return await _employeeService.GetEmployeeByIdAsync(request.Id, cancellationToken);
        }

        [HttpPost("Create")]
        public async Task<Result<EmployeeDetailsDto>> CreateEmployee([FromBody] UpsertEmployeeRequest request, CancellationToken cancellationToken)
        {
            return await _employeeService.CreateEmployeeAsync(request, cancellationToken);
        }

        [HttpPost("Update")]
        public async Task<Result<EmployeeDetailsDto>> UpdateEmployee([FromBody] UpdateEmployeeRequest request, CancellationToken cancellationToken)
        {
            return await _employeeService.UpdateEmployeeAsync(request.EmployeeId, request, cancellationToken);
        }

        [HttpPost("Delete")]
        public async Task<Result<bool>> DeleteEmployee([FromBody] IdRequest request, CancellationToken cancellationToken)
        {
            return await _employeeService.DeleteEmployeeAsync(request.Id, cancellationToken);
        }

        [HttpPost("NextCode")]
        public async Task<Result<string>> GetNextEmployeeCode(CancellationToken cancellationToken)
        {
            return await _employeeService.GetNextEmployeeCodeAsync(cancellationToken);
        }
    }

    public class IdRequest
    {
        public int Id { get; set; }
    }

    public class UpdateEmployeeRequest : UpsertEmployeeRequest
    {
        public int EmployeeId { get; set; }
    }
}
