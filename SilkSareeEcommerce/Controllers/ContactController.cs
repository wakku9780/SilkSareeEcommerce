using Microsoft.AspNetCore.Mvc;
using SilkSareeEcommerce.Data;
using SilkSareeEcommerce.Models;
using SilkSareeEcommerce.Services;

namespace SilkSareeEcommerce.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public ContactController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("Contact/Submit")]
        public IActionResult Submit(Contact model)
        {
            if (ModelState.IsValid)
            {
                // Save to database
                _context.Contacts.Add(model);
                _context.SaveChanges();

                // Send Email to Admin
                string subject = "New Contact Form Submission";
                string body = $"<h2>New Contact Request</h2><p><strong>Name:</strong> {model.Name}</p><p><strong>Email:</strong> {model.Email}</p><p><strong>Message:</strong> {model.Message}</p>";

                _emailService.SendEmail("wakku9780@gmail.com", subject, body);

                TempData["SuccessMessage"] = "Your message has been sent!";
                return RedirectToAction("Index");
            }

            return View("Index", model);
        }
    }
}
