using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;

namespace UserAzureFunctions.OutputBinding.MultiDestination;

public class MultiDestinationAndClientWithSdk
{
    private readonly ServiceBusClient _serviceBusClient;

    public MultiDestinationAndClientWithSdk(IConfiguration configuration)
    {
        _serviceBusClient = new ServiceBusClient(configuration["UserReportGenServiceBus_NameSpace_Conn_Str"]);
    }


    [Function("MultiDestinationAndClientWithSdk")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        var msg = "from-" + nameof(MultiDestinationAndClientWithSdk) + "::::::" + req.Query["val"];
        var sender1 = _serviceBusClient.CreateSender("user-report-gen-queue");
        var sender2 = _serviceBusClient.CreateSender("user-audit-queue");
        await sender1.SendMessageAsync(new ServiceBusMessage(msg));
        await sender2.SendMessageAsync(new ServiceBusMessage(msg));
        return new OkObjectResult(msg);
    }
}