using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.AuthModels;
using Models.Database;
using Services.Interfaces;
using Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
        .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>
        (
            "mongodb://localhost:27017", "Auth"
        ).AddDefaultTokenProviders();

builder.Services.AddDbContext<WatchContext>(opt =>
{
    opt.UseNpgsql("user id=postgres;password=password;host=localhost;database=postgres");
});

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ISmartwatchServices, SmartwatchServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
