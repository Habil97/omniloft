using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Mvc;
using Verveo.Services;
using Verveo.DataAccess;
using System.Threading.Tasks;
using Verveo.Entities;
using System.Security.Claims;

namespace Verveo.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;
        private readonly IProductRepository _productService;
        private readonly ICouponService _couponService;

        public CartController(CartService cartService, IProductRepository productService, ICouponService couponService)
        {
            _cartService = cartService;
            _productService = productService;
            _couponService = couponService;
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(Verveo.Models.CheckoutViewModel model)
        {
            // Sunucu tarafı ek doğrulama
            if (string.IsNullOrWhiteSpace(model.FullName))
                ModelState.AddModelError("FullName", "Ad Soyad zorunlu.");
            if (string.IsNullOrWhiteSpace(model.Email) || !model.Email.Contains("@"))
                ModelState.AddModelError("Email", "Geçerli bir e-posta giriniz.");
            if (string.IsNullOrWhiteSpace(model.Phone))
                ModelState.AddModelError("Phone", "Telefon zorunlu.");
            if (string.IsNullOrWhiteSpace(model.Address))
                ModelState.AddModelError("Address", "Adres zorunlu.");
            if (string.IsNullOrWhiteSpace(model.City))
                ModelState.AddModelError("City", "İl zorunlu.");
            if (string.IsNullOrWhiteSpace(model.District))
                ModelState.AddModelError("District", "İlçe zorunlu.");
            if (string.IsNullOrWhiteSpace(model.PostalCode))
                ModelState.AddModelError("PostalCode", "Posta kodu zorunlu.");
            if (string.IsNullOrWhiteSpace(model.CardName))
                ModelState.AddModelError("CardName", "Kart üzerindeki isim zorunlu.");
            if (string.IsNullOrWhiteSpace(model.CardNumber) || model.CardNumber.Length != 16)
                ModelState.AddModelError("CardNumber", "16 haneli kart numarası giriniz.");
            if (string.IsNullOrWhiteSpace(model.CardExpiry) || model.CardExpiry.Length != 5 || !model.CardExpiry.Contains("/"))
                ModelState.AddModelError("CardExpiry", "Son kullanma tarihi MM/YY formatında olmalı.");
            if (string.IsNullOrWhiteSpace(model.CardCvv) || model.CardCvv.Length < 3 || model.CardCvv.Length > 4)
                ModelState.AddModelError("CardCvv", "Geçerli bir CVV giriniz.");

            if (!ModelState.IsValid)
            {
                ViewBag.PaymentMessage = "Lütfen tüm alanları doğru doldurun.";
                return View(model);
            }

            var options = new Options();
            options.ApiKey = "sandbox-3T9nK0NgFNZwEjFLDyZ9yKisYomdTTIg";
            options.SecretKey = "bOAAV7npnUZFkmIxJGjQtpW26GCAyPPv";
            options.BaseUrl = "https://sandbox-api.iyzipay.com";

            var request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = Guid.NewGuid().ToString();

            int userId = User.Identity.IsAuthenticated
                ? int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)
                : 0;

            var cartItems = userId > 0
                ? _cartService.GetCartItems(userId)
                : _cartService.GetCartFromSession(HttpContext.Session)?.Items ?? new List<CartItem>();

            decimal subtotal = cartItems.Sum(item => item.Product.Price * item.Quantity);
            decimal discount = 0;
            var discountStr = HttpContext.Session.GetString("CouponDiscount");
            if (!string.IsNullOrEmpty(discountStr))
                decimal.TryParse(discountStr, out discount);
            decimal paidPrice = subtotal - discount;

            request.Price = subtotal.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
            request.PaidPrice = paidPrice.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

            var paymentCard = new PaymentCard();
            paymentCard.CardHolderName = model.CardName;
            paymentCard.CardNumber = model.CardNumber;
            paymentCard.ExpireMonth = model.CardExpiry.Substring(0, 2);
            paymentCard.ExpireYear = "20" + model.CardExpiry.Substring(3, 2);
            paymentCard.Cvc = model.CardCvv;
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;

            var buyer = new Buyer();
            buyer.Id = Guid.NewGuid().ToString();
            buyer.Name = model.FullName;
            var fullNameParts = model.FullName.Trim().Split(' ');
            buyer.Name = fullNameParts.First();
            buyer.Surname = fullNameParts.Length > 1 ? fullNameParts.Last() : "Soyad"; // Eğer tek kelime ise varsayılan bir değer ata
            buyer.GsmNumber = model.Phone;
            buyer.Email = model.Email;
            buyer.IdentityNumber = "11111111111"; // Test için
            buyer.RegistrationAddress = model.Address;
            buyer.City = model.City;
            buyer.Country = "Turkey";
            buyer.ZipCode = model.PostalCode;
            request.Buyer = buyer;

            var shippingAddress = new Address();
            shippingAddress.ContactName = model.FullName;
            shippingAddress.City = model.City;
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = model.Address;
            shippingAddress.ZipCode = model.PostalCode;
            request.ShippingAddress = shippingAddress;
            request.BillingAddress = shippingAddress;

            var iyziBasketItems = new System.Collections.Generic.List<BasketItem>();
            foreach (var item in cartItems)
            {
                var iyziItem = new BasketItem();
                iyziItem.Id = item.Product.Id.ToString();
                iyziItem.Name = item.Product.Name;
                iyziItem.Category1 = "Ürün";
                iyziItem.ItemType = BasketItemType.PHYSICAL.ToString();
                iyziItem.Price = (item.Product.Price * item.Quantity).ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
                iyziBasketItems.Add(iyziItem);
            }
            request.BasketItems = iyziBasketItems;

            var payment = await Iyzipay.Model.Payment.Create(request, options);

            if (payment.Status == "success")
            {
                if (userId > 0)
                {
                    _cartService.CreateOrderFromCart(userId);
                }
                else
                {
                    // Anonim kullanıcı ise, sipariş oluşturulamaz
                    ViewBag.PaymentMessage = "Sipariş oluşturmak için giriş yapmalısınız.";
                    return View(model);
                }
                ViewBag.PaymentMessage = "Ödeme başarıyla tamamlandı! Siparişiniz alınmıştır.";
                return RedirectToAction("OrderSuccess");
            }
            else
            {
                ViewBag.PaymentMessage = $"Ödeme başarısız: {payment.ErrorMessage}";
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            // Burada ödeme sayfası veya ödeme adımı için bir view dönebiliriz
            return View(); // Views/Cart/Checkout.cshtml
        }

        [HttpPost]
        public IActionResult ApplyCoupon(string couponCode)
        {
            int userId = User.Identity.IsAuthenticated
                ? int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)
                : 0;

            Cart cart = userId > 0
                ? _cartService.GetCart(userId)
                : _cartService.GetCartFromSession(HttpContext.Session);

            var orderTotal = cart.Items.Sum(i => i.Product.Price * i.Quantity);

            var coupon = _couponService.GetValidCoupon(couponCode, orderTotal);
            if (coupon != null)
            {
                decimal discount = orderTotal * coupon.DiscountRate / 100;
                ViewBag.CouponDiscount = discount;
                HttpContext.Session.SetString("AppliedCoupon", couponCode);
                HttpContext.Session.SetString("CouponDiscount", discount.ToString());
                ViewBag.AppliedCoupon = couponCode;
                ViewBag.CouponMessage = "İndirim uygulandı!";
            }
            else
            {
                HttpContext.Session.Remove("AppliedCoupon");
                HttpContext.Session.Remove("CouponDiscount");
                ViewBag.AppliedCoupon = null;
                ViewBag.CouponMessage = "Kupon kodu geçersiz veya kullanım süresi dolmuş.";
            }

            return View("Index", cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            int userId = User.Identity.IsAuthenticated
                ? int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value)
                : 0;

            if (userId > 0)
            {
                _cartService.AddToCart(userId, productId, quantity);
            }
            else
            {
                var cart = _cartService.GetCartFromSession(HttpContext.Session);
                if (cart == null)
                    cart = new Cart { Items = new List<CartItem>() };

                _cartService.AddToCart(cart, productId, quantity);
                _cartService.SaveCartToSession(HttpContext.Session, cart);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            int userId = User.Identity.IsAuthenticated
                ? int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value)
                : 0;

            Cart cart = userId > 0
                ? _cartService.GetCart(userId)
                : _cartService.GetCartFromSession(HttpContext.Session);

            // Session sepetinden gelen ürünlerin Product nesnesini doldur
            if (cart != null && cart.Items != null)
            {
                foreach (var item in cart.Items)
                {
                    if (item.Product == null)
                        item.Product = _productService.GetById(item.ProductId);
                }
            }

            return View(cart);
        }

        [HttpPost]
        public IActionResult Remove(int productId)
        {
            int userId = User.Identity.IsAuthenticated
                ? int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)
                : 0;

            if (userId > 0)
            {
                _cartService.RemoveFromCart(userId, productId);
            }
            else
            {
                var cart = _cartService.GetCartFromSession(HttpContext.Session);
                if (cart != null)
                {
                    var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                    if (item != null)
                    {
                        cart.Items.Remove(item);
                        _cartService.SaveCartToSession(HttpContext.Session, cart);
                    }
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Clear()
        {
            int userId = User.Identity.IsAuthenticated
                ? int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)
                : 0;

            if (userId > 0)
            {
                _cartService.ClearCart(userId);
            }
            else
            {
                _cartService.ClearCartFromSession(HttpContext.Session);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            // Örnek userId alma yöntemi:
            int userId = int.Parse(User.FindFirst("UserId").Value);

            _cartService.UpdateQuantity(userId, productId, quantity);
            return RedirectToAction("Index");
        }

        public IActionResult OrderSuccess()
        {
            return View();
        }
    }
}
