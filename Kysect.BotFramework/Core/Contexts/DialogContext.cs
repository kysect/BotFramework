using System.Linq;
using System.Threading.Tasks;
using Kysect.BotFramework.Data;
using Kysect.BotFramework.Data.Entities;

namespace Kysect.BotFramework.Core.Contexts
{
    public class DialogContext
    {
        private readonly BotFrameworkDbContext _dbContext;
        private readonly long _senderInfoId;

        private int? _state;

        public SenderInfo SenderInfo { get; }

        public DialogContext(SenderInfo senderInfo)
            => SenderInfo = senderInfo;

        public DialogContext(int state, long senderInfoId, SenderInfo senderInfo, BotFrameworkDbContext dbContext)
            : this(senderInfo)
        {
            _senderInfoId = senderInfoId;
            _state = state;
            _dbContext = dbContext;
        }

        public int? GetState() => _state;

        public async Task SetStateAsync(int state)
        {
            _state = state;

            if (_dbContext is not null)
            {
                await SaveChangesAsync();
            }
        }

        private async Task SaveChangesAsync()
        {
            DialogContextEntity context = _dbContext.DialogContexts.FirstOrDefault(
                x =>
                    x.SenderInfoId == _senderInfoId
                    && x.ContextType == SenderInfo.ContextType);

            context!.State = _state!.Value;
            _dbContext.DialogContexts.Update(context);
            await _dbContext.SaveChangesAsync();
        }
    }
}