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
    public partial class DiscordApiProvider : IBotApiProvider, IDisposable
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
    }
}