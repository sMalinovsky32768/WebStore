using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Models
{
    public class Good
    {
        [Required]
        public int ID { get; set; }

        [Required]
        [MaxLength(100)]
        [MinLength(5)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        [MinLength(5)]
        public string Articul { get; set; }

        [Required]
        public decimal Value { get; set; }
        public int ProducerID { get; set; }
        public int GoodTypeID { get; set; }
        public Producer Producer { get; set; }
        public GoodType GoodType { get; set; }
        public List<Basket> Baskets { get; set; }

        public Good()
        {
            Baskets = new List<Basket>();
        }
    }
}
