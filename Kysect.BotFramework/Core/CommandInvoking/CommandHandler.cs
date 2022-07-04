using System.Threading.Tasks;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.Commands;
using Kysect.BotFramework.Core.Tools;
using Kysect.BotFramework.Core.Tools.Extensions;
using Kysect.BotFramework.Core.Tools.Loggers;

namespace Kysect.BotFramework.Core.CommandInvoking
{
    public class CommandHandler
    {
        private readonly CommandContainer _args;
        private readonly IBotCommand _command;

        public CommandHandler(CommandContainer args, IBotCommand command)
        {
            _args = args;
            _command = command;
        }

        public Result CheckArguments()
        {
            var argumentHandler = new ArgumentHandler(_args, _command);

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
            Result canExecute = _command.CanExecute(_args);

            return canExecute.IsSuccess
                ? Result.Ok()
                : Result.Fail(
                    $"Command [{descriptor.CommandName}] cannot be executed: {canExecute.Message}");
        }

        public async Task<IBotMessage> ExecuteCommand()
        {
            try
            {
                return await _command.Execute(_args);
            }
            catch
            {
                LoggerHolder.Instance.Error(
                    $"Command execution failed. Command: {_args.CommandName}; arguments: {string.Join(", ", _args.Arguments)}");
                throw;
            }
        }
    }
}