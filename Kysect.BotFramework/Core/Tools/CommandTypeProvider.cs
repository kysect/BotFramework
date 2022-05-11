using System;
using System.Collections.Generic;

namespace Kysect.BotFramework.Core.Tools;

public class CommandTypeProvider
{
    private readonly bool _caseSensitive;
    private readonly Dictionary<string, Type> _commandTypes = new Dictionary<string, Type>();

    public CommandTypeProvider(Dictionary<string, Type> commandTypes, bool caseSensitive)
    {
        _caseSensitive = caseSensitive;
        foreach (var commandTypesKey in commandTypes.Keys)
        {
            if (!caseSensitive)
                _commandTypes[commandTypesKey.ToLower()] = commandTypes[commandTypesKey];
            else
                _commandTypes[commandTypesKey] = commandTypes[commandTypesKey];
        }
    }

    public Type GetCommandTypeOrDefault(string command)
    {
        if (!_caseSensitive)
            return _commandTypes.GetValueOrDefault(command);
        
        return _commandTypes.GetValueOrDefault(command);
    }
}