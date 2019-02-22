namespace EcommerceRestaurant.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RemenberMe { get; set; }
    }
}
