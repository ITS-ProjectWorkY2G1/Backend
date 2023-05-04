using Models.AuthModels;

namespace Services.Interfaces
{
    public interface IAccountService
    {
        Task RegisterAsync(RegisterModel registerModel);
    }
}