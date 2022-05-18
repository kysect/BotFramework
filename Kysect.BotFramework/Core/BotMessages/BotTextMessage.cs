using System.Threading.Tasks;
using Kysect.BotFramework.ApiProviders;
using Kysect.BotFramework.Core.Contexts;

namespace Kysect.BotFramework.Core.BotMessages
{
    public class BotTextMessage : IBotMessage
    {
        public string Text { get; }

        public BotTextMessage(string text)
        {
            Text = text;
        }
        
        public async Task SendAsync(IBotApiProvider apiProvider, SenderInfo sender)
        {
            await apiProvider.SendTextMessageAsync(Text, sender);
        }
    }
}