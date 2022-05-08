using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Rest;
using Kysect.BotFramework.Core.BotMedia;
using Kysect.BotFramework.Core.Contexts;
using Kysect.BotFramework.Core.Tools;
using Kysect.BotFramework.Core.Tools.Loggers;

namespace Kysect.BotFramework.ApiProviders.Discord;

public partial class DiscordApiProvider : IBotApiProvider, IDisposable
{
    public async Task<Result> SendMultipleMediaAsync(List<IBotMediaFile> mediaFiles, string text, SenderInfo sender)
    {
        Result result;
        if (mediaFiles.First() is IBotOnlineFile onlineFile)
        {
            result = await SendOnlineMediaAsync(onlineFile, text, sender);
        }
        else
        {
            result = await SendMediaAsync(mediaFiles.First(), text, sender);
        }
        
        foreach (IBotMediaFile media in mediaFiles.Skip(1))
        {
            if (result.IsFailed)
            {
                return result;
            }

            if (media is IBotOnlineFile onlineMediaFile)
            {
                result = await SendOnlineMediaAsync(onlineMediaFile, string.Empty, sender);
            }
            else
            {
                result = await SendMediaAsync(media, string.Empty, sender);
            }
        }

        return result;
    }

    public async Task<Result> SendMediaAsync(IBotMediaFile mediaFile, string text, SenderInfo sender)
    {
        Result result = CheckText(text);
        if (result.IsFailed)
        {
            return result;
        }
        
        try
        {
            RestUserMessage message = await _client.GetGuild((ulong) sender.ChatId)
                .GetTextChannel((ulong) sender.UserSenderId)
                .SendFileAsync(mediaFile.Path, text);
            return Result.Ok();
        }
        catch (Exception e)
        {
            const string message = "Error while sending message";
            LoggerHolder.Instance.Error(e, message);
            return Result.Fail(e.Message);
        }
    }

    public async Task<Result> SendOnlineMediaAsync(IBotOnlineFile file, string text, SenderInfo sender)
    {
        if (text.Length != 0)
        {
            Result result = await SendTextAsync(text, sender);
            if (result.IsFailed)
            {
                return result;
            }
        }

        return await SendTextAsync(file.Path, sender);
    }
}