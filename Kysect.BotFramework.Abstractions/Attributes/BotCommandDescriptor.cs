namespace Kysect.BotFramework.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class BotCommandDescriptorAttribute : Attribute
{
    public string CommandName { get; }
    public string Description { get; }
    public bool ArgumentsOptional { get; }

    public BotCommandDescriptorAttribute(
        string commandName,
        string description = "",
        bool argumentsOptional = false)
    {
        CommandName = commandName;
        Description = description;
        ArgumentsOptional = argumentsOptional;
    }
}
