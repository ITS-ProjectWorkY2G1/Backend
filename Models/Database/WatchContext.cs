using Microsoft.EntityFrameworkCore;
using Models.WatchModels;

namespace Models.Database;

public partial class WatchContext : DbContext
{
    public DbSet<Smartwatch> Smartwatch { get; set; }
    public DbSet<Session> Session { get; set; }

    public WatchContext(DbContextOptions<WatchContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       modelBuilder.Entity<Smartwatch>().HasKey(x => new { x.SessionId, x.Id, x.UserId, x.HeartRate, x.Position, x.Timestamp});
    }
}
