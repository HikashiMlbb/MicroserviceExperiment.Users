namespace SharedKernel;

public record Result
{
    public Error? Error { get; init; }
    public bool IsSuccess => Error is null;

    private Result(Error? error = null)
    {
        Error = error;
    }

    public static Result Success()
    {
        return new Result();
    }

    public static Result Failure(Error error)
    {
        return new Result(error);
    }
}