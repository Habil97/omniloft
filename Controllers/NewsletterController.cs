using Microsoft.AspNetCore.Mvc;
using Verveo.Entities;
using Verveo.DataAccess;
using System;
using System.Linq;

namespace Verveo.Controllers
{
    public class NewsletterController : Controller
    {
        private readonly VerveoDbContext _context;
        public NewsletterController(VerveoDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Subscribe(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return RedirectToAction("Index", "Home");

            var exists = _context.NewsletterSubscribers.FirstOrDefault(x => x.Email == email);
            if (exists == null)
            {
                _context.NewsletterSubscribers.Add(new NewsletterSubscriber
                {
                    Email = email,
                    SubscribedAt = DateTime.Now,
                    IsActive = true
                });
                _context.SaveChanges();
                TempData["NewsletterSuccess"] = "Başarıyla abone oldunuz. Kampanya ve duyurular e-posta adresinize gönderilecektir.";
            }
            else
            {
                TempData["NewsletterSuccess"] = "Bu e-posta zaten abone!";
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
