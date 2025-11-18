using PaymentAPI.Application.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Application.Wrappers
{
    public class ApiResponse<T> where T : class
    {
        public int Status { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }

        public ApiResponse(int statusCode, T data, string message)
        {
            Status = statusCode;
            Data = data;
            Message = message;
        }

        // Helper methods
        public static ApiResponse<T> Success(T data, string message = "Request processed successfully")
        {
            return new ApiResponse<T>(StatusCode.OK, data, message);
        }

        public static ApiResponse<T> Created(T data, string message = "Resource created successfully")
        {
            return new ApiResponse<T>(StatusCode.Created, data, message);
        }

        public static ApiResponse<T> Fail(string message)
        {
            return new ApiResponse<T>(StatusCode.BadRequest, default, message);
        }

        public static ApiResponse<T> NotFound(string message)
        {
            return new ApiResponse<T>(StatusCode.NotFound, default, message);
        }

        public static ApiResponse<T> InternalError(string message = "An unexpected error occurred")
        {
            return new ApiResponse<T>(StatusCode.InternalServerCode, default, message);
        }
    }
}
