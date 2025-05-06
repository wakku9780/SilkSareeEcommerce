using DinkToPdf;
using DinkToPdf.Contracts;
using SilkSareeEcommerce.Models;

namespace SilkSareeEcommerce.Services
{
    public class PdfService
    {

        private readonly IConverter _converter;

        public PdfService(IConverter converter)
        {
            _converter = converter;
        }

        public byte[] GenerateOrderPdf(Order order,string shippingAddress)
        {
            string htmlContent = $@"
            <html>
            <head><style>body{{ font-family: Arial; }}</style></head>
            <body>
                <h2>Order Invoice - #{order.Id}</h2>
                <p><strong>Date:</strong> {DateTime.Now}</p>
                <p><strong>Shipping Address:</strong> {order.ShippingAddress.Address}</p>
                <h3>Items:</h3>
                <ul>
                    {string.Join("", order.OrderItems.Select(item => $"<li>{item.Product.Name} - {item.Quantity} x ₹{item.Price}</li>"))}
                </ul>
                <p><strong>Total:</strong> ₹{order.TotalAmount}</p>
            </body>
            </html>";

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                    DocumentTitle = "Order Invoice"
                },
                Objects = {
                new ObjectSettings {
                    HtmlContent = htmlContent
                }
            }
            };

            return _converter.Convert(doc);
        }
    }
}
