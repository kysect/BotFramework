using Kysect.BotFramework.Abstractions.BotMedia;
using Kysect.BotFramework.Abstractions.Contexts;

namespace Kysect.BotFramework.Abstractions.Commands;

public interface ICommandContainer
{
    string CommandName { get; }
    List<IBotMediaFile> MediaFiles { get; }
    ISenderInfo SenderInfo { get; }

    bool StartsWithPrefix(char prefix);

    ICommandContainer RemovePrefix(char prefix);

    string ToString();
}