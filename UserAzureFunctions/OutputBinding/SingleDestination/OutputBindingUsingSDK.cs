using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;

namespace UserAzureFunctions.OutputBinding.SingleDestination;

public class OutputBindingUsingSDK
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly IConfiguration _configuration;
    public OutputBindingUsingSDK(IConfiguration configuration)
    {
        _configuration = configuration;
        _serviceBusClient = new ServiceBusClient(configuration["UserReportGenServiceBus_NameSpace_Conn_Str"]);
    }


    [Function("OutputBindingUsingSDK")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        var msg = "got it from sdk::::::" + req.Query["val"];
        var sender = _serviceBusClient.CreateSender("user-report-gen-queue");
        await sender.SendMessageAsync(new ServiceBusMessage(msg));
        return new OkObjectResult("Welcome to Azure Functions via sdk!");
    }
}