using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Store.Models
{
    public class Basket
    {
        public bool isSelected;

        public int ID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public User User { get; set; }

        [Required]
        public int GoodID { get; set; }

        [Required]
        public Good Good { get; set; }

        [Required]
        [DisplayName("Count")]
        public int GoodCount { get; set; } = 1;
    }
}
