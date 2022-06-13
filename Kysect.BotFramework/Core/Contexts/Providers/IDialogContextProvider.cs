namespace Kysect.BotFramework.Core.Contexts.Providers;

public interface IDialogContextProvider
{
    SenderInfo SenderInfo { get; set; }

    DialogContext GetDialogContext();
}
