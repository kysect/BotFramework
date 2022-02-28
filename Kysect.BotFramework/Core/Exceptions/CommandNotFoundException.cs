using System;

namespace Kysect.BotFramework.Core.Exceptions;

public class CommandNotFoundException : BotException
{
    public CommandNotFoundException() : base() { }
    public CommandNotFoundException(string message) : base(message) { }

    public CommandNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    { }
}