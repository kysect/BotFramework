using System;
using System.Threading.Tasks;
using Kysect.BotFramework.Core.Contexts;
using Kysect.BotFramework.Core.Tools;
using Kysect.BotFramework.Core.Tools.Loggers;
using Telegram.Bot.Types;

namespace Kysect.BotFramework.ApiProviders.Telegram;

public partial class TelegramApiProvider : IBotApiProvider, IDisposable
{
    public async Task<Result> SendTextMessageAsync(string text, SenderInfo sender)
    {
        if (text.Length == 0)
        {
            LoggerHolder.Instance.Error("The message wasn't sent by the command, the length must not be zero.");
            return Result.Ok();
        }

        return await SendText(text, sender);
    }

    private async Task<Result> SendText(string text, SenderInfo sender)
    {
        Result result = CheckText(text);
        if (result.IsFailed)
        {
            return result;
        }
            
        try
        {
            Message message = await _client.SendTextMessageAsync(sender.ChatId, text);
            return Result.Ok();
        }
        catch (Exception e)
        {
            const string message = "Error while sending message";
            LoggerHolder.Instance.Error(e, message);
            return Result.Fail(e.Message);
        }
    }

    private Result CheckText(string text)
    {
        if (text.Length > 4096)
        {
            string errorMessage = "The message wasn't sent by the command, the length is too big.";
            return Result.Fail(errorMessage);
        }

        return Result.Ok();
    }
}