using System;
using System.Threading.Tasks;
using Discord.Rest;
using Kysect.BotFramework.Core.Contexts;
using Kysect.BotFramework.Core.Tools;
using Kysect.BotFramework.Core.Tools.Loggers;

namespace Kysect.BotFramework.ApiProviders.Discord;

public partial class DiscordApiProvider : IBotApiProvider, IDisposable
{
    public async Task<Result> SendTextMessageAsync(string text, SenderInfo sender)
    {
        if (text.Length == 0)
        {
            LoggerHolder.Instance.Error("The message wasn't sent by the command, the length must not be zero.");
            return Result.Ok();
        }

        return await SendTextAsync(text, sender);
    }
    
    private async Task<Result> SendTextAsync(string text, SenderInfo sender)
    {
        Result result = CheckText(text);
        if (result.IsFailed)
        {
            return result;
        }

        var discordSender = (DiscordSenderInfo)sender;
    
        try
        {
            RestUserMessage message = await _client.GetGuild(discordSender.GuildId)
                .GetTextChannel((ulong) sender.ChatId)
                .SendMessageAsync(text);
            return Result.Ok();
        }
        catch (Exception e)
        {
            var message = "Error while sending message";
            LoggerHolder.Instance.Error(e, message);
            return Result.Fail(e.Message);
        }
    }

    private Result CheckText(string text)
    {
        if (text.Length > 2000)
        {
            string errorMessage = "The message wasn't sent by the command, the length is too big.";
            return Result.Fail(errorMessage);
        }

        return Result.Ok();
    }
}