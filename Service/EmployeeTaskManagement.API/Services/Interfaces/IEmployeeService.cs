using EmployeeTaskManagement.API.Common;
using EmployeeTaskManagement.API.Models;

namespace EmployeeTaskManagement.API.Services.Interfaces
{
    public class IEmployeeService
    {
        Task<Result<Emplo>> GetAllEmployee(CancellationToken token);
    }
}
