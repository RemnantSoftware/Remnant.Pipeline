using Remnant.Pipeline.Core;
using Remnant.Pipeline.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Remnant.Pipeline.Tests.Events
{
    public class OnSomeEvent : IEvent
    {
			public string Text { get; set; }
    }
}
