using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Entities;

namespace UserAzureFunctions.Functions.Durable.EntityTrigger
{
    public class EnitityTriggerBySharding
    {
        [Function(nameof(EnitityTriggerBySharding))]
        public static async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "api/v1/durable-entity-trigger/{action}")]
    HttpRequestData requestData,
        [DurableClient] DurableTaskClient client,
        FunctionContext context,
        string action)
        {
            var userId = requestData.Query["userId"]!;

            var entityId = new EntityInstanceId(nameof(UserEntity), userId);

            switch (action)
            {
                case "create":
                    await client.Entities.SignalEntityAsync(entityId, "Create", userId);
                    break;
                case "add":
                    await client.Entities.SignalEntityAsync(entityId, "Add", 100);
                    break;
            }

            var res = requestData.CreateResponse(System.Net.HttpStatusCode.OK);

            if (action.Equals("get"))
            {
                var stateResponse = await client.Entities.GetEntityAsync<UserEntity>(entityId);

                if (stateResponse != null)
                {
                    var entity = stateResponse!.State;
                    await res.WriteStringAsync(System.Text.Json.JsonSerializer.Serialize(entity));
                }
                else
                {
                    await res.WriteStringAsync("Entity is yet to be created or is not present");
                }

            }
            else
            {
                await res.WriteStringAsync("No State info Present");
            }
            return res;
        }
    }

    public class UserEntity
    {
        //note - only public properites's state are stored
        public string Name { get; set; }
        public int Balance { get; set; }

        public void Add(int val)
        {
            Balance += val;
        }

        public void Create(string name)
        {
            Name = name;
            Balance = 0;
        }

        [Function(nameof(UserEntity))]
        public async Task Run([EntityTrigger] TaskEntityDispatcher dispatcher)
        {
            await dispatcher.DispatchAsync<UserEntity>();
        }
    }
}


