﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentResults;
using Kysect.BotFramework.ApiProviders;
using Kysect.BotFramework.Core.CommandInvoking;
using Kysect.BotFramework.Core.Tools.Loggers;
using Serilog;

namespace Kysect.BotFramework.Core
{
    public class BotManager : IDisposable
    {
        private readonly CommandHandler _commandHandler;
        private readonly IBotApiProvider _apiProvider;
        private readonly ICommandParser _commandParser;

        private char _prefix = '\0';
        private bool _caseSensitive = true;
        private bool _sendErrorLogToUser;

        public BotManager(IBotApiProvider apiProvider)
        {
            _apiProvider = apiProvider;

            _commandParser = new CommandParser();
            _commandHandler = new CommandHandler();
        }

        public void Start()
        {
            _apiProvider.OnMessage += ApiProviderOnMessage;
        }

        public BotManager AddDefaultLogger()
        {
            LoggerHolder.Instance.Information("Default logger was initalized");
            return this;
        }

        public BotManager AddLogger(ILogger logger)
        {
            LoggerHolder.Init(logger);
            LoggerHolder.Instance.Information("Logger was initalized");

            return this;
        }

        public BotManager SetPrefix(char prefix)
        {
            _prefix = prefix;
            LoggerHolder.Instance.Debug($"New prefix set: {prefix}");
            return this;
        }

        public BotManager WithoutCaseSensitiveCommands()
        {
            _caseSensitive = false;
            _commandHandler.WithoutCaseSensitiveCommands();
            LoggerHolder.Instance.Debug("Case sensitive was disabled");

            return this;
        }

        public BotManager EnableErrorLogToUser()
        {
            _sendErrorLogToUser = true;
            LoggerHolder.Instance.Information("Enable log redirection to user");

            return this;
        }

        public BotManager AddCommand(IBotCommand command)
        {
            _commandHandler.RegisterCommand(command);
            LoggerHolder.Instance.Information($"New command added: {command.CommandName}");

            return this;
        }

        public BotManager AddCommands(ICollection<IBotCommand> commands)
        {
            foreach (IBotCommand command in commands)
                _commandHandler.RegisterCommand(command);

            LoggerHolder.Instance.Information($"New commands added: {string.Join(", ", commands.Select(c => c.CommandName))}");

            return this;
        }

        private void ApiProviderOnMessage(object sender, BotEventArgs e)
        {
            
            try
            {
                ProcessMessage(e);
            }
            catch (Exception exception)
            {
                LoggerHolder.Instance.Error(exception, $"Message handling from [{e.Username}] failed.");
                LoggerHolder.Instance.Debug($"Failed message: {e.Text}");
                //FYI: we do not need to restart on each exception, but probably we have case were restart must be.
                //_apiProvider.Restart();
            }
        }

        private void ProcessMessage(BotEventArgs e)
        {
            Result<CommandArgumentContainer> commandResult = _commandParser.ParseCommand(e);
            if (commandResult.IsFailed)
            {
                HandlerError(commandResult, e);
                return;
            }

            if (!commandResult.Value.EnsureStartWithPrefix(_prefix))
                return;

            commandResult = _commandHandler.IsCorrectArgumentCount(commandResult.Value.ApplySettings(_prefix, _caseSensitive));
            if (commandResult.IsFailed)
            {
                HandlerError(commandResult, e);
                return;
            }

            commandResult = _commandHandler.IsCommandCanBeExecuted(commandResult.Value);
            if (commandResult.IsFailed)
            {
                HandlerError(commandResult, e);
                return;
            }

            Result<string> executionResult = _commandHandler.ExecuteCommand(commandResult.Value);
            if (executionResult.IsFailed)
            {
                HandlerError(commandResult, e);
                return;
            }

            _apiProvider.WriteMessage(new BotEventArgs(executionResult.Value, commandResult.Value));
        }

        private void HandlerError(Result result, BotEventArgs botEventArgs)
        {
            LoggerHolder.Instance.Error(result.ToString());

            _apiProvider.WriteMessage(new BotEventArgs("Something went wrong.", botEventArgs));
            if (_sendErrorLogToUser)
                _apiProvider.WriteMessage(new BotEventArgs(result.ToString(), botEventArgs));
        }

        public void Dispose()
        {
            _apiProvider.OnMessage -= ApiProviderOnMessage;
        }
    }
}