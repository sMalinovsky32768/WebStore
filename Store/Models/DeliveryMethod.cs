using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Models
{
    public class DeliveryMethod
    {
        public int ID { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(3)]
        public string Name { get; set; }

        public List<Order> Orders { get; set; }
    }
}
