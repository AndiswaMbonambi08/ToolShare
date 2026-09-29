namespace ToolShare.Domain.Exceptions;

public class ToolShareException : Exception
{
        public abstract int StatusCode { get; }
        public abstract string Title { get; }

        protected ToolShareException(string message) : base(message) { }
}

public class RequestValidationException : ToolShareException
{
    public override int StatusCode => 400;
    public override string Title => "bad request";

    public RequestValidationException(string message) : base(message) { }
}
    
public class MemberNotFoundException : ToolShareException
{
    public override int StatusCode => 404;
    public override string Title => "Member Not Found";

    public MemberNotFoundException(string message) : base(message) { }
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