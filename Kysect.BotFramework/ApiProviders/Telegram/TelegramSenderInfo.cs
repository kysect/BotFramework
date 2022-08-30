﻿using Kysect.BotFramework.Abstractions.Contexts;
using Kysect.BotFramework.Core.Contexts;
using Kysect.BotFramework.Data;
using Kysect.BotFramework.Data.Entities;

namespace Kysect.BotFramework.ApiProviders.Telegram
{
    public class TelegramSenderInfo : SenderInfo
    {
        internal override ContextType ContextType => ContextType.Telegram;

        public TelegramSenderInfo(long chatId, long userSenderId, string userSenderUsername, bool isAdmin)
            : base(chatId, userSenderId, userSenderUsername, isAdmin)
        { }
        
        private TelegramSenderInfoEntity ToEntity()
        {
            var entity = new TelegramSenderInfoEntity()
            {
                ChatId = ChatId,
                UserSenderId = UserSenderId
            };
            return entity;
        }

        internal override IDialogContext GetOrCreateDialogContext(BotFrameworkDbContext dbContext)
        {
            var contextSenderInfo = TelegramSenderInfoEntity.GetOrCreate(this, dbContext);
            var contextModel = DialogContextEntity.GetOrCreate(contextSenderInfo, ContextType, dbContext);
            
            return new DialogContext(contextModel.State, contextModel.SenderInfoId, this, ContextType, dbContext);
        }
    }
}