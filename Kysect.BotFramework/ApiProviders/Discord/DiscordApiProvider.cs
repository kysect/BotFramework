using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.Core.BotMedia;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.Contexts;
using Kysect.BotFramework.Core.Tools;
using Kysect.BotFramework.Core.Tools.Loggers;
using Kysect.BotFramework.DefaultCommands;
using Kysect.BotFramework.Settings;

namespace Kysect.BotFramework.ApiProviders.Discord
{
    public class DiscordApiProvider : IBotApiProvider, IDisposable
    {
        private readonly object _lock = new object();
        private readonly DiscordSettings _settings;
        private DiscordSocketClient _client;

        public DiscordApiProvider(ISettingsProvider<DiscordSettings> settingsProvider)
        {
            _settings = settingsProvider.GetSettings();
            Initialize();
        }

        public event EventHandler<BotNewMessageEventArgs> OnMessage;

        public void Restart()
        {
            lock (_lock)
            {
                if (_client != null)
                {
                    Dispose();
                }

                Initialize();
            }
        }

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

        public async Task<Result> SendTextMessageAsync(string text, SenderInfo sender)
        {
            if (text.Length == 0)
            {
                LoggerHolder.Instance.Error("The message wasn't sent by the command, the length must not be zero.");
                return Result.Ok();
            }

            return await SendTextAsync(text, sender);
        }

        public void Dispose()
        {
            _client.MessageReceived -= ClientOnMessage;
            _client.StopAsync();
        }

        private void Initialize()
        {
            _client = new DiscordSocketClient();

            _client.LoginAsync(TokenType.Bot, _settings.AccessToken);

            _client.MessageReceived += ClientOnMessage;
            _client.StartAsync();
        }

        private Task ClientOnMessage(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message is null)
            {
                return Task.CompletedTask;
            }

            var context = new SocketCommandContext(_client, message);
            if (context.User.IsBot || context.Guild is null)
            {
                return Task.CompletedTask;
            }

            LoggerHolder.Instance.Debug($"New message event: {context.Message}");

            IBotMessage botMessage = ParseMessage(message, context);

            OnMessage?.Invoke(context.Client,
                              new BotNewMessageEventArgs(
                                  botMessage,
                                  new DiscordSenderInfo(
                                      (long) context.Channel.Id,
                                      (long) context.User.Id,
                                      context.User.Username,
                                      CheckIsAdmin(context.User),
                                      context.Guild.Id)
                                  )
                              );

            return Task.CompletedTask;
        }

        private IBotMessage ParseMessage(SocketUserMessage message, SocketCommandContext context)
        {
            if (context.Message.Attachments.Count == 0)
            {
                return new BotTextMessage(context.Message.Content);
            }

            if (context.Message.Attachments.Count == 1)
            {
                IBotOnlineFile onlineFile =
                    GetOnlineFile(message.Attachments.First().Filename, message.Attachments.First().Url);
                return onlineFile is not null
                    ? new BotSingleMediaMessage(context.Message.Content, onlineFile)
                    : new BotTextMessage(context.Message.Content);
            }

            List<IBotMediaFile> mediaFiles = context.Message.Attachments
                                                    .Select(attachment =>
                                                                GetOnlineFile(attachment.Filename, attachment.Url))
                                                    .Where(onlineFile => onlineFile is not null).Cast<IBotMediaFile>()
                                                    .ToList();

            if (!mediaFiles.Any())
            {
                return new BotTextMessage(context.Message.Content);
            }


            if (mediaFiles.Count == 1)
            {
                return new BotSingleMediaMessage(context.Message.Content, mediaFiles.First());
            }


            return new BotMultipleMediaMessage(context.Message.Content, mediaFiles);
        }

        private IBotOnlineFile GetOnlineFile(string filename, string url)
        {
            switch (ParseMediaType(filename))
            {
                case MediaTypeEnum.Photo: return new BotOnlinePhotoFile(url);
                case MediaTypeEnum.Video: return new BotOnlineVideoFile(url);
                default:
                    LoggerHolder.Instance.Information($"Skipped file: {filename}");
                    return null;
            }
        }

        private MediaTypeEnum ParseMediaType(string filename)
        {
            if (filename.EndsWith("png") || filename.EndsWith("jpg") ||
                filename.EndsWith("bmp"))
            {
                return MediaTypeEnum.Photo;
            }

            if (filename.EndsWith("mp4") || filename.EndsWith("mov") ||
                filename.EndsWith("wmv") || filename.EndsWith("avi"))
            {
                return MediaTypeEnum.Video;
            }

            return MediaTypeEnum.Undefined;
        }

        private bool CheckIsAdmin(SocketUser user)
        {
            var socketGuildUser = user as SocketGuildUser;
            return socketGuildUser.GuildPermissions.Administrator;
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
}