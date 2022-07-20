using System.Collections.Generic;
using System.Threading.Tasks;
using Kysect.BotFramework.Abstractions.BotMedia;
using Kysect.BotFramework.Abstractions.BotMessages;
using Kysect.BotFramework.Abstractions.Contexts;
using Kysect.BotFramework.ApiProviders;

namespace Kysect.BotFramework.Core.BotMessages
{
    public class BotMultipleMediaMessage : IBotMessage
    {
        public string Text { get; }
        public List<IBotMediaFile> MediaFiles { get; }

        public BotMultipleMediaMessage(string text, List<IBotMediaFile> mediaFiles)
        {
            Text = text;
            MediaFiles = mediaFiles;
        }
        
        public async Task SendAsync(IBotApiProvider apiProvider, ISenderInfo sender)
        {
            await apiProvider.SendMultipleMediaAsync(MediaFiles, Text, sender);
        }
    }
}