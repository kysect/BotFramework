using System;
using FluentResults;
using Kysect.BotFramework.Core.Tools.Extensions;

namespace Kysect.BotFramework.Core.Commands
{
    public interface IBotCommand
    {
        Result CanExecute(CommandContainer args);
    }
}