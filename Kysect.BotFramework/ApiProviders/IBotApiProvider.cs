using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.Core.BotMedia;
using Kysect.BotFramework.Core.Contexts;
using Kysect.BotFramework.Core.Tools;

namespace Kysect.BotFramework.ApiProviders
{
    public interface IBotApiProvider
    {
        event EventHandler<BotNewMessageEventArgs> OnMessage;

        void Restart();
        Task<Result> SendMultipleMediaAsync(List<IBotMediaFile> mediaFiles, string text, SenderInfo sender);
        Task<Result> SendMediaAsync(IBotMediaFile mediaFile, string text, SenderInfo sender);
        Task<Result> SendOnlineMediaAsync(IBotOnlineFile file, string text, SenderInfo sender);
        Task<Result> SendTextMessageAsync(string text, SenderInfo sender);
    }
}