using Kysect.BotFramework.Abstractions.Contexts;

namespace Kysect.BotFramework.Abstractions.Visitors;

public interface ISenderInfoVisitor<out TContext>
{
    TContext Visit(IDiscordSenderInfo senderInfo);

    TContext Visit(ITelegramSenderInfo senderInfo);
}