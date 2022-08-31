using Kysect.BotFramework.Abstractions.Contexts;
using Kysect.BotFramework.Abstractions.Visitors;
using System;

namespace Kysect.BotFramework.ApiProviders.Telegram;

public class TelegramSenderInfo : ITelegramSenderInfo
{
    public TelegramSenderInfo(long chatId, long userSenderId, string userSenderUsername, bool isAdmin)
    {
        ChatId = chatId;
        UserSenderId = userSenderId;
        UserSenderUsername = userSenderUsername;
        IsAdmin = isAdmin;
    }

    public long ChatId { get; }

    public long UserSenderId { get; }

    public string UserSenderUsername { get; }

    public bool IsAdmin { get; }

    public TContext Accept<TContext>(IContextVisitor<TContext> visitor)
    {
        if (visitor is null)
            throw new ArgumentNullException(nameof(visitor));
        
        return visitor.Visit(this);
    }
}