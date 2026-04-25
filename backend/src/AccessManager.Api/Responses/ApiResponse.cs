namespace AccessManager.Api.Responses;

public class ApiResponse
{
    public bool Success { get; set; }
    public IReadOnlyCollection<string> Errors { get; set; } = [];

    public static ApiResponse Ok()
    {
        return new ApiResponse
        {
            Success = true
        };
    }

    public static ApiResponse Fail(IReadOnlyCollection<string> errors)
    {
        return new ApiResponse
        {
            Success = false,
            Errors = errors
        };
    }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public IReadOnlyCollection<string> Errors { get; set; } = [];

    public static ApiResponse<T> Ok(T data)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data
        };
    }

    public static ApiResponse<T> Fail(IReadOnlyCollection<string> errors)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Errors = errors
        };
    }
}
