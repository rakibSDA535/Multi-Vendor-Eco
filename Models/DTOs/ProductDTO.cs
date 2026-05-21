using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop111.Models.DTOs
{
    public class ProductDTO
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
        public int VendorId { get; set; }//
        [Required]
        public int GenreId { get; set; }
        public IFormFile? ImageFile { get; set; }
        public IEnumerable<SelectListItem>? GenreList { get; set; }
        public List<ProductInformationDTO> ProductInformations { get; set; } = new List<ProductInformationDTO>();
        [Required] // স্টকের পরিমাণ অবশ্যই দিতে হবে
        public StockDTO Stock { get; set; } = new StockDTO();
    }
}

