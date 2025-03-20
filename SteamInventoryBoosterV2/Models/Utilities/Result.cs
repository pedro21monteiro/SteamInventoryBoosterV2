using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInventoryBoosterV2.Models.Utilities
{
    public class Result
    {
        public bool IsSuccess { get; }
        public Error? Error { get; }

        protected Result(bool isSuccess, Error? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new Result(true, null);

        public static Result Failure(Error error) => new Result(false, error);
    }

    public class Result<T> : Result
    {
        public T? Value { get; }

        private Result(T value) : base(true, null)
        {
            Value = value;
        }

        private Result(Error error) : base(false, error) { }

        public static Result<T> Success(T value) => new Result<T>(value);

        public static new Result<T> Failure(Error error) => new Result<T>(error);
    }
}
