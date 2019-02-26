namespace EcommerceRestaurant.Web.Data
{
    using System.Linq;
    using System.Threading.Tasks;
    using Entities;
    using Microsoft.EntityFrameworkCore;

    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly DataContext context;

        public ProductRepository(DataContext context) : base(context)
        {
            this.context = context;
        }

        public IQueryable<Product> GetAllWithUser(string apiUrl)
        {
            var products = this.context.Products.Include(p => p.User);

            foreach (var product in products)
            {
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    product.ImageFullPath = $"{apiUrl}{product.ImageUrl.Substring(1)}";
                } 
            }

            return products;
        }

        public override async Task<Product> GetByIdAsync(int id)
        {
            return await this.context.Products.Include(p => p.User).Where(p => p.Id == id).FirstOrDefaultAsync();
        }
    }
}