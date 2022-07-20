using Kysect.BotFramework.Enums;

namespace Kysect.BotFramework.Abstractions.BotMedia;

public interface IBotMediaFile
{
    MediaType MediaType { get; }
    string Path { get; }
}