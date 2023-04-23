using System;

namespace WolfRPG.Core.CommandConsole
{
	public interface IConsoleCommand
	{
		string Word { get; }
		ConsoleArgumentType[] Arguments { get;}

		void Execute(object[] arguments, Action<string> onError);
	}
}