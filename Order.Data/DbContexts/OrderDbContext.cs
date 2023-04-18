using Microsoft.EntityFrameworkCore;
using OrderModule.Core.Domain;
using orderEntity = OrderModule.Core.Domain;

namespace OrderModule.Data.DbContexts
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options)
           : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            base.OnConfiguring(dbContextOptionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<orderEntity.Order> Orders { get; set; } = default!;

        public DbSet<OrderDetail> OrderDetails { get; set; } = default!;
    }
}
