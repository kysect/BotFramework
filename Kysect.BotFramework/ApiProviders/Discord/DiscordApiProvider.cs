using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Kysect.BotFramework.Abstactions.Settings;
using Kysect.BotFramework.Abstractions;
using Kysect.BotFramework.Abstractions.BotMedia;
using Kysect.BotFramework.Abstractions.BotMessages;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.Core.BotMedia;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.Tools.Loggers;
using Kysect.BotFramework.Enums;

namespace Kysect.BotFramework.ApiProviders.Discord
{
    public partial class DiscordApiProvider : IBotApiProvider, IDisposable
    {
        private readonly object _lock = new();
        private readonly DiscordSettings _settings;
        private DiscordSocketClient _client;

        public DiscordApiProvider(ISettingsProvider<DiscordSettings> settingsProvider)
        {
            _settings = settingsProvider.GetSettings();
            Initialize();
        }

        public event EventHandler<IBotEventArgs> OnMessage;

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
                              new BotEventArgs(
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
            List<IBotMediaFile> mediaFiles = context.Message.Attachments
                                                    .Select(attachment =>
                                                                GetOnlineFile(attachment.Filename, attachment.Url))
                                                    .Where(onlineFile => onlineFile is not null)
                                                    .Cast<IBotMediaFile>()
                                                    .ToList();

            IBotMessage parsedMessage = mediaFiles.Count switch
            {
                0 => new BotTextMessage(context.Message.Content),
                1 => new BotSingleMediaMessage(context.Message.Content, mediaFiles.Single()),
                _ => new BotMultipleMediaMessage(context.Message.Content, mediaFiles),
            };

            return parsedMessage;
        }

        private IBotOnlineFile GetOnlineFile(string filename, string url)
        {
            switch (ParseMediaType(filename))
            {
                case MediaType.Photo: return new BotOnlinePhotoFile(url);
                case MediaType.Video: return new BotOnlineVideoFile(url);
                default:
                    LoggerHolder.Instance.Information($"Skipped file: {filename}");
                    return null;
            }
        }

        private MediaType ParseMediaType(string filename)
        {
            if (filename.EndsWith("png") || filename.EndsWith("jpg") ||
                filename.EndsWith("bmp"))
            {
                return MediaType.Photo;
            }

            if (filename.EndsWith("mp4") || filename.EndsWith("mov") ||
                filename.EndsWith("wmv") || filename.EndsWith("avi"))
            {
                return MediaType.Video;
            }

            return MediaType.Undefined;
        }

        private bool CheckIsAdmin(SocketUser user)
        {
            var socketGuildUser = user as SocketGuildUser;
            return socketGuildUser.GuildPermissions.Administrator;
        }
    }
}