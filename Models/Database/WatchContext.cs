using System;
using System.Collections.Generic;
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
}
