using System.Linq;
using Kysect.BotFramework.Data;
using Kysect.BotFramework.Data.Entities;

namespace Kysect.BotFramework.Core.Contexts
{
    public class DialogContext
    {
        private readonly BotFrameworkDbContext _dbContext;
        
        private readonly long _senderInfoId;

        private int _state;

        public SenderInfo SenderInfo { get; }

        public int State
        {
            get => _state;
            set
            {
                _state = value;
                SaveChanges();
            }
        }

        public DialogContext(int state, long senderInfoId, SenderInfo senderInfo, BotFrameworkDbContext dbContext)
        {
            SenderInfo = senderInfo;
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

            context!.State = State;
            _dbContext.DialogContexts.Update(context);
            _dbContext.SaveChanges();
        }
    }
}