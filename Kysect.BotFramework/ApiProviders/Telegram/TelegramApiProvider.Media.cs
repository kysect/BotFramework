using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kysect.BotFramework.Core.BotMedia;
using Kysect.BotFramework.Core.Contexts;
using Kysect.BotFramework.Core.Tools;
using Kysect.BotFramework.Core.Tools.Loggers;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace Kysect.BotFramework.ApiProviders.Telegram;

public partial class TelegramApiProvider : IBotApiProvider, IDisposable
{
    public async Task<Result> SendMultipleMediaAsync(List<IBotMediaFile> mediaFiles, string text, SenderInfo sender)
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
                Message[] task = await _client.SendMediaGroupAsync(filesToSend, sender.ChatId);

                return Result.Ok();
            }
            catch (Exception e)
            {
                const string message = "Error while sending message";
                LoggerHolder.Instance.Error(e, message);

                return Result.Fail(e.Message);
            }
            finally
            {
                foreach (FileStream stream in streams)
                {
                    stream.Close();
                }
            }
        }
        
        private List<IAlbumInputMedia> CollectInputMedia(List<IBotMediaFile> mediaFiles, string text,
            List<FileStream> streams)
        {
            List<IAlbumInputMedia> filesToSend = new List<IAlbumInputMedia>();
            IAlbumInputMedia fileToSend;
            if (mediaFiles.First() is IBotOnlineFile onlineFile)
            {
                fileToSend = mediaFiles.First().MediaType switch
                {
                    MediaTypeEnum.Photo => new InputMediaPhoto(onlineFile.Id) {Caption = text},
                    MediaTypeEnum.Video => new InputMediaVideo(onlineFile.Id) {Caption = text}
                };
            }
            else
            {
                streams.Add(File.Open(mediaFiles.First().Path, FileMode.Open));
                var inputMedia = new InputMedia(streams.Last(),
                                                mediaFiles.First().Path.Split(Path.DirectorySeparatorChar).Last());
                fileToSend = mediaFiles.First().MediaType switch
                {
                    MediaTypeEnum.Photo => new InputMediaPhoto(inputMedia) {Caption = text},
                    MediaTypeEnum.Video => new InputMediaVideo(inputMedia) {Caption = text}
                };
            }

            filesToSend.Add(fileToSend);

            foreach (IBotMediaFile mediaFile in mediaFiles.Skip(1))
            {
                if (mediaFile is IBotOnlineFile onlineMediaFile)
                {
                    fileToSend = mediaFile.MediaType switch
                    {
                        MediaTypeEnum.Photo => new InputMediaPhoto(onlineMediaFile.Id) {Caption = text},
                        MediaTypeEnum.Video => new InputMediaVideo(onlineMediaFile.Id) {Caption = text}
                    };
                }
                else
                {
                    streams.Add(File.Open(mediaFile.Path, FileMode.Open));
                    var inputMedia = new InputMedia(streams.Last(),
                                                    mediaFile.Path.Split(Path.DirectorySeparatorChar).Last());
                    fileToSend = mediaFile.MediaType switch
                    {
                        MediaTypeEnum.Photo => new InputMediaPhoto(inputMedia),
                        MediaTypeEnum.Video => new InputMediaVideo(inputMedia)
                    };
                }

                filesToSend.Add(fileToSend);
            }

            return filesToSend;
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

        public async Task<Result> SendMediaAsync(IBotMediaFile mediaFile, string text, SenderInfo sender)
        {
            Result result = CheckText(text);
            if (result.IsFailed)
            {
                return result;
            }

            FileStream stream = File.Open(mediaFile.Path, FileMode.Open);
            var fileToSend = new InputMedia(stream, mediaFile.Path.Split(Path.DirectorySeparatorChar).Last());
            

            try
            {
                Message message = mediaFile.MediaType switch
                {
                    MediaTypeEnum.Photo => await _client.SendPhotoAsync(sender.ChatId, fileToSend, text),
                    MediaTypeEnum.Video => await _client.SendVideoAsync(sender.ChatId, fileToSend, caption: text)
                };
                return Result.Ok();
            }
            catch (Exception e)
            {
                const string message = "Error while sending message";
                LoggerHolder.Instance.Error(e, message);
                return Result.Fail(e.Message);
            }
            finally
            {
                stream.Close();
            }
        }

        public async Task<Result> SendOnlineMediaAsync(IBotOnlineFile file, string text, SenderInfo sender)
        {
            Result result = CheckText(text);
            if (result.IsFailed)
            {
                return result;
            }

            string fileIdentifier = file.Id ?? file.Path;
            
            try
            {
                Message msg = file.MediaType switch
                {
                    MediaTypeEnum.Photo => await _client.SendPhotoAsync(sender.ChatId, fileIdentifier, text),
                    MediaTypeEnum.Video => await _client.SendVideoAsync(sender.ChatId, fileIdentifier, caption: text)
                };
                return Result.Ok();
            }
            catch (Exception e)
            {
                const string message = "Error while sending message";
                LoggerHolder.Instance.Error(e, message);
                return Result.Fail(e.Message);
            }
        }
}