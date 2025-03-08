using System;
using System.Threading.Tasks;
using SilkSareeEcommerce.Models;
using SilkSareeEcommerce.Repositories;

namespace SilkSareeEcommerce.Services
{
    public class PaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        //public async Task<Payment> ProcessPaymentAsync(Payment payment)
        //{
        //    payment.Status = "Completed"; // Razorpay ya PayPal ke response ke according update hoga
        //    return await _paymentRepository.AddAsync(payment);
        //}
    }
}
