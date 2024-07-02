using System;
using System.Text;
using Microsoft.Extensions.Options;
using netploy.rabbit_producer.api.models;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace netploy.rabbit_producer.api.utils
{
	public class RabbitMQService : IRabbitMQService
	{
        private readonly RabbitMQSettings _rabbitMQSettings;
        public RabbitMQService(IOptions<RabbitMQSettings> options)
        {
            this._rabbitMQSettings = options.Value;
        }
        public IConnection CreateChannel()
        {
            var connection = new ConnectionFactory()
            {
                UserName = _rabbitMQSettings.UserName,
                Password = _rabbitMQSettings.Password,
                HostName = _rabbitMQSettings.HostName
            };
            connection.DispatchConsumersAsync = true;
            var channel = connection.CreateConnection();
            return channel;
        }

        public void SendMessage(SampleMessageType sampleMessage)
        {
            using var connection = CreateChannel();
            using var model = connection.CreateModel();
            var content = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(sampleMessage));
            model.QueueDeclare(queue: _rabbitMQSettings.QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            model.BasicPublish("", _rabbitMQSettings.QueueName, basicProperties: null, body: content);
        }
    }
}

