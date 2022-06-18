using Kysect.BotFramework.Core.Commands;

namespace Kysect.BotFramework.Core.CommandInvoking
{
    public interface ICommandParser
    {
        CommandContainer ParseCommand(BotNewMessageEventArgs botArguments);
    }
}