using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace UserManagementApiReverb.BusinessLayer.NotificationServices;

public class SnsService
{
    private readonly IAmazonSimpleNotificationService _snsClient;

    public SnsService(IAmazonSimpleNotificationService snsClient)
    {
        _snsClient = snsClient;
    }

    public async Task<string> CreateTopicAsync(string topicName)
    {
        var response = await _snsClient.CreateTopicAsync(topicName);
        return response.TopicArn;
    }

    public async Task SubscribeEmailAsync(string topicArn, string email)
    {
        var request = new SubscribeRequest
        {
            Protocol = "email",
            TopicArn = topicArn,
            Endpoint = email
        };

        await _snsClient.SubscribeAsync(request);
    }
}