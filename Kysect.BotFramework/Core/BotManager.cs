using System;
using System.Threading.Tasks;
using Kysect.BotFramework.ApiProviders;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.CommandInvoking;
using Kysect.BotFramework.Core.Commands;
using Kysect.BotFramework.Core.Exceptions;
using Kysect.BotFramework.Core.Tools;
using Kysect.BotFramework.Core.Tools.Loggers;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.BotFramework.Core
{
    public class BotManager : IDisposable
    {
        private readonly IBotApiProvider _apiProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly ICommandParser _commandParser;
        private readonly char _prefix;
        private readonly bool _sendErrorLogToUser;

        public BotManager(IBotApiProvider apiProvider, IServiceProvider provider, char prefix, bool sendErrorLogToUser)
        {
            _apiProvider = apiProvider;
            _serviceProvider = provider;
            _prefix = prefix;
            _sendErrorLogToUser = sendErrorLogToUser;
            _commandParser = new CommandParser();
        }

        public void Dispose()
        {
            _apiProvider.OnMessage -= ApiProviderOnMessage;
        }

        public void Start()
        {
            _apiProvider.OnMessage += ApiProviderOnMessage;
        }

        private void ApiProviderOnMessage(object sender, BotNewMessageEventArgs e)
        {
#pragma warning disable CS4014
            RunCommandProcessing(e);
#pragma warning restore CS4014
        }

        private async Task RunCommandProcessing(BotNewMessageEventArgs e)
        {
            try
            {
                await ProcessMessage(e);
            }
            catch (BotException exception)
            {
                await HandlerError(exception, e);
            }
            catch (Exception exception)
            {
                LoggerHolder.Instance.Error(exception, $"Message handling from [{e.SenderInfo.UserSenderUsername}] failed.");
                LoggerHolder.Instance.Debug($"Failed message: {e.Message.Text}");
                //FYI: we do not need to restart on each exception, but probably we have case were restart must be.
                //_apiProvider.Restart();
            }
        }

        private async Task ProcessMessage(BotNewMessageEventArgs e)
        {
            using var scope = _serviceProvider.CreateScope();
            var commandHandler = new CommandHandler(scope.ServiceProvider);
            
            var botEventArgs = new BotEventArgs(e.Message, e.SenderInfo, scope.ServiceProvider);
            CommandContainer commandContainer = _commandParser.ParseCommand(botEventArgs);

            if (!commandContainer.StartsWithPrefix(_prefix))
            {
                return;
            }

            commandContainer.RemovePrefix(_prefix);

            Result checkResult = commandHandler.CheckArgsCount(commandContainer);
            if (!checkResult.IsSuccess)
            {
                throw new CommandArgumentsException(checkResult.Message);
            }

            checkResult = commandHandler.CanCommandBeExecuted(commandContainer);
            if (!checkResult.IsSuccess)
            {
                throw new CommandCantBeExecutedException(checkResult.Message);
            }

            IBotMessage message = await commandHandler.ExecuteCommand(commandContainer);

            await message.SendAsync(_apiProvider, e.SenderInfo);
        }

        private async Task HandlerError(BotException exception, BotNewMessageEventArgs botEventArgs)
        {
            LoggerHolder.Instance.Error(exception.Message);

            if (_sendErrorLogToUser)
            {
                var errorLogMessage = new BotTextMessage(exception.Message);
                await errorLogMessage.SendAsync(_apiProvider, botEventArgs.SenderInfo);
            }
            else
            {
                var errorMessage = new BotTextMessage("Something went wrong.");
                await errorMessage.SendAsync(_apiProvider, botEventArgs.SenderInfo);
            }
        }
    }
}