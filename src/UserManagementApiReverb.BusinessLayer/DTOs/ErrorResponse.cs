namespace UserManagementApiReverb.BusinessLayer.DTOs;

/// <summary>
/// Standart format for Api error responses
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// the http status code of the error
    /// </summary>
    /// <example>400 Bad Request</example>
    public int StatusCode { get; set; }
    
    /// <summary>
    /// a short explanation of the error
    /// </summary>
    /// <example>Invalid input: email is required</example>
    public string Message { get; set; }
}