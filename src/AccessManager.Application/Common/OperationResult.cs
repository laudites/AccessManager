namespace AccessManager.Application.Common;

public class OperationResult
{
    protected OperationResult(bool success, IReadOnlyCollection<string> errors)
    {
        Success = success;
        Errors = errors;
    }

    public bool Success { get; }
    public IReadOnlyCollection<string> Errors { get; }

    public static OperationResult Ok()
    {
        return new OperationResult(true, []);
    }

    public static OperationResult Fail(params string[] errors)
    {
        return new OperationResult(false, errors);
    }
}

public class OperationResult<T> : OperationResult
{
    private OperationResult(bool success, T? data, IReadOnlyCollection<string> errors)
        : base(success, errors)
    {
        Data = data;
    }

    public T? Data { get; }

    public static OperationResult<T> Ok(T data)
    {
        return new OperationResult<T>(true, data, []);
    }

    public static new OperationResult<T> Fail(params string[] errors)
    {
        return new OperationResult<T>(false, default, errors);
    }
}
