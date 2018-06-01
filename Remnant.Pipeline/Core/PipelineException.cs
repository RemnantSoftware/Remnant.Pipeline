using Remnant.Pipeline.Interfaces;
using System;

namespace Remnant.Pipeline.Core
{
	public class PipelineException : Exception
	{
		public PipelineException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public IPipelineStage CurrentStage { get; set; }
		public IEvent CurrentEvent { get; set; }
	}
}