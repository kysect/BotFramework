using System;
using System.Threading.Tasks;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.Tools;
using Kysect.BotFramework.Core.Tools.Extensions;

namespace Kysect.BotFramework.Core.Commands
{
    public interface IBotCommand
    {
        Result CanExecute(CommandContainer args);
        
        Task<IBotMessage> Execute(CommandContainer args);
    }
}