using System;
using Kysect.BotFramework.Core.Commands;
using Kysect.BotFramework.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.BotFramework.Core.Tools.Extensions
{
    internal static class ServiceProviderExtensions
    {
        private static Type GetCommandType(this IServiceProvider  provider, string commandName)
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
        
        public static BotCommandDescriptorAttribute GetCommandDescriptor(this IServiceProvider  provider, string commandName)
            => provider.GetCommand(commandName).GetBotCommandDescriptorAttribute();
    }
}