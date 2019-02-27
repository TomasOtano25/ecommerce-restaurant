namespace EcommerceRestaurant.Web.Data.Entities
{
    using Microsoft.AspNetCore.Identity;

    public class User: IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public int CityId { get; set; }

        public City City { get; set; }
    }
}
