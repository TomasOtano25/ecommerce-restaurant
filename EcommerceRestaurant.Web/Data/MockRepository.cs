
namespace EcommerceRestaurant.Web.Data
{
    using Entities;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class MockRepository
    {
        public void AddProduct(Product product)
        {
            throw new NotImplementedException();
        }

        public Product GetProduct(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Product> GetProducts()
        {
            var products = new List<Product>();
            products.Add(new Product() { Id = 1, Name = "One", Price = 10, Stock = 10 });
            products.Add(new Product() { Id = 2, Name = "Two", Price = 20, Stock = 10 });
            products.Add(new Product() { Id = 3, Name = "Three", Price = 30, Stock = 10 });
            products.Add(new Product() { Id = 4, Name = "Four", Price = 40, Stock = 10 });
            products.Add(new Product() { Id = 5, Name = "Five", Price = 50, Stock = 10 });

            return products;
        }

        public bool ProductExists(int id)
        {
            throw new NotImplementedException();
        }

        public void RemoveProduct(Product product)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveAllASync()
        {
            throw new NotImplementedException();
        }

        public void UpdateProduct(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
