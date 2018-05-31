using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Remnant.Exceptions;
using Remnant.Resources;

namespace Remnant
{

	public class Shield : IShield, IShieldWith, IShieldRaise
	{
		#region Fields

		private static IShield _singleton;
		private static readonly object _padLock = new object();

		private Action<Exception> _callback;

		private readonly Dictionary<Type, string> _registry = new Dictionary<Type, string>();
		[ThreadStatic]
		private static Type _exceptionType;
		[ThreadStatic]
		private static string _message;
		[ThreadStatic]
		private static bool _isCustomMessage;
		[ThreadStatic]
		private static object[] _messageParameters;

		#endregion

		#region Constructors & Finalizors

		protected Shield()
		{
			(this as IShield).Register<NullReferenceException>(Messages.CannotBeNull);
			(this as IShield).Register<NotNullException>(Messages.MustBeNull);
			(this as IShield).Register<ArgumentOutOfRangeException>(Messages.OutOfRange);
			(this as IShield).Register<AssertException>(Messages.AssertMessage);
			(this as IShield).Register<NullOrEmptyException>(Messages.CannotBeNullOrEmpty);
			(this as IShield).Register<NullOrWhitespaceException>(Messages.CannotBeNullOrWhitespace);
			(this as IShield).Register<MinMaxRangeException>(Messages.OutsideMinMaxRange);
			(this as IShield).Register<InvalidOperationException>(Messages.InvalidType);
		}

		#endregion

		#region Static Members

		private static IShield Instance
		{
			get
			{
				lock (_padLock)
				{
					return _singleton ?? (_singleton = new Shield());
				}
			}
		}

		public static IShield Register<TException>(string message)
			where TException : Exception
		{
			return Instance.Register<TException>(message);
		}

		public static IShield DeRegister<TException>()
			where TException : Exception
		{
			return Instance.DeRegister<TException>();
		}

		public static IShield Log(Action<Exception> callback)
		{
			return Instance.Log(callback);
		}

		public static IShieldWith Against(bool assertion, params object[] messageParameters)
		{
			return Instance.Against(assertion, messageParameters);
		}

		public static IShieldWith Against(Func<bool> assertion, params object[] messageParameters)
		{
			return Instance.Against(assertion, messageParameters);
		}

		public static IShieldWith AgainstNullOrEmpty(string instance, params object[] messageParameters)
		{
			return Instance.AgainstNullOrEmpty(instance, messageParameters);
		}

#if !TARGET_3_5
		public static IShieldWith AgainstNullOrWhitespace(string instance, params object[] messageParameters)
		{
			return Instance.AgainstNullOrWhitespace(instance, messageParameters);
		}
#endif

		public static IShieldWith AgainstNull(object instance, params object[] messageParameters)
		{
			return Instance.AgainstNull(instance, messageParameters);
		}

		public static IShieldWith AgainstNotNull(object instance, params object[] messageParameters)
		{
			return Instance.AgainstNotNull(instance, messageParameters);
		}

		public static IShieldWith AgainstNotInRange(int value, int min, int max, params object[] messageParameters)
		{
			return Instance.AgainstNotInRange(value, min, max, messageParameters);
		}

		public static IShieldWith AgainstNotInRange(DateTime date, DateTime minDate, DateTime maxDate, params object[] messageParameters)
		{
			return Instance.AgainstNotInRange(date, minDate, maxDate, messageParameters);
		}

		public static IShieldWith MustBeOfType<TType>(object instance, params object[] messageParameters)
		{
			return Instance.MustBeOfType<TType>(instance, messageParameters);
		}

		#endregion

		#region Private Members

		private string GetExceptionMessage<TException>(bool overrideMessage) where TException : Exception
		{
			return (_registry.ContainsKey(typeof(TException)))
							 ? _registry[typeof(TException)]
							 : overrideMessage ? null : _message;
		}

		private void AssignException<TException>(bool overrideMessage = false) where TException : Exception
		{
			if ((_message == null || overrideMessage) && !_isCustomMessage)
				_message = GetExceptionMessage<TException>(overrideMessage);
			_exceptionType = typeof(TException);
		}

		private static int CountMessagePlaceHolders()
		{
			return Regex.Matches(_message.Replace("{{", string.Empty), @"\{(\d+)")
					.OfType<Match>()
					.Select(match => int.Parse(match.Groups[1].Value))
					.Union(Enumerable.Repeat(-1, 1))
					.Max() + 1;
		}

		private static string ParseMessage()
		{
			var parseMessage = _message;

			if (_message != null)
			{
				var totalMessagePlaceHolders = CountMessagePlaceHolders();
				if (totalMessagePlaceHolders > 0)
				{
					if (_messageParameters == null ||
						(_messageParameters.Length != totalMessagePlaceHolders))
						throw new ShieldException(
							string.Format(Messages.ParameterCountMismatch, _message));
					parseMessage = string.Format(_message, _messageParameters);
				}
			}

			return parseMessage;
		}

		#endregion

		#region Interface IShield

		IShield IShield.Log(Action<Exception> callback)
		{
			_callback = callback;
			return this;
		}

		IShield IShield.Register<TException>(string message)
		{
			lock (_padLock)
			{
				if (_registry.ContainsKey(typeof(TException)))
					_registry[typeof(TException)] = message;
				else
					_registry.Add(typeof(TException), message);
				return this;
			}
		}

		IShield IShield.DeRegister<TException>()
		{
			lock (_padLock)
			{
				if (_registry.ContainsKey(typeof(TException)))
					_registry.Remove(typeof(TException));
				return this;
			}
		}

		IShieldWith IShield.Against(bool assertion, params object[] messageParameters)
		{
			if (_exceptionType != null) return this;
			_messageParameters = messageParameters;
			if (assertion)
				AssignException<AssertException>();
			return this;
		}

		IShieldWith IShield.Against(Func<bool> assertion, params object[] messageParameters)
		{
			if (_exceptionType != null) return this;
			_messageParameters = messageParameters;
			if (assertion())
				AssignException<AssertException>();
			return this;
		}

		IShieldWith IShield.AgainstNullOrEmpty(string instance, params object[] messageParameters)
		{
			if (_exceptionType != null) return this;
			_messageParameters = messageParameters;
			if (string.IsNullOrEmpty(instance))
				AssignException<NullOrEmptyException>();
			return this;
		}

#if !TARGET_3_5
		IShieldWith IShield.AgainstNullOrWhitespace(string instance, params object[] messageParameters)
		{
			if (_exceptionType != null) return this;
			_messageParameters = messageParameters;
			if (string.IsNullOrWhiteSpace(instance))
				AssignException<NullOrWhitespaceException>();
			return this;
		}
#endif

		IShieldWith IShield.AgainstNull(object instance, params object[] messageParameters)
		{
			if (_exceptionType != null) return this;
			_messageParameters = messageParameters;
			if (_exceptionType == null && instance == null)
				AssignException<NullReferenceException>();
			return this;
		}

		IShieldWith IShield.AgainstNotNull(object instance, params object[] messageParameters)
		{
			if (_exceptionType != null) return this;
			_messageParameters = messageParameters;
			if (_exceptionType == null && instance != null)
				AssignException<NotNullException>();
			return this;
		}

		IShieldWith IShield.AgainstNotInRange(int value, int min, int max, params object[] messageParameters)
		{
			if (_exceptionType != null) return this;
			_messageParameters = messageParameters;
			if (value < min || value > max)
				AssignException<MinMaxRangeException>();
			return this;
		}

		IShieldWith IShield.AgainstNotInRange(DateTime date, DateTime minDate, DateTime maxDate, params object[] messageParameters)
		{
			if (_exceptionType != null) return this;
			_messageParameters = messageParameters;
			if (date < minDate || date > maxDate)
				AssignException<MinMaxRangeException>();
			return this;
		}

		IShieldWith IShield.MustBeOfType<TType>(object instance, params object[] messageParameters)
		{
			if (_exceptionType != null) return this;
			_messageParameters = messageParameters;
			if (!(instance is TType))
				AssignException<InvalidOperationException>();
			return this;
		}


		#endregion

		#region Interface IShieldWith & IShieldRaise

		public void Raise(Exception innerException = null)
		{
			try
			{
				if (_exceptionType == null) return;

				// construct exception with message				
				var parsedMessage = ParseMessage();
				Exception exception = null;

				if (parsedMessage != null)
				{
					if (innerException != null)
						exception = (Exception)Activator.CreateInstance(_exceptionType, new object[] { parsedMessage, innerException });
					else
						exception = (Exception)Activator.CreateInstance(_exceptionType, parsedMessage);
				}
				else
				{
					if (innerException != null)
						exception = (Exception)Activator.CreateInstance(_exceptionType, new object[] { _messageParameters, innerException });
					else
						exception = (Exception)Activator.CreateInstance(_exceptionType, _messageParameters);
				}

        //var exception = parsedMessage != null
        //	? (Exception)Activator.CreateInstance(_exceptionType, new object[] { parsedMessage, innerException })
        //	: (Exception)Activator.CreateInstance(_exceptionType, new object[] { _messageParameters, innerException });

        // perform logging if log callback is provided
        _callback?.Invoke(exception);

        throw exception;
			}
			finally
			{
				_exceptionType = null;
				_message = null;
				_isCustomMessage = false;
				_messageParameters = null;
			}
		}

		public void Raise<TException>(Exception innerException)
			where TException : Exception
		{
			if (_exceptionType != null)
				AssignException<TException>(overrideMessage: true);
			Raise(innerException);
		}

		public IShieldWith WithMessage(string message)
		{
			_message = message;
			_isCustomMessage = true;
			return this;
		}

		public IShield And
		{
			get
			{
				return this;
			}
		}

		public IShieldRaise WithParameters(params object[] parameters)
		{
			_messageParameters = parameters;
			return this;
		}

		#endregion

	}
}
