using Remnant.Pipeline.Core;
using Remnant.Pipeline.Interfaces;

namespace Remnant.Pipeline.Events
{
	public class OnExitStage : AsyncEvent
	{
		public IPipelineStage Stage { get; set; }
	}
}