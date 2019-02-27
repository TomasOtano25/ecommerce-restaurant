namespace EcommerceRestaurant.Web.Controllers
{
    using System.Threading.Tasks;
    using EcommerceRestaurant.Web.Data.Entities;
    using EcommerceRestaurant.Web.Data.Repositories;
    using EcommerceRestaurant.Web.Models;
    using Microsoft.AspNetCore.Mvc;

    public class CountriesController : Controller
    {
        private readonly ICountryRepository countryRespository;

        public CountriesController(ICountryRepository countryRespository)
        {
            this.countryRespository = countryRespository;
        }

        public async Task<IActionResult> Index()
        {
            return View(await this.countryRespository.GetAllAsync());
        }

        public async Task<IActionResult> DeleteCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await this.countryRespository.GetCityAsync(id.Value);
            if (city == null)
            {
                return NotFound();
            }

            var countryId = await this.countryRespository.DeleteCityAsync(city);
            return this.RedirectToAction($"Details/{countryId}");
        }

        public async Task<IActionResult> EditCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await this.countryRespository.GetCityAsync(id.Value);
            if (city == null)
            {
                return NotFound();
            }
            return View(city);
        }

        [HttpPost]
        public async Task<IActionResult> EditCity(City city)
        {
            if (this.ModelState.IsValid)
            {
                var countryId = await this.countryRespository.UpdateCityAsync(city);
                if (countryId != 0)
                {
                    return this.RedirectToAction($"Details/{countryId}");
                }
            }

            return View(city);
        }

        public async Task<IActionResult> AddCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await this.countryRespository.GetAsync(id.Value);
            if (country == null)
            {
                return NotFound();
            }

            var model = new CityViewModel
            {
                CountryId = country.Id
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddCity(CityViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                await this.countryRespository.AddCity(model);
                return this.RedirectToAction($"Details/{model.CountryId}");
            }

            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await this.countryRespository.GetAsync(id.Value);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Country country)
        {
            if (this.ModelState.IsValid)
            {
                await this.countryRespository.CreateAsync(country);
                return this.RedirectToAction(nameof(Index));
            }

            return View(country);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await this.countryRespository.GetAsync(id.Value);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Country country)
        {
            if(this.ModelState.IsValid)
            {
                await this.countryRespository.UpdateAsync(country);
                return this.RedirectToAction(nameof(Index));
            }

            return View(country);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await this.countryRespository.GetAsync(id.Value);
            if (country == null)
            {
                return NotFound();
            }

            await this.countryRespository.DeleteAsync(country);
            return RedirectToAction(nameof(Index));
        }
    }
}