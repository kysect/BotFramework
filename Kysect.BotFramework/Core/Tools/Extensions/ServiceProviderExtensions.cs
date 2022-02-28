using System;
using System.Collections;
using System.Collections.Concurrent;
using Kysect.BotFramework.Core.Commands;
using Kysect.BotFramework.Core.Exceptions;
using Kysect.BotFramework.Core.Tools.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.BotFramework.Core.Tools.Extensions
{
    internal static class ServiceProviderExtensions
    {
        private static ConcurrentDictionary<string, Type> CommandsTypes = new ();

        private static Type GetCommandType(this ServiceProvider  provider, string commandName, bool caseSensitive)
        {
            var engine = provider.GetFieldValue("_engine");
            var callSiteFactory = engine.GetPropertyValue("CallSiteFactory");
            var descriptorLookup = callSiteFactory.GetFieldValue("_descriptorLookup");

            if (descriptorLookup is not IDictionary dictionary)
            {
                return null;
            }

            foreach (DictionaryEntry entry in dictionary)
            {
                var type = (Type)entry.Key;
                var descriptor = type.GetBotCommandDescriptorAttribute();
                if (descriptor is null || !typeof(IBotCommand).IsAssignableFrom(type))
                    continue;

                if (!caseSensitive && string.Equals(descriptor.CommandName, commandName,
                                                    StringComparison.InvariantCultureIgnoreCase))
                {
                    return type;
                }

                if (string.Equals(descriptor.CommandName, commandName))
                {
                    return type;
                }
            }

            return null;
        }

        public static IBotCommand GetCommand(this ServiceProvider provider, string commandName, bool caseSensitive)
        {
            var type = CommandsTypes.GetOrAdd(
                commandName,
                t => provider.GetCommandType(commandName, caseSensitive));
            
            if (type is null)
                throw new CommandNotFoundException("Couldn't find command with such name");
            
            return provider.GetService(type) as IBotCommand;
        }
        
        public static BotCommandDescriptorAttribute GetCommandDescriptor(this ServiceProvider  provider, string commandName, bool caseSensitive)
            => provider.GetCommand(commandName, caseSensitive).GetBotCommandDescriptorAttribute();
    }
}