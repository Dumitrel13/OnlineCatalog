namespace OnlineCatalog.Helpers.Interfaces
{
    public interface IEmailService
    {
        Task<bool> AccountCreationEmail(string emailAddress, string password);
    }
}
