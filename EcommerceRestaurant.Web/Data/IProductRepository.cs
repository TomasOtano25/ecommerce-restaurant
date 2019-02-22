namespace EcommerceRestaurant.Web.Data
{
    using Entities;
    using System.Linq;

    public interface IProductRepository : IGenericRepository<Product>
    {
        IQueryable<Product> GetAllWithUser(string apiUrl);
    }
}
