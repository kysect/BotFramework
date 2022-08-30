﻿using System;
using System.Threading.Tasks;
using Kysect.BotFramework.Abstractions;
using Kysect.BotFramework.Abstractions.BotMessages;
using Kysect.BotFramework.Abstractions.CommandInvoking;
using Kysect.BotFramework.Abstractions.Commands;
using Kysect.BotFramework.ApiProviders;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.CommandInvoking;
using Kysect.BotFramework.Core.Exceptions;
using Kysect.BotFramework.Core.Tools.Extensions;
using Kysect.BotFramework.Core.Tools.Loggers;
using Kysect.BotFramework.Tools;
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

        private void ApiProviderOnMessage(object sender, IBotEventArgs e)
        {
#pragma warning disable CS4014
            RunCommandProcessing(e);
#pragma warning restore CS4014
        }

        private async Task RunCommandProcessing(IBotEventArgs e)
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

        private async Task ProcessMessage(IBotEventArgs e)
        {
            using var scope = _serviceProvider.CreateScope();
            
            ICommandContainer commandContainer = _commandParser.ParseCommand(e);

            if (!commandContainer.StartsWithPrefix(_prefix))
            {
                return;
            }

            commandContainer.RemovePrefix(_prefix);

            IBotCommand command = scope.ServiceProvider.GetCommand(commandContainer.CommandName);
            var commandHandler = new CommandHandler(command, commandContainer, e.GetCommandArguments());
            
            Result checkResult = commandHandler.CheckArguments();
            if (checkResult.IsFailed)
            {
                throw new CommandArgumentsException(checkResult.Message);
            }

            checkResult = commandHandler.CanCommandBeExecuted();
            if (checkResult.IsFailed)
            {
                throw new CommandCantBeExecutedException(checkResult.Message);
            }

            IBotMessage message = await commandHandler.ExecuteCommand();

            await message.SendAsync(_apiProvider, e.SenderInfo);
        }

        private async Task HandlerError(BotException exception, IBotEventArgs botEventArgs)
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