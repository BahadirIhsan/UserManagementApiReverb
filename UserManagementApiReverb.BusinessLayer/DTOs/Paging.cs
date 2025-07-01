namespace UserManagementApiReverb.BusinessLayer.DTOs;

public record Paging
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}