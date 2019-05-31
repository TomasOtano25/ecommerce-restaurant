using EcommerceRestaurant.Web.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace EcommerceRestaurant.Web.Test
{
    public class WithTestDatabase
    {
        public static async Task Run(Func testFunc)
        {
            var options = new DbContextOptionsBuilder()
                .UseInMemoryDatabase("IN_MEMORY_DATABASE")
                .Options;

            using (var context = new DataContext(options))
            {
                try
                {
                    await context.Database.EnsureCreatedAsync();
                }
                catch (Exception)   
                {

                    throw;
                }

            }

        }
    }
}
