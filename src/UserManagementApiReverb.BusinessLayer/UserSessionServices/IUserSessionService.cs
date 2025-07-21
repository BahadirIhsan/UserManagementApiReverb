using System;
using System.Threading.Tasks;
using UserManagementApiReverb.Entities.Entities;

namespace UserManagementApiReverb.BusinessLayer.Services.Abstract
{
    public interface IUserSessionService
    {
        Task<UserSession> CreateSessionAsync(Guid userId, string accessToken);
        Task<bool> DeleteSessionAsync(string accessToken);
        Task<UserSession?> GetSessionByTokenAsync(string accessToken);
    }
}