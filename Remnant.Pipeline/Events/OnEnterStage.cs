using Remnant.Pipeline.Core;
using Remnant.Pipeline.Interfaces;

namespace Remnant.Pipeline.Events
{
	public class OnEnterStage : AsyncEvent
	{
		public IPipelineStage Stage { get; set; }
	}
}