using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SilkSareeEcommerce.Models;
using SilkSareeEcommerce.Repositories;
using Microsoft.Extensions.Logging;

namespace SilkSareeEcommerce.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly PdfService _pdfService;
        private readonly EmailService _emailService;
        private readonly EmailTemplateService _emailTemplateService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository, 
            PdfService pdfService, 
            EmailService emailService,
            EmailTemplateService emailTemplateService,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _pdfService = pdfService;
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
            _logger = logger;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllOrdersAsync();
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            return await _orderRepository.AddAsync(order);
        }

        public async Task<List<Order>> GetUserOrdersAsync(string userId)
        {
            return await _orderRepository.GetOrdersByUserIdAsync(userId);
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _orderRepository.GetByIdAsync(orderId);
        }
        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            return await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus);
        }
        public async Task<Order?> CreateOrderAsync(string userId, List<CartItem> cartItems, string paymentMethod)
        {
            if (cartItems == null || !cartItems.Any()) return null;

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = cartItems.Sum(item => item.Quantity * item.Product.Price),
                PaymentMethod = paymentMethod, // ✅ Payment method save kar raha hai
                Status = (paymentMethod == "COD") ? "Confirmed" : "Pending", // ✅ COD ke liye Confirmed, PayPal ke liye Pending
                OrderItems = cartItems.Select(item => new OrderItem
                {
                    ProductId = item.Product.Id,
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                }).ToList()
            };

            return await _orderRepository.CreateOrderAsync(order);
        }


        public async Task<Order?> CreateOrderAsync(string userId, List<CartItem> cartItems, string paymentMethod, int shippingAddressId)
        {
            if (cartItems == null || !cartItems.Any()) return null;

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = cartItems.Sum(item => item.Quantity * item.Product.Price),
                PaymentMethod = paymentMethod, // ✅ Payment method save kar raha hai
                Status = (paymentMethod == "COD") ? "Confirmed" : "Pending", // ✅ COD ke liye Confirmed, PayPal ke liye Pending
                ShippingAddressId = shippingAddressId,  // Added shippingAddressId here
                OrderItems = cartItems.Select(item => new OrderItem
                {
                    ProductId = item.Product.Id,
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                }).ToList()
            };

            return await _orderRepository.CreateOrderAsync(order);
        }



        //public async Task<Order?> CreateOrderAsync(string userId, List<CartItem> cartItems)
        //{
        //    if (cartItems == null || !cartItems.Any()) return null;

        //    var order = new Order
        //    {
        //        UserId = userId,
        //        OrderDate = DateTime.UtcNow,
        //        TotalAmount = cartItems.Sum(item => item.Quantity * item.Product.Price),
        //        OrderItems = cartItems.Select(item => new OrderItem
        //        {
        //            ProductId = item.Product.Id,
        //            Quantity = item.Quantity,
        //            Price = item.Product.Price
        //        }).ToList()
        //    };

        //    return await _orderRepository.CreateOrderAsync(order);
        //}




        public async Task<Order?> CreateOrderFromBuyNowAsync(string userId, Product product, int quantity, string paymentMethod)
        {
            if (product.Quantity < quantity)
            {
                return null;
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                PaymentMethod = paymentMethod,
                TotalAmount = product.Price * quantity,
                Status = "Pending",
                OrderItems = new List<OrderItem>
        {
            new OrderItem
            {
                ProductId = product.Id,
                Quantity = quantity,
                Price = product.Price
            }
        }
            };

            // Reduce stock
            

            
            return await _orderRepository.CreateOrderAsync(order);
        }




        public Task ConfirmOrderAsync(string userId, IEnumerable<CartItem> cartItems)
        {
           // var order = await CreateOrderAsync(userId,   cartItems.ToList());
            //if (order != null)
            //{
            //    // Order confirmed, ab database me save karna hoga
            //    await _orderRepository.CreateOrderAsync(order);
            //}
            return Task.CompletedTask;
        }


        //public async Task<byte[]> GenerateOrderInvoiceAsync(int orderId)
        //{
        //    var order = await _orderRepository.GetOrderWithDetailsAsync(orderId);
        //    if (order == null) throw new Exception("Order not found!");

        //    return _pdfService.GenerateOrderPdf(order); // You'll create this in PDF service
        //}

        public async Task<byte[]> GenerateOrderInvoiceAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderWithDetailsAsync(orderId);
            if (order == null) throw new Exception("Order not found!");

            // Access the ShippingAddress from the order (as it's a navigation property)
            var shippingAddress = order.ShippingAddress?.Address ?? "Address not specified";

            // Now, generate the invoice using the shipping address and other order details
            return _pdfService.GenerateOrderPdf(order, shippingAddress); // You can pass the shipping address too if required
        }

        /// <summary>
        /// Sends order confirmation email to the customer
        /// </summary>
        public async Task<bool> SendOrderConfirmationEmailAsync(Order order, ApplicationUser user, string shippingAddress)
        {
            try
            {
                if (string.IsNullOrEmpty(user.Email))
                {
                    _logger.LogWarning($"User {user.Id} does not have an email address");
                    return false;
                }

                // Generate HTML email content using template
                var emailContent = await _emailTemplateService.GetOrderConfirmationEmailAsync(order, user, shippingAddress);
                
                var subject = $"🎉 Order Confirmation - Order #{order.Id} - Silk Saree";
                
                // Send the email
                var emailSent = await _emailService.SendEmailAsync(user.Email, subject, emailContent);
                
                if (emailSent)
                {
                    _logger.LogInformation($"Order confirmation email sent successfully to {user.Email} for order #{order.Id}");
                }
                else
                {
                    _logger.LogError($"Failed to send order confirmation email to {user.Email} for order #{order.Id}");
                }
                
                return emailSent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending order confirmation email for order #{order.Id}: {ex.Message}");
                return false;
            }
        }






    }
}
