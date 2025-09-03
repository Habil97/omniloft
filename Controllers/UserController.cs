using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Verveo.Services;
using Verveo.DataAccess;
using Verveo.Entities;
using Verveo.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Verveo.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserService _userService;
        private readonly VerveoDbContext _dbContext;
        private readonly ProductReviewService _productReviewService;
        private readonly FavoriteProductService _favoriteProductService;
        private readonly CartService _cartService; // CartService bağımlılığını ekleyin

        public UserController(VerveoDbContext dbContext, ProductReviewService productReviewService, UserService userService, FavoriteProductService favoriteProductService, CartService cartService)
        {
            _dbContext = dbContext;
            _userService = userService;
            _productReviewService = productReviewService;
            _favoriteProductService = favoriteProductService;
            _cartService = cartService; // CartService'i burada başlatın
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _userService.Authenticate(model.Email, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "E-posta veya şifre hatalı.");
                return View(model);
            }

            var roleRepo = new EfRoleRepository(_dbContext);
            var role = roleRepo.GetById(user.RoleId);
            if (role == null)
            {
                ModelState.AddModelError("", "Kullanıcı rolü bulunamadı.");
                return View(model);
            }

            // OTURUM AÇMA KODU BURADA
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, role.Name) // Rol bilgisini ekle!
            };

            var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");
            await HttpContext.SignInAsync("MyCookieAuth", new ClaimsPrincipal(claimsIdentity));

            // Kullanıcı login olduktan sonra session sepetini kullanıcıya aktar
            _cartService.TransferSessionCartToUser(HttpContext.Session, user.Id);

            // Session sepetini temizle
            _cartService.ClearCartFromSession(HttpContext.Session);

            // Sepete yönlendir
            return RedirectToAction("Index", "Cart");

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Profile()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login");

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                // Kullanıcı oturum açmamış, login’e yönlendir
                return RedirectToAction("Login", "User");
            }
            var userId = int.Parse(userIdClaim.Value);

            var user = _dbContext.Users
                .Include(u => u.Reviews)
                .ThenInclude(r => r.Product)
                .Include(u => u.FavoriteProducts)
                .ThenInclude(f => f.Product)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return RedirectToAction("Login");

            user.FavoriteProducts = _favoriteProductService.GetFavorites(userId);

            return View(user);
        }

        [HttpPost]
        public IActionResult UpdatePassword(int id, string NewPassword)
        {
            var user = _userService.GetById(id);
            if (user == null)
                return NotFound();
            if (string.IsNullOrEmpty(NewPassword))
            {
                ModelState.AddModelError("", "Parola boş olamaz.");
                return View("Profile", user);
            }
            user.Password = BCrypt.Net.BCrypt.HashPassword(NewPassword);
            _userService.UpdateUser(user);
            ViewBag.Message = "Parola başarıyla güncellendi.";
            return View("Profile", user);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var user = _userService.GetAllUsers().FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();
            _userService.DeleteUser(id);
            HttpContext.Session.Clear();
            return RedirectToAction("Register");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (!ModelState.IsValid)
                return View(user);

            // Varsayılan rol ataması (örneğin User rolü id'si 2 ise)
            user.RoleId = 3;

            _userService.AddUser(user);
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(string email, string newPassword)
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var user = _userService.GetById(userId);
            if (user == null || user.Email != email)
            {
                ViewBag.Message = "E-posta adresi eşleşmiyor!";
                return View("Profile", user);
            }

            _userService.UpdatePassword(userId, newPassword);
            ViewBag.Message = "Parolanız başarıyla güncellendi.";
            return View("Profile", user);
        }

        public IActionResult MyReviews()
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var reviews = _productReviewService.GetByUserId(userId);
            return View(reviews);
        }

        [HttpGet]
        public IActionResult EditReview(int id)
        {
            var review = _productReviewService.GetById(id);
            var userId = int.Parse(User.FindFirst("UserId").Value);
            if (review == null || review.UserId != userId)
                return Forbid();
            return View(review);
        }

        [HttpPost]
        public IActionResult EditReview(ProductReview model)
        {
            var review = _productReviewService.GetById(model.Id);
            var userId = int.Parse(User.FindFirst("UserId").Value);
            if (review == null || review.UserId != userId)
                return Forbid();

            review.Comment = model.Comment;
            review.Rating = model.Rating;
            review.IsApproved = false;
            _productReviewService.UpdateReview(review);

            TempData["ReviewMessage"] = "Yorumunuz güncellendi ve tekrar moderasyona gönderildi.";
            return RedirectToAction("Profile");
        }

        [HttpPost]
        public IActionResult DeleteReview(int id)
        {
            var review = _productReviewService.GetById(id);
            var userId = int.Parse(User.FindFirst("UserId").Value);
            if (review == null || review.UserId != userId)
                return Forbid();

            _productReviewService.DeleteReview(id);
            TempData["ReviewMessage"] = "Yorumunuz silindi.";
            return RedirectToAction("MyReviews");
        }

        [Authorize]
        public IActionResult Favorites()
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var favorites = _favoriteProductService.GetFavorites(userId);
            return View(favorites);
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddFavorite(int productId)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login", "User");

            var userId = int.Parse(userIdClaim.Value);
            _favoriteProductService.AddFavorite(userId, productId);

            return RedirectToAction("Details", "Product", new { id = productId });
        }

        [Authorize]
        [HttpPost]
        public IActionResult RemoveFavorite(int productId)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login", "User");

            var userId = int.Parse(userIdClaim.Value);
            _favoriteProductService.RemoveFavorite(userId, productId);

            return RedirectToAction("Profile");
        }
    }
}
