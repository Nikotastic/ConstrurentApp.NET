namespace Firmness.Core.Common
{
    // Resultado simple que representa éxito o fracaso con un mensaje opcional y un código de error
    public class Result
    {
        public bool IsSuccess { get; }
        public string? ErrorMessage { get; }
        public string? ErrorCode { get; }

        protected Result(bool isSuccess, string? errorMessage = null, string? errorCode = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
        }

        public static Result Success() => new Result(true);
        public static Result Failure(string errorMessage) => new Result(false, errorMessage);
        public static Result Failure(string errorMessage, string errorCode) => new Result(false, errorMessage, errorCode);
    }

    // Resultado con dato genérico
    public class Result<T> : Result
    {
        public T? Value { get; }

        private Result(T value) : base(true)
        {
            Value = value;
        }

        private Result(string errorMessage, string? errorCode = null) : base(false, errorMessage, errorCode)
        {
            Value = default;
        }

        public static Result<T> Success(T value) => new Result<T>(value);
        public new static Result<T> Failure(string errorMessage) => new Result<T>(errorMessage);
        public new static Result<T> Failure(string errorMessage, string errorCode) => new Result<T>(errorMessage, errorCode);
    }
}
