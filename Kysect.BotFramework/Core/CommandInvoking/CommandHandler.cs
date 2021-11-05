using System;
using FluentResults;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.Commands;
using Kysect.BotFramework.Core.Tools.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.BotFramework.Core.CommandInvoking
{
    public class CommandHandler
    {
        private readonly ServiceProvider _serviceProvider;
        //TODO: move to some kind of config/settings
        private bool _caseSensitive;

        public CommandHandler(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Result CheckArgsCount(CommandContainer args)
        {
            Result<BotCommandDescriptorAttribute> commandTask = _serviceProvider.GetCommandDescriptor(args.CommandName, _caseSensitive);
            if (commandTask.IsFailed)
            {
                return commandTask.ToResult<CommandContainer>();
            }

            return commandTask.Value.Args.Length == args.Arguments.Count
                ? Result.Ok()
                : Result.Fail<CommandContainer>(
                    "Cannot execute command. Argument count miss matched with command signature");
        }


        public Result CanCommandBeExecuted(CommandContainer args)
        {
            Result<IBotCommand> commandTask = _serviceProvider.GetCommand(args.CommandName, _caseSensitive);
            
            if (commandTask.IsFailed)
            {
                return commandTask.ToResult<CommandContainer>();
            }

            IBotCommand command = commandTask.Value;
            var descriptor = command.GetType().GetBotCommandDescriptorAttribute();
            Result canExecute = command.CanExecute(args);

            return canExecute.IsSuccess
                ? Result.Ok()
                : Result.Fail<CommandContainer>(
                    $"Command [{descriptor.CommandName}] cannot be executed: {canExecute}");
        }

        public CommandHandler SetCaseSensitive(bool caseSensitive)
        {
            _caseSensitive = caseSensitive;
            return this;
        }

        public Result<IBotMessage> ExecuteCommand(CommandContainer args)
        {
            Result<IBotCommand> commandTask = _serviceProvider.GetCommand(args.CommandName, _caseSensitive);

            if (!commandTask.IsSuccess)
            {
                return commandTask.ToResult<IBotMessage>();
            }

            var command = commandTask.Value;
            
            try
            {
                return command switch
                {
                    IBotAsyncCommand asyncCommand => asyncCommand.Execute(args).Result,
                    IBotSyncCommand syncCommand => syncCommand.Execute(args),
                    _ => Result.Fail(new Error("Command execution failed. Wrong command inheritance."))
                };
            }
            catch (Exception e)
            {
                var errorMessage =
                    $"Command execution failed. Command: {args.CommandName}; arguments: {string.Join(", ", args.Arguments)}";
                return Result.Fail(new Error(errorMessage).CausedBy(e));
            }
        }
    }
}