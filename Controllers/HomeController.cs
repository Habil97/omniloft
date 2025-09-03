using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Verveo.Models;
using Verveo.Services;

namespace Verveo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly SliderService _sliderService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ProductService productService, CategoryService categoryService, SliderService sliderService, ILogger<HomeController> logger)
        {
            _productService = productService;
            _categoryService = categoryService;
            _sliderService = sliderService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var model = new HomeViewModel
            {
                FeaturedProducts = _productService.GetFeaturedProducts(),
                Categories = _categoryService.GetAllCategories().ToList(),
                Sliders = _sliderService.GetAllSliders()
                    .Where(s => s.IsActive && !string.IsNullOrEmpty(s.ImagePath))
                    .OrderBy(s => s.Order)
                    .ToList()
            };
            return View(model);
        }

        // HomeViewModel artık Models klasöründe

        public IActionResult About() => View();
        public IActionResult Trademark() => View();
        public IActionResult Vision() => View();
        public IActionResult HumanResources() => View();
        public IActionResult Privacy() => View();
        public IActionResult Faq() => View();
        public IActionResult Warranty() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
