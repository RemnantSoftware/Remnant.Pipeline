using System;

namespace Remnant.Exceptions
{
	public class NotNullException : Exception
	{
		public NotNullException(string message)
			: base(message)
		{
		}

		public NotNullException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
