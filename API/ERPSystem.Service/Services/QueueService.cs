using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using ERPSystem.Common;
using ERPSystem.DataModel.Setting;

namespace ERPSystem.Service;

public interface IQueueService
{
    bool Publish(string routingKey, byte[] message, byte deliveryMode = 2);
    bool Publish(string routingKey, string message, byte deliveryMode = 2);
    void PublishToSpecificQueue(string queue, string message);
    void Dispose();
}

public class QueueService : IQueueService, IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    private readonly ILogger _logger;

    public QueueService(IConfiguration configuration, string queueName = "queue_service_connection")
    {
        _configuration = configuration;
        _logger = ApplicationVariables.LoggerFactory.CreateLogger<QueueService>();
        var factory = GetConnectionFactory();
        if (factory != null)
        {
            _connection = factory.CreateConnection(queueName);
        }
    }

    private ConnectionFactory? GetConnectionFactory()
    {
        var queueSetting = _configuration.GetSection(Common.Infrastructure.Constants.Settings.DefineQueueConnectionSettings).Get<QueueConnectionSetting>();
        if (queueSetting != null)
        {
            return new ConnectionFactory()
            {
                HostName = queueSetting.Host,
                VirtualHost = queueSetting.VirtualHost,
                Port = queueSetting.Port,
                UserName = queueSetting.UserName,
                Password = queueSetting.Password,
            };
        }

        return null;
    }

    public bool Publish(string routingKey, string message, byte deliveryMode = 2)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        return Publish(routingKey, data, deliveryMode);
    }
    
    public bool Publish(string routingKey, byte[] message, byte deliveryMode = 2)
    {
        try
        {
            using (IModel channel = _connection.CreateModel())
            {
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = deliveryMode;
                properties.Expiration = Common.Infrastructure.Constants.RabbitMqConfig.MessageTimeToLive.ToString();

                channel.BasicPublish(exchange: Common.Infrastructure.Constants.RabbitMqConfig.ExchangeName,
                    routingKey: routingKey,
                    basicProperties: properties,
                    body: message);
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message + ex.StackTrace);
            return false;
        }
    }

    
        /// <summary>
        /// Publish message to a specific queue
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="message"></param>
        public void PublishToSpecificQueue(string queue, string message)
        {
            try
            {
                using (var channel = _connection.CreateModel())
                {
                    var byteMessage = Encoding.UTF8.GetBytes(message);
                    channel.QueueDeclare(queue: queue,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "",
                        routingKey: queue,
                        basicProperties: properties,
                        body: byteMessage);
                
                    channel.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + ex.StackTrace);
            }
        }

    public void Dispose()
    {
        try
        {
            if(_connection.IsOpen) _connection.Close();
            _connection.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message + ex.StackTrace);
        }
    }
}