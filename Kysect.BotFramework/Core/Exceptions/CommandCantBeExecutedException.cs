using System;

namespace Kysect.BotFramework.Core.Exceptions;

public class CommandCantBeExecutedException : BotInnerException
{
    public CommandCantBeExecutedException() : base() { }
    public CommandCantBeExecutedException(string message) : base(message) { }
    public CommandCantBeExecutedException(string message, Exception innerException)
        : base(message, innerException)
    { }
}