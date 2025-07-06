namespace SharedKernel;

public record Result
{
    public Error? Error { get; init; }
    public bool IsSuccess => Error is null;

    private Result(Error? error = null)
    {
        Error = error;
    }

    public static Result Success() => new();
    public static Result Failure(Error error) => new(error);

    public static implicit operator Result(Error error)
        => Failure(error);
}