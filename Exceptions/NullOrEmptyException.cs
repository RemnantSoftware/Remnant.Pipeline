using System;

namespace Remnant.Exceptions
{
	public class NullOrEmptyException : Exception
	{
		public NullOrEmptyException(string message)
			: base(message)
   	{   		
   	}

		public NullOrEmptyException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
