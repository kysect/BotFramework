using System;
using System.Collections.Generic;
using System.Linq;
using Kysect.BotFramework.Core.BotMedia;
using Kysect.BotFramework.Core.Contexts;
using Kysect.BotFramework.Core.Contexts.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.BotFramework.Core.Commands
{
    public class CommandContainer
    {
        private readonly ServiceProvider _serviceProvider;

        public string CommandName { get; private set; }
        public List<string> Arguments { get; }
        public List<IBotMediaFile> MediaFiles { get; }

        public DialogContext DialogContext => _serviceProvider
            .GetRequiredService<IDialogContextProvider>()
            .GetDialogContext();

        public CommandContainer(string commandName, ServiceProvider serviceProvider, List<string> arguments,
            List<IBotMediaFile> mediaFiles)
        {
            CommandName = commandName;
            _serviceProvider = serviceProvider;
            Arguments = arguments;
            MediaFiles = mediaFiles;
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