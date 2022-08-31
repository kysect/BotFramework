using Kysect.BotFramework.Abstractions.Contexts;

namespace Kysect.BotFramework.Abstactions.Services;

public interface IDialogContextProvider
{
    IDialogContext GetDialogContext(ISenderInfo senderInfo);
}