using System;
using Remnant.Pipeline.Core;
using Remnant.Pipeline.Interfaces;

namespace Remnant.Pipeline.Events
{
	public class OnException : IEvent
	{
		public OnException()
		{
		}

		public OnException(Exception e)
		{
			Exception = e;
		}

		public Exception Exception { get; private set; }
	}
}