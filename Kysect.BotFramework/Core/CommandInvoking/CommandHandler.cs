using System.Collections.Generic;
using System.Threading.Tasks;
using Kysect.BotFramework.Abstractions.BotMessages;
using Kysect.BotFramework.Abstractions.Commands;
using Kysect.BotFramework.Core.Tools.Extensions;
using Kysect.BotFramework.Core.Tools.Loggers;
using Kysect.BotFramework.Tools;

namespace Kysect.BotFramework.Core.CommandInvoking
{
    public class CommandHandler
    {
        private readonly IBotCommand _command;
        private readonly ICommandContainer _commandContainer;
        private readonly List<string> _arguments;

        public CommandHandler(
            IBotCommand command,
            ICommandContainer commandContainer,
            List<string> arguments)
        {
            _command = command;
            _commandContainer = commandContainer;
            _arguments = arguments;
        }

        public Result CheckArguments()
        {
            var argumentHandler = new ArgumentHandler(_command, _arguments);

            Result checkResult = argumentHandler.CheckArgumentsCount();
            if (checkResult.IsFailed)
            {
                return checkResult;
            }

            return argumentHandler.TryAssignArguments();
        }

        public Result CanCommandBeExecuted()
        {
            var descriptor = _command.GetBotCommandDescriptorAttribute();
            Result canExecute = _command.CanExecute(_commandContainer);

            return canExecute.IsSuccess
                ? Result.Ok()
                : Result.Fail(
                    $"Command [{descriptor.CommandName}] cannot be executed: {canExecute.Message}");
        }

        public async Task<IBotMessage> ExecuteCommand()
        {
            try
            {
                return await _command.Execute(_commandContainer);
            }
            catch
            {
                LoggerHolder.Instance.Error(
                    $"Command execution failed. Command: {_commandContainer.CommandName}; arguments: {string.Join(", ", _arguments)}");
                throw;
            }
        }
    }
}