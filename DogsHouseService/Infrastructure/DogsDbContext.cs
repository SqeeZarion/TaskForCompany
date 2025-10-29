using DogsHouseService.Domain;
using Microsoft.EntityFrameworkCore;

namespace DogsHouseService.Infrastructure;

public class DogsDbContext : DbContext
{
    public DbSet<Dog> Dogs => Set<Dog>();

    public DogsDbContext(DbContextOptions<DogsDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dog>(entity =>
        {
            entity.ToTable("dogs");
            entity.HasKey(d => d.Id);
            entity.HasIndex(d => d.Name).IsUnique();

            entity.Property(d => d.Name).IsRequired();
            entity.Property(d => d.Color).IsRequired();
            entity.Property(d => d.TailLength).HasColumnName("tail_length");
            entity.Property(d => d.Weight).HasColumnName("weight");
        });
    }
}