using Kysect.BotFramework.Data;

namespace Kysect.BotFramework.Core.Contexts.Providers;

public class StorageDialogContextProvider : IDialogContextProvider
{
    private readonly BotFrameworkDbContext _dbContext;

    public StorageDialogContextProvider(BotFrameworkDbContext dbContext)
        => _dbContext = dbContext;

    public DialogContext GetDialogContext(SenderInfo senderInfo)
        => senderInfo.GetOrCreateDialogContext(_dbContext);
}
