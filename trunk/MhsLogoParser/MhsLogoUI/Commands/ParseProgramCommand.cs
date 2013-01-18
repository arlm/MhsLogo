﻿using System;
using System.Windows.Input;
using MhsLogoController;
using MhsLogoParser;

namespace MhsLogoUI.Commands
{
	public class ParseProgramCommand : ICommand
	{
		public event EventHandler<ParseErrorEventArgs> ParseResult;
		public event EventHandler<LogoCommandEventArgs> LogoCommand;

		#region ICommand Members

		public event EventHandler CanExecuteChanged;

		public void Execute(object parameter)
		{
			var program = (string) parameter;
			try
			{
				var commands = LogoController.CreateAndParse(program);
				foreach (var logoCommand in commands)
				{
					var innerRepeatCommand = logoCommand as LogoRepeatCommand;
					if (innerRepeatCommand != null)
					{
						FireRepeatCommands(innerRepeatCommand);
					}
					else
					{
						FireSingleCommand(new LogoCommandEventArgs(logoCommand));
					}
				}
				FireParseResult(new ParseErrorEventArgs(false, String.Empty, String.Empty));
			}
			catch (Exception ex)
			{
				if (ex is LogoScannerException || ex is LogoSyntaxErrorException)
				{
					FireParseResult(new ParseErrorEventArgs(true, ex.Message, program));
				}
				else
				{
					throw;
				}
			}
		}

		private void FireRepeatCommands(LogoRepeatCommand repeatCommand)
		{
			for (int i = 0; i < repeatCommand.Repeat; i++)
			{
				foreach (ILogoCommand command in repeatCommand.Commands)
				{
					var innerRepeatCommand = command as LogoRepeatCommand;
					if (innerRepeatCommand != null)
					{
						FireRepeatCommands(innerRepeatCommand);
					}
					else
					{
						FireSingleCommand(new LogoCommandEventArgs(command));
					}
				}
			}
		}

		public void FireSingleCommand(LogoCommandEventArgs e)
		{
			EventHandler<LogoCommandEventArgs> handler = LogoCommand;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		public void FireParseResult(ParseErrorEventArgs e)
		{
			EventHandler<ParseErrorEventArgs> handler = ParseResult;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		#endregion
	}
}