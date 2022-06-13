using Kysect.BotFramework.Data;

namespace Kysect.BotFramework.Core.Contexts.Providers;

public class StorageDialogContextProvider : IDialogContextProvider
{
    private readonly BotFrameworkDbContext _dbContext;

    private SenderInfo _senderInfo;
    private DialogContext _currentSenderDialogContext;

    public StorageDialogContextProvider(BotFrameworkDbContext dbContext)
        => _dbContext = dbContext;

    public SenderInfo SenderInfo
    {
        get => _senderInfo;
        set
        {
            _senderInfo = value;
            _currentSenderDialogContext = _senderInfo.GetOrCreateDialogContext(_dbContext);
        }
    }

    public DialogContext GetDialogContext() => _currentSenderDialogContext;
}
