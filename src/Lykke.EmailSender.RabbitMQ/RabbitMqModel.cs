using System;
using System.Linq;

namespace Lykke.EmailSender.RabbitMQ
{
    public class EmailRabbitMqContract
    {
        public class EmailRabbitMqAttachmentContract
        {
            public string FileName { get; set; }
            public string Mime { get; set; }
            public string Data { get; set; }
            public static EmailRabbitMqAttachmentContract Create(EmailAttachment src)
            {
                return new EmailRabbitMqAttachmentContract
                {
                    FileName = src.FileName,
                    Data =  Convert.ToBase64String(src.Data),
                    Mime = src.Mime
                };
            }
            public EmailAttachment ToDomain()
            {
                return new EmailAttachment
                {
                    FileName = FileName,
                    Mime = Mime,
                    Data = Convert.FromBase64String(Data)
                };
            }
        }

        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; }
        public EmailRabbitMqAttachmentContract[] Attachments { get; set; }

    }



    public static class DomainContractMapper
    {

        public static EmailRabbitMqContract ToContract(this EmailModel model)
        {
            return new EmailRabbitMqContract
            {
                Subject = model.Subject,
                Body = model.Body,
                IsHtml = model.IsHtml,
                Attachments =
                    model
                    .Attachments
                    .Select(EmailRabbitMqContract.EmailRabbitMqAttachmentContract.Create)
                    .ToArray()
            };
        }

        public static EmailModel ToDomain(this EmailRabbitMqContract src)
        {
            return new EmailModel
            {
                Subject = src.Subject,
                Body = src.Body,
                IsHtml = src.IsHtml,
                Attachments = src
                .Attachments
                .Select(a => a.ToDomain())
                .ToArray()
            };
        }

    }



}
