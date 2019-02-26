namespace EcommerceRestaurant.Web.Data.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Entities;
    using Models;
    using Microsoft.AspNetCore.Mvc.Rendering;
    
    public interface IOrderRepository: IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetAllAsync(string userName);

        Task<IEnumerable<OrderDetailTemp>> GetDetailTempsAsync(string userName);

        IEnumerable<SelectListItem> GetComboProducts();

        Task AddItemToOrderAsync(AddItemViewModel model, string userName);

        Task ModifyOrderDetailTempQuantityAsync(int id, double quantity);

        Task DeleteDetailTempAsync(int id);

        Task<bool> ConfirmOrderAsync(string userName);
    }
}
