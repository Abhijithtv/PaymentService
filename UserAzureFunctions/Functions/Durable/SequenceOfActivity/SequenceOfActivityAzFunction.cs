using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;

namespace UserAzureFunctions.Functions.Durable.SequenceOfActivity
{
    public class SequenceOfActivityAzFunction
    {
        [Function(nameof(SequenceOfActivityAzFunction))]
        public static async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "api/v1/loan-approval/{userId}")] HttpRequestData requestData,
            [DurableClient] DurableTaskClient client,
            FunctionContext functionContext,
            string userId)
        {
            var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(ActivityManager), Tuple.Create(userId, int.Parse(requestData.Query["amt"] ?? "0")));
            return await client.CreateCheckStatusResponseAsync(requestData, instanceId);
        }

        [Function(nameof(ActivityManager))]
        public static async Task<Tuple<int, bool, string>> ActivityManager([OrchestrationTrigger] TaskOrchestrationContext orchestrationContext,
            FunctionContext functionContext)
        {
            var userIdAndReqAmount = orchestrationContext.GetInput<Tuple<string, int>>();
            var userId = userIdAndReqAmount!.Item1;
            var reqAmount = userIdAndReqAmount!.Item2;
            var creditLimit = await orchestrationContext.CallActivityAsync<int>(nameof(GetCreditLimitByUserId), userId);
            var isAllowed = await orchestrationContext.CallActivityAsync<bool>(nameof(IsCreditAllowed), Tuple.Create(userId, creditLimit, reqAmount));
            var approvedBy = isAllowed ? await orchestrationContext.CallActivityAsync<string>(nameof(CreditApprover), reqAmount) : "NA";
            return Tuple.Create(creditLimit, isAllowed, approvedBy);
        }

        [Function(nameof(GetCreditLimitByUserId))]
        public static Task<int> GetCreditLimitByUserId([ActivityTrigger] string userId)
        {
            return userId switch
            {
                "messi" => Task.FromResult(1000),
                "ronaldo" => Task.FromResult(500),
                "abhijith" => Task.FromResult(2000),
                _ => Task.FromResult(200)
            };
        }

        [Function(nameof(IsCreditAllowed))]
        public static Task<bool> IsCreditAllowed([ActivityTrigger] Tuple<string, int, int> userIdAndLimitAndReqAmount)
        {
            var userId = userIdAndLimitAndReqAmount.Item1;
            var limit = userIdAndLimitAndReqAmount.Item2;
            var requestAmount = userIdAndLimitAndReqAmount.Item3;
            return Task.FromResult(requestAmount < limit && userId.Length > 2);
        }

        [Function(nameof(CreditApprover))]
        public static Task<string> CreditApprover([ActivityTrigger] int amount)
        {
            return Task.FromResult(amount > 1000 ? "RBI Regional Manager" : "SBI Regional Manager");
        }
    }
}
