using System.Text;
using SilkSareeEcommerce.Models;

namespace SilkSareeEcommerce.Services
{
    public class EmailTemplateService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmailTemplateService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> GetOrderConfirmationEmailAsync(Order order, ApplicationUser user, string shippingAddress)
        {
            var templatePath = Path.Combine(_webHostEnvironment.WebRootPath, "email-templates", "order-confirmation.html");
            
            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Email template not found at: {templatePath}");
            }

            var template = await File.ReadAllTextAsync(templatePath);
            
            // Build order items HTML
            var orderItemsHtml = new StringBuilder();
            foreach (var item in order.OrderItems)
            {
                orderItemsHtml.AppendLine($@"
                    <div class='product-item'>
                        <div class='product-name'>{item.Product?.Name ?? "Product"}</div>
                        <div class='product-price'>₹{item.Price} x {item.Quantity}</div>
                    </div>");
            }

            // Replace placeholders in template
            var emailContent = template
                .Replace("{{CustomerName}}", user.FullName ?? user.UserName ?? "Valued Customer")
                .Replace("{{OrderNumber}}", order.Id.ToString())
                .Replace("{{OrderDate}}", order.OrderDate.ToString("dddd, MMMM dd, yyyy"))
                .Replace("{{OrderItems}}", orderItemsHtml.ToString())
                .Replace("{{TotalAmount}}", order.TotalAmount.ToString("C", new System.Globalization.CultureInfo("en-IN")))
                .Replace("{{PaymentMethod}}", order.PaymentMethod ?? "Not specified")
                .Replace("{{OrderStatus}}", order.Status ?? "Pending")
                .Replace("{{ShippingAddress}}", shippingAddress)
                .Replace("{{WebsiteUrl}}", "https://silksareeecommerce-14.onrender.com");

            return emailContent;
        }
    }
}
