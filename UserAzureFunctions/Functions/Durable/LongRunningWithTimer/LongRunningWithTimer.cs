using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;

namespace UserAzureFunctions.Functions.Durable.LongRunningWithTimer
{
    public class LongRunningWithTimer
    {
        [Function(nameof(LongRunningWithTimer))]
        public static async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "api/v2/user-on-board")] HttpRequestData requestData,
            [DurableClient] DurableTaskClient client)
        {
            var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(UserOnboarderV2), requestData.Query["userName"]);
            return await client.CreateCheckStatusResponseAsync(requestData, instanceId);
        }

        [Function(nameof(UserOnboarderV2))]
        public static async Task<Tuple<string?, string>> UserOnboarderV2([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            var userName = context.GetInput<string>();
            var dateTime = context.CurrentUtcDateTime.AddSeconds(60);
            var timerTask = context.CreateTimer(dateTime, CancellationToken.None);
            var kycStatusTask = context.WaitForExternalEvent<bool>("KycCompleted");

            var winner = await Task.WhenAny(kycStatusTask, timerTask);

            bool kycStatus = false;

            if (winner == kycStatusTask)
            {
                kycStatus = await kycStatusTask;
            }

            return Tuple.Create(userName,
                kycStatus ?
                    "Account Creation Completed" :
                    "KYC is pending");
        }

    }
}
