namespace EcommerceRestaurant.Web.Data.Repositories
{
    using Models;
    using Entities;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface ICountryRepository : IGenericRepository<Country>
    {
        Task<IEnumerable<Country>> GetAllAsync();

        Task<Country> GetAsync(int id);

        Task<Country> GetAsync(City city);

        Task<City> GetCityAsync(int id);

        Task AddCity(CityViewModel model);

        Task<int> UpdateCityAsync(City city);

        Task<int> DeleteCityAsync(City city);

        IEnumerable<SelectListItem> GetComboCountries();

        IEnumerable<SelectListItem> GetComboCities(int countryId);
    }
}
