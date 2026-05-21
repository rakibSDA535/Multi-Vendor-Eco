using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop111.Models
{
    [Table("Product")]
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string? ProductName { get; set; }

        [Required]
        [MaxLength(40)]
        public string? CompanyName { get; set; }
        [Required]
        public double Price { get; set; }
        [Required, Column(TypeName = "date")]
        public DateTime ReleaseDate { get; set; }
        public bool OnSale { get; set; }
        public string? Image { get; set; }
        public int? VendorId { get; set; }
        // এটি যোগ করলে ডাটাবেজে ApplicationUser-এর সাথে রিলেশন তৈরি হবে
        [ForeignKey("VendorId")]
        public virtual Vendor? Vendor { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public Stock Stock { get; set; }
        public List<OrderDetail> OrderDetail { get; set; }
        public List<CartDetail> CartDetail { get; set; }
        [NotMapped]
        public string GenreName { get; set; }
        [NotMapped]
        public int Quantity { get; set; }
        public ICollection<ProductInformation> ProductInformations { get; set; } = new List<ProductInformation>();


    }
}
