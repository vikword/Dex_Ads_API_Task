using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dex_Ads_API_Task.Models
{
    public class AddDBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Advertisement> Ads { get; set; }
        public AddDBContext(DbContextOptions<AddDBContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
