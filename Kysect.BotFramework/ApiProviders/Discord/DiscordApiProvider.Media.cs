using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kysect.BotFramework.Abstractions.BotMedia;
using Kysect.BotFramework.Abstractions.Contexts;
using Kysect.BotFramework.Core.Tools.Loggers;
using Kysect.BotFramework.Tools;

namespace Kysect.BotFramework.ApiProviders.Discord;

public partial class DiscordApiProvider
{
    public async Task<Result> SendMultipleMediaAsync(List<IBotMediaFile> mediaFiles, string text, ISenderInfo sender)
    {
        var result = await SendMediaAsync(mediaFiles.First(), text, sender);

        foreach (var media in mediaFiles.Skip(1))
        {
            if (result.IsFailed)
            {
                return result;
            }

            result = await SendMediaAsync(media, string.Empty, sender);
        }

        return result;
    }

    public async Task<Result> SendMediaAsync(IBotMediaFile mediaFile, string text, ISenderInfo sender)
    {
        if (mediaFile is IBotOnlineFile onlineFile)
            return await SendOnlineMediaAsync(onlineFile, text, sender);
        
        Result result = CheckText(text);
        if (result.IsFailed)
        {
            return result;
        }
        
        try
        {
            await _client.GetGuild((ulong) sender.ChatId)
                .GetTextChannel((ulong) sender.UserSenderId)
                .SendFileAsync(mediaFile.Path, text);
            return Result.Ok();
        }
        catch (Exception e)
        {
            const string message = "Error while sending message";
            LoggerHolder.Instance.Error(e, message);
            return Result.Fail(e);
        }
    }

    private async Task<Result> SendOnlineMediaAsync(IBotOnlineFile file, string text, ISenderInfo sender)
    {
        if (text.Length != 0)
        {
            Result result = await SendTextMessageAsync(text, sender);
            if (result.IsFailed)
            {
                return result;
            }
        }

        return await SendTextAsync(file.Path, sender);
    }
}