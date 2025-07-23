namespace UserManagementApiReverb.BusinessLayer.DTOs.User;

/// <summary>
/// Request model for registering a new user.
/// </summary>
public class UserRequestRegister
{
    /// <summary>
    /// The desired username of the user.
    /// </summary>
    /// <example>baho</example>
    public string Username { get; set; }
    /// <summary>
    /// The user's password.
    /// </summary>
    /// <example>Bahadir123.?</example>
    public string Password { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    /// <summary>
    /// The user's email address.
    /// </summary>
    /// <example>bahadir@example.com</example>
    public string Email { get; set; }    
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public DateTime? Birthday { get; set; }
    
}