using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;

namespace UserAzureFunctions.Functions.Durable.ParallelActivity
{
    public class ParallelAzFunction
    {
        [Function(nameof(ParallelAzFunction))]
        public static async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "api/v1/sendMail")] HttpRequestData requestData,
            [DurableClient] DurableTaskClient client,
            FunctionContext functionContext)
        {
            var emails = await requestData.ReadFromJsonAsync<string[]>();
            var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(Runnner), emails);
            return await client.CreateCheckStatusResponseAsync(requestData, instanceId);
        }

        [Function(nameof(Runnner))]
        public static async Task<Tuple<string, bool>[]> Runnner([OrchestrationTrigger] TaskOrchestrationContext orchestrationContext)
        {
            var emails = orchestrationContext.GetInput<string[]>();

            var tasks = new List<Task<Tuple<string, bool>>>();
            foreach (var email in emails)
            {
                tasks.Add(orchestrationContext.CallActivityAsync<Tuple<string, bool>>(nameof(SendToPerson), email));
                tasks.Add(orchestrationContext.CallActivityAsync<Tuple<string, bool>>(nameof(SendToPersonViaGmail), email));
                tasks.Add(orchestrationContext.CallActivityAsync<Tuple<string, bool>>(nameof(SendToPersonViaOutlook), email));
            }
            var res = await Task.WhenAll(tasks);
            return res;
        }

        [Function(nameof(SendToPerson))]
        public static async Task<Tuple<string, bool>> SendToPerson([ActivityTrigger] string email)
        {
            await Task.Delay(1000); //mocking
            return Tuple.Create($"SendToPerson-{email}", true);
        }

        [Function(nameof(SendToPersonViaOutlook))]
        public static async Task<Tuple<string, bool>> SendToPersonViaOutlook([ActivityTrigger] string email)
        {
            await Task.Delay(1000); //mocking
            return Tuple.Create($"SendToPersonViaOutlook-{email}", true);
        }

        [Function(nameof(SendToPersonViaGmail))]
        public static async Task<Tuple<string, bool>> SendToPersonViaGmail([ActivityTrigger] string email)
        {
            await Task.Delay(1000); //mocking
            return Tuple.Create($"SendToPersonViaGmail-{email}", true);
        }

    }
}
