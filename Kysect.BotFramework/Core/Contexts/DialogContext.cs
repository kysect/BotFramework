using System.Linq;
using System.Threading.Tasks;
using Kysect.BotFramework.Data;
using Kysect.BotFramework.Data.Entities;

namespace Kysect.BotFramework.Core.Contexts
{
    public class DialogContext
    {
        private readonly long _senderInfoId;

        public SenderInfo SenderInfo { get; }
        public int? State { get; set; }

        public DialogContext(SenderInfo senderInfo)
            => SenderInfo = senderInfo;

        public DialogContext(int state, long senderInfoId, SenderInfo senderInfo)
            : this(senderInfo)
        {
            _senderInfoId = senderInfoId;
            State = state;
        }

        internal async Task SaveChangesAsync(BotFrameworkDbContext dbContext)
        {
            DialogContextEntity context = dbContext.DialogContexts.FirstOrDefault(
                x =>
                    x.SenderInfoId == _senderInfoId
                    && x.ContextType == SenderInfo.ContextType);

            context!.State = State.GetValueOrDefault();
            dbContext.DialogContexts.Update(context);
            await dbContext.SaveChangesAsync();
        }
    }
}