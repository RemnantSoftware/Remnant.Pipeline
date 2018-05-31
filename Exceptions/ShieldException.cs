using System;

namespace Remnant.Exceptions
{
  public class ShieldException : Exception
  {
		public ShieldException(string message)
			: base(message)
   	{   		
   	}

		public ShieldException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
  }
}
