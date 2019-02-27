namespace EcommerceRestaurant.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Data.Entities;
    using Data.Repositories;
    using Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System;

    public class OrdersController : Controller
    {
        private readonly IOrderRepository orderRepository;
        private readonly IProductRepository productRepository;

        public OrdersController(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            this.orderRepository = orderRepository;
            this.productRepository = productRepository;
        }

        public async Task<IActionResult> Index()
        {
            var model = await this.orderRepository.GetAllAsync(this.User.Identity.Name);
            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var model = await this.orderRepository.GetDetailTempsAsync(this.User.Identity.Name);
            return View(model);
        }

        public IActionResult AddProduct()
        {
            var model = new OrderDetailViewModel
            {
                Quantity = 1,
                Products = this.GetProducts()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(AddItemViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                await this.orderRepository.AddItemToOrderAsync(model, this.User.Identity.Name);
                return this.RedirectToAction("Create");
            }

            return this.View(model);
        }

        public async Task<IActionResult> DeleteItem(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            await this.orderRepository.DeleteDetailTempAsync(id.Value);
            return this.RedirectToAction("Create");
        }

        public async Task<IActionResult> ConfirmOrder()
        {
            var response = await this.orderRepository.ConfirmOrderAsync(this.User.Identity.Name);
            if (response)
            {
                return this.RedirectToAction("Index");
            }

            return this.RedirectToAction("Create");
        }

        public async Task<IActionResult> Deliver(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await this.orderRepository.GetByIdAsync(id.Value);
            if (order == null)
            {
                return NotFound();
            }

            var model = new DeliverViewModel
            {
                Id = order.Id,
                DeliveryDate = order.DeliveryDate != DateTime.MinValue ? order.DeliveryDate : DateTime.Now
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Deliver(DeliverViewModel model)
        {
            if (this.ModelState.IsValid)
            {   
                await this.orderRepository.DeliverOrder(model);
                return this.RedirectToAction("Index");
            }

            return this.View();
        }

        private IEnumerable<SelectListItem> GetProducts()
        {
                var products = this.productRepository.GetAll().ToList();
                products.Insert(0, new Product
                {
                    Id = 0,
                    Name = "(Select a product...)"
                });

                return products.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                }).ToList();
        }
    }
}