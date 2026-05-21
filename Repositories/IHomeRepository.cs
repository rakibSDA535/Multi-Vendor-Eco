using Shop111.Models;

namespace Shop111.Repositories
{
    public interface IHomeRepository
    {
        Task<IEnumerable<Product>> GetProducts(string sTerm = "", int genreId = 0);
        Task<IEnumerable<Genre>> Genres();
    }
}
