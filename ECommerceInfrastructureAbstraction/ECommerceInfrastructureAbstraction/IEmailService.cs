namespace ECommerceInfrastructureAbstraction
{
    public interface IEmailService
    {
        Task SendEmailAsync(string fromEmail, string toEmail,string subject,string content);
    }
}
