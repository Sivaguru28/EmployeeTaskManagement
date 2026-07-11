using EmployeeTaskManagement.API.Common;
using EmployeeTaskManagement.API.DTOs.Auth.Requests;
using EmployeeTaskManagement.API.DTOs.Auth.Responses;

namespace EmployeeTaskManagement.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Result<LoginResponse>> LoginAsync(LoginRequest login, CancellationToken token);
    }
}
