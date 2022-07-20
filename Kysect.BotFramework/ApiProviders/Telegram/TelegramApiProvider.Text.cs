using System;
using System.Threading.Tasks;
using Kysect.BotFramework.Abstractions.Contexts;
using Kysect.BotFramework.Core.Tools.Loggers;
using Kysect.BotFramework.Tools;

namespace Kysect.BotFramework.ApiProviders.Telegram;

public partial class TelegramApiProvider
{
    public async Task<Result> SendTextMessageAsync(string text, ISenderInfo sender)
    {
        

        return await SendText(text, sender);
    }

    private async Task<Result> SendText(string text, ISenderInfo sender)
    {
        Result result = CheckText(text);
        if (result.IsFailed)
        {
            return result;
        }
            
        try
        {
            await _client.SendTextMessageAsync(sender.ChatId, text);
            return Result.Ok();
        }
        catch (Exception e)
        {
            const string message = "Error while sending message";
            LoggerHolder.Instance.Error(e, message);
            return Result.Fail(e);
        }
    }

    private Result CheckText(string text)
    {
        return text.Length switch
        {
            > 4096 => Result.Fail("The message wasn't sent by the command, the length is too big."),
            0 => Result.Fail("The message wasn't sent by the command, the length must not be zero"),
            _ => Result.Ok()
        };
    }
}