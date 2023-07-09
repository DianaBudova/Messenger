using Microsoft.EntityFrameworkCore;
using Messenger.Models.DB;

namespace Messenger.DAL;

public class DataContext : DbContext
{
    public DbSet<User> User { get; set; }
    public DbSet<Endpoint> Endpoint { get; set; }
    public DbSet<Chat> Chat { get; set; }

    public DataContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(prop => prop.Nickname)
            .IsUnique();
    }
}
