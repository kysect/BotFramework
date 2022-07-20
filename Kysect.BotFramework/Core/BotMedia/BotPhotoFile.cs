using Kysect.BotFramework.Abstractions.BotMedia;
using Kysect.BotFramework.Enums;

namespace Kysect.BotFramework.Core.BotMedia
{
    public class BotPhotoFile : IBotMediaFile
    {
        public BotPhotoFile(string path)
        {
            Path = path;
        }

        public MediaType MediaType => MediaType.Photo;
        public string Path { get; }
    }
}