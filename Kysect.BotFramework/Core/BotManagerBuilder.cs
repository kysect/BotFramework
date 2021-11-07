using System;
using System.Collections.Generic;
using Kysect.BotFramework.ApiProviders;
using Kysect.BotFramework.Core.CommandInvoking;
using Kysect.BotFramework.Core.Commands;
using Kysect.BotFramework.Core.Exceptions;
using Kysect.BotFramework.Core.Tools.Extensions;
using Kysect.BotFramework.Core.Tools.Loggers;
using Kysect.BotFramework.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Kysect.BotFramework.Core
{
    public class BotManagerBuilder
    {
        private bool _caseSensitive = true;

        private char _prefix = '\0';
        private bool _sendErrorLogToUser;

        private bool _dbContextInitialized = false;

        //FK: we should make this private, shouldn't we?
        public ServiceCollection ServiceCollection { get; } = new ServiceCollection();

        public BotManagerBuilder AddLogger(ILogger logger)
        {
            LoggerHolder.Init(logger);
            LoggerHolder.Instance.Information("Logger was initialized");

            return this;
        }

        public BotManagerBuilder SetPrefix(char prefix)
        {
            _prefix = prefix;
            LoggerHolder.Instance.Debug($"New prefix set: {prefix}");
            return this;
        }

        public BotManagerBuilder SetCaseSensitive(bool caseSensitive)
        {
            _caseSensitive = caseSensitive;
            LoggerHolder.Instance.Debug($"Case sensitive was updated: {caseSensitive}");

            return this;
        }

        public BotManagerBuilder EnableErrorLogToUser()
        {
            _sendErrorLogToUser = true;
            LoggerHolder.Instance.Information("Enable log redirection to user");

            return this;
        }

        public BotManagerBuilder AddCommand<T>() where T : class, IBotCommand
        {
            var descriptor = typeof(T).GetBotCommandDescriptorAttribute();

            if (descriptor is null)
                throw new BotValidException("Command must have descriptor attribute");
            ServiceCollection.AddScoped<T>();
            LoggerHolder.Instance.Information($"New command added: {descriptor.CommandName}");

            return this;
        }

        public BotManagerBuilder SetDatabaseOptions(Action<DbContextOptionsBuilder> optionsAction)
        {
            ServiceCollection.AddDbContext<BotFrameworkDbContext>(optionsAction);
            _dbContextInitialized = true;
            return this;
        }

        public BotManager Build(IBotApiProvider apiProvider)
        {
            if (!_dbContextInitialized)
                throw new BotValidException("Database context options was not initialized");

            ServiceProvider serviceProvider = ServiceCollection.BuildServiceProvider();
            var commandHandler = new CommandHandler(serviceProvider);
            commandHandler.SetCaseSensitive(_caseSensitive);
            return new BotManager(apiProvider, commandHandler, _prefix, _sendErrorLogToUser, serviceProvider);
        }
    }
}