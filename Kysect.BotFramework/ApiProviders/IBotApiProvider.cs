using System;
using System.Collections.Generic;
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
        Result SendMultipleMedia(List<IBotMediaFile> mediaFiles, string text, SenderInfo sender);
        Result SendMedia(IBotMediaFile mediaFile, string text, SenderInfo sender);
        Result SendOnlineMedia(IBotOnlineFile file, string text, SenderInfo sender);
        Result SendTextMessage(string text, SenderInfo sender);
    }
}