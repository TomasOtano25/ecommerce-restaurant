using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EcommerceRestaurant.Web.Tests.Controllers.API
{
    public class ProductsControllerIntegrationTests: IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public ProductsControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory)
        {
            this._client = factory.CreateClient();
        }

        [Fact]
        public async Task CanGetProducts()
        {
            var httpResponse = await _client.GetAsync("/api/Products");

            httpResponse.EnsureSuccessStatusCode();
        }
    }
}
