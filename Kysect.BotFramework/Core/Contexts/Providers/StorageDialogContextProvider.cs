﻿using System;
using Kysect.BotFramework.Abstactions.Services;
using Kysect.BotFramework.Abstractions.Contexts;
using Kysect.BotFramework.Abstractions.Visitors;

namespace Kysect.BotFramework.Core.Contexts.Providers;

public class StorageDialogContextProvider : IDialogContextProvider
{
    private readonly IContextVisitor<IDialogContext> _dialogContextVisitor;

    public StorageDialogContextProvider(IContextVisitor<IDialogContext> dialogContextVisitor)
    {
        _dialogContextVisitor = dialogContextVisitor;
    }

    public IDialogContext GetDialogContext(ISenderInfo senderInfo)
    {
        if (senderInfo is null)
            throw new ArgumentNullException(nameof(senderInfo));

        return senderInfo.Accept(_dialogContextVisitor);
    }
}