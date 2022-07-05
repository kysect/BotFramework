using Kysect.BotFramework.Core.Commands;
using Kysect.BotFramework.Core.Exceptions;

namespace Kysect.BotFramework.Core.CommandInvoking
{
    public class CommandParser : ICommandParser
    {
        public CommandContainer ParseCommand(BotEventArgs botArguments)
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