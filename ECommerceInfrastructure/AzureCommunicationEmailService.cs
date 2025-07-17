using ECommerceInfrastructureAbstraction;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using System.Runtime.CompilerServices;

namespace ECommerceInfrastructure
{
    public class AzureCommunicationEmailService : IEmailService
    {
        private readonly AzureEmailCommunicationSettings azureEmailCommunicationSettings;

        public AzureCommunicationEmailService(IOptions<AzureEmailCommunicationSettings> options)
        {
            azureEmailCommunicationSettings = options.Value;
        }
        public Task SendEmailAsync(string toEmail)
        {
            throw new NotImplementedException();
        }
    }
}
