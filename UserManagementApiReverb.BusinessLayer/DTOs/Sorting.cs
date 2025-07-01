namespace UserManagementApiReverb.BusinessLayer.DTOs;

public record Sorting
{
    public string? sortBy {get; init;} = "createdAt";
    public string? sortDir {get; init;} = "desc";
}