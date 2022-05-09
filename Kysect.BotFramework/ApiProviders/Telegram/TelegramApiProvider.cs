using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.Core.BotMedia;
using Kysect.BotFramework.Core.BotMessages;
using Kysect.BotFramework.Core.Contexts;
using Kysect.BotFramework.Core.Tools;
using Kysect.BotFramework.Core.Tools.Loggers;
using Kysect.BotFramework.DefaultCommands;
using Kysect.BotFramework.Settings;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace Kysect.BotFramework.ApiProviders.Telegram
{
    public partial class TelegramApiProvider : IBotApiProvider, IDisposable
    {
        private readonly object _lock = new object();
        private readonly TelegramSettings _settings;
        private TelegramBotClient _client;

        public TelegramApiProvider(ISettingsProvider<TelegramSettings> settingsProvider)
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

        private void Initialize()
        {
            _client = new TelegramBotClient(_settings.AccessToken);

            _client.OnMessage += ClientOnMessage;
            _client.StartReceiving();
        }

        public void Dispose()
        {
            _client.OnMessage -= ClientOnMessage;
            _client.StopReceiving();
        }

        private void ClientOnMessage(object sender, MessageEventArgs e)
        {
            LoggerHolder.Instance.Debug("New message event: {@e}", e);
            string text = e.Message.Text ?? e.Message.Caption;
            IBotMessage message = e.Message.Type switch
            {
                MessageType.Photo => new BotSingleMediaMessage(text,
                    new BotOnlinePhotoFile(GetFileLink(e.Message.Photo.Last().FileId), e.Message.Photo.Last().FileId)),
                MessageType.Video => new BotSingleMediaMessage(text,
                    new BotOnlineVideoFile(GetFileLink(e.Message.Video.FileId), e.Message.Video.FileId)),
                _ => new BotTextMessage(text),
            };

            OnMessage?.Invoke(sender,
                new BotNewMessageEventArgs(
                    message,
                    new TelegramSenderInfo(
                        e.Message.Chat.Id,
                        e.Message.From.Id,
                        e.Message.From.FirstName,
                        CheckIsAdmin(e.Message.From.Id, e.Message.Chat.Id)
                    )
                ));
        }

        private string GetFileLink(string id) =>
            $"https://api.telegram.org/file/bot{_settings.AccessToken}/{_client.GetFileAsync(id).Result.FilePath}";

        private bool CheckIsAdmin(int userId, long chatId)
        {
            var chatMember = _client.GetChatMemberAsync(chatId, userId).Result;
            return chatMember.Status is ChatMemberStatus.Administrator or ChatMemberStatus.Creator;
        }
    }
}