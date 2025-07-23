namespace UserManagementApiReverb.BusinessLayer.DTOs.User;
/// <summary>
/// Represents the response data for a created user.
/// </summary>
public class UserResponse
{
    /// <summary>
    /// Unique identifier of the user.
    /// </summary>
    /// <example>c78b1d34-5653-4f65-845e-d05bcd143d4f</example>
    public Guid UserId { get; set; }

    /// <summary>
    /// The username of the user.
    /// </summary>
    /// <example>Baho</example>
    public string Username { get; set; }

    /// <summary>
    /// The first name of the user.
    /// </summary>
    /// <example>Bahadır</example>
    public string FirstName { get; set; }

    /// <summary>
    /// The last name of the user.
    /// </summary>
    /// <example>Herdem</example>
    public string LastName { get; set; }

    /// <summary>
    /// The email address of the user.
    /// </summary>
    /// <example>bahadir@example.com</example>
    public string Email { get; set; }
}