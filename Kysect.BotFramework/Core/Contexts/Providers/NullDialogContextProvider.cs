namespace Kysect.BotFramework.Core.Contexts.Providers;

public class NullDialogContextProvider : IDialogContextProvider
{
    public SenderInfo SenderInfo { get; set; }

    public DialogContext GetDialogContext() => new(SenderInfo);
}
