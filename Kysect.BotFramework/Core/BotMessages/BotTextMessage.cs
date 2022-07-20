using System.Threading.Tasks;
using Kysect.BotFramework.Abstractions.BotMessages;
using Kysect.BotFramework.Abstractions.Contexts;
using Kysect.BotFramework.ApiProviders;

namespace Kysect.BotFramework.Core.BotMessages
{
    public class BotTextMessage : IBotMessage
    {
        public string Text { get; }

        public BotTextMessage(string text)
        {
            Text = text;
        }
        
        public async Task SendAsync(IBotApiProvider apiProvider, ISenderInfo sender)
        {
            await apiProvider.SendTextMessageAsync(Text, sender);
        }
    }
}