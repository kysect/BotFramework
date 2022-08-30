﻿using System.IO;
using Kysect.BotFramework.Abstactions.Settings;
using Newtonsoft.Json;

namespace Kysect.BotFramework.Settings
{
    public class ConfigSettingsProvider<TSettings> : ISettingsProvider<TSettings> where TSettings : new()
    {
        private readonly string _configPath;

        public ConfigSettingsProvider(string configPath)
        {
            _configPath = configPath;
        }

        public TSettings GetSettings() => JsonConvert.DeserializeObject<TSettings>(File.ReadAllText(_configPath));
    }
}