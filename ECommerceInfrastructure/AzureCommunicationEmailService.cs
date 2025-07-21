using Azure;
using Azure.Communication.Email;
using Azure.Core.Diagnostics;
using Azure.Identity;
using ECommerceInfrastructureAbstraction;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using System.Diagnostics.Tracing;
using System.Net;
using System.Runtime.CompilerServices;

namespace ECommerceInfrastructure
{
    public class AzureCommunicationEmailService : IEmailService
    {
        private readonly AzureEmailCommunicationSettings azureEmailCommunicationSettings;

        private readonly EmailClient emailClient;
        public AzureCommunicationEmailService(IOptions<AzureEmailCommunicationSettings> options)
        {
            azureEmailCommunicationSettings = options.Value;
        }

        public async Task SendEmailAsync(string toEmail)
        {
            var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
            {
                ExcludeEnvironmentCredential = true,
                ExcludeManagedIdentityCredential = true,
                ExcludeSharedTokenCacheCredential = true,
                //ExcludeAzureCliCredential = true,
                ExcludeInteractiveBrowserCredential = true,
                ExcludeAzurePowerShellCredential = true,
                ExcludeVisualStudioCredential = true,
            });

            
           // AzureEventSourceListener listener = AzureEventSourceListener.CreateConsoleLogger(EventLevel.Verbose);


            var emailClient = new EmailClient(new Uri(azureEmailCommunicationSettings.UrI), credential);

            var emailContent = new EmailContent($"Order Confirmation - Order ")
            {
                PlainText = $"Thank you for your order! Your order  has been received.",
                Html = $"<h1>Order Confirmation</h1><pTaking you for your order! Your order  has been received.</p>"
            };
            var sendResult = await emailClient.SendAsync(WaitUntil.Completed,new EmailMessage(azureEmailCommunicationSettings.EmailFrom, toEmail, emailContent));
        }
    }
}
