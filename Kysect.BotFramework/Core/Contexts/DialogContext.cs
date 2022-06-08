using System.Linq;
using System.Threading.Tasks;
using Kysect.BotFramework.Data;
using Kysect.BotFramework.Data.Entities;

namespace Kysect.BotFramework.Core.Contexts
{
    public class DialogContext
    {
        public int State { get; set; }
        private readonly ContextType _contextType;
        private readonly long _senderInfoId;
        public SenderInfo SenderInfo { get; }

        public DialogContext(ContextType contextType, SenderInfo senderInfo)
        {
            _contextType = contextType;
            SenderInfo = senderInfo;
        }

        public DialogContext(int state, long senderInfoId, ContextType contextType, SenderInfo senderInfo)
            : this(contextType, senderInfo)
        {
            State = state;
            _senderInfoId = senderInfoId;
        }

        internal async Task SaveChangesAsync(BotFrameworkDbContext dbContext)
        {
            DialogContextEntity context = dbContext.DialogContexts.FirstOrDefault(x => x.SenderInfoId == _senderInfoId && x.ContextType == _contextType);
            context.State = State;
            dbContext.DialogContexts.Update(context);
            await dbContext.SaveChangesAsync();
        }
    }
}