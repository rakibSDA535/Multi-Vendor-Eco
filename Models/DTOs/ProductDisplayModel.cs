using Shop111.Models;

namespace Shop111.Models.DTOs
{
    public class ProductDisplayModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Genre> Genres { get; set; }
        public string STerm { get; set; } = "";
        public int GenreId { get; set; } = 0;

        public IEnumerable<TopNSoldProductModel> TopNSoldProducts { get; set; }

    }
}



        

