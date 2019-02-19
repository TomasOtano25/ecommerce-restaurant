namespace EcommerceRestaurant.Web.Data
{
    using Entities;
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Linq;
    using System.Threading.Tasks;


    public class SeedDb
    {
        private readonly DataContext context;
        private readonly UserManager<User> userManager;
        private readonly Random random;

        public SeedDb(DataContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.random = new Random();
        }

        public async Task SeedAsync()
        {
            await this.context.Database.EnsureCreatedAsync();


            var user = await this.userManager.FindByEmailAsync("tomasotano25@gmail.com");
            if (user == null)
            {
                user = new User
                {
                    FirstName = "Tomas",
                    LastName = "Garcia",
                    Email = "tomasotano25@gmail.com",
                    UserName = "tomasotano25@gmail.com",
                    PhoneNumber = "8496388432"
                };

                var result = await this.userManager.CreateAsync(user, "T12121212");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }
            }

            if (!this.context.Products.Any())
            {
                this.AddProduct("First Product", user);
                this.AddProduct("Second Product", user);
                this.AddProduct("Third Product", user);
                await this.context.SaveChangesAsync();
            }
        }

        private void AddProduct(string name, User user)
        {
            this.context.Products.Add(new Product()
            {
                Name = name,
                Price = this.random.Next(1000),
                IsAvailabe = true,
                Stock = this.random.Next(100),
                User = user
            });
        }

    }
}
