using System;
using System.Collections.Generic;
using System.Reflection;
using FluentScanning;
using Kysect.BotFramework.ApiProviders;
using Kysect.BotFramework.Core.Commands;
using Kysect.BotFramework.Core.Contexts.Providers;
using Kysect.BotFramework.Core.Exceptions;
using Kysect.BotFramework.Core.Tools;
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
        private readonly Dictionary<string, Type> _commandTypes = new();

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
            _commandTypes[descriptor.CommandName] = typeof(T);
            ServiceCollection.AddScoped<T>();
            LoggerHolder.Instance.Information($"New command added: {descriptor.CommandName}");

            return this;
        }

        public BotManagerBuilder AddCommandsFromAssembly(Assembly assembly)
        {
            var scanner = new AssemblyScanner(assembly);
            var commandTypes = scanner.ScanForTypesThat()
                .AreAssignableTo<IBotCommand>()
                .HaveAttribute<BotCommandDescriptorAttribute>()
                .ArePublic()
                .AsTypes();

            foreach (var commandType in commandTypes)
            {
                _commandTypes[commandType.GetBotCommandDescriptorAttribute().CommandName] = commandType;
                ServiceCollection.AddScoped(commandType);
            }
            
            return this;
        }

        public BotManagerBuilder SetDatabaseOptions(Action<DbContextOptionsBuilder> optionsAction)
        {
            ServiceCollection
                .AddDbContext<BotFrameworkDbContext>(optionsAction)
                .AddScoped<IDialogContextProvider, StorageDialogContextProvider>();

            return this;
        }

        public BotManager Build(IBotApiProvider apiProvider)
        {
            ServiceCollection.AddSingleton(new CommandTypeProvider(_commandTypes, _caseSensitive));

            ServiceProvider serviceProvider = ServiceCollection.BuildServiceProvider();
            serviceProvider.ValidateCommands(_commandTypes.Values);
            return new BotManager(apiProvider, serviceProvider, _prefix, _sendErrorLogToUser);
        }
    }
}