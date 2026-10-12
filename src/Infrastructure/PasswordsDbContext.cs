using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public sealed class PasswordsDbContext(DbContextOptions<PasswordsDbContext> options) : DbContext(options)
{
    public DbSet<PasswordRecord> PasswordRecords => Set<PasswordRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var passwordRecord = modelBuilder.Entity<PasswordRecord>();
        passwordRecord.ToTable("PasswordRecords");
        passwordRecord.HasKey(record => record.Id);
        passwordRecord.Property(record => record.Value).IsRequired();
        passwordRecord.Property(record => record.CreatedAt).IsRequired();
    }
}
