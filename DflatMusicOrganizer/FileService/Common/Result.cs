namespace FileService.Common;

public class Result<T, TError>
{
    private T? _value;
    private TError? _error;

    public bool IsSuccess { get; }
    public T Value
    {
        get => IsSuccess ? _value! : throw new InvalidOperationException("No value present.");
        private set => _value = value;
    }
    public TError Error
    {
        get => !IsSuccess ? _error! : throw new InvalidOperationException("No error present.");
        private set => _error = value;
    }

    private Result(bool isSuccess, T? value, TError? error) => (IsSuccess, _value, _error) = (isSuccess, value, error);
    public static Result<T, TError> Success(T value) => new(true, value, default);
    public static Result<T, TError> Failure(TError error) => new(false, default, error);
}
