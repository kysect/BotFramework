using System;
using System.Threading.Tasks;
using Kysect.BotFramework.ApiProviders;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.CommandInvoking;
using Kysect.BotFramework.Core.Commands;
using Kysect.BotFramework.Core.Contexts;
using Kysect.BotFramework.Core.Exceptions;
using Kysect.BotFramework.Core.Tools;
using Kysect.BotFramework.Core.Tools.Loggers;
using Kysect.BotFramework.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.BotFramework.Core
{
    public class BotManager : IDisposable
    {
        private readonly IBotApiProvider _apiProvider;
        private readonly CommandHandler _commandHandler;
        private readonly ICommandParser _commandParser;
        private readonly char _prefix;
        private readonly bool _sendErrorLogToUser;
        private readonly ServiceProvider _serviceProvider;

        public BotManager(IBotApiProvider apiProvider, CommandHandler commandHandler, char prefix,
            bool sendErrorLogToUser, ServiceProvider provider)
        {
            _serviceProvider = provider;
            _apiProvider = apiProvider;
            _prefix = prefix;
            _sendErrorLogToUser = sendErrorLogToUser;
            _commandParser = new CommandParser();
            _commandHandler = commandHandler;
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
            var dbContext =  _serviceProvider.GetRequiredService<BotFrameworkDbContext>();
            DialogContext context = e.SenderInfo.GetOrCreateDialogContext(dbContext);
            var botEventArgs = new BotEventArgs(e.Message, context);
            
            CommandContainer commandContainer = _commandParser.ParseCommand(botEventArgs);

            if (!commandContainer.StartsWithPrefix(_prefix))
            {
                return;
            }

            commandContainer.RemovePrefix(_prefix);

            Result checkResult = _commandHandler.CheckArgsCount(commandContainer);
            if (!checkResult.IsSuccess)
            {
                throw new CommandArgumentsException(checkResult.Message);
            }

            checkResult = _commandHandler.CanCommandBeExecuted(commandContainer);
            if (!checkResult.IsSuccess)
            {
                throw new CommandCantBeExecutedException(checkResult.Message);
            }

            IBotMessage message = await _commandHandler.ExecuteCommand(commandContainer);

            await commandContainer.Context.SaveChangesAsync(dbContext);
            
            SenderInfo sender = commandContainer.Context.SenderInfo;

            await message.SendAsync(_apiProvider, sender);
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