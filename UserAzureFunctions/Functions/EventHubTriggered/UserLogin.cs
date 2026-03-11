using Azure.Messaging.EventHubs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace UserAzureFunctions.Functions.EventHubTriggered;
public class UserLogin
{
    private readonly ILogger<UserLogin> _logger;

    public UserLogin(ILogger<UserLogin> logger)
    {
        _logger = logger;
    }

    // [Function(nameof(UserLogin))]
    public void Run([EventHubTrigger("login-event", Connection = "User_Login_Event_Conn_Str")] EventData[] events)
    {
        foreach (EventData @event in events)
        {
            _logger.LogCritical("Event Body: {body}", @event.Body);
            _logger.LogCritical("Event Content-Type: {contentType}", @event.ContentType);
        }
    }
}