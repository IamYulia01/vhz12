using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VHZ_App.Models;
using System.Net.Mail;
using System.Net;

namespace VHZ_App.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly SmtpSettings _smtpSettings;

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _smtpSettings = configuration.GetSection("Smtp").Get<SmtpSettings>();
        }

        [BindProperty]
        public Feedback Feedback { get; set; } = new Feedback();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(_smtpSettings.User, "ОБРАТНАЯ СВЯЗЬ ВХЗ");
                    message.To.Add("buhoykrab@gmail.com");
                    message.Subject = $"Обратная связь: {Feedback.Subject}";

                    // Красивое HTML-письмо в фиолетовых тонах
                    message.IsBodyHtml = true;
                    message.Body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{
                            font-family: 'Arial', sans-serif;
                            line-height: 1.6;
                            color: #333;
                            max-width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                        }}
                        .header {{
                            background-color: #6a0dad;
                            color: white;
                            padding: 15px;
                            text-align: center;
                            border-radius: 5px 5px 0 0;
                        }}
                        .content {{
                            border: 1px solid #ddd;
                            border-top: none;
                            padding: 20px;
                            background-color: #f9f3ff;
                            border-radius: 0 0 5px 5px;
                        }}
                        .field {{
                            margin-bottom: 15px;
                            padding-bottom: 15px;
                            border-bottom: 1px dashed #d1b3ff;
                        }}
                        .field:last-child {{
                            border-bottom: none;
                            margin-bottom: 0;
                            padding-bottom: 0;
                        }}
                        .field-label {{
                            font-weight: bold;
                            color: #6a0dad;
                            display: block;
                            margin-bottom: 5px;
                        }}
                        .message-text {{
                            white-space: pre-line;
                            background-color: white;
                            padding: 10px;
                            border-radius: 3px;
                            border-left: 3px solid #6a0dad;
                        }}
                        .footer {{
                            margin-top: 20px;
                            font-size: 12px;
                            color: #888;
                            text-align: center;
                        }}
                    </style>
                </head>
                <body>
                    <div class='header'>
                        <h2>Новое сообщение с сайта Владимирского химического завода</h2>
                    </div>
                    
                    <div class='content'>
                        <div class='field'>
                            <span class='field-label'>Имя:</span>
                            {Feedback.Name}
                        </div>
                        
                        <div class='field'>
                            <span class='field-label'>Email:</span>
                            {Feedback.Email}
                        </div>
                        
                        <div class='field'>
                            <span class='field-label'>Телефон:</span>
                            {Feedback.Phone ?? "не указан"}
                        </div>
                        
                        <div class='field'>
                            <span class='field-label'>Тема:</span>
                            {Feedback.Subject}
                        </div>
                        
                        <div class='field'>
                            <span class='field-label'>Сообщение:</span>
                            <div class='message-text'>{Feedback.Message}</div>
                        </div>
                    </div>
                    
                    <div class='footer'>
                        Это письмо было отправлено автоматически. Пожалуйста, не отвечайте на него напрямую.
                    </div>
                </body>
                </html>
            ";

                    // Добавляем Reply-To на email отправителя
                    if (!string.IsNullOrEmpty(Feedback.Email))
                    {
                        message.ReplyToList.Add(new MailAddress(Feedback.Email, Feedback.Name));
                    }

                    using (var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
                    {
                        client.Credentials = new NetworkCredential(_smtpSettings.User, _smtpSettings.Pass);
                        client.EnableSsl = true;
                        await client.SendMailAsync(message);
                    }
                }

                TempData["SuccessMessage"] = "Спасибо за ваше сообщение! Мы свяжемся с вами в ближайшее время.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отправке сообщения");
                ModelState.AddModelError(string.Empty, "Произошла ошибка при отправке сообщения. Пожалуйста, попробуйте позже.");
                return Page();
            }
        }
    }
}