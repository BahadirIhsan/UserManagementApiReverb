namespace UserManagementApiReverb.BusinessLayer.DTOs.User;

public record UserFilter
{
    public string? Username {get; init;}
    public string? Email {get; init;}
    public string? FirstName {get; init;}
    public string? LastName {get; init;}
    public string? PhoneNumber {get; init;}
    public string? Address {get; init;}

}