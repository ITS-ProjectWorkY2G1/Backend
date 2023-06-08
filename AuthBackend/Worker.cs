using Microsoft.AspNetCore.Identity;
using Models;
using Models.AuthModels;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static System.Formats.Asn1.AsnWriter;

namespace AuthBackend
{
    public class Worker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Worker> _logger;
        private async Task CreateUsers(string _Username, string _Email, IServiceScope scope)
        {
            _logger.LogDebug("Worker - Creating users");

            //Here you could create a super user who will maintain the web app
            var poweruser = new ApplicationUser
            {
                UserName = _Username,
                Email = _Email
            };
            //Ensure you have these values in your appsettings.json file
            string userPWD = "Pwdpwd1!";
            var _usermanager = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<ApplicationUser>>();
            await _usermanager.CreateAsync(poweruser, userPWD);
        }
        public Worker(IServiceProvider serviceProvider, ILogger<Worker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {            
            _logger.LogDebug("//////////// Auth worker start ////////////");
            using var scope = _serviceProvider.CreateScope();

            var dbcontext = scope.ServiceProvider.GetRequiredService<WatchContext>();

            if (dbcontext.Users.Count() == 0) await FillDb(scope);

            _logger.LogDebug("//////////// Auth worker End ////////////");
        }
        public async Task FillDb(IServiceScope scope)
        {

            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            await CreateUsers("Admin", "admin@admin.com", scope);

            _logger.LogDebug("Creating OIDC applications");
            if (await manager.FindByClientIdAsync("mudblazorTest") == null)
            {
                OpenIddictApplicationDescriptor toCreate = new OpenIddictApplicationDescriptor
                {
                    ClientId = "mudblazorTest",
                    ClientSecret = "bf45541e-66ac-4acb-a495-ee6f96d971ab",
                    ConsentType = ConsentTypes.Explicit,
                    DisplayName = "blazor client",
                    PostLogoutRedirectUris =
                {
                    new Uri("https://localhost:5001/signout-callback-oidc")
                },
                    RedirectUris =
                {
                    new Uri("https://localhost:5001/signin-oidc")
                },
                    Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Logout,
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.RefreshToken,
                    Permissions.ResponseTypes.Code,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile
                },
                    Requirements =
                {
                    Requirements.Features.ProofKeyForCodeExchange
                }
                };

                var el = System.Text.Json.JsonSerializer.SerializeToElement(Guid.Parse("8504DB27-96D5-4923-9C75-81D6F43C69A2"));
                toCreate.Properties.Add("ClientID", el);
                await manager.CreateAsync(toCreate);
            }

            if (await manager.FindByClientIdAsync("test-postman") == null)
            {
                OpenIddictApplicationDescriptor toCreate = new OpenIddictApplicationDescriptor
                {
                    ClientId = "test-postman",
                    ClientSecret = "postman-secret",
                    ConsentType = ConsentTypes.Explicit,
                    DisplayName = "test postman",
                    RedirectUris =
                {
                    new Uri("https://oauth.pstmn.io/v1/callback")
                },
                    Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Logout,
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.RefreshToken,
                    Permissions.ResponseTypes.Code,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile
                },
                    Requirements =
                {
                    Requirements.Features.ProofKeyForCodeExchange
                }
                };

                var el = System.Text.Json.JsonSerializer.SerializeToElement<Guid>(Guid.Parse("8504DB27-96D5-4923-9C75-81D6F43C69A2"));
                toCreate.Properties.Add("ClientID", el);
                await manager.CreateAsync(toCreate);
            }

#if (!DEBUG)
            return;
#endif
        }
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
