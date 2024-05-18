using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ERPSystem.Common;

using ERPSystem.Repository;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using ERPSystem.Common.Resources;
using ERPSystem.Common.Infrastructure;
using ERPSystem.DataModel;
using ERPSystem.DataModel.Setting;
using RabbitMQ.Client.Events;
using ERPSystem.Service.RabbitMq;
using ERPSystem.Service.RabbitMq;

namespace ERPSystem.Service;

public interface IConsumerService
{
    void Register();
}

public class ConsumerService : IConsumerService
{
    private readonly IConfiguration _configuration;
    private readonly HttpContext? _httpContext;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;


    public ConsumerService(IConfiguration configuration)
    {
        _configuration = configuration;
        _mapper = ApplicationVariables.Mapper;
        _logger = ApplicationVariables.LoggerFactory.CreateLogger<ConsumerService>();
    }

    public void Register()
    {
        // Register general consumer
        HandleThreadExportDataToFile(Common.Infrastructure.Constants.RabbitMqConfig.TopicExportMember);
    }


    /// <summary>
    /// Handler status for door
    /// </summary>
    /// <param name="routingKey"></param>
    private void HandleThreadExportDataToFile(string routingKey)
    {
        var factory = QueueHelper.GetConnectionFactory(_configuration);
        var _connection = factory.CreateConnection("queue_service_connection");
        var channel = _connection.CreateModel();
        var queueName = channel.QueueDeclare(queue: routingKey,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null).QueueName;
        channel.QueueBind(queue: queueName,
                exchange: Constants.RabbitMqConfig.ExchangeName,
                routingKey: routingKey,
                arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
           var threadExportConsumer = new ThreadExportConsumer(_configuration,new HttpContextAccessor(), _mapper);
                Console.WriteLine(ea.Body);
                threadExportConsumer.DoWork(ea.Body.ToArray());
                channel.BasicAck(ea.DeliveryTag, false);
        };
        channel.BasicConsume(queue: queueName,
            autoAck: false,
            consumerTag: "export_member",
            noLocal: false,
            exclusive: false,
            arguments: null,
            consumer: consumer
            );
    }
}
