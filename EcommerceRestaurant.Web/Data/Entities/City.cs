namespace EcommerceRestaurant.Web.Data.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class City: IEntity
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "City")]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
