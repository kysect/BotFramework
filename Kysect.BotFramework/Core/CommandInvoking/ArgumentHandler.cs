using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Kysect.BotFramework.Core.Commands;
using Kysect.BotFramework.Core.Tools;
using Kysect.BotFramework.Core.Tools.Extensions;

namespace Kysect.BotFramework.Core.CommandInvoking;

public class ArgumentHandler
{
    private readonly CommandContainer _args;
    private readonly IBotCommand _command;

    private IEnumerable<(string, string)> _arguments;

    public ArgumentHandler(CommandContainer args, IBotCommand command)
    {
        _args = args;
        _command = command;
    }

    public Result CheckArgumentsCount()
    {
        List<string> commandArgumentNames = _command.GetBotCommandArgumentNames();
        List<string> commandArguments = _args.Arguments;

        if (commandArguments.Count > commandArgumentNames.Count)
        {
            return Result.Fail("Too many arguments");
        }

        if (commandArguments.Count != commandArgumentNames.Count
            && !_command.GetBotCommandDescriptorAttribute().ArgumentsOptional)
        {
            return Result.Fail("Not enough arguments");
        }

        _arguments = commandArgumentNames.Zip(_args.Arguments, (c, a) => (c, a));

        return Result.Ok();
    }

    public Result TryAssignArguments()
    {
        foreach (var (commandArgumentName, propertyValue) in _arguments)
        {
            Result checkResult = TryAssignArgument(commandArgumentName, propertyValue);
            if (checkResult.IsFailed)
            {
                return checkResult;
            }
        }

        return Result.Ok();
    }

    private Result TryAssignArgument(string commandArgumentName, string propertyValue)
    {
        PropertyInfo property = _command.GetType().GetProperty(commandArgumentName,
            BindingFlags.Public | BindingFlags.Instance);

        try 
        {
            var convertedValue = Convert.ChangeType(
                propertyValue,
                property!.PropertyType,
                CultureInfo.CurrentCulture);

            property.SetValue(_command, convertedValue);
        }
        catch (FormatException)
        {
            return Result.Fail(
                $"Argument \"{commandArgumentName}\" is in the wrong format");
        }

        return Result.Ok();
    }
}