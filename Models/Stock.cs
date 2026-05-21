using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Build.Framework;

namespace Shop111.Models
{
    [Table("Stock")]
    public class Stock
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? VendorId { get; set; } // ভেন্ডরের Identity User ID
        public virtual ApplicationUser Vendor { get; set; }
        public int Quantity { get; set; }

        public Product? Product { get; set; }
    }
}
