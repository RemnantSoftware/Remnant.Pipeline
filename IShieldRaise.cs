using System;

namespace Remnant
{
	public interface IShieldRaise
	{		
		/// <summary>
		/// Raise the shield
		/// </summary>
		void Raise(Exception innerException = null);

		/// <summary>
		/// Raise the shield with a specific exception type to use
		/// </summary>
		/// <typeparam name="TException">The generic exception type</typeparam>
		void Raise<TException>(Exception innerException = null) where TException : Exception;
	}
}
