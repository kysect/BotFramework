namespace Kysect.BotFramework.Core.Contexts.Providers;

public interface IDialogContextProvider
{
    DialogContext GetDialogContext(SenderInfo senderInfo);
}
