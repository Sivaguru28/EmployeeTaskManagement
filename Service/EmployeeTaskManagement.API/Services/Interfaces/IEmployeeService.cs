using EmployeeTaskManagement.API.Common;
using EmployeeTaskManagement.API.DTOs.Employee.Requests;
using EmployeeTaskManagement.API.DTOs.Employee.Responses;
using EmployeeTaskManagement.API.Models;

namespace EmployeeTaskManagement.API.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<Result<GetEmployeeListResponse>> GetEmployeeListAsync(GetEmployeeListRequest request,CancellationToken token);
    }
}
