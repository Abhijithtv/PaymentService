using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;

namespace UserAzureFunctions.Functions.Durable.ExternalEvent
{
    public class UserOnboardingFn
    {
        [Function(nameof(UserOnboardingFn))]
        public static async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "api/v1/user-on-board")] HttpRequestData requestData,
            [DurableClient] DurableTaskClient client)
        {
            var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(UserOnboarder), requestData.Query["userName"]);
            return await client.CreateCheckStatusResponseAsync(requestData, instanceId);
        }

        [Function(nameof(UserOnboarder))]
        public static async Task<Tuple<string?, string, int, string>> UserOnboarder([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            var userName = context.GetInput<string>();
            var banKIDAndBalance = await context.CallActivityAsync<Tuple<string, int>>(nameof(CreateUserAccount), userName);
            var kycStatus = await context.WaitForExternalEvent<bool>("KycCompleted");

            return Tuple.Create(userName,
                banKIDAndBalance.Item1,
                banKIDAndBalance.Item2,
                kycStatus ?
                    "Account Creation Completed" :
                    "KYC is pending");
        }

        [Function(nameof(CreateUserAccount))]
        public static Tuple<string, int> CreateUserAccount([ActivityTrigger] string userName)
        {
            return Tuple.Create("id-01-" + userName, 1000);
        }

        [Function(nameof(DoKyc))]
        public static HttpResponseData DoKyc([HttpTrigger(AuthorizationLevel.Function, "get", Route = "api/v1/kyc/{userId}")] HttpRequestData requestData,
            [DurableClient] DurableTaskClient client
            , string userId)
        {
            var instanceId = requestData.Query["instanceId"];
            client.RaiseEventAsync(instanceId!, "KycCompleted", true);
            return requestData.CreateResponse(System.Net.HttpStatusCode.OK);
        }
    }
}
