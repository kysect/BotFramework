namespace Kysect.BotFramework.Abstractions.BotMedia;

public interface IBotOnlineFile : IBotMediaFile
{
    string Id { get; }
}