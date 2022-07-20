using Kysect.BotFramework.Abstractions.Contexts;
using Kysect.BotFramework.ApiProviders;

namespace Kysect.BotFramework.Abstractions.BotMessages;

public interface IBotMessage
{
    string Text { get; }

    Task SendAsync(IBotApiProvider apiProvider, ISenderInfo sender);
}