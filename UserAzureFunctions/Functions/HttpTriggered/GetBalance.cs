using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace UserAzureFunctions.Functions.HttpTriggered;

public class GetBalance
{
    private readonly ILogger<GetBalance> _logger;
    private readonly Random random;
    public GetBalance(ILogger<GetBalance> logger)
    {
        _logger = logger;
        random = new Random();
    }

    [Function("GetBalance")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "api/v1/balance")] HttpRequest req)
    {
        var userId = req.Query["id"];
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult($"Welcome to Azure Functions! userId = {userId} and balance = {random.Next(0, 1000)}");
    }
}