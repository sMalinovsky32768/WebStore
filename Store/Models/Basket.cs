using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
        public int GoodCount { get; set; } = 1;
    }
}
