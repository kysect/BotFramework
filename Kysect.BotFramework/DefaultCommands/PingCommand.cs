using System.Threading.Tasks;
using Kysect.BotFramework.Abstractions.BotMessages;
using Kysect.BotFramework.Abstractions.Commands;
using Kysect.BotFramework.Attributes;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Tools;

namespace Kysect.BotFramework.DefaultCommands
{
    [BotCommandDescriptor("ping", "Answers pong on message.")]
    public class PingCommand : IBotCommand
    {
        public Result CanExecute(ICommandContainer args) => Result.Ok();

        public async Task<IBotMessage> Execute(ICommandContainer args)
        {
            return new BotTextMessage("Pong!");
        }
    }
}