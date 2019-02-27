namespace EcommerceRestaurant.Web.Data.Repositories
{
    using Models;
    using Entities;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CountryRepository : GenericRepository<Country>, ICountryRepository
    {
        private readonly DataContext context;

        public CountryRepository(DataContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Country>> GetAllAsync()
        {
            return await this.context.Countries
                .Include(c => c.Cities)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Country> GetAsync(int id)
        {
            return await this.context.Countries
                .Include(c => c.Cities)
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<City> GetCityAsync(int id)
        {
            return await this.context.Cities.FindAsync(id);
        }

        public async Task AddCity(CityViewModel model)
        {
            var country = await this.GetAsync(model.CountryId);
            if (country == null)
            {
                return;
            }

            country.Cities.Add(new City { Name = model.Name });
            await this.UpdateAsync(country);
        }

        public async Task<int> UpdateCityAsync(City city)
        {
            var country = await this.GetCountryByCity(city);
            if (country == null)
            {
                return 0;
            }

            this.context.Cities.Update(city);
            await this.context.SaveChangesAsync();
            return country.Id;
        }

        public async Task<int> DeleteCityAsync(City city)
        {
            var country = await this.GetCountryByCity(city);
            if (country == null)
            {
                return 0;
            }

            this.context.Cities.Remove(city);
            await this.context.SaveChangesAsync();
            return country.Id;
        }

        private async Task<Country> GetCountryByCity(City city)
        {
            return await this.context.Countries
                .Where(c => c.Cities.Any(ci => ci.Id == city.Id))
                .FirstOrDefaultAsync();
        }
    }
}
