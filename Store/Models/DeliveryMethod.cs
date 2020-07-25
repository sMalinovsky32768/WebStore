using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Store.Models
{
    public class DeliveryMethod
    {
        public int ID { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(3)]
        [DisplayName("Delivery Method")]
        public string Name { get; set; }

        public List<Order> Orders { get; set; }
    }
}
