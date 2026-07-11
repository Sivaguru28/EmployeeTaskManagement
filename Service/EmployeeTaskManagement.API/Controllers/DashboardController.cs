using EmployeeTaskManagement.API.Common;
using EmployeeTaskManagement.API.DTOs.Dashboard.Responses;
using EmployeeTaskManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeTaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpPost("GetStats")]
        public async Task<Result<DashboardStatsDto>> GetDashboardStats(CancellationToken cancellationToken)
        {
            return await _dashboardService.GetDashboardStatsAsync(cancellationToken);
        }
    }
}
