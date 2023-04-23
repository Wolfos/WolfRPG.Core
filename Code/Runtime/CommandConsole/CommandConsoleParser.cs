using System;
using System.Collections.Generic;
using UnityEngine;

namespace WolfRPG.Core.CommandConsole
{
	public class CommandConsoleParser
	{
		private const char SeparatorChar = ',';

		private Dictionary<string, IConsoleCommand> _consoleCommands = new();

		public CommandConsoleParser()	
		{
			RegisterDefaultCommands();
		}

		private void RegisterDefaultCommands()
		{
			RegisterCommand(new EchoCommand());
		}
		
		public void RegisterCommand(IConsoleCommand command)
		{
			if (_consoleCommands.ContainsKey(command.Word))
			{
				Debug.LogError($"Command {command.Word} was already registered");
				return;
			}
			
			_consoleCommands.Add(command.Word, command);
		}

		public void ExecuteCommand(string commandText)
		{
			var separated = commandText.Split(SeparatorChar);
			if (separated.Length == 0) return;
			
			var word = separated[0].ToLower();
			word = word.TrimStart(); // Trim starting spaces
			word = word.TrimEnd(); // Trim trailing spaces
			if (_consoleCommands.ContainsKey(word) == false)
			{
				CommandConsole.LogError($"Command \"{word}\" not found");
				return;
			}

			var command = _consoleCommands[word];
			var argumentCount = command.Arguments.Length;
			if (separated.Length != argumentCount + 1)
			{
				CommandConsole.LogError($"Command \"{word}\" requires {argumentCount} arguments but found {separated.Length - 1}");
				return;
			}

			var arguments = new object[argumentCount];
			for (var i = 0; i < argumentCount; i++)
			{
				var argument = separated[i + 1];
				argument = argument.TrimStart();
				argument = argument.TrimEnd();

				var argumentType = command.Arguments[i];
				switch (argumentType)
				{
					case ConsoleArgumentType.String:
						arguments[i] = argument;
						break;
					case ConsoleArgumentType.Int:
						if (int.TryParse(argument, out int argAsInt) == false)
						{
							CommandConsole.LogError($"Argument \"{argument}\" should be int but could not be parsed");
							return;
						}

						arguments[i] = argAsInt;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			
			command.Execute(arguments, CommandConsole.LogError);
		}
	}
}