using Microsoft.AspNetCore.Identity;
using AuthBackend;
using Models.Database;
using static OpenIddict.Abstractions.OpenIddictConstants;
using MongoDB.Driver;
using Microsoft.IdentityModel.Tokens;
using Models.AuthModels;
using Microsoft.AspNetCore.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using log4net;

var builder = WebApplication.CreateBuilder(args);


var config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false)
        .Build();

builder.Services.Configure<DatabaseSettings>("IdentityDatabase",
    builder.Configuration.GetSection("IdentityDatabase"));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
        .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>
        (
            config["IdentityDatabase:ConnectionString"], "Auth"
        );

builder.Services.AddControllersWithViews()
    .AddJsonOptions(
        options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Logging.AddLog4Net("log4net.config");

builder.Services.Configure<IdentityOptions>(options =>
{
    // Configure Identity to use the same JWT claims as OpenIddict instead
    // of the legacy WS-Federation claims it uses by default (ClaimTypes),
    // which saves you from doing the mapping in your authorization controller.
    options.ClaimsIdentity.UserNameClaimType = Claims.Name;
    options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
    options.ClaimsIdentity.RoleClaimType = Claims.Role;
    options.ClaimsIdentity.EmailClaimType = Claims.Email;

    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
});

builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseMongoDb().UseDatabase(new MongoClient(config["IdentityDatabase:ConnectionString"]).GetDatabase("Auth"));        
    })

    .AddServer(options =>
    {

        // Enable the authorization, introspection and token endpoints.
        options.SetAuthorizationEndpointUris("/connect/authorize")
                .SetLogoutEndpointUris("/connect/logout")
                .SetTokenEndpointUris("/connect/token")
                .SetUserinfoEndpointUris("/connect/userinfo");

        // Mark the "email", "profile" and "roles" scopes as supported scopes.
        options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles);

        // Note: this sample only uses the authorization code flow but you can enable
        // the other flows if you need to support implicit, password or client credentials.
        options.AllowAuthorizationCodeFlow()/*.RequireProofKeyForCodeExchange()*/;
        options.AllowClientCredentialsFlow();

        options.SetAccessTokenLifetime(TimeSpan.FromDays(1));

        options.AddEncryptionKey(new SymmetricSecurityKey(
            Convert.FromBase64String("DTjd/GnduI1Kfzen8V8BvaNUfb/VKgXltV7Kbk8iNkY=")));

#if (DEBUG)

        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();

#else
        options.AddEncryptionCertificate(builder.Configuration["certificate:encryptionthumbprint"]!, StoreName.My, StoreLocation.LocalMachine)
               .AddSigningCertificate(builder.Configuration["certificate:signingthumbprint"]!, StoreName.My, StoreLocation.LocalMachine);
#endif



        // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
        //
        // Note: unlike other samples, this sample doesn't use token endpoint pass-through
        // to handle token requests in a custom MVC action. As such, the token requests
        // will be automatically handled by OpenIddict, that will reuse the identity
        // resolved from the authorization code to produce access and identity tokens.
        //
        options.UseAspNetCore()
                    .EnableAuthorizationEndpointPassthrough()
                    .EnableLogoutEndpointPassthrough()
                    .EnableTokenEndpointPassthrough()
                    .EnableUserinfoEndpointPassthrough()
                    .EnableStatusCodePagesIntegration();
    })

    // Register the OpenIddict validation components.
    .AddValidation(options =>
    {
        // Import the configuration from the local OpenIddict server instance.
        options.UseLocalServer();

        // Register the ASP.NET Core host.
        options.UseAspNetCore();
    });

builder.Services.AddAuthorization();

builder.Services.AddHostedService<Worker>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()!.DeclaringType);

app.UseExceptionHandler(exceptionHandlerApp =>
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        context.Response.ContentType = Text.Plain;

        log.Fatal(context.Features.Get<IExceptionHandlerFeature>()!.Error.Message);
    });
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
});

app.MapControllers();

app.UseExceptionHandler("/Error");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();