namespace EcommerceRestaurant.Web.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Helpers;
    using Entities;
    using Microsoft.AspNetCore.Identity;

    public class SeedDb
    {
        private readonly DataContext context;
        private readonly IUserHelper userHelper;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly Random random;

        public SeedDb(DataContext context, IUserHelper userHelper, RoleManager<IdentityRole> roleManager)
        {
            this.context = context;
            this.userHelper = userHelper;
            this.roleManager = roleManager;
            this.random = new Random();
        }

        public IUserHelper UserHelper { get; }

        public async Task SeedAsync()
        {
            await this.context.Database.EnsureCreatedAsync();

            await this.CheckRole("Admin");
            await this.CheckRole("Customer");

            if (!this.context.Countries.Any())
            {
                var cities = new List<City>();
                cities.Add(new City { Name = "Santo Domingo" });
                cities.Add(new City { Name = "Santiago" });
                cities.Add(new City { Name = "San Juan" });

                this.context.Countries.Add(new Country
                {
                    Cities = cities,
                    Name = "Republica Dominicana"
                });

                await this.context.SaveChangesAsync();
            }

            var user = await this.userHelper.GetUserByEmailAsync("tomasotano25@gmail.com");
            if (user == null)
            {
                user = new User
                {
                    FirstName = "Tomas",
                    LastName = "Garcia",
                    Email = "tomasotano25@gmail.com",
                    UserName = "tomasotano25@gmail.com",
                    PhoneNumber = "8496388432",
                    Address = "Calle Duarte #382",
                    CityId = this.context.Countries.FirstOrDefault().Cities.FirstOrDefault().Id,
                    City = this.context.Countries.FirstOrDefault().Cities.FirstOrDefault()
                };

                var result = await this.userHelper.AddUserAsync(user, "T12121212");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                await this.userHelper.AddToRoleUserAsync(user, "Admin");
            }

            if (!this.context.Products.Any())
            {
                this.AddProduct("First Product", user);
                this.AddProduct("Second Product", user);
                this.AddProduct("Third Product", user);
                await this.context.SaveChangesAsync();
            }
        }

        private async Task CheckRole(string roleName)
        {
            var roleExists = await this.roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await this.roleManager.CreateAsync(new IdentityRole
                {
                    Name = roleName
                });
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
