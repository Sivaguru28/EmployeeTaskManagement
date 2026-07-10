using System.Net;

namespace EmployeeTaskManagement.API.Common
{
    public class Result<T>
    {
        public bool Success { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }

        public List<string> Errors { get; set; } = new();

        public static Result<T> SuccessResult(T data, string message = "Success")
        {
            return new Result<T>
            {
                Success = true,
                StatusCode = HttpStatusCode.OK,
                Message = message,
                Data = data
            };
        }

        public static Result<T> FailureResult(HttpStatusCode statusCode, string message, List<string>? errors = null)
        {
            return new Result<T>
            {
                Success = false,
                StatusCode = statusCode,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
}
