using System.Collections.Generic;
using System.Linq;
using Kysect.BotFramework.Core.BotMedia;
using Kysect.BotFramework.Core.Contexts;

namespace Kysect.BotFramework.Core.Commands
{
    public class CommandContainer
    {
        internal List<string> Arguments { get; }

        public string CommandName { get; private set; }
        public List<IBotMediaFile> MediaFiles { get; }
        public SenderInfo SenderInfo { get; }

        public CommandContainer(
            string commandName,
            List<string> arguments,
            List<IBotMediaFile> mediaFiles,
            SenderInfo senderInfo) 
        {
            CommandName = commandName;
            Arguments = arguments;
            MediaFiles = mediaFiles;
            SenderInfo = senderInfo;
        }

        public bool StartsWithPrefix(char prefix) => prefix == '\0' || CommandName.FirstOrDefault() == prefix;

        public CommandContainer RemovePrefix(char prefix)
        {
            if (CommandName.FirstOrDefault() == prefix)
            {
                CommandName = CommandName.Remove(0, 1);
            }

            return this;
        }

        public override string ToString() =>
            $"[CommandArgumentContainer CommandName:{CommandName}; Arguments:{string.Join(",", Arguments)}]";
    }
}