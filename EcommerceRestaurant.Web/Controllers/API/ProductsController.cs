namespace EcommerceRestaurant.Web.Controllers.API
{
    using Data;
    using Data.Entities;
    using EcommerceRestaurant.Web.Helpers;
    using EcommerceRestaurant.Web.Models;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using System.Linq;
    using System.Threading.Tasks;

    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductsController : Controller
    {
        private readonly IProductRepository productRepository;
        private readonly IUserHelper userHelper;
        private readonly IOptions<MyConfig> config;

        private const int SIZE_PAGE_DEFAULT = 10;
        private const int PAGE_INDEX_DEFAULT = 0;

        public ProductsController(IProductRepository productRepository, IUserHelper userHelper, IOptions<MyConfig> config)
        {
            this.productRepository = productRepository;
            this.userHelper = userHelper;
            this.config = config;
        }

        // Get api/products?pageSize=5&pageIndex=1
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] int pageSize = SIZE_PAGE_DEFAULT, [FromQuery] int pageIndex = PAGE_INDEX_DEFAULT)
        {
            var totalProducts = await this.productRepository.LongCountAync();

            var productsOnPage = this.productRepository
                .GetAllWithUser(this.config.Value.ApiUrl)
                .OrderBy(p => p.Name)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToList();

            var model = new PaginatedItemsViewModel<Product>(pageIndex, pageSize, totalProducts, productsOnPage);

            // return Ok(this.productRepository.GetAllWithUser(this.config.Value.ApiUrl).OrderBy(p => p.Name));
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody] Common.Models.Product product)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(ModelState);
            }

            var user = await this.userHelper.GetUserByEmailAsync(product.User.Email);
            if (user == null)
            {
                return this.BadRequest("Invalid user");
            }

            //TODO: Upload image
            var entityProduct = new Product
            {
                IsAvailabe = product.IsAvailabe,
                LastPurchase = product.LastPurchase,
                LastSale = product.LastSale,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                User = user
            };

            var newProduct = await this.productRepository.CreateAsync(entityProduct);
            return Ok(newProduct);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct([FromRoute] int id, [FromBody] Common.Models.Product product)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(ModelState);
            }

            if (id != product.Id)
            {
                return this.BadRequest();
            }

            var oldProduct = await this.productRepository.GetByIdAsync(id);
            if (oldProduct == null)
            {
                return this.BadRequest("Product Id don't exists.");
            }

            //TODO: Upload image
            oldProduct.IsAvailabe = product.IsAvailabe;
            oldProduct.LastPurchase = product.LastPurchase;
            oldProduct.LastSale = product.LastSale;
            oldProduct.Name = product.Name;
            oldProduct.Price = product.Price;
            oldProduct.Stock = product.Stock;

            var updatedProduct = await this.productRepository.UpdateAsync(oldProduct);
            return Ok(updatedProduct);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(ModelState);
            }

            var product = await this.productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return this.NotFound();
            }

            await this.productRepository.DeleteAsync(product);
            return Ok(product);
        }
    }
}
