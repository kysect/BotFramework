using Kysect.BotFramework.Abstractions.BotMedia;
using Kysect.BotFramework.Abstractions.BotMessages;
using Kysect.BotFramework.Abstractions.Contexts;

namespace Kysect.BotFramework.Abstractions;

public interface IBotEventArgs
{
    IBotMessage Message { get; }
    ISenderInfo SenderInfo { get; }

    string FindCommandName();

    List<string> GetCommandArguments();

    List<IBotMediaFile> GetMediaFiles();
}