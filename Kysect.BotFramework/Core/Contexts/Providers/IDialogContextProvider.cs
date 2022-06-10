using System.Threading.Tasks;

namespace Kysect.BotFramework.Core.Contexts.Providers;

public interface IDialogContextProvider
{
    SenderInfo SenderInfo { get; set; }

    DialogContext GetDialogContext();

    Task SaveChangesAsync();
}
