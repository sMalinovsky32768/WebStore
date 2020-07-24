using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? RoleID { get; set; }
        public Role Role { get; set; }
        public List<Basket> Baskets { get; set; }

        public User()
        {
            Baskets = new List<Basket>();
        }
    }
}
