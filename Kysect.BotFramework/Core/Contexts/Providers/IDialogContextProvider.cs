namespace Kysect.BotFramework.Core.Contexts.Providers;

public interface IDialogContextProvider
{
    SenderInfo SenderInfo { get; }

    DialogContext GetDialogContext();
}
