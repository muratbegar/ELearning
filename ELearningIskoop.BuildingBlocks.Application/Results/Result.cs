using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ELearningIskoop.BuildingBlocks.Application.Results
{
    // Result pattern implementation - başarı/başarısızlık durumlarını encapsulate eder
    // Başarılı ve başarısız sonuçları temsil eden temel sınıf
    public class Result
    {
        protected Result(bool isSuccess, string? error)
        {
            if (isSuccess && !string.IsNullOrWhiteSpace(error))
                throw new InvalidOperationException("Başarılı result hata mesajı içeremez");

            if (!isSuccess && string.IsNullOrWhiteSpace(error))
                throw new InvalidOperationException("Başarısız result hata mesajı içermelidir");

            IsSuccess = isSuccess;
            Error = error;
        }
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string? Error { get; }
        public static Result Success() => new(true, null);
        public static Result Failure(string error) => new(false, error);

        public static Result<T> Success<T>(T value) => new(value, true, null);
        public static Result<T> Failure<T>(string error) => new(default, false, error);
    }


    /// Generic result pattern - değer dönen operasyonlar için

    public class Result<T> : Result
    {
        private readonly T? _value;

        protected internal Result(T? value, bool isSuccess, string? error) : base(isSuccess, error)
        {
            _value = value;
        }

        public T Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("Başarısız result'tan değer alınamaz");

        public static implicit operator Result<T>(T value) => Success(value);
    }
}
