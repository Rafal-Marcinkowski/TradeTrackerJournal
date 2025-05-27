using EFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<HotStockDay> HotStockDays { get; set; }
    public DbSet<HotStockItem> HotStockItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HotStockDay>()
         .HasMany(d => d.HotStockItems)
         .WithOne(i => i.HotStockDay)
         .HasForeignKey(i => i.HotStockDayId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}
