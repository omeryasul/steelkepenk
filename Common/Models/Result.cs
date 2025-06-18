using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models
{
    public class Result
    {
        protected Result(bool succeeded, IEnumerable<string> errors)
        {
            Succeeded = succeeded;
            Errors = errors.ToArray();
        }

        public bool Succeeded { get; init; }
        public bool Failed => !Succeeded;

        // Failed method'ları da ekleyelim geriye uyumluluk için
        public static Result Failedd(string error)
        {
            return new Result(false, new[] { error });
        }

        public static Result Failedd(IEnumerable<string> errors)
        {
            return new Result(false, errors);
        }
        public string[] Errors { get; init; }

        public static Result Success()
        {
            return new Result(true, Array.Empty<string>());
        }

        public static Result Success(string message)
        {
            return new Result(true, Array.Empty<string>());
        }

        public static Result Failure(IEnumerable<string> errors)
        {
            return new Result(false, errors);
        }

        public static Result Failure(string error)
        {
            return new Result(false, new[] { error });
        }
    }

    public class Result<T> : Result
    {
        protected internal Result(bool succeeded, T data, IEnumerable<string> errors)
            : base(succeeded, errors)
        {
            Data = data;
        }

        public T Data { get; init; }

        // Failed method'ları da ekleyelim geriye uyumluluk için
        public static Result<T> Failed(string error)
        {
            return new Result<T>(false, default!, new[] { error });
        }

        public static Result<T> Failed(IEnumerable<string> errors)
        {
            return new Result<T>(false, default!, errors);
        }

        public static Result<T> Success(T data)
        {
            return new Result<T>(true, data, Array.Empty<string>());
        }

        public static Result<T> Success(T data, string message)
        {
            return new Result<T>(true, data, Array.Empty<string>());
        }

        public static new Result<T> Failure(IEnumerable<string> errors)
        {
            return new Result<T>(false, default!, errors);
        }

        public static new Result<T> Failure(string error)
        {
            return new Result<T>(false, default!, new[] { error });
        }

    }
}