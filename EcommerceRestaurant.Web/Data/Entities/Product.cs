namespace EcommerceRestaurant.Web.Data.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Product : IEntity
    {
        public int Id { get; set; }

        [MaxLength(128, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        [Required]
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }

        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        [Display(Name = "Last Purchase")]
        public DateTime? LastPurchase { get; set; }

        [Display(Name = "Last Sale")]
        public DateTime? LastSale { get; set; }

        [Display(Name = "Is Availabe")]
        public bool IsAvailabe { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public double Stock { get; set; }

        public User User { get; set; }

        // only read
        [NotMapped]
        public string ImageFullPath
        {
            get;    
            
                /*if (string.IsNullOrEmpty(this.ImageUrl))
                {
                    return null;
                }

                return $"https://ecommercerestaurantweb20190221110133.azurewebsites.net{this.ImageUrl}";*/
            
            set;
        }
    }
}
