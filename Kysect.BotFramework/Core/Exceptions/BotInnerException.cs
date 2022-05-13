using System;

namespace Kysect.BotFramework.Core.Exceptions;

public class BotInnerException : BotException
{
    public BotInnerException() { }
    public BotInnerException(string message) : base(message) { }
    public BotInnerException(string message, Exception innerException) : base(message, innerException) { }
    
}