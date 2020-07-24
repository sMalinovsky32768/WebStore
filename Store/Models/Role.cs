using System.Collections.Generic;

namespace Store.Models
{
    public class Role
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; }

        public Role()
        {
            Users = new List<User>();
        }
    }
}
