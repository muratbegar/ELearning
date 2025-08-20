using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopLearning.Shared.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string Error { get; }
        public int? ErrorCode { get; }

        public IEnumerable<string> Errors { get; }
        public DateTime Timestamp { get; }


        protected Result(bool isSuccess, string error = null, int? errorCode = null, IEnumerable<string> errors = null)
        {
            if (isSuccess && (!string.IsNullOrEmpty(error) || errors?.Any() == true))
                throw new InvalidOperationException("Successful result cannot have errors");

            if (!isSuccess && string.IsNullOrEmpty(error) && errors?.Any() != true)
                throw new InvalidOperationException("Failed result must have errors");

            IsSuccess = isSuccess;
            Error = error;
            ErrorCode = errorCode;
            Errors = errors ?? (string.IsNullOrEmpty(error) ? Array.Empty<string>() : new[] { error });
            Timestamp = DateTime.UtcNow;
        }

        // Factory methods
        public static Result Success() => new Result(true);
        public static Result Failure(string error, int? errorCode = null) => new Result(false, error, errorCode);

        public static Result Failure(IEnumerable<string> errors, int? errorCode = null) =>
            new Result(false, null, errorCode, errors);

        // Generic versions
        public static Result<T> Success<T>(T value) => Result<T>.Success(value);
        public static Result<T> Failure<T>(string error, int? errorCode = null) => Result<T>.Failure(error, errorCode);

        public static Result<T> Failure<T>(IEnumerable<string> errors, int? errorCode = null) =>
            Result<T>.Failure(errors, errorCode);

        // Combine multiple results
        public static Result Combine(params Result[] results)
        {
            var failures = results.Where(r => r.IsFailure).ToArray();
            if (failures.Any())
            {
                var allErrors = failures.SelectMany(f => f.Errors);
                return Failure(allErrors);
            }

            return Success();
        }

        // Pattern matching support
        public Result OnSuccess(Action action)
        {
            if (IsSuccess) action();
            return this;
        }

        public Result OnFailure(Action<string> action)
        {
            if (IsFailure) action(Error);
            return this;
        }

        public Result<TResult> Bind<TResult>(Func<Result<TResult>> func)
        {
            return IsSuccess ? func() : Result<TResult>.Failure(Error, ErrorCode);
        }
    }

    public class Result<T> : Result
    {
        public T Value { get; }

        private Result(T value, bool isSuccess, string error = null, int? errorCode = null,
            IEnumerable<string> errors = null)
            : base(isSuccess, error, errorCode, errors)
        {
            Value = value;
        }

        public static Result<T> Success(T value) => new Result<T>(value, true);

        public static new Result<T> Failure(string error, int? errorCode = null) =>
            new Result<T>(default, false, error, errorCode);

        public static new Result<T> Failure(IEnumerable<string> errors, int? errorCode = null) =>
            new Result<T>(default, false, null, errorCode, errors);

        // Implicit conversions
        public static implicit operator Result<T>(T value) => Success(value);

        public static implicit operator T(Result<T> result) => result.IsSuccess
            ? result.Value
            : throw new InvalidOperationException("Cannot get value from failed result");

        // Functional methods
        public Result<TResult> Map<TResult>(Func<T, TResult> mapper)
        {
            return IsSuccess ? Result.Success(mapper(Value)) : Result.Failure<TResult>(Error, ErrorCode);
        }

        public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> binder)
        {
            return IsSuccess ? binder(Value) : Result.Failure<TResult>(Error, ErrorCode);
        }

        public T ValueOr(T defaultValue) => IsSuccess ? Value : defaultValue;
        public T ValueOr(Func<T> defaultValueProvider) => IsSuccess ? Value : defaultValueProvider();
    }
}