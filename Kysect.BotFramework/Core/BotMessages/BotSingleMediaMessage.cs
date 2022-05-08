using System.Threading.Tasks;
using Kysect.BotFramework.ApiProviders;
using Kysect.BotFramework.Core.BotMedia;
using Kysect.BotFramework.Core.Contexts;

namespace Kysect.BotFramework.Core.BotMessages
{
    public class BotSingleMediaMessage : IBotMessage
    {
        public IBotMediaFile MediaFile { get; }

        public BotSingleMediaMessage(string text, IBotMediaFile mediaFile)
        {
            Text = text;
            MediaFile = mediaFile;
        }

        public string Text { get; }

        public async Task SendAsync(IBotApiProvider apiProvider, SenderInfo sender)
        {
            if (MediaFile is IBotOnlineFile onlineFile)
            {
                await apiProvider.SendOnlineMediaAsync(onlineFile, Text, sender);
            }
            else
            {
                await apiProvider.SendMediaAsync(MediaFile, Text, sender);
            }
        }
    }
}