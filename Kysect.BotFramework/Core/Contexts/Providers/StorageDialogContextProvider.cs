using Kysect.BotFramework.Abstactions.Services;
using Kysect.BotFramework.Abstractions.Contexts;
using Kysect.BotFramework.Data;

namespace Kysect.BotFramework.Core.Contexts.Providers;

public class StorageDialogContextProvider : IDialogContextProvider
{
    private readonly BotFrameworkDbContext _dbContext;

    public StorageDialogContextProvider(BotFrameworkDbContext dbContext)
        => _dbContext = dbContext;

    public IDialogContext GetDialogContext(ISenderInfo senderInfo)
        => (senderInfo as SenderInfo).GetOrCreateDialogContext(_dbContext);
}