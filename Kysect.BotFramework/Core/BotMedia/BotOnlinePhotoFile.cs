using Kysect.BotFramework.Abstractions.BotMedia;
using Kysect.BotFramework.Enums;

namespace Kysect.BotFramework.Core.BotMedia
{
    public class BotOnlinePhotoFile : IBotOnlineFile
    {
        public BotOnlinePhotoFile(string path)
        {
            Path = path;
        }

        public BotOnlinePhotoFile(string path, string id)
        {
            Path = path;
            Id = id;
        }

        public MediaType MediaType => MediaType.Photo;
        public string Path { get; }
        public string Id { get; }
    }
}