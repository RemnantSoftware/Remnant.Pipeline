using Remnant.Pipeline.Core;

namespace Remnant.Pipeline.Interfaces
{
	public interface IConnector
	{
		IConnectorForEvent ForStages(params string[] stages);

		IConnectorForEvent ForEvent<TEvent>() where TEvent : IEvent;
	}
}