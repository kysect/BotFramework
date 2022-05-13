using System.Linq;
using Kysect.BotFramework.Core.Contexts;
using Kysect.BotFramework.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kysect.BotFramework.Data;

public static class DialogContextDbSetExtensions
{
    public static DialogContextEntity GetOrCreateDialogContext(
        this BotFrameworkDbContext context,
        long senderInfoId,
        ContextType type)
    {
        DialogContextEntity contextModel = context.DialogContexts.FirstOrDefault(
            c => 
                c.SenderInfoId == senderInfoId 
                && c.ContextType == type);

        if (contextModel is null)
        {
            contextModel = new DialogContextEntity(type,senderInfoId);
            context.Add(contextModel);
            context.SaveChanges();
        }

        return contextModel;
    }
}