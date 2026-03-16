using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using UserAzureFunctions.Functions.Durable.Models;

namespace UserAzureFunctions.Functions.Durable.SingleActivity;

public static class SingleActivityAzFunction
{
    //entry point
    [Function(nameof(SingleActivityAzFunction))]
    public static async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "api/v1/durable/singleActivity")]
    HttpRequestData requestData,
        [DurableClient] DurableTaskClient client,
        FunctionContext context)
    {
        var instanceId = await client.ScheduleNewOrchestrationInstanceAsync("Master", int.Parse(requestData.Query["id"] ?? "-1"));

        return await client.CreateCheckStatusResponseAsync(requestData, instanceId);
    }

    [Function("Master")]
    //OrchestrationTrigger make it this orchestartion controller
    public static async Task Orchestartor([OrchestrationTrigger] TaskOrchestrationContext orchContext
        , FunctionContext functionContext)
    {
        Console.WriteLine("Hi...I got invoked");
        var res = await orchContext.CallActivityAsync<int>(nameof(GetAge), new AgeEntity
        {
            UserId = orchContext.GetInput<int>(),
            SeqId = 1
        });
        Console.WriteLine(res);
    }


    [Function(nameof(GetAge))]
    public static int GetAge([ActivityTrigger] AgeEntity ageEntity)
    {
        return ageEntity.UserId % 2 == 0 ? -1 : 1;
    }
}