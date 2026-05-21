using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop111.Models;
using Shop111.Data;

namespace Shop111.Controllers
{
    [Authorize(Roles = "Manager")]
    public class VendorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VendorController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // ড্যাশবোর্ড: ভেন্ডরের তথ্য ও পরিসংখ্যান
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var vendor = await _context.Vendors
                .Include(v => v.User)
                .Include(v => v.Location)
                .Include(v => v.Products)
                .ThenInclude(p => p.Genre)
                .FirstOrDefaultAsync(v => v.UserId == userId);

            if (vendor == null) return NotFound("Vendor profile not found.");
            return View(vendor);
        }

        // প্রোডাক্ট লিস্ট
        public async Task<IActionResult> MyProducts()
        {
            var userId = _userManager.GetUserId(User);
            var products = await _context.Products
                .Include(p => p.Genre)
                .Where(p => p.Vendor.UserId == userId)
                .ToListAsync();
            return View(products);
        }

        // ১. নতুন প্রোডাক্ট যোগ করার পেজ দেখানো
        [HttpGet]
        public async Task<IActionResult> AddProduct()
        {
            var genres = await _context.Genres.ToListAsync();

            var model = new ProductDTO
            {
                GenreList = genres.Select(g => new SelectListItem
                {
                    Text = g.GenreName,
                    Value = g.Id.ToString()
                }).ToList(),
                ProductInformations = new List<ProductInformationDTO>(), // মাস্ট ডিটেইলস
                Stock = new StockDTO { Quantity = 0 },
                ReleaseDate = DateTime.Now
            };

            return View(model);
        }

        // ২. ডাটা সেভ করা
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(ProductDTO model) // এখানে ProductDTO ই ব্যবহার করুন
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var vendor = await _context.Vendors.FirstOrDefaultAsync(v => v.UserId == userId);

                if (vendor != null)
                {
                    // ইমেজ সেভ করা
                    if (model.ImageFile != null)
                    {
                        model.Image = SaveImage(model.ImageFile);
                    }

                    // DTO থেকে মূল Product মডেলে ডাটা ট্রান্সফার (Mapping)
                    var product = new Product
                    {
                        ProductName = model.ProductName,
                        CompanyName = model.CompanyName,
                        Price = model.Price,
                        GenreId = model.GenreId,
                        ReleaseDate = model.ReleaseDate,
                        Image = model.Image,
                        VendorId = vendor.Id,
                        OnSale = model.OnSale,
                        // স্টকের তথ্য যোগ করা
                        Stock = new Stock { Quantity = model.Stock.Quantity },
                        // ডিটেইলস ইনফরমেশন যোগ করা
                        ProductInformations = model.ProductInformations.Select(info => new ProductInformation
                        {
                            Description = info.Description,
                            MadeIn = info.MadeIn
                        }).ToList()
                    };

                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();

                    TempData["successMessage"] = "Product added successfully!";
                    return RedirectToAction(nameof(MyProducts));
                }
            }

            // যদি ModelState ইনভ্যালিড হয়, তবে ড্রপডাউন লিস্ট আবার লোড করে পেজটি ফেরত পাঠান
            var genres = await _context.Genres.ToListAsync();
            model.GenreList = genres.Select(g => new SelectListItem
            {
                Text = g.GenreName,
                Value = g.Id.ToString()
            }).ToList();

            return View(model); // এখন আর Error আসবে না, কারণ টাইপ ঠিক আছে (ProductDTO)
        }

        // প্রোডাক্ট এডিট (এখানেই আপনি সমস্যা হতে পারে বলেছিলেন, তাই এটি চেক করা হয়েছে)
        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var userId = _userManager.GetUserId(User);
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.Vendor.UserId == userId);

            if (product == null) return NotFound();

            ViewBag.Genres = new SelectList(await _context.Genres.ToListAsync(), "Id", "GenreName", product.GenreId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(Product model, IFormFile? imageFile)
        {
            var userId = _userManager.GetUserId(User);
            var existingProduct = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == model.Id && p.Vendor.UserId == userId);

            if (existingProduct == null) return Unauthorized();

            if (imageFile != null)
            {
                model.Image = SaveImage(imageFile);
            }
            else
            {
                model.Image = existingProduct.Image; // আগের ছবি রেখে দেওয়া
            }

            model.VendorId = existingProduct.VendorId; // ভেন্ডর আইডি পরিবর্তন করা যাবে না

            _context.Update(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyProducts));
        }

        private string SaveImage(IFormFile file)
        {
            string uniqueName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "images", uniqueName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return uniqueName;
        }
    }
}