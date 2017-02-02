using System.Linq;
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
            var outModel = new
            {
                subject = model.Subject,
                body = model.Body,
                isHtml = model.IsHtml,
                attachments = model.Attachments?.Select(a => new
                {
                    fileName = a.FileName,
                    mime = a.Mime,
                    data = a.Data.ToBase64()
                })
            };

            return outModel.ToJson().ToUtf8Bytes();
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
