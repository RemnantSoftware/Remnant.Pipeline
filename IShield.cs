using System;
using System.Collections;
using System.Collections.Generic;

namespace Remnant
{
	public interface IShield
	{
		/// <summary>
		/// Register an exception with its message.
		/// Registering an exception that is already registered will overwrite the previous message.
		/// Using the 'WithMessage' will also overwrite the registered message.
		/// </summary>
		/// <typeparam name="TException">The exception to register</typeparam>
		/// <param name="message">The message to display for the exception (can contain parameter placeholders, ex: {0})</param>
		/// <returns>Returns the instance of the Shield</returns>
		IShield Register<TException>(string message) where TException : Exception;

		/// <summary>
		/// Deregister an exception with its message.
		/// </summary>
		/// <typeparam name="TException">The exception to register</typeparam>
		/// <returns>Returns the instance of the Shield</returns>
		IShield DeRegister<TException>() where TException : Exception;

		/// <summary>
		/// Shield will call your method with the exception for logging purposes
		/// </summary>
		/// <param name="callback">The callback method</param>
		/// <returns>Returns the Shield instance</returns>
		IShield Log(Action<Exception> callback);

		/// <summary>
		/// Shield against a boolean evaluated expression
		/// </summary>
		/// <param name="assertion">The assertion</param>
		/// <param name="messageParameters">Optional message parameters</param>
		/// <returns>Returns the Shield instance</returns>
		IShieldWith Against(bool assertion, params object[] messageParameters);

		/// <summary>
		/// Shield against a boolean evaluated expression
		/// </summary>
		/// <param name="assertion">The assertion</param>
		/// <param name="messageParameters">Optional message parameters</param>
		/// <returns>Returns the Shield instance</returns>
		IShieldWith Against(Func<bool> assertion, params object[] messageParameters);

		/// <summary>
		/// Shield against nullable object 
		/// </summary>
		/// <param name="instance">The object instance</param>
		/// <param name="messageParameters">Optional message parameters</param>
		/// <returns>Returns the Shield instance</returns>
		IShieldWith AgainstNull(object instance, params object[] messageParameters);

		/// <summary>
		/// Shield against not nullable object 
		/// </summary>
		/// <param name="instance">The object instance</param>
		/// <param name="messageParameters">Optional message parameters</param>
		/// <returns>Returns the Shield instance</returns>
		IShieldWith AgainstNotNull(object instance, params object[] messageParameters);

		/// <summary>
		/// Shield against a null or empty string
		/// </summary>
		/// <param name="instance">The string instance</param>
		/// <param name="messageParameters">Optional message parameters</param>
		/// <returns>Returns the Shield instance</returns>
		IShieldWith AgainstNullOrEmpty(string instance, params object[] messageParameters);

#if !TARGET_3_5
		/// <summary>
		/// Shield against a null or whitespace string
		/// </summary>
		/// <param name="instance">The string instance</param>
		/// <param name="messageParameters">Optional message parameters</param>
		/// <returns>Returns the Shield instance</returns>
		IShieldWith AgainstNullOrWhitespace(string instance, params object[] messageParameters);
#endif

		/// <summary>
		/// Shield against an out of range value
		/// </summary>
		/// <param name="value">The value</param>
		/// <param name="min">The minimum of the value range</param>
		/// <param name="max">The maximum of the value range</param>
		/// <param name="messageParameters">Optional message parameters</param>
		/// <returns>Returns the Shield instance</returns>
		IShieldWith AgainstNotInRange(int value, int min, int max, params object[] messageParameters);

		/// <summary>
		/// Shield against an out of range date
		/// </summary>
		/// <param name="date">The value</param>
		/// <param name="minDate">The minimum of the date range</param>
		/// <param name="maxDate">The maximum of the date range</param>
		/// <param name="messageParameters">Optional message parameters</param>
		/// <returns>Returns the Shield instance</returns>
		IShieldWith AgainstNotInRange(DateTime date, DateTime minDate, DateTime maxDate, params object[] messageParameters);

		/// <summary>
		/// Shield that object is of a specific type
		/// </summary>
		/// <typeparam name="TType">The generic type</typeparam>
		/// <param name="instance">The instance to check</param>
		/// <param name="messageParameters">Optional message parameters</param>
		/// <returns>Returns the Shield instance</returns>
		IShieldWith MustBeOfType<TType>(object instance, params object[] messageParameters);
	}
}
