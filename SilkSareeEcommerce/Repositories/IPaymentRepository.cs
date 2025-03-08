using SilkSareeEcommerce.Models;
using System.Threading.Tasks;

namespace SilkSareeEcommerce.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task<Payment> GetPaymentByTransactionIdAsync(string transactionId);
        Task UpdatePaymentStatusAsync(string transactionId, string status);
    }
}
