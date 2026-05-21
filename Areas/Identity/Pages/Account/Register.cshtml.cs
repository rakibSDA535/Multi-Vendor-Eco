using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Shop111.Constants;
using Shop111.Models;
using System.ComponentModel.DataAnnotations;

namespace Shop111.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = (IUserEmailStore<ApplicationUser>)userStore;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public IEnumerable<SelectListItem> RoleList { get; set; }

        public IEnumerable<SelectListItem> LocationList { get; set; }

        public class InputModel
        {
            [Required]
            public string FullName { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public string PhoneNumber { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Role { get; set; }

            // Vendor Fields
            public string? VendorName { get; set; }
            public string? VendorEmail { get; set; }

            [DataType(DataType.Date)]
            public DateTime JoinDate { get; set; } = DateTime.Today;

            public string? VendorAddress { get; set; }

            public string? VendorDescription { get; set; }

            public string? VendorType { get; set; }

            public string? TradeLicenceNo { get; set; }

            public int? LocationId { get; set; }

            // ===== OWNER =====

            public string? OwnerDescription { get; set; }

            // ===== IMAGE =====

            public IFormFile? VendorImage { get; set; }

            public IFormFile? OwnerImage { get; set; }
        }
        private async Task<string?> SaveImage(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;

            // পাথ অবশ্যই wwwroot এর ভেতরে হতে হবে
            string uploadFolder = Path.Combine(_environment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);

            string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            string filePath = Path.Combine(uploadFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }


        public async Task OnGetAsync()
        {
            LoadData();
            Input = new InputModel//for Auto filling JoinDate in the form
            {
                JoinDate = DateTime.Today
            };
            ExternalLogins =
                (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .ToList();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            LoadData();

            if (!ModelState.IsValid)
            {
                //var errors = ModelState.Values.SelectMany(v => v.Errors);
                //foreach (var error in errors)
                //{
                //    Console.WriteLine(error.ErrorMessage); // আউটপুট উইন্ডোতে এরর দেখুন
                //}
                LoadData();
                return Page();
            }

            var existingUser = await _userManager.FindByEmailAsync(Input.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "Email already exists.");
                LoadData();
                return Page();
            }

            bool isManager = Input.Role == nameof(Roles.Manager);

            // ১. ভেন্ডর ভ্যালিডেশন চেক
            if (isManager)
            {
                if (string.IsNullOrWhiteSpace(Input.VendorName))
                {
                    ModelState.AddModelError("", "Vendor Name is required.");
                    return Page();
                }
            }

            var user = new ApplicationUser
            {
                FullName = Input.FullName,
                PhoneNumber = Input.PhoneNumber,
                Email = Input.Email,
                UserName = Input.Email,
                IsApproved = !isManager // ম্যানেজার হলে এপ্রুভাল লাগবে
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Input.Role);

                if (isManager)
                {
                    // ২. ইমেজ সেভ করা (আপনার ভিউতে দেখানোর জন্য পাথ 'uploads' এর জায়গায় 'Images' করা ভালো হতে পারে)
                    // আমি আপনার কন্ট্রোলারের সাথে মিল রেখে এখানে 'uploads' রাখছি
                    string? vendorImage = await SaveImage(Input.VendorImage);
                    string? ownerImage = await SaveImage(Input.OwnerImage);

                    Vendor vendor = new Vendor
                    {
                        UserId = user.Id,
                        VendorName = Input.VendorName!,
                        VendorEmail = Input.VendorEmail,
                        VendorNumber = Input.PhoneNumber,
                        VendorAddress = Input.VendorAddress,
                        VendorDescription = Input.VendorDescription,
                        VendorType = Input.VendorType,
                        TradeLicenceNo = Input.TradeLicenceNo,
                        OwnerDescription = Input.OwnerDescription,
                        VendorImage = vendorImage, // শুধু ফাইলের নাম সেভ হবে
                        OwnerImage = ownerImage,
                        LocationId = Input.LocationId ?? 1,
                        JoinDate =  Input.JoinDate,
                    };

                    _context.Vendors.Add(vendor);
                    await _context.SaveChangesAsync();
                }

                if (user.IsApproved)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToPage("/Index");
                }

                TempData["StatusMessage"] = "Registration successful. Waiting for Admin Approval.";
                return RedirectToPage("./Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        private void LoadData()
        {
            RoleList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = nameof(Roles.User),
                    Text = "Regular User"
                },
                new SelectListItem
                {
                    Value = nameof(Roles.Manager),
                    Text = "Vendor / Manager"
                }
            };

            LocationList = _context.Locations
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.City
                }).ToList();
        }
    }
}

//=========================================






