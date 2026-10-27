namespace ToolShare.Domain.Exceptions;

public abstract class ToolShareException : Exception
{
    public abstract int StatusCode { get; }
    public abstract string Title { get; }

    protected ToolShareException(string message) : base(message) { }
}

public class RequestValidationException : ToolShareException
{
    public override int StatusCode => StatusCodes.Status400BadRequest;
    public override string Title => "Bad Request";

    // Field name -> messages. Null when the failure is not tied to specific fields.
    public IReadOnlyDictionary<string, string[]>? Errors { get; }

    public RequestValidationException(string message) : base(message) { }

    public RequestValidationException(string message, IReadOnlyDictionary<string, string[]> errors)
        : base(message)
    {
        Errors = errors;
    }
}

public class NotFoundException : ToolShareException
{
    public override int StatusCode => StatusCodes.Status404NotFound;
    public override string Title => "Not Found";

    public NotFoundException(string message) : base(message) { }
}

public class ConflictException : ToolShareException
{
    public override int StatusCode => StatusCodes.Status409Conflict;
    public override string Title => "Conflict";

    public ConflictException(string message) : base(message) { }
}

public class IdempotencyConflictException : ToolShareException
{
    public override int StatusCode => StatusCodes.Status422UnprocessableEntity;
    public override string Title => "Unprocessable Entity";

    public IdempotencyConflictException(string message) : base(message) { }
}