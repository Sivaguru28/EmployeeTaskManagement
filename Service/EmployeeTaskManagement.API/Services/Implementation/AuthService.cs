using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmployeeTaskManagement.API.Common;
using EmployeeTaskManagement.API.DTOs.Auth.Requests;
using EmployeeTaskManagement.API.DTOs.Auth.Responses;
using EmployeeTaskManagement.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using EmployeeTaskManagement.API.Common.Attributes;

namespace EmployeeTaskManagement.API.Services.Implementation
{
    [ScopedService]
    public class AuthService : IAuthService
    {
        readonly IConfiguration _configuration;
        public AuthService(IConfiguration configuration) { 
            _configuration = configuration;
        }

        public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken token) 
        {
            if (request.UserName != "admin" || request.Password != "Admin@123") 
            { 
                return Result<LoginResponse>.FailureResult(System.Net.HttpStatusCode.Unauthorized, "Invalid username or password");
            }
            var JwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = JwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret is not configured.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, request.UserName),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var expiryMinutes = Convert.ToInt32(JwtSettings["ExpiryInMinutes"] ?? "60");
            var expirationTime = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expirationTime,
                Issuer = JwtSettings["Issuer"],
                Audience = JwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(securityToken);

            var responseData = new LoginResponse
            {
                Token = tokenString,
                Username = request.UserName,
                Expiration = expirationTime
            };

            return Result<LoginResponse>.SuccessResult(responseData, "Authentication successful.");
        }


    }
}
