using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Store.Models
{
    public class GoodType
    {
        public int ID { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(3)]
        [DisplayName("Type")]
        public string Name { get; set; }

        [Required]
        [MaxLength(10)]
        [MinLength(3)]
        public string Code { get; set; }

        public List<Good> Goods { get; set; }

        public GoodType()
        {
            Goods = new List<Good>();
        }
    }
}
