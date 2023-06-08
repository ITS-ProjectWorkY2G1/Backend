using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.AuthModels;
using Models.WatchModels;

namespace Models;

public partial class WatchContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Smartwatch> Smartwatch { get; set; }
    public DbSet<Session> Session { get; set; }

    public WatchContext(DbContextOptions<WatchContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("user id=postgres;password=password;host=pwg1db.uksouth.cloudapp.azure.com; database=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
