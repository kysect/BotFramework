﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
            => command.GetType().GetBotCommandDescriptorAttribute();

        public static List<string> GetBotCommandArgumentNames<T>(this T command)
            where T : IBotCommand
            => command
                .GetType()
                .GetProperties()
                .Where(p => p.GetCustomAttribute<BotCommandArgumentAttribute>() is not null)
                .Select(p => p.Name)
                .ToList();
    }
}