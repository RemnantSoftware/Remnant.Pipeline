using System;

namespace Remnant.Exceptions
{
	public class AssertException : Exception
	{
		public AssertException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public AssertException(string message)
			: base(message)
   	{   		
   	}
	}
}
