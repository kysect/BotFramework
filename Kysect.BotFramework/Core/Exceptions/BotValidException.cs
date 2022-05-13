using System;

namespace Kysect.BotFramework.Core.Exceptions
{
    public class BotValidException : ArgumentException
    {
        public BotValidException() { }
        public BotValidException(string message) : base(message) { }
    }
}