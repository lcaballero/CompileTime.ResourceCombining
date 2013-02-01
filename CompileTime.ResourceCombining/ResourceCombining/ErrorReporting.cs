using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CompileTime.ResourceCombining
{
	public class ErrorReporting
	{
		public List<string> Info { get; private set; }
		public List<string> Warnings { get; private set; }
		public List<string> Errors { get; private set; }

		public ErrorReporting()
		{
			Info = new List<string>();
			Warnings = new List<string>();
			Errors = new List<string>();
		}

		private string NewMessage(string fmt, params object[] args)
		{
			string message =
				(args != null && args.Length > 0)
				? string.Format(fmt, args)
				: fmt;

			return message;
		}

		public ErrorReporting AddInfo(string fmt, params object[] args)
		{
			Info.Add(NewMessage(fmt, args));
			return this;
		}

		public ErrorReporting AddWarning(string fmt, params object[] args)
		{
			Warnings.Add(NewMessage(fmt, args));

			return this;
		}

		public ErrorReporting AddError(string fmt, params object[] args)
		{
			Errors.Add(NewMessage(fmt, args));

			return this;
		}
	}
}
