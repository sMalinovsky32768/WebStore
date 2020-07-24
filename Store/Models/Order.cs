using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Models
{
    public class Order
    {
        public int ID { get; set; }

        [Required]
        public int BasketID { get; set; }

        [Required]
        public Basket Basket { get; set; }

        [Required]
        public int DeliveryMethodID { get; set; }

        [Required]
        public DeliveryMethod DeliveryMethod { get; set; }
    }
}
