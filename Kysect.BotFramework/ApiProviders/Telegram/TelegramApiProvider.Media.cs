using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kysect.BotFramework.Abstractions.BotMedia;
using Kysect.BotFramework.Abstractions.Contexts;
using Kysect.BotFramework.Core.Tools.Extensions;
using Kysect.BotFramework.Core.Tools.Loggers;
using Kysect.BotFramework.Enums;
using Kysect.BotFramework.Tools;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace Kysect.BotFramework.ApiProviders.Telegram;

public partial class TelegramApiProvider
{
    public async Task<Result> SendMultipleMediaAsync(List<IBotMediaFile> mediaFiles, string text, ISenderInfo sender)
    {
        var checkResult = CheckMediaFiles(mediaFiles);
        if (checkResult.IsFailed)
            return checkResult;

        Result result = CheckText(text);
        if (result.IsFailed)
        {
            return result;
        }

        List<FileStream> streams = new List<FileStream>();
        List<IAlbumInputMedia> filesToSend = CollectInputMedia(mediaFiles, text, streams);

        try
        {
            await _client.SendMediaGroupAsync(filesToSend, sender.ChatId);
            return Result.Ok();
        }
        catch (Exception e)
        {
            const string message = "Error while sending message";
            LoggerHolder.Instance.Error(e, message);

            return Result.Fail(e);
        }
        finally
        {
            foreach (var stream in streams)
            {
                stream.Close();
            }
        }
    }
    
    private List<IAlbumInputMedia> CollectInputMedia(List<IBotMediaFile> mediaFiles, string text,
        List<FileStream> streams)
    {
        List<IAlbumInputMedia> filesToSend = new List<IAlbumInputMedia>();
        filesToSend.Add(ConstructAlbumInputMedia(mediaFiles.First(), text, streams));

        foreach (IBotMediaFile mediaFile in mediaFiles.Skip(1))
        {
            filesToSend.Add(ConstructAlbumInputMedia(mediaFile, String.Empty, streams));
        }

        return filesToSend;
    }

    private IAlbumInputMedia ConstructAlbumInputMedia(IBotMediaFile mediaFile, string caption, List<FileStream> streams)
    {
        if (mediaFile is IBotOnlineFile onlineFile)
        {
            return mediaFile.MediaType switch
            {
                MediaType.Photo => new InputMediaPhoto(onlineFile.Id) {Caption = caption},
                MediaType.Video => new InputMediaVideo(onlineFile.Id) {Caption = caption}
            };
        }
        else
        {
            streams.Add(File.Open(mediaFile.Path, FileMode.Open));
            var inputMedia = new InputMedia(streams.Last(),
                mediaFile.GetFileExtension());
            return mediaFile.MediaType switch
            {
                MediaType.Photo => new InputMediaPhoto(inputMedia) {Caption = caption},
                MediaType.Video => new InputMediaVideo(inputMedia) {Caption = caption}
            };
        }
    }

    private Result CheckMediaFiles(List<IBotMediaFile> mediaFiles)
    {
        //TODO: hack
        if (mediaFiles.Count <= 10)
        {
            return Result.Ok();
        }

        const string message = "Too many files provided";
        LoggerHolder.Instance.Error(message);
        return Result.Fail(message);

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

        FileStream stream = File.Open(mediaFile.Path, FileMode.Open);
        var fileToSend = new InputMedia(stream, mediaFile.GetFileExtension());

        try
        {
            if (mediaFile.MediaType == MediaType.Photo)
                await _client.SendPhotoAsync(sender.ChatId, fileToSend, text);
            else if (mediaFile.MediaType == MediaType.Video)
                await _client.SendVideoAsync(sender.ChatId, fileToSend, caption: text);

            return Result.Ok();
        }
        catch (Exception e)
        {
            const string message = "Error while sending message";
            LoggerHolder.Instance.Error(e, message);
            return Result.Fail(e);
        }
        finally
        {
            stream.Close();
        }
    }

    public async Task<Result> SendOnlineMediaAsync(IBotOnlineFile file, string text, ISenderInfo sender)
    {
        Result result = CheckText(text);
        if (result.IsFailed)
        {
            return result;
        }

        string fileIdentifier = file.Id ?? file.Path;
        
        try
        {
            if (file.MediaType == MediaType.Photo)
                await _client.SendPhotoAsync(sender.ChatId, fileIdentifier, text);
            else if (file.MediaType == MediaType.Video)
                await _client.SendVideoAsync(sender.ChatId, fileIdentifier, caption: text);

            return Result.Ok();
        }
        catch (Exception e)
        {
            const string message = "Error while sending message";
            LoggerHolder.Instance.Error(e, message);
            return Result.Fail(e);
        }
    }
}