﻿using System.Threading.Tasks;
using FluentResults;
using Tef.BotFramework.Core;

namespace Tef.BotFramework.Abstractions
{
    public interface IBotCommand
    {
        string CommandName { get; }

        string Description { get; }

        string[] Args { get; }

        Result CanExecute(CommandArgumentContainer args);

        Task<Result<string>> ExecuteAsync(CommandArgumentContainer args);
    }
}