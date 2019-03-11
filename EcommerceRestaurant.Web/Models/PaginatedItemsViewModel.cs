namespace EcommerceRestaurant.Web.Models
{
    using System.Collections.Generic;

    public class PaginatedItemsViewModel<TEntity> where TEntity : class
    {
        public int PageIndex { get; private set; }

        public int PageSize { get; private set; }

        public long Count { get; private set; }

        public IEnumerable<TEntity> Data { get; set; }

        public PaginatedItemsViewModel(int pageIndex, int PageSize, long count, IEnumerable<TEntity> data)
        {
            this.PageIndex = pageIndex;
            this.PageSize = PageSize;
            this.Count = count;
            this.Data = data;
        }
    }
}
