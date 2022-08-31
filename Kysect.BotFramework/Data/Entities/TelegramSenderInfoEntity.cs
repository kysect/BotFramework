using System.Linq;
using Kysect.BotFramework.Abstractions.Contexts;

namespace Kysect.BotFramework.Data.Entities
{
    public class TelegramSenderInfoEntity : SenderInfoEntity
    {
        public static TelegramSenderInfoEntity GetOrCreate(ITelegramSenderInfo senderInfo, BotFrameworkDbContext dbContext)
        {
            var senderInfoEntity = dbContext.TelegramSenderInfos.FirstOrDefault(
                si => 
                si.ChatId == senderInfo.ChatId
                && si.UserSenderId == senderInfo.UserSenderId
            );

            if (senderInfoEntity is null)
            {
                senderInfoEntity = new TelegramSenderInfoEntity()
                {
                    ChatId = senderInfo.ChatId,
                    UserSenderId = senderInfo.UserSenderId
                };

                dbContext.TelegramSenderInfos.Add(senderInfoEntity);
                dbContext.SaveChanges();
            }

            return senderInfoEntity;
        }
    }
}