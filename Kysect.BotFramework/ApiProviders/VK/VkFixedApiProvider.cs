using System;
using System.Collections.Generic;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.Core.BotMedia;
using Kysect.BotFramework.Core.Contexts;
using Kysect.BotFramework.Core.Tools;

namespace Kysect.BotFramework.ApiProviders.VK
{
    public class VkFixedApiProvider : IBotApiProvider, IDisposable
    {
        public event EventHandler<BotNewMessageEventArgs> OnMessage;
        public void Restart()
        {
            throw new NotImplementedException();
        }

        public Result SendMultipleMedia(List<IBotMediaFile> mediaFiles, string text, SenderInfo sender) => throw new NotImplementedException();

        public Result SendMedia(IBotMediaFile mediaFile, string text, SenderInfo sender) => throw new NotImplementedException();

        public Result SendOnlineMedia(IBotOnlineFile file, string text, SenderInfo sender) => throw new NotImplementedException();

        public Result SendTextMessage(string text, SenderInfo sender) => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}