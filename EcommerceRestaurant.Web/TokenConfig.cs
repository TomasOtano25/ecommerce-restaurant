using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceRestaurant.Web
{
    public class TokenConfig
    {
        public string Key { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }
    }
}
