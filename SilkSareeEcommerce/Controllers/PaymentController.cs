using Microsoft.AspNetCore.Mvc;
using SilkSareeEcommerce.Services;
using System.Threading.Tasks;
using PayPal.Api;  // Make sure this is included


namespace SilkSareeEcommerce.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PayPalService _payPalService;

        public PaymentController(PayPalService payPalService)
        {
            _payPalService = payPalService;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult CreatePayment()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment(int orderId, decimal amount)
        {
            string returnUrl = Url.Action("PaymentSuccess", "Payment", null, Request.Scheme);
            string cancelUrl = Url.Action("PaymentCancel", "Payment", null, Request.Scheme);

            // Call PayPal API to create a payment
            var payment = await _payPalService.CreatePaymentAsync(orderId, amount, "USD", returnUrl, cancelUrl);

            // Fix: Get the PayPal Payment approval URL
            var approvalUrl = payment.links.FirstOrDefault(l => l.rel == "approval_url")?.href;

            if (!string.IsNullOrEmpty(approvalUrl))
                return Redirect(approvalUrl);

            return View("PaymentCancel");
        }

        public IActionResult PaymentSuccess()
        {
            return View();
        }

        public IActionResult PaymentCancel()
        {
            return View();
        }
    }
}
