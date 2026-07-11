namespace EmployeeTaskManagement.API.DTOs.Auth.Responses
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
    }       
    
}
