using System;

namespace WolfRPG.Core.CommandConsole
{
	public class EchoCommand: IConsoleCommand
	{
		public string Word => "echo";
		public ConsoleArgumentType[] Arguments { get; } = {ConsoleArgumentType.String};
		
		public void Execute(object[] arguments, Action<string> onError)
		{
			// Argument count and type are guaranteed to be correct at this point
			var message = (string)arguments[0];
			
			CommandConsole.LogMessage(message);
		}
	}
}