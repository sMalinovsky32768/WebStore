using System;
using System.ComponentModel.DataAnnotations;

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

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Date { get; set; }
    }
}
