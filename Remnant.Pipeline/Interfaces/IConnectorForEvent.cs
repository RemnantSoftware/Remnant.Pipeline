using Remnant.Pipeline.Core;

namespace Remnant.Pipeline.Interfaces
{
	public interface IConnectorForEvent
	{
		IConnectorForEvent ForEvent<TEvent>() where TEvent : IEvent;

		/// <summary>
		/// Connect one pipeline with another
		/// </summary>
		/// <param name="pipeline"></param>
		/// <returns></returns>
		IConnector Connect(IPipeline pipeline);
	}
}