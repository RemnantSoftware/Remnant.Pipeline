using System.Collections.Generic;
using System.Collections.ObjectModel;
using Remnant.Pipeline.Interfaces;

namespace Remnant.Pipeline.Core
{
	internal class Connector : IConnector, IConnectorForEvent
	{
		private readonly List<Connection> _connections = new List<Connection>();
		private Connection _currentConnection;


		public IConnectorForEvent ForStage(string stage)
		{
			_currentConnection.Stage = new PipelineStage(stage); //??, _currentConnection.Pipeline);
			return this;
		}

		public IConnectorForEvent ForEvent<TEvent>() where TEvent : IEvent
		{
			_currentConnection.ForEvent<TEvent>();
			return this;
		}

		public IConnectorForEvent ForEvent(string eventNamePattern)
		{
			_currentConnection.ForEvent(eventNamePattern);
			return this;
		}

		public List<Connection> IsEventRegistered(PipelineStage stage, IEvent @event)
		{
			return _connections.FindAll(conn => conn.IsEventRegistered(@event));
		}

		public IConnector Connect(IPipeline pipeline)
		{
			_currentConnection = _connections.Find(connection => connection.Pipeline == pipeline);

			if (_currentConnection == null)
			{
				_currentConnection = new Connection(pipeline);
				_connections.Add(_currentConnection);

				pipeline.Configuration.AutoRaiseEvents(false);
				pipeline.RunAsync();
			}
			return this;
		}

		public ReadOnlyCollection<Connection> Connections { get { return new ReadOnlyCollection<Connection>(_connections); } }

	}
}