using System.Collections.Generic;
using System.Linq;
using Kysect.BotFramework.Abstractions.BotMedia;
using Kysect.BotFramework.Abstractions.Commands;
using Kysect.BotFramework.Abstractions.Contexts;

namespace Kysect.BotFramework.Core.Commands
{
    public class CommandContainer : ICommandContainer
    {
        private readonly List<string> _arguments;

        public string CommandName { get; private set; }
        public List<IBotMediaFile> MediaFiles { get; }
        public ISenderInfo SenderInfo { get; }

        public CommandContainer(
            string commandName,
            List<string> arguments,
            List<IBotMediaFile> mediaFiles,
            ISenderInfo senderInfo) 
        {
            CommandName = commandName;
            _arguments = arguments;
            MediaFiles = mediaFiles;
            SenderInfo = senderInfo;
        }

        public bool StartsWithPrefix(char prefix) => prefix == '\0' || CommandName.FirstOrDefault() == prefix;

        public ICommandContainer RemovePrefix(char prefix)
        {
            if (CommandName.FirstOrDefault() == prefix)
            {
                CommandName = CommandName.Remove(0, 1);
            }

            return this;
        }

        public override string ToString() =>
            $"[CommandArgumentContainer CommandName:{CommandName}; Arguments:{string.Join(",", _arguments)}]";
    }
}