using ReactShop.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactShop.Core
{
    public class Context : DbContext
    {
        public DbSet<CartItem> CartItem { get; set; }
        public DbSet<Product> Product { get; set; }
    }
}
