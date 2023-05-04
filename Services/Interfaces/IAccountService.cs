using Models.AuthModels;

namespace Services.Interfaces
{
    public interface IAccountService
    {
        Task Register(RegisterModel registerModel);
    }
}