
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop111.Constants;
using Shop111.Models;
using Shop111.Models.DTOs;
using Shop111.Repositories;
using Shop111.Shared;
// SQLitePCL এর প্রয়োজন নেই, তাই এটি মুছে ফেলা হলো

namespace Shop111.Controllers;

public class ProductController : Controller
{
    private readonly IProductRepository _productRepo;
    private readonly IGenreRepository _genreRepo;
    private readonly IFileService _fileService;
        private readonly UserManager<ApplicationUser> _userManager; // ইউজার ম্যানেজারের রেফারেন্স
        private readonly ApplicationDbContext _context; // ApplicationDbContext এর রেফারেন্স


    public ProductController(IProductRepository productRepo, IGenreRepository genreRepo, IFileService fileService, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _productRepo = productRepo;
        _genreRepo = genreRepo;
        _fileService = fileService;
        _userManager = userManager;
        _context = context;
    }

    [Authorize(Roles = nameof(Roles.Admin) + ", " + nameof(Roles.Manager))]
    //public async Task<IActionResult> Index(string searchQuery, int page = 1, int pageSize = 5)
    //{
    //    var query = _productRepo.GetAllProductsQueryable();
    //    // নিশ্চিত করুন যে GetAllProductsQueryable() এ Stock এবং Genre ইনক্লুড করা আছে
    //    if (!string.IsNullOrEmpty(searchQuery))
    //    {
    //        query = query.Where(p => p.ProductName.Contains(searchQuery) || p.CompanyName.Contains(searchQuery));
    //    }

    //    // মোট আইটেম গণনা
    //    int totalItems = await query.CountAsync();

    //    // pageSize এর মান নেগেটিভ বা শূন্য হলে তাকে ডিফল্ট মান (যেমন 5) দিন
    //    if (pageSize < 1) pageSize = 5;

    //    int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

    //    // পেজ ভ্যালিডেশন
    //    if (page > totalPages && totalPages > 0) page = totalPages;
    //    if (page < 1) page = 1;

    //    // পেজিনেশন লজিক
    //    var products = await query
    //        .OrderBy(p => p.ProductName)
    //        .Skip((page - 1) * pageSize)
    //        .Take(pageSize)
    //        .ToListAsync();

    //    // ViewBag আপডেট
    //    ViewBag.SearchQuery = searchQuery;
    //    ViewBag.CurrentPage = page;
    //    ViewBag.TotalPages = totalPages;
    //    ViewBag.PageSize = pageSize;
    //    ViewBag.TotalItems = totalItems;

    //    return View(products);
    //}
    public async Task<IActionResult> Index(string searchQuery, int page = 1, int pageSize = 5)
    {
        // ১. শুরুতে কুয়েরি জেনারেট করা
        var query = _productRepo.GetAllProductsQueryable();

        // ২. ম্যানেজার ফিল্টারিং (এখানেই আপনার ভুল ছিল)
        if (User.IsInRole(nameof(Roles.Manager)))
        {
            var userId = _userManager.GetUserId(User);
            query = query.Where(p => p.Vendor.UserId == userId);
            // সরাসরি query ব্যবহার করুন, নতুন ভেরিয়েবল দরকার নেই
            var products = await _context.Products
                                 .Where(p => p.Vendor.UserId == userId)
                                 .Include(p => p.Stock) // স্টক তথ্যসহ
                                 .ToListAsync();
        }

        // ৩. সার্চিং লজিক
        if (!string.IsNullOrEmpty(searchQuery))
        {
            query = query.Where(p => p.ProductName.Contains(searchQuery) || p.CompanyName.Contains(searchQuery));
        }

        // ৪. পেজিনেশন প্যারামিটার ক্যালকুলেশন
        int totalItems = await query.CountAsync();
        if (pageSize < 1) pageSize = 5;
        int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        if (page > totalPages && totalPages > 0) page = totalPages;
        if (page < 1) page = 1;

        // ৫. ফাইনাল ডাটা রিট্রিভ (ভুল: এখানে 'var products' নামে আগে থেকেই ডাটা ছিল, তাই 'var' বাদ দিন)
        var pagedProducts = await query // নাম পরিবর্তন করা হলো কনফ্লিক্ট এড়াতে
            .Include(p => p.Stock)
            .OrderBy(p => p.ProductName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.SearchQuery = searchQuery;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = totalItems;

        return View(pagedProducts);
    }

    [Authorize(Roles = nameof(Roles.Admin) + ", " + nameof(Roles.Manager))]
    public async Task<IActionResult> AddProduct()
    {
        // ========Manager (Vendor) এর জন্য চেক করা হচ্ছে যে তার অ্যাকাউন্ট লাইভ আছে কিনা
        // যদি ইউজার এডমিন হয়, তবে সরাসরি এক্সেস পাবে। 
        // যদি ম্যানেজার হয়, তবেই কেবল IsLive চেক হবে।
        if (!User.IsInRole("Admin"))
        {
            var userId = _userManager.GetUserId(User);
            var vendor = await _context.Vendors.FirstOrDefaultAsync(v => v.UserId == userId);

            if (vendor == null || !vendor.IsLive)
            {
                TempData["errorMessage"] = "Your account is paused. You cannot manage products.";
                return RedirectToAction("Index", "Home");
            }
        }
        // =========Manager (Vendor) এর জন্য চেক করা হচ্ছে যে তার অ্যাকাউন্ট লাইভ আছে কিনা
        var genreSelectList = (await _genreRepo.GetGenres()).Select(genre => new SelectListItem
        {
            Text = genre.GenreName,
            Value = genre.Id.ToString(),
        });

        // initialize with empty product detail list
        ProductDTO productToAdd = new()
        {
            GenreList = genreSelectList,
            ReleaseDate = DateTime.Today, // ReleaseDate এর জন্য ডিফল্ট মান
            ProductInformations = new List<ProductInformationDTO>(),
            Stock = new StockDTO() // StockDTO ইনিশিয়ালাইজ করা হলো
        };
        return View(productToAdd);
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct(ProductDTO productToAdd)
    {
        var genreSelectList = (await _genreRepo.GetGenres()).Select(genre => new SelectListItem
        {
            Text = genre.GenreName,
            Value = genre.Id.ToString(),
        });
        productToAdd.GenreList = genreSelectList;

        // filter out empty ProductInformations (Master-Details logic)
        productToAdd.ProductInformations = productToAdd.ProductInformations?
            .Where(d => !string.IsNullOrWhiteSpace(d.Description) || !string.IsNullOrWhiteSpace(d.MadeIn))
            .ToList() ?? new List<ProductInformationDTO>();

        if (!ModelState.IsValid)
            return View(productToAdd);

        try
        {
            if (productToAdd.ImageFile != null)
            {
                if (productToAdd.ImageFile.Length > 10 * 1024 * 1024)
                    throw new InvalidOperationException("Image file can not exceed 10 MB");

                string[] allowedExtensions = [".jpeg", ".jpg", ".png"];
                string imageName = await _fileService.SaveFile(productToAdd.ImageFile, allowedExtensions);
                productToAdd.Image = imageName;
            }

            Product product = new()
            {
                Id = productToAdd.Id,
                ProductName = productToAdd.ProductName,
                CompanyName = productToAdd.CompanyName,
                Image = productToAdd.Image,
                GenreId = productToAdd.GenreId,
                Price = productToAdd.Price,
                ReleaseDate = productToAdd.ReleaseDate,
                OnSale = productToAdd.OnSale,
            };

            // add ProductInformations (Details)
            foreach (var detail in productToAdd.ProductInformations)
            {
                product.ProductInformations.Add(new ProductInformation
                {
                    Description = detail.Description,
                    MadeIn = detail.MadeIn
                });
            }

            // নতুন পরিবর্তন: Product এর সাথে Stock যুক্ত করা
            product.Stock = new Stock
            {
                Quantity = productToAdd.Stock.Quantity,
                // ProductId স্বয়ংক্রিয়ভাবে EF Core দ্বারা সেট হবে
            };

            await _productRepo.AddProduct(product);
            TempData["successMessage"] = "Product is added successfully";
            return RedirectToAction(nameof(AddProduct));
        }
        catch (InvalidOperationException ex)
        {
            TempData["errorMessage"] = ex.Message;
            return View(productToAdd);
        }
        catch (FileNotFoundException ex)
        {
            TempData["errorMessage"] = ex.Message;
            return View(productToAdd);
        }
        catch (Exception)
        {
            TempData["errorMessage"] = "Error on saving data";
            return View(productToAdd);
        }
    }

    [Authorize(Roles = nameof(Roles.Admin) + ", " + nameof(Roles.Manager))]
    public async Task<IActionResult> UpdateProduct(int id)
    {
        // Stock এবং ProductInformations লোড করার জন্য GetProductById() মেথডটি আপডেট করা প্রয়োজন। 
        // ধরে নিচ্ছি এটি সঠিকভাবে Stock এবং ProductInformations লোড করছে।
        var product = await _productRepo.GetProductById(id);

        if (product == null)
        {
            TempData["errorMessage"] = $"Product with the id: {id} does not found";
            return RedirectToAction(nameof(Index));
        }

        var genreSelectList = (await _genreRepo.GetGenres()).Select(genre => new SelectListItem
        {
            Text = genre.GenreName,
            Value = genre.Id.ToString(),
            Selected = genre.Id == product.GenreId
        });

        ProductDTO productToUpdate = new()
        {
            Id = product.Id,
            GenreList = genreSelectList,
            ProductName = product.ProductName,
            CompanyName = product.CompanyName,
            GenreId = product.GenreId,
            Price = product.Price,
            Image = product.Image,
            ReleaseDate = product.ReleaseDate,
            OnSale = product.OnSale,
            // Stock তথ্য DTO তে ম্যাপ করা হলো
            Stock = product.Stock != null ? new StockDTO
            {
                Id = product.Stock.Id,
                Quantity = product.Stock.Quantity
            } : new StockDTO(),
            // ProductInformations ম্যাপ করা
            ProductInformations = product.ProductInformations.Select(d => new ProductInformationDTO
            {
                Id = d.Id,
                Description = d.Description,
                MadeIn = d.MadeIn
            }).ToList()
        };

        return View(productToUpdate);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProduct(ProductDTO productToUpdate)
    {
        var genreSelectList = (await _genreRepo.GetGenres()).Select(genre => new SelectListItem
        {
            Text = genre.GenreName,
            Value = genre.Id.ToString(),
            Selected = genre.Id == productToUpdate.GenreId
        });
        productToUpdate.GenreList = genreSelectList;

        // filter out empty ProductInformations (Master-Details logic)
        productToUpdate.ProductInformations = productToUpdate.ProductInformations?
            .Where(d => !string.IsNullOrWhiteSpace(d.Description) || !string.IsNullOrWhiteSpace(d.MadeIn))
            .ToList() ?? new List<ProductInformationDTO>();

        if (!ModelState.IsValid)
            return View(productToUpdate);

        try
        {
            string oldImage = "";
            if (productToUpdate.ImageFile != null)
            {
                if (productToUpdate.ImageFile.Length > 2 * 1024 * 1024)
                    throw new InvalidOperationException("Image file can not exceed 2 MB");

                string[] allowedExtensions = [".jpeg", ".jpg", ".png"];
                string imageName = await _fileService.SaveFile(productToUpdate.ImageFile, allowedExtensions);
                oldImage = productToUpdate.Image; // Save old image name to delete later
                productToUpdate.Image = imageName;
            }

            // বিদ্যমান প্রোডাক্টটি তার সকল নেভিগেশন প্রপার্টি (যেমন Stock, ProductInformations) সহ লোড করতে হবে
            var existingProduct = await _productRepo.GetProductById(productToUpdate.Id);

            if (existingProduct == null)
            {
                TempData["errorMessage"] = "Product not found";
                return RedirectToAction(nameof(Index));
            }

            // Update main info
            existingProduct.ProductName = productToUpdate.ProductName;
            existingProduct.CompanyName = productToUpdate.CompanyName;
            existingProduct.GenreId = productToUpdate.GenreId;
            existingProduct.Price = productToUpdate.Price;
            existingProduct.ReleaseDate = productToUpdate.ReleaseDate;
            existingProduct.OnSale = productToUpdate.OnSale;
            existingProduct.Image = productToUpdate.Image ?? existingProduct.Image;

            // ৪. স্টকের পরিমাণ আপডেট করা - "existingProduct" ব্যবহার করা হয়েছে
            if (existingProduct.Stock != null)
            {
                existingProduct.Stock.Quantity = productToUpdate.Stock.Quantity;
            }
            else // যদি কোনো কারণে স্টক না থাকে, তবে নতুন স্টক তৈরি করা
            {
                // এটি সম্ভব যদি Product/Stock এর রিলেশনশিপ One-to-One হয় এবং Stock আগে তৈরি না হয়।
                existingProduct.Stock = new Stock
                {
                    Quantity = productToUpdate.Stock.Quantity,
                    // ProductId = existingProduct.Id // EF Core স্বয়ংক্রিয়ভাবে হ্যান্ডেল করবে
                };
            }

            // Update ProductInformations (remove, update, add) - Master-Details logic
            var detailIds = productToUpdate.ProductInformations.Select(d => d.Id).ToList();
            var toRemove = existingProduct.ProductInformations.Where(d => !detailIds.Contains(d.Id)).ToList();

            // Remove deleted items from the collection
            foreach (var d in toRemove) existingProduct.ProductInformations.Remove(d);

            // Update existing and add new items
            foreach (var detailVm in productToUpdate.ProductInformations)
            {
                if (detailVm.Id > 0)
                {
                    // existing detail
                    var existingDetail = existingProduct.ProductInformations.FirstOrDefault(d => d.Id == detailVm.Id);
                    if (existingDetail != null)
                    {
                        existingDetail.Description = detailVm.Description;
                        existingDetail.MadeIn = detailVm.MadeIn;
                    }
                }
                else
                {
                    // new detail
                    existingProduct.ProductInformations.Add(new ProductInformation
                    {
                        Description = detailVm.Description,
                        MadeIn = detailVm.MadeIn
                    });
                }
            }

            await _productRepo.UpdateProduct(existingProduct);

            // Delete old image file after successful database update
            if (!string.IsNullOrWhiteSpace(oldImage))
                _fileService.DeleteFile(oldImage);

            TempData["successMessage"] = "Product is updated successfully";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["errorMessage"] = $"Error on saving data: {ex.Message}";
            return View(productToUpdate);
        }
    }

    [Authorize(Roles = $"{nameof(Roles.User)}, {nameof(Roles.Admin)}, {nameof(Roles.Manager)}")]
    public async Task<IActionResult> ProductDetails(int id)
    {
        var product = await _productRepo.GetProductById(id);

        if (product == null)
        {
            TempData["errorMessage"] = $"Product with ID {id} was not found.";
            return RedirectToAction(nameof(Index));
        }

        // Ensuring GenreName is populated for the view
        product.GenreName = product.Genre?.GenreName ?? "N/A";

        return View(product);
    }

    [Authorize(Roles = nameof(Roles.Admin) + ", " + nameof(Roles.Manager))]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            var product = await _productRepo.GetProductById(id);
            if (product == null)
            {
                TempData["errorMessage"] = $"Product with the id: {id} does not found";
            }
            else
            {
                await _productRepo.DeleteProduct(product);
                // The related ProductInformations (details) and Stock are assumed to be deleted via cascade delete.

                if (!string.IsNullOrWhiteSpace(product.Image))
                    _fileService.DeleteFile(product.Image);
            }
        }
        catch (Exception ex)
        {
            TempData["errorMessage"] = $"Error on deleting data: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }
}
