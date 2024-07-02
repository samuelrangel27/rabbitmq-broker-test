using System;
using netploy.rabbit_producer.api.models;

namespace netploy.rabbit_producer.api.utils
{
	public interface IRabbitMQService
	{
		void SendMessage(SampleMessageType sampleMessage);
	}
}

