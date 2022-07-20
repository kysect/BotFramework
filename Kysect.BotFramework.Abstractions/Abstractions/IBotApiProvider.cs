using Kysect.BotFramework.Abstractions;
using Kysect.BotFramework.Abstractions.BotMedia;
using Kysect.BotFramework.Abstractions.Contexts;
using Kysect.BotFramework.Tools;

namespace Kysect.BotFramework.ApiProviders;

public interface IBotApiProvider
{
    event EventHandler<IBotEventArgs> OnMessage;

    void Restart();
    Task<Result> SendMultipleMediaAsync(List<IBotMediaFile> mediaFiles, string text, ISenderInfo sender);
    Task<Result> SendMediaAsync(IBotMediaFile mediaFile, string text, ISenderInfo sender);
    Task<Result> SendTextMessageAsync(string text, ISenderInfo sender);
}