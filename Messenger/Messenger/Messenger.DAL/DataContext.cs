using Microsoft.EntityFrameworkCore;
using Messenger.Models.DB;
using System.Configuration;

namespace Messenger.DAL;

public class DataContext : DbContext
{
    public DbSet<Server> Server { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<Chat> Chat { get; set; }

    //public DataContext(DbContextOptions options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string? connectionString = ConfigurationManager.ConnectionStrings["LocalConnection"].ConnectionString;
        if (connectionString is null)
            return;
        optionsBuilder.UseSqlServer(connectionString);

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(prop => prop.Nickname)
            .IsUnique();

        modelBuilder.Entity<Chat>()
            .HasOne(prop => prop.Sender)
            .WithMany(prop => prop.SentChat)
            .HasForeignKey(prop => prop.SenderId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Chat>()
            .HasOne(prop => prop.Recipient)
            .WithMany(prop => prop.ReceivedChat)
            .HasForeignKey(prop => prop.RecipientId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Server>()
            .HasIndex(prop => prop.NameServer)
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }

    public void ReseedIdentity<T>() where T : class =>
        this.Database.ExecuteSqlRaw($"DBCC CHECKIDENT ('{typeof(T).Name}', RESEED, 0)");

    public int Clear<T>() where T : class =>
        this.Database.ExecuteSqlRaw($"delete from [{typeof(T).Name}]; DBCC CHECKIDENT ('{typeof(T).Name}', RESEED, 0)");
}
