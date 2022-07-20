using Kysect.BotFramework.Abstractions.Contexts;

namespace Kysect.BotFramework.Abstactions.Services;

public interface IDialogContextProvider
{
    public IDialogContext GetDialogContext(ISenderInfo senderInfo);
}