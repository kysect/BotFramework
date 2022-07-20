using System.Linq;
using Kysect.BotFramework.Abstractions.Contexts;
using Kysect.BotFramework.Data;
using Kysect.BotFramework.Data.Entities;

namespace Kysect.BotFramework.Core.Contexts
{
    public class DialogContext : IDialogContext
    {
        private readonly BotFrameworkDbContext _dbContext;
        
        private readonly long _senderInfoId;
        private readonly ContextType _contextType;

        private int _state;

        public ISenderInfo SenderInfo { get; }

        public int State
        {
            get => _state;
            set
            {
                _state = value;
                SaveChanges();
            }
        }

        public DialogContext(
            int state,
            long senderInfoId,
            SenderInfo senderInfo,
            ContextType contextType,
            BotFrameworkDbContext dbContext)
        {
            SenderInfo = senderInfo;
            _senderInfoId = senderInfoId;
            _state = state;
            _contextType = contextType;
            _dbContext = dbContext;
        }

        private void SaveChanges()
        {
            DialogContextEntity context = _dbContext.DialogContexts.FirstOrDefault(
                x =>
                    x.SenderInfoId == _senderInfoId
                    && x.ContextType == _contextType);

            context!.State = State;
            _dbContext.DialogContexts.Update(context);
            _dbContext.SaveChanges();
        }
    }
}