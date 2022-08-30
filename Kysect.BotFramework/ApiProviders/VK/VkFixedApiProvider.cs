using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kysect.BotFramework.Abstractions;
using Kysect.BotFramework.Abstractions.BotMedia;
using Kysect.BotFramework.Abstractions.Contexts;
using Kysect.BotFramework.Tools;

namespace Kysect.BotFramework.ApiProviders.VK
{
    public class VkFixedApiProvider : IBotApiProvider, IDisposable
    {
        public event EventHandler<IBotEventArgs> OnMessage;
        public void Restart()
        {
            throw new NotImplementedException();
        }

        public Task<Result> SendMultipleMediaAsync(List<IBotMediaFile> mediaFiles, string text, ISenderInfo sender) => throw new NotImplementedException();

        public Task<Result> SendMediaAsync(IBotMediaFile mediaFile, string text, ISenderInfo sender) => throw new NotImplementedException();

        public Task<Result> SendOnlineMediaAsync(IBotOnlineFile file, string text, ISenderInfo sender) => throw new NotImplementedException();

        public Task<Result> SendTextMessageAsync(string text, ISenderInfo sender) => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}