using Kysect.BotFramework.Abstractions.BotMessages;
using Kysect.BotFramework.Tools;

namespace Kysect.BotFramework.Abstractions.Commands;

public interface IBotCommand
{
    Result CanExecute(ICommandContainer args);
        
    Task<IBotMessage> Execute(ICommandContainer args);
}