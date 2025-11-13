namespace Transferencia.Dtos.Response
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string? ErrorType { get; }
        public string? Message { get; }

        public Result(bool success, string? errorType, string? message)
        {
            IsSuccess = success;
            ErrorType = errorType;
            Message = message;
        }

        public static Result Success() => new Result(true, null, null);

        public static Result Failure(string errorType, string message) => new Result(false, errorType, message);
    }
}
