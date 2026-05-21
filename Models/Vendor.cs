using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop111.Models
{
    [Table("Vendor")]
    public class Vendor
    {
        public int Id { get; set; }

        // --- ভেন্ডর বা দোকানের তথ্য ---
        [Required]
        public string VendorName { get; set; }
        public string? VendorImage { get; set; }
        public string? VendorAddress { get; set; }
        public string? VendorEmail { get; set; }
        [Required]
        public string VendorNumber { get; set; } // দোকানের কন্টাক্ট নম্বর
        public string? VendorDescription { get; set; }
        public string? VendorType { get; set; } // যেমন: ইলেকট্রনিক্স, গ্রোসারি ইত্যাদি
        public string? TradeLicenceNo { get; set; }

        // --- মালিকের (Owner) অতিরিক্ত তথ্য ---
        public string? OwnerImage { get; set; }
        public string? OwnerDescription { get; set; }
        // দ্রষ্টব্য: মালিকের নাম, ইমেইল, পাসওয়ার্ড Identity (ApplicationUser) থেকে আসবে

        // --- স্ট্যাটাস ও ট্র্যাকিং ---
        public DateTime? JoinDate { get; set; } = DateTime.Today;
        public bool IsLive { get; set; } = false; // অ্যাডমিন চাইলে দোকানটি হাইড করে রাখতে পারবে

        // --- রিলেশনশিপ (Identity User এর সাথে কানেকশন) ---
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        // --- লোকেশন রিলেশনশিপ ---
        public int LocationId { get; set; }
        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }

        // একজন ভেন্ডরের অনেক প্রোডাক্ট থাকে
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
