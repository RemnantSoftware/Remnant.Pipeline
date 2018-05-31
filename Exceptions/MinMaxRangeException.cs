using System;

namespace Remnant.Exceptions
{
	public class MinMaxRangeException : Exception
	{
		public MinMaxRangeException(string message)
			: base(message)
		{
		}

		public MinMaxRangeException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

	}
}
