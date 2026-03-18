using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;

namespace UserAzureFunctions.Functions.Durable.MultiOrchestrator
{
    public class MultiOrchestratorAzFunction
    {

        //Note - orchestrationContext is created for each orchestartor
        [Function(nameof(MultiOrchestratorAzFunction))]
        public static async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "api/v1/multi-orch/loan-approval/{userId}")] HttpRequestData requestData,
            [DurableClient] DurableTaskClient client,
            FunctionContext functionContext,
            string userId)
        {
            var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(ActivityManagerForMultiOrch), Tuple.Create(userId, int.Parse(requestData.Query["amt"] ?? "0")));
            return await client.CreateCheckStatusResponseAsync(requestData, instanceId);
        }

        [Function(nameof(ActivityManagerForMultiOrch))]
        public static async Task<Tuple<int, bool, string>> ActivityManagerForMultiOrch([OrchestrationTrigger] TaskOrchestrationContext orchestrationContext,
            FunctionContext functionContext)
        {
            var userIdAndReqAmount = orchestrationContext.GetInput<Tuple<string, int>>();
            var userId = userIdAndReqAmount!.Item1;
            var reqAmount = userIdAndReqAmount!.Item2;
            var creditLimit = await orchestrationContext.CallSubOrchestratorAsync<int>(nameof(ActivityManagerForCreditLimitByUserId), userId);
            var isAllowed = await orchestrationContext.CallSubOrchestratorAsync<bool>(nameof(ActivityManagerForIsCreditAllowed), Tuple.Create(userId, creditLimit, reqAmount));
            var approvedBy = await orchestrationContext.CallSubOrchestratorAsync<string>(nameof(ActivityManagerForCreditApprover), Tuple.Create(reqAmount, isAllowed));
            return Tuple.Create(creditLimit, isAllowed, approvedBy);
        }

        [Function(nameof(ActivityManagerForCreditLimitByUserId))]
        public static async Task<int> ActivityManagerForCreditLimitByUserId([OrchestrationTrigger] TaskOrchestrationContext orchestrationContext,
           FunctionContext functionContext)
        {
            var userId = orchestrationContext.GetInput<string>();
            var creditLimit = await orchestrationContext.CallActivityAsync<int>(nameof(GetCreditLimitByUserIdV2), userId);
            return creditLimit;
        }

        [Function(nameof(ActivityManagerForIsCreditAllowed))]
        public static async Task<bool> ActivityManagerForIsCreditAllowed([OrchestrationTrigger] TaskOrchestrationContext orchestrationContext,
            FunctionContext functionContext)
        {
            var userIdAndReqAmountAndCreditLimit = orchestrationContext.GetInput<Tuple<string, int, int>>();
            var userId = userIdAndReqAmountAndCreditLimit!.Item1;
            var reqAmount = userIdAndReqAmountAndCreditLimit!.Item2;
            var creditLimit = userIdAndReqAmountAndCreditLimit!.Item3;
            var isAllowed = await orchestrationContext.CallActivityAsync<bool>(nameof(IsCreditAllowedV2), Tuple.Create(userId, creditLimit, reqAmount));
            return isAllowed;
        }

        [Function(nameof(ActivityManagerForCreditApprover))]
        public static async Task<string> ActivityManagerForCreditApprover([OrchestrationTrigger] TaskOrchestrationContext orchestrationContext,
            FunctionContext functionContext)
        {
            var reqAmountAndIsAllowed = orchestrationContext.GetInput<Tuple<int, bool>>();
            int reqAmount = reqAmountAndIsAllowed!.Item1;
            bool isAllowed = reqAmountAndIsAllowed.Item2;
            var approvedBy = isAllowed ? await orchestrationContext.CallActivityAsync<string>(nameof(CreditApproverV2), reqAmount) : "NA";
            return approvedBy;
        }

        #region ActivityFunctions

        [Function(nameof(GetCreditLimitByUserIdV2))]
        public static Task<int> GetCreditLimitByUserIdV2([ActivityTrigger] string userId)
        {
            return userId switch
            {
                "messi" => Task.FromResult(1000),
                "ronaldo" => Task.FromResult(500),
                "abhijith" => Task.FromResult(2000),
                _ => Task.FromResult(200)
            };
        }



        [Function(nameof(IsCreditAllowedV2))]
        public static Task<bool> IsCreditAllowedV2([ActivityTrigger] Tuple<string, int, int> userIdAndLimitAndReqAmount)
        {
            var userId = userIdAndLimitAndReqAmount.Item1;
            var limit = userIdAndLimitAndReqAmount.Item2;
            var requestAmount = userIdAndLimitAndReqAmount.Item3;
            return Task.FromResult(requestAmount < limit && userId.Length > 2);
        }

        [Function(nameof(CreditApproverV2))]
        public static Task<string> CreditApproverV2([ActivityTrigger] int amount)
        {
            return Task.FromResult(amount > 1000 ? "RBI Regional Manager" : "SBI Regional Manager");
        }

        #endregion
    }
}
