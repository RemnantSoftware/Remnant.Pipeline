using Remnant.Pipeline.Core;

namespace Remnant.Pipeline.Tests.Events
{
	public class OnParseAsyncEvent : AsyncEvent
    {
			public string Text { get; set; }
    }
}
