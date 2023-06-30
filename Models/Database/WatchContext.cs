using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.AuthModels;
using Models.WatchModels;

namespace Models;

public partial class WatchContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Smartwatch> Smartwatch { get; set; }
    public DbSet<Session> Session { get; set; }
    private IConfiguration _conf { get; set; }
    public WatchContext(IConfiguration conf, DbContextOptions<WatchContext> options)
        : base(options)
    {
        _conf = conf;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql(_conf.GetConnectionString("db"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Smartwatch>()
            .HasKey(c => new { c.Id, c.Timestamp });
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
