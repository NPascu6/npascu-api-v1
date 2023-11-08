using npascu_api_v1.Services.Email.Interface;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace npascu_api_v1.Services.Email.Implementation
{
    public class EmailService: IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string message)
        {
            var sendGridApiKey = _config.GetSection("SMTP").GetSection("Sendgrid").GetSection("ApiKey").Value;
            var senderEmail = _config.GetSection("SMTP").GetSection("Sendgrid").GetSection("SenderEmail").Value;
            var senderName = _config.GetSection("SMTP").GetSection("Sendgrid").GetSection("SenderName").Value;

            var client = new SendGridClient(sendGridApiKey);

            var from = new EmailAddress(senderEmail, senderName);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);

            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                return true;
            }
            else
            {
                // Handle the error
                throw new Exception($"Email sending failed: {response.StatusCode}");
            }
        }
    }
}
