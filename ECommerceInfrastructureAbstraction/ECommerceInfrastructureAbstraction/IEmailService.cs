namespace ECommerceInfrastructureAbstraction
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail);
    }
}
