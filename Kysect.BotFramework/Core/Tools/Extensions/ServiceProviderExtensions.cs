using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Kysect.BotFramework.Core.Commands;
using Kysect.BotFramework.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.BotFramework.Core.Tools.Extensions
{
    internal static class ServiceProviderExtensions
    {
        private static Type GetCommandType(this IServiceProvider provider, string commandName)
        {
            var commandTypeProvider = provider.GetRequiredService<CommandTypeProvider>();
            return commandTypeProvider.GetCommandTypeOrDefault(commandName);
        }

        public static IBotCommand GetCommand(this IServiceProvider provider, string commandName)
        {
            var type = provider.GetCommandType(commandName);

            if (type is null)
                throw new CommandNotFoundException("Couldn't find command with such name");

            return provider.GetService(type) as IBotCommand;
        }

        public static void ValidateCommands(this IServiceProvider provider,
            IEnumerable<Type> commandTypes)
            => commandTypes
                .ToList()
                .ForEach(c =>
                    (provider.GetRequiredService(c) as IBotCommand)
                        .GetBotCommandArgumentNames()
                        .ForEach(n =>
                        {
                            PropertyInfo property = c.GetProperty(n,
                                BindingFlags.Public | BindingFlags.Instance);

                            if (property is null)
                            {
                                throw new Exception(
                                    $"Argument property {n} in command {c} is not accessible");
                            }
                            
                            if (!property.CanWrite)
                            {
                                throw new Exception(
                                    $"Argument property {n} in command {c} can not be set");
                            }
                        })
                    );
    }
}