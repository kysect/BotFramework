namespace Kysect.BotFramework.Core.Contexts.Providers;

public class NullDialogContextProvider : IDialogContextProvider
{
    private readonly SenderInfoProvider _senderInfoProvider;

    public NullDialogContextProvider(SenderInfoProvider senderInfoProvider)
        => _senderInfoProvider = senderInfoProvider;

    public SenderInfo SenderInfo => _senderInfoProvider.SenderInfo;

    public DialogContext GetDialogContext() => new(SenderInfo);
}
