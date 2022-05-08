using System.Threading.Tasks;
using Kysect.BotFramework.ApiProviders;
using Kysect.BotFramework.Core.Contexts;

namespace Kysect.BotFramework.Core.BotMessages
{
    public class BotTextMessage : IBotMessage
    {
        public BotTextMessage(string text)
        {
            Text = text;
        }

        public string Text { get; }

        public async Task SendAsync(IBotApiProvider apiProvider, SenderInfo sender)
        {
            await apiProvider.SendTextMessageAsync(Text, sender);
        }
    }
}