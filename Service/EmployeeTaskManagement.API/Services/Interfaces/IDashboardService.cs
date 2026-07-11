using EmployeeTaskManagement.API.Common;
using EmployeeTaskManagement.API.DTOs.Dashboard.Responses;

namespace EmployeeTaskManagement.API.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<Result<DashboardStatsDto>> GetDashboardStatsAsync(CancellationToken token);
    }
}
