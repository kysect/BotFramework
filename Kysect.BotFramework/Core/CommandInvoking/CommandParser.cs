using Kysect.BotFramework.Abstractions;
using Kysect.BotFramework.Abstractions.CommandInvoking;
using Kysect.BotFramework.Abstractions.Commands;
using Kysect.BotFramework.Core.Commands;
using Kysect.BotFramework.Core.Exceptions;

namespace Kysect.BotFramework.Core.CommandInvoking
{
    public class CommandParser : ICommandParser
    {
        public ICommandContainer ParseCommand(IBotEventArgs botArguments)
        {
            string commandName = botArguments.FindCommandName();

            if (string.IsNullOrWhiteSpace(commandName))
            {
                throw new CommandNotFoundException(
                    $"[{nameof(CommandParser)}]: Message do not contains command name.");
            }

            return new CommandContainer(
                commandName,
                botArguments.GetCommandArguments(),
                botArguments.GetMediaFiles(),
                botArguments.SenderInfo);
        }
    }
}