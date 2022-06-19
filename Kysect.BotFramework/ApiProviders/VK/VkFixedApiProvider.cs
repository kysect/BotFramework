using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.Core.BotMedia;
using Kysect.BotFramework.Core.Contexts;
using Kysect.BotFramework.Core.Tools;

namespace Kysect.BotFramework.ApiProviders.VK
{
    public class VkFixedApiProvider : IBotApiProvider, IDisposable
    {
        public event EventHandler<BotEventArgs> OnMessage;
        public void Restart()
        {
            throw new NotImplementedException();
        }

        public Task<Result> SendMultipleMediaAsync(List<IBotMediaFile> mediaFiles, string text, SenderInfo sender) => throw new NotImplementedException();

        public Task<Result> SendMediaAsync(IBotMediaFile mediaFile, string text, SenderInfo sender) => throw new NotImplementedException();

        public Task<Result> SendOnlineMediaAsync(IBotOnlineFile file, string text, SenderInfo sender) => throw new NotImplementedException();

        public Task<Result> SendTextMessageAsync(string text, SenderInfo sender) => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}