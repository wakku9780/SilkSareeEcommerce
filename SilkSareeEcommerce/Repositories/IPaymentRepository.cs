using SilkSareeEcommerce.Models;

namespace SilkSareeEcommerce.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment> AddAsync(Payment payment);  // Add a new payment
    }
}
