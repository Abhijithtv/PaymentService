using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Entities;

namespace UserAzureFunctions.Functions.Durable.EntityTrigger
{
    public class EntityTriggeredAzFunctionWithOrchestartion
    {
        [Function(nameof(EntityTriggeredAzFunctionWithOrchestartion))]
        public static async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "api/v1/durable/entity-trigger-with-orch/{action}")] HttpRequestData requestData,
            [DurableClient] DurableTaskClient client,
            string action)
        {
            var userId = requestData.Query["userId"];
            var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(Manager), Tuple.Create(action, userId));
            return await client.CreateCheckStatusResponseAsync(requestData, instanceId);
        }

        [Function(nameof(Manager))]
        public static async Task<UserEntityV2> Manager([OrchestrationTrigger] TaskOrchestrationContext orchestrationContext)
        {
            var actionAndUser = orchestrationContext.GetInput<Tuple<string, string>>();
            var entityId = new EntityInstanceId(nameof(UserEntityV2), actionAndUser!.Item2);
            if (actionAndUser.Item1 == "create")
            {
                var res = await orchestrationContext.Entities
                    .CallEntityAsync<UserEntityV2>(entityId, "Create", actionAndUser.Item2);
                return res;
            }
            else
            {
                var res = await orchestrationContext.Entities
                    .CallEntityAsync<UserEntityV2>(entityId, "Add", 500);
                return res;
            }
        }
    }

    public class UserEntityV2
    {
        //note - only public properites's state are stored
        public string Name { get; set; }
        public int Balance { get; set; }

        public UserEntityV2 Add(int val)
        {
            Balance += val;
            return this;
        }

        public UserEntityV2 Create(string name)
        {
            Name = name;
            Balance = 0;
            return this;
        }

        [Function(nameof(UserEntityV2))]
        public async Task Run([EntityTrigger] TaskEntityDispatcher dispatcher)
        {
            await dispatcher.DispatchAsync<UserEntityV2>();
        }
    }
}
