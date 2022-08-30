using Kysect.BotFramework.Abstractions.Commands;

namespace Kysect.BotFramework.Abstractions.CommandInvoking;

public interface ICommandParser
{
    ICommandContainer ParseCommand(IBotEventArgs botArguments);
}