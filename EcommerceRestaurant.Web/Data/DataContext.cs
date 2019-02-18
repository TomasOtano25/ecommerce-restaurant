namespace EcommerceRestaurant.Web.Data
{
    using EcommerceRestaurant.Web.Data.Entities;
    using Microsoft.EntityFrameworkCore;
    // Ctrl + .

    public class DataContext: DbContext
    {
        public DbSet<Product> Products { get; set; }

        public DataContext(DbContextOptions<DataContext> options): base(options)
        {

        }
    }
}
