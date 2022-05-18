using System.IO;
using System.Linq;
using Kysect.BotFramework.Core.BotMedia;

namespace Kysect.BotFramework.Settings;

public static class MediaFileExtensions
{
    public static string GetFileExtension(this IBotMediaFile mediaFile)
    {
        return mediaFile.Path.Split(Path.DirectorySeparatorChar).Last();
    }
}