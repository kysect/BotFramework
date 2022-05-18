using System.Threading.Tasks;
using Kysect.BotFramework.ApiProviders;
using Kysect.BotFramework.Core.BotMedia;
using Kysect.BotFramework.Core.Contexts;

namespace Kysect.BotFramework.Core.BotMessages
{
    public class BotSingleMediaMessage : IBotMessage
    {
        public string Text { get; }
        public IBotMediaFile MediaFile { get; }

        public BotSingleMediaMessage(string text, IBotMediaFile mediaFile)
        {
            Text = text;
            MediaFile = mediaFile;
        }

        public async Task SendAsync(IBotApiProvider apiProvider, SenderInfo sender)
        {
            await apiProvider.SendMediaAsync(MediaFile, Text, sender);
        }
    }
}