using System;
using Kysect.BotFramework.Abstactions.Services;
using Kysect.BotFramework.Abstractions.Contexts;
using Kysect.BotFramework.Abstractions.Visitors;

namespace Kysect.BotFramework.Core.Contexts.Providers;

public class StorageDialogContextProvider : IDialogContextProvider
{
    private readonly ISenderInfoVisitor<IDialogContext> _dialogSenderInfoVisitor;

    public StorageDialogContextProvider(ISenderInfoVisitor<IDialogContext> dialogSenderInfoVisitor)
    {
        _dialogSenderInfoVisitor = dialogSenderInfoVisitor;
    }

    public IDialogContext GetDialogContext(ISenderInfo senderInfo)
    {
        if (senderInfo is null)
            throw new ArgumentNullException(nameof(senderInfo));

        return senderInfo.Accept(_dialogSenderInfoVisitor);
    }
}