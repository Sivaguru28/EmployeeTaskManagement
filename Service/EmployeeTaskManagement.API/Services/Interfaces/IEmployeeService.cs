using EmployeeTaskManagement.API.Common;
using EmployeeTaskManagement.API.DTOs.Employee.Requests;
using EmployeeTaskManagement.API.DTOs.Employee.Responses;
using EmployeeTaskManagement.API.Models;

namespace EmployeeTaskManagement.API.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<Result<GetEmployeeListResponse>> GetEmployeeListAsync(GetEmployeeListRequest request, CancellationToken token);
        Task<Result<EmployeeDetailsDto>> GetEmployeeByIdAsync(int employeeId, CancellationToken token);
        Task<Result<EmployeeDetailsDto>> CreateEmployeeAsync(UpsertEmployeeRequest request, CancellationToken token);
        Task<Result<EmployeeDetailsDto>> UpdateEmployeeAsync(int employeeId, UpsertEmployeeRequest request, CancellationToken token);
        Task<Result<bool>> DeleteEmployeeAsync(int employeeId, CancellationToken token);
        Task<Result<string>> GetNextEmployeeCodeAsync(CancellationToken token);
    }
}
