using System;

namespace Kysect.BotFramework.Core.Exceptions;

public class CommandArgumentsException : BotException
{
    public CommandArgumentsException() : base() { }
    public CommandArgumentsException(string message) : base(message) { }
    public CommandArgumentsException(string message, Exception innerException) 
        : base(message, innerException)
    { }
}