using System.Collections.Generic;
using System.Linq;
using Kysect.BotFramework.Abstractions;
using Kysect.BotFramework.Abstractions.BotMedia;
using Kysect.BotFramework.Abstractions.BotMessages;
using Kysect.BotFramework.Abstractions.Contexts;
using Kysect.BotFramework.Core.BotMessages;

namespace Kysect.BotFramework.Core;

public class BotEventArgs : IBotEventArgs
{
    public IBotMessage Message { get; }
    public ISenderInfo SenderInfo { get; }

    public BotEventArgs(IBotMessage message, ISenderInfo senderInfo)
    {
        Message = message;
        SenderInfo = senderInfo;
    }

    public string FindCommandName()
    {
        if (Message.Text is null)
        {
            return string.Empty;
        }

        return Message.Text.Split().FirstOrDefault();
    }

    public List<string> GetCommandArguments() => Message.Text.Split().Skip(1).ToList();

    public List<IBotMediaFile> GetMediaFiles()
    {
        if (Message is BotSingleMediaMessage singleMediaMessage)
        {
            return new List<IBotMediaFile> { singleMediaMessage.MediaFile };
        }

        if (Message is BotMultipleMediaMessage multipleMediaMessage)
        {
            return multipleMediaMessage.MediaFiles;
        }

        return new List<IBotMediaFile>();
    }
}