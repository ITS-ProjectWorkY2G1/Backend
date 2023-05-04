using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Models.AuthModels;
using Services.Interfaces;

namespace Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public AccountService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task RegisterAsync(RegisterModel registerModel)
        {
            var user = await _userManager.FindByEmailAsync(registerModel.Email);
            if (user != null)
            {
                throw new ArgumentException("User email already taken");
            }

            var newuser = new ApplicationUser { UserName = registerModel.Username, Id = Guid.NewGuid(), Email = registerModel.Email };

            var newuserResponse = await _userManager.CreateAsync(newuser, registerModel.Password);
            if (!newuserResponse.Succeeded)
            {
                IEnumerable<IdentityError> errors = newuserResponse.Errors;
                throw new ArgumentException(errors.First().Description);
            }
        }
    }
}
