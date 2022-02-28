using System;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.Commands;
using Kysect.BotFramework.Core.Tools;
using Kysect.BotFramework.Core.Tools.Extensions;
using Kysect.BotFramework.Core.Tools.Loggers;
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
            var commandDescriptor = _serviceProvider.GetCommandDescriptor(args.CommandName, _caseSensitive);

            return commandDescriptor.Args.Length == args.Arguments.Count
                ? Result.Ok() 
                : Result.Fail("Wrong arguments count");
        }


        public Result CanCommandBeExecuted(CommandContainer args)
        {
            IBotCommand command = _serviceProvider.GetCommand(args.CommandName, _caseSensitive);
            
            var descriptor = command.GetBotCommandDescriptorAttribute();
            Result canExecute = command.CanExecute(args);

            return canExecute.IsSuccess
                ? Result.Ok()
                : Result.Fail(
                    $"Command [{descriptor.CommandName}] cannot be executed: {canExecute}");
        }

        public CommandHandler SetCaseSensitive(bool caseSensitive)
        {
            _caseSensitive = caseSensitive;
            return this;
        }

        public IBotMessage ExecuteCommand(CommandContainer args)
        {
            IBotCommand command = _serviceProvider.GetCommand(args.CommandName, _caseSensitive);

            try
            {
                return command switch
                {
                    IBotAsyncCommand asyncCommand => asyncCommand.Execute(args).Result,
                    IBotSyncCommand syncCommand => syncCommand.Execute(args),
                    _ => throw new ArgumentOutOfRangeException(command.GetType().Name,"Command execution failed. Wrong command inheritance.")
                };
            }
            catch (Exception e)
            {
                LoggerHolder.Instance.Error("Command execution failed. Command: {args.CommandName}; arguments: {string.Join(", ", args.Arguments)}");
                throw;
            }
        }
    }
}