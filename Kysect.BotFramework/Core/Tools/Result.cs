namespace Kysect.BotFramework.Core.Tools;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailed => !IsSuccess;
    public string Message { get; }
    
    private Result(string message)
    {
        IsSuccess = false;
        Message = message;
    }

    private Result()
    {
        IsSuccess = true;
    }

    public static Result Ok() => new Result();

    public static Result Fail(string message) => new Result(message);
}