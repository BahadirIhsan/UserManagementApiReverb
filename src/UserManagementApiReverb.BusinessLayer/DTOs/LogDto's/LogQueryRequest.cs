namespace UserManagementApiReverb.BusinessLayer.DTOs.LogDto_s;

public record LogQueryRequest
{
    public string? Level { get; init; }
    public string? EventType { get; init; }
    public string? Operation { get; init; }
    public string? Category { get; init; }
    public int Minutes { get; init; } = 60;
    public int MaxItems { get; init; } = 100;
}