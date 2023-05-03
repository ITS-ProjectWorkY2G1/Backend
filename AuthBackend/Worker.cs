using Microsoft.AspNetCore.Identity;
using Models.AuthModels;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AuthBackend
{
    public class Worker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Worker> _logger;
        private async Task InitializeRoles(RoleManager<ApplicationRole> roleManager)
        {
            _logger.LogDebug("Worker - Creating roles");
            using var scope = _serviceProvider.CreateScope();

            if (!await roleManager.RoleExistsAsync("SuperAdmin"))
            {
                ApplicationRole newRole = new ApplicationRole { Id = Guid.NewGuid(), Name = "SuperAdmin" };
                
                await roleManager.CreateAsync(newRole);
            }
        }
        private async Task CreateUsers(string _Role, string _Username, string _Email)
        {
            _logger.LogDebug("Worker - Creating users");
            using var scope = _serviceProvider.CreateScope();

            IdentityResult roleResult;

            var _UserManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var _RoleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            await InitializeRoles(_RoleManager);

            var roleExist = await _RoleManager.RoleExistsAsync(_Role);
            if (!roleExist)
            {
                //create the roles and seed them to the database: Question 1
                ApplicationRole ir = new ApplicationRole { Id = Guid.NewGuid(), Name = _Role };
                roleResult = await _RoleManager.CreateAsync(ir);
            }

            //Here you could create a super user who will maintain the web app
            var poweruser = new ApplicationUser
            {
                UserName = _Username,
                Email = _Email
            };
            //Ensure you have these values in your appsettings.json file
            string userPWD = "Pwdpwd1!";
            var _user = await _UserManager.FindByEmailAsync(_Email);

            if (_user == null)
            {
                var createPowerUser = await _UserManager.CreateAsync(poweruser, userPWD);
                if (createPowerUser.Succeeded)
                {
                    //here we tie the new user to the role
                    await _UserManager.AddToRoleAsync(poweruser, _Role);
                }
            }
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
            var _UserManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var _RoleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            var roles = _RoleManager.Roles.Count();
            var users = _UserManager.Users.Count();

            await FillDb();

            _logger.LogDebug("//////////// Auth worker End ////////////");
        }
        public async Task FillDb()
        {

            using var scope = _serviceProvider.CreateScope();

            await CreateUsers("SuperAdmin", "SuperAdmin", "super@admin.com");

            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            _logger.LogDebug("Creating OIDC applications");
            if (await manager.FindByClientIdAsync("mudblazor-client") == null)
            {
                OpenIddictApplicationDescriptor toCreate = new OpenIddictApplicationDescriptor
                {
                    ClientId = "mudblazorTest",
                    ClientSecret = "bf45541e-66ac-4acb-a495-ee6f96d971ab",
                    ConsentType = ConsentTypes.Explicit,
                    DisplayName = "blazor client",
                    PostLogoutRedirectUris =
                {
                    new Uri("https://plc.admin.softray.it/signout-callback-oidc")
                },
                    RedirectUris =
                {
                    new Uri("https://plc.admin.softray.it/signin-oidc")
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
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Roles
                },
                    Requirements =
                {
                    Requirements.Features.ProofKeyForCodeExchange
                }
                };

                var el = System.Text.Json.JsonSerializer.SerializeToElement(Guid.Parse("8504DB27-96D5-4923-9C75-81D6F43C69A2"));
                toCreate.Properties.Add("ClientID", el);
                //await manager.CreateAsync(toCreate);
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
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Roles
                },
                    Requirements =
                {
                    Requirements.Features.ProofKeyForCodeExchange
                }
                };

                var el = System.Text.Json.JsonSerializer.SerializeToElement<Guid>(Guid.Parse("8504DB27-96D5-4923-9C75-81D6F43C69A2"));
                toCreate.Properties.Add("ClientID", el);
                //await manager.CreateAsync(toCreate);
            }

#if (!DEBUG)
            return;
#endif
        }
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
