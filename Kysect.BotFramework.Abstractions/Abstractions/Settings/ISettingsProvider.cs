namespace Kysect.BotFramework.Abstactions.Settings;

public interface ISettingsProvider<out TSettings>
{
    TSettings GetSettings();
}