using EFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<HotStockDay> HotStockDays { get; set; }
    public DbSet<HotStockItem> HotStockItems { get; set; }
    public DbSet<NotepadCompanyItem> CompanyItems { get; set; }
    public DbSet<CompanySummary> CompanySummaries { get; set; }
    public DbSet<Note> CompanyNotes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HotStockItem>()
             .HasOne(i => i.HotStockDay)
             .WithMany(d => d.HotStockItems)
             .HasForeignKey(i => i.HotStockDayId)
             .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<NotepadCompanyItem>()
            .HasOne(i => i.Summary)
            .WithOne(s => s.NotepadCompanyItem)
            .HasForeignKey<CompanySummary>(s => s.NotepadCompanyItemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<NotepadCompanyItem>()
            .HasMany(i => i.Notes)
            .WithOne(n => n.NotepadCompanyItem)
            .HasForeignKey(n => n.NotepadCompanyItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
