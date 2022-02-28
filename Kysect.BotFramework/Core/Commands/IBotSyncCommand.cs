using Kysect.BotFramework.Core.BotMessages;

namespace Kysect.BotFramework.Core.Commands
{
    public interface IBotSyncCommand : IBotCommand
    {
        IBotMessage Execute(CommandContainer args);
    }
}