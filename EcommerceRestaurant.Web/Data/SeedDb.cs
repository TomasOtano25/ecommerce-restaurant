namespace EcommerceRestaurant.Web.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Helpers;
    using Entities;
    using Microsoft.AspNetCore.Identity;
   
    public class SeedDb
    {
        private readonly DataContext context;
        private readonly IUserHelper userHelper;
        private readonly Random random;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            this.context = context;
            this.UserHelper = userHelper;
          
            this.random = new Random();
        }

        public IUserHelper UserHelper { get; }

        public async Task SeedAsync()
        {
            await this.context.Database.EnsureCreatedAsync();


            var user = await this.userHelper.GetUserByEmailAsync("tomasotano25@gmail.com");
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

                var result = await this.userHelper.AddUserAsync(user, "T12121212");
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
