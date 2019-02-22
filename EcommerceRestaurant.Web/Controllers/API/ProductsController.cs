namespace EcommerceRestaurant.Web.Controllers.API
{
    using Data;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using System.Linq;

    [Route("api/[Controller]")]
    public class ProductsController : Controller
    {
        private readonly IProductRepository productRepository;
        private readonly IOptions<MyConfig> config;

        public ProductsController(IProductRepository productRepository, IOptions<MyConfig> config)
        {
            this.productRepository = productRepository;
            this.config = config;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            return Ok(this.productRepository.GetAllWithUser(this.config.Value.ApiUrl).OrderBy(p => p.Name));
        }
    }
}
