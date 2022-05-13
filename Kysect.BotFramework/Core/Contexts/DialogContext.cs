using System.Linq;
using System.Threading.Tasks;
using Kysect.BotFramework.Data;
using Kysect.BotFramework.Data.Entities;

namespace Kysect.BotFramework.Core.Contexts
{
    public class DialogContext
    {
        private readonly ContextType _contextType;
        private readonly long _senderInfoId;

        public int State { get; set; }
        public SenderInfo SenderInfo { get; }

        public DialogContext(int state, long senderInfoId, ContextType contextType, SenderInfo senderInfo)
        {
            State = state;
            _senderInfoId = senderInfoId;
            _contextType = contextType;
            SenderInfo = senderInfo;
        }

        internal async Task SaveChangesAsync(BotFrameworkDbContext dbContext)
        {
            DialogContextEntity context = DialogContextEntity.GetOrCreate(_senderInfoId, _contextType, dbContext);
            context.State = State;
            dbContext.DialogContexts.Update(context);
            await dbContext.SaveChangesAsync();
        }
    }
}