﻿namespace Kysect.BotFramework.Core.BotMedia
{
    public class BotOnlineVideoFile : IBotOnlineFile
    {
        public BotOnlineVideoFile(string path)
        {
            Path = path;
        }

        public BotOnlineVideoFile(string path, string id)
        {
            Path = path;
            Id = id;
        }

        public MediaTypeEnum MediaType => MediaTypeEnum.Video;
        public string Path { get; }
        public string Id { get; }
    }
}