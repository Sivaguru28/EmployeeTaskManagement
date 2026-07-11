using EmployeeTaskManagement.API.Common;
using EmployeeTaskManagement.API.Common.Attributes;
using EmployeeTaskManagement.API.DTOs.Auth.Requests;
using EmployeeTaskManagement.API.DTOs.Auth.Responses;
using EmployeeTaskManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeTaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Authenticates the user and generates a signed JWT token.
        /// Pattern: Enterprise POST-only login endpoint.
        /// </summary>
        /// 

        [HttpPost("Login")] 
        public async Task<Result<LoginResponse>> Login([FromBody] LoginRequest request,CancellationToken token)
        {
            return await _authService.LoginAsync(request,token);
        }


    }

}
