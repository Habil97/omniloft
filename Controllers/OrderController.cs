using Microsoft.AspNetCore.Mvc;
using Verveo.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class OrderController : Controller
{
    private readonly IOrderRepository _orderRepo;
    public OrderController(IOrderRepository orderRepo) { _orderRepo = orderRepo; }

    // Müşterinin kendi siparişlerini listele
    public IActionResult MyOrders()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            // Kullanıcı giriş yapmamışsa login sayfasına yönlendir
            return RedirectToAction("Login", "User");
        }
        int userId = int.Parse(userIdClaim.Value);

        var orders = _orderRepo.GetByUserId(userId);
        return View(orders);
    }

    // Sipariş detayları
    public IActionResult Details(int id)
    {
        var order = _orderRepo.GetById(id);
        return View(order);
    }
}