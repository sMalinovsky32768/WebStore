using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;

namespace Store.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Good> Goods { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Producer> Producers { get; set; }
        public DbSet<GoodType> GoodTypes { get; set; }
        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
        
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            (string adminEmail, string adminPassword) =
                Security.EncryptUserData("admin@mail.ru", "admin");

            var adminRole = new Role
            {
                ID = 1,
                Name = "admin",
            };
            var userRole = new Role
            {
                ID = 2,
                Name = "user",
            };
            var adminUser = new User
            {
                ID = 1,
                Email = adminEmail,
                Password = adminPassword,
                RoleID = adminRole.ID,
            };

            modelBuilder.Entity<Role>().HasData(new Role[]
            {
                adminRole,
                userRole,
            });
            modelBuilder.Entity<User>().HasData(new User[] { adminUser });
            modelBuilder.Entity<DeliveryMethod>().HasData(new DeliveryMethod[]
            {
                new DeliveryMethod
                {
                    ID = 1,
                    Name = "Самовывоз",
                },
                new DeliveryMethod
                {
                    ID = 2,
                    Name = "Почта России",
                },
                new DeliveryMethod
                {
                    ID = 3,
                    Name = "Курьерская доставка",
                },
            });
            base.OnModelCreating(modelBuilder);
        }

        public string BasketString(int uid)
        {
            var builder = new StringBuilder();
            int count = 0;
            decimal value = 0m;
            var baskets = Baskets.Include(b => b.Good).AsEnumerable().Where(b => b.UserID == uid && !GetIsPlaced(b.ID));
            foreach (var item in baskets)
            {
                count += item.GoodCount;
                value += item.Good.Value * item.GoodCount;
            }
            builder.Append($"{count}");
            if (count > 10 && count <= 20)
                builder.Append(" товаров");
            else
                switch (count % 10)
                {
                    case 0:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        builder.Append(" товаров");
                        break;
                    case 1:
                        builder.Append(" товар");
                        break;
                    case 2:
                    case 3:
                    case 4:
                        builder.Append(" товара");
                        break;
                }
            builder.Append($" на сумму {value:# ##0.00} рублей");
            return builder.ToString();
        }

        internal bool GetIsPlaced(int ID)
        {
            Order order = Orders.FirstOrDefault(o => o.BasketID == ID);
            return order != null;
        }
    }
}
