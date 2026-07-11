using EmployeeTaskManagement.API.Common;
using EmployeeTaskManagement.API.Common.Attributes;
using EmployeeTaskManagement.API.Data;
using EmployeeTaskManagement.API.DTOs.Dashboard.Responses;
using EmployeeTaskManagement.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeTaskManagement.API.Services.Implementation
{
    [ScopedService]
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<DashboardStatsDto>> GetDashboardStatsAsync(CancellationToken token)
        {
            // Call the database Stored Procedure 'GetDashboardStats' using EF Core 8 SqlQueryRaw
            var results = await _context.Database
                .SqlQueryRaw<DashboardStatsDto>("EXEC dbo.GetDashboardStats")
                .ToListAsync(token);

            var stats = results.FirstOrDefault();

            if (stats == null)
            {
                // Fallback default in case of empty SP result
                stats = new DashboardStatsDto();
            }

            return Result<DashboardStatsDto>.SuccessResult(stats, "Dashboard statistics retrieved successfully.");
        }
    }
}
