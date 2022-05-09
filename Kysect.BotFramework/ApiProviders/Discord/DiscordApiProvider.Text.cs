using System;
using System.Threading.Tasks;
using Kysect.BotFramework.Core.Contexts;
using Kysect.BotFramework.Core.Tools;
using Kysect.BotFramework.Core.Tools.Loggers;

namespace Kysect.BotFramework.ApiProviders.Discord;

public partial class DiscordApiProvider
{
    public async Task<Result> SendTextMessageAsync(string text, SenderInfo sender)
    {
        Result result = CheckText(text);
        if (result.IsFailed)
        {
            return result;
        }

        return await SendTextAsync(text, sender);

    }
    
    private async Task<Result> SendTextAsync(string text, SenderInfo sender)
    {
        var discordSender = (DiscordSenderInfo)sender;
    
        try
        { 
            await _client.GetGuild(discordSender.GuildId)
                .GetTextChannel((ulong) sender.ChatId)
                .SendMessageAsync(text);
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
        return text.Length switch
        {
            > 2000 => Result.Fail("The message wasn't sent by the command, the length is too big."),
            0 => Result.Fail("The message wasn't sent by the command, the length must not be zero"),
            _ => Result.Ok()
        };
    }
}