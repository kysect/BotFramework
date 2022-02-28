using System;
using Kysect.BotFramework.Core.Tools;
using Kysect.BotFramework.Core.Tools.Extensions;

namespace Kysect.BotFramework.Core.Commands
{
    public interface IBotCommand
    {
        Result CanExecute(CommandContainer args);
    }
}