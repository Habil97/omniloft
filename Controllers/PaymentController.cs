using Microsoft.AspNetCore.Mvc;
using Verveo.Services;

public class PaymentController : Controller
{
    private readonly PaymentService _paymentService;
    public PaymentController(PaymentService paymentService) { _paymentService = paymentService; }

    [HttpPost]
    public IActionResult Pay(int orderId, decimal amount)
    {
        _paymentService.ProcessPayment(orderId, amount);
        return RedirectToAction("Details", "Order", new { id = orderId });
    }
}