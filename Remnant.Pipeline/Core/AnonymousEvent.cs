

using Remnant.Pipeline.Interfaces;

namespace Remnant.Pipeline.Core
{
	public class AnonymousEvent<TObject> : IEvent
	{
		public TObject Object { get; set; }
	}
}
