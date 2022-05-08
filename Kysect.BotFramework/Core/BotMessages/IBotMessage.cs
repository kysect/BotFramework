using System.Threading.Tasks;
using Kysect.BotFramework.ApiProviders;
using Kysect.BotFramework.Core.Contexts;

namespace Kysect.BotFramework.Core.BotMessages
{
    public interface IBotMessage
    {
        string Text { get; }

        Task SendAsync(IBotApiProvider apiProvider, SenderInfo sender);
    }
}