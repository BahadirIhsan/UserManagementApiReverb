using Microsoft.EntityFrameworkCore;
using UserManagementApiReverb.BusinessLayer.CloudWatchMetricsService;
using UserManagementApiReverb.BusinessLayer.DTOs.Auth;
using UserManagementApiReverb.BusinessLayer.Logging;
using UserManagementApiReverb.BusinessLayer.Services.Abstract;
using UserManagementApiReverb.DataAccessLayer;

namespace UserManagementApiReverb.BusinessLayer.AuthServices;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly ITokenService _token;
    private readonly IAppLogger _logger;
    private readonly IUserSessionService _userSessionService;
    private readonly ICloudWatchMetricsService _cloudWatchMetricsService;
    
    public AuthService(AppDbContext db, ITokenService token, IAppLogger logger, IUserSessionService userSessionService, ICloudWatchMetricsService cloudWatchMetricsService)
    {
        _db = db;
        _token = token;
        _logger = logger;
        _userSessionService = userSessionService;
        _cloudWatchMetricsService = cloudWatchMetricsService;
    }
    
    public async Task<LoginResponse> LoginUserAsync(LoginRequest req)
    {
        var user = await _db.Users.Include(u => u.UserRoles)
                            .ThenInclude(u => u.Role)
                            .FirstOrDefaultAsync(u => u.Email == req.Email);
        
        if (user == null)
        {
            _logger.LogWarn("Login attempt failed: user not found", LogCategories.Security, new {req.Email});
            await _cloudWatchMetricsService.SendFailedLoginMetricAsync("/login", req.Email); // METRİK GÖNDERİMİ
            Console.WriteLine("**************************************");
            throw new UnauthorizedAccessException("User not found");
        }

        bool isValid = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);
        if (!isValid)
        {
            _logger.LogWarn("Incorrect password attempt", LogCategories.Security, new {req.Email});
            return null;
        }
        
        // token üeritliyor 
        var tokenResult = _token.GenerateToken(user);
        // session kayıt işlemi
        var session = await _userSessionService.CreateSessionAsync(user.UserId, tokenResult.AccessToken);
        
        
        _logger.LogInfo("User logged in and session created", LogCategories.Security, new {req.Email});
        
        // BU KISMI İMPLEMENTE EDEREK DEVAMINDA BU BLOĞU AÇMAMIZ LAZIM.
        /*
        // 2. Session oluştur
        var session = await _userSessionService.CreateSessionAsync(user.Id, accessToken);

        // 3. Audit Log kaydı yap (session kaydı için)
        await _auditLogger.LogChange(
            userId: user.Id,
            table: "UserSessions",
            oldValues: null,
            newValues: session,
            action: "Create"
        );
        */
        return new LoginResponse
        {
            AccessToken = tokenResult.AccessToken,
            ExpiresAt = tokenResult.ExpiresAt
        };
    }
}