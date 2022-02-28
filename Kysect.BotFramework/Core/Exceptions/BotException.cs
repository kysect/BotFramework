using System;

namespace Kysect.BotFramework.Core.Exceptions;

public class BotException : Exception
{
    public BotException() : base() { }
    public BotException(string message) : base(message) { }
    
    public BotException(string message, Exception innerException)
        : base(message, innerException)
    { }
}