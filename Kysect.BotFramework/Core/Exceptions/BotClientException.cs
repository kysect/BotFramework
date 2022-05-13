using System;

namespace Kysect.BotFramework.Core.Exceptions;

public class BotClientException : BotException
{
    public BotClientException() { }
    public BotClientException(string message) : base(message) { }
    public BotClientException(string message, Exception innerException) : base(message, innerException) { }
}