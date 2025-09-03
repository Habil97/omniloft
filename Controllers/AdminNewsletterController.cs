using Microsoft.AspNetCore.Mvc;
using Verveo.DataAccess;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Verveo.Entities;
using System.Collections.Generic;

namespace Verveo.Controllers
{
    public class AdminNewsletterController : Controller
    {
        private readonly VerveoDbContext _context;
        public AdminNewsletterController(VerveoDbContext context)
        {
            _context = context;
        }

        public IActionResult Newsletter()
        {
            var subscribers = _context.NewsletterSubscribers.Where(x => x.IsActive).ToList();
            return View(subscribers);
        }

        [HttpPost]
        public IActionResult SendNewsletter(string message)
        {
            var subscribers = _context.NewsletterSubscribers.Where(x => x.IsActive).Select(x => x.Email).ToList();
            if (subscribers.Count == 0 || string.IsNullOrWhiteSpace(message))
                return RedirectToAction("Newsletter");

            // SMTP ayarlarını buraya girin
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("habilcakir97@gmail.com", "axpk uscv thva auec"),
                EnableSsl = true,
            };
            foreach (var email in subscribers)
            {
                var mail = new MailMessage("habilcakir97@gmail.com", email)
                {
                    Subject = "Verveo E-Bülten",
                    Body = message,
                    IsBodyHtml = false
                };
                smtpClient.Send(mail);
            }
                TempData["NewsletterSuccess"] = "Toplu mail başarıyla gönderildi.";
            return RedirectToAction("Newsletter");
        }
    }
}
