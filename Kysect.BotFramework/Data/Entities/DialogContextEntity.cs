using System.Linq;
using Kysect.BotFramework.Core.Contexts;

namespace Kysect.BotFramework.Data.Entities
{
    public class DialogContextEntity
    {
        public int State { get; set; }
        public ContextType ContextType { get; }
        public long SenderInfoId { get; }

        public DialogContextEntity(ContextType contextType, long senderInfoId)
        {
            ContextType = contextType;
            SenderInfoId = senderInfoId;
        }

        public static DialogContextEntity GetOrCreate(
            long senderInfoId,
            ContextType type,
            BotFrameworkDbContext dbContext)
        {
            DialogContextEntity contextModel = dbContext.DialogContexts.FirstOrDefault(
                c => 
                    c.SenderInfoId == senderInfoId 
                    && c.ContextType == type);

            if (contextModel is null)
            {
                contextModel = new DialogContextEntity(type,senderInfoId);
                dbContext.DialogContexts.Add(contextModel);
                dbContext.SaveChanges();
            }

            return contextModel;
        }
    }
}