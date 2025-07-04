using System.Text;
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using UserManagementApiReverb.BusinessLayer.DTOs.LogDto_s;

namespace UserManagementApiReverb.BusinessLayer.Logging;

public class LogQueryService :  ILogQueryService
{
    private readonly IAmazonCloudWatchLogs _logs;
    private const string LogGroup = "UserManagementApiReverb";

    public LogQueryService(IAmazonCloudWatchLogs logs)
    {
        _logs = logs;
    }
    public async Task<IReadOnlyList<LogEntry>> QueryAsync(LogQueryRequest req, CancellationToken ct)
    {
             /* 3.1 SELECT alanları */
        var q = new StringBuilder(
            "fields @timestamp, Level, Properties.Category, " +     
            "Properties.EventType, Properties.Operation, " +
            "@message, UserId " +
            "| sort @timestamp desc | limit " + req.MaxItems);

        /* 3.2 Dinamik filtre listesi */
        var filters = new List<string>();
        if (req.Level      is not null) filters.Add($"Level = '{req.Level}'");
        if (req.Category   is not null) filters.Add($"Properties.Category = '{req.Category}'"); 
        if (req.EventType  is not null) filters.Add($"Properties.EventType = '{req.EventType}'");
        if (req.Operation  is not null) filters.Add($"Properties.Operation = '{req.Operation}'");

        if (filters.Any())
            q.Insert(0, $"filter {string.Join(" and ", filters)} | ");

        /* 3.3 Query başlatma (değişmedi) */
        var start = DateTimeOffset.UtcNow.AddMinutes(-req.Minutes).ToUnixTimeSeconds();
        var id = (await _logs.StartQueryAsync(new()
        {
            LogGroupNames = [ LogGroup ],
            StartTime     = start,
            EndTime       = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            QueryString   = q.ToString()
        }, ct)).QueryId;

        GetQueryResultsResponse res;
        do {
            await Task.Delay(1000, ct);
            res = await _logs.GetQueryResultsAsync(new() { QueryId = id }, ct);
        } while (res.Status == QueryStatus.Running);

        /* 3.4 Satırları DTO’ya dönüştürme */
        return res.Results.Select(row => new LogEntry(
            Timestamp : DateTime.Parse(row.First(c => c.Field=="@timestamp").Value),
            Level     : row.First(c => c.Field=="Level").Value,
            Category  : row.FirstOrDefault(c => c.Field=="Properties.Category")?.Value ?? "",   
            EventType : row.FirstOrDefault(c => c.Field=="Properties.EventType")?.Value ?? "",
            Operation : row.FirstOrDefault(c => c.Field=="Properties.Operation")?.Value ?? "",
            Message   : row.First(c => c.Field=="@message").Value,
            UserId    : row.FirstOrDefault(c => c.Field=="UserId")?.Value
        )).ToList();
    }
}