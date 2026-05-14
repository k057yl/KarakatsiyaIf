using brevo_csharp.Api;
using brevo_csharp.Model;
using Karakatsiya.Services.Interfaces;
using Microsoft.Extensions.Localization;
using Configuration = brevo_csharp.Client.Configuration;
using Task = System.Threading.Tasks.Task;

namespace Karakatsiya.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly TransactionalEmailsApi _brevoApi;

        public EmailService(IConfiguration config, ILogger<EmailService> logger, IStringLocalizer<SharedResource> localizer)
        {
            _logger = logger;
            _localizer = localizer;

            var apiKey = config["Brevo:ApiKey"] ?? throw new ArgumentNullException("Brevo ApiKey missing");
            _fromEmail = config["Brevo:FromEmail"] ?? throw new ArgumentNullException("Brevo FromEmail missing");
            _fromName = config["Brevo:FromName"] ?? throw new ArgumentNullException("Brevo FromName missing");

            var brevoConfig = new Configuration();
            brevoConfig.ApiKey.Add("api-key", apiKey);

            _brevoApi = new TransactionalEmailsApi(brevoConfig);
        }

        public async Task SendEmailAsync(string toEmail, string subjectKey, string bodyKey, params object[] args)
        {
            var subject = _localizer[subjectKey].Value;
            var bodyTemplate = _localizer[bodyKey].Value;
            var htmlContent = args.Length > 0 ? string.Format(bodyTemplate, args) : bodyTemplate;

            var sendSmtpEmail = new SendSmtpEmail(
                sender: new SendSmtpEmailSender(_fromName, _fromEmail),
                to: new List<SendSmtpEmailTo> { new SendSmtpEmailTo(toEmail) },
                subject: subject,
                htmlContent: htmlContent
            );

            try
            {
                var result = await _brevoApi.SendTransacEmailAsync(sendSmtpEmail);
                _logger.LogInformation("Письмо улетело на {Email}, messageId: {Id}", toEmail, result.MessageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Пиздец, не удалось отправить письмо на {Email}", toEmail);
            }
        }
    }
}