using System.Threading.Tasks;
using Kysect.BotFramework.Abstractions.BotMedia;
using Kysect.BotFramework.Abstractions.BotMessages;
using Kysect.BotFramework.Abstractions.Contexts;
using Kysect.BotFramework.ApiProviders;

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

        public async Task SendAsync(IBotApiProvider apiProvider, ISenderInfo sender)
        {
            await apiProvider.SendMediaAsync(MediaFile, Text, sender);
        }
    }
}