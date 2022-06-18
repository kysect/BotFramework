using System;
using System.Collections.Generic;
using System.Linq;
using Kysect.BotFramework.Core.BotMedia;
using Kysect.BotFramework.Core.Contexts;
using Kysect.BotFramework.Core.Contexts.Providers;
using Kysect.BotFramework.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.BotFramework.Core.Commands
{
    public class CommandContainer
    {
        private readonly SenderInfo _senderInfo;

        private readonly IServiceProvider _serviceProvider;

        public string CommandName { get; private set; }
        public List<string> Arguments { get; }
        public List<IBotMediaFile> MediaFiles { get; }
        
        public DialogContext DialogContext => 
            _serviceProvider.GetService<IDialogContextProvider>()?.GetDialogContext(_senderInfo)
            ?? throw new BotValidException("Database context options were not initialized");

        public CommandContainer(string commandName, List<string> arguments, List<IBotMediaFile> mediaFiles,
            SenderInfo senderInfo, IServiceProvider serviceProvider)
        {
            CommandName = commandName;
            Arguments = arguments;
            MediaFiles = mediaFiles;
            _senderInfo = senderInfo;
            _serviceProvider = serviceProvider;
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