using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop111.Data;
using Shop111.Models;

namespace Shop111.Repositories
{
    public interface IProductRepository
    {
         Task AddProduct(Product product);
         Task UpdateProduct(Product product);
         Task DeleteProduct(Product product);
         Task<Product?> GetProductById(int id);
        Task<IEnumerable<Product>> GetProducts();
        // **নতুন মেথড: IQueryable রিটার্ন করবে**
         IQueryable<Product> GetAllProductsQueryable();

    }

    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task AddProduct(Product product)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
            }

            public async Task UpdateProduct(Product product)
            {
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }

            public async Task DeleteProduct(Product product)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        //======================xx
        public async Task<Product?> GetProductById(int id)
        {
            return await _context.Products
                .Include(p => p.Genre)      // Genre টেবিলও Include করো
                .Include(p => p.ProductInformations)
                .Include(p => p.Stock)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        //==============================xx
        public async Task<IEnumerable<Product>> GetProducts() => await _context.Products.Include(a => a.Genre).ToListAsync();
        // **নতুন মেথডের বাস্তবায়ন**
        public IQueryable<Product> GetAllProductsQueryable()
        {
            // Genre সহ IQueryable<Product> রিটার্ন করা হলো, কিন্তু ডেটা এখনো লোড করা হয়নি
            return _context.Products.Include(p => p.Genre).Include(p => p.ProductInformations).Include(p => p.Stock).AsQueryable();//
        }
    }
    
}
