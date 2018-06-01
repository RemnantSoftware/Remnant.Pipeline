using Remnant.Pipeline.Core;

namespace Remnant.Pipeline.Interfaces
{
	public interface IConnector
	{
		IConnectorForEvent ForStage(string stage);
		IConnectorForEvent ForEvent<TEvent>() where TEvent : IEvent;
	}
}