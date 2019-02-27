namespace EcommerceRestaurant.Web.Data.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Entities;
    using Helpers;
    using Models;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using System;

    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly DataContext context;
        private readonly IUserHelper userHelper;

        public OrderRepository(DataContext context, IUserHelper userHelper) : base(context)
        {
            this.context = context;
            this.userHelper = userHelper;
        }

        // GetOrdersAsync
        public async Task<IEnumerable<Order>> GetAllAsync(string userName)
        {
            var user = await this.userHelper.GetUserByNameAsync(userName);
            if (user == null)
            {
                return null;
            }

            var orders = this.context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.User == user)
                .OrderBy(o => o.OrderDate);

            return orders;
        }

        public async Task<IEnumerable<OrderDetailTemp>> GetDetailTempsAsync(string userName)
        {
            var user = await this.userHelper.GetUserByNameAsync(userName);
            if (user == null)
            {
                return null;
            }

            var orderDetailTemps = this.context.OrderDetailTemps
                .Include(odt => odt.Product)
                .Where(odt => odt.User == user)
                .OrderBy(odt => odt.Product.Name);

            return orderDetailTemps;
        }

        public IEnumerable<SelectListItem> GetComboProducts()
        {
            var list = this.context.Products.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a product...)",
                Value = "0"
            });

            return list;
        }

        //TODO: Refactorizar
        public async Task AddItemToOrderAsync(AddItemViewModel model, string userName)
        {
            var user = await this.userHelper.GetUserByNameAsync(userName);
            if (user == null)
            {
                return;
            }

            var product = await this.context.Products.FindAsync(model.ProductId);
            if (product == null)
            {
                return;
            }

            var orderDetailTemp = await this.context.OrderDetailTemps
               .Where(odt => odt.User == user && odt.Product == product)
               .FirstOrDefaultAsync();

            if (orderDetailTemp == null)
            {
                orderDetailTemp = new OrderDetailTemp
                {
                    Price = product.Price,
                    Product = product,
                    Quantity = model.Quantity,
                    User = user
                };

                this.context.OrderDetailTemps.Add(orderDetailTemp);
            }
            else
            {
                orderDetailTemp.Quantity += model.Quantity;
                this.context.OrderDetailTemps.Update(orderDetailTemp);
            }

            await this.context.SaveChangesAsync();
        }

        //TODO: Revisar la funcionalidad de eliminacion
        public async Task ModifyOrderDetailTempQuantityAsync(int id, double quantity)
        {
            var orderDetailTemp = await this.context.OrderDetailTemps.FindAsync(id);
            if (orderDetailTemp == null)
            {
                return;
            }

            orderDetailTemp.Quantity += quantity;
            if (orderDetailTemp.Quantity > 0)
            {
                this.context.OrderDetailTemps.Update(orderDetailTemp);

            }
            else if (orderDetailTemp.Quantity == 0) {
                this.context.OrderDetailTemps.Remove(orderDetailTemp);
            }
            await this.context.SaveChangesAsync();
        }

        public async Task DeleteDetailTempAsync(int id)
        {
            var orderDetailTemp = await this.context.OrderDetailTemps.FindAsync(id);

            if (orderDetailTemp == null)
            {
                return;
            }

            this.context.OrderDetailTemps.Remove(orderDetailTemp);

            await this.context.SaveChangesAsync();
        }

        public async Task<bool> ConfirmOrderAsync(string userName)
        {
            var user = await this.userHelper.GetUserByNameAsync(userName);
            if (user == null)
            {
                return false;
            }

            var orderDetailTemps = await this.context.OrderDetailTemps
                .Include(odt => odt.Product)
                .Where(odt => odt.User == user)
                .ToListAsync();

            if (orderDetailTemps == null || orderDetailTemps.Count() == 0)
            {
                return false;
            }

            var details = orderDetailTemps.Select(o => new OrderDetail
            {
                Price = o.Price,
                Product = o.Product,
                Quantity = o.Quantity
            }).ToList();

            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                User = user,
                Items = details
            };

            this.context.Orders.Add(order);
            this.context.OrderDetailTemps.RemoveRange(orderDetailTemps);
            await this.context.SaveChangesAsync();
            return true;
        }

        public async Task DeliverOrder(DeliverViewModel model)
        {
            var order = await this.GetByIdAsync(model.Id);
            if (order == null)
            {
                return;
            }

            order.DeliveryDate = model.DeliveryDate;
            await this.UpdateAsync(order);
        }
    }
}
