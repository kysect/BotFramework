﻿using Kysect.BotFramework.Abstractions.Visitors;

namespace Kysect.BotFramework.Abstractions.Contexts;

public interface ISenderInfo
{
    long ChatId { get; }
    long UserSenderId { get; }
    string UserSenderUsername { get; }
    bool IsAdmin { get; }

    TContext Accept<TContext>(IContextVisitor<TContext> visitor);
}