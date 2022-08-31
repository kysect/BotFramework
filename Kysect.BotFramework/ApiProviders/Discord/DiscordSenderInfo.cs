using Kysect.BotFramework.Abstractions.Contexts;
using Kysect.BotFramework.Abstractions.Visitors;

namespace Kysect.BotFramework.ApiProviders.Discord;

public class DiscordSenderInfo : IDiscordSenderInfo
{
    public DiscordSenderInfo(long chatId, long userSenderId, string userSenderUsername, bool isAdmin, ulong guildId)
    {
        GuildId = guildId;
        ChatId = chatId;
        UserSenderId = userSenderId;
        UserSenderUsername = userSenderUsername;
        IsAdmin = isAdmin;
    }

    public long ChatId { get; }

    public long UserSenderId { get; }

    public string UserSenderUsername { get; }

    public bool IsAdmin { get; }

    public ulong GuildId { get; }

    public TContext Accept<TContext>(IContextVisitor<TContext> visitor)
        => visitor.Visit(this);
}