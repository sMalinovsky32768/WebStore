using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Models
{
    public class Producer 
    {
        public int ID { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(3)]
        [DisplayName("Producer")]
        public string Name { get; set; }

        [Required]
        [MaxLength(10)]
        [MinLength(3)]
        public string Code { get; set; }

        public List<Good> Goods { get; set; }

        public Producer()
        {
            Goods = new List<Good>();
        }
    }
}
