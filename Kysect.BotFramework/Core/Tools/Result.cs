using System;

namespace Kysect.BotFramework.Core.Tools;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailed => !IsSuccess;
    public string Message { get; }
    public Exception Exception { get; }
    
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

    private Result()
    {
        IsSuccess = true;
    }

    public static Result Ok() => new Result();

    public static Result Fail(string message) => new Result(message);
    public static Result Fail(Exception exception) => new Result(exception);
}