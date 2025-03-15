using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInventoryBoosterV2.Models.Utilities
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public Error? Error { get; }

        private Result(T value)
        {
            IsSuccess = true;
            Value = value;
            Error = null;
        }

        public Result(Error error)
        {
            IsSuccess = false;
            Error = error;
        }

        public static Result<T> Success(T value) => new Result<T>(value);

        public static Result<T> Failure(Error error) => new Result<T>(error);
    }
}
