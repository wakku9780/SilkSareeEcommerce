using CloudinaryDotNet;
using PayPal.Api;
using SilkSareeEcommerce.Repositories;
using System;

namespace SilkSareeEcommerce.Services
{
    public class PayPalService
    {
        private readonly IConfiguration _config;
        private readonly IPaymentRepository _paymentRepository;

        public PayPalService(IConfiguration config, IPaymentRepository paymentRepository)
        {
            _config = config;
            _paymentRepository = paymentRepository;
        }

        private APIContext GetAPIContext()
        {
            var clientId = _config["PayPal:ClientId"];
            var clientSecret = _config["PayPal:Secret"];
            var mode = _config["PayPal:Mode"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(mode))
            {
                throw new InvalidOperationException("PayPal configuration is incomplete. Please check your appsettings.json file.");
            }

            var config = new Dictionary<string, string>
        {
            { "mode", mode }
        };

            var accessToken = new OAuthTokenCredential(clientId, clientSecret, config).GetAccessToken();
            return new APIContext(accessToken) { Config = config };
        }

        public PayPal.Api.Payment CreatePayment(int orderId, decimal amount, string currency, string returnUrl, string cancelUrl)
        {
            var apiContext = GetAPIContext();

            var payment = new PayPal.Api.Payment
            {
                intent = "sale",
                payer = new Payer { payment_method = "paypal" },
                transactions = new List<Transaction>
        {
            new Transaction
            {
                amount = new Amount
                {
                    total = amount.ToString("F2"), // Format to 2 decimal places
                    currency = currency
                },
                description = "Saree Purchase"
            }
        },
                redirect_urls = new RedirectUrls
                {
                    return_url = returnUrl,
                    cancel_url = cancelUrl
                }
            };

            var createdPayment = payment.Create(apiContext);
            return createdPayment; // ✅ PayPal API ka response return karo
        }

    }
}
