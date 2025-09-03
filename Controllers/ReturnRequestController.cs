using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Verveo.Entities;
using Verveo.Services;

namespace Verveo.Controllers
{
    [Authorize]
    public class ReturnRequestController : Controller
    {
        private readonly ReturnRequestService _returnService;
        public ReturnRequestController(ReturnRequestService returnService)
        {
            _returnService = returnService;
        }

        // Kullanıcı: Talep oluşturma formu
        [HttpGet]
        public IActionResult Create()
        {
            // ...model hazırlama...
            return View("~/Views/ReturnRequests/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int orderId, string type, string reason)
        {
            int userId = int.Parse(User.FindFirst("UserId")!.Value);
            _returnService.CreateRequest(userId, orderId, type, reason);
            TempData["ToastMessage"] = "Talebiniz başarıyla alındı.";
            TempData["ToastType"] = "success";
            return RedirectToAction("MyOrders", "Order");
        }

        // Kullanıcı: Taleplerim
        public IActionResult MyRequests()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "User");
            }
            int userId = int.Parse(userIdClaim.Value);

            var requests = _returnService.GetByUserId(userId);
            return View("~/Views/ReturnRequests/MyRequests.cshtml", requests); // View yolunu açıkça belirttik
        }

        // Admin: Tüm talepler
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var requests = _returnService.GetAllRequests();
            return View(requests);
        }

        // Admin: Talep durumu güncelle
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            ReturnStatus newStatus = (ReturnStatus)Enum.Parse(typeof(ReturnStatus), status);
            _returnService.UpdateStatus(id, newStatus);

            // Doğru view yolunu belirt!
            var requests = _returnService.GetAllRequests();
            return View("~/Views/ReturnRequests/Index.cshtml", requests);
        }
    }
}