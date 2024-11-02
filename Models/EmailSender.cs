using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendConfirmationEmailAsync(string email, string subject, string message)
    {
        var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
        {
            Port = int.Parse(_configuration["EmailSettings:SmtpPort"]),
            Credentials = new NetworkCredential(_configuration["EmailSettings:SmtpUser"], _configuration["EmailSettings:SmtpPass"]),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_configuration["EmailSettings:SmtpUser"]),
            Subject = subject,
            IsBodyHtml = true,
        };

        string messageBody = $@"
    <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #1e1c21;
                    color: #ffffff;
                    margin: 0;
                    padding: 20px;
                }}
                .container {{
                    max-width: 600px;
                    margin: auto;
                    background-color: #2c2a2e;
                    border-radius: 8px;
                    padding: 30px;
                    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.5);
                }}
                h1 {{
                    color: #00bcd4;
                    text-align: center;
                }}
                p {{
                    font-size: 16px;
                    line-height: 1.5;
                    color: #ffffff;
                    text-align: center;
                }}
                a {{
                    display: inline-block; 
                    color: #fcf400;
                    text-decoration: none;
                    font-weight: bold;
                    padding: 15px 20px;
                    border: 2px solid #fcf400;
                    border-radius: 5px;
                    background-color: #00bcd4; 
                    transition: background-color 0.3s, color 0.3s, border-color 0.3s;
                    margin-top: 20px; 
                    cursor: pointer;
                }}
                a:hover {{
                    background-color: #ffdd57; 
                    color: #2c2a2e;
                }}
                .footer {{
                    margin-top: 20px;
                    font-size: 14px;
                    text-align: center;
                }}
                .footer p {{
                    color: #b0b0b0; 
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h1>Mark Restaurant</h1>
                <p>{message}</p>
                <div class='footer'>
                    <p>Thank you for choosing Mark Restaurant!</p>
                    <p>We look forward to serving you.</p>
                </div>
            </div>
        </body>
    </html>";

        mailMessage.Body = messageBody;
        mailMessage.To.Add(email);

        await smtpClient.SendMailAsync(mailMessage);
    }

    public async Task SendReceiptEmailAsync(string email, string subject, string orderNumber, string customerName, List<MarkRestaurant.Models.Order> orders, double totalPrice)
    {
        var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
        {
            Port = int.Parse(_configuration["EmailSettings:SmtpPort"]),
            Credentials = new NetworkCredential(_configuration["EmailSettings:SmtpUser"], _configuration["EmailSettings:SmtpPass"]),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_configuration["EmailSettings:SmtpUser"]),
            Subject = subject,
            IsBodyHtml = true,
        };

        string orderItemsHtml = string.Empty;
        foreach (var item in orders)
        {
            orderItemsHtml += $"<li><span class='order-item-title'>{item.Product.Title} - </span><span class='order-item-price'>₼{item.Product.Price}</span></li>";
        }

        string messageBody = $@"
    <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #1e1c21;
                    color: #ffffff;
                    margin: 0;
                    padding: 20px;
                }}
                .container {{
                    max-width: 600px;
                    margin: auto;
                    background-color: #2c2a2e;
                    border-radius: 8px;
                    padding: 30px;
                    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.5);
                }}
                h1 {{
                    color: #00bcd4;
                    text-align: center;
                }}
                p {{
                    font-size: 16px;
                    line-height: 1.5;
                    text-align: center;
                    color: #ffffff;
                }}
                .order-details {{
                    text-align: left;
                    margin-top: 20px;
                    color: #ffffff;
                }}
                ul {{
                    list-style-type: none;
                    padding: 0;
                }}
                ul li {{
                    margin-bottom: 10px;
                    color: #FFBE33;
                }}
                .total {{
                    font-size: 18px;
                    font-weight: bold;
                    color: #00bcd4;
                    text-align: center;
                }}
                .footer {{
                    margin-top: 20px;
                    font-size: 14px;
                    text-align: center;
                }}
                .footer p {{
                    color: #b0b0b0; 
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h1>Receipt from Mark Restaurant</h1>
                <p>Thank you for your order, {customerName}!</p>
                <div class='order-details'>
                    <p>Order Number: <strong>{orderNumber}</strong></p>
                    <p>Here is the summary of your order:</p>
                    <ul>
                        {orderItemsHtml}
                    </ul>
                    <p class='total'>Total Amount: ₼{totalPrice}</p>
                </div>
                <div class='footer'>
                    <p>We hope you enjoyed your meal!</p>
                    <p>Looking forward to your next visit to Mark Restaurant.</p>
                </div>
            </div>
        </body>
    </html>";

        mailMessage.Body = messageBody;
        mailMessage.To.Add(email);

        await smtpClient.SendMailAsync(mailMessage);
    }

    public async Task SendForgotPasswordEmailAsync(string email, string resetLink)
    {
        var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
        {
            Port = int.Parse(_configuration["EmailSettings:SmtpPort"]),
            Credentials = new NetworkCredential(_configuration["EmailSettings:SmtpUser"], _configuration["EmailSettings:SmtpPass"]),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_configuration["EmailSettings:SmtpUser"]),
            Subject = "Reset Your Password",
            IsBodyHtml = true,
        };

        string messageBody = $@"
    <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #1e1c21;
                    color: #ffffff;
                    margin: 0;
                    padding: 20px;
                }}
                .container {{
                    max-width: 600px;
                    margin: auto;
                    background-color: #2c2a2e;
                    border-radius: 8px;
                    padding: 30px;
                    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.5);
                }}
                h1 {{
                    color: #00bcd4;
                    text-align: center;
                }}
                p {{
                    font-size: 16px;
                    line-height: 1.5;
                    color: #ffffff;
                    text-align: center;
                }}
                .reset-button {{
                    display: block;
                    margin: 0 auto;
                    width: fit-content;
                    color: #fcf400;
                    text-decoration: none;
                    font-weight: bold;
                    padding: 15px 20px;
                    border: 2px solid #fcf400;
                    border-radius: 5px;
                    background-color: #00bcd4; 
                    transition: background-color 0.3s, color 0.3s, border-color 0.3s;
                    margin-top: 20px; 
                    cursor: pointer;
                }}
                .reset-button:hover {{
                    background-color: #ffdd57; 
                    color: #2c2a2e;
                }}
                .footer {{
                    margin-top: 20px;
                    font-size: 14px;
                    text-align: center;
                }}
                .footer p {{
                    color: #b0b0b0; 
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h1>Reset Your Password</h1>
                <p>You requested a password reset. Please click the link below to reset your password:</p>
                <a href='{resetLink}' class='reset-button'>Reset Password</a>
                <div class='footer'>
                    <p>If you did not request this, please ignore this email.</p>
                    <p>Thank you for choosing Mark Restaurant!</p>
                </div>
            </div>
        </body>
    </html>";

        mailMessage.Body = messageBody;
        mailMessage.To.Add(email);

        await smtpClient.SendMailAsync(mailMessage);
    }

    public async Task SendPasswordChangedEmailAsync(string email)
    {
        var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
        {
            Port = int.Parse(_configuration["EmailSettings:SmtpPort"]),
            Credentials = new NetworkCredential(_configuration["EmailSettings:SmtpUser"], _configuration["EmailSettings:SmtpPass"]),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_configuration["EmailSettings:SmtpUser"]),
            Subject = "Your Password Has Been Changed",
            IsBodyHtml = true,
        };

        string messageBody = $@"
    <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #1e1c21;
                    color: #ffffff;
                    margin: 0;
                    padding: 20px;
                }}
                .container {{
                    max-width: 600px;
                    margin: auto;
                    background-color: #2c2a2e;
                    border-radius: 8px;
                    padding: 30px;
                    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.5);
                }}
                h1 {{
                    color: #00bcd4;
                    text-align: center;
                }}
                p {{
                    font-size: 16px;
                    line-height: 1.5;
                    color: #ffffff;
                    text-align: center;
                }}
                .footer {{
                    margin-top: 20px;
                    font-size: 14px;
                    text-align: center;
                }}
                .footer p {{
                    color: #b0b0b0; 
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h1>Your Password Has Been Changed</h1>
                <p>If you did not request this change, please contact support immediately.</p>
                <div class='footer'>
                    <p>Thank you for choosing Mark Restaurant!</p>
                </div>
            </div>
        </body>
    </html>";

        mailMessage.Body = messageBody;
        mailMessage.To.Add(email);

        await smtpClient.SendMailAsync(mailMessage);
    }

}
