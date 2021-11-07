using System;
using System.Collections.Concurrent;
using System.Reflection;
using Kysect.BotFramework.Core.Commands;

namespace Kysect.BotFramework.Core.Tools.Extensions
{
    public static class GetAttributeExtensions
    {
        private static readonly ConcurrentDictionary<Type, BotCommandDescriptorAttribute> Attributes = new ();

        public static BotCommandDescriptorAttribute GetBotCommandDescriptorAttribute(this Type type)
            => Attributes.GetOrAdd(
                type,
                t => t.GetCustomAttribute<BotCommandDescriptorAttribute>());

        public static BotCommandDescriptorAttribute GetBotCommandDescriptorAttribute<T>(this T command)
            where T : IBotCommand
            => typeof(T).GetBotCommandDescriptorAttribute();
    }
}
