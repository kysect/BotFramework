namespace Kysect.BotFramework.Tools;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailed => !IsSuccess;
    public string? Message { get; }
    public Exception? Exception { get; }

    private Result() => IsSuccess = true;

    private Result(string message)
    {
        IsSuccess = false;
        Message = message;
    }

    private Result(Exception exception)
    {
        IsSuccess = false;
        Message = exception.Message;
        Exception = exception;
    }
    
    public static Result Ok() => new();

    public static Result Fail(string message) => new(message);
    public static Result Fail(Exception exception) => new(exception);
}