using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace IT2163_07.Models
{
    public class EmailSender
    {
        public EmailOptions emailOptions { get; set; }
        public EmailSender(IOptions<EmailOptions> optionsAccessor)
        {
            emailOptions = optionsAccessor.Value;
        }
        public async Task<bool> SendEmailAsync(string email, string subject, string confirmLink)
        {
            var apiKey = emailOptions.APIKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("daryl.pru@gmail.com", "NVYRO");
            var to = new EmailAddress(email, "Dear Customer,");
            var plainTextContent = "";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, confirmLink);
            var response = await client.SendEmailAsync(msg);
            Console.WriteLine(response.IsSuccessStatusCode);
            return response.IsSuccessStatusCode;
        }
    }
}
