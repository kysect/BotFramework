using System.Threading.Tasks;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.Tools;

namespace Kysect.BotFramework.Core.Commands
{
    public interface IBotAsyncCommand : IBotCommand
    {
        Task<IBotMessage> Execute(CommandContainer args);
    }
}