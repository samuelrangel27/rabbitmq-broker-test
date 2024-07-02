using System;
namespace netploy.rabbit_producer.api.models
{
	public class SampleMessageType
	{
		public Guid MessageId { get; set; }
		public string SampleText { get; set; }
		public int SampleNumber { get; set; }
	}
}

