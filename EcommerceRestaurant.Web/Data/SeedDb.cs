namespace EcommerceRestaurant.Web.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Helpers;
    using Entities;
    using Microsoft.AspNetCore.Identity;
    using Bogus;

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

            await this.CheckRoles();

            if (!this.context.Countries.Any())
            {
                await this.CountriesAndCities();
            }

            await this.CheckUser("bob@bob.com", "Bob", "Bob", "Customer");
            await this.CheckUser("brad@gmail.com", "Brad", "Pit", "Customer");
            await this.CheckUser("angelina@gmail.com", "Angelina", "Jolie", "Customer");

            var user = await this.CheckUser("tomasotano25@gmail.com", "Tomas", "Garcia", "Admin");

            if (!this.context.Products.Any())
            {
                this.AddProduct("AirPods", 159, user);
                this.AddProduct("iPad Pro", 799, user);
                this.AddProduct("iPhone X", 749, user);
                this.AddProduct("iWatch Series 4", 399, user);
                this.AddProduct("Mac Book Pro", 1299, user);
                this.AddProduct("Magic Mouse", 47, user);
                this.AddProduct("Wireless Charging Pad", 67.67M, user);
                await this.context.SaveChangesAsync();
            }
        }

        private void AddProduct(string name, decimal price, User user)
        {
            this.context.Products.Add(new Product
            {
                Name = name,
                Price = price,
                IsAvailabe = true,
                Stock= this.random.Next(100),
                User = user,
                ImageUrl = $"~/images/Products/{name}.png"
            });
        }

        private async Task CountriesAndCities()
        {
            var citiesRD = new List<City>
            {
                new City { Name = "Santo Domingo" },
                new City { Name = "Santiago" },
                new City { Name = "San Juan" }
            };

            this.context.Countries.Add(new Country
            {
                Cities = citiesRD,
                Name = "Republica Dominicana"
            });

            var citiesUSA = new List<City>
            {
                new City { Name = "New York" },
                new City { Name = "Los Ángeles" },
                new City { Name = "Chicago" }
            };

            this.context.Countries.Add(new Country
            {
                Cities = citiesUSA,
                Name = "Estados Unidos"
            });

            var citiesArg = new List<City>
            {
                new City { Name = "Córdoba" },
                new City { Name = "Buenos Aires" },
                new City { Name = "Rosario" }
            };

            this.context.Countries.Add(new Country
            {
               Cities = citiesArg,
               Name = "Argentina"
            });

            var citiesCol = new List<City>
            {
                new City { Name = "Medellín" },
                new City { Name = "Bogotá" },
                new City { Name = "Calí" }
            };

            this.context.Countries.Add(new Country
            {
                Cities = citiesCol,
                Name = "Colombia"
            });

            await this.context.SaveChangesAsync();
        }

        private async Task CheckRoles()
        {
            await this.userHelper.CheckRoleAsync("Admin");
            await this.userHelper.CheckRoleAsync("Customer");
        }

        private async Task<User> CheckUser(string userName, string firstName, string lastName, string role)
        {
            var user = await this.userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                user = await this.AddUser(userName, firstName, lastName, role);
            }

            var isInRole = await this.userHelper.IsUserInRoleAsync(user, role);
            if (!isInRole)
            {
                await this.userHelper.AddUserToRoleUserAsync(user, role);
            }

            return user;
        }

        private async Task<User> AddUser(string userName, string firstName, string lastName, string role)
        {
            var faker = new Faker("es");

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = userName,
                UserName = userName,
                Address = faker.Address.StreetAddress(),
                PhoneNumber = faker.Phone.PhoneNumber(format: "##########"),
                CityId = this.context.Countries.FirstOrDefault().Cities.FirstOrDefault().Id,
                City = this.context.Countries.FirstOrDefault().Cities.FirstOrDefault()
            };

            var result = await this.userHelper.AddUserAsync(user, "T12121212");
            if (result != IdentityResult.Success)
            {
                throw new InvalidOperationException("Could not create the user in seeder");
            }

            await this.userHelper.AddUserToRoleUserAsync(user, role);
            var token = await this.userHelper.GenerateEmailConfirmationTokenAsync(user);
            await this.userHelper.ConfirmEmailAsync(user, token);

            return user;
        }
    }
}
