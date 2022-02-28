using System;
using System.Threading.Tasks;
using Kysect.BotFramework.ApiProviders.Telegram;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.DefaultCommands;
using Kysect.BotFramework.Settings;
using Microsoft.EntityFrameworkCore;

namespace Kysect.BotFramework.ConsoleTest
{
    public static class Program
    {
        public static string TelegramToken
        {
            get
            {
                throw new Exception("Set token");
                return string.Empty;
            }
        }

        private static async Task MainAsync()
        {

            var settings = new ConstSettingsProvider<TelegramSettings>(new TelegramSettings(TelegramToken));
            var api = new TelegramApiProvider(settings);

            BotManager botManager = new BotManagerBuilder()
                .SetDatabaseOptions(o => { o.UseSqlite("Filename=bf.db"); })
                .SetPrefix('!')
                .SetCaseSensitive(false)
                .AddCommand<PingCommand>()
                .Build(api);

            botManager.Start();

            await Task.Delay(-1);
        }

        private static void Main() => MainAsync().GetAwaiter().GetResult();
    }
}