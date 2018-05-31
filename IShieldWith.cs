using System;

namespace Remnant
{
	public interface IShieldWith
	{
		/// <summary>
		/// Concatenate assertion
		/// </summary>
		IShield And { get; }

		/// <summary>
		/// Specify the message to be used
		/// (Also use 'WithParameters' if message contains placeholders)
		/// </summary>
		/// <param name="message">The message</param>
		/// <returns>Returns the shield instance</returns>
		IShieldWith WithMessage(string message);

		/// <summary>
		/// Speficy the parameters that must be used for the message
		/// </summary>
		/// <param name="parameters">A list of parameters</param>
		/// <returns>Returns the shield instance</returns>
		IShieldRaise WithParameters(params object[] parameters);

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
