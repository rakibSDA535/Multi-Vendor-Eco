using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shop111.Constants;
using Shop111.Models.DTOs;
using Shop111.Repositories;

namespace Shop111.Controllers;

[Authorize(Roles = nameof(Roles.Admin))]
public class AdminOperationsController : Controller
{
    private readonly IUserOrderRepository _userOrderRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    public AdminOperationsController(IUserOrderRepository userOrderRepository, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userOrderRepository = userOrderRepository;
        _userManager = userManager;
        _context = context;
    }

    public async Task<IActionResult> AllOrders()
    {
        var orders = await _userOrderRepository.UserOrders(true);
        return View(orders);
    }

    public async Task<IActionResult> TogglePaymentStatus(int orderId)
    {
        try
        {
            await _userOrderRepository.TogglePaymentStatus(orderId);
        }
        catch (Exception ex)
        {
            // log exception here
        }
        return RedirectToAction(nameof(AllOrders));
    }

    public async Task<IActionResult> UpdateOrderStatus(int orderId)
    {
        var order = await _userOrderRepository.GetOrderById(orderId);
        if (order == null)
        {
            throw new InvalidOperationException($"Order with id:{orderId} does not found.");
        }
        var orderStatusList = Enum.GetValues(typeof(EOrderStatus))
                .Cast<EOrderStatus>()
                .Select(orderStatus =>
                {
                    return new SelectListItem
                    {
                        Value = ((int)orderStatus).ToString(),
                        Text = orderStatus.ToString()
                    };
                });

        var data = new UpdateOrderStatusModel
        {
            OrderId = orderId,
            OrderStatus = order.OrderStatus,
            OrderStatusList = orderStatusList
        };
        return View(data);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateOrderStatus(UpdateOrderStatusModel data)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                data.OrderStatusList = Enum.GetValues(typeof(EOrderStatus))
               .Cast<EOrderStatus>()
               .Select(orderStatus =>
               {
                   return new SelectListItem
                   {
                       Value = ((int)orderStatus).ToString(),
                       Text = orderStatus.ToString(),
                       Selected = orderStatus == data.OrderStatus
                   };
               });
                return View(data);
            }

            await _userOrderRepository.ChangeOrderStatus(data);
            TempData["msg"] = "Updated successfully";
        }
        catch (Exception ex)
        {
            // catch exception here
            TempData["msg"] = "Something went wrong";
        }
        return RedirectToAction(nameof(UpdateOrderStatus), new { orderId = data.OrderId });
    }



    // পেন্ডিং ম্যানেজারদের তালিকা
    // ১. পেন্ডিং ম্যানেজারদের তালিকা(ফিল্টারসহ)
    //১. পেন্ডিং ম্যানেজারদের তালিকা (ফিল্টারসহ)
    public async Task<IActionResult> PendingManagers()
    {
        // শুধুমাত্র ম্যানেজার রোলের ইউজারদের খুঁজে বের করা
        var managersInRole = await _userManager.GetUsersInRoleAsync("Manager");
        var managerIds = managersInRole.Select(m => m.Id).ToList();

        var users = await _userManager.Users
            .Include(u => u.Vendor)
            .ThenInclude(v => v.Location)
            .Where(u => managerIds.Contains(u.Id)) // শুধুমাত্র ম্যানেজারদের নিবে
            .ToListAsync();
        return View(users);
    }

    // ২. ম্যানেজার অ্যাপ্রুভ করার অ্যাকশন (Fix: এই মেথডটি মিসিং ছিল)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveManager(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            user.IsApproved = true; // ডাটাবেস কলাম অনুযায়ী
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                TempData["msg"] = "Manager approved successfully!";
            else
                TempData["msg"] = "Error: Could not approve manager.";
        }
        return RedirectToAction(nameof(PendingManagers));
    }

    // ৩. ম্যানেজার রিজেক্ট/স্ট্যাটাস আপডেট
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectManager(string userId)
    {
        // ১. ইউজারকে তার আইডি দিয়ে খুঁজে বের করা
        var user = await _userManager.FindByIdAsync(userId);

        if (user != null)
        {
            // ২. ইউজারের সাথে যদি ভেন্ডর ডাটা থাকে তবে সেটিও ডিলিট করা (Referential Integrity)
            var vendor = await _context.Vendors.FirstOrDefaultAsync(v => v.UserId == userId);
            if (vendor != null)
            {
                _context.Vendors.Remove(vendor);
            }

            // ৩. মেইন ইউজার অ্যাকাউন্ট ডিলিট করা
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                await _context.SaveChangesAsync();
                TempData["msg"] = "Manager request has been rejected and removed.";
            }
            else
            {
                TempData["msg"] = "Error: Could not reject the manager.";
            }
        }

        return RedirectToAction(nameof(PendingManagers));
    }

    // ৪. পজ বা লাইভ করার অ্যাকশন
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleManagerStatus(string userId)
    {
        // ইউজার এবং তার সাথে ভেন্ডর ডাটা লোড করা
        var user = await _userManager.Users
            .Include(u => u.Vendor)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.Vendor != null)
        {
            // IsLive পরিবর্তন করা
            user.Vendor.IsLive = !user.Vendor.IsLive;
            await _context.SaveChangesAsync();
            TempData["msg"] = user.Vendor.IsLive ? $"{user.FullName} is now Live." : $"{user.FullName} is now Paused.";
        }
        else
        {
            TempData["msg"] = "Vendor profile not found for this user.";
        }
        return RedirectToAction(nameof(PendingManagers));
    }
    // 5. অ্যাপ্রুভড ম্যানেজার ডিলিট করার অ্যাকশন
    [HttpPost]
    public async Task<IActionResult> DeleteManager(string userId)
    {
        var user = await _userManager.Users.Include(u => u.Vendor).FirstOrDefaultAsync(u => u.Id == userId);
        if (user != null)
        {
            if (user.Vendor != null) _context.Vendors.Remove(user.Vendor);
            await _userManager.DeleteAsync(user);
            await _context.SaveChangesAsync();
            TempData["msg"] = "Manager deleted successfully.";
        }
        return RedirectToAction(nameof(PendingManagers));
    }
    // 6.সবাইকে দেখতে চাইলে
    public async Task<IActionResult> AllManagers()
    {
        // যারা ম্যানেজার রোল-এ আছে তাদের লিস্ট
        var managers = await _userManager.GetUsersInRoleAsync(nameof(Roles.Manager));
        var managerIds = managers.Select(m => m.Id).ToList();

        var users = await _userManager.Users
            .Include(u => u.Vendor)
            .ThenInclude(v => v.Location)
            .Where(u => u.IsApproved) // শুধুমাত্র যারা অনুমোদিত
            .Where(u => managerIds.Contains(u.Id))
            .ToListAsync();

        //return View("PendingManagers", managers); // আলাদা ভিউ ফাইল না থাকলে PendingManagers ভিউ দিয়েই দেখা যাবে
        return View("PendingManagers", users); // আলাদা ভিউ ফাইল না থাকলে PendingManagers ভিউ দিয়েই দেখা যাবে
    }
    // 7.Details দেখতে চাইলে
    public async Task<IActionResult> ManagerDetails(string userId)
    {
        var user = await _userManager.Users
            .Include(u => u.Vendor)
            .ThenInclude(v => v.Location)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }
    // 8.UserStatistics দেখতে চাইলে
    public async Task<IActionResult> UserStatistics()//Continue
    {
        // ১. সকল ইউজারদের লিস্ট নিয়ে আসা
        var allUsers = await _userManager.Users.Include(u => u.Vendor).ToListAsync();

        // ২. রোল অনুযায়ী আলাদা আলাদা সংখ্যা বের করা
        var admins = await _userManager.GetUsersInRoleAsync("Admin");
        var managers = await _userManager.GetUsersInRoleAsync("Manager");
        var user = await _userManager.GetUsersInRoleAsync("User");
        // ৩. ভিউতে ডাটা পাস করা
        ViewBag.AdminCount = admins.Count;
        ViewBag.ManagerCount = managers.Count;
        ViewBag.UserCount = user.Count;
        ViewBag.TotalUsers = allUsers.Count;


        return View(allUsers);
    }

    // ১. এডিট পেজ দেখানো (GET)
    public async Task<IActionResult> ManagerEdit(string userId)//Continue
    {
        var user = await _userManager.Users
            .Include(u => u.Vendor)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return NotFound();

        return View(user);
    }

    // ২. ডাটা আপডেট করা (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManagerEdit(string id, ApplicationUser model)//Continue
    {
        if (id != model.Id) return NotFound();
        // ১. প্রথমে ডাটাবেস থেকে অরিজিনাল ইউজারটি বের করুন
        var user = await _userManager.Users
            .Include(u => u.Vendor)
            .FirstOrDefaultAsync(u => u.Id == model.Id);

        if (user == null)
        {
            return NotFound();
        }

        // ২. ডাটা আপডেট লজিক (আপনার ডাটাবেস কলামের নাম অনুযায়ী চেক করুন)
        user.FullName = model.FullName;
        user.PhoneNumber = model.PhoneNumber;

        if (user.Vendor != null && model.Vendor != null)
        {
            user.Vendor.VendorName = model.Vendor.VendorName;
            user.Vendor.VendorAddress = model.Vendor.VendorAddress;
            user.Vendor.OwnerDescription = model.Vendor.OwnerDescription;
            user.Vendor.VendorDescription = model.Vendor.VendorDescription;
        }

        // ৩. সেভ করা
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            await _context.SaveChangesAsync();
            TempData["msg"] = "Updated successfully!";
            return RedirectToAction(nameof(PendingManagers));
        }

        return View(model);
    }
    public async Task<IActionResult> Dashboard()
    {
        // ১. অনুমোদিত ম্যানেজারের সংখ্যা (যারা IsApproved = true)
        var approvedCount = await _userManager.Users
            .CountAsync(u => u.IsApproved);

        // ২. পেন্ডিং রিকোয়েস্টের সংখ্যা (যারা IsApproved = false)
        var pendingCount = await _userManager.Users
            .CountAsync(u => !u.IsApproved);

        // ৩. রিজেক্টেড সংখ্যা (যদি আপনি ডাটা ডিলিট করে দেন, তবে এটি ট্র্যাক করা সম্ভব নয়)
        // যদি ডাটা ডিলিট না করে Status কলাম রাখতেন, তবে এটিও কাউন্ট করা যেত।

        ViewBag.ApprovedCount = approvedCount;
        ViewBag.PendingCount = pendingCount;

        return View();
    }

}
