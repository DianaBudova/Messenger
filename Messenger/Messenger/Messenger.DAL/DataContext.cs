using Microsoft.EntityFrameworkCore;
using Messenger.Models.DB;

namespace Messenger.DAL;

public class DataContext : DbContext
{
    public DbSet<Server> Server { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<Chat> Chat { get; set; }

    public DataContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(prop => prop.Nickname)
            .IsUnique();

        modelBuilder.Entity<Server>()
            .HasIndex(prop => prop.NameServer)
            .IsUnique();
    }

    public void ReseedIdentity<T>() where T : class =>
        this.Database.ExecuteSqlRaw($"DBCC CHECKIDENT ('{typeof(T).Name}', RESEED, 0)");

    public int Clear<T>() where T : class =>
        this.Database.ExecuteSqlRaw($"delete from [{typeof(T).Name}]; DBCC CHECKIDENT ('{typeof(T).Name}', RESEED, 0)");
}
