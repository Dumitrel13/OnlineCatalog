using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace OnlineCatalog.Helpers
{
	public class EmailSender : IEmailSender
	{
        private readonly string _sendGridKey;

		public EmailSender(IConfiguration configuration)
		{
            _sendGridKey = configuration.GetSection("SendGridMailSender")["ApiKey"]!;

		}
		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			if (string.IsNullOrEmpty(_sendGridKey))
			{
				throw new Exception("Null SendGridKey");
			}
			await Execute(_sendGridKey, subject, htmlMessage, email);
		}

		public async Task Execute(string apiKey, string subject, string message, string toEmail)
		{
			var client = new SendGridClient(apiKey);
			var msg = new SendGridMessage()
			{
				From = new EmailAddress("phantasmkooky@gmail.com", "Password Recovery"),
				Subject = subject,
				PlainTextContent = message,
				HtmlContent = message
			};
			msg.AddTo(new EmailAddress(toEmail));

			msg.SetClickTracking(false, false);
			var response = await client.SendEmailAsync(msg);
		}
	}
}
