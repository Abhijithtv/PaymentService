using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Entities;

namespace UserAzureFunctions.Functions.Durable.EntityTrigger
{
    public class EnitityTriggerWithoutSharding
    {
        [Function(nameof(EnitityTriggerWithoutSharding))]
        public static async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "api/v1/durabl/entity-trigger-without-shard/{collectionId}/{action}")]
        HttpRequestData requestData,
        [DurableClient] DurableTaskClient client,
        FunctionContext context,
        string collectionId,
        string action)
        {
            var userId = requestData.Query["userId"]!;

            var entityId = new EntityInstanceId(nameof(UserEntityList), collectionId);

            switch (action)
            {
                case "create":
                    await client.Entities.SignalEntityAsync(entityId, "Create", userId);
                    break;
                case "add":
                    await client.Entities.SignalEntityAsync(entityId, "Add", Tuple.Create(userId, 200));
                    break;
            }

            var res = requestData.CreateResponse(System.Net.HttpStatusCode.OK);

            if (action.Equals("get"))
            {
                var stateResponse = await client.Entities.GetEntityAsync<UserEntityList>(entityId);

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

        [Function(nameof(UserEntityList))]
        public async Task EntityRun([EntityTrigger] TaskEntityDispatcher dispatcher)
        {
            await dispatcher.DispatchAsync<UserEntityList>();
        }
    }

    public class UserEntityList
    {
        public List<UserEntity> UserEntityLists { get; set; }
        public UserEntityList()
        {
            UserEntityLists = new List<UserEntity>();
        }

        public void Add(Tuple<string, int> userNameAndval)
        {
            UserEntityLists.FirstOrDefault(x => x.Name == userNameAndval.Item1)
                ?.Add(userNameAndval.Item2);
        }

        public void Create(string name)
        {
            if (UserEntityLists.Any(x => x.Name == name))
            {
                return;
            }
            UserEntityLists.Add(new UserEntity { Name = name, Balance = 0 });
        }
    }
}
