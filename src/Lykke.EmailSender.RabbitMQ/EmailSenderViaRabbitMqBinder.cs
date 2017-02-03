using Common;
using Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Publisher;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.EmailSender.RabbitMQ
{

    public class EmailRabbitMqSerializer : IRabbitMqSerializer<EmailModel>
    {
        public byte[] Serialize(EmailModel model)
        {
            return model.ToContract().ToJson().ToUtf8Bytes();
        }
    }

    public static class EmailSenderViaRabbitMqBinder
    {

        public static void UseEmailSenderViaRabbitMq(this IServiceCollection serviceCollection,
            RabbitMqSettings rabbitMqSettings, ILog log)
        {

            var rabbitMqBroker
                = new RabbitMqPublisher<EmailModel>(rabbitMqSettings)
                    .SetLogger(log)
                    .SetSerializer(new EmailRabbitMqSerializer());

            serviceCollection.AddSingleton<IEmailSender>(
                new EmailSenderViaQueue(rabbitMqBroker));

            rabbitMqBroker.Start();
        }

    }


}
