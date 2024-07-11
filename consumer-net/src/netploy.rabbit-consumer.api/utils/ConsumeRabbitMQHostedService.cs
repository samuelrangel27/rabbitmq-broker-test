using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace netploy.rabbit_consumer.api;

public class ConsumeRabbitMQHostedService : BackgroundService
{
  private readonly ILogger _logger;
  private IConnection _connection;
  private IModel _channel;
  private readonly RabbitMQSettings _rabbitMQSettings;

  public ConsumeRabbitMQHostedService(ILoggerFactory loggerFactory, IOptions<RabbitMQSettings> options)
  {
    this._logger = loggerFactory.CreateLogger<ConsumeRabbitMQHostedService>();
    this._rabbitMQSettings = options.Value;
    InitRabbitMQ();
  }

  private void InitRabbitMQ()
  {
    var factory = new ConnectionFactory
    {
      HostName = _rabbitMQSettings.HostName,
      UserName = _rabbitMQSettings.UserName,
      Password = _rabbitMQSettings.Password
    };

    // create connection
    _connection = factory.CreateConnection();

    // create channel
    _channel = _connection.CreateModel();

    // _channel.ExchangeDeclare("demo.exchange", ExchangeType.Topic);
    _channel.QueueDeclare(queue: _rabbitMQSettings.QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
    // _channel.QueueBind("demo.queue.log", "demo.exchange", "demo.queue.*", null);
    // _channel.BasicQos(0, 1, false);

    _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
  }

  protected override Task ExecuteAsync(CancellationToken stoppingToken)
  {
    stoppingToken.ThrowIfCancellationRequested();

    var consumer = new EventingBasicConsumer(_channel);
    consumer.Received += (ch, ea) =>
    {
      // received message
      var content = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());

      // handle the received message
      HandleMessage(content);
      _channel.BasicAck(ea.DeliveryTag, false);
    };

    consumer.Shutdown += OnConsumerShutdown;
    consumer.Registered += OnConsumerRegistered;
    consumer.Unregistered += OnConsumerUnregistered;
    consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

    _channel.BasicConsume(_rabbitMQSettings.QueueName, false, consumer);
    return Task.CompletedTask;
  }

  private void HandleMessage(string content)
  {
    // we just print this message
    _logger.LogInformation($"consumer received {content}");
  }

  private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
  private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
  private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
  private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
  private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

  public override void Dispose()
  {
    _channel.Close();
    _connection.Close();
    base.Dispose();
  }
}
