using Kysect.BotFramework.Abstractions.BotMedia;
using System.IO;
using System.Linq;

namespace Kysect.BotFramework.Core.Tools.Extensions;

public static class MediaFileExtensions
{
    public static string GetFileExtension(this IBotMediaFile mediaFile)
    {
        return mediaFile.Path.Split(Path.DirectorySeparatorChar).Last();
    }
}