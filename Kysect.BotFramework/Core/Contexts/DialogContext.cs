using System.Linq;
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

        public int? State
        {
            get => _state;
            set
            {
                _state = value;

                if (_dbContext is not null)
                {
                    SaveChanges();
                }
            }
        }

        public DialogContext(SenderInfo senderInfo)
            => SenderInfo = senderInfo;

        public DialogContext(int state, long senderInfoId, SenderInfo senderInfo, BotFrameworkDbContext dbContext)
            : this(senderInfo)
        {
            _senderInfoId = senderInfoId;
            _state = state;
            _dbContext = dbContext;
        }

        private void SaveChanges()
        {
            DialogContextEntity context = _dbContext.DialogContexts.FirstOrDefault(
                x =>
                    x.SenderInfoId == _senderInfoId
                    && x.ContextType == SenderInfo.ContextType);

            context!.State = State.GetValueOrDefault();
            _dbContext.DialogContexts.Update(context);
            _dbContext.SaveChanges();
        }
    }
}