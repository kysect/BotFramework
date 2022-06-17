using Kysect.BotFramework.Data;

namespace Kysect.BotFramework.Core.Contexts.Providers;

public class StorageDialogContextProvider : IDialogContextProvider
{
    private readonly BotFrameworkDbContext _dbContext;
    private readonly SenderInfoProvider _senderInfoProvider;

    public StorageDialogContextProvider(BotFrameworkDbContext dbContext,
        SenderInfoProvider senderInfoProvider)
    {
        _dbContext = dbContext;
        _senderInfoProvider = senderInfoProvider;
    }

    public SenderInfo SenderInfo => _senderInfoProvider.SenderInfo;

    public DialogContext GetDialogContext()
        => SenderInfo.GetOrCreateDialogContext(_dbContext);
}
