using System;
using Remnant.Pipeline.Core;
using Remnant.Pipeline.Interfaces;

namespace Remnant.Pipeline.Events
{
	public class OnAbortPipeline : IEvent
	{
		public Exception Exception { get; set; }
	}
}