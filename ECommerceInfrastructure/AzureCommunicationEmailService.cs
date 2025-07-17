using Azure;
using Azure.Communication.Email;
using Azure.Identity;
using ECommerceInfrastructureAbstraction;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using System.Net;
using System.Runtime.CompilerServices;

namespace ECommerceInfrastructure
{
    public class AzureCommunicationEmailService : IEmailService
    {
        private readonly AzureEmailCommunicationSettings azureEmailCommunicationSettings;

        private readonly EmailClient emailClient;
        public AzureCommunicationEmailService()
        {
            emailClient = new EmailClient(new Uri(""), new DefaultAzureCredential());
        }

        public async Task SendEmailAsync(string toEmail)
        {
            var emailContent = new EmailContent($"Order Confirmation - Order ")
            {
                PlainText = $"Thank you for your order! Your order  has been received.",
                Html = $"<h1>Order Confirmation</h1><pTaking you for your order! Your order  has been received.</p>"
            };
            var sendResult = await emailClient.SendAsync(WaitUntil.Completed,new EmailMessage( "fromEmail", toEmail, emailContent));
        }
    }
}
