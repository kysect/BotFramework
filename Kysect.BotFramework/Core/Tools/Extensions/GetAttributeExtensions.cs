using System;
using System.Collections.Concurrent;
using Kysect.BotFramework.Core.Commands;

namespace Kysect.BotFramework.Core.Tools.Extensions
{
    public static class GetAttributeExtensions
    {
        private static readonly ConcurrentDictionary<Type, BotCommandDescriptorAttribute> Attributes = new ();
        
        public static BotCommandDescriptorAttribute GetBotCommandDescriptorAttribute(this Type type)
            => Attributes.GetOrAdd(
                type,
                t => Attribute.GetCustomAttribute(t, typeof(BotCommandDescriptorAttribute))
                    as BotCommandDescriptorAttribute);
    }
}