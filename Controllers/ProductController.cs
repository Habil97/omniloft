using Microsoft.AspNetCore.Mvc;
using Verveo.Services;
using Verveo.DataAccess;
using Verveo.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Verveo.ViewModels; // EKLE
using System.Security.Claims; // EKLE

namespace Verveo.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly VerveoDbContext _dbContext;
        private readonly ProductReviewService _productReviewService;
        private readonly FavoriteProductService _favoriteProductService; // FavoriteProductService ekle

        public ProductController(
            VerveoDbContext dbContext,
            ProductService productService,
            ProductReviewService productReviewService,
            FavoriteProductService favoriteProductService) // FavoriteProductService constructor'a ekle
        {
            _dbContext = dbContext;
            _productService = productService;
            _productReviewService = productReviewService;
            _favoriteProductService = favoriteProductService; // FavoriteProductService ata
        }

        public IActionResult Index(string q)
        {
            // Filtre parametrelerini HttpContext üzerinden al
            var selectedCategoryStr = HttpContext.Request.Query["category"].ToString();
            if (string.IsNullOrEmpty(selectedCategoryStr) || selectedCategoryStr == "0")
            {
                selectedCategoryStr = "";
            }
            var minPriceStr = HttpContext.Request.Query["minPrice"].ToString();
            var maxPriceStr = HttpContext.Request.Query["maxPrice"].ToString();
            var sortStr = HttpContext.Request.Query["sort"].ToString();
            // Sıralama parametresi boş veya "featured" ise varsayılan olarak boş string ata
            if (string.IsNullOrEmpty(sortStr) || sortStr == "featured")
            {
                sortStr = "";
            }
            ViewBag.SelectedCategory = selectedCategoryStr;
            ViewBag.MinPrice = minPriceStr;
            ViewBag.MaxPrice = maxPriceStr;
            ViewBag.Sort = sortStr;
            var products = _productService.GetAllProducts()
                .Where(p => p.IsActive && p.Stock > 0)
                .ToList();
            // Kategorileri ViewBag ile aktar
            ViewBag.Categories = _dbContext.Categories.ToList();

            // Arama
            if (!string.IsNullOrWhiteSpace(q))
            {
                var query = q.ToLower();
                products = products.Where(p =>
                    (!string.IsNullOrEmpty(p.Name) && p.Name.ToLower().Contains(query)) ||
                    (!string.IsNullOrEmpty(p.Description) && p.Description.ToLower().Contains(query))
                ).ToList();
                ViewData["q"] = q;
            }
            else
            {
                ViewData["q"] = "";
            }

            // Filtreleme
            if (int.TryParse(selectedCategoryStr, out int categoryId) && categoryId > 0)
            {
                products = products.Where(p => p.CategoryId == categoryId).ToList();
            }
            if (decimal.TryParse(minPriceStr, out decimal minPrice))
            {
                products = products.Where(p => p.Price >= minPrice).ToList();
            }
            if (decimal.TryParse(maxPriceStr, out decimal maxPrice))
            {
                products = products.Where(p => p.Price <= maxPrice).ToList();
            }

            // Sıralama
            switch (sortStr)
            {
                case "price_asc":
                    products = products.OrderBy(p => p.Price).ToList();
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price).ToList();
                    break;
                case "name_asc":
                    products = products.OrderBy(p => p.Name).ToList();
                    break;
                case "name_desc":
                    products = products.OrderByDescending(p => p.Name).ToList();
                    break;
                case "featured":
                    products = products.OrderByDescending(p => p.IsFeatured).ToList();
                    break;
            }

            return View(products);
        }
            public IActionResult Category(int id)
            {
                var products = _productService.GetAllProducts().Where(p => p.CategoryId == id).ToList();
                return View("Index", products);
            }

            public IActionResult Details(int id)
            {
                var product = _productService.GetById(id);

                var features = _productService.GetFeatures(id);

                var reviews = _dbContext.ProductReviews
                    .Include(r => r.User)
                    .Where(r => r.ProductId == id && r.IsApproved)
                    .ToList();

                var isFavorite = false;
                if (User.Identity.IsAuthenticated)
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim != null)
                    {
                        var userId = int.Parse(userIdClaim.Value);
                        isFavorite = _favoriteProductService.IsFavorite(userId, id);
                    }
                }

                var model = new ProductDetailViewModel
                {
                    Product = product,
                    Features = features,
                    Reviews = reviews,
                    IsFavorite = isFavorite
                };
                return View(model);
            }

            [HttpPost]
            [Authorize]
public IActionResult AddReview(int productId, int rating, string comment)
{
    // oturumdan kullanıcı id’sini al
    var userId = int.Parse(User.FindFirst("UserId").Value);
    var review = new ProductReview
    {
        ProductId = productId,
        UserId = userId,
        Rating = rating,
        Comment = comment,
        CreatedAt = DateTime.Now,
        IsApproved = false // Moderasyon için
    };
      _productReviewService.AddReview(review);
    TempData["ReviewMessage"] = "Yorumunuz moderasyon için gönderildi.";
    return RedirectToAction("Details", new { id = productId });
}

[Authorize]
[HttpGet]
public IActionResult EditReview(int id)
{
    var review = _productReviewService.GetById(id);
    var userId = int.Parse(User.FindFirst("UserId").Value);
    if (review == null || review.UserId != userId)
        return Forbid();
    return View(review);
}

[Authorize]
[HttpPost]
public IActionResult EditReview(ProductReview model)
{
    var review = _productReviewService.GetById(model.Id);
    var userId = int.Parse(User.FindFirst("UserId").Value);
    if (review == null || review.UserId != userId)
        return Forbid();

    review.Comment = model.Comment;
    review.Rating = model.Rating;
    review.IsApproved = false; // Düzenlendiğinde tekrar onaya düşsün
    _productReviewService.UpdateReview(review);

    TempData["ReviewMessage"] = "Yorumunuz güncellendi ve tekrar moderasyona gönderildi.";
    return RedirectToAction("Details", new { id = review.ProductId });
}

[Authorize]
[HttpPost]
public IActionResult DeleteReview(int id)
{
    var review = _productReviewService.GetById(id);
    var userId = int.Parse(User.FindFirst("UserId").Value);
    if (review == null || review.UserId != userId)
        return Forbid();

    _productReviewService.DeleteReview(id);
    TempData["ReviewMessage"] = "Yorumunuz silindi.";
    return RedirectToAction("Details", new { id = review.ProductId });
}
    }
}
