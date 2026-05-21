////Licensed to the.NET Foundation under one or more agreements.
//// The .NET Foundation licenses this file to you under the MIT license.
//#nullable disable

//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.UI.Services;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.Extensions.Logging;

//namespace Shop111.Areas.Identity.Pages.Account
//{
//    public class LoginModel : PageModel
//    {
//        private readonly SignInManager<ApplicationUser> _signInManager;
//        private readonly ILogger<LoginModel> _logger;

//        public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger)
//        {
//            _signInManager = signInManager;
//            _logger = logger;
//        }

//        /// <summary>
//        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
//        ///     directly from your code. This API may change or be removed in future releases.
//        /// </summary>
//        [BindProperty]
//        public InputModel Input { get; set; }

//        /// <summary>
//        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
//        ///     directly from your code. This API may change or be removed in future releases.
//        /// </summary>
//        public IList<AuthenticationScheme> ExternalLogins { get; set; }

//        /// <summary>
//        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
//        ///     directly from your code. This API may change or be removed in future releases.
//        /// </summary>
//        public string ReturnUrl { get; set; }

//        /// <summary>
//        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
//        ///     directly from your code. This API may change or be removed in future releases.
//        /// </summary>
//        [TempData]
//        public string ErrorMessage { get; set; }

//        /// <summary>
//        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
//        ///     directly from your code. This API may change or be removed in future releases.
//        /// </summary>
//        public class InputModel
//        {
//            /// <summary>
//            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
//            ///     directly from your code. This API may change or be removed in future releases.
//            /// </summary>
//            [Required]
//            [EmailAddress]
//            public string Email { get; set; }

//            /// <summary>
//            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
//            ///     directly from your code. This API may change or be removed in future releases.
//            /// </summary>
//            [Required]
//            [DataType(DataType.Password)]
//            public string Password { get; set; }

//            /// <summary>
//            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
//            ///     directly from your code. This API may change or be removed in future releases.
//            /// </summary>
//            [Display(Name = "Remember me?")]
//            public bool RememberMe { get; set; }
//        }

//        public async Task OnGetAsync(string returnUrl = null)
//        {
//            if (!string.IsNullOrEmpty(ErrorMessage))
//            {
//                ModelState.AddModelError(string.Empty, ErrorMessage);
//            }

//            returnUrl ??= Url.Content("~/");

//            // Clear the existing external cookie to ensure a clean login process
//            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

//            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

//            ReturnUrl = returnUrl;
//        }

//        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
//        {
//            returnUrl ??= Url.Content("~/");

//            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

//            if (ModelState.IsValid)
//            {
//                // This doesn't count login failures towards account lockout
//                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
//                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
//                if (result.Succeeded)
//                {
//                    _logger.LogInformation("User logged in.");
//                    return LocalRedirect(returnUrl);
//                }
//                if (result.RequiresTwoFactor)
//                {
//                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
//                }
//                if (result.IsLockedOut)
//                {
//                    _logger.LogWarning("User account locked out.");
//                    return RedirectToPage("./Lockout");
//                }
//                else
//                {
//                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
//                    return Page();
//                }
//            }

//            // If we got this far, something failed, redisplay form
//            return Page();
//        }
//    }
//}
//==================================
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Shop111.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager; // UserManager যোগ করা হয়েছে
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<ApplicationUser> userManager) // ইনজেক্ট করা হয়েছে
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        //public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        //{
        //    returnUrl ??= Url.Content("~/");

        //    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userManager.FindByEmailAsync(Input.Email);


        //        if (user != null)
        //        {

        //            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        //            var isManager = await _userManager.IsInRoleAsync(user, "Manager");
        //            if (isManager && !user.IsApproved)
        //            {
        //                ModelState.AddModelError(string.Empty, "আপনার ম্যানেজার অ্যাকাউন্টটি এখনো অনুমোদিত নয়।");
        //                return Page();
        //            }
        //        }

        //        var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

        //        if (result.Succeeded)
        //        {
        //            _logger.LogInformation("ইউজার লগইন করেছেন।");
        //            return LocalRedirect(returnUrl);
        //        }
        //        if (result.RequiresTwoFactor)
        //        {
        //            return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
        //        }
        //        if (result.IsLockedOut)
        //        {
        //            _logger.LogWarning("ইউজার অ্যাকাউন্ট লক আউট করা হয়েছে।");
        //            return RedirectToPage("./Lockout");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError(string.Empty, "ইমেইল বা পাসওয়ার্ড সঠিক নয়।");
        //            return Page();
        //        }
        //    }

        //    return Page();
        //}
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // ১. ইমেইল দিয়ে ইউজারকে খুঁজে বের করা
                var user = await _userManager.FindByEmailAsync(Input.Email);

                if (user != null)
                {
                    // ২. রোল এবং অ্যাপ্রুভাল চেক করা
                    var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                    var isManager = await _userManager.IsInRoleAsync(user, "Manager");
                    var isUser = await _userManager.IsInRoleAsync(user, "User");

                    // কাস্টম লজিক: 
                    // যদি ইউজার 'Admin' না হয় এবং তার IsApproved স্ট্যাটাস false থাকে, তবে তাকে ঢুকতে দেওয়া হবে না।
                    if (!isAdmin && !user.IsApproved)
                    {
                        // ম্যানেজার বা সাধারণ ইউজার যেই হোক, অ্যাপ্রুভ না থাকলে এখানে আটকাবে
                        string errorMessage = isManager
                            ? "আপনার ম্যানেজার অ্যাকাউন্টটি এখনো অনুমোদিত নয়। অ্যাডমিনের অনুমতির জন্য অপেক্ষা করুন।"
                            : "আপনার অ্যাকাউন্টটি এখনো অনুমোদিত নয়।";

                        ModelState.AddModelError(string.Empty, errorMessage);
                        return Page();
                    }
                }
                else
                {
                    // ইউজার যদি ডাটাবেজে না থাকে তবে সরাসরি এরর দেওয়া ভালো (নিরাপত্তার জন্য ইউজারকে জানানো হবে না যে ইমেইল ভুল না কি পাসওয়ার্ড)
                    ModelState.AddModelError(string.Empty, "ইমেইল বা পাসওয়ার্ড সঠিক নয়।");
                    return Page();
                }

                // ৩. পাসওয়ার্ড যাচাই এবং সাইন-ইন প্রসেস
                // মনে রাখবেন: PasswordSignInAsync কেবল তখনই কল হবে যদি ইউজার ডাটাবেজে থাকে এবং অ্যাপ্রুভড হয়।
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("ইউজার সফলভাবে লগইন করেছেন।");
                    return LocalRedirect(returnUrl);
                }

                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("ইউজার অ্যাকাউন্ট লক আউট করা হয়েছে।");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    // পাসওয়ার্ড ভুল হলে এই এরর দেখাবে
                    ModelState.AddModelError(string.Empty, "ইমেইল বা পাসওয়ার্ড সঠিক নয়।");
                    return Page();
                }
            }

            // যদি ModelState ভ্যালিড না হয়
            return Page();
        }
    }
}