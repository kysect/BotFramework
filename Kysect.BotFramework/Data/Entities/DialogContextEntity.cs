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
    }
}