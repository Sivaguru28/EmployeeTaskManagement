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
    }
}
