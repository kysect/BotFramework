using System.Threading.Tasks;
using Kysect.BotFramework.ApiProviders.Telegram;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.DefaultCommands;
using Kysect.BotFramework.Settings;

namespace Kysect.BotFramework.ConsoleTest
{
    public static class Program
    {
        private static async Task MainAsync()
        {
            var telegramToken = "***token***";

            var settings = new ConstSettingsProvider<TelegramSettings>(new TelegramSettings(telegramToken));
            var api = new TelegramApiProvider(settings);

            BotManager botManager = new BotManagerBuilder()
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