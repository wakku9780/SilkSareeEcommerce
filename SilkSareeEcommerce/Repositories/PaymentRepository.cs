using SilkSareeEcommerce.Data;
using SilkSareeEcommerce.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace SilkSareeEcommerce.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment> GetPaymentByTransactionIdAsync(string transactionId)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.TransactionId == transactionId);
        }

        public async Task UpdatePaymentStatusAsync(string transactionId, string status)
        {
            var payment = await GetPaymentByTransactionIdAsync(transactionId);
            if (payment != null)
            {
                payment.Status = status;
                await _context.SaveChangesAsync();
            }
        }
    }
}
