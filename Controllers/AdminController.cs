using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Verveo.Services;
using Verveo.DataAccess;
using Verveo.Entities;
using Verveo.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using System.Threading.Tasks;
using Verveo.ViewModels;

namespace Verveo.Controllers
{
    // [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ProductService _productService;
        private readonly VerveoDbContext _dbContext;
        private readonly CategoryService _categoryService;
        private readonly UserService _userService;
        private readonly OrderService _orderService;
        private readonly RoleService _roleService;
        private readonly SliderService _sliderService;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ReturnRequestService _returnRequestService;
        private readonly ProductReviewService _productReviewService;

        public AdminController(
            ProductService productService,
            VerveoDbContext dbContext,
            CategoryService categoryService,
            UserService userService,
            OrderService orderService,
            RoleService roleService,
            SliderService sliderService,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            ReturnRequestService returnRequestService,
            ProductReviewService productReviewService
        )
        {
            _productService = productService;
            _dbContext = dbContext;
            _categoryService = categoryService;
            _userService = userService;
            _orderService = orderService;
            _roleService = roleService;
            _sliderService = sliderService;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _returnRequestService = returnRequestService;
            _productReviewService = productReviewService;
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            ViewBag.Roles = _roleService.GetAllRoles().ToList();
            return View(new User());
        }

        [HttpPost]
        public IActionResult AddUser(User user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password) || user.RoleId == 0)
            {
                ModelState.AddModelError("", "Tüm alanlar zorunludur.");
                ViewBag.Roles = _roleService.GetAllRoles().ToList();
                return View(user);
            }
            _userService.AddUser(user);
            return RedirectToAction("Users");
        }

        [HttpGet]
        public IActionResult EditUser(int id)
        {
            var user = _userService.GetById(id);
            if (user == null)
                return NotFound();
            ViewBag.Roles = _roleService.GetAllRoles().ToList();
            return View(user);
        }

        [HttpPost]
        public IActionResult EditUser(User user)
        {
            var existingUser = _userService.GetById(user.Id);
            if (existingUser == null)
                return NotFound();

            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.RoleId = user.RoleId;

            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                if (!user.Password.StartsWith("$2a$") && !user.Password.StartsWith("$2b$") && !user.Password.StartsWith("$2y$"))
                {
                    existingUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                }
                else
                {
                    existingUser.Password = user.Password;
                }
            }

            _userService.UpdateUser(existingUser);

            return RedirectToAction("Users");
        }

        [HttpGet]
        public IActionResult Sliders()
        {
            var sliders = _sliderService.GetAllSliders().ToList();
            return View(sliders);
        }

        [HttpGet]
        public IActionResult AddSlider()
        {
            return View(new Slider());
        }

        [HttpPost]
        public IActionResult AddSlider(Slider slider, IFormFile? ImageFile)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }
                slider.ImagePath = "/images/" + uniqueFileName;
            }
            _sliderService.AddSlider(slider);
            return RedirectToAction("Sliders");
        }

        [HttpGet]
        public IActionResult EditSlider(int id)
        {
            var slider = _sliderService.GetSliderById(id);
            if (slider == null)
                return NotFound();
            return View(slider);
        }

        [HttpPost]
        public IActionResult EditSlider(Slider model, IFormFile? ImageFile)
        {
            var slider = _sliderService.GetSliderById(model.Id);
            if (slider == null)
                return NotFound();

            slider.Title = model.Title;
            slider.Description = model.Description;
            slider.ButtonText = model.ButtonText;
            slider.ButtonUrl = model.ButtonUrl;
            slider.IsActive = model.IsActive; // DİKKAT: Bu satır olmalı!
            slider.Order = model.Order;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }
                slider.ImagePath = "/images/" + uniqueFileName;
            }

            _sliderService.UpdateSlider(slider);
            return RedirectToAction("Sliders");
        }

        [HttpGet]
        public IActionResult DeleteSlider(int id)
        {
            _sliderService.DeleteSlider(id);
            return RedirectToAction("Sliders");
        }

        [HttpGet]
        public IActionResult AddRole()
        {
            return View(new Role());
        }

        [HttpPost]
        public IActionResult AddRole(Role role)
        {
            _roleService.AddRole(role);
            return RedirectToAction("Roles");
        }

        [HttpGet]
        public IActionResult EditRole(int id)
        {
            var role = _roleService.GetRoleById(id);
            if (role == null)
                return NotFound();
            return View(role);
        }

        [HttpPost]
        public IActionResult EditRole(Role role)
        {
            _roleService.UpdateRole(role);
            return RedirectToAction("Roles");
        }

        [HttpGet]
        public IActionResult Products()
        {
            var products = _productService.GetAllProducts().ToList();
            return View(products);
        }

        [HttpGet]
        public IActionResult Orders()
        {
            var orders = _orderService.GetAllOrders().ToList();
            return View(orders);
        }

        [HttpGet]
        public IActionResult Categories()
        {
            var categories = _categoryService.GetAllCategories().ToList();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Users()
        {
            var users = _userService.GetAllUsers().ToList();
            return View(users);
        }

        [HttpGet]
        public IActionResult Roles()
        {
            var roles = _roleService.GetAllRoles().ToList();
            return View(roles);
        }

        [HttpGet]
        public IActionResult Index()
        {
            var products = _productService.GetAllProducts().ToList();
            var orders = _orderService.GetAllOrders().ToList();
            var categories = _categoryService.GetAllCategories().ToList();

            var orderItems = _dbContext.OrderItems.ToList();

            var today = DateTime.Today;
            var last7Days = Enumerable.Range(0, 7)
                .Select(i => today.AddDays(-6 + i))
                .ToList();

            var ordersPerDayLabels = last7Days.Select(d => d.ToString("dd.MM")).ToList();
            var ordersPerDay = last7Days
                .Select(date => orders.Count(o => o.CreatedAt.Date == date))
                .ToList();
            var revenuePerDay = last7Days
                .Select(date => orders.Where(o => o.CreatedAt.Date == date).Sum(o => o.Total))
                .ToList();

            var topProducts = orderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    Count = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            var topProductsLabels = topProducts
                .Select(tp => products.FirstOrDefault(p => p.Id == tp.ProductId)?.Name ?? "Bilinmiyor")
                .ToList();
            var topProductsData = topProducts.Select(tp => tp.Count).ToList();

            var topCategories = orderItems
                .GroupBy(oi => products.FirstOrDefault(p => p.Id == oi.ProductId)?.CategoryId)
                .Where(g => g.Key != null)
                .Select(g => new
                {
                    CategoryId = g.Key.Value,
                    Count = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            var topCategoriesLabels = topCategories
                .Select(tc => categories.FirstOrDefault(c => c.Id == tc.CategoryId)?.Name ?? "Bilinmiyor")
                .ToList();
            var topCategoriesData = topCategories.Select(tc => tc.Count).ToList();

            ViewBag.OrdersPerDayLabels = ordersPerDayLabels;
            ViewBag.OrdersPerDay = ordersPerDay;
            ViewBag.RevenuePerDay = revenuePerDay;
            ViewBag.TopProductsLabels = topProductsLabels;
            ViewBag.TopProductsData = topProductsData;
            ViewBag.TopCategoriesLabels = topCategoriesLabels;
            ViewBag.TopCategoriesData = topCategoriesData;

            var model = new AdminPanelViewModel
            {
                Products = products,
                Orders = orders,
                Categories = categories,
                Users = _userService.GetAllUsers().ToList(),
                Roles = _roleService.GetAllRoles().ToList(),
                Sliders = _sliderService.GetAllSliders().ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(int orderId, Verveo.Entities.OrderStatus status)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null)
                return NotFound();
            order.Status = status;
            _orderService.UpdateOrder(order);
            TempData["OrderStatusSuccess"] = "Sipariş durumu başarıyla güncellendi.";
            return RedirectToAction("Orders");
        }

        [HttpGet]
        public IActionResult DeleteProduct(int id)
        {
            var product = _productService.GetAllProducts().FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();
            _productService.DeleteProduct(id);
            return RedirectToAction("Products");
        }

        [HttpGet]
        public IActionResult AddCategory()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddCategory(Category category)
        {
            if (string.IsNullOrEmpty(category.Name))
            {
                ModelState.AddModelError("", "Kategori adı zorunlu.");
                return View(category);
            }
            _categoryService.AddCategory(category);
            return RedirectToAction("Categories");
        }

        [HttpGet]
        public IActionResult EditCategory(int id)
        {
            var category = _categoryService.GetCategoryById(id);
            if (category == null)
                return NotFound();
            return View(category);
        }
        [HttpPost]
        public IActionResult EditCategory(Category category)
        {
            if (string.IsNullOrEmpty(category.Name))
            {
                ModelState.AddModelError("", "Kategori adı zorunlu.");
                return View(category);
            }
            _categoryService.UpdateCategory(category);
            return RedirectToAction("Categories");
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            var categories = _categoryService.GetAllCategories().ToList();
            var model = new ProductFeatureViewModel
            {
                Product = new Product(),
                Features = new List<ProductFeature>()
            };
            ViewBag.Categories = categories;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductFeatureViewModel model, IFormFile? ImageFile)
        {
            var product = model.Product;

            if (product.Stock == 0)
            {
                product.IsActive = false;
                product.IsFeatured = false;
            }

            _productService.AddProduct(product);

            // Ana görseli kaydet (ImageFile)
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }
                product.ImagePath = "/images/" + uniqueFileName;
                _productService.UpdateProduct(product);
            }

            // Galeri görsellerini kaydet
            if (model.GalleryFiles != null && model.GalleryFiles.Count > 0)
            {
                foreach (var file in model.GalleryFiles)
                {
                    if (file != null && file.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        var productImage = new ProductImage
                        {
                            ProductId = product.Id,
                            ImagePath = "/images/" + uniqueFileName
                        };
                        _dbContext.ProductImages.Add(productImage);
                    }
                }
                await _dbContext.SaveChangesAsync();
            }

            // Özellikleri kaydet
            var validFeatures = model.Features
                .Where(f => !string.IsNullOrWhiteSpace(f.Name) && !string.IsNullOrWhiteSpace(f.Value))
                .ToList();
            _productService.UpdateProductFeatures(product.Id, validFeatures);

            return RedirectToAction("Products");
        }

        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            var product = _productService.GetProductWithFeatures(id);
            var categories = _categoryService.GetAllCategories().ToList();
            var model = new ProductFeatureViewModel
            {
                Product = product,
                Features = product.Features?.ToList() ?? new List<ProductFeature>()
            };
            ViewBag.Categories = categories;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductFeatureViewModel model, IFormFile? ImageFile, List<IFormFile> GalleryFiles)
        {
            var product = _productService.GetById(model.Product.Id);
            if (product == null)
                return NotFound();

            // Diğer alanları güncelle
            product.Name = model.Product.Name;
            product.Description = model.Product.Description;
            product.Price = model.Product.Price;
            product.CategoryId = model.Product.CategoryId;
            product.Stock = model.Product.Stock;
            product.IsActive = model.Product.IsActive;
            product.IsFeatured = model.Product.IsFeatured;

            // Ana görsel güncelleme
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }
                product.ImagePath = "/images/" + uniqueFileName;
            }
            // Eğer yeni resim seçilmediyse eski resim korunur

            // Galeri görselleri ekleme
            if (GalleryFiles != null && GalleryFiles.Count > 0)
            {
                foreach (var file in GalleryFiles)
                {
                    if (file != null && file.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        var productImage = new ProductImage
                        {
                            ProductId = product.Id,
                            ImagePath = "/images/" + uniqueFileName
                        };
                        _dbContext.ProductImages.Add(productImage);
                    }
                }
                await _dbContext.SaveChangesAsync();
            }

            _productService.UpdateProduct(product);

            // Özellikleri güncelle
            var validFeatures = model.Features
                .Where(f => !string.IsNullOrWhiteSpace(f.Name) && !string.IsNullOrWhiteSpace(f.Value))
                .ToList();
            _productService.UpdateProductFeatures(product.Id, validFeatures);

            return RedirectToAction("Products");
        }

        public IActionResult Dashboard()
        {
            var today = DateTime.Today;
            var orders = _orderService.GetAll()
                .Where(o => o.CreatedAt >= today.AddDays(-6))
                .ToList();

            var last7Days = Enumerable.Range(0, 7)
                .Select(i => today.AddDays(-6 + i))
                .ToList();

            var orderCounts = last7Days
                .Select(date => new
                {
                    Date = date.ToString("dd.MM"),
                    Count = orders.Count(o => o.CreatedAt.Date == date)
                })
                .ToList();

            ViewBag.OrderCounts = orderCounts;
            return View();
        }

        [HttpGet]
        public IActionResult AdminPanel()
        {
            var model = new AdminPanelViewModel
            {
                Products = _productService.GetAllProducts().ToList(),
                Orders = _orderService.GetAllOrders().ToList(),
                Categories = _categoryService.GetAllCategories().ToList(),
                Users = _userService.GetAllUsers().ToList(),
                Roles = _roleService.GetAllRoles().ToList(),
                Sliders = _sliderService.GetAllSliders().ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUser(int id)
        {
            var user = _userService.GetById(id);
            if (user == null)
                return NotFound();

            _userService.DeleteUser(id);

            return RedirectToAction("Users");
        }

        [HttpGet]
        public IActionResult ReturnRequests()
        {
            var requests = _dbContext.ReturnRequests
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            return View("~/Views/ReturnRequests/Index.cshtml", requests);
        }

        [HttpGet]
        public IActionResult MyRequests(int userId)
        {
            var requests = _returnRequestService.GetRequestsByUser(userId);
            return View("~/Views/ReturnRequests/MyRequests.cshtml", requests);
        }

        [HttpGet]
        public IActionResult Logs()
        {
            var logsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            List<string> logLines = new List<string>();

            if (Directory.Exists(logsDirectory))
            {
                // Sadece en son oluşturulan log dosyasını al
                var lastLogFile = Directory.GetFiles(logsDirectory, "*.txt")
                                       .OrderByDescending(f => f)
                                       .FirstOrDefault();

                if (lastLogFile != null)
                {
                    try
                    {
                        logLines = System.IO.File.ReadAllLines(lastLogFile)
                                             .Reverse()
                                             .Take(200)
                                             .ToList(); // Son 200 satırı al
                    }
                    catch (IOException)
                    {
                        logLines.Add("Log dosyası başka bir işlem tarafından kullanılıyor. Lütfen sayfayı tekrar yükleyin.");
                    }
                }
                else
                {
                    logLines.Add("Log dosyası bulunamadı.");
                }
            }
            else
            {
                logLines.Add("Logs klasörü bulunamadı.");
            }

            return View(logLines);
        }

        public IActionResult Reviews()
        {
            var pendingReviews = _dbContext.ProductReviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .Where(r => !r.IsApproved)
                .ToList();
            return View(pendingReviews);
        }

        [HttpPost]
        public IActionResult ApproveReview(int id)
        {
            _productReviewService.ApproveReview(id);
            return RedirectToAction("Reviews");
        }

        [HttpGet]
        public IActionResult EditReview(int id)
        {
            var review = _productReviewService.GetById(id);
            if (review == null)
                return NotFound();
            return View(review);
        }

        [HttpPost]
        public IActionResult EditReview(ProductReview model)
        {
            var review = _productReviewService.GetById(model.Id);
            if (review == null)
                return NotFound();

            review.Comment = model.Comment;
            review.Rating = model.Rating;
            review.IsApproved = false; // Admin veya kullanıcı güncellerse tekrar onaya düşer
            _productReviewService.UpdateReview(review);

            TempData["ReviewMessage"] = "Yorum güncellendi ve tekrar moderasyona gönderildi.";
            return RedirectToAction("Reviews");
        }

        [HttpPost]
        public IActionResult DeleteReview(int id)
        {
            _productReviewService.DeleteReview(id);
            TempData["ReviewMessage"] = "Yorum silindi.";
            return RedirectToAction("Reviews");
        }
    }
}