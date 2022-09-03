namespace Kysect.BotFramework.Abstractions.Contexts;

public interface IDiscordSenderInfo : ISenderInfo
{
    ulong GuildId { get; }
}