namespace EcommerceRestaurant.Web.Data
{
    using Entities;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    // Ctrl + .

    // public class DataContext: DbContext
    public class DataContext: IdentityDbContext<User>
    {
        public DbSet<Product> Products { get; set; }

        public DataContext(DbContextOptions<DataContext> options): base(options)
        {

        }
    }
}
