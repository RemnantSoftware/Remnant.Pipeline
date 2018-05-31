using System;

namespace Remnant.Exceptions
{
	public class NullOrWhitespaceException : Exception
	{
		public NullOrWhitespaceException(string message)
			: base(message)
   	{   		
   	}

		public NullOrWhitespaceException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
