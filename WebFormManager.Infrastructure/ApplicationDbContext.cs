using Microsoft.EntityFrameworkCore;
using WebFormManager.Domain.Entities;

namespace WebFormManager.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    public DbSet<Submission> Submissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Submission>()
            .Property(s => s.Data)
            .HasColumnType("jsonb");
    }
}