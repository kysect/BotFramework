using System;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.BotFramework.Core.Commands
{
    public class BotCommandDescriptorAttribute : Attribute
    {
        public string CommandName { get; }
        public string Description { get; }
        public string[] Args { get; }

        public BotCommandDescriptorAttribute(string commandName, string description, string[] args)
        {
            CommandName = commandName;
            Description = description;
            Args = args;
        }

        public BotCommandDescriptorAttribute(string commandName, string description)
        {
            CommandName = commandName;
            Description = description;
            Args = Array.Empty<string>();
        }
    }
}