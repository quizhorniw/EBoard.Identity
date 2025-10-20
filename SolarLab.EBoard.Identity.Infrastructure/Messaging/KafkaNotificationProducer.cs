using Confluent.Kafka;
using Newtonsoft.Json;
using SolarLab.EBoard.Identity.Application.Abstractions.Messaging;

namespace SolarLab.EBoard.Identity.Infrastructure.Messaging;

public class KafkaNotificationProducer : IMessageProducer
{
    private readonly IProducer<string, string> _producer;
    private const string Topic = "notifications";

    public KafkaNotificationProducer(IProducer<string, string> producer)
    {
        _producer = producer;
    }

    public async Task SendAsync(object message, CancellationToken cancellationToken = default)
    {
        try
        {
            var kafkaMessage = new Message<string, string>
            {
                Value = JsonConvert.SerializeObject(message)
            };

            await _producer.ProduceAsync(Topic, kafkaMessage, cancellationToken);
        }
        catch (ProduceException<string, string> e)
        {
            throw new ApplicationException("Error while producing notification message", e);
        }
    }
}