using Kysect.BotFramework.Abstractions.BotMedia;
using Kysect.BotFramework.Enums;

namespace Kysect.BotFramework.Core.BotMedia
{
    public class BotVideoFile : IBotMediaFile
    {
        public BotVideoFile(string path)
        {
            Path = path;
        }

        public MediaType MediaType => MediaType.Video;
        public string Path { get; }
    }
}