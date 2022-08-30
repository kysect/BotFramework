﻿using Kysect.BotFramework.Abstractions.Contexts;
using Kysect.BotFramework.Data;

namespace Kysect.BotFramework.Core.Contexts
{
    public abstract class SenderInfo : ISenderInfo
    {
        public long ChatId { get; internal set; }
        public long UserSenderId { get; internal set; }
        public string UserSenderUsername { get; internal set; }
        public bool IsAdmin { get; internal set; }

        internal abstract ContextType ContextType { get; }

        protected SenderInfo(long chatId, long userSenderId, string userSenderUsername, bool isAdmin)
        {
            ChatId = chatId;
            UserSenderId = userSenderId;
            UserSenderUsername = userSenderUsername;
            IsAdmin = isAdmin;
        }

        internal abstract IDialogContext GetOrCreateDialogContext(BotFrameworkDbContext dbContext);
    }
}