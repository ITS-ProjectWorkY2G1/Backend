using Models.WatchModels;

namespace Services.Services
{
    public interface ISmartwatchServices
    {
        Task<List<Session>> GetAllSessionsAsync();
        Task<List<Smartwatch>> GetAllSmartwatchesAsync();
        Task<List<Session>> GetSessionsAsync(Guid userId);
        Task<List<Smartwatch>> GetSmartwatchAsync(Guid userId, Guid SessionId);
    }
}