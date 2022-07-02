using System;

namespace Kysect.BotFramework.Core.Commands;

[AttributeUsage(AttributeTargets.Class)]
public class BotCommandDescriptorAttribute : Attribute
{
    public string CommandName { get; }
    public string Description { get; }
    public OptionalArguments OptionalArguments { get; }

    public BotCommandDescriptorAttribute(string commandName, string description = "",
        OptionalArguments optionalArguments = OptionalArguments.None)
    {
        CommandName = commandName;
        Description = description;
        OptionalArguments = optionalArguments;
    }
}
