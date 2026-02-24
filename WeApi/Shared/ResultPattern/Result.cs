namespace Shared.ResultPattern;

public class Result<T>
{
    private readonly T? _value;

    private Result(T value)
    {
        Value = value;
        IsSuccess = true;
        Error = null;
    }

    private Result(Error? error = null)
    {
        IsSuccess = false;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public T Value
    {
        get { return _value!; }
        private init => _value = value;
    }

    public Error? Error { get; }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Error error) => new(error);

    public static implicit operator Result<T>(T success) => Success(success);
    public static implicit operator Result<T>(Error error) => Failure(error);
}
