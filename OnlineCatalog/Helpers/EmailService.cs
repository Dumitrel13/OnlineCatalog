using System.Net;
using Microsoft.Extensions.Localization;
using OnlineCatalog.Helpers.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace OnlineCatalog.Helpers
{
    public class EmailService : IEmailService
    {
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly string _sendGridKey;

        [ActivatorUtilitiesConstructor]
        public EmailService(IStringLocalizer<SharedResources> localizer, IConfiguration configuration)
        {
            _localizer = localizer;
            _sendGridKey = configuration.GetSection("SendGridMailSender")["ApiKey"]!;
        }

        //public async Task<bool> SendEmail(string emailAddress, string password)
        //{
        //    var apiKey = _configuration.GetSection("SendgridMailSender")["ApiKey"];

        //    var client = new SendGridClient(apiKey);

        //    var from = new EmailAddress("phantasmkooky@gmail.com", "No replay");
        //    var to = new EmailAddress(emailAddress, "Receiver");
        //    var subject = "Account information";
        //    var plainTextcontent =
        //        $"Your account has been created. You can connect using this information email: {emailAddress} password: {password}";
        //    var htmlContent = $"<p>Your account has been created. You can connect using this information email: {emailAddress} password: {password}</p>";

        //    var message = MailHelper.CreateSingleEmail(from, to, subject, plainTextcontent, htmlContent);

        //    var response = await client.SendEmailAsync(message);
        //    return response.StatusCode == System.Net.HttpStatusCode.Accepted;
        //}

        public async Task<bool> AccountCreationEmail(string emailAddress, string password)
        {
            var client = new SendGridClient(_sendGridKey);

            var msg = new SendGridMessage()
            {
                From = new EmailAddress("phantasmkooky@gmail.com", "Account created"),
                Subject = "Account information",
                PlainTextContent = $"<p>Your account has been created. You can connect using this information email: {emailAddress} password: {password}</p>",
                HtmlContent = $"<p>Your account has been created. You can connect using this information email: {emailAddress} password: {password}</p>",
            };
            msg.AddTo(new EmailAddress(emailAddress));

            msg.SetClickTracking(false, false);
            var response = await client.SendEmailAsync(msg);

            return response.StatusCode == HttpStatusCode.Accepted;
        }
    }
}
