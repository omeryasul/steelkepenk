using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public string[]? Errors { get; set; }

        public static ApiResponse<T> SuccessResult(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        public static ApiResponse<T> ErrorResult(string message, string[]? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }

        public static ApiResponse<T> ErrorResult(string[] errors)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = "Validation errors occurred",
                Errors = errors
            };
        }
    }

    public class ApiResponse : ApiResponse<object>
    {
        public static ApiResponse SuccessResult(string? message = null)
        {
            return new ApiResponse
            {
                Success = true,
                Message = message
            };
        }

        public static new ApiResponse ErrorResult(string message, string[]? errors = null)
        {
            return new ApiResponse
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }
    }
}
