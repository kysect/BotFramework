namespace Kysect.BotFramework.Abstractions.Contexts;

public interface IDialogContext
{
    ISenderInfo SenderInfo { get; }

    int State { get; set; }
}