using System.ComponentModel.DataAnnotations;

namespace Shop111.Models.DTOs
{
    public class VendorDTO
    {
        // --- ভেন্ডর বা দোকানের তথ্য ---
        [Required(ErrorMessage = "দোকানের নাম দেওয়া বাধ্যতামূলক")]
        [Display(Name = "Shop Name")]
        public string VendorName { get; set; }

        [Display(Name = "Shop Address")]
        public string? VendorAddress { get; set; }

        [EmailAddress(ErrorMessage = "সঠিক ইমেইল এড্রেস দিন")]
        [Display(Name = "Shop Email")]
        public string? VendorEmail { get; set; }

        [Required(ErrorMessage = "কন্টাক্ট নম্বর প্রয়োজন")]
        [Phone]
        [Display(Name = "Contact Number")]
        public string VendorNumber { get; set; }

        [Display(Name = "Description")]
        public string? VendorDescription { get; set; }

        [Display(Name = "Category/Type")]
        public string? VendorType { get; set; }
        [Display(Name = "Join Date")]
        public DateTime? JoinDate { get; set; } = DateTime.UtcNow;
        [Display(Name = "Is Live")]
        public bool IsLive { get; set; } = false;

        [Display(Name = "Trade Licence Number")]
        public string? TradeLicenceNo { get; set; }

        // --- ইমেজ হ্যান্ডলিং (IFormFile ব্যবহার করা হয়েছে ফাইল আপলোডের জন্য) ---
        [Display(Name = "Shop Image")]
        public IFormFile? VendorImageFile { get; set; }

        [Display(Name = "Owner Image")]
        public IFormFile? OwnerImageFile { get; set; }

        // --- মালিকের তথ্য ---
        [Display(Name = "Owner Biography")]
        public string? OwnerDescription { get; set; }

        // --- লোকেশন ---
        [Required(ErrorMessage = "লোকেশন সিলেক্ট করুন")]
        [Display(Name = "Location")]
        public int LocationId { get; set; }

        // Identity User এর সাথে লিংক করার জন্য
        public string? UserId { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
