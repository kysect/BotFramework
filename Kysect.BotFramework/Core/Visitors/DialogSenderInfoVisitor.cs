using Kysect.BotFramework.Abstractions.Contexts;
using Kysect.BotFramework.Abstractions.Visitors;
using Kysect.BotFramework.Core.Contexts;
using Kysect.BotFramework.Data;
using Kysect.BotFramework.Data.Entities;

namespace Kysect.BotFramework.Core.Visitors;

public class DialogSenderInfoVisitor : ISenderInfoVisitor<IDialogContext>
{
    private readonly BotFrameworkDbContext _dbContext;

    public DialogSenderInfoVisitor(BotFrameworkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IDialogContext Visit(IDiscordSenderInfo senderInfo)
    {
        var contextSenderInfo = DiscordSenderInfoEntity.GetOrCreate(senderInfo, _dbContext);
        var contextModel = DialogContextEntity.GetOrCreate(contextSenderInfo, ContextType.Discord, _dbContext);

        return new DialogContext(contextModel.State, contextModel.SenderInfoId, ContextType.Discord, _dbContext);
    }

    public IDialogContext Visit(ITelegramSenderInfo senderInfo)
    {
        var contextSenderInfo = TelegramSenderInfoEntity.GetOrCreate(senderInfo, _dbContext);
        var contextModel = DialogContextEntity.GetOrCreate(contextSenderInfo, ContextType.Telegram, _dbContext);

        return new DialogContext(contextModel.State, contextModel.SenderInfoId, ContextType.Telegram, _dbContext);
    }
}