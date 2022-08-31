using Kysect.BotFramework.Abstractions.Contexts;

namespace Kysect.BotFramework.Abstractions.Visitors;

public interface IContextVisitor<out TContext>
{
    TContext Visit(IDiscordSenderInfo senderInfo);

    TContext Visit(ITelegramSenderInfo senderInfo);
}