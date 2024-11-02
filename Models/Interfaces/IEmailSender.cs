public interface IEmailSender
{
    Task SendConfirmationEmailAsync(string email, string subject, string message);
    Task SendReceiptEmailAsync(string email, string subject, string orderNumber, string customerName, List<MarkRestaurant.Models.Order> orderItems, double totalPrice);
    Task SendForgotPasswordEmailAsync(string email, string resetLink);
    Task SendPasswordChangedEmailAsync(string email);
}
