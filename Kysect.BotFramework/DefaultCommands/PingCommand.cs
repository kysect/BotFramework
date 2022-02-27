using System.Threading.Tasks;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.Commands;
using Kysect.BotFramework.Core.Tools;

namespace Kysect.BotFramework.DefaultCommands
{
    [BotCommandDescriptor("ping", "Answers pong on message.")]
    public class PingCommand : IBotAsyncCommand
    {
        public Result CanExecute(CommandContainer args) => Result.Ok();

        public async Task<IBotMessage> Execute(CommandContainer args)
        {
            return new BotTextMessage("Pong!");
        }
    }
}