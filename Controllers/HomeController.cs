using Shop111.Models;
using Shop111.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Shop111.Repositories;

namespace Shop111.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homeRepository;
        private readonly IReportRepository _reportRepository;

        public HomeController(ILogger<HomeController> logger, IHomeRepository homeRepository, IReportRepository reportRepository)
        {
            _homeRepository = homeRepository;
            _logger = logger;
            _reportRepository = reportRepository;

        }

        public async Task<IActionResult> Index(string sterm = "", int genreId = 0, double minPrice = 0,double maxPrice = 100000)
        {
            if (minPrice < 0) minPrice = 0;
            if (maxPrice < 0) maxPrice = 100000;
            IEnumerable<Product> products = await _homeRepository.GetProducts(sterm, genreId);

            // 🔥 Price Filter
            //products = products.Where(p => p.Price <= maxPrice);
            products = products.Where(p => p.Price >= minPrice && p.Price <= maxPrice).ToList();

            IEnumerable<Genre> genres = await _homeRepository.Genres();
            var topProducts = await _reportRepository.GetTopNSellingProductsByDate(DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);

            ProductDisplayModel productModel = new ProductDisplayModel
            {
                Products = products,
                Genres = genres,
                STerm = sterm,
                GenreId = genreId,
                TopNSoldProducts = topProducts
            };
            ViewBag.MinPrice = minPrice; // Minimum price for the slider
            ViewBag.MaxPrice = maxPrice;

            return View(productModel);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}

