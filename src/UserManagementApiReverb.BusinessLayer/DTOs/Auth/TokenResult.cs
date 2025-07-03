namespace UserManagementApiReverb.BusinessLayer.DTOs.Auth;

public class TokenResult
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}