using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserManagementApiReverb.BusinessLayer.Services.Abstract;
using UserManagementApiReverb.DataAccessLayer;
using UserManagementApiReverb.Entities.Entities;

namespace UserManagementApiReverb.BusinessLayer.Services.Concrete
{
    public class UserSessionService : IUserSessionService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<UserSessionService> _logger;

        public UserSessionService(AppDbContext db, ILogger<UserSessionService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<UserSession> CreateSessionAsync(Guid userId, string accessToken)
        {
            var session = new UserSession
            {
                UserId = userId,
                AccessToken = accessToken,
                RefreshToken = "", // Eğer yoksa boş geçilir
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(1) // Token süresine göre ayarlanabilir
            };

            _db.UserSessions.Add(session);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Session created for User {UserId}", userId);
            return session;
        }

        public async Task<bool> DeleteSessionAsync(string accessToken)
        {
            var session = await _db.UserSessions.FirstOrDefaultAsync(s => s.AccessToken == accessToken);
            if (session == null)
            {
                _logger.LogWarning("No session found for token: {Token}", accessToken);
                return false;
            }

            _db.UserSessions.Remove(session);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Session deleted for User {UserId}", session.UserId);
            return true;
        }

        public async Task<UserSession?> GetSessionByTokenAsync(string accessToken)
        {
            return await _db.UserSessions
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.AccessToken == accessToken);
        }
    }
}
