using System.Collections.Generic;
using System.Threading.Tasks;
using Kysect.BotFramework.ApiProviders;
using Kysect.BotFramework.Core.BotMedia;
using Kysect.BotFramework.Core.Contexts;

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
        
        public async Task SendAsync(IBotApiProvider apiProvider, SenderInfo sender)
        {
            await apiProvider.SendMultipleMediaAsync(MediaFiles, Text, sender);
        }
    }
}