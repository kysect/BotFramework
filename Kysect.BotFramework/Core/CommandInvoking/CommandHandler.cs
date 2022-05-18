using System;
using System.Threading.Tasks;
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

        public CommandHandler(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Result CheckArgsCount(CommandContainer args)
        {
            var commandDescriptor = _serviceProvider.GetCommandDescriptor(args.CommandName);

            return commandDescriptor.Args.Length == args.Arguments.Count
                ? Result.Ok() 
                : Result.Fail("Wrong arguments count");
        }


        public Result CanCommandBeExecuted(CommandContainer args)
        {
            IBotCommand command = _serviceProvider.GetCommand(args.CommandName);
            
            var descriptor = command.GetBotCommandDescriptorAttribute();
            Result canExecute = command.CanExecute(args);

            return canExecute.IsSuccess
                ? Result.Ok()
                : Result.Fail(
                    $"Command [{descriptor.CommandName}] cannot be executed: {canExecute}");
        }

        public async Task<IBotMessage> ExecuteCommand(CommandContainer args)
        {
            IBotCommand command = _serviceProvider.GetCommand(args.CommandName);

            try
            {
                return await command.Execute(args);
            }
            catch (Exception e)
            {
                LoggerHolder.Instance.Error("Command execution failed. Command: {args.CommandName}; arguments: {string.Join(", ", args.Arguments)}");
                throw;
            }
        }
    }
}