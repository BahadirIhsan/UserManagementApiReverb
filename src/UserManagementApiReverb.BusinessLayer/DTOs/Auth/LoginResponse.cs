namespace UserManagementApiReverb.BusinessLayer.DTOs.Auth;

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    
    // public string RefreshToken { get; set; } = string.Empty;
}