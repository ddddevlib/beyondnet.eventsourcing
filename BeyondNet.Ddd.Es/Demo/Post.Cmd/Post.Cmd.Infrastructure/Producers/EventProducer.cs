using System.Text.Json;
using BeyondNet.Cqrs.Producers.Interfaces;
using BeyondNet.Ddd.Interfaces;
using Confluent.Kafka;

using Microsoft.Extensions.Options;

namespace Post.Cmd.Infrastructure.Producers
{
    public class EventProducer : IEventProducer
    {
        private readonly ProducerConfig _config;

        public EventProducer(IOptions<ProducerConfig> config)
        {
            _config = config.Value;
        }

        public async Task ProduceAsync<T>(string topic, T @event) where T : IDomainEvent
        {
            using var producer = new ProducerBuilder<string, string>(_config)
                .SetKeySerializer(Serializers.Utf8)
                .SetValueSerializer(Serializers.Utf8)
                .Build();

            var eventMessage = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = JsonSerializer.Serialize(@event, @event.GetType())
            };

            var deliveryResult = await producer.ProduceAsync(topic, eventMessage);

            if (deliveryResult.Status == PersistenceStatus.NotPersisted)
            {
                throw new Exception($"Could not produce {@event.GetType().Name} message to topic - {topic} due to the following reason: {deliveryResult.Message}.");
            }
        }
    }
}